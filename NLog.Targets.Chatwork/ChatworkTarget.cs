using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chatwork.Service;
using NLog.Common;

namespace NLog.Targets.Chatwork
{
    [Target("ChatworkTarget")]
    public class ChatworkTarget : TargetWithLayout
    {
        public string Token { get; set; }
        public int RoomId { get; set; }

        [DefaultValue(3)]
        public int? Timeout { get; set; }

        public string ToMembers { get; set; }

        private ChatworkClient client;

        private string ToHeader = "";
         
        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            if (string.IsNullOrEmpty(Token))
            {
                InternalLogger.Fatal("認証Tokenを指定してください");
                return;
            }

            client = new ChatworkClient(Token)
            {
                Timeout = TimeSpan.FromSeconds(Timeout ?? 3)
            };

            if (!string.IsNullOrEmpty(ToMembers))
            {
                var contacts = client.Room.GetRoomMembersAsync(RoomId).Result;
                foreach (var idText in ToMembers.Split(','))
                {
                    int id;
                    if (int.TryParse(idText.Trim(), out id))
                    {
                        var contact = contacts.FirstOrDefault(c => c.account_id == id);
                        if (contact != null)
                        {
                            ToHeader += string.Format("[To:{0}] {1}さん\r\n", id, contact.name);
                        }
                        else
                        {
                            InternalLogger.Warn("Id {0} は指定されたルームID {1}に存在しません", id, RoomId);
                        }
                    }
                }
            }
            
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var logMessage = ToHeader + Layout.Render(logEvent);

            try
            {
                var res = client.Room.SendMessgesAsync(RoomId, logMessage).Result;
                InternalLogger.Debug("送信したメッセージID: {0}", res.message_id);
            }
            catch (Exception e)
            {
                InternalLogger.Fatal(e.ToString());
            }
        }
    }
}
