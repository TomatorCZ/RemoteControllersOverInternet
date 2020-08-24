using Microsoft.AspNetCore.Components;
using RemoteController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Pages
{
    public partial class Watch:ComponentBase 
    {
        Client.Fragments.Notificator child;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await _user.ConnectAsync(new Uri(@"ws://localhost:5001/ws"));
            }
            catch (Exception)
            {

                return;
            }
            
            await _user.SendAsync(new InitialMessage());

            ConfigurationMessage msg = new ConfigurationMessage();
            msg.AddBinding(1, typeof(NotificatorControllerEvent));
            await _user.SendAsync(msg);
            await _user.SendAsync(new NotificatorControllerEvent("notifcator","non",1));

            while (_user.IsConnected)
            {
                HandleMessage(await _user.ReceiveAsync());
            }
        }

        private void HandleMessage(ControllerEvent @event)
        {
            if (@event is NotificatorControllerEvent notificator)
            {
                child.AddInfo(notificator);
                StateHasChanged();
            }
        }
    }
}
