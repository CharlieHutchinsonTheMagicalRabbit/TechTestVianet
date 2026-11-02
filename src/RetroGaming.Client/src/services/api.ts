import axios, { AxiosError } from 'axios';
import type {
  ManufacturerDto,
  ManufacturerDetailDto,
  CreateManufacturerRequest,
  UpdateManufacturerRequest,
  ConsoleDto,
  CreateConsoleRequest,
  UpdateConsoleRequest,
  ConsoleSearchQuery,
  ManufacturerMapLocationDto,
  ProblemDetails
} from '../types';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json'
  }
});

export function getErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const axiosError = error as AxiosError<ProblemDetails>;
    if (axiosError.response?.data) {
      const data = axiosError.response.data;
      if (data.detail) {
        return data.detail;
      }
      if (data.title) {
        return data.title;
      }
      if (data.errors && typeof data.errors === 'object') {
        const firstKey = Object.keys(data.errors)[0];
        if (firstKey && data.errors[firstKey]?.length) {
          return `${firstKey}: ${data.errors[firstKey][0]}`;
        }
      }
    }
    if (axiosError.message) {
      return axiosError.message;
    }
  } else if (error instanceof Error) {
    return error.message;
  }
  return 'An unexpected error occurred.';
}

export const manufacturersApi = {
  async getAll(): Promise<ManufacturerDto[]> {
    const response = await api.get<ManufacturerDto[]>('/manufacturers');
    return response.data;
  },

  async getById(id: string): Promise<ManufacturerDetailDto> {
    const response = await api.get<ManufacturerDetailDto>(`/manufacturers/${id}`);
    return response.data;
  },

  async create(data: CreateManufacturerRequest): Promise<ManufacturerDto> {
    const response = await api.post<ManufacturerDto>('/manufacturers', data);
    return response.data;
  },

  async update(id: string, data: UpdateManufacturerRequest): Promise<ManufacturerDto> {
    const response = await api.put<ManufacturerDto>(`/manufacturers/${id}`, data);
    return response.data;
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/manufacturers/${id}`);
  }
};

export const consolesApi = {
  async getAll(query?: ConsoleSearchQuery): Promise<ConsoleDto[]> {
    const params = new URLSearchParams();
    if (query?.searchTerm) params.append('searchTerm', query.searchTerm);
    if (query?.manufacturerId) params.append('manufacturerId', query.manufacturerId);
    if (query?.generation !== undefined && query.generation !== null) params.append('generation', query.generation.toString());
    if (query?.isDiscontinued !== undefined && query.isDiscontinued !== null) params.append('isDiscontinued', query.isDiscontinued.toString());
    if (query?.sortBy) params.append('sortBy', query.sortBy);
    if (query?.sortDescending !== undefined) params.append('sortDescending', query.sortDescending.toString());

    const response = await api.get<ConsoleDto[]>('/consoles', { params });
    return response.data;
  },

  async getById(id: string): Promise<ConsoleDto> {
    const response = await api.get<ConsoleDto>(`/consoles/${id}`);
    return response.data;
  },

  async create(data: CreateConsoleRequest): Promise<ConsoleDto> {
    const response = await api.post<ConsoleDto>('/consoles', data);
    return response.data;
  },

  async update(id: string, data: UpdateConsoleRequest): Promise<ConsoleDto> {
    const response = await api.put<ConsoleDto>(`/consoles/${id}`, data);
    return response.data;
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/consoles/${id}`);
  }
};

export const mapApi = {
  async getLocations(): Promise<ManufacturerMapLocationDto[]> {
    const response = await api.get<ManufacturerMapLocationDto[]>('/map/locations');
    return response.data;
  }
};

export default api;
