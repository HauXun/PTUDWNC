import { Route, BrowserRouter as Router, Routes } from 'react-router-dom';
import Footer from '../components/Footer';
import Post from '../components/Post/Post';
import About from '../pages/About';
import Authors from '../pages/Admin/Author/Authors';
import * as AuthorEdit from '../pages/Admin/Author/Edit';
import Categories from '../pages/Admin/Category/Categories';
import * as CategoryEdit from '../pages/Admin/Category/Edit';
import Comments from '../pages/Admin/Comment/Comments';
import * as AdminIndex from '../pages/Admin/Index';
import AdminLayout from '../pages/Admin/Layout';
import * as PostEdit from '../pages/Admin/Post/Edit';
import Posts from '../pages/Admin/Post/Posts';
import * as TagEdit from '../pages/Admin/Tag/Edit';
import Tags from '../pages/Admin/Tag/Tags';
import BadRequest from '../pages/BadRequest';
import Contact from '../pages/Contact';
import Index from '../pages/Index';
import Layout from '../pages/Layout';
import NotFound from '../pages/NotFound';
import Rss from '../pages/Rss';
import Subscribers from '../pages/Admin/Subscriber/Subscribers';

export default function App() {
  return (
    <div>
      <Router>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route path="/" element={<Index />} />
            <Route path="blog" element={<Index />} />
            <Route path="blog/post" element={<Post />} />
            <Route path="blog/post/:year/:month/:day/:slug" element={<Post />} />
            <Route path="blog/author" element={<Index />} />
            <Route path="blog/author/:slug" element={<Index />} />
            <Route path="blog/category" element={<Index />} />
            <Route path="blog/category/:slug" element={<Index />} />
            <Route path="blog/tag" element={<Index />} />
            <Route path="blog/archives" element={<Index />} />
            <Route path="blog/contact" element={<Contact />} />
            <Route path="blog/about" element={<About />} />
            <Route path="blog/rss" element={<Rss />} />
          </Route>
          <Route path="/admin" element={<AdminLayout />}>
            <Route path="/admin" element={<AdminIndex.default />} />
            <Route path="/admin/authors" element={<Authors />} />
            <Route path="/admin/authors/edit" element={<AuthorEdit.default />} />
            <Route path="/admin/authors/edit/:id" element={<AuthorEdit.default />} />
            <Route path="/admin/categories" element={<Categories />} />
            <Route path="/admin/categories/edit" element={<CategoryEdit.default />} />
            <Route path="/admin/categories/edit/:id" element={<CategoryEdit.default />} />
            <Route path="/admin/posts" element={<Posts />} />
            <Route path="/admin/posts/edit" element={<PostEdit.default />} />
            <Route path="/admin/posts/edit/:id" element={<PostEdit.default />} />
            <Route path="/admin/tags" element={<Tags />} />
            <Route path="/admin/tags/edit" element={<TagEdit.default />} />
            <Route path="/admin/tags/edit/:id" element={<TagEdit.default />} />
            <Route path="/admin/comments" element={<Comments />} />
            <Route path="/admin/subscribers" element={<Subscribers />} />
          </Route>
          <Route path="/400" element={<BadRequest />} />
          <Route path="*" element={<NotFound />} />
        </Routes>
        <Footer />
      </Router>
    </div>
  );
}
