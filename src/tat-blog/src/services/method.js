import axios from 'axios';

export async function get_api(api) {
  try {
    const response = await axios.get(api);
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}

export async function post_api(api, payload) {
  try {
    const response = await axios.post(api, payload);
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}

export async function delete_api(api, payload) {
  try {
    const response = await axios.delete(api, payload);
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}
