namespace ImageService.Commands
{
    public interface ICommand
    {    // The Function That will Execute The command
        string Execute(string[] args, out bool result);         
    }
}
