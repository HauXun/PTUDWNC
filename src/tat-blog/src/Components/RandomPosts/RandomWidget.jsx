import { useEffect, useState } from 'react';
import { getRandomPost } from '../../Services/Widgets';
import PostItem from '../PostItem';

const RandomWidget = () => {
  const [postsList, setPostsList] = useState([]);

  useEffect(() => {
    getRandomPost().then((data) => {
      if (data) setPostsList(data);
      else setPostsList([]);
    });
  }, []);

  return (
    <div className="mb-4">
      <h3 className="text-success mb-2">Top 5 bài viết ngẫu nhiên</h3>
      {postsList.length > 0 &&
        postsList.map((item, index) => {
          return <PostItem postItem={item} key={index} />;
        })}
      <hr />
    </div>
  );
};

export default RandomWidget;
