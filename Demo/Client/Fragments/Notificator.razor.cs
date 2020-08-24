using RemoteController;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Client.Fragments
{
    public partial class Notificator: RemoteControllerBase
    {
        public List<ClientActionInfo> _info = new List<ClientActionInfo>();

        public void AddInfo(NotificatorControllerEvent @event)
        {
            _info.Add(new ClientActionInfo(@event));
            StateHasChanged();
        }
    }
}
