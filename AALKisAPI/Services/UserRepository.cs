using System.Text.RegularExpressions;

using AALKisAPI.Data;

using AALKisShared.Records;

using Microsoft.EntityFrameworkCore;

using UsersEntity = AALKisAPI.Models.User;

namespace AALKisAPI.Services
{
    public class UserRepository : IUserRepository
    {

        private readonly NoteDB _database;

        public UserRepository(NoteDB database) 
        {
            _database = database;
        }

        public async Task SignIn (string username, string password, string email)
        {
            UsersEntity user = new UsersEntity() 
            {
                Name = username,
                Email = email,
                Password = password
            };
            _database.Users.Add(user);
            _database.SaveChanges();

            var users = _database.Users.ToList(); // Retrieve all users from the database
            foreach (var usertest in users)
            {
                Console.WriteLine($"User ID: {usertest.Id}, Name: {usertest.Name}, Email: {usertest.Email}");
                // Print other properties as needed
            }

        }

        public bool IsSignedIn (string username, string password) 
        { 
            return _database.Users.Where(u  => u.Name == username && u.Password == password).Any();
        }


        public bool IsValidName(string name)
        {
            if (name == "" || name == null || name.Length > 80 )
                return false;
            return true;
        }
        public bool IsValidEmail(string email)
        {
            if (email == null || email.Length == 0) return false;
            Regex validateEmailRegex = new Regex("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");
            if (validateEmailRegex.IsMatch(email))
                return true;
            return false;
        }

        public bool IsNameTaken (string name)
        {
            Console.WriteLine("takennnn");
            return _database.Users.Any(u => u.Name == name);
        }

        public AALKisAPI.Models.User GetUser (string name) 
        {
            return _database.Users.FirstOrDefault(u => u.Name == name);
        }
    }
}
