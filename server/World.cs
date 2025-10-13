
using System.Diagnostics.CodeAnalysis;

namespace Server
{
    public class World
    {
        public World()
        {
        }

        public static void RegisterActor(string name, Type type)
        {

        }

        public static void CreateWorld()
        {

        }

        public static Actor? GetActor()
        {
            return null;
        }

        public static ActorId CreateActor(string name)
        {
            return new ActorId(0);
        }
    }
}