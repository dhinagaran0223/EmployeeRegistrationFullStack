import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import {
  AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { ToastService } from '../../services/toast.service';
import { Country, EmployeePayload, State } from '../../models/employee.model';

function forbiddenAddressChars(control: AbstractControl): ValidationErrors | null {
  return /[$%!+]/.test(control.value ?? '') ? { forbiddenChars: true } : null;
}

function alphabetsOnly(control: AbstractControl): ValidationErrors | null {
  return /^[A-Za-z ]+$/.test((control.value ?? '').trim()) ? null : { alphabetsOnly: true };
}

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './employee-form.component.html',
  styleUrl: './employee-form.component.css'
})
export class EmployeeFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private api = inject(ApiService);
  private toast = inject(ToastService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  employeeId: number | null = null;
  isEdit = false;
  countries: Country[] = [];
  states: State[] = [];
  loading = false;
  maxDob = '';

  form = this.fb.nonNullable.group({
    employeeName: ['', [Validators.required, Validators.maxLength(30), alphabetsOnly]],
    age: [0, [Validators.required, Validators.min(1), Validators.max(999)]],
    mobileNum: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
    dob: [''],
    addressLine1: ['', [Validators.required, Validators.maxLength(250), forbiddenAddressChars]],
    addressLine2: ['', [Validators.maxLength(250), forbiddenAddressChars]],
    pincode: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]],
    countryId: [0, [Validators.required, Validators.min(1)]],
    stateId: [0, [Validators.required, Validators.min(1)]]
  });

  ngOnInit(): void {
    this.maxDob = new Date().toISOString().slice(0, 10);
    this.api.getCountries().subscribe(c => this.countries = c);

    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.employeeId = id;
      this.isEdit = true;
      this.loadEmployee(id);
    }

    this.form.controls.dob.valueChanges.subscribe(dob => {
      if (dob) {
        const age = this.calculateAge(new Date(`${dob}T00:00:00`));
        this.form.controls.age.setValue(age > 0 ? age : 0, { emitEvent: false });
      }
    });

    this.form.controls.countryId.valueChanges.subscribe(countryId => {
      if (countryId > 0) {
        this.api.getStatesByCountry(countryId).subscribe(states => {
          this.states = states;
          const currentState = this.form.controls.stateId.value;
          if (!states.some(s => s.stateId === currentState)) {
            this.form.controls.stateId.setValue(0, { emitEvent: false });
          }
        });
      } else {
        this.states = [];
        this.form.controls.stateId.setValue(0, { emitEvent: false });
      }
    });

    this.form.controls.stateId.valueChanges.subscribe(stateId => {
      if (stateId > 0 && !this.form.controls.countryId.value) {
        this.api.getCountryByState(stateId).subscribe(country => {
          this.form.controls.countryId.setValue(country.countryId);
        });
      }
    });
  }

  loadEmployee(id: number): void {
    this.loading = true;
    this.api.getEmployee(id).subscribe({
      next: e => {
        this.form.patchValue({
          employeeName: e.employeeName,
          age: e.age,
          mobileNum: e.mobileNum,
          dob: e.dob ? e.dob.slice(0, 10) : '',
          addressLine1: e.addressLine1,
          addressLine2: e.addressLine2 ?? '',
          pincode: e.pincode,
          countryId: e.countryId,
          stateId: e.stateId
        });
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toast.show('Employee not found.');
        this.router.navigate(['/employees']);
      }
    });
  }

  calculateAge(dob: Date): number {
    const today = new Date();
    let age = today.getFullYear() - dob.getFullYear();
    const month = today.getMonth() - dob.getMonth();
    if (month < 0 || (month === 0 && today.getDate() < dob.getDate())) age--;
    return age;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.toast.show('Please correct the validation errors.');
      return;
    }

    const raw = this.form.getRawValue();
    const payload: EmployeePayload = {
      employeeName: raw.employeeName.trim(),
      age: raw.age,
      mobileNum: raw.mobileNum,
      dob: raw.dob || null,
      addressLine1: raw.addressLine1.trim(),
      addressLine2: raw.addressLine2.trim() || null,
      pincode: raw.pincode,
      countryId: raw.countryId,
      stateId: raw.stateId
    };

    this.loading = true;
    const request = this.isEdit && this.employeeId
      ? this.api.updateEmployee(this.employeeId, payload)
      : this.api.createEmployee(payload);

    request.subscribe({
      next: () => {
        this.loading = false;
        this.toast.show(this.isEdit ? 'Employee updated successfully.' : 'Employee registered successfully.');
        this.router.navigate(['/employees']);
      },
      error: err => {
        this.loading = false;
        this.toast.show(err?.error?.message ?? 'Unable to save employee.');
      }
    });
  }

  hasError(name: string, error: string): boolean {
    const c = this.form.get(name);
    return !!c && c.touched && c.hasError(error);
  }
}
