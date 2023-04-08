import { useEffect, useState } from 'react';
import Card from 'react-bootstrap/Card';
import { Link } from 'react-router-dom';
import { getTagCloud } from '../../services/widgets';
import TagList from '../TagList';

const TagCloudWidget = () => {
  const [tagsList, setTagsList] = useState([]);

  useEffect(() => {
    getTagCloud().then((data) => {
      if (data) setTagsList(data);
      else setTagsList([]);
    });
  }, []);

  return (
    <div className="mb-4">
      <h3 className="text-success mb-2">Các thẻ tag cloud</h3>
      {tagsList.length > 0 && (
        <div className="tag-list">
          <TagList tagList={tagsList} />
        </div>
      )}
    </div>
  );
};

export default TagCloudWidget;
