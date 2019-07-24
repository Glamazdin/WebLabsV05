using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebLabsV05.Services
{
    public class FileLogger : ILogger
    {
        private string _filePath;
        private object _lock;
        public FileLogger(string path)
        {
            _filePath = path;
            _lock = new object();
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel==LogLevel.Information;
        }

        public void Log<TState>(LogLevel logLevel, 
            EventId eventId, 
            TState state, 
            Exception exception, 
            Func<TState, Exception, string> formatter)
        {
            if(formatter!=null)
            {
                lock(_lock)
                {
                    File.AppendAllText(_filePath, formatter(state, exception) 
                        + Environment.NewLine);
                }
            }
        }
    }

    public class FileLoggerProvider : ILoggerProvider
    {
        private string _filepath;
        public FileLoggerProvider(string path)
        {
            _filepath = path;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(_filepath);
        }

        public void Dispose()
        {           
        }
        
    }

    
}
