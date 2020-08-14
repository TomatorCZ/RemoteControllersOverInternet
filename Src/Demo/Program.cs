using System;
using System.Threading.Tasks;
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
            await server.Run();
        }
    }
}
