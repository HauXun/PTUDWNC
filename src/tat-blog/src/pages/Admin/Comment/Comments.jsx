import React, { useEffect, useState } from 'react';
import Button from 'react-bootstrap/Button';
import Table from 'react-bootstrap/Table';
import { useSelector } from 'react-redux';
import { Link, useParams } from 'react-router-dom';
import Loading from '../../../components/Loading';
import VerifyModal from '../../../components/Modal/VerifyModal';
import Pager from '../../../components/Pager';
import { deleteCommentById, getComments, switchCommentCensored } from '../../../services/commentRepository';
import { useQuery } from '../../../utils/utils';
import CommentFilterPane from './CommentFilterPane';

const Comments = () => {
  const [commentList, setCommentList] = useState([]);
  const [metadata, setMetadata] = useState({});
  const [commentQuery, setCommentQuery] = useState({});
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [isVisibleLoading, setIsVisibleLoading] = useState(true);
  const commentFilter = useSelector((state) => state.commentFilter);

  let { id } = useParams();
  const query = useQuery();
  const p = query.get('p') ?? 1;
  const ps = query.get('ps') ?? 2;

  const handleCloseDeleteModal = () => setShowDeleteModal(false);
  const handleShowDeleteModal = () => setShowDeleteModal(true);

  useEffect(() => {
    document.title = 'Danh sách các bình luận';
    getComments(
      commentFilter.keyword,
      ps,
      p,
      commentFilter.userName,
      commentFilter.postTitle,
      commentFilter.year,
      commentFilter.month,
      commentFilter.day,
      commentFilter.censored,
    ).then((data) => {
      if (data) {
        setCommentQuery((pre) => {
          return { ...pre, to: '/admin/comments' };
        });
        setCommentList(data.items);
        setMetadata(data.metadata);
      } else setCommentList([]);
      setIsVisibleLoading(false);
    });
  }, [
    commentFilter.keyword,
    commentFilter.userName,
    commentFilter.postTitle,
    commentFilter.year,
    commentFilter.month,
    commentFilter.day,
    commentFilter.censored,
    p,
    ps,
  ]);

  const deletedChanged = (id) => {
    deleteCommentById(id).then((data) => {
      if (data) {
        setCommentList((pre) => {
          alert('Comment deleted');
          return pre.filter((post) => post.id !== id);
        });
      }
    });
  };

  const censoredOnChanged = (id) => {
    switchCommentCensored(id).then((data) => {
      if (data) {
        setCommentList((pre) => {
          return pre.filter((post) => post.id !== id);
        });
      }
    });
  };

  return (
    <>
      <h1>Danh sách các bình luận {id}</h1>
      <CommentFilterPane />
      {isVisibleLoading ? (
        <Loading />
      ) : (
        <Table striped responsive bordered>
          <thead>
            <tr>
              <th>Tên người bình luận</th>
              <th>Nội dung</th>
              <th>Kiểm duyệt</th>
            </tr>
          </thead>
          <tbody>
            {commentList && commentList.length > 0 ? (
              commentList.map((item, index) => (
                <tr key={index}>
                  <td>
                    <Link to={`/admin/comments/edit/${item.id}`} className="text-bold">
                      {item.userName}
                    </Link>
                    <p className="text-muted">Ngày bình luận: {new Date(item.postDate).toISOString().replace("T"," ").substring(0, 19)}</p>
                  </td>
                  <td>{item.content}</td>
                  <td>
                    {item.censored ? (
                      <Button onClick={(e) => censoredOnChanged(item.id)} variant="success">
                        Có
                      </Button>
                    ) : (
                      <Button onClick={(e) => censoredOnChanged(item.id)} variant="warning">
                        Không
                      </Button>
                    )}
                  </td>
                  <td>
                    <Button onClick={handleShowDeleteModal} variant="danger">
                      Xoá
                    </Button>
                    <VerifyModal
                      show={showDeleteModal}
                      modalTitle="Xoá các bình luận"
                      modalBody={`Bạn có chắc chắn muốn xoá các bình luận " ${item.name} "?`}
                      handleClose={handleCloseDeleteModal}
                      onVerify={() => deletedChanged(item.id)}
                    />
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan={4}>
                  <h4 className="text-danger text-center">Không tìm thấy các bình luận nào</h4>
                </td>
              </tr>
            )}
          </tbody>
        </Table>
      )}
      {commentList && commentList.length > 0 && (
        <Pager postQuery={{ ...commentQuery }} metadata={metadata} />
      )}
    </>
  );
};

export default Comments;
