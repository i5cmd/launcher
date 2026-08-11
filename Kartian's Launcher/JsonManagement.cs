using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kartian_s_Launcher
{
    public class JsonManagement
    {
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

        public void RemoveItem(AppDetails app, ObservableCollection<AppDetails> actualList)
        {
            actualList.Remove(app);
            SaveJson(actualList);
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
    }
}
