using ImageService.Commands;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.ImageService.Commands
{
    class LogCommand : ICommand
    {
        public string Execute(string[] args, out bool result)
        {
            Dictionary<int, string[]> map = new Dictionary<int, string[]>();

            string logName = ConfigurationManager.AppSettings["LogName"];
            EventLog myLog = new EventLog(logName, ".");
            EventLogEntry entry;
            EventLogEntryCollection entries = myLog.Entries;

            for (int i = (entries.Count -1); i > 0; i--)
            {
                entry = entries[i];
                string[] str = new string[2];
                str[0] = entry.EntryType.ToString();
                str[1] = entry.Message.ToString();
                int.TryParse(entry.InstanceId.ToString(), out int id);
                map.Add(id,str);
                if (id == 1)
                {
                    break;
                }
            }
            JObject logObj = new JObject
            {
                ["CommandEnum"] = (int)CommandEnum.LogCommand,
                ["logMap"] = JsonConvert.SerializeObject(map),
                ["firstTime"] = "true"
            };
            result = true;
            return logObj.ToString();
        }
    }
}
