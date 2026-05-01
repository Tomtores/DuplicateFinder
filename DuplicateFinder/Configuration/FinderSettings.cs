using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SettingsFilename);
        }

        private void ReadFile(IDictionary<string, string> defaultSettings)
        {
            var path = GetFilename();
            if (File.Exists(path))
            {
                using (var reader = new StreamReader(File.OpenRead(path), Encoding.UTF8))
                {
                    var header = reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        var row = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(row))
                        {
                            continue;
                        }

                        var values = row.Split('\t');
                        _settings[values.GetValueSafe(0)] = values.GetValueSafe(1);
                    }
                }
            }
            else
            {
                _settings = defaultSettings;
            }

            if (UseHashCaching)
            {
                EnsureSalt(true);
            }
        }

        private void WriteOutFile()
        {
            var path = GetFilename();
            using (var file = new StreamWriter(path, false, Encoding.UTF8))
            {
                file.WriteLine("Setting\tValue\n");
                foreach (var item in this._settings)
                {
                    file.WriteLine("{0}\t{1}", item.Key, item.Value);
                }
            }
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
            get { 
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
        }
    }
}
