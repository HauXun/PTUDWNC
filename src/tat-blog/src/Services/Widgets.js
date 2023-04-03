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
