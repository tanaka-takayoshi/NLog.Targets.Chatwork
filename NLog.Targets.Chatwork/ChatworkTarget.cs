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

        private ChatworkClient client;
            
        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            if (string.IsNullOrEmpty(Token))
            {
                InternalLogger.Fatal("認証Tokenを指定してください");
                return;;
            }

            client = new ChatworkClient(Token)
            {
                Timeout = TimeSpan.FromSeconds(Timeout ?? 3)
            };
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var logMessage = Layout.Render(logEvent);

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
