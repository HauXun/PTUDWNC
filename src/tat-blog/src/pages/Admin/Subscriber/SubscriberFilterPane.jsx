import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { useDispatch, useSelector } from 'react-redux';
import { Link } from 'react-router-dom';
import {
  reset,
  updateKeyword,
  updateEmail,
  updateForceLock,
  updateUnsubscribeVoluntary
} from './subscriberFilterSlice';

const SubscriberFilterPane = () => {
  const subscriberFilter = useSelector((state) => state.subscriberFilter);
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
          value={subscriberFilter.keyword}
          onChange={(e) => dispatch(updateKeyword(e.target.value))}
        />
      </Form.Group>
      <Form.Group className="col-auto">
        <Form.Label className="visually-hidden"> Email người bình luận </Form.Label>
        <Form.Control
          type="text"
          placeholder="Nhập email người bình luận..."
          name="email"
          value={subscriberFilter.email}
          onChange={(e) => dispatch(updateEmail(e.target.value))}
        />
      </Form.Group>
      <Form.Group className="col-auto">
        <Form.Label className="form-check-label"> Buộc khoá </Form.Label>
        <Form.Control
          className="form-check-input"
          type="checkbox"
          name="forceLock"
          checked={subscriberFilter.forceLock}
          title="Force Lock"
          onChange={(e) => dispatch(updateForceLock(e.target.checked))}
        />
      </Form.Group>
      <Form.Group className="col-auto">
        <Form.Label className="form-check-label"> Tự huỷ đăng ký </Form.Label>
        <Form.Control
          className="form-check-input"
          type="checkbox"
          name="unsubscribeVoluntary"
          checked={subscriberFilter.unsubscribeVoluntary}
          title="Unsubscribe Voluntary"
          onChange={(e) => dispatch(updateUnsubscribeVoluntary(e.target.checked))}
        />
      </Form.Group>
      <Form.Group className="col-auto">
        <Button variant="danger" type="reset">
          Xóa lọc
        </Button>
        <Link to="/admin/subscribers/edit" className="btn btn-success ms-2">
          Thêm mới
        </Link>
      </Form.Group>
    </Form>
  );
};

export default SubscriberFilterPane;
