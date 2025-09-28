
namespace services
{
    public interface IDispatcher
    {
        void Dispatch(BinaryReader reader);
    }
}