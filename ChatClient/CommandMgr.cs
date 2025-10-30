
using Client;

namespace ChatClient;

// new {room_name}
// enter {room_name}
// leave
// quit
// to {actor_name} {msg}
// login {user_name} {password}
// msg {msg}

class CommandInput
{
    string[]? tokens = null;
    int index = 0;

    public string? command;
    public string remain = "";
    public CommandInput(string command)
    {
        remain = command;
        tokens = command.Split();

        this.command = NextToken();
    }

    public string NextToken()
    {
        if (tokens == null)
        {
            return "";
        }

        while (index < tokens.Length)
        {
            string token = tokens[index];
            ++index;
            string result = token.Trim();
            if (result.Length != 0)
            {
                remain = remain.Substring(token.Length);
                return result;
            }
        }

        return "";
    }
}

class CommandMgr
{
    public InputMgr inputMgr;

    HashSet<string> hasSubCommand;

    ChatClient Player
    {
        get { return Game.GetPlayer<ChatClient>()!; }
    }

    public CommandMgr()
    {
        inputMgr = new InputMgr(this);
        hasSubCommand = new HashSet<string>();

        RegisterSubCommands();
    }

    void RegisterSubCommands()
    {
        hasSubCommand.Add("new");
    }

    public void Tick()
    {
        inputMgr.Tick();
    }

    public void InputCommand(string command)
    {
        CommandInput input = new CommandInput(command);

        ProcessCommand(input);
    }

    void ProcessCommand(CommandInput input)
    {
        input.remain = input.remain.Trim();
        if (input.command == "new")
        {
            NewRoom(input);
        }
        else if (input.command == "enter")
        {
            EnterRoom(input);
        }
        else if (input.command == "leave")
        {
            Leave();
        }
        else if (input.command == "quit" || input.command == "exit")
        {
            Quit();
        }
        else if (input.command == "to")
        {
            To(input);
        }
        else if (input.command == "login")
        {
            Login(input);
        }
        else if (input.command == "msg")
        {
            Msg(input);
        }
        else if (input.command == "help")
        {
            HelpCommand();
        }
    }

    public void NewRoom(CommandInput command)
    {
        string roomName = command.remain.Trim();

        var player = Game.GetPlayer<ChatClient>();
        if (player == null)
        {
            Console.WriteLine("请先登录.");
            return;
        }

        player.CommandNewRoom(roomName);
    }

    public void EnterRoom(CommandInput command)
    {
        string roomName = command.remain.Trim();

        var player = Game.GetPlayer<ChatClient>();
        player!.CommandEnterRoom(roomName);
    }

    public void Leave()
    {
        Player.server!.LeaveRoom();
    }

    public void Quit()
    {
        Console.WriteLine($"Command.Quit");
        Game.running = false;
    }

    public void To(CommandInput command)
    {
//        Console.WriteLine($"Command.To: {command.remain}");
        string userName = command.NextToken().Trim();
        string msg = command.remain.Trim();

        Player.CommandMessageTo(userName, msg);
//        Console.WriteLine($"To [{userName}], [{msg}]");
    }

    public void Login(CommandInput command)
    {
        string remain = command.remain;
        Console.WriteLine($"Command.Login: {remain}");
        string[] tokens = remain.Split();

        string username = tokens[0];
        string password = tokens[1];

        var player = Game.GetPlayer<LoginClient>();
        player!.CommandLogin(username, password);
    }

    public void Msg(CommandInput command)
    {
        string remain = command.remain;

        Player.CommandChatMessage(remain.Trim());
    }

    public void HelpCommand()
    {
        Console.WriteLine("login {userName} {password}"); // login
        Console.WriteLine("new {roomName}"); // new room 
        Console.WriteLine("leave"); // leave
        Console.WriteLine("enter {roomName}"); // enter roomname
        Console.WriteLine("msg chat msg in room"); // chat message
        Console.WriteLine("help"); // help
        Console.WriteLine("exit|quit"); // exit the chat
        Console.WriteLine("to {userName} {msg}");
    }
}