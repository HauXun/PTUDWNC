import { delete_api, get_api, post_api } from './method';

export async function getCategories(
  keyword = '',
  pageSize = 10,
  pageNumber = 1,
  showOnMenu,
  sortColumn = '',
  sortOrder = ''
) {
  let url = new URL('https://localhost:7029/api/categories');
  keyword !== '' && url.searchParams.append('Keyword', keyword);
  sortColumn !== '' && url.searchParams.append('SortColumn', sortColumn);
  sortOrder !== '' && url.searchParams.append('SortOrder', sortOrder);
  url.searchParams.append('PageSize', pageSize);
  url.searchParams.append('PageNumber', pageNumber);
  url.searchParams.append('ShowOnMenu', showOnMenu);

  return get_api(url.href);
}

export async function getCategoryById(id = 0) {
  if (id > 0) return get_api(`https://localhost:7029/api/categories/${id}`);
  return null;
}

export function addOrUpdateCategory(formData) {
  return post_api('https://localhost:7029/api/categories', formData);
}

export async function switchShowOnMenu(id) {
  if (id > 0) return post_api(`https://localhost:7029/api/categories/show/switch/${id}`);
  return null;
}

export async function deleteCategoryById(id) {
  if (id > 0) return delete_api(`https://localhost:7029/api/categories/${id}`);
  return null;
}
