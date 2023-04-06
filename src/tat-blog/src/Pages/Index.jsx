import { useEffect, useState, useMemo } from 'react';
import { useLocation, useParams } from 'react-router-dom';
import PostItem from '../Components/PostItem';
import Pager from '../Components/Pager';
import {
  getPosts,
  getPostByArchives,
  getPostByCategorySlug,
  getPostByTagSlug,
  getPostByAuthorSlug,
} from '../Services/BlogRepository';
import BestAuthors from '../Components/BestAuthors/BestAuthors';

const Index = () => {
  const [postList, setPostList] = useState([]);
  const [metadata, setMetadata] = useState({});
  const { pathname, search } = useLocation();
  const { slug: dynamicSlug } = useParams();

  function useQuery() {
    return useMemo(() => new URLSearchParams(search), [search]);
  }

  const query = useQuery(),
    k = query.get('k') ?? '',
    p = query.get('p') ?? 1,
    ps = query.get('ps') ?? 2,
    slug = query.get('slug'),
    year = query.get('year'),
    month = query.get('month');

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
          data.postquery.restQuery = `&year=${year}&month=${month}`;
          data.metadata.actionName = 'archives';
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
          <Pager postquery={{ keyword: k }} metadata={metadata} />
        </div>
      </>
    );
  else return <></>;
};

export default Index;
