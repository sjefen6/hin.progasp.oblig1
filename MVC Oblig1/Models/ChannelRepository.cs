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
                if ((ch.Closed == false && p == null) || p.Level == 0)
                {
                    p = new Permission();
                    p.UserId = user.UserId;
                    p.ChannelName = channelName;
                    p.Level = level;

                    db.Permissions.InsertOnSubmit(p);
                    db.SubmitChanges();
                }
            } catch (NullReferenceException e) {

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

        internal bool isClosed(string id)
        {
            if (true) 
            {
                //ruben er tøff
            }
            throw new NotImplementedException();
        }
    }
}