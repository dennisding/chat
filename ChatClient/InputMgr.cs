
using System.Threading.Channels;

namespace ChatClient;

class InputMgr
{
    Channel<string> channel;
    bool running = false;
    public InputMgr()
    {
        running = true;
        channel = Channel.CreateUnbounded<string>();

        Task.Run(HandleReadAsync);
    }

    public void Tick()
    {
        while (channel.Reader.TryRead(out string? line))
        {
            Console.WriteLine(line);
        }
    }

    async Task HandleReadAsync()
    {
        while (running)
        {
            string? line = await Console.In.ReadLineAsync();
            if (line != null)
            {
                await channel.Writer.WriteAsync(line);
            }
        }
    }
}