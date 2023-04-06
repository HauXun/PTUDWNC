import Navbar from '../Components/Navbar';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Sidebar from '../Components/Sidebar';
import Footer from '../Components/Footer';
import Layout from '../Pages/Layout';
import Index from '../Pages/Index';
import Contact from '../Pages/Contact';
import About from '../Pages/About';
import Rss from '../Pages/Rss';
import Post from '../Components/Post/Post';

export default function App() {
  return (
    <div>
      <Router>
        <Navbar />
        <div className="container-fluid">
          <div className="row">
            <div className="col-9">
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
              </Routes>
            </div>
            <div className="col-3 border-start">
              <Sidebar />
            </div>
          </div>
        </div>
        <Footer />
      </Router>
    </div>
  );
}
