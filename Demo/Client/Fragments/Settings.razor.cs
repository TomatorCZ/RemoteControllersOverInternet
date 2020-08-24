using Microsoft.AspNetCore.Components;
using RemoteController;
using System;
using System.Threading.Tasks;

namespace Client.Fragments
{
    public partial class Settings : ComponentBase
    {
        private TextBoxControllerEvent _textBoxEvent = null;

        [Parameter]
        public EventCallback<TextBoxControllerEvent> OnNameSetCallback { get; set; }

        public void OnTextChanged(TextBoxControllerEvent @event)
        {
            _textBoxEvent = @event;
        }

        public async Task OnSubmit(ButtonControllerEvent @event)
        {
            if (_textBoxEvent != null && !String.IsNullOrEmpty(_textBoxEvent.Text) && @event.Event == ButtonEvent.Click)
            {
                await OnNameSetCallback.InvokeAsync(_textBoxEvent);
            }
        }
    }
}
