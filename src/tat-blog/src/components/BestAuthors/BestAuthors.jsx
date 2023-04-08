import { useEffect, useState } from 'react';
import Card from 'react-bootstrap/Card';
import Row from 'react-bootstrap/Row';
import { getBestAuthor } from '../../services/authorRepository';
import { isEmptyOrSpaces } from '../../utils/utils';
import { Link } from 'react-router-dom';

const BestAuthor = () => {
  const [authorsList, setAuthorsList] = useState([]);

  useEffect(() => {
    getBestAuthor().then((data) => {
      if (data) setAuthorsList(data);
      else setAuthorsList([]);
    });
  }, []);

  return (
    <div className="mb-4">
      <h3 className="text-success mb-2">Top 5 tác giả có nhiều bài viết nhất</h3>
      {authorsList.length > 0 &&
        authorsList.map((item, index) => {
          let imageUrl = isEmptyOrSpaces(item.imageUrl)
            ? import.meta.env.VITE_PUBLIC_URL + '/images/image_1.jpg'
            : `${item.imageUrl}`;

          return (
            <Card key={index} className="mt-3 p-2">
              <Row className="g-0">
                <Card.Title>
                  <Link
                    to={`/blog/author?slug=${item.urlSlug}`}
                    style={{ textDecoration: 'none' }}
                    title={item.fullName}
                  >
                    {item.fullName}
                  </Link>
                </Card.Title>
                <Card.Img variant="top" src={imageUrl} alt="default" className="rounded w-50" />
                <p>
                  Ngày bắt đầu làm việc: <i>{item.joinedDate}</i>
                </p>
                <h5>Email liên hệ:</h5>
                {item.email}
              </Row>
            </Card>
          );
        })}
      <hr />
    </div>
  );
};

export default BestAuthor;
