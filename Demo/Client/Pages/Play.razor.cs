using Microsoft.AspNetCore.Components;
using RemoteController;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Client.Pages
{
    public partial class Play : ComponentBase
    {
        private bool isNameChose = false;

        private async Task OnNameChose(TextBoxControllerEvent @event)
        {
            isNameChose = true;
            StateHasChanged();

            while (!_user.IsConnected)
                await Task.Delay(1000);
            
            await _user.SendAsync(@event);

            ConfigurationMessage msg = new ConfigurationMessage();
            msg.AddBinding(1, typeof(ButtonControllerEvent));
            msg.AddBinding(2, typeof(ButtonControllerEvent));
            msg.AddBinding(3, typeof(ButtonControllerEvent));
            await _user.SendAsync(msg);
        }

        private async Task OnMove(ButtonControllerEvent @event)
        {
            if (_user.IsConnected)
                await _user.SendAsync(@event);
        }

        protected override async Task OnInitializedAsync()
        {
            await _user.ConnectAsync(new Uri(@"ws://localhost:5001/ws"));
            await _user.SendAsync(new InitialMessage());

            ConfigurationMessage msg = new ConfigurationMessage();
            msg.AddBinding(1, typeof(TextBoxControllerEvent));
            await _user.SendAsync(msg);
        }


    }
}
