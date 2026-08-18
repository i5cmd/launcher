using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kartian_s_Launcher
{
    public class AppDetails
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string IconPath { get; set; } = "";
        public string Author { get; set; }
        public long Size { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Arguments { get; set; }
        public bool AdminRights { get; set; }
    }
    public class AppAdding
    {
        public AppDetails GetDetails(string path)
        {
            string exePath = path;
            FileVersionInfo fileInfo;
            FileVersionInfo parentInfo;
            string parentPath = Path.GetDirectoryName(exePath);
            try
            {
                fileInfo = FileVersionInfo.GetVersionInfo(exePath);
            }
            catch
            {
                return new AppDetails { };
            }

            return new AppDetails
            {
                Name = !string.IsNullOrEmpty(fileInfo.InternalName) ? fileInfo.InternalName : Path.GetFileName(exePath),
                Path = exePath,
                IconPath = GetIco(parentPath),
                Author = !string.IsNullOrEmpty(fileInfo.CompanyName) ? fileInfo.CompanyName : "",
                Description = !string.IsNullOrEmpty(fileInfo.FileDescription) ? fileInfo.FileDescription : "",
                Size = new FileInfo(exePath).Length,
                Date = new FileInfo(exePath).CreationTime
            };
        }

        public string GetIco(string folderPath)
        {
            string[] icoFiles = Directory.GetFiles(folderPath, "*.ico");
            if (icoFiles.Length == 0)
            {
                return Path.Combine(System.AppContext.BaseDirectory, "Assets", "generic.webp");
            }
            return icoFiles[0];
        }
    }
}
