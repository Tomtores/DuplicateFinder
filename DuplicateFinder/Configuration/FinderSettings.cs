using DuplicateFinder.Enums;
using DuplicateFinder.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuplicateFinder
{
    public class FinderSettings
    {
        #region Singleton

        private static FinderSettings instance;
        public static FinderSettings GetInstance()
        {
            if (instance == null)
            {
                instance = new FinderSettings();
            }

            return instance;
        }

        private FinderSettings()
        {
            ReadFile(defaultSettings);
        }

        #endregion

        private const string SettingsFilename = "settings.tsv";
        private IDictionary<string, string> _settings = new Dictionary<string, string>();

        private readonly IDictionary<string, string> defaultSettings = new Dictionary<string, string>()
            {
                { SettingNames.IgnoreEmpty, true.ToString() },
                { SettingNames.Thumbsize, 80.ToString() },
                { SettingNames.LogLevel, LogLevel.Warning.ToString() },
            };

        private T GetValue<T>(string option)
        {
            string setting;
            if (_settings.TryGetValue(option, out setting))
            {
                var baseType = Nullable.GetUnderlyingType(typeof(T));
                if (baseType != null)
                {
                    // T is nullable
                    return setting == "null" ? default(T) : (T)Convert.ChangeType(setting, baseType);
                }

                return (T)Convert.ChangeType(setting, typeof(T));
            }
            else
            {
                return default(T);
            }
        }

        private void SetValue(string option, object value)
        {
            this._settings[option] = value == null ? "null" : value?.ToString();
            WriteOutFile();
        }

        private string GetFilename()
        {
            return Path.Combine(Utilities.AppPath, SettingsFilename);
        }

        private void ReadFile(IDictionary<string, string> defaultSettings)
        {
            var path = GetFilename();
            var settings = SettingsFileAccessor.ReadSettingsFromFile(path);

            if (settings == null)
            {
                this._settings = defaultSettings;
            }
            else
            {
                this._settings = settings.ToDictionary(s => s.Key, s => s.Value);
            }

            if (UseHashCaching)
            {
                EnsureSalt(true);
            }
        }        

        private void WriteOutFile()
        {
            var path = GetFilename();
            SettingsFileAccessor.WriteSettingsToFile(path, this._settings.Select(s => (s.Key, s.Value)));
        }        

        #region Settings

        public bool UseCRC
        {
            get
            {
                return GetValue<bool>(SettingNames.UseCRC);
            }
            set
            {
                SetValue(SettingNames.UseCRC, value);
            }
        }

        public bool IgnoreEmpty
        {
            get
            {
                return GetValue<bool>(SettingNames.IgnoreEmpty);
            }
            set
            {
                SetValue(SettingNames.IgnoreEmpty, value);
            }
        }

        public bool CountDirectoryFiles
        {
            get
            {
                return GetValue<bool>(SettingNames.CountDirectoryFiles);
            }
            set
            {
                SetValue(SettingNames.CountDirectoryFiles, value);
            }
        }

        public bool UseHashCaching
        {
            get
            {
                var value = GetValue<bool>(SettingNames.UseHashCaching);
                EnsureSalt(value);
                return value;
            }
            set
            {
                SetValue(SettingNames.UseHashCaching, value);
                EnsureSalt(value);
            }
        }

        public Guid? HashSalt
        {
            get
            {
                var value = GetValue<string>(SettingNames.HashSalt);
                if (value == null)
                {
                    return null;
                }

                return Guid.Parse(value);
            }

            set => SetValue(SettingNames.HashSalt, value?.ToString());
        }

        public void EnsureSalt(bool cacheEnabled)
        {
            if (cacheEnabled && HashSalt == null)
            {
                HashSalt = Guid.NewGuid();
            }
        }

        /// <summary>
        /// Minimal size to search for. Null if not specified.
        /// </summary>
        public int? MinSizeKB
        {
            get
            {
                return GetValue<int?>(SettingNames.MinSize);
            }
            set
            {
                SetValue(SettingNames.MinSize, value);
            }
        }

        /// <summary>
        /// Maximal size to search for. Null if not specified.
        /// </summary>
        public int? MaxSizeKB
        {
            get
            {
                return GetValue<int?>(SettingNames.MaxSize);
            }
            set
            {
                SetValue(SettingNames.MaxSize, value);
            }
        }

        public string LastDir
        {
            get
            {
                return GetValue<string>(SettingNames.LastDir);
            }
            set
            {
                SetValue(SettingNames.LastDir, value);
            }
        }

        public List<string> ScanPaths
        {
            get
            {
                return (GetValue<string>(SettingNames.ScanPaths) ?? string.Empty).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            set
            {
                SetValue(SettingNames.ScanPaths, string.Join(";", value));
            }
        }

        public string KeepTrashLastDir
        {
            get
            {
                return GetValue<string>(SettingNames.KeepTrashLastDir);
            }
            set
            {
                SetValue(SettingNames.KeepTrashLastDir, value);
            }
        }

        public string Ignores
        {
            get
            {
                return GetValue<string>(SettingNames.Ignored);
            }
            set
            {
                SetValue(SettingNames.Ignored, value);
            }
        }

        public int Thumbsize
        {
            get
            {
                return GetValue<int>(SettingNames.Thumbsize);
            }
            set
            {
                SetValue(SettingNames.Thumbsize, value);
            }
        }

        public bool PreviewEnabled
        {
            get
            {
                return GetValue<bool>(SettingNames.PreviewEnabled);
            }
            set
            {
                SetValue(SettingNames.PreviewEnabled, value);
            }
        }

        public LogLevel LogLevel
        {
            get
            {
                var value = GetValue<string>(SettingNames.LogLevel);
                if(Enum.TryParse(value, out LogLevel logLevel))
                {
                    return logLevel;
                }

                return LogLevel.Warning;
            }
            set
            {
                SetValue(SettingNames.LogLevel, value.ToString("g"));
            }
        }

        #endregion

        private class SettingNames
        {
            public const string UseCRC = "UseCRC";
            public const string IgnoreEmpty = "IgnoreEmpty";
            public const string CountDirectoryFiles = "CountDirectoryFiles";
            public const string UseHashCaching = "UseHashCaching";
            public const string HashSalt = "HashSalt";
            public const string MinSize = "MinSize";
            public const string MaxSize = "MaxSize";
            public const string LastDir = "LastDir";
            public const string ScanPaths = "ScanPaths";
            public const string KeepTrashLastDir = "KeepTrashLastDir";
            public const string Ignored = "Ignored";
            public const string Thumbsize = "Thumbsize";
            public const string PreviewEnabled = "PreviewEnabled";
            public const string LogLevel = "LogLevel";
        }
    }
}
