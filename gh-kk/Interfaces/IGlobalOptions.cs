using System.CommandLine;

namespace gh_kk.Interfaces
{
    /// <summary>
    /// Interface for managing global command line options.
    /// </summary>
    public interface IGlobalOptions
    {
        /// <summary>
        /// Adds a command line option with the specified name.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="option">The command line option to add.</param>
        void AddOption(string name, Option option);

        /// <summary>
        /// Gets a command line option with the specified name and type.
        /// </summary>
        /// <typeparam name="T">The type of the option value.</typeparam>
        /// <param name="name">The name of the option to get.</param>
        /// <returns>The command line option.</returns>
        /// <exception cref="System.ArgumentException">Thrown when the option doesn't exist or is not of the specified type.</exception>
        Option<T> GetOption<T>(string name);
    }
}