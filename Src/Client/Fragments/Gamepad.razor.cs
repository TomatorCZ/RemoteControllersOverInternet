using Microsoft.AspNetCore.Components;
using RemoteController;
using System.Threading.Tasks;

namespace Client.Fragments
{
    public partial class Gamepad : ComponentBase
    {
        [Parameter]
        public EventCallback<ButtonControllerEvent> OnMoveCallback { get; set; }

        public async Task OnButtonChanged(ButtonControllerEvent @event)
        {
            await OnMoveCallback.InvokeAsync(@event);
        }
    }
}
