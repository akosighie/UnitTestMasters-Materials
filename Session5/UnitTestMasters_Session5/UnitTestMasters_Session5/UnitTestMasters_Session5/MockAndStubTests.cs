using Moq;
using Xunit;

namespace UnitTestMasters_Session5
{
    public class MockAndStubTests
    {
        //Mock - emulate and examines outgoing interaction; produces side-effect; command;
        [Fact]
        public void Sending_a_greetings_email()
        {
            var emailGatewayMock = new Mock<IEmailGateway1>();
            var sut = new Controller(emailGatewayMock.Object);

            sut.GreetUser("user@email.com");

            emailGatewayMock.Verify(
                x => x.SendGreetingsEmail("user@email.com"),
                Times.Once);
        }

        //Stub - emulate incoming interaction; does not produce side effect; query;
        [Fact]
        public void Creating_a_report()
        {
            var stub = new Mock<IDatabase>();
            stub.Setup(x => x.GetNumberOfUsers()).Returns(10);
            var sut = new Controller(stub.Object);

            Report report = sut.CreateReport();

            Assert.Equal(10, report.NumberOfUsers);
        }
    }

    public class Controller
    {
        private readonly IEmailGateway1 _emailGateway;
        private readonly IDatabase _database;

        public Controller(IEmailGateway1 emailGateway)
        {
            _emailGateway = emailGateway;
        }

        public Controller(IDatabase database)
        {
            _database = database;
        }

        public void GreetUser(string userEmail)
        {
            _emailGateway.SendGreetingsEmail(userEmail);
        }

        public Report CreateReport()
        {
            int numberOfUsers = _database.GetNumberOfUsers();
            return new Report(numberOfUsers);
        }
    }

    public class Report
    {
        public int NumberOfUsers { get; }

        public Report(int numberOfUsers)
        {
            NumberOfUsers = numberOfUsers;
        }
    }

    public interface IDatabase
    {
        int GetNumberOfUsers();
    }

    public interface IEmailGateway1
    {
        void SendGreetingsEmail(string userEmail);
    }
}