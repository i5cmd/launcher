using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using NHotkey.WinUI;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using WinRT.Interop;
using WinUIEx;
using static Kartian_s_Launcher.AppAdding;
using static Kartian_s_Launcher.SystemComponents;

namespace Kartian_s_Launcher
{
    public sealed partial class MainWindow : Window
    {
        private AppAdding ap;
        private AppDetails curApp;
        private EditDetails edit;
        private Settings settings; //that's hotkeys. i didnt think that through well... oh welp!
        private Options options;
        private HotkeyConfig hc;
        private int curIndex;
        public ObservableCollection<AppDetails> ProgramsDetails;
        public ObservableCollection<ShortcutDetails> shortcuters;
        private JsonManagement json;
        private SystemComponents sys;
        private TrayIcon trayIcon;
        private HotkeyManager hm;
        private ProgramConfiguration config;

        public MainWindow()
        {
            InitializeComponent();
            ap = new AppAdding();
            json = new JsonManagement();
            sys = new SystemComponents();
            hc = new HotkeyConfig();
            ProgramsDetails = new ObservableCollection<AppDetails>(json.LoadJson());
            this.AppWindow.MoveAndResize(new RectInt32
            {
                Height = 600,
                Width = 800
            });
            var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
            appWindowPresenter.PreferredMinimumWidth = 800;
            appWindowPresenter.PreferredMinimumHeight = 600;
            this.AppWindow.Closing += AppWindow_Closing;
            CreateSystemTray();
            shortcuters = (ObservableCollection<ShortcutDetails>)json.LoadShortcutJson();
            config = json.LoadOptions();
            hc.ReloadHotkeys(sys, shortcuters);
        }

        private void Settings_SendCurrentHotkeyList(object? sender, ObservableCollection<ShortcutDetails> e)
        {
            shortcuters = e;
            hc.ReloadHotkeys(sys, shortcuters);
        }

        private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            if (config.WindowXButton)
            {
                args.Cancel = true;
                this.AppWindow.Hide();
                trayIcon.IsVisible = true;
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private async void AddItem_Click(object sender, RoutedEventArgs e)
        {
            string path = await sys.OpenFilePicker(this, ".exe");
            if (path != null)
            {
                ProgramsDetails.Add(ap.GetDetails(path));
                json.SaveJson(ProgramsDetails);
            }
        }

        

        private void Programs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Programs.SelectedItem is AppDetails app)
            {
                FullScreen(app);
                Details.Visibility = Visibility.Visible;
                curApp = app;
                curIndex = Programs.SelectedIndex;
            }
        }

        private void FullScreen(AppDetails app)
        {
            try { Iconx2.Source = new BitmapImage(new Uri(app.IconPath)); }
            catch { Iconx2.Source = null; }
            Titlex2.Text = app.Name;
            Authorx2.Text = app.Author;
            Pathx2.Text = app.Path;
            Descx2.Text = app.Description;
            Sizex2.Text = $"{((double)app.Size / 1024 / 1024).ToString("F2")} MB";
            Datex2.Text = app.Date.ToLongDateString();
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (curApp != null)
            {
                edit = new EditDetails(curApp);
                edit.DetailsChanged += ChangeData;
                sys.ModalWindow(this, edit);
            }
        }

        private void ChangeData(object sender, Edits e)
        {
            string oldString = curApp.Name;
            curApp.Name = e.Name;
            curApp.IconPath = e.IconPath;
            curApp.Author = e.Author;
            curApp.Description = e.Description;
            curApp.AdminRights = e.AdminRights;
            curApp.Arguments = e.Arguments;
            RefreshList();
            hc.FindHotkeyAndReplaceName(oldString, e.Name, shortcuters);
            json.SaveShortcutJson(shortcuters);
        }

        private void RefreshList()
        {
            Programs.ItemsSource = null;
            Programs.ItemsSource = ProgramsDetails;
            
            try
            {
                Programs.SelectedItem = ProgramsDetails[curIndex];
                FullScreen(ProgramsDetails[curIndex]);
                Details.Visibility = Visibility.Visible;
            }
            catch
            {
                Programs.SelectedItem = null;
                FullScreen(new AppDetails());
            }
            json.SaveJson(ProgramsDetails);
        }

        private async void RunItem_Click(object sender, RoutedEventArgs e)
        {
            if (curApp != null)
            {
                ProcessErrorsExecutive pee = sys.RunProcess(curApp.Path, curApp.Arguments, curApp.AdminRights);
                if (!pee.runs)
                {
                    await sys.ShowErrorMessages(this.Content, pee.errormessage, "");
                    if (pee.code == -2)
                    {
                        json.RemoveItem(curApp, ProgramsDetails, shortcuters);
                        curApp = null;
                    }
                }
            }
        }

        private async void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (curApp != null)
            {
                bool result = await sys.ShowDialog(this.Content, "Are you sure?", $"Do you want to remove {curApp.Name}?");
                if (result)
                {
                    json.RemoveItem(curApp, ProgramsDetails, shortcuters);
                    shortcuters = (ObservableCollection<ShortcutDetails>)json.LoadShortcutJson();
                    curApp = null;
                    hc.ReloadHotkeys(sys, shortcuters);
                    RefreshList();
                    Details.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ExitProgram_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void CreateSystemTray()
        {
            var windowId = this.AppWindow.Id;
            string path = Path.Combine(AppContext.BaseDirectory, "Assets", "appicon.ico");
            var icon = new System.Drawing.Icon(path);
            IconId iconId = Win32Interop.GetIconIdFromIcon(icon.Handle);
            trayIcon = new TrayIcon(0, iconId, "Kartian's Launcher");
            trayIcon.IsVisible = false;

            trayIcon.Selected += (sender, e) => // left click
            {
                this.AppWindow.Show();
                trayIcon.IsVisible = false;
            };

            /* trayIcon.RightDoubleClick += (sender, e) => doesnt really work so
            {
                Environment.Exit(0);
            }; */

            trayIcon.ContextMenu += (sender, e) => // right click
            {
                Environment.Exit(0);
            };

            HotkeyManager.Current.AddOrReplace("OpenLauncher", new KeyboardAccelerator { Key = VirtualKey.Home, Modifiers = VirtualKeyModifiers.Menu | VirtualKeyModifiers.Control }, (sender, e) =>
            {
                if (config.AltHome)
                {
                    if (!this.AppWindow.IsVisible)
                    {
                        this.AppWindow.Show();
                        trayIcon.IsVisible = false;
                    }
                    else
                    {
                        this.AppWindow.Hide();
                        trayIcon.IsVisible = true;
                    }
                }
            });
        }


        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            settings = new Settings(this, ProgramsDetails, shortcuters);
            settings.SendCurrentHotkeyList += Settings_SendCurrentHotkeyList;
            sys.ModalWindow(this, settings);
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            options = new Options(config);
            options.SendConfig += Options_SendConfig;
            sys.ModalWindow(this, options);
        }

        private void Options_SendConfig(object? sender, ProgramConfiguration e)
        {
            config = e;
        }
    }
}
