namespace Codecov.Services.VersionControl
{
    internal interface ITerminalService
    {
        string Run(string command, string commandArguments);
    }
}
