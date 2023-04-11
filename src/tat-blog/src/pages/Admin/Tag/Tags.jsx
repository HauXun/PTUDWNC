import React, { useEffect, useState } from 'react';
import Button from 'react-bootstrap/Button';
import Table from 'react-bootstrap/Table';
import { useSelector } from 'react-redux';
import { Link, useParams } from 'react-router-dom';
import Loading from '../../../components/Loading';
import VerifyModal from '../../../components/Modal/VerifyModal';
import Pager from '../../../components/Pager';
import { deleteTagById, getTags } from '../../../services/tagRepository';
import { useQuery } from '../../../utils/utils';
import TagFilterPane from './TagFilterPane';

const Tags = () => {
  const [tagList, setTagList] = useState([]);
  const [metadata, setMetadata] = useState({});
  const [tagQuery, setTagQuery] = useState({});
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [isVisibleLoading, setIsVisibleLoading] = useState(true);
  const [tagDelete, setTagDelete] = useState({});
  const tagFilter = useSelector((state) => state.tagFilter);

  let { id } = useParams();
  const query = useQuery();
  const p = query.get('p') ?? 1;
  const ps = query.get('ps') ?? 2;

  const handleCloseDeleteModal = () => setShowDeleteModal(false);
  const handleShowDeleteModal = (tag) => {
    setShowDeleteModal(true);
    setTagDelete(tag);
  };

  useEffect(() => {
    document.title = 'Danh sách thẻ';
    getTags(
      tagFilter.keyword,
      ps,
      p,
      tagFilter.name,
    ).then((data) => {
      if (data) {
        setTagQuery((pre) => {
          return { ...pre, to: '/admin/tags' };
        });
        setTagList(data.items);
        setMetadata(data.metadata);
      } else setTagList([]);
      setIsVisibleLoading(false);
    });
  }, [
    tagFilter.keyword,
    tagFilter.name,
    p,
    ps,
  ]);

  const deletedChanged = (id) => {
    deleteTagById(id).then((data) => {
      if (data) {
        setTagList((pre) => {
          alert('Tag deleted');
          return pre.filter((post) => post.id !== id);
        });
      }
    });
  };

  return (
    <>
      <h1>Danh sách thẻ {id}</h1>
      <TagFilterPane />
      {isVisibleLoading ? (
        <Loading />
      ) : (
        <Table striped responsive bordered>
          <thead>
            <tr>
              <th>Tên thẻ</th>
              <th>Bài viết liên quan</th>
            </tr>
          </thead>
          <tbody>
            {tagList && tagList.length > 0 ? (
              tagList.map((item, index) => (
                <tr key={index}>
                  <td>
                    <Link to={`/admin/tags/edit/${item.id}`} className="text-bold">
                      {item.name}
                    </Link>
                    <p className="text-muted">{item.description}</p>
                  </td>
                  <td>{item.postCount}</td>
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
                  <h4 className="text-danger text-center">Không tìm thấy thẻ nào</h4>
                </td>
              </tr>
            )}
          </tbody>
        </Table>
      )}
      {tagList && tagList.length > 0 && (
        <Pager postQuery={{ ...tagQuery }} metadata={metadata} />
      )}
      <VerifyModal
        show={showDeleteModal}
        modalTitle="Xoá thẻ"
        modalBody={`Bạn có chắc chắn muốn xoá thẻ " ${tagDelete.name} "?`}
        handleClose={handleCloseDeleteModal}
        onVerify={() => deletedChanged(tagDelete.id)}
      />
    </>
  );
};

export default Tags;
