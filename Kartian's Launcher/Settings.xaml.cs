using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NHotkey.WinUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Core;
using Windows.System;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Kartian_s_Launcher
{
    public class ShortcutDetails
    {
        public AppDetails app { get; set; }
        public KeyboardAccelerator inputs { get; set; } // like alt, t or alt, space, r
        public string inputsOne { get; set; }
    }
    public sealed partial class Settings : Window
    {
        private ObservableCollection<ShortcutDetails> shortcuters;
        private VirtualKeyModifiers modifier;
        private VirtualKey mainKey;
        private ObservableCollection<AppDetails> gimmeit;
        private SystemComponents sys;
        private JsonManagement json;
        private AppDetails currentApp;
        public Settings(ObservableCollection<AppDetails> appDetails)
        {
            InitializeComponent();
            gimmeit = appDetails;
            sys = new SystemComponents();
            json = new JsonManagement();
            shortcuters = (ObservableCollection<ShortcutDetails>)json.LoadShortcutJson();
            ReloadHotkeys();
        }

        private void ShortcutInput_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Menu || e.Key == VirtualKey.LeftWindows || e.Key == VirtualKey.RightWindows || e.Key == VirtualKey.Control || e.Key == VirtualKey.Shift || e.Key == VirtualKey.Application) return;
            mainKey = e.Key;
            ShortcutInput.SelectionLength = 0;
            ShortcutInput.Text = e.Key.ToString();
        }

        private void ModifierSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModifierSelection.SelectedItem is ListBoxItem selectedItem)
            {
                switch (selectedItem.Name)
                {
                    case "Menu":
                        modifier = VirtualKeyModifiers.Menu;
                        break;
                    case "LWindows":
                        modifier = VirtualKeyModifiers.Windows;
                        break;
                    case "Control":
                        modifier = VirtualKeyModifiers.Control;
                        break;
                    case "Shift":
                        modifier = VirtualKeyModifiers.Shift;
                        break;

                }
            }
        }

        private void RefreshList()
        {
            ShortcutsList.ItemsSource = null;
            ShortcutsList.ItemsSource = shortcuters;
        }

        private void AddShortcut_Click(object sender, RoutedEventArgs e)
        {
            string name = currentApp.Name;
            try
            {
                HotkeyManager.Current.AddOrReplace(name, new KeyboardAccelerator { Key = mainKey, Modifiers = modifier }, (sender, e) =>
                {
                    sys.RunProcess(currentApp.Path, "");
                });
                string inputString = $"{modifier.ToString()} + {mainKey.ToString()}";
                shortcuters.Add(new ShortcutDetails { app = currentApp, inputs = new KeyboardAccelerator { Key = mainKey, Modifiers = modifier }, inputsOne = inputString });
                json.SaveShortcutJson(shortcuters);
            }
            catch (NHotkey.HotkeyAlreadyRegisteredException ex)
            {
                sys.ShowErrorMessages(this.Content, "This hotkey is already registered. Try a different combination.");
            }
            catch (Exception ex)
            {
                sys.ShowErrorMessages(this.Content, ex.ToString());
            }
        }
        private void AddAppsToComboBox()
        {

        }

        private void AppComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppComboBox.SelectedItem is AppDetails app)
            {
                currentApp = app;
            }
        }

        private void ReloadHotkeys()
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
