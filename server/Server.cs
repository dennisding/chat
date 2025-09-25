
namespace server
{
    class Server
    {
        public Server() 
        { 

        }

        public void ServeAt(int port)
        {
            Console.WriteLine($"serve_at: {port}");
        }

        public void ServeForeverAt(int port)
        {
            Console.WriteLine($"server_at port: {port}");
        }
    }
}