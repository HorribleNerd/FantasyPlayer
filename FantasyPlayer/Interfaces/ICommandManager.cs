using System;
using System.Collections.Generic;
using FantasyPlayer.Manager;

namespace FantasyPlayer.Interfaces;

public interface ICommandManager
{
    void ParseCommand(string argsString);
    void PrintHelp(bool boolValue, int intValue, CallbackResponse response);
    string GetCommandExample(OptionType optionType);

    Dictionary<string, (OptionType type, string[] aliases, string helpString, Action<bool, int, CallbackResponse>
        commandCallback)> Commands { get; }
}