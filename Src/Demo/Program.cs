using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Configuration;
using RemoteController;
using RemoteControllers;

namespace Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello this is demo!");

            await Test1();
        }

        static public async Task Test1()
        {
            // Launch Client
            Server server = new Server();

            new Task(async () => await server.RunAsync()).Start();

            await server.Manager.RecieveEventAsync();
        }
    }
}
