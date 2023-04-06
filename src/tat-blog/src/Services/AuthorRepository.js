import axios from 'axios';

export async function getBestAuthor(limit = 5) {
  try {
    const response = await axios.get(`https://localhost:7029/api/authors/best/${limit}`);
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}
