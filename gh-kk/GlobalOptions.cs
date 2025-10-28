using System;
using System.Collections.Generic;
using System.CommandLine;
using gh_kk.Interfaces;

namespace gh_kk;

public sealed class GlobalOptions : IGlobalOptions
{
    private readonly Dictionary<string, Option> _options = new();

    public void AddOption(string name, Option option)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(option);

        if (!_options.ContainsKey(name))
        {
            _options.Add(name, option);
        }
    }

    public Option<T> GetOption<T>(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (!_options.TryGetValue(name, out var option))
        {
            throw new ArgumentException($"Option '{name}' does not exist.", nameof(name));
        }

        return option as Option<T> 
            ?? throw new ArgumentException($"Option '{name}' is not of type Option<{typeof(T).Name}>.", nameof(name));
    }
}
