import React, { useEffect, useState } from 'react';
import Button from 'react-bootstrap/Button';
import Table from 'react-bootstrap/Table';
import { useSelector } from 'react-redux';
import { Link, useParams } from 'react-router-dom';
import Loading from '../../../components/Loading';
import VerifyModal from '../../../components/Modal/VerifyModal';
import Pager from '../../../components/Pager';
import { deleteAuthorById, getAuthors } from '../../../services/authorRepository';
import { useQuery } from '../../../utils/utils';
import AuthorFilterPane from './AuthorFilterPane';

const Categories = () => {
  const [authorList, setAuthorList] = useState([]);
  const [metadata, setMetadata] = useState({});
  const [authorQuery, setAuthorQuery] = useState({});
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [isVisibleLoading, setIsVisibleLoading] = useState(true);
  const [authorDelete, setAuthorDelete] = useState({});
  const authorFilter = useSelector((state) => state.authorFilter);

  let { id } = useParams();
  const query = useQuery();
  const p = query.get('p') ?? 1;
  const ps = query.get('ps') ?? 2;

  const handleCloseDeleteModal = () => setShowDeleteModal(false);
  const handleShowDeleteModal = (author) => {
    setShowDeleteModal(true);
    setCategoryDelete(author);
  };

  useEffect(() => {
    document.title = 'Danh sách tác giả';
    getAuthors(authorFilter.keyword, ps, p, authorFilter.fullName, authorFilter.email).then(
      (data) => {
        if (data) {
          setAuthorQuery((pre) => {
            return { ...pre, to: '/admin/authors' };
          });
          setAuthorList(data.items);
          setMetadata(data.metadata);
        } else setAuthorList([]);
        setIsVisibleLoading(false);
      }
    );
  }, [
    authorFilter.keyword,
    authorFilter.showOnMenu,
    authorFilter.fullName,
    authorFilter.email,
    p,
    ps,
  ]);

  const deletedChanged = (id) => {
    deleteAuthorById(id).then((data) => {
      if (data) {
        setAuthorList((pre) => {
          alert('Author deleted');
          return pre.filter((post) => post.id !== id);
        });
      }
    });
  };

  return (
    <>
      <h1>Danh sách tác giả {id}</h1>
      <AuthorFilterPane />
      {isVisibleLoading ? (
        <Loading />
      ) : (
        <Table striped responsive bordered>
          <thead>
            <tr>
              <th>Tên tác giả</th>
              <th>Bài viết liên quan</th>
              <th>Email</th>
            </tr>
          </thead>
          <tbody>
            {authorList && authorList.length > 0 ? (
              authorList.map((item, index) => (
                <tr key={index}>
                  <td>
                    <Link to={`/admin/authors/edit/${item.id}`} className="text-bold">
                      {item.fullName}
                    </Link>
                    <p className="text-muted">
                      Ngày tham gia:{' '}
                      {new Date(item.joinedDate).toISOString().replace('T', ' ').substring(0, 19)}
                    </p>
                  </td>
                  <td>{item.postCount}</td>
                  <td>{item.email}</td>
                  <td>
                    <Button onClick={handleShowDeleteModal} variant="danger">
                      Xoá
                    </Button>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan={4}>
                  <h4 className="text-danger text-center">Không tìm thấy tác giả nào</h4>
                </td>
              </tr>
            )}
          </tbody>
        </Table>
      )}
      {authorList && authorList.length > 0 && (
        <Pager postQuery={{ ...authorQuery }} metadata={metadata} />
      )}
      <VerifyModal
        show={showDeleteModal}
        modalTitle="Xoá tác giả"
        modalBody={`Bạn có chắc chắn muốn xoá tác giả " ${authorDelete.name} "?`}
        handleClose={handleCloseDeleteModal}
        onVerify={() => deletedChanged(authorDelete.id)}
      />
    </>
  );
};

export default Categories;
