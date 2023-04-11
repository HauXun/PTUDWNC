import { useEffect, useState } from 'react';
import { useLocation, useParams } from 'react-router-dom';
import PostItem from '../components/PostItem';
import Pager from '../components/Pager';
import {
  getPosts,
  getPostByArchives,
  getPostByCategorySlug,
  getPostByTagSlug,
  getPostByAuthorSlug,
} from '../services/blogRepository';
import BestAuthors from '../components/BestAuthors/BestAuthors';
import { useQuery } from '../utils/utils';

const Index = () => {
  const [postList, setPostList] = useState([]);
  const [metadata, setMetadata] = useState({});
  const [postQuery, setPostQuery] = useState({});
  const { pathname } = useLocation();
  const { slug: dynamicSlug } = useParams();

  const query = useQuery();
  const k = query.get('k') ?? '';
  const p = query.get('p') ?? 1;
  const ps = query.get('ps') ?? 2;
  const slug = query.get('slug');
  const year = query.get('year');
  const month = query.get('month');

  useEffect(() => {
    document.title = 'Trang  chủ';

    if (pathname.includes('category')) {
      const passSlug = slug || dynamicSlug;
      getPostByCategorySlug(passSlug, ps, p).then((data) => {
        if (data.items) {
          data.metadata.slug = passSlug;
          data.metadata.actionName = 'category';
          setPostList(data.items);
          setMetadata(data.metadata);
        } else setPostList([]);
      });
    } else if (pathname.includes('tag')) {
      const passSlug = slug || dynamicSlug;
      getPostByTagSlug(passSlug, ps, p).then((data) => {
        if (data.items) {
          data.metadata.slug = passSlug;
          data.metadata.actionName = 'tag';
          setPostList(data.items);
          setMetadata(data.metadata);
        } else setPostList([]);
      });
    } else if (pathname.includes('author')) {
      const passSlug = slug || dynamicSlug;
      getPostByAuthorSlug(passSlug, ps, p).then((data) => {
        if (data.items) {
          data.metadata.slug = passSlug;
          data.metadata.actionName = 'author';
          setPostList(data.items);
          setMetadata(data.metadata);
        } else setPostList([]);
      });
    } else if (pathname.includes('archives')) {
      getPostByArchives(year, month, ps, p).then((data) => {
        if (data.items) {
          data.metadata.actionName = 'archives';
          setPostQuery((pre) => {
            return { ...pre, restQuery: `&year=${year}&month=${month}` };
          });
          setPostList(data.items);
          setMetadata(data.metadata);
        } else setPostList([]);
      });
    } else {
      getPosts(k, ps, p).then((data) => {
        if (data) {
          setPostList(data.items);
          setMetadata(data.metadata);
        } else setPostList([]);
      });
    }
  }, [k, p, ps, slug, year, month]);

  useEffect(() => {
    window.scrollTo(0, 0);
  }, [postList]);

  if (postList.length > 0)
    return (
      <>
        <hr />
        <BestAuthors />
        <div className="p-4">
          {postList.map((item, index) => {
            return <PostItem postItem={item} key={index} />;
          })}
          <Pager postQuery={{ ...postQuery, keyword: k }} metadata={metadata} />
        </div>
      </>
    );
  else return <></>;
};

export default Index;
