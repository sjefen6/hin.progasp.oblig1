using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MVC_Oblig1.Models
{
    public class MessageSendDM
    {
        private ChannelRepository channelRepository;

        public Message Message;

        [Required]
        public SelectList Receivers { get; private set;}

        

        public MessageSendDM(Message Message)
        {
            channelRepository = new ChannelRepository();
            this.Message = Message;
            if (Message.Receiver.HasValue)
            {
                Receivers = new SelectList(channelRepository.userDB.getAllUsers(), "UserID", "UserName", channelRepository.userDB.getUser((Guid)Message.Receiver).UserName);
            }
            else
            {
                Receivers = new SelectList(channelRepository.userDB.getAllUsers(), "UserID", "UserName");
            }
        }
    }

    public class MessageFormViewModel
    {
        private ChannelRepository channelRepository;

        public Message Message;
        public string ProfileImage;
        public string Username;
        public AttachmentModel Attachment;

        public MessageFormViewModel(Message Message)
        {
            channelRepository = new ChannelRepository();
            this.Message = Message;
            ProfileImage = getGravatar(channelRepository.userDB.getUser(Message.UserId).aspnet_Membership.Email);
            Username = channelRepository.userDB.getUser(Message.UserId).UserName;
            Attachment = null;
        }

        private string getGravatar(string email)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return string.Format("http://www.gravatar.com/avatar/{0}?d=mm", sBuilder.ToString());
        }
    }
}
