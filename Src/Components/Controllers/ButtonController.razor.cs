using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteController
{
    public partial class ButtonController : RemoteControllerBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback<ButtonControllerEvent> ClickCallback { get; set; }

        [Parameter]
        public EventCallback<ButtonControllerEvent> MouseUpCallback { get; set; }
        
        [Parameter]
        public EventCallback<ButtonControllerEvent> MouseDownCallback { get; set; }

        #region Events
        protected async Task OnClick() => await ClickCallback.InvokeAsync(new ButtonControllerEvent(ID, ButtonEvent.Click));

        protected async Task OnMouseDown() => await MouseDownCallback.InvokeAsync(new ButtonControllerEvent(ID, ButtonEvent.MouseDown));

        protected async Task OnMouseUp() => await MouseUpCallback.InvokeAsync(new ButtonControllerEvent(ID, ButtonEvent.MouseUp));
        #endregion

    }
}
