using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Collections.Concurrent;
using System.Diagnostics;
using PixelCMS.Core.Models;
using PixelCMS.Mailer;
using SignalRChat.Common;


namespace PixelCMS.Web.LCSK
{
    public class ChatHub : Hub
    {
        private const string CONFIG_FILE = "lcsk.dat";
        /*
        List<Agent> Agents
        {
            get
            {
                if (HttpContext.Current.Cache["lcsk_agents"] != null)
                    return (List<Agent>)HttpContext.Current.Cache["lcsk_agents"];
                else
                {
                    List<Agent> agents = new List<Agent>();
                    HttpContext.Current.Cache.Add(
                        "lcsk_agents",
                        agents,
                        null,
                        DateTime.Now.AddHours(1),
                        TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Default, null);
                    return agents;
                }
            }
        }
        */
        private static ConcurrentDictionary<string, Agent> Agents;
        private static ConcurrentDictionary<string, string> ChatSessions;
        pixelcmsEntities db=new pixelcmsEntities();
       // static List<UserDetail> ConnectedUsers = new List<UserDetail>();

        // mess liên hệ và đơn hàng 
        public void message(string mess)
        {
            if (mess == "order")
            {
                Clients.All.MessReceivedOrder(mess);
            }
            else if (mess == "contact")
            {
                Clients.All.MessReceived(mess);
            }
        }

        public void SendPrivateMessage(string toUserId, string message)
        {

            string fromUserId = Context.ConnectionId;

            var toUser = Agents.Values.FirstOrDefault(x => x.Id == toUserId);
            var fromUser = Agents.Values.FirstOrDefault(x => x.Id == fromUserId);

            if (toUser != null && fromUser != null)
            {
                // send to 
                Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.Name, message);

                // send to caller user
                Clients.Caller.sendPrivateMessage(toUserId, fromUser.Name, message);
            }
        }

        public void AgentConnect(string name, string pass)
        {
            var id = Context.ConnectionId;

            if (Agents == null)
                Agents = new ConcurrentDictionary<string, Agent>();

            if (ChatSessions == null)
                ChatSessions = new ConcurrentDictionary<string, string>();

            string hashPass = ToHash(pass);

            var config = GetConfig();
            if (config == null || config.Length < 2)
            {
                Clients.Caller.loginResult(false, "config", "");
            }
            else if ((config[0] == hashPass) || (config[1] == hashPass))
            {
                var agent = new Agent()
                {
                    Id = Context.ConnectionId,
                    Name = name,
                    IsOnline = true
                };
                //remove user dang online
                var check = Agents.Values.Any(x => x.Name == name);
                if (check)
                {
                   Agent tmp = null;
                    Agents.TryRemove(name, out tmp );
                }
                if (Agents.TryAdd(name, agent))
                {

                    Clients.Caller.loginResult(true, agent.Id, agent.Name);

                    Clients.All.onlineStatus(Agents.Count(x => x.Value.IsOnline) > 0);

                    //edit by luan
                    Clients.Caller.onConnected(id, name, Agents.Values);
                    Clients.AllExcept(id).onNewUserConnected(id, name);
                }
                else
                {
                    Clients.Caller.loginResult(false, "error", "");
                }
            }
            else
                Clients.Caller.loginResult(false, "pass", "");
        }

        public void ChangeStatus(bool online)
        {
            var agent = Agents.SingleOrDefault(x => x.Value.Id == Context.ConnectionId).Value;
            if (agent != null)
            {
                agent.IsOnline = online;

                // TODO: Check if the agent was in chat sessions.

                Clients.All.onlineStatus(Agents.Count(x => x.Value.IsOnline) > 0);
            }
        }

        public void EngageVisitor(string connectionId)
        {
            var agent = Agents.SingleOrDefault(x => x.Value.Id == Context.ConnectionId).Value;
            if(agent != null)
            {
                ChatSessions.TryAdd(connectionId, agent.Id);

                Clients.Caller.newChat(connectionId);

                Clients.Client(connectionId).setChat(connectionId, agent.Name, false);

                Clients.Caller.addMessage(connectionId, "system", "Bạn đã mời khách đang truy cập vào trò chuyện...");
                Clients.Client(connectionId).addMessage(agent.Name, "Hi.Tôi là " + agent.Name + " cho tôi biết nếu bạn có bất kỳ câu hỏi nào.");
            }
        }

        public void LogVisit(string page, string referrer, string existingChatId)
        {
            if (Agents == null)
                Agents = new ConcurrentDictionary<string, Agent>();

            Clients.Caller.onlineStatus(Agents.Count(x => x.Value.IsOnline) > 0);

            if (!string.IsNullOrEmpty(existingChatId) &&
                ChatSessions.ContainsKey(existingChatId))
            {
                var agentId = ChatSessions[existingChatId];
                Clients.Client(agentId).visitorSwitchPage(existingChatId, Context.ConnectionId, page);

                var agent = Agents.SingleOrDefault(x => x.Value.Id == agentId).Value;

                if (agent != null)
                    Clients.Caller.setChat(Context.ConnectionId, agent.Name, true);

                string buffer = "";
                ChatSessions.TryRemove(existingChatId, out buffer);

                ChatSessions.TryAdd(Context.ConnectionId, agentId);
            }

            foreach (var agent in Agents)
            {
                var chatWith = (from c in ChatSessions
                               join a in Agents on c.Value equals a.Value.Id
                               where c.Key == Context.ConnectionId
                               select a.Value.Name).SingleOrDefault();

                Clients.Client(agent.Value.Id).newVisit(page, referrer, chatWith, Context.ConnectionId);
            }
        }

        public void RequestChat(string message)
        {
            // We assign the chat to the less buzy agent
            var workload = from a in Agents
                           where a.Value.IsOnline
                           select new
                           {
                               Id = a.Value.Id,
                               Name = a.Value.Name,
                               Count = ChatSessions.Count(x => x.Value == a.Value.Id)
                           };

            if (workload == null)
            {
                Clients.Caller.addMessage("", "No agent are currently available.");
                return;
            }

            var lessBuzy = workload.OrderBy(x => x.Count).FirstOrDefault();

            if (lessBuzy == null)
            {
                Clients.Caller.addMessage("", "Không có Quản trị viên nào.");
                return;
            }
            
            ChatSessions.TryAdd(Context.ConnectionId, lessBuzy.Id);

            Clients.Client(lessBuzy.Id).newChat(Context.ConnectionId);

            Clients.Caller.setChat(Context.ConnectionId, lessBuzy.Name, false);

            Clients.Client(lessBuzy.Id).addMessage(Context.ConnectionId, "visitor", message);
            Clients.Caller.addMessage("me", message);
        }

        public void Transfer(string connectionId, string agentName, string messages)
        {
            if(!Agents.ContainsKey(agentName))
            {
                Clients.Caller.addMessage(Context.ConnectionId, "system", "Người quản lý này không tồn tại: " + agentName);
                return;
            }

            var agent = Agents[agentName];
            if(!agent.IsOnline)
            {
                Clients.Caller.addMessage(Context.ConnectionId, "system", agentName + " không online.");
                return;
            }

            if(!ChatSessions.ContainsKey(connectionId))
            {
                Clients.Caller.addMessage(Context.ConnectionId, "system", "Phiên chat này không tồn tại nữa.");
                return;
            }

            string currentAgentId = "";
            if (ChatSessions.TryRemove(connectionId, out currentAgentId) &&
                ChatSessions.TryAdd(connectionId, agent.Id))
            {
                Clients.Client(agent.Id).newChat(connectionId);
                Clients.Client(agent.Id).addMessage(connectionId, "system", "Trò mới chuyện chuyển giao cho bạn.");
                Clients.Client(agent.Id).addMessage(connectionId, ">>", "Bắt đầu cuộc trò chuyện trước đây");
                Clients.Client(agent.Id).addMessage("", messages);
                Clients.Client(agent.Id).addMessage(connectionId, "<<", "Kết thúc cuộc trò chuyện trước đây");

                Clients.Client(connectionId).addMessage("", "Bạn đã được chuyển giao cho " + agent.Name);
                Clients.Client(connectionId).setChat(connectionId, agent.Name, true);

                Clients.Caller.addMessage(connectionId, "system", "Trò chuyện chuyển giao cho" + agentName);
            }
        }

        public void Send(string data)
        {
            Clients.Caller.addMessage("me", data);

            if (ChatSessions.ContainsKey(Context.ConnectionId))
            {
                var opId = ChatSessions[Context.ConnectionId];
                Clients.Client(opId).addMessage(Context.ConnectionId, "visitor", data);
            }
            else
            {
                Debug.WriteLine("Phiên chat không tìm thấy.");

                // refactor this
                var workload = from a in Agents
                               where a.Value.IsOnline
                               select new
                               {
                                   Id = a.Value.Id,
                                   Name = a.Value.Name,
                                   Count = ChatSessions.Count(x => x.Value == a.Value.Id)
                               };

                if (workload == null)
                {
                    Clients.Caller.addMessage("", "No agent are currently available.");
                    return;
                }

                var lessBuzy = workload.OrderBy(x => x.Count).FirstOrDefault();

                if (lessBuzy == null)
                {
                    Clients.Caller.addMessage("", "Không có người quản lý nào.");
                    return;
                }

                ChatSessions.TryAdd(Context.ConnectionId, lessBuzy.Id);

                Clients.Client(lessBuzy.Id).newChat(Context.ConnectionId);

                Clients.Caller.setChat(Context.ConnectionId, lessBuzy.Name, false);

                Clients.Client(lessBuzy.Id).addMessage(Context.ConnectionId, "system", "Khách này dường như đã mất phiên trò chuyện của họ.");
                Clients.Client(lessBuzy.Id).addMessage(Context.ConnectionId, "visitor", data);
            }
        }

        public void OpSend(string id, string data)
        {
            var agent = Agents.SingleOrDefault(x => x.Value.Id == Context.ConnectionId).Value;
            if (agent == null)
            {
                Clients.Caller.addMessage(id, "system", "Chúng tôi không thể gửi tin nhắn của bạn, xin vui lòng tải lại trang web.");
                return;
            }

            if (id == "internal")
            {
                foreach (var a in Agents.Where(x => x.Value.IsOnline))
                    Clients.Client(a.Value.Id).addMessage(id, agent.Name, data);
                        
            }
            else if (ChatSessions.ContainsKey(id))
            {
                Clients.Caller.addMessage(id, "you", data);
                Clients.Client(id).addMessage(agent.Name, data);
            }
        }

        public void CloseChat(string id)
        {
            if (ChatSessions.ContainsKey(id))
            {
                Clients.Client(id).addMessage("", "Quản trị viên đóng cửa phiên chat.");

                string buffer = "";
                ChatSessions.TryRemove(id, out buffer);
            }
        }

        public void LeaveChat(string id)
        {
            // was it an agent
            var agent = Agents.SingleOrDefault(x => x.Value.Id == id).Value;
            if (agent != null)
            {
                Agent tmp = null;
                if (Agents.TryRemove(agent.Name, out tmp))
                {

                    var sessions = ChatSessions.Where(x => x.Value == agent.Id);
                    if (sessions != null)
                    {
                        foreach (var session in sessions)
                            Clients.Client(session.Key).addMessage("", "Quản trị viên đã bị ngắt kết nối.");
                    }

                    Clients.All.updateStatus(Agents.Count(x => x.Value.IsOnline) > 0);
                }
            }

            // was it a visitor
            if (ChatSessions!=null&&ChatSessions.ContainsKey(id))
            {
                var agentId = ChatSessions[id];
                Clients.Client(agentId).addMessage(id, "system", "khách truy cập đóng kết nối.");
            }
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            if (Agents != null)
            {
                var item = Agents.Values.FirstOrDefault(x => x.Id == Context.ConnectionId);
                if (item != null)
                {
                    Clients.All.onUserDisconnected(Context.ConnectionId, item.Name);
                }

                LeaveChat(Context.ConnectionId);
                
            }
            return Clients.All.leave(Context.ConnectionId); 
        }

        public void SendEmail(string from, string message)
        {
           /* var msg = new MailMessage();
            msg.To.Add(new MailAddress(from));
            msg.Subject = "LCSK - Offline Contact";
            msg.Body = "You received an offline contact from your LCSK chat widget.\r\n\r\n" + message;*/
            /* using (var client = new SmtpClient())
           {
               client.Send(msg);
           }*/

            var email = new Email();

            email.DateSend = DateTime.Now;
            email.Email1 = from;
            email.Text =message;
            email.Subject = "Liên hệ từ cửa sổ chat";
            db.Emails.Add(email);

            //lấy email lưu vào EmailSubscribe
            var EmailSubscribe = new EmailSubscribe();
            EmailSubscribe.Email = email.Email1;
            EmailSubscribe.Active = true;
            EmailSubscribe.Date = DateTime.Now;
            //add role là member cho EmailSubscribe
            var role = db.webpages_Roles.FirstOrDefault(x => x.RoleName == "member");
            if (role != null)
                EmailSubscribe.webpages_Roles.Add(role);

            //check EmailSubscribes đã tồn tại hay chưa nếu đã tồn tại ko lưu nữa.//cái này lưu để sử dụng cho mail marketing
            var IsExist = db.EmailSubscribes.Any(x => x.Email == email.Email1);
            if (!IsExist)
                db.EmailSubscribes.Add(EmailSubscribe);
            //end add mail

            db.SaveChanges();

            UserMailer _UserMailer = new UserMailer();
            var emails = db.Options.FirstOrDefault(s => s.Code == "email-mail");
            var emailsetting = "luanpb@thekymoi.com";
            if (emails != null)
            {
                emailsetting = emails.Value;
            }
            _UserMailer.SendMailContact(emailsetting, email.Username, email.Email1, email.Subject, email.Text, email.Email1, email.Phone, email.Address);
           
        }

        #region Install and config methods
        public void getInstallState()
        {
            var config = GetConfig();

            if (config != null && config.Length >= 2)
                Clients.Caller.installState(true, config[0]);
            else
                Clients.Caller.installState(false, "lcskv2hctemptoken");
        }

        public void AdminRequest(string pass)
        {
            var config = GetConfig();

            if (config != null && config.Length >= 2)
            {
                if (config[0] == ToHash(pass))
                    Clients.Caller.adminResult(true, config[0]);
                else
                    Clients.Caller.adminResult(false, "");
            }
            else
                Clients.Caller.adminResult(false, "");
        }

        public void SetConfig(string token, string adminPass, string agentPass)
        {
            bool shouldSave = false;
            var config = GetConfig();

            if (config != null && config.Length >= 2)
            {
                if (config[0] == token)
                    shouldSave = true;
            }
            if (token == "lcskv2hctemptoken")
                shouldSave = true;

            if (shouldSave)
            {
                string configPath = HttpContext.Current.Server.MapPath("~/App_Data/" + CONFIG_FILE);

                File.WriteAllText(
                    configPath,
                    ToHash(adminPass) + "\n" + ToHash(agentPass));

                Clients.Caller.setConfigResult(true, "Config file updated.");
            }
            else
                Clients.Caller.setConfigResult(false, "Unable to save the config file.");
        }

        private string[] GetConfig()
        {
            string configPath = HttpContext.Current.Server.MapPath("~/App_Data/" + CONFIG_FILE);
            if (File.Exists(configPath))
            {
                return File.ReadAllLines(configPath);
            }
            return null;
        }

        public string ToHash(string password)
        {
            if (string.IsNullOrEmpty(password))
                return "";

            var provider = new SHA1CryptoServiceProvider();
            var encoding = new UnicodeEncoding();
            return Convert.ToBase64String(provider.ComputeHash(encoding.GetBytes(password)));
        }
        #endregion
    }
}