
//global using ActorId = long;

namespace Common
{
    enum Location
    {
        None,
        Client = 1,
        Gate = 2,
        Core = 4,
        Shadow = 8,
    }
    public readonly record struct ActorId(long value);
}
