
using System.Threading.Channels;

namespace ChatClient;

class InputMgr
{
    Channel<string> channel;
    bool running = false;
    CommandMgr commandMgr;

    public InputMgr(CommandMgr commandMgr)
    {
        running = true;
        channel = Channel.CreateUnbounded<string>();

        Task.Run(HandleReadAsync);
        this.commandMgr = commandMgr;
    }

    public void Tick()
    {
        while (channel.Reader.TryRead(out string? line))
        {
            commandMgr.InputCommand(line);
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