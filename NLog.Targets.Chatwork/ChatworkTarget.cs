using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
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

        [DefaultValue(1)]
        public int MaxRetry { get; set; }

        private ChatworkClient client;

        private Task<string> asyncTask;


        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            if (string.IsNullOrEmpty(Token))
            {
                InternalLogger.Fatal("認証Tokenを指定してください");

                return;
            }
            if (MaxRetry < 1)
            {
                InternalLogger.Fatal("MaxRetry は1以上の値を指定してください。1を指定します。");
                MaxRetry = 1;
            }

            client = new ChatworkClient(Token)
            {
                Timeout = TimeSpan.FromSeconds(Timeout ?? 3)
            };

            asyncTask = GetHeaderAsync();
        }

        private async Task<string> GetHeaderAsync()
        {
            if (string.IsNullOrEmpty(ToMembers))
            {
                return "";
            }
            else
            {
                try
                {
                    var contacts = await client.Room.GetRoomMembersAsync(RoomId).ConfigureAwait(false);
                    if (!ToMembers.Equals("all", StringComparison.InvariantCultureIgnoreCase))
                    {
                        contacts = contacts.Where(c => ToMembers.Contains(c.account_id.ToString())).ToList();
                    }
                    return string.Join("\r",
                            contacts.Select(c => $"[To:{c.account_id}] {c.name}さん"))
                            + "\r\r";
                }
                catch (Exception e)
                {
                    InternalLogger.Fatal("Failed to load room members. Use id only: {0}", e);
                    if (ToMembers.Equals("all", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return "";
                    }
                    else
                    {
                        return string.Join("\r",
                                ToMembers.Split(',').Select(id => $"[To:{id}] "))
                            + "\r\r";
                    }
                }
            }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var toHeader = asyncTask.Result;
            var logMessage = toHeader + Layout.Render(logEvent);

            Exception lastError = null;
            for (var i = 0; i < MaxRetry; i++)
            {
                try
                {
                    var res = client.Room.SendMessgesAsync(RoomId, logMessage).Result;
                    InternalLogger.Debug("送信したメッセージID: {0}", res.message_id);
                    return;
                }
                catch (Exception e)
                {
                    lastError = e;
                }
                Task.Delay(500).Wait();
            }
            InternalLogger.Fatal(lastError?.ToString() ?? "");
        }
    }

}
