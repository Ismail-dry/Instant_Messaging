using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Instant_Messaging;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;
using Owin;

[assembly: OwinStartup(typeof(Instant_Messaging.Startup))]

namespace Instant_Messaging
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}