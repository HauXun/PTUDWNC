import axios from 'axios';

export async function postSubscriber(subscribeEmail) {
  try {
    const response = await axios.post(`https://localhost:7029/api/subscribers`, {
      subscribeEmail: subscribeEmail,
      cancelReason: '',
      forceLock: false,
      unsubscribeVoluntary: false,
      adminNotes: '',
    });
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}
