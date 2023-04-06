import { useEffect, useState } from 'react';
import ListGroup from 'react-bootstrap/ListGroup';
import { Link } from 'react-router-dom';
import { getArchivesPost } from '../../Services/Widgets';

const ArchivesWidget = () => {
  const [datesList, setDatesList] = useState([]);

  useEffect(() => {
    getArchivesPost().then((data) => {
      if (data) setDatesList(data);
      else setDatesList([]);
    });
  }, []);

  return (
    <div className="mb-4">
      <h3 className="text-success mb-2">
        Danh sách 12 tháng gần nhất và số lượng bài viết tương ứng
      </h3>
      {datesList.length > 0 && (
        <ListGroup>
          {datesList.map((date, index) => {
            return (
              <ListGroup.Item key={index} className="text-primary">
                <Link
                  to={`/blog/archives?year=${date.year}&month=${date.month}`}
                  style={{ textDecoration: 'none' }}
                  title={date.monthName}
                >
                  {`${date.monthName} ${date.year}`}
                  <span>&nbsp;({date.postCount})</span>
                </Link>
              </ListGroup.Item>
            );
          })}
        </ListGroup>
      )}
    </div>
  );
};

export default ArchivesWidget;
