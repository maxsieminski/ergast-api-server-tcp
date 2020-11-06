using System.Net;

namespace TCP_Server_Asynchronous
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncServer server = new AsyncServer(IPAddress.Any, 2048);
            server.Start();
        }
    }
}
