import { delete_api, get_api, post_api } from './method';

export async function getPosts(
  keyword = '',
  pageSize = 10,
  pageNumber = 1,
  authorId = '',
  categoryId = '',
  year = '',
  month = '',
  publishedOnly = true,
  sortColumn = '',
  sortOrder = ''
) {
  let url = new URL('https://localhost:7029/api/posts');
  keyword !== '' && url.searchParams.append('Keyword', keyword);
  authorId !== '' && url.searchParams.append('AuthorId', authorId);
  categoryId !== '' && url.searchParams.append('CategoryId', categoryId);
  year !== '' && url.searchParams.append('Year', year);
  month !== '' && url.searchParams.append('Month', month);
  sortColumn !== '' && url.searchParams.append('SortColumn', sortColumn);
  sortOrder !== '' && url.searchParams.append('SortOrder', sortOrder);
  url.searchParams.append('PageSize', pageSize);
  url.searchParams.append('PageNumber', pageNumber);
  url.searchParams.append('PublishedOnly', publishedOnly);
  url.searchParams.append('NotPublished', !publishedOnly);

  return get_api(url.href);
}

export async function getPost(year = 2023, month = 1, day = 1, slug = '') {
  return get_api(
    `https://localhost:7029/api/posts?PageSize=1&PageNumber=1&PublishedOnly=true&NotPublished=false&Year=${year}&Month=${month}&Day=${day}&PostSlug=${slug}`
  );
}

export async function getFilter() {
  return get_api(`https://localhost:7029/api/posts/get-filter`);
}

export async function getPostBySlug(slug = '') {
  return get_api(`https://localhost:7029/api/posts/byslug/${slug}?PageSize=10&PageNumber=1`);
}

export async function getPostByCategorySlug(slug = '', pageSize = 10, pageNumber = 1) {
  return get_api(
    `https://localhost:7029/api/posts?PublishedOnly=true&NotPublished=false&CategorySlug=${slug}&PageSize=${pageSize}&PageNumber=${pageNumber}`
  );
}

export async function getPostByAuthorSlug(slug = '', pageSize = 10, pageNumber = 1) {
  return get_api(
    `https://localhost:7029/api/posts?PublishedOnly=true&NotPublished=false&AuthorSlug=${slug}&PageSize=${pageSize}&PageNumber=${pageNumber}`
  );
}

export async function getPostByTagSlug(slug = '', pageSize = 10, pageNumber = 1) {
  return get_api(
    `https://localhost:7029/api/posts?PublishedOnly=true&NotPublished=false&TagSlug=${slug}&PageSize=${pageSize}&PageNumber=${pageNumber}`
  );
}

export async function getPostByArchives(year = 2023, month = 1, pageSize = 10, pageNumber = 1) {
  return get_api(
    `https://localhost:7029/api/posts?PublishedOnly=true&NotPublished=false&Year=${year}&Month=${month}&PageSize=${pageSize}&PageNumber=${pageNumber}`
  );
}

export async function getPostById(id = 0) {
  if (id > 0) return get_api(`https://localhost:7029/api/posts/${id}`);
  return null;
}

export function addOrUpdatePost(formData) {
  return post_api('https://localhost:7029/api/posts', formData);
}

export async function switchPostPublished(id) {
  if (id > 0) return post_api(`https://localhost:7029/api/posts/published/switch/${id}`);
  return null;
}

export async function deletePostById(id) {
  if (id > 0) return delete_api(`https://localhost:7029/api/posts/${id}`);
  return null;
}
