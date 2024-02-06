using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabratVoiceActivity
{
    public enum ModConfigLoadErrorType
    {
        None,
        JsonDeserializationFailed,
        FileNotFound
    }

    public sealed class ModConfiguration
    {
        private static ILogger<ModConfiguration> _logger = UnityClassLogger<ModConfiguration>.Create();
        private static ModConfiguration _instance;

        private static ModConfigLoadErrorType _errorType = ModConfigLoadErrorType.None;
        private static bool _handledError;

        private static DateTime _lastUpdateTime;
        private static TimeSpan _timeToStop = TimeSpan.FromMinutes(1);

        private static readonly ModConfiguration _defaultConfiguration = new ModConfiguration
        {
            ModGuid = Entry.GUID,
            ModVersion = Entry.VERSION,
            IsVoiceActivationEnabled = true
        };

        public static bool FailedToLoad => _errorType != ModConfigLoadErrorType.None;

        public static string ConfigurationPath
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, "lrva.config.json");
            }
        }

        public static ModConfiguration Instance => _instance;

        public static ModConfigLoadErrorType LoadConfiguration()
        {
            _logger.Info("Loading configuration: " + ConfigurationPath);

            if (!File.Exists(ConfigurationPath))
                _errorType = ModConfigLoadErrorType.FileNotFound;

            if (_instance != null)
                Task.Factory.StartNew(UpdateModTask, TaskCreationOptions.LongRunning);

            HandleError();
            return _errorType;
        }

        private static async Task UpdateModTask()
        {
            Stopwatch sw = Stopwatch.StartNew();

            for (; ; )
            {
                if ((DateTime.Now - _lastUpdateTime) > _timeToStop)
                {
                    sw.Restart();

                    try
                    {
                        _instance = JsonConvert.DeserializeObject<ModConfiguration>(File.ReadAllText(ConfigurationPath));
                        _logger.Warning("Loaded mod config in " + sw.ElapsedMilliseconds + "ms");
                    }
                    catch
                    {
                        _logger.Error("Configuration JSON deserialization failed");
                        _errorType = ModConfigLoadErrorType.JsonDeserializationFailed;
                    }

                    HandleError();
                    _lastUpdateTime = DateTime.Now;
                }
            }
        }

        private static void HandleError()
        { 
            if (!_handledError)
            {
                if (_errorType == ModConfigLoadErrorType.JsonDeserializationFailed)
                    _logger.Error("Failed to load configuration -- JSON deserialization failed");

                if (_errorType == ModConfigLoadErrorType.FileNotFound)
                {
                    _logger.Error("Failed to load mod -- config was not found in the Game directory '" + ConfigurationPath + "' (creating file for you!)");
                    File.WriteAllText(ConfigurationPath, JsonConvert.SerializeObject(_defaultConfiguration));

                    _instance = _defaultConfiguration;
                    _errorType = ModConfigLoadErrorType.None;
                }

                _handledError = true;
            }
        }

        public string ModGuid { get; set; }
        public string ModVersion { get; set; }
        public bool IsVoiceActivationEnabled { get; set; }
    }
}
