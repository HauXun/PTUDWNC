import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { useDispatch, useSelector } from 'react-redux';
import { Link } from 'react-router-dom';
import {
  reset,
  updateKeyword,
  updateFullName,
  updateEmail
} from './authorFilterSlice';

const AuthorFilterPane = () => {
  const authorFilter = useSelector((state) => state.authorFilter);
  const dispatch = useDispatch();

  const handleReset = (e) => {
    dispatch(reset());
  };

  const handleSubmit = (e) => {
    e.preventDefault();
  };

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
          value={authorFilter.keyword}
          onChange={(e) => dispatch(updateKeyword(e.target.value))}
        />
      </Form.Group>
      <Form.Group className="col-auto">
        <Form.Label className="visually-hidden"> Tên tác giả </Form.Label>
        <Form.Control
          type="text"
          placeholder="Nhập tên tác giả..."
          name="fullName"
          value={authorFilter.fullName}
          onChange={(e) => dispatch(updateFullName(e.target.value))}
        />
      </Form.Group>
      <Form.Group className="col-auto">
        <Form.Label className="visually-hidden"> Email </Form.Label>
        <Form.Control
          type="email"
          placeholder="Nhập email..."
          name="email"
          value={authorFilter.email}
          onChange={(e) => dispatch(updateEmail(e.target.value))}
        />
      </Form.Group>
      <Form.Group className="col-auto">
        <Button variant="danger" type="reset">
          Xóa lọc
        </Button>
        <Link to="/admin/authors/edit" className="btn btn-success ms-2">
          Thêm mới
        </Link>
      </Form.Group>
    </Form>
  );
};

export default AuthorFilterPane;
