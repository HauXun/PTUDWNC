import { useEffect } from 'react';

const Rss = () => {
  useEffect(() => {
    document.title = 'Rss Feed';
  }, []);
  return <h1>Đây là trang Rss Feed</h1>;
};

export default Rss;
