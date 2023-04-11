import { createSlice } from '@reduxjs/toolkit';

const initialState = {
  keyword: '',
  userName: '',
  postTitle: '',
  year: '',
  month: '',
  day: '',
  censored: true,
};

export const commentFilterSlice = createSlice({
  name: 'commentFilter',
  initialState,
  reducers: {
    reset: (state, action) => {
      return initialState;
    },
    updateKeyword: (state, action) => {
      state.keyword = action.payload;
    },
    updateUserName: (state, action) => {
      state.userName = action.payload;
    },
    updatePostTitle: (state, action) => {
      state.postTitle = action.payload;
    },
    updateDay: (state, action) => {
      state.day = action.payload;
    },
    updateMonth: (state, action) => {
      state.month = action.payload;
    },
    updateYear: (state, action) => {
      state.year = action.payload;
    },
    updateCensored: (state, action) => {
      state.censored = action.payload;
    },
  },
});

export const {
  reset,
  updateKeyword,
  updateUserName,
  updatePostTitle,
  updateDay,
  updateMonth,
  updateYear,
  updateCensored,
} = commentFilterSlice.actions;
