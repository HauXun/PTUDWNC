import { get_api, post_api } from './method';

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
