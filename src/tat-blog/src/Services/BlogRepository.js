import axios from 'axios';

export async function getPosts(
  keyword = '',
  pageSize = 10,
  pageNumber = 1,
  sortColumn = '',
  sortOrder = '',
  publishedOnly = true
) {
  try {
    const response = await axios.get(
      `https://localhost:7029/api/posts?keyword=${keyword}&PageSize=${pageSize}&PageNumber=${pageNumber}&SortColumn=${sortColumn}&SortOrder=${sortOrder}&PublishedOnly=${publishedOnly}&NotPublished=${!publishedOnly}`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}
