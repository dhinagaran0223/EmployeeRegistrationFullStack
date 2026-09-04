export interface EmployeeList {
  employeeId: number;
  employeeName: string;
  age: number;
  mobileNum: string;
}

export interface EmployeeDetail extends EmployeeList {
  pincode: string;
  dob: string | null;
  addressLine1: string;
  addressLine2: string | null;
  stateId: number;
  stateName: string;
  countryId: number;
  countryName: string;
}

export interface EmployeePayload {
  employeeName: string;
  age: number;
  mobileNum: string;
  pincode: string;
  dob: string | null;
  addressLine1: string;
  addressLine2: string | null;
  stateId: number;
  countryId: number;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
}

export interface Country {
  countryId: number;
  countryName: string;
}

export interface State {
  stateId: number;
  stateName: string;
  countryId: number;
}
