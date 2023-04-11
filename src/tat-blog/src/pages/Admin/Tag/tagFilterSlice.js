import { createSlice } from '@reduxjs/toolkit';

const initialState = {
  keyword: '',
  name: '',
};

export const tagFilterSlice = createSlice({
  name: 'tagFilter',
  initialState,
  reducers: {
    reset: (state, action) => {
      return initialState;
    },
    updateKeyword: (state, action) => {
      state.keyword = action.payload;
    },
    updateName: (state, action) => {
      state.name = action.payload;
    },
  },
});

export const {
  reset,
  updateKeyword,
  updateName,
} = tagFilterSlice.actions;
