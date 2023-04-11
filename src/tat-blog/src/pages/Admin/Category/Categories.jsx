import React, { useEffect, useState } from 'react';
import Button from 'react-bootstrap/Button';
import Table from 'react-bootstrap/Table';
import { useSelector } from 'react-redux';
import { Link, useParams } from 'react-router-dom';
import Loading from '../../../components/Loading';
import VerifyModal from '../../../components/Modal/VerifyModal';
import Pager from '../../../components/Pager';
import { deleteCategoryById, getCategories, switchShowOnMenu } from '../../../services/categoryRepository';
import { useQuery } from '../../../utils/utils';
import CategoryFilterPane from './CategoryFilterPane';

const Categories = () => {
  const [categoryList, setCategoryList] = useState([]);
  const [metadata, setMetadata] = useState({});
  const [categoryQuery, setCategoryQuery] = useState({});
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [isVisibleLoading, setIsVisibleLoading] = useState(true);
  const [categoryDelete, setCategoryDelete] = useState({});
  const categoryFilter = useSelector((state) => state.categoryFilter);

  let { id } = useParams();
  const query = useQuery();
  const p = query.get('p') ?? 1;
  const ps = query.get('ps') ?? 2;

  const handleCloseDeleteModal = () => setShowDeleteModal(false);
  const handleShowDeleteModal = (category) => {
    setShowDeleteModal(true);
    setCategoryDelete(category);
  };

  useEffect(() => {
    document.title = 'Danh sách chủ đề';
    getCategories(
      categoryFilter.keyword,
      ps,
      p,
      categoryFilter.showOnMenu,
    ).then((data) => {
      if (data) {
        setCategoryQuery((pre) => {
          return { ...pre, to: '/admin/categories' };
        });
        setCategoryList(data.items);
        setMetadata(data.metadata);
      } else setCategoryList([]);
      setIsVisibleLoading(false);
    });
  }, [
    categoryFilter.keyword,
    categoryFilter.showOnMenu,
    p,
    ps,
  ]);

  const showOnMenuChanged = (id) => {
    switchShowOnMenu(id).then((data) => {
      if (data) {
        setCategoryList((pre) => {
          return pre.filter((post) => post.id !== id);
        });
      }
    });
  };

  const deletedChanged = (id) => {
    deleteCategoryById(id).then((data) => {
      if (data) {
        setCategoryList((pre) => {
          alert('Category deleted');
          return pre.filter((post) => post.id !== id);
        });
      }
    });
  };

  return (
    <>
      <h1>Danh sách chủ đề {id}</h1>
      <CategoryFilterPane />
      {isVisibleLoading ? (
        <Loading />
      ) : (
        <Table striped responsive bordered>
          <thead>
            <tr>
              <th>Tiêu đề</th>
              <th>Bài viết liên quan</th>
              <th>Hiển thị</th>
            </tr>
          </thead>
          <tbody>
            {categoryList && categoryList.length > 0 ? (
              categoryList.map((item, index) => (
                <tr key={index}>
                  <td>
                    <Link to={`/admin/categories/edit/${item.id}`} className="text-bold">
                      {item.name}
                    </Link>
                    <p className="text-muted">{item.description}</p>
                  </td>
                  <td>{item.postCount}</td>
                  <td>
                    {item.showOnMenu ? (
                      <Button onClick={(e) => showOnMenuChanged(item.id)} variant="success">
                        Có
                      </Button>
                    ) : (
                      <Button onClick={(e) => showOnMenuChanged(item.id)} variant="warning">
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
                      modalTitle="Xoá chủ đề"
                      modalBody={`Bạn có chắc chắn muốn xoá chủ đề " ${item.name} "?`}
                      handleClose={handleCloseDeleteModal}
                      onVerify={() => deletedChanged(item.id)}
                    />
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan={4}>
                  <h4 className="text-danger text-center">Không tìm thấy chủ đề nào</h4>
                </td>
              </tr>
            )}
          </tbody>
        </Table>
      )}
      {categoryList && categoryList.length > 0 && (
        <Pager postQuery={{ ...categoryQuery }} metadata={metadata} />
      )}
    </>
  );
};

export default Categories;
