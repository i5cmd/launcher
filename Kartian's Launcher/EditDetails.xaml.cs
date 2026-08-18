using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Kartian_s_Launcher
{
    public class Edits
    {
        public string Name { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Arguments { get; set; } = string.Empty;
        public bool AdminRights { get; set; }
    }
    public sealed partial class EditDetails : Window
    {
        public event EventHandler<Edits> DetailsChanged;
        private SystemComponents sys;
        private string path = Path.Combine(System.AppContext.BaseDirectory, "Assets", "generic.webp");
        private AppDetails app;
        public EditDetails(AppDetails curApp)
        {
            InitializeComponent();
            sys = new SystemComponents();
            this.AppWindow.MoveAndResize(new RectInt32
            {
                Height = 350,
                Width = 500
            });
            var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
            appWindowPresenter.IsResizable = false;
            appWindowPresenter.IsAlwaysOnTop = true;
            appWindowPresenter.IsMaximizable = false;
            appWindowPresenter.IsMinimizable = false;
            app = curApp;
            LoadUp();
        }

        private void SaveEditedContent_Click(object sender, RoutedEventArgs e)
        {
            DetailsChanged?.Invoke(this, new Edits()
            {
                Name = TitleBox.Text,
                IconPath = path,
                Author = AuthorBox.Text,
                Description = DescBox.Text,
                AdminRights = AdminBox.IsChecked ?? false,
                Arguments = ArgsBox.Text
            });
            this.Close();
        }
        

        private async void ChooseIcon_Click(object sender, RoutedEventArgs e)
        {
            path = await sys.OpenFilePicker(this, ".png");
        }

        private void LoadUp()
        {
            path = app.IconPath;
            TitleBox.Text = app.Name;
            PathText.Text = app.IconPath;
            AuthorBox.Text = app.Author;
            DescBox.Text = app.Description;
            AdminBox.IsChecked = app.AdminRights;
            ArgsBox.Text = app.Arguments;
        }
    }
}
