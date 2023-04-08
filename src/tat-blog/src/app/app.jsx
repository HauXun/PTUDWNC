import { Route, BrowserRouter as Router, Routes } from 'react-router-dom';
import Footer from '../components/Footer';
import Post from '../components/Post/Post';
import About from '../pages/About';
import Authors from '../pages/Admin/Authors';
import Categories from '../pages/Admin/Categories';
import Comments from '../pages/Admin/Comments';
import * as AdminIndex from '../pages/Admin/Index';
import AdminLayout from '../pages/Admin/Layout';
import Posts from '../pages/Admin/Post/Posts';
import Tags from '../pages/Admin/Tags';
import BadRequest from '../pages/BadRequest';
import Contact from '../pages/Contact';
import Index from '../pages/Index';
import Layout from '../pages/Layout';
import NotFound from '../pages/NotFound';
import Rss from '../pages/Rss';
import Edit from '../pages/Admin/Post/Edit';

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
            <Route path="/admin/categories" element={<Categories />} />
            <Route path="/admin/comments" element={<Comments />} />
            <Route path="/admin/posts" element={<Posts />} />
            <Route path="/admin/posts/edit" element={<Edit />} />
            <Route path="/admin/posts/edit/:id" element={<Edit />} />
            <Route path="/admin/tags" element={<Tags />} />
          </Route>
          <Route path="/400" element={<BadRequest />} />
          <Route path="*" element={<NotFound />} />
        </Routes>
        <Footer />
      </Router>
    </div>
  );
}
