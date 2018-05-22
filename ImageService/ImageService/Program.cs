using System.ServiceProcess;

namespace ImageService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ImageService.ImageService(args)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
