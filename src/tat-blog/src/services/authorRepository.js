import { get_api } from './method';

export async function getBestAuthor(limit = 5) {
  return get_api(`https://localhost:7029/api/authors/best/${limit}`);
}
