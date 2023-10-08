using System;
using System.Collections.Generic;
using FantasyPlayer.Manager;

namespace FantasyPlayer.Interfaces;

public interface ICommandManagerFP
{
    void ParseCommand(string argsString);
    void PrintHelp(string stringValue, int intValue, CallbackResponse response);
    string GetCommandExample(OptionType optionType);

    Dictionary<string, (OptionType type, string[] aliases, string helpString, Action<string, int, CallbackResponse>
        commandCallback)> Commands { get; }
}