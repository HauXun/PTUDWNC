import { createSlice } from '@reduxjs/toolkit';

const initialState = {
  keyword: '',
  authorId: '',
  categoryId: '',
  year: '',
  month: '',
  published: true,
};

export const postFilterSlice = createSlice({
  name: 'postFilter',
  initialState,
  reducers: {
    reset: (state, action) => {
      return initialState;
    },
    updateKeyword: (state, action) => {
      state.keyword = action.payload;
    },
    updateAuthorId: (state, action) => {
      state.authorId = action.payload;
    },
    updateCategoryId: (state, action) => {
      state.categoryId = action.payload;
    },
    updateMonth: (state, action) => {
      state.month = action.payload;
    },
    updateYear: (state, action) => {
      state.year = action.payload;
    },
    updatePublished: (state, action) => {
      state.published = action.payload;
    },
  },
});

export const {
  reset,
  updateKeyword,
  updateAuthorId,
  updateCategoryId,
  updateMonth,
  updateYear,
  updatePublished,
} = postFilterSlice.actions;
