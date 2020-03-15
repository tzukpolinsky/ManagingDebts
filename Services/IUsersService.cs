using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IUsersService
    {
        bool Create(UserEntity user);
        bool EditUser(UserEntity user);
        (bool isSuccess, string msg) ChangePassword(UserEntity[] users);
        UserEntity[] GetAll();
        (bool isSuccess, string msg, UserEntity user) Login(UserEntity user);
        UserEntity GetById(UserEntity user);
    }
}
