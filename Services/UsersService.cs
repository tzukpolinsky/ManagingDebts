using Entities;
using ManagingDebts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;
namespace Services
{
    public class UsersService: IUsersService
    {   
        private readonly ManagingDebtsContext context;
        public UsersService(ManagingDebtsContext managingDebtsContext)
        {
            this.context = managingDebtsContext;
        }
        public UserEntity GetById(UserEntity user)
        {
            var dbUser = context.Users.Find(user.Id);
            return new UserEntity {CustomerId=dbUser.CustomerId, Id = dbUser.UserId, Email = dbUser.UserEmail, FirstName = dbUser.UserFirstName, LastName = dbUser.UserLastName, IsActive = dbUser.UserIsActive, IsSuperAdmin = dbUser.UserIsSuperAdmin };
        }
        public UserEntity[] GetAll()
        {
            return context.Users
                .Select(x => new UserEntity { CustomerId = x.CustomerId, Email = x.UserEmail, FirstName = x.UserFirstName, LastName = x.UserLastName, Id = x.UserId, IsActive = x.UserIsActive })
                .AsNoTracking().ToArray();
        }
        public (bool isSuccess, string msg) ChangePassword(UserEntity[] users)
        {
            var result = Login(users[0]);
            if (result.isSuccess)
            {
                var userToChange = context.Users.Find(users[1].Id);
                if (userToChange !=null)
                {
                    var password = Encoding.UTF8.GetString(Convert.FromBase64String(users[1].Password));
                    userToChange.UserPassword = Argon2.Hash(password);
                    context.SaveChangesAsync();
                    return (true, "סיסמת המשתמש שונתה בהצלחה");
                }
                return (false, "לא קיים משתמש לפי המזהה שהוכנס");
            }
            return (result.isSuccess, "סיסמת המשתמש הנוכחי : " + result.msg);
        }
        public bool Create(UserEntity user)
        {
            try
            {
                var password = Encoding.UTF8.GetString(Convert.FromBase64String(user.Password));
                var hash = Argon2.Hash(password);
                context.Users.Add(new Users { CustomerId = user.CustomerId, UserEmail = user.Email, UserFirstName = user.FirstName, UserId = user.Id, UserIsActive = true, UserLastName = user.LastName, UserPassword = hash });
                context.SaveChanges();
                return true;
                   
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public (bool isSuccess,string msg,UserEntity user) Login(UserEntity user)
        {
            var dbUser = context.Users.Find(user.Id);
            if (dbUser==null)
            {
                return (false, "לא קיים משתמש ברשות שנבחרה",null);
            }
            if (!dbUser.UserIsActive)
            {
                return (false, "היוזר אינו פעיל", null);
            }
            if (dbUser.UserIsActive)
            {
                var password = Encoding.UTF8.GetString(Convert.FromBase64String(user.Password));
                var verified = Argon2.Verify(dbUser.UserPassword, password);
                return (verified, verified ? "ההתחברות בוצעה בהצלחה" : "פרטי המשתמש אינם תקינים", new UserEntity
                {
                    Id = dbUser.UserId,
                    FirstName = dbUser.UserFirstName,
                    Email = dbUser.UserEmail,
                    LastName = dbUser.UserLastName,
                    IsActive = true,
                    IsSuperAdmin = dbUser.UserIsSuperAdmin,
                    CustomerId = dbUser.CustomerId
                });
            }
            return (false, "היוזר אינו פעיל", null);
        }
        public bool EditUser(UserEntity user)
        {
            try
            {
                var dbUser = context.Users.Find(user.Id);
                if (dbUser != null)
                {
                    dbUser.CustomerId = user.CustomerId;
                    dbUser.UserEmail = user.Email;
                    dbUser.UserFirstName = user.FirstName;
                    dbUser.UserId = user.Id;
                    dbUser.UserIsActive = user.IsActive;
                    dbUser.UserLastName = user.LastName;
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
