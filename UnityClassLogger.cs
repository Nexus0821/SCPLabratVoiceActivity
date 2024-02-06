using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabratVoiceActivity
{
    public interface ILogger<T>
    {
        void Info(object message);
        void Warning(object message);
        void Error(object message);

        string ClassName { get; }
    }

    public sealed class UnityClassLogger<T> : ILogger<T>
    {
        static readonly object _lock = new object();
        public static ILogger<T> Create() => new UnityClassLogger<T>();
        public UnityClassLogger() 
        { 
            _logSource = Logger.CreateLogSource(ClassName);
        }

        public void Info(object message) => _logSource.LogInfo(message);

        public void Warning(object message) => _logSource.LogWarning(message);

        public void Error(object message) => _logSource.LogError(message);

        public string ClassName => typeof(T).Name;
        private ManualLogSource _logSource;
    }
}
