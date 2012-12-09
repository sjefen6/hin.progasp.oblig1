using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVC_Oblig1.Models;

namespace MVC_Oblig1.Models
{
    public class UserRepository
    {
        private ChannelDataClassesDataContext db;
        private ChannelRepository owner;

        public UserRepository(ChannelRepository owner, ChannelDataClassesDataContext db)
        {
            this.owner = owner;
            this.db = db;
        }

        public aspnet_User getUser(string username)
        {
            return db.aspnet_Users.SingleOrDefault(usr => usr.UserName == username);
        }

        public aspnet_User getUser(Guid userID)
        {
            return db.aspnet_Users.SingleOrDefault(usr => usr.UserId == userID);
        }

        public List<aspnet_User> getAllUsers()
        {
            return db.aspnet_Users.ToList();
        }
    }
}
