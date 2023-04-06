import { useEffect, useState } from 'react';
import Row from 'react-bootstrap/Row';
import FormGroup from 'react-bootstrap/FormGroup';
import FormControl from 'react-bootstrap/FormControl';
import FormLabel from 'react-bootstrap/FormLabel';
import Button from 'react-bootstrap/Button';
import { getArchivesPost } from '../../Services/Widgets';
import Form from 'react-bootstrap/Form';
import { postSubscriber } from '../../Services/SubscriberRepository';

const NewsLetterForm = () => {
  const [datesList, setDatesList] = useState([]);
  const [email, setEmail] = useState('');

  const emailSubmit = (e) => {
    e.preventDefault();

    if (email)
    {
      postSubscriber(email).then((data) => {
        if (data) {
          alert('Thông tin của bạn đã được lưu lại, chờ duyệt nhé!');
      
          setEmail('');
        }
      });
    }
  };

  useEffect(() => {
    getArchivesPost().then((data) => {
      if (data) setDatesList(data);
      else setDatesList([]);
    });
  }, []);

  return (
    <div className="mb-4">
      <h3 className="text-success mb-2">
      Đăng ký nhận thông báo khi có bài viết mới
      </h3>
      <Row>
        <section>
          <Form onSubmit={emailSubmit}>
            <FormGroup className='mb-3'>
              <FormLabel htmlFor='email'>Email:</FormLabel>
              <FormControl type='email' id='email' name='email' autoComplete='email' aria-required placeholder='name@example.com' value={email} onChange={(e) => setEmail(e.target.value)} />
            </FormGroup>
            <Button type='submit' className='btn btn-primary'>Đăng ký</Button>
          </Form>
        </section>
      </Row>
    </div>
  );
};

export default NewsLetterForm;
