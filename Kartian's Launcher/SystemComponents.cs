using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT.Interop;
using System.ComponentModel;
using Windows.UI.Notifications;
using Microsoft.UI.Xaml.Controls;
using Windows.Media.Core;

namespace Kartian_s_Launcher
{
    public class SystemComponents
    {
        public class ProcessErrorsExecutive
        {
            public bool runs { get; set; }
            public string errormessage { get; set; }
            public int code { get; set; }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        public void ModalWindow(object target, Window window)
        {
            IntPtr hwnd = WindowNative.GetWindowHandle(target);
            EnableWindow(hwnd, false);
            window.Closed += (s, args) =>
            {
                EnableWindow(hwnd, true);
            };
            window.Activate();
        }
        public async Task<string> OpenFilePicker(object target, string fileType)
        {
            var picker = new FileOpenPicker();

            IntPtr hwnd = WindowNative.GetWindowHandle(target);
            InitializeWithWindow.Initialize(picker, hwnd);

            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add(fileType);

            var file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                return null;
            }
            return file.Path;
        }

        public ProcessErrorsExecutive RunProcess(string path, string launchoptions, bool admin) // launchoptions for the future, dw dudes!
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                return new ProcessErrorsExecutive()
                {
                    runs = false,
                    errormessage = $"-2 - The path to the file is either null or doesn't contain anything. Fix the data file.",
                    code = -2
                };

            }
            if (!File.Exists(path))
            {
                return new ProcessErrorsExecutive()
                {
                    runs = false,
                    errormessage = "-3 - The file doesn't exist in directory.",
                    code = -3
                };
            }

            try
            {
                Process.Start(CreateProcess(path, launchoptions, admin));
                return new ProcessErrorsExecutive()
                {
                    runs = true,
                    errormessage = "Why are you so interested into that?? Everything runs well! Now shut up."
                };
            }
            catch (Win32Exception ex)
            {
                return new ProcessErrorsExecutive()
                {
                    runs = false,
                    errormessage = $"{ex.ErrorCode} - {ex.Message.ToString()}",
                    code = ex.ErrorCode
                };
            }
        }

        public async Task ShowErrorMessages(UIElement target, string message, string title)
        {
            if (String.IsNullOrEmpty(title))
            {
                title = "Error! x_X";
            }
            ContentDialog dialog = new ContentDialog()
            {
                Title = title,
                Content = $"An error occured: {message}",
                CloseButtonText = "OK",
                XamlRoot = target.XamlRoot

            };

            var result = await dialog.ShowAsync();
        }

        public async Task<bool> ShowDialog(UIElement target, string title, string message)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = title,
                Content = message,
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No",
                XamlRoot = target.XamlRoot
            };

            ContentDialogResult result = await dialog.ShowAsync();

            return result == ContentDialogResult.Primary;
        }

        private ProcessStartInfo CreateProcess(string path, string launchoptions, bool admin)
        {
            ProcessStartInfo process = new ProcessStartInfo();
            process.FileName = path;
            process.Arguments = launchoptions;
            if (admin)
            {
                process.UseShellExecute = true;
                process.Verb = "runas";
            }
            return process;
        }
    }
}
