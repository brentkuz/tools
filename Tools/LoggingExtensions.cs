//Author: Brent Kuzmanich
//Comment: This class is used to log error to both text files and event logs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingExtensions
{
    public static class LoggingExtensions
    {
        #region static
        //extension method for logging to the event log
        //"log" = name of log in event viewer
        public static void LogEvent(this Exception ex, string log, EventLogEntryType type)
        {
            //check for log and create new if null
            if (!EventLog.SourceExists(log))
            {
                EventLog.CreateEventSource(log, log);
            }
            //write new log entry
            EventLog entry = new EventLog(log);
            entry.Source = log;  
            entry.WriteEntry(BuildErrorString(ex), type);
        }
        //extension method for logging to a specified text file
        //"path" = file path to log to
        public static void LogText(this Exception ex, string path)
        {
            //Write to file
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine("****** {0} *************************************************************************", DateTime.Now);
                sw.WriteLine("Type: {0}  Source: {1}", ex.GetType(), ex.Source);
                sw.WriteLine("Message: " + ex.Message);
                sw.WriteLine("Stack Trace: " + ex.StackTrace);
                sw.Close();
            }
        }
        #endregion

        #region private
        //method to build an error string for Event Log
        private static string BuildErrorString(Exception ex)
        {
            return ex.ToString() + "       Source: " + ex.Source + "       Message: " + ex.Message;
        }
        #endregion
    }
}
