using Microsoft.UI.Windowing;
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Core;
using Windows.System;

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
        private VirtualKeyModifiers modifier;
        private VirtualKey mainKey;
        private ObservableCollection<AppDetails> gimmeit;
        private ObservableCollection<ShortcutDetails> shortcuters;
        public event EventHandler<ObservableCollection<ShortcutDetails>> SendCurrentHotkeyList;
        private SystemComponents sys;
        private JsonManagement json;
        private AppDetails currentApp;
        private Window mainWindow;
        private HotkeyManager hmm;
        private ShortcutDetails cur;
        public Settings(Window window, ObservableCollection<AppDetails> details, ObservableCollection<ShortcutDetails> shortcuts)
        {
            InitializeComponent();
            gimmeit = details;
            sys = new SystemComponents();
            json = new JsonManagement();
            mainWindow = window;
            shortcuters = shortcuts;
            this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32
            {
                Height = 420,
                Width = 530
            });
            var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
            appWindowPresenter.IsResizable = false;
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
            ShortcutDetails dts;
            if (currentApp != null)
            {
                string name = currentApp.Name;
                try
                {
                    string inputString = $"{modifier.ToString()} + {mainKey.ToString()}";
                    foreach (var el in shortcuters)
                    {
                        if (el.inputs.Key == mainKey && el.inputs.Modifiers == modifier)
                        {
                            sys.ShowErrorMessages(this.Content, "This hotkey is already registered. Try a different combination.", "");
                            return;
                        }
                    }

                    HotkeyManager.Current.AddOrReplace($"{currentApp.Name}M", new KeyboardAccelerator { Key = mainKey, Modifiers = modifier }, (s, e) => {}); // test
                    HotkeyManager.Current.Remove($"{currentApp.Name}M"); // it works at least lol

                    dts = new ShortcutDetails { app = currentApp, inputs = new KeyboardAccelerator { Key = mainKey, Modifiers = modifier }, inputsOne = inputString };
                    shortcuters.Add(dts);
                    json.SaveShortcutJson(shortcuters);
                    SendCurrentHotkeyList?.Invoke(this, shortcuters);
                }
                catch (NHotkey.HotkeyAlreadyRegisteredException ex)
                {
                    sys.ShowErrorMessages(this.Content, "This hotkey is already registered. Try a different combination.", "");
                    return;
                }
                catch (Exception ex)
                {
                    sys.ShowErrorMessages(this.Content, ex.ToString(), "");
                    return;
                }
            }
        }

        private void AppComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppComboBox.SelectedItem is AppDetails app)
            {
                currentApp = app;
            }
        }


        private void RemoveShortcut_Click(object sender, RoutedEventArgs e)
        {
            if (cur != null)
            {
                int indext = shortcuters.IndexOf(cur);
                shortcuters.Remove(cur);
                json.SaveShortcutJson(shortcuters);
                SendCurrentHotkeyList?.Invoke(this, shortcuters);
                cur = null;
                try { ShortcutsList.SelectedItem = shortcuters[indext]; }
                catch { ShortcutsList.SelectedItem = null; }
            }
        }

        private void ShortcutsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShortcutsList.SelectedItem is ShortcutDetails shortcut)
            {
                cur = shortcut;
                RemoveShortcut.IsEnabled = true;
            }
            else
            {
                RemoveShortcut.IsEnabled = false;
            }
        }
    }
}
