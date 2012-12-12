using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Oblig1.Models
{
    public class ChannelRepository
    {
        private ChannelDataClassesDataContext db;
        public UserRepository userDB { get; private set; }

        public ChannelRepository()
        {
            db = new ChannelDataClassesDataContext();
            userDB = new UserRepository(this, db);
        }

        private Channel getChannel(String channelName)
        {
            return (from c in db.Channels
                    where c.Name == channelName
                    select c).FirstOrDefault();
        }

        public IQueryable<Channel> showAllChannels()
        {
            return db.Channels;
        }

        public IQueryable<Message> getAllMessages(String channelName)
        {
            return (from m in db.Messages
                    where m.ChannelName == channelName
                    orderby m.id descending
                    select m);
        }

        public void join(Channel joinChannel, string username)
        {
            if (getChannel(joinChannel.Name) == null)
            {
                create(joinChannel, username);
            }
            else
            {
                join(joinChannel.Name, username, 1);
            }
        }

        public void invite(string channelName, string username)
        {
            join(channelName, username, 0);
        }

        private void join(string channelName, string username, int level)
        {
            aspnet_User user = userDB.getUser(username);

            Permission p = (from q in db.Permissions
                            where q.ChannelName == channelName
                            && q.UserId == userDB.getUser(username).UserId
                            select q).FirstOrDefault();
            Channel ch = getChannel(channelName);

            try
            {
                if ((ch.Closed == false && p == null) || level == 0 || p.Level == 0)
                {
                    p = new Permission();
                    p.UserId = user.UserId;
                    p.ChannelName = channelName;
                    p.Level = level;

                    db.Permissions.InsertOnSubmit(p);
                    db.SubmitChanges();
                }
            }
            catch (NullReferenceException e)
            {

                throw new NotInvitedException();
            }
        }

        private void create(Channel joinChannel, string username)
        {
            joinChannel.Closed = false;
            db.Channels.InsertOnSubmit(joinChannel);
            db.SubmitChanges();
            join(joinChannel.Name, username, 3);
        }

        public void sendMessage(Message newMessage, string username)
        {
            newMessage.Timestamp = DateTime.Now;
            newMessage.UserId = userDB.getUser(username).UserId;
            db.Messages.InsertOnSubmit(newMessage);
            db.SubmitChanges();
        }

        public IQueryable<Permission> getAllPermissions(string channelName)
        {
            return (from p in db.Permissions
                    where p.ChannelName == channelName
                    select p);
        }

        public IQueryable<Permission> showAllJoinedChannels(string username)
        {
            return (from p in db.Permissions
                    where p.UserId == userDB.getUser(username).UserId
                    & p.Level > 0
                    select p);
        }

        public IQueryable<aspnet_User> showAllDMChannels(string username)
        {
            IQueryable<Message> messages = from m in db.Messages
                    where (m.UserId == userDB.getUser(username).UserId &&
                                               m.Receiver != null) ||
                    m.Receiver == userDB.getUser(username).UserId
                    select m;

            List<Guid> userIDs = new List<Guid>();

            foreach (Message m in messages)
            {
                if (m.UserId == userDB.getUser(username).UserId)
                {
                    userIDs.Add((Guid) m.Receiver);
                }
                else
                {
                    userIDs.Add(m.UserId);
                }
            }

            userIDs = userIDs.Distinct().ToList<Guid>();

            List<aspnet_User> users = new List<aspnet_User>();

            foreach(Guid id in userIDs){
                    users.Add((from u in db.aspnet_Users
                    where u.UserId == id 
                    select u).First<aspnet_User>());
            }
            
            return users.AsQueryable();
        }

        public Permission getPermission(string channelName, string username)
        {
            try
            {
                return (from p in db.Permissions
                        where p.UserId == userDB.getUser(username).UserId
                        && p.ChannelName == channelName
                        select p).First();
            }
            catch
            {
                return null;
            }
        }

        public Boolean isOp(string channelName, string username)
        {
            try
            {
                Permission p = getPermission(channelName, username);
                Boolean b = getPermission(channelName, username).Level >= 2;
                return b;
            }
            catch
            {
                return false;
            }
        }

        public Boolean isJoined(string channelName, string username)
        {
            try
            {
                return getPermission(channelName, username).Level >= 1;
            }
            catch
            {
                return false;
            }
        }

        public void updatePermission(Permission p)
        {
            db.SubmitChanges();
        }

        public bool isClosed(string channelName)
        {
            Channel ch = getChannel(channelName);
            return ch.Closed ?? false; // ?? false = false if null
        }

        public IQueryable<Message> getAllMessages(string user1, string user2)
        {
            return (from m in db.Messages
                    where (m.UserId == userDB.getUser(user1).UserId &&
                    m.Receiver == userDB.getUser(user2).UserId) ||
                    (m.UserId == userDB.getUser(user2).UserId &&
                    m.Receiver == userDB.getUser(user1).UserId)
                    orderby m.id descending
                    select m);
        }
    }
}