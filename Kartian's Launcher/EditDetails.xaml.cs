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
        public string Name { get; set; }
        public string IconPath { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
    }
    public sealed partial class EditDetails : Window
    {
        public event EventHandler<Edits> DetailsChanged;
        private string path = "C:\\Users\\Karty\\source\\repos\\Kartian's Launcher\\Kartian's Launcher\\Assets\\generic.webp";
        private SystemComponents sys;
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
            path = curApp.IconPath;
            TitleBox.Text = curApp.Name;
            PathText.Text = curApp.IconPath;
            AuthorBox.Text = curApp.Author;
            DescBox.Text = curApp.Description;
        }

        private void SaveEditedContent_Click(object sender, RoutedEventArgs e)
        {
            DetailsChanged?.Invoke(this, new Edits()
            {
                Name = TitleBox.Text,
                IconPath = path,
                Author = AuthorBox.Text,
                Description = DescBox.Text
            });
            this.Close();
        }
        

        private async void ChooseIcon_Click(object sender, RoutedEventArgs e)
        {
            path = await sys.OpenFilePicker(this, ".png");
        }
    }
}
