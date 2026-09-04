import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, OnDestroy } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { ToastService } from '../../services/toast.service';
import { EmployeeList } from '../../models/employee.model';
import { Subject, debounceTime, switchMap, takeUntil } from 'rxjs';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './employee-list.component.html',
  styleUrl: './employee-list.component.css'
})
export class EmployeeListComponent implements OnInit, OnDestroy {

  private api = inject(ApiService);
  private toast = inject(ToastService);
  private filterChanged$ = new Subject<void>();
  private destroy$ = new Subject<void>();

  employees: EmployeeList[] = [];

  pageNumber = 1;
  pageSize = 5;
  totalPages = 0;
  totalRecords = 0;
  nameFilter = '';
  mobileFilter = '';
  loading = false;
  showDeleteModal = false;
  employeeToDeleteId: number | null = null;
  employeeToDeleteName = '';

  ngOnInit(): void {
    this.load();
    this.filterChanged$
      .pipe(
        debounceTime(300),
        switchMap(() => {
          this.pageNumber = 1;
          this.loading = true;
          return this.api.getEmployees(
            this.pageNumber,
            this.pageSize,
            this.nameFilter,
            this.mobileFilter
          );
        }),
        takeUntil(this.destroy$)
      )
      .subscribe({
        next: result => {
          this.employees = result.items;
          this.totalPages = result.totalPages;
          this.totalRecords = result.totalRecords;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.toast.show('Unable to load employees.');
        }
      });
  }

  load(): void {
    this.loading = true;
    this.api.getEmployees(
      this.pageNumber,
      this.pageSize,
      this.nameFilter,
      this.mobileFilter
    )
      .subscribe({
        next: result => {
          this.employees = result.items;
          this.totalPages = result.totalPages;
          this.totalRecords = result.totalRecords;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.toast.show('Unable to load employees.');
        }
      });
  }

  onFilterChange(): void {
    this.filterChanged$.next();
  }

  search(): void {
    this.pageNumber = 1;
    this.load();
  }

  clearFilters(): void {
    this.nameFilter = '';
    this.mobileFilter = '';
    this.search();
  }

  previous(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.load();
    }
  }

  next(): void {
    if (this.pageNumber < this.totalPages) {
      this.pageNumber++;
      this.load();
    }
  }

  delete(id: number, name: string): void {
  this.employeeToDeleteId = id;
  this.employeeToDeleteName = name;
  this.showDeleteModal = true;
}

  cancelDelete(): void {
  this.showDeleteModal = false;
  this.employeeToDeleteId = null;
  this.employeeToDeleteName = '';
}

confirmDelete(): void {

  if (this.employeeToDeleteId === null) return;

  const id = this.employeeToDeleteId;

  this.api.deleteEmployee(id).subscribe({
    next: () => {

      this.showDeleteModal = false;
      this.employeeToDeleteId = null;
      this.employeeToDeleteName = '';

      this.toast.show('Employee deleted successfully.');

      if (this.pageNumber > 1 && this.employees.length === 1) {
        this.pageNumber--;
      }

      this.load();
    },

    error: () => {
      this.toast.show('Unable to delete employee.');
    }
  });
}

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}