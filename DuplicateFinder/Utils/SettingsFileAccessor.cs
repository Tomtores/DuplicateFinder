using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DuplicateFinder.Utils
{
    public static class SettingsFileAccessor
    {
        public static void WriteSettingsToFile(string path, IEnumerable<(string Key, string Value)> settings)
        {
            using (var file = new StreamWriter(path, false, Encoding.UTF8))
            {
                file.WriteLine("Setting\tValue\n");

                foreach (var item in settings)
                {
                    file.WriteLine("{0}\t{1}", item.Key, item.Value);
                }
            }
        }

        public static IEnumerable<(string Key, string Value)> ReadSettingsFromFile(string path)
        {            
            if (File.Exists(path))
            {
                using (var reader = new StreamReader(File.OpenRead(path), Encoding.UTF8))
                {
                    var result = new List<(string, string)>();
                    
                    var header = reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        var row = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(row))
                        {
                            continue;
                        }

                        var values = row.Split('\t');
                        result.Add((values.GetValueSafe(0), values.GetValueSafe(1)));
                    }

                    return result;
                }
            }
            else
            {
                return null;
            }            
        }
    }
}
