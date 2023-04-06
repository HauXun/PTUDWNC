import { useEffect, useState, useMemo } from 'react';
import { useLocation, useParams } from 'react-router-dom';
import { getPostBySlug, getPost } from '../../Services/BlogRepository';
import { getCommentsByPost, postCommentsByPost } from '../../Services/CommentRepository';
import { isEmptyOrSpaces } from '../../Utils/Utils';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import CommentItem from '../Comment/CommentItem';
import Pager from '../Pager';

function Post() {
  const [post, setPost] = useState();
  const [metadata, setMetadata] = useState({});
  const [comments, setComments] = useState([]);
  const [username, setUsername] = useState('');
  const [comment, setComment] = useState('');
  const { search } = useLocation();
  const { year, month, day, slug: postSlug } = useParams();

  function useQuery() {
    return useMemo(() => new URLSearchParams(search), [search]);
  }

  const commentSubmit = (e) => {
    e.preventDefault();

    if (post)
    {
      postCommentsByPost(post.id, username, comment).then((data) => {
        if (data) {
          setComments(pre => [...pre, data]);

          alert('Comment của bạn đã được lưu lại, chờ duyệt nhé!');
      
          setUsername('');
          setComment('');
        }
      });
    }
  };

  const query = useQuery(),
    slug = query.get('slug'),
    p = query.get('p') ?? 1,
    ps = query.get('ps') ?? 2;

  useEffect(() => {
    document.title = 'Trang  chủ';

    if (postSlug) {
      getPost(Number(year), Number(month), Number(day), postSlug).then((data) => {
        if (data) {
          let imageUrl = isEmptyOrSpaces(data.items[0].imageUrl)
            ? import.meta.env.VITE_PUBLIC_URL + '/images/image_1.jpg'
            : `${data.items[0].imageUrl}`;
          data.items[0].imageUrl = imageUrl;
          setPost(data.items[0]);
        } else setPost();
      });
    } else {
      getPostBySlug(slug).then((data) => {
        if (data) {
          let imageUrl = isEmptyOrSpaces(data.imageUrl)
            ? import.meta.env.VITE_PUBLIC_URL + '/images/image_1.jpg'
            : `${data.imageUrl}`;
          data.imageUrl = imageUrl;
          setPost(data);
        } else setPost([]);
      });
    }
  }, [slug, year, month, day, postSlug]);

  useEffect(() => {
    if (post) {
      getCommentsByPost(post.id, ps, p).then((data) => {
        if (data) {
          setComments(data.items);
          setMetadata(data.metadata);
        } else {
          setComments([]);
          setMetadata({});
        }
      });
    }
  }, [post, ps, p]);

  if (post)
    return (
      <>
        <div className="p-4 mb-5">
          <h3 className="text-success mb-2">Tiêu đề: {post.title}</h3>
          <p>
            Ngày tạo: <i>{post.postedDate}</i>
          </p>
          <img src={post.imageUrl} alt={post.title} className="img-thumbnail" />
          <hr />
          <h5>Giới thiệu bài viết:</h5>
          {post.shortDescription}
          <h5>Mô tả bài viết:</h5>
          {post.description}
          <h6>
            Lượt xem: <p className="text-primary">{post.viewCount}</p>
          </h6>
          <h6>
            Tác giả: <p className="text-primary">{post.author.fullName}</p>
          </h6>
          <h6>
            Chủ đề: <p className="text-primary">{post.category.name}</p>
          </h6>
          <figure>
            {post.tags &&
              post.tags.length > 0 &&
              post.tags.map((tag, i) => {
                <h6 key={i}>
                  <text className="text-primary">{tag.name}</text>
                </h6>;
              })}
          </figure>
        </div>

        <Row className="p-4 mb-3">
          <Col sm={3}>
            <span className="badge rounded-pill bg-black">Các bình luận</span>
          </Col>
          {comments &&
            comments.length > 0 &&
            comments.map((comment, i) => {
              return <CommentItem commentItem={comment} key={i} />;
            })}
          <Pager postquery={{}} metadata={metadata} />
        </Row>

        <Form className="row gy-2 gx-3 align-items-center" onSubmit={commentSubmit}>
          <input type="hidden" name="postId" value={post.id} />
          <Row className="mb-3">
            <Col sm={5} className="badge rounded-pill bg-primary">
              <span>Bình luận về bài viết</span>
            </Col>
          </Row>
          <Row className="mb-3">
            <Form.Group controlId="form.name">
              <Col sm={2}>
                <Form.Label>Tên của bạn</Form.Label>
              </Col>
              <Col sm={5}>
                <Form.Control
                  type="text"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                  placeholder="Enter your name"
                  required
                />
              </Col>
            </Form.Group>
          </Row>
          <Row className="mb-3">
            <Form.Group controlId="form.comment">
              <Col sm={2}>
                <Form.Label>Nội dung</Form.Label>
              </Col>
              <Col sm={12}>
                <Form.Control
                  as="textarea"
                  value={comment}
                  onChange={(e) => setComment(e.target.value)}
                  placeholder="Enter your comment"
                  style={{ maxHeight: 300 + 'px' }}
                  rows={50}
                  required
                />
              </Col>
            </Form.Group>
          </Row>
          <Row className="mb-3">
            <div className="d-block d-flex flex-row-reverse">
              <Button type="submit">Bình luận</Button>
            </div>
          </Row>
        </Form>
      </>
    );
  else return <></>;
}

export default Post;
