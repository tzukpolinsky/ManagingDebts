using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IMailService
    {
        bool SendErrorEmail(UserEntity user);
        bool SendAccessDeniedEmail(UserEntity user);
        bool SendSystemSabotage(UserEntity user, string reasonExplanation);
    }
}
