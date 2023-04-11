import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { useDispatch, useSelector } from 'react-redux';
import { Link } from 'react-router-dom';
import {
  reset,
  updateKeyword,
  updateShowOn,
} from './categoryFilterSlice';

const CategoryFilterPane = () => {
  const categoryFilter = useSelector((state) => state.categoryFilter);
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
          value={categoryFilter.keyword}
          onChange={(e) => dispatch(updateKeyword(e.target.value))}
        />
      </Form.Group>
      <Form.Group className="col-auto">
        <Form.Label className="form-check-label"> Hiển thị </Form.Label>
        <Form.Control
          className="form-check-input"
          type="checkbox"
          name="showOnMenu"
          checked={categoryFilter.showOnMenu}
          title="Show On Menu"
          onChange={(e) => dispatch(updateShowOn(e.target.checked))}
        />
      </Form.Group>
      <Form.Group className="col-auto">
        <Button variant="danger" type="reset">
          Xóa lọc
        </Button>
        <Link to="/admin/categories/edit" className="btn btn-success ms-2">
          Thêm mới
        </Link>
      </Form.Group>
    </Form>
  );
};

export default CategoryFilterPane;
