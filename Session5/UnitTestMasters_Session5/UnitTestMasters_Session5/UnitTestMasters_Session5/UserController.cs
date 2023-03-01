using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestMasters_Session5
{
    public class UserController
    {
        public void RenameUser(int userId, string newName)
        {
            User user = GetUserFromDatabase(userId);

            string normalizedName = user.NormalizeName(newName);
            user.Name = normalizedName;

            SaveUserToDatabase(user);
        }

        private void SaveUserToDatabase(User user)
        {
        }

        private User GetUserFromDatabase(int userId)
        {
            return new User();
        }
    }

    //User class with leaking implementation details
    public class User
    {
        public string Name { get; set; }

        public string NormalizeName(string name)
        {
            string result = (name ?? "").Trim();

            if (result.Length > 50)
                return result.Substring(0, 50);

            return result;
        }
    }


    //version of User with well-designed API
    //public class User
    //{
    //    private string _name;
    //    public string Name
    //    {
    //        get => _name;
    //        set => _name = NormalizeName(value);
    //    }

    //    private string NormalizeName(string name)
    //    {
    //        string result = (name ?? "").Trim();

    //        if (result.Length > 50)
    //            return result.Substring(0, 50);

    //        return result;
    //    }
    //}

    //public class UserController
    //{
    //    public void RenameUser(int userId, string newName)
    //    {
    //        User user = GetUserFromDatabase(userId);
    //        user.Name = newName;
    //        SaveUserToDatabase(user);
    //    }

    //    private void SaveUserToDatabase(User user)
    //    {
    //    }

    //    private User GetUserFromDatabase(int userId)
    //    {
    //        return new User();
    //    }
    //}
}
