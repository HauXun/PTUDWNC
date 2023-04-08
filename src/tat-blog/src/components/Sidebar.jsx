import SearchForm from './SearchForm';
import CategoriesWidget from './CategoriesWidget';
import FeaturedWidget from './FeaturedPosts/FeaturedWidget';
import TagCloudWidget from './TagCloud/TagCloudWidget';
import ArchivesWidget from './Archives/ArchivesWidget';
import NewsLetterForm from './NewsLetterForm/NewsLetterForm';

const Sidebar = () => {
  return (
    <div className="pt-4 ps-2">
      <SearchForm />
      <CategoriesWidget />
      <FeaturedWidget />
      <NewsLetterForm />
      <TagCloudWidget />
      <ArchivesWidget />
    </div>
  );
};

export default Sidebar;
