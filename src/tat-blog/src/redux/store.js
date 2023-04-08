import { configureStore } from '@reduxjs/toolkit';
import { reducer } from './reducer';

const store = configureStore({ reducer: { postFilter: reducer } });

export default store;
