import { post_api } from './method';

export async function postSubscriber(subscribeEmail) {
  return post_api(`https://localhost:7029/api/subscribers`, {
    subscribeEmail: subscribeEmail,
    cancelReason: '',
    forceLock: false,
    unsubscribeVoluntary: false,
    adminNotes: '',
  });
}
