using RemoteController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client
{
    public struct ClientActionInfo
    {
        public string Name { get; }
        public string Button{ get; }

        public ClientActionInfo(NotificatorControllerEvent @event)
        {
            Name = @event.Name;
            Button = @event.Button;
        }
        
    }
}
