using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ImageService.Server;
using ImageService.Controller;
using ImageService.Model;
using ImageService.Logging;
using ImageService.Logging.Model;
using System.Configuration;
using ImageService.Infrastructure;
using System.Timers;
using ImageService.ImageService.Server;

namespace ImageService.ImageService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    public partial class ImageService : ServiceBase
    {
        private int eventId = 1;
        private ImageServer m_imageServer; // The Image Server
        public ILoggingService logging;
        private EventLog eventLog1;

        /// <summary>
        /// Constructor of ImageService class
        /// </summary>
        /// <param name="args">
        /// Optional - strings that contains the SourceName and the LogName
        /// </param>
        public ImageService(string[] args)
        {
            // First reading from the appconfig creating the eventLog
            InitializeComponent();
            string eventSourceName = ConfigurationManager.AppSettings["SourceName"];
            string logName = ConfigurationManager.AppSettings["LogName"];
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName);
            }
            eventLog1.Source = eventSourceName;
            eventLog1.Log = logName;
        }


        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        /// <summary>
        /// Starting our service
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Starting ImageService", EventLogEntryType.Information, eventId++);
            // Reading from appconfig 
            InitializeService();
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Stopping ImageService", EventLogEntryType.Information, eventId++);
            this.m_imageServer.CloseServer();
        }
        /// <summary>
        /// Updating our entry according to the message
        /// </summary>
        /// <param name="sender">
        /// The message sender
        /// </param>
        /// <param name="e">
        /// The events arguments including the message and its type
        /// </param>
        public void OnLog(object sender, MessageRecievedEventArgs e)
        {
            // Updating our log according to the message status
            switch (e.Status)
            {
                case MessageTypeEnum.INFO:
                    eventLog1.WriteEntry(e.Message, EventLogEntryType.Information, eventId++);
                    break;
                case MessageTypeEnum.FAIL:
                    eventLog1.WriteEntry(e.Message, EventLogEntryType.FailureAudit, eventId++);
                    break;
                case MessageTypeEnum.WARNING:
                    eventLog1.WriteEntry(e.Message, EventLogEntryType.Warning, eventId++);
                    break;
            }
        }

        /// <summary>
        /// Initializing the eventLog
        /// </summary>
        private void InitializeComponent()
        {
            this.eventLog1 = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).EndInit();

        }

        /// <summary>
        /// Reading the app.config.
        /// Initializing and creating members.
        /// </summary>
        private void InitializeService()
        {
            this.logging = new LoggingService();
            logging.MessageRecieved += OnLog;
            this.m_imageServer = new ImageServer(this.logging);
            // Lastly updating our entry
            eventLog1.WriteEntry("End of initialzation", EventLogEntryType.Information, eventId++ );
        }
    }
}
