import { get_api } from './method';

export async function getTotalPosts() {
  return get_api(`https://localhost:7029/api/dashboard/total/posts`);
}

export async function getTotalUnpublishedPosts() {
  return get_api(`https://localhost:7029/api/dashboard/total/posts/unpublished`);
}

export async function getTotalCategories() {
  return get_api(`https://localhost:7029/api/dashboard/total/categories`);
}

export async function getTotalAuthor() {
  return get_api(`https://localhost:7029/api/dashboard/total/authors`);
}

export async function getTotalWaitingComments() {
  return get_api(`https://localhost:7029/api/dashboard/total/comments/waiting`);
}

export async function getTotalSubscribers() {
  return get_api(`https://localhost:7029/api/dashboard/total/subscribers`);
}

export async function getTotalNewestSubscriber() {
  return get_api(`https://localhost:7029/api/dashboard/total/subscribers/newest`);
}