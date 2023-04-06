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

export async function getPost(
  year = 2023,
  month = 1,
  day = 1,
  slug = '',
) {
  try {
    const response = await axios.get(
      `https://localhost:7029/api/posts?PageSize=1&PageNumber=1&PublishedOnly=true&NotPublished=false&Year=${year}&Month=${month}&Day=${day}&PostSlug=${slug}`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}

export async function getPostBySlug(slug = '') {
  try {
    const response = await axios.get(
      `https://localhost:7029/api/posts/byslug/${slug}?PageSize=10&PageNumber=1`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}

export async function getPostByCategorySlug(slug = '', pageSize = 10, pageNumber = 1) {
  try {
    const response = await axios.get(
      `https://localhost:7029/api/posts?PublishedOnly=true&NotPublished=false&CategorySlug=${slug}&PageSize=${pageSize}&PageNumber=${pageNumber}`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}

export async function getPostByAuthorSlug(slug = '', pageSize = 10, pageNumber = 1) {
  try {
    const response = await axios.get(
      `https://localhost:7029/api/posts?PublishedOnly=true&NotPublished=false&AuthorSlug=${slug}&PageSize=${pageSize}&PageNumber=${pageNumber}`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}

export async function getPostByTagSlug(slug = '', pageSize = 10, pageNumber = 1) {
  try {
    const response = await axios.get(
      `https://localhost:7029/api/posts?PublishedOnly=true&NotPublished=false&TagSlug=${slug}&PageSize=${pageSize}&PageNumber=${pageNumber}`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}

export async function getPostByArchives(year = 2023, month = 1, pageSize = 10, pageNumber = 1) {
  try {
    const response = await axios.get(
      `https://localhost:7029/api/posts?PublishedOnly=true&NotPublished=false&Year=${year}&Month=${month}&PageSize=${pageSize}&PageNumber=${pageNumber}`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}
