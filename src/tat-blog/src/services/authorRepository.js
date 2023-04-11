import { delete_api, get_api, post_api } from './method';

export async function getBestAuthor(limit = 5) {
  return get_api(`https://localhost:7029/api/authors/best/${limit}`);
}

export async function getAuthors(
  keyword = '',
  pageSize = 10,
  pageNumber = 1,
  fullName = '',
  email = '',
  sortColumn = '',
  sortOrder = ''
) {
  let url = new URL('https://localhost:7029/api/authors');
  keyword !== '' && url.searchParams.append('Keyword', keyword);
  fullName !== '' && url.searchParams.append('FullName', fullName);
  email !== '' && url.searchParams.append('Email', email);
  sortColumn !== '' && url.searchParams.append('SortColumn', sortColumn);
  sortOrder !== '' && url.searchParams.append('SortOrder', sortOrder);
  url.searchParams.append('PageSize', pageSize);
  url.searchParams.append('PageNumber', pageNumber);

  return get_api(url.href);
}

export async function getAuthorById(id = 0) {
  if (id > 0) return get_api(`https://localhost:7029/api/authors/${id}`);
  return null;
}

export function addOrUpdateAuthor(formData) {
  return post_api('https://localhost:7029/api/authors', formData);
}

export async function deleteAuthorById(id) {
  if (id > 0) return delete_api(`https://localhost:7029/api/authors/${id}`);
  return null;
}
