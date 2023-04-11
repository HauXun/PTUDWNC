import { get_api, post_api } from './method';

export async function getComments(
  keyword = '',
  pageSize = 10,
  pageNumber = 1,
  userName = '',
  postTitle = '',
  year = '',
  month = '',
  day = '',
  censored,
  sortColumn = '',
  sortOrder = ''
) {
  let url = new URL('https://localhost:7029/api/comments');
  keyword !== '' && url.searchParams.append('Keyword', keyword);
  userName !== '' && url.searchParams.append('FullName', userName);
  postTitle !== '' && url.searchParams.append('Email', postTitle);
  year !== '' && url.searchParams.append('Email', year);
  month !== '' && url.searchParams.append('Email', month);
  day !== '' && url.searchParams.append('Email', day);
  sortColumn !== '' && url.searchParams.append('SortColumn', sortColumn);
  sortOrder !== '' && url.searchParams.append('SortOrder', sortOrder);
  url.searchParams.append('PageSize', pageSize);
  url.searchParams.append('PageNumber', pageNumber);
  url.searchParams.append('Censored', censored);

  return get_api(url.href);
}

export async function getCommentsByPost(postId, pageSize = 10, pageNumber = 1) {
  return get_api(
    `https://localhost:7029/api/posts/${postId}/comments?PageSize=${pageSize}&PageNumber=${pageNumber}`
  );
}

export async function postCommentsByPost(postId, userName, content) {
  return post_api(`https://localhost:7029/api/comments`, {
    userName: userName,
    content: content,
    postID: postId,
    censored: true,
    postDate: new Date().toISOString(),
  });
}

export async function getFilter() {
  return get_api(`https://localhost:7029/api/comments/get-filter`);
}

export async function switchCommentCensored(id) {
  if (id > 0) return post_api(`https://localhost:7029/api/comments/toggle/${id}`);
  return null;
}

export async function deleteCommentById(id) {
  if (id > 0) return delete_api(`https://localhost:7029/api/comments/${id}`);
  return null;
}
