namespace AALKisAPI.Services
{
    public interface IUserRepository
    {
        public  Task SignIn(string username, string password, string email);
        public bool IsSignedIn(string username, string password);
        public bool IsValidName(string name);
        public bool IsValidEmail(string email);
        public bool IsNameTaken(string name);
        public AALKisAPI.Models.Users GetUser(string name);

    }
}
