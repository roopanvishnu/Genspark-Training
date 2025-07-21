
export interface User {
  id: string;
  username: string;
  email: string;
  role: string;
  createdAt: string;
}

export interface CreateUserDto {
  username: string;
  email: string;
  password: string;
  role: string;
}

export interface UpdateUserDto {
  username?: string;
  email?: string;
  role?: string;
}
