import { createSlice } from '@reduxjs/toolkit';

const initialState = {
  keyword: '',
  email: '',
  forceLock: false,
  unsubscribeVoluntary: false,
};

export const subscriberFilterSlice = createSlice({
  name: 'commentFilter',
  initialState,
  reducers: {
    reset: (state, action) => {
      return initialState;
    },
    updateKeyword: (state, action) => {
      state.keyword = action.payload;
    },
    updateEmail: (state, action) => {
      state.userName = action.payload;
    },
    updateForceLock: (state, action) => {
      state.forceLock = action.payload;
    },
    updateUnsubscribeVoluntary: (state, action) => {
      state.unsubscribeVoluntary = action.payload;
    },
  },
});

export const {
  reset,
  updateKeyword,
  updateEmail,
  updateForceLock,
  updateUnsubscribeVoluntary,
} = subscriberFilterSlice.actions;
