using CivicaBookLibraryApi.Data.Contract;
using CivicaBookLibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CivicaBookLibraryApi.Data.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly IAppDbContext _appDbContext;

        public UserRepository(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public User GetUserById(int id)
        {
            var user = _appDbContext.Users.Include(p => p.SecurityQuestion).FirstOrDefault(c => c.UserId == id);
            return user;
        }

        public bool RegisterUser(User user)
        {
            var result = false;
            if (user != null)
            {
                _appDbContext.Users.Add(user);
                _appDbContext.SaveChanges();
                result = true;
            }

            return result;
        }
            
        public User? ValidateUser(string username)
        {
            User? user = _appDbContext.Users.FirstOrDefault(c => c.LoginId.ToLower() == username.ToLower() || c.Email == username.ToLower());
            return user;
        }

        public int UserCount()
        {
            return _appDbContext.Users.Count();

        }

        public bool UserExists(string loginId, string email)
        {
            if (_appDbContext.Users.Any(c => c.LoginId.ToLower() == loginId.ToLower() || c.Email.ToLower() == email.ToLower()))
            {
                return true;
            }
            return false;
        }

        public bool UpdateUser(User user)
        {
            var result = false;
            if (user != null)
            {
                _appDbContext.Users.Update(user);
                _appDbContext.SaveChanges();
                result = true;
            }
            return result;
        }
        public IEnumerable<User> GetAllUsers()
        {
            List<User> users = _appDbContext.Users.Include(p => p.BookIssues).ToList();
            return users;
        }
        public IEnumerable<User> GetPaginatedUsers(int page, int pageSize, string? search, string sortOrder)
        {
            int skip = (page - 1) * pageSize;
            IQueryable<User> users = _appDbContext.Users.Include(p => p.BookIssues).Where(c=>c.IsAdmin == false);
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(c => c.Name.Contains(search) || c.LoginId.Contains(search));
            }
            switch (sortOrder.ToLower())
            {
                case "asc":
                    users = users.OrderBy(b => b.Name);
                    break;
                case "desc":
                    users = users.OrderByDescending(b => b.Name);
                    break;
                default:
                    users = users.OrderBy(b => b.Name);
                    break;
            }
            return users
            .Skip(skip)
                .Take(pageSize)
                .ToList();
        }
        public bool DeleteUser(int id)
        {
            var result = false;
            var user = _appDbContext.Users.Find(id);
            if (user != null)
            {
                _appDbContext.Users.Remove(user);
                _appDbContext.SaveChanges();
                result = true;
            }
            return result;
        }
        public int TotalUsers(string? search)
        {
            IQueryable<User> users = _appDbContext.Users.Where(c=>c.IsAdmin == false);

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(c => c.Name.Contains(search) || c.LoginId.Contains(search));
            }
            return users.Count();
        }
    }
}
