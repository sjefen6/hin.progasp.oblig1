using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Text;
using MVC_Oblig1.Controllers;
using System.Web.Script.Serialization;

namespace MVC_Oblig1.Models
{
    [Serializable]
    struct SMS
    {
        public int id;
        public string SND;
        public string RCV;
        public string TXT;
        public string date;
        public string status;

        public string getString()
        {
            return "SND=" + SND + "&RCV=" + RCV + "&TXT=" + TXT;
        }
    }

    public class PswinRepository
    {
        String apiurl = "http://malmen.hin.no/pswin/SMS";

        ChannelRepository chRep;

        public PswinRepository()
        {
             chRep = new ChannelRepository();
        }

        public List<Message> getMessageToChannel(string ChannelName)
        {

            return null;
        }

        private SMS getFromCodeword(string codeword)
        {
            WebRequest req = HttpWebRequest.Create(apiurl + "/ChatInCode/" + codeword);
            WebResponse res = req.GetResponse();

            StreamReader sr = new StreamReader(res.GetResponseStream());
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            return javaScriptSerializer.Deserialize<SMS>(sr.ReadToEnd());
        }

        public void sendSMS(string message, string reciver)
        {
            SMS sms = new SMS();
            sms.RCV = reciver;
            sms.TXT = message;
            sms.SND = "26112";

            // Create a request using a URL that can receive a post. 
            WebRequest request = HttpWebRequest.Create(apiurl + "/sendsms/");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = sms.getString();
            byte[] byteArray = Encoding.UTF8.GetBytes (postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream ();
            // Write the data to the request stream.
            dataStream.Write (byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close ();
            // Get the response.
            WebResponse response = request.GetResponse ();
            // Display the status.
            Console.WriteLine (((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream ();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader (dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd ();
            // Display the content.
            Console.WriteLine (responseFromServer);
            // Clean up the streams.
            reader.Close ();
            dataStream.Close ();
            response.Close ();
        }

        internal void lookupUserMessages()
        {
            foreach (aspnet_User reciver in chRep.userDB.getAllUsers())
            {
                SMS sms = getFromCodeword("d " + reciver.UserName);
                if (sms.status == "received")
                {
                    foreach (aspnet_User sender in chRep.userDB.getAllUsers())
                    {
                        if (sender.MobileAlias == sms.SND)
                        {
                            Message m = new Message();
                            m.Receiver = reciver.UserId;
                            m.Content = sms.TXT;
                            chRep.sendMessage(m, sender.UserName);
                        }
                    }
                }
            }

            foreach (Channel c in chRep.showAllChannels())
            {
                SMS sms = getFromCodeword("c " + c.Name);
                if (sms.status == "received")
                {
                    foreach (aspnet_User sender in chRep.userDB.getAllUsers())
                    {
                        if (sender.MobileAlias == sms.SND)
                        {
                            if (sms.TXT == "c " + c.Name)
                            {
                                Message m = chRep.getAllMessages(c.Name).ToList().Last();
                                if (m != null)
                                {
                                    sendSMS(chRep.userDB.getUser(m.UserId).UserName + ": " + m.Content, sender.MobileAlias);
                                }
                            }
                            else
                            {
                                Message m = new Message();
                                m.ChannelName = c.Name;
                                m.Content = sms.TXT;
                                chRep.sendMessage(m, sender.UserName);
                            }
                        }
                    }
                }
            }
        }
    }
}
