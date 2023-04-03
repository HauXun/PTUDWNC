import { Link } from 'react-router-dom';

const TagList = ({ tagList }) => {
  if (tagList && Array.isArray(tagList) && tagList.length > 0)
    return (
      <>
        {tagList.map((item, index) => {
          return (
            <Link
              to={`/blog/tag?slug=${item}`}
              title={item}
              className="btn  btn-sm  btn-outline-secondary me-1"
              key={index}
            >
              {item}
            </Link>
          );
        })}
      </>
    );
  else return <></>;
};

export default TagList;
