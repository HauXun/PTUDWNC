import { useEffect, useState } from 'react';
import Card from 'react-bootstrap/Card';
import { Link } from 'react-router-dom';
import { getFeaturedPost } from '../../Services/Widgets';

const FeaturedWidget = () => {
  const [postsList, setPostsList] = useState([]);

  useEffect(() => {
    getFeaturedPost().then((data) => {
      if (data) setPostsList(data);
      else setPostsList([]);
    });
  }, []);

  return (
    <div className="mb-4">
      <h3 className="text-success mb-2">Top 3 bài viết được xem nhiều nhất</h3>
      {postsList.length > 0 &&
        postsList.map((item, index) => {
          return (
            <Card.Body key={index}>
              <Card.Title>
                <Link
                  to={`/blog/post?slug=${item.urlSlug}`}
                  title={item.title}
                  style={{ textDecoration: 'none' }}
                >
                  {item.title}
                </Link>
              </Card.Title>
            </Card.Body>
          );
        })}
      <hr />
    </div>
  );
};

export default FeaturedWidget;
