import axios from 'axios';

export async function getCategories(pageSize = 100, pageNumber = 1, showOnMenu = true) {
  try {
    const response = await axios.get(
      `https://localhost:7029/api/categories?ShowOnMenu=${showOnMenu}&PageSize=${pageSize}&PageNumber=${pageNumber}`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}

export async function getFeaturedPost(limit = 5) {
  try {
    const response = await axios.get(
      `https://localhost:7029/api/posts/featured/${limit}`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}

export async function getRandomPost(limit = 5) {
  try {
    const response = await axios.get(
      `https://localhost:7029/api/posts/random/${limit}`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}

export async function getArchivesPost(limit = 12) {
  try {
    const response = await axios.get(
      `https://localhost:7029/api/posts/archives/${limit}`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}

export async function getTagCloud(limit = 12) {
  try {
    const response = await axios.get(
      `https://localhost:7029/api/tags/all`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}

export async function getBestAuthor(limit = 12) {
  try {
    const response = await axios.get(
      `https://localhost:7029/api/authors/best/${limit}`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}