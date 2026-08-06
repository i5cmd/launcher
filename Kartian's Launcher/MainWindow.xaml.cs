using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using WinRT.Interop;
using static Kartian_s_Launcher.AppAdding;
using static Kartian_s_Launcher.SystemComponents;

namespace Kartian_s_Launcher
{
    public sealed partial class MainWindow : Window
    {
        private AppAdding ap;
        private AppDetails curApp;
        private EditDetails edit;
        private int curIndex;
        public ObservableCollection<AppDetails> ProgramsDetails;
        private JsonManagement json;
        private SystemComponents sys;

        public MainWindow()
        {
            InitializeComponent();
            ap = new AppAdding();
            json = new JsonManagement();
            sys = new SystemComponents();
            ProgramsDetails = new ObservableCollection<AppDetails>(json.LoadJson());
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
                curApp = app;
                curIndex = Programs.SelectedIndex;
            }
        }

        private void FullScreen(AppDetails app)
        {
            Iconx2.Source = new BitmapImage(new Uri(app.IconPath));
            Titlex2.Text = app.Name;
            Authorx2.Text = app.Author;
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
            curApp.Name = e.Name;
            curApp.IconPath = e.IconPath;
            curApp.Author = e.Author;
            RefreshList();
        }

        private void RefreshList()
        {
            Programs.ItemsSource = null;
            Programs.ItemsSource = ProgramsDetails;
            Programs.SelectedItem = ProgramsDetails[curIndex];
            FullScreen(ProgramsDetails[curIndex]);
            json.SaveJson(ProgramsDetails);
        }

        private async void RunItem_Click(object sender, RoutedEventArgs e)
        {
            if (curApp != null)
            {
                ProcessErrorsExecutive pee = sys.RunProcess(curApp.Path, "");
                if (!pee.runs)
                {
                    await sys.ShowErrorMessages(this.Content, pee.errormessage);
                    if (pee.code == -2)
                    {
                        json.RemoveItem(curApp, ProgramsDetails);
                        curApp = null;
                    }
                }
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (curApp != null)
            {
                json.RemoveItem(curApp, ProgramsDetails);
                curApp = null;
            }
        }
    }
}
