import { createSlice } from '@reduxjs/toolkit';

const initialState = {
  keyword: '',
  fullName: '',
  email: '',
};

export const authorFilterSlice = createSlice({
  name: 'authorFilter',
  initialState,
  reducers: {
    reset: (state, action) => {
      return initialState;
    },
    updateKeyword: (state, action) => {
      state.keyword = action.payload;
    },
    updateFullName: (state, action) => {
      state.fullName = action.payload;
    },
    updateEmail: (state, action) => {
      state.email = action.payload;
    },
  },
});

export const {
  reset,
  updateKeyword,
  updateFullName,
  updateEmail
} = authorFilterSlice.actions;
