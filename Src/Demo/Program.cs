using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Configuration;
using RemoteController;

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

            while (true)
            {
                var action = await server.Manager.RecieveEventAsync();
                Console.WriteLine(action.Event.ToString());
            }

        }
    }
}
