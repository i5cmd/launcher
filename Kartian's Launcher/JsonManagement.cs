using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kartian_s_Launcher
{
    public class JsonManagement
    {
        //ill optimize it soon
        public void SaveJson(IEnumerable<AppDetails> data)
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string myAppFolder = Path.Combine(localAppData, "Kartian's Launcher");
            Directory.CreateDirectory(myAppFolder);
            string file = Path.Combine(myAppFolder, "data.json");
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(file, json);
        }

        public IEnumerable<AppDetails> LoadJson()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string myAppFolder = Path.Combine(localAppData, "Kartian's Launcher");
            string file = Path.Combine(myAppFolder, "data.json");
            if (File.Exists(file))
            {
                string fileContent = File.ReadAllText(file);
                return JsonSerializer.Deserialize<List<AppDetails>>(fileContent);
            }
            else
            {
                return new ObservableCollection<AppDetails>();
            }
        }

        public void RemoveItem(AppDetails app, ObservableCollection<AppDetails> actualList, ObservableCollection<ShortcutDetails> sh)
        {
            actualList.Remove(app);
            SaveJson(actualList);
            foreach (var el in sh.ToList())
            {
                if (el.app == app)
                {
                    sh.Remove(el);
                }
            }
            SaveShortcutJson(sh);
        }

        public void SaveShortcutJson(IEnumerable<ShortcutDetails> data)
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string myAppFolder = Path.Combine(localAppData, "Kartian's Launcher");
            Directory.CreateDirectory(myAppFolder);
            string file = Path.Combine(myAppFolder, "shortcuts.json");
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(file, json);
        }

        public IEnumerable<ShortcutDetails> LoadShortcutJson()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string myAppFolder = Path.Combine(localAppData, "Kartian's Launcher");
            string file = Path.Combine(myAppFolder, "shortcuts.json");
            if (File.Exists(file))
            {
                string fileContent = File.ReadAllText(file);
                return JsonSerializer.Deserialize<ObservableCollection<ShortcutDetails>>(fileContent);
            }
            else
            {
                return new ObservableCollection<ShortcutDetails>();
            }
        }

        public void SaveOptions(ProgramConfiguration data)
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string myAppFolder = Path.Combine(localAppData, "Kartian's Launcher");
            Directory.CreateDirectory(myAppFolder);
            string file = Path.Combine(myAppFolder, "options.json");
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(file, json);
        }

        public ProgramConfiguration LoadOptions()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string myAppFolder = Path.Combine(localAppData, "Kartian's Launcher");
            string file = Path.Combine(myAppFolder, "options.json");
            if (File.Exists(file))
            {
                string fileContent = File.ReadAllText(file);
                return JsonSerializer.Deserialize<ProgramConfiguration>(fileContent);
            }
            else
            {
                return new ProgramConfiguration();
            }
        }
    }
}