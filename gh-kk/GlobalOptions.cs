using System;
using System.Collections;
using System.CommandLine;

namespace gh_kk;

public class GlobalOptions
{
    readonly Hashtable options;

    public GlobalOptions()
    {
        options = [];
    }

    public void AddOption(string name, Option option)
    {
        if (!options.ContainsKey(name))
        {
            options.Add(name, option);
        }
    }

    public Option<T> GetOption<T>(string name)
    {
        // throw if optionValue is null bacause options does not contain the key
        var optionValue = options[name] as Option<T> ?? throw new ArgumentException($"Option {name} is not of type Option<{typeof(T).Name}>");
        return optionValue;
    }
}
