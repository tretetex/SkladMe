using System;
using SkDAL.Model;

namespace SkladMe.API.Methods
{
    public class Users
    {
        private SkladchikApiClient _skladchikApiClient;

        public Users(SkladchikApiClient skladchikApiClient)
        {
            _skladchikApiClient = skladchikApiClient;
        }

        public User Get()
        {
            throw new NotImplementedException();
        }
    }
}
