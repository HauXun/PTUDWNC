import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import {
  faCodeFork,
  faTicket,
  faDatabase,
  faUser,
  faCube,
  faDashboard,
  faCompassDrafting,
} from '@fortawesome/free-solid-svg-icons';
import { useEffect, useState } from 'react';
import {
  getTotalAuthor,
  getTotalCategories,
  getTotalNewestSubscriber,
  getTotalPosts,
  getTotalSubscribers,
  getTotalUnpublishedPosts,
  getTotalWaitingComments,
} from '../../services/dashboardRepository';

const Index = () => {
  const [statistic, setStatistic] = useState({
    totalPosts: 0,
    totalUnpublishedPosts: 0,
    totalCategories: 0,
    totalAuthor: 0,
    totalWaitingComments: 0,
    totalSubscribers: 0,
    totalNewestSubscriber: 0,
  });

  useEffect(() => {
    Promise.all([
      getTotalAuthor(),
      getTotalCategories(),
      getTotalNewestSubscriber(),
      getTotalPosts(),
      getTotalSubscribers(),
      getTotalUnpublishedPosts(),
      getTotalWaitingComments(),
    ]).then(
      ([
        totalAuthor,
        totalCategories,
        totalNewestSubscriber,
        totalPosts,
        totalSubscribers,
        totalUnpublishedPosts,
        totalWaitingComments,
      ]) => {
        setStatistic({
          totalAuthor: totalAuthor ?? 0,
          totalCategories: totalCategories ?? 0,
          totalNewestSubscriber: totalNewestSubscriber ?? 0,
          totalPosts: totalPosts ?? 0,
          totalSubscribers: totalSubscribers ?? 0,
          totalUnpublishedPosts: totalUnpublishedPosts ?? 0,
          totalWaitingComments: totalWaitingComments ?? 0,
        });
      }
    );
  }, []);

  return (
    <div className="container">
      <div className="row">
        <div className="col-md-3 mb-5">
          <div className="card">
            <div className="card-body card-counter text-white bg-primary">
              <FontAwesomeIcon icon={faCodeFork} /> &nbsp;
              <span className="count-numbers">{statistic.totalPosts}</span> <br />
              <span className="count-name">Số bài viết</span>
            </div>
          </div>
        </div>

        <div className="col-md-3 mb-5">
          <div className="card">
            <div className="card-body card-counter text-white bg-danger">
              <FontAwesomeIcon icon={faTicket} /> &nbsp;
              <span className="count-numbers">{statistic.totalUnpublishedPosts}</span> <br />
              <span className="count-name">Số bài viết chưa xuất bản</span>
            </div>
          </div>
        </div>

        <div className="col-md-3 mb-5">
          <div className="card">
            <div className="card-body card-counter text-white bg-success">
              <FontAwesomeIcon icon={faDatabase} /> &nbsp;
              <span className="count-numbers">{statistic.totalCategories}</span> <br />
              <span className="count-name">Số lượng chủ đề</span>
            </div>
          </div>
        </div>

        <div className="col-md-3 mb-5">
          <div className="card">
            <div className="card-body card-counter text-white bg-info">
              <FontAwesomeIcon icon={faUser} /> &nbsp;
              <span className="count-numbers">{statistic.totalAuthor}</span> <br />
              <span className="count-name">Số lượng tác giả</span>
            </div>
          </div>
        </div>

        <div className="col-md-3 mb-5">
          <div className="card">
            <div className="card-body card-counter text-white bg-secondary">
              <FontAwesomeIcon icon={faCube} /> &nbsp;
              <span className="count-numbers">{statistic.totalWaitingComments}</span> <br />
              <span className="count-name">Số lượng bình luận đang chờ phê duyệt</span>
            </div>
          </div>
        </div>

        <div className="col-md-3 mb-5">
          <div className="card">
            <div className="card-body card-counter text-white bg-warning">
              <FontAwesomeIcon icon={faDashboard} /> &nbsp;
              <span className="count-numbers">{statistic.totalSubscribers}</span> <br />
              <span className="count-name">Số lượng người theo dõi</span>
            </div>
          </div>
        </div>

        <div className="col-md-3 mb-5">
          <div className="card">
            <div className="card-body card-counter text-white bg-dark">
              <FontAwesomeIcon icon={faCompassDrafting} /> &nbsp;
              <span className="count-numbers">{statistic.totalNewestSubscriber}</span> <br />
              <span className="count-name">Số lượng người mới theo dõi đăng ký (trong ngày)</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
export default Index;
