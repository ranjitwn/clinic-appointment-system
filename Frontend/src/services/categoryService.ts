import type { CategoryCreateDTO } from "../types/CategoryCreateDTO";
import type { CategoryDTO } from "../types/CategoryDTO";
import { getAuthHeaders } from "./authHeaders";

const API_BASE: string = import.meta.env.VITE_API_BASE_URL as string;

export async function getCategories(): Promise<CategoryDTO[]> {
  const response = await fetch(`${API_BASE}/api/Category`);

  const result = (await response.json()) as CategoryDTO[] & {
    message?: string;
  };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to fetch categories");
  }

  return result;
}



export async function createCategory(data: CategoryCreateDTO) {
  const response = await fetch(`${API_BASE}/api/Category`, {
    method: "POST",
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

  const result = (await response.json()) as CategoryDTO & {
    message?: string;
  };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to create category");
  }

  return result;
}

export async function updateCategory(id: number, data: CategoryCreateDTO) {
  const response = await fetch(`${API_BASE}/api/Category/${String(id)}`, {
    method: "PUT",
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

  const result = (await response.json()) as { message?: string };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to update category");
  }

  return result;
}


export async function deleteCategory(id: number) {
  const response = await fetch(`${API_BASE}/api/Category/${String(id)}`, {
    method: "DELETE",
    headers: getAuthHeaders(),
  });

  const result = (await response.json()) as { message?: string };

  if (!response.ok) {
    throw new Error(result.message ?? "Failed to delete category");
  }

  return result;
}