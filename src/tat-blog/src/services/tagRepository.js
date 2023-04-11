import { delete_api, get_api, post_api } from './method';

export async function getTags(
  keyword = '',
  pageSize = 10,
  pageNumber = 1,
  name = '',
  sortColumn = '',
  sortOrder = ''
) {
  let url = new URL('https://localhost:7029/api/tags');
  keyword !== '' && url.searchParams.append('Keyword', keyword);
  name !== '' && url.searchParams.append('Name', name);
  sortColumn !== '' && url.searchParams.append('SortColumn', sortColumn);
  sortOrder !== '' && url.searchParams.append('SortOrder', sortOrder);
  url.searchParams.append('PageSize', pageSize);
  url.searchParams.append('PageNumber', pageNumber);

  return get_api(url.href);
}

export async function getTagById(id = 0) {
  if (id > 0) return get_api(`https://localhost:7029/api/tags/${id}`);
  return null;
}

export function addOrUpdateTag(formData) {
  return post_api('https://localhost:7029/api/tags', formData);
}

export async function deleteTagById(id) {
  if (id > 0) return delete_api(`https://localhost:7029/api/tags/${id}`);
  return null;
}
