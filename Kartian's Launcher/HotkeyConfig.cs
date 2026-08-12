using NHotkey.WinUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kartian_s_Launcher
{
    public class HotkeyConfig
    {
        public void ReloadHotkeys(SystemComponents sys, ObservableCollection<ShortcutDetails> shortcuters)
        {
            foreach (var el in shortcuters)
            {
                HotkeyManager.Current.AddOrReplace(el.app.Name, el.inputs, (sender, e) =>
                {
                    sys.RunProcess(el.app.Path, "");
                });
            }
        }
    }
}
