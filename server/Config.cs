
namespace Server;

public class Config
{
    public string root = "";
    public string startActor = "Server";
    public string connectActor = "Login";
    public int clientPort = 666;
    public int servicePort = 1000;

    public Config()
    {

    }

    public static Config GetConfig(int index)
    {
        if (index == 1)
        {
            return CreateConfig_1();
        }
        else if (index == 2)
        {
            return CreateConfig_2();
        }

        return new Config();
    }

    public static Config CreateConfig_1()
    {
        Config config = new Config();

        return config;
    }

    public static Config CreateConfig_2()
    {
        Config config = new Config();

        config.servicePort = 1001;

        return config;
    }
}
