import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { useDispatch, useSelector } from 'react-redux';
import { Link } from 'react-router-dom';
import {
  reset,
  updateKeyword,
  updateUserName,
  updatePostTitle,
  updateDay,
  updateMonth,
  updateYear,
  updateCensored,
} from './commentFilterSlice';
import { getFilter } from '../../../services/commentRepository';
import { useEffect, useState } from 'react';

const CommentFilterPane = () => {
  const commentFilter = useSelector((state) => state.commentFilter);
  const dispatch = useDispatch();
  const [filter, setFilter] = useState({ monthList: [] });

  const handleReset = (e) => {
    dispatch(reset());
  };

  const handleSubmit = (e) => {
    e.preventDefault();
  };

  useEffect(() => {
    getFilter().then((data) => {
      if (data) {
        setFilter({
          monthList: data.monthList,
        });
      } else {
        setFilter({ monthList: [] });
      }
    });
  }, []);

  return (
    <Form
      method="get"
      onReset={handleReset}
      onSubmit={handleSubmit}
      className="row gy-2 gx-3 align-items-center p-2"
    >
      <Form.Group className="col-auto">
        <Form.Label className="visually-hidden"> Keyword </Form.Label>
        <Form.Control
          type="text"
          placeholder="Nhập từ khóa..."
          name="keyword"
          value={commentFilter.keyword}
          onChange={(e) => dispatch(updateKeyword(e.target.value))}
        />
      </Form.Group>
      <Form.Group className="col-auto">
        <Form.Label className="visually-hidden"> Tên người bình luận </Form.Label>
        <Form.Control
          type="text"
          placeholder="Nhập tên người bình luận..."
          name="userName"
          value={commentFilter.userName}
          onChange={(e) => dispatch(updateUserName(e.target.value))}
        />
      </Form.Group>
      <Form.Group className="col-auto">
        <Form.Label className="visually-hidden"> Tên bài viết </Form.Label>
        <Form.Control
          type="text"
          placeholder="Nhập tên bài viết..."
          name="postTitle"
          value={commentFilter.postTitle}
          onChange={(e) => dispatch(updatePostTitle(e.target.value))}
        />
      </Form.Group>
      <Form.Group className="col-auto">
        <Form.Label className="visually-hidden"> Year </Form.Label>
        <Form.Control
          type="number"
          placeholder="Nhập năm..."
          name="year"
          value={commentFilter.year}
          max={commentFilter.year}
          onChange={(e) => dispatch(updateYear(e.target.value))}
        />
      </Form.Group>
      <Form.Group className="col-auto">
        <Form.Label className="visually-hidden"> Month </Form.Label>
        <Form.Select
          name="month"
          value={commentFilter.month}
          onChange={(e) => dispatch(updateMonth(e.target.value))}
          title="Month"
        >
          <option value="">-- Chọn tháng --</option>
          {filter.monthList.length > 0 &&
            filter.monthList.map((item, index) => (
              <option key={index} value={item.value}>
                {item.text}
              </option>
            ))}
        </Form.Select>
      </Form.Group>
      <Form.Group className="col-auto">
        <Form.Label className="visually-hidden"> Day </Form.Label>
        <Form.Control
          type="number"
          placeholder="Nhập ngày..."
          name="day"
          value={commentFilter.day}
          max={commentFilter.day}
          onChange={(e) => dispatch(updateDay(e.target.value))}
        />
      </Form.Group>
      <Form.Group className="col-auto">
        <Form.Label className="form-check-label"> Đã kiểm duyệt </Form.Label>
        <Form.Control
          className="form-check-input"
          type="checkbox"
          name="censored"
          checked={commentFilter.censored}
          title="Censored"
          onChange={(e) => dispatch(updateCensored(e.target.checked))}
        />
      </Form.Group>
      <Form.Group className="col-auto">
        <Button variant="danger" type="reset">
          Xóa lọc
        </Button>
        <Link to="/admin/comments/edit" className="btn btn-success ms-2">
          Thêm mới
        </Link>
      </Form.Group>
    </Form>
  );
};

export default CommentFilterPane;
