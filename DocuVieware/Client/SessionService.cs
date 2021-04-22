using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocuVieware.Client
{
    public class SessionService
    {
        public SessionService()
        {
            SessionId = Guid.NewGuid().ToString();
            Key = SessionId.Split("-")[^1];
        }
        public string SessionId { get; }
        public string Key { get; }

    }
}
