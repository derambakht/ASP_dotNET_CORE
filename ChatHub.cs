using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRSample
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        //register step by step
        //https://github.com/aspnet/Docs/blob/master/aspnetcore/tutorials/signalr.md
        //********************************************

        //startup.cs
        //********************************************
        
        //configureServices
        //services.AddSignalR();
        
        //configure
        //app.UseSignalR(routes =>
        //    {
        //        routes.MapHub<ChatHub>("/chatHub");
        //    });


        //send message from controller to clients
        //********************************************

        //IHubContext<ChatHub> _hubcontext;
        //public AdminController(IHubContext<ChatHub> hubcontext)
        //{
        //    _hubcontext = hubcontext;
        //}

        //public async Task<IActionResult> SendMessage(string message)
        //{
        //    await _hubcontext.Clients.All.SendAsync("ReceiveMessage", "admin", message);

        //    return View();
        //}
    }
}
