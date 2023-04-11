import { createSlice } from '@reduxjs/toolkit';

const initialState = {
  keyword: '',
  showOnMenu: true,
};

export const categoryFilterSlice = createSlice({
  name: 'categoryFilter',
  initialState,
  reducers: {
    reset: (state, action) => {
      return initialState;
    },
    updateKeyword: (state, action) => {
      state.keyword = action.payload;
    },
    updateShowOn: (state, action) => {
      state.showOnMenu = action.payload;
    },
  },
});

export const {
  reset,
  updateKeyword,
  updateShowOn,
} = categoryFilterSlice.actions;
