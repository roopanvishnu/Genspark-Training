export interface UserDto {
  id: string;
  username: string;
  email: string;
  role: 'Employee' | 'HR';
  gender: 'Male' | 'Female' | 'Other';
  isActive: boolean;
  createdAt: string;
}

export interface CreateUserDto {
  username: string;
  email: string;
  password: string;
  role: 'Employee' | 'HR';
  gender: 'Male' | 'Female' | 'Other';
  isActive: boolean;
}

export interface UpdateUserDto {
  username: string;
  email: string;
  role: 'Employee' | 'HR';
  gender: 'Male' | 'Female' | 'Other';
  isActive: boolean;
}
