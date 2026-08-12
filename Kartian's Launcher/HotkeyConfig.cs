using Microsoft.UI.Xaml.Input;
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
        private List<string> oldList = new List<string>();
        public void ReloadHotkeys(SystemComponents sys, ObservableCollection<ShortcutDetails> shortcuters)
        {

            foreach (var el in oldList)
            {
                HotkeyManager.Current.Remove(el);
            }
            oldList.Clear();
            foreach (var el in shortcuters)
            {
                var accelerator = new KeyboardAccelerator
                {
                    Key = el.inputs.Key,
                    Modifiers = el.inputs.Modifiers
                };
                HotkeyManager.Current.AddOrReplace(el.app.Name, accelerator, (sender, e) =>
                {
                    sys.RunProcess(el.app.Path, "");
                });
                oldList.Add(el.app.Name);
            }
        }
    }
}
