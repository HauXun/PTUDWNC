import { post_api, get_api } from './method';

export async function getSubscribers(
  keyword = '',
  pageSize = 10,
  pageNumber = 1,
  email = '',
  forceLock = false,
  unsubscribeVoluntary  = false,
  sortColumn = '',
  sortOrder = ''
) {
  let url = new URL('https://localhost:7029/api/subscribers');
  keyword !== '' && url.searchParams.append('Keyword', keyword);
  email !== '' && url.searchParams.append('Email', email);
  sortColumn !== '' && url.searchParams.append('SortColumn', sortColumn);
  sortOrder !== '' && url.searchParams.append('SortOrder', sortOrder);
  url.searchParams.append('PageSize', pageSize);
  url.searchParams.append('PageNumber', pageNumber);
  url.searchParams.append('ForceLock', forceLock);
  url.searchParams.append('UnsubscribeVoluntary', unsubscribeVoluntary);

  return get_api(url.href);
}

export async function postSubscriber(subscribeEmail) {
  return post_api(`https://localhost:7029/api/subscribers`, {
    subscribeEmail: subscribeEmail,
    cancelReason: '',
    forceLock: false,
    unsubscribeVoluntary: false,
    adminNotes: '',
  });
}

export async function blockSubscriber(id, payload) {
  return post_api(`https://localhost:7029/api/subscribers/${id}`, payload);
}
