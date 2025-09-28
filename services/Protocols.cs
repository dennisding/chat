namespace services
{
    public interface IServerMethod
    {
        public void Echo(string msg)
        {
            Console.WriteLine($"Echo msg: {msg}");
        }
    }

    public interface IClientMethod
    {
        public void EchoBack(string msg)
        {
            Console.WriteLine($"EchoBack msg!!!{msg}");
        }
    }
}