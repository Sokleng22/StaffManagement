import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:5009/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const staffApi = {
  // Get all staff with pagination and filtering
  getAll: async (params = {}) => {
    const response = await api.get('/staff', { params });
    return response.data;
  },

  // Get staff by ID
  getById: async (id) => {
    const response = await api.get(`/staff/${id}`);
    return response.data;
  },

  // Create new staff
  create: async (staff) => {
    const response = await api.post('/staff', staff);
    return response.data;
  },

  // Update existing staff
  update: async (id, staff) => {
    const response = await api.put(`/staff/${id}`, staff);
    return response.data;
  },

  // Delete staff
  delete: async (id) => {
    await api.delete(`/staff/${id}`);
  },

  // Search staff
  search: async (searchTerm) => {
    const response = await api.get('/staff/search', { params: { searchTerm } });
    return response.data;
  },

  // Export to CSV
  exportToCsv: async () => {
    const response = await api.get('/staff/export/csv', { responseType: 'blob' });
    return response.data;
  },

  // Export to Excel
  exportToExcel: async () => {
    const response = await api.get('/staff/export/excel', { responseType: 'blob' });
    return response.data;
  },

  // Get departments
  getDepartments: async () => {
    const response = await api.get('/staff/departments');
    return response.data;
  },

  // Get positions
  getPositions: async () => {
    const response = await api.get('/staff/positions');
    return response.data;
  },
};

export default staffApi;
