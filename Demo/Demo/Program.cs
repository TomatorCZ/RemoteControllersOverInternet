using System.Net.WebSockets;
using System.Threading.Tasks;
using Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RemoteController;
using Microsoft.AspNetCore.Hosting;

namespace Demo
{
    class Program
    {         
        static async Task Main(string[] args)
        {
            Game game = new Game();
            await game.RunAsync();
        }
    }

    public class Game
    {
        #region Classes,Enums
        enum EntityType { Unknown, Watcher, Player, }

        class MyPlayer : Player
        {
            public EntityType Entity {get;set;}
            public string Name { get; set; }
            public MyPlayer(WebSocket socket) : base(socket){}
        }

        class MyPlayerFactory : IClientFactory<MyPlayer>
        {
            public MyPlayer Create(WebSocket socket)
            {
                return new MyPlayer(socket);
            }
        }

        class MyStartUp
        {
            public MyStartUp(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; }

            public void ConfigureServices(IServiceCollection services)
            {
                //Add client Manager to handle clients
                services.AddClientManager<MyPlayer>(new MyPlayerFactory());
            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseWebAssemblyDebugging();
                }
                else
                {
                    app.UseExceptionHandler("/Error");
                    app.UseHsts();
                }

                //Web Socket
                app.UseWebSockets();
                app.UseWebSocketMiddleware<MyPlayer>("/ws");

                //app.UseHttpsRedirection();
                app.UseBlazorFrameworkFiles();
                app.UseStaticFiles();


                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapFallbackToFile("index.html");
                });
            }
        }
        #endregion

        public async Task RunAsync()
        {
            // Launch Server
            Server<MyStartUp, MyPlayer> server = new Server<MyStartUp, MyPlayer>();
            new Task(async () => await server.RunAsync()).Start();

            var manager = server.Manager;
            
            while (true)
            {
                var message = await manager.RecieveEventAsync();
                await HandleMessage(message, manager);
            }
        }

        private async Task HandleMessage(InfoControllerEvent info, ClientManager<MyPlayer> manager)
        {
            if (info.Event is NotificatorControllerEvent)
            {
                manager.GetClient(info.Sender).Entity = EntityType.Watcher;

                await manager.GetClient(info.Sender).SendAsync(new InitialMessage());

                ConfigurationMessage message = new ConfigurationMessage();
                message.AddBinding(1, typeof(NotificatorControllerEvent));
                await manager.GetClient(info.Sender).SendAsync(message);
                return;
            }

            if (info.Event is TextBoxControllerEvent textBox)
            {
                manager.GetClient(info.Sender).Name = textBox.Text;
                manager.GetClient(info.Sender).Entity = EntityType.Player;
                return;
            }

            if (info.Event is ButtonControllerEvent button)
            {
                var send = MakeMessage(info, manager);

                foreach (var client in manager.Players)
                {
                    if (client.Value.Entity == EntityType.Watcher)
                    {
                        await manager.SendAsync(client.Key, send);
                    }
                }
            }
        }

        private NotificatorControllerEvent MakeMessage(InfoControllerEvent info, ClientManager<MyPlayer> manager)
        {
            if (info.Event is ButtonControllerEvent button)
            {
                if (button.SenderID == 1)
                    return new NotificatorControllerEvent(manager.GetClient(info.Sender).Name, "Left", 1);

                if (button.SenderID == 2)
                    return new NotificatorControllerEvent(manager.GetClient(info.Sender).Name, "Up", 1);

                if (button.SenderID == 3)
                    return new NotificatorControllerEvent(manager.GetClient(info.Sender).Name, "Right", 1);

                return null;
            }
            else
            {
                return null;
            }
        }
    }

    
}
