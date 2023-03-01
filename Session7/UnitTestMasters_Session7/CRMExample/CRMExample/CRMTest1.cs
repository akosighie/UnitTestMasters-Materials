namespace CRMTest1
{
    public class Tests
    {
        [Fact]
        public void Changing_email_without_changing_user_type()
        {
            var sut = new User(1, "user@mycorp.com", UserType.Employee);

            var numberOfEmployees = sut.ChangeEmail("new@mycorp.com", "mycorp.com", 1);

            Assert.Equal(1, numberOfEmployees);
            Assert.Equal("new@mycorp.com", sut.Email);
            Assert.Equal(UserType.Employee, sut.Type);
        }

        [Fact]
        public void Changing_email_from_corporate_to_non_corporate()
        {
            var sut = new User(1, "user@mycorp.com", UserType.Employee);

            var numberOfEmployees = sut.ChangeEmail("new@gmail.com", "mycorp.com", 1);

            Assert.Equal(0, numberOfEmployees);
            Assert.Equal("new@gmail.com", sut.Email);
            Assert.Equal(UserType.Customer, sut.Type);
        }

        [Fact]
        public void Changing_email_from_non_corporate_to_corporate()
        {
            var sut = new User(1, "user@gmail.com", UserType.Customer);

            var numberOfEmployees = sut.ChangeEmail("new@mycorp.com", "mycorp.com", 1);

            Assert.Equal(2, numberOfEmployees);
            Assert.Equal("new@mycorp.com", sut.Email);
            Assert.Equal(UserType.Employee, sut.Type);
        }

        [Fact]
        public void Changing_email_to_the_same_one()
        {
            var sut = new User(1, "user@gmail.com", UserType.Customer);

            var numberOfEmployees = sut.ChangeEmail("user@gmail.com", "mycorp.com", 1);

            Assert.Equal(1, numberOfEmployees);
            Assert.Equal("user@gmail.com", sut.Email);
            Assert.Equal(UserType.Customer, sut.Type);
        }
    }

    public class User
    {
        public int UserId { get; private set; }
        public string Email { get; private set; }
        public UserType Type { get; private set; }

        public User(int userId, string email, UserType type)
        {
            UserId = userId;
            Email = email;
            Type = type;
        }

        public int ChangeEmail(string newEmail,
            string companyDomainName, int numberOfEmployees)
        {
            if (Email == newEmail)
                return numberOfEmployees;

            string emailDomain = newEmail.Split('@')[1];
            bool isEmailCorporate = emailDomain == companyDomainName;
            UserType newType = isEmailCorporate
                ? UserType.Employee
                : UserType.Customer;

            if (Type != newType)
            {
                int delta = newType == UserType.Employee ? 1 : -1;
                int newNumber = numberOfEmployees + delta;
                numberOfEmployees = newNumber;
            }

            Email = newEmail;
            Type = newType;

            return numberOfEmployees;
        }
    }

    public class UserController
    {
        private readonly Database _database = new Database();
        private readonly MessageBus _messageBus = new MessageBus();

        public void ChangeEmail(int userId, string newEmail)
        {
            object[] data = _database.GetUserById(userId);
            string email = (string)data[1];
            UserType type = (UserType)data[2];
            var user = new User(userId, email, type);

            object[] companyData = _database.GetCompany();
            string companyDomainName = (string)companyData[0];
            int numberOfEmployees = (int)companyData[1];

            int newNumberOfEmployees = user.ChangeEmail(
                newEmail, companyDomainName, numberOfEmployees);

            _database.SaveCompany(newNumberOfEmployees);
            _database.SaveUser(user);
            _messageBus.SendEmailChangedMessage(userId, newEmail);
        }
    }

    public enum UserType
    {
        Customer = 1,
        Employee = 2
    }

    public class Database
    {
        public object[] GetUserById(int userId)
        {
            return null;
        }

        public User GetUserByEmail(string email)
        {
            return null;
        }

        public void SaveUser(User user)
        {
        }

        public object[] GetCompany()
        {
            return null;
        }

        public void SaveCompany(int newNumber)
        {
        }
    }

    public class MessageBus
    {
        private IBus _bus;

        public void SendEmailChangedMessage(int userId, string newEmail)
        {
            _bus.Send($"Subject: USER; Type: EMAIL CHANGED; Id: {userId}; NewEmail: {newEmail}");
        }
    }

    internal interface IBus
    {
        void Send(string message);
    }
}