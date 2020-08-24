using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Client.Fragments
{
    public partial class TextBox : RemoteController.RemoteControllerBase
    {
        public string _value = null;

        [Parameter]
        public EventCallback<TextBoxControllerEvent> ValueChanged { get; set; }

        private Task OnValueChanged(ChangeEventArgs e)
        {
            _value = e.Value.ToString();
            return ValueChanged.InvokeAsync(new TextBoxControllerEvent(e.Value.ToString(), ID));
        }
    }
}
