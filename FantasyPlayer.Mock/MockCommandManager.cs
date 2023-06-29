using FantasyPlayer.Interfaces;
using FantasyPlayer.Manager;

namespace FantasyPlayer.Mock;

public class MockCommandManager : ICommandManager 
{
    public void ParseCommand(string argsString)
    {
        
    }

    public void PrintHelp(bool boolValue, int intValue, CallbackResponse response)
    {
        
    }

    public string GetCommandExample(OptionType optionType)
    {
        return "";
    }

    public Dictionary<string, (OptionType type, string[] aliases, string helpString, Action<bool, int, CallbackResponse>
        commandCallback)> Commands
    {
        get;
    } = new Dictionary<string, (OptionType type, string[] aliases, string helpString,
        Action<bool, int, CallbackResponse> commandCallback)>();
}