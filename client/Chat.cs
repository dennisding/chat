
namespace client
{
    class Chat
    {
        ClientServices services;
        static string name = "";
        public Chat(ClientServices services)
        {
            this.services = services;
        }

        public void Poll()
        {
        }

        public static string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
    }
}