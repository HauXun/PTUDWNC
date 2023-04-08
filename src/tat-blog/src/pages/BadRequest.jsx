import { Link } from 'react-router-dom';
import { useQuery } from '../utils/utils';
const BadRequest = () => {
  let query = useQuery(),
    redirectTo = query.get('redirectTo') ?? '/';
  return <> … </>;
};
export default BadRequest;
