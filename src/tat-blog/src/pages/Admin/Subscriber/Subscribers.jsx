import React, { useEffect, useState } from 'react';
import Button from 'react-bootstrap/Button';
import Table from 'react-bootstrap/Table';
import { useSelector } from 'react-redux';
import { Link, useParams } from 'react-router-dom';
import Loading from '../../../components/Loading';
import Pager from '../../../components/Pager';
import { blockSubscriber, getSubscribers } from '../../../services/subscriberRepository';
import { useQuery } from '../../../utils/utils';
import SubscriberFilterPane from './SubscriberFilterPane';

const Subscribers = () => {
  const [subscriberList, setSubscriberList] = useState([]);
  const [metadata, setMetadata] = useState({});
  const [subscriberQuery, setSubscriberQuery] = useState({});
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [isVisibleLoading, setIsVisibleLoading] = useState(true);
  const subscriberFilter = useSelector((state) => state.subscriberFilter);

  let { id } = useParams();
  const query = useQuery();
  const p = query.get('p') ?? 1;
  const ps = query.get('ps') ?? 2;

  const handleCloseDeleteModal = () => setShowDeleteModal(false);
  const handleShowDeleteModal = () => setShowDeleteModal(true);

  useEffect(() => {
    document.title = 'Danh sách người bình luận';
    getSubscribers(
      subscriberFilter.keyword,
      ps,
      p,
      subscriberFilter.email,
      subscriberFilter.forceLock,
      subscriberFilter.unsubscribeVoluntary,
    ).then((data) => {
      if (data) {
        setSubscriberQuery((pre) => {
          return { ...pre, to: '/admin/subscribers' };
        });
        setSubscriberList(data.items);
        setMetadata(data.metadata);
      } else setSubscriberList([]);
      setIsVisibleLoading(false);
    });
  }, [
    subscriberFilter.keyword,
    subscriberFilter.email,
    subscriberFilter.forceLock,
    subscriberFilter.unsubscribeVoluntary,
    p,
    ps,
  ]);

  const forceLockChanged = (id) => {
    blockSubscriber(id, {}).then((data) => {
      if (data) {
        setSubscriberList((pre) => {
          return pre.filter((post) => post.id !== id);
        });
      }
    });
  };

  return (
    <>
      <h1>Danh sách người bình luận {id}</h1>
      <SubscriberFilterPane />
      {isVisibleLoading ? (
        <Loading />
      ) : (
        <Table striped responsive bordered>
          <thead>
            <tr>
              <th>Email người đăng ký</th>
              <th>Lý do huỷ</th>
              <th>Ghi chú của quản trị</th>
              <th>Buộc khoá</th>
            </tr>
          </thead>
          <tbody>
            {subscriberList && subscriberList.length > 0 ? (
              subscriberList.map((item, index) => (
                <tr key={index}>
                  <td>
                    <Link to={`/admin/subscribers/edit/${item.id}`} className="text-bold">
                      {item.subscribeEmail}
                    </Link>
                    <p className="text-muted">Ngày đăng ký: {new Date(item.subDated).toISOString().replace("T"," ").substring(0, 19)}</p>
                    {item.unSubDated && <p className="text-muted">Ngày huỷ đăng ký: {new Date(item.postDate).toISOString().replace("T"," ").substring(0, 19)}</p>}
                    {item.unsubscribeVoluntary && <p className="text-muted">Tự huỷ đăng ký</p>}
                  </td>
                  <td>{item.cancelReason}</td>
                  <td>{item.adminNotes}</td>
                  <td>
                    {item.forceLock ? (
                      <Button onClick={(e) => forceLockChanged(item.id)} variant="success">
                        Không
                      </Button>
                    ) : (
                      <Button onClick={(e) => forceLockChanged(item.id)} variant="warning">
                        Khoá
                      </Button>
                    )}
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan={4}>
                  <h4 className="text-danger text-center">Không tìm thấy người bình luận nào</h4>
                </td>
              </tr>
            )}
          </tbody>
        </Table>
      )}
      {subscriberList && subscriberList.length > 0 && (
        <Pager postQuery={{ ...subscriberQuery }} metadata={metadata} />
      )}
    </>
  );
};

export default Subscribers;
