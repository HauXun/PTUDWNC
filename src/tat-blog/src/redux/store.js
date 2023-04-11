import { configureStore } from '@reduxjs/toolkit';
import { postFilterSlice } from '../pages/Admin/Post/postFilterSlice';
import { categoryFilterSlice } from '../pages/Admin/Category/categoryFilterSlice';
import { authorFilterSlice } from '../pages/Admin/Author/authorFilterSlice';
import { tagFilterSlice } from '../pages/Admin/Tag/tagFilterSlice';
import { commentFilterSlice } from '../pages/Admin/Comment/commentFilterSlice';
import { subscriberFilterSlice } from '../pages/Admin/Subscriber/subscriberFilterSlice';

const store = configureStore({
  reducer: {
    postFilter: postFilterSlice.reducer,
    categoryFilter: categoryFilterSlice.reducer,
    authorFilter: authorFilterSlice.reducer,
    tagFilter: tagFilterSlice.reducer,
    commentFilter: commentFilterSlice.reducer,
    subscriberFilter: subscriberFilterSlice.reducer,
  },
});

export default store;
