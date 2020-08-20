using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
    public class RemoteControllerBase : ComponentBase
    {
        [Parameter]
        public byte ID { get; set; }     
    }
}
