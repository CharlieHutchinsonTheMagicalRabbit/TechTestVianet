export interface ManufacturerDto {
  id: string;
  name: string;
  hqCity: string;
  hqCountry: string;
  foundedYear: number;
  latitude: number;
  longitude: number;
  consoleCount: number;
}

export interface ManufacturerDetailDto {
  id: string;
  name: string;
  hqCity: string;
  hqCountry: string;
  foundedYear: number;
  latitude: number;
  longitude: number;
  consoles: ConsoleDto[];
}

export interface CreateManufacturerRequest {
  name: string;
  hqCity: string;
  hqCountry: string;
  foundedYear: number;
  latitude: number;
  longitude: number;
}

export interface UpdateManufacturerRequest {
  name: string;
  hqCity: string;
  hqCountry: string;
  foundedYear: number;
  latitude: number;
  longitude: number;
}

export interface ConsoleDto {
  id: string;
  manufacturerId: string;
  manufacturerName: string;
  name: string;
  releaseYear: number;
  generation: number;
  unitsSoldInMillions: number;
  isDiscontinued: boolean;
}

export interface CreateConsoleRequest {
  manufacturerId: string;
  name: string;
  releaseYear: number;
  generation: number;
  unitsSoldInMillions: number;
  isDiscontinued: boolean;
}

export interface UpdateConsoleRequest {
  name: string;
  releaseYear: number;
  generation: number;
  unitsSoldInMillions: number;
  isDiscontinued: boolean;
}

export interface ConsoleSearchQuery {
  searchTerm?: string;
  manufacturerId?: string;
  generation?: number;
  isDiscontinued?: boolean;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface ManufacturerMapLocationDto {
  id: string;
  name: string;
  locationString: string;
  latitude: number;
  longitude: number;
  consoleCount: number;
}

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
}

export interface ToastMessage {
  id: string;
  type: 'success' | 'error' | 'info' | 'warning';
  title: string;
  message?: string;
  timeout?: number;
}
