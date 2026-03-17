import type { CategoryCreateDTO } from "../types/CategoryCreateDTO";
import type { CategoryDTO } from "../types/CategoryDTO";
import { getAuthHeaders } from "./authHeaders";
import { fetchJson, friendlyStatusError } from "./apiHelpers";

const API_BASE: string = import.meta.env.VITE_API_BASE_URL as string;

export async function getCategories(): Promise<CategoryDTO[]> {
  const { data: result, ok, status } = await fetchJson<CategoryDTO[] & { message?: string }>(
    `${API_BASE}/api/Category`
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}

export async function createCategory(data: CategoryCreateDTO) {
  const { data: result, ok, status } = await fetchJson<CategoryDTO & { message?: string }>(
    `${API_BASE}/api/Category`,
    {
      method: "POST",
      headers: getAuthHeaders(),
      body: JSON.stringify(data),
    }
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}

export async function updateCategory(id: number, data: CategoryCreateDTO) {
  const { data: result, ok, status } = await fetchJson<{ message?: string }>(
    `${API_BASE}/api/Category/${String(id)}`,
    {
      method: "PUT",
      headers: getAuthHeaders(),
      body: JSON.stringify(data),
    }
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}

export async function deleteCategory(id: number) {
  const { data: result, ok, status } = await fetchJson<{ message?: string }>(
    `${API_BASE}/api/Category/${String(id)}`,
    {
      method: "DELETE",
      headers: getAuthHeaders(),
    }
  );

  if (!ok) {
    throw new Error(result.message ?? friendlyStatusError(status));
  }

  return result;
}
