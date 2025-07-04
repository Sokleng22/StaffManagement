// Staff API service for managing staff data
const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:5009';

export interface Staff {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  department: string;
  position: string;
  salary: number;
  hireDate: string;
  isActive: boolean;
}

export interface CreateStaffDto {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  department: string;
  position: string;
  salary: number;
  hireDate: string;
  isActive: boolean;
}

export class StaffApiService {
  private async request<T>(endpoint: string, options?: RequestInit): Promise<T> {
    const url = `${API_BASE_URL}/api/staff${endpoint}`;
    const response = await fetch(url, {
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers,
      },
      ...options,
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    return response.json();
  }

  async getAllStaff(): Promise<Staff[]> {
    return this.request<Staff[]>('');
  }

  async getStaffById(id: number): Promise<Staff> {
    return this.request<Staff>(`/${id}`);
  }

  async createStaff(staff: CreateStaffDto): Promise<Staff> {
    return this.request<Staff>('', {
      method: 'POST',
      body: JSON.stringify(staff),
    });
  }

  async updateStaff(id: number, staff: CreateStaffDto): Promise<Staff> {
    return this.request<Staff>(`/${id}`, {
      method: 'PUT',
      body: JSON.stringify(staff),
    });
  }

  async deleteStaff(id: number): Promise<void> {
    await this.request<void>(`/${id}`, {
      method: 'DELETE',
    });
  }
}

export const staffApiService = new StaffApiService();