import { get_api } from './method';

export async function getCategories(pageSize = 100, pageNumber = 1, showOnMenu = true) {
  return get_api(
    `https://localhost:7029/api/categories?ShowOnMenu=${showOnMenu}&PageSize=${pageSize}&PageNumber=${pageNumber}`
  );
}

export async function getFeaturedPost(limit = 5) {
  return get_api(`https://localhost:7029/api/posts/featured/${limit}`);
}

export async function getRandomPost(limit = 5) {
  return get_api(`https://localhost:7029/api/posts/random/${limit}`);
}

export async function getArchivesPost(limit = 12) {
  return get_api(`https://localhost:7029/api/posts/archives/${limit}`);
}

export async function getTagCloud(limit = 12) {
  return get_api(`https://localhost:7029/api/tags/all`);
}

export async function getBestAuthor(limit = 12) {
  return get_api(`https://localhost:7029/api/authors/best/${limit}`);
}
