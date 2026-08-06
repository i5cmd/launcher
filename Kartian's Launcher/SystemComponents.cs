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

        public ProcessErrorsExecutive RunProcess(string path, string launchoptions) // launchoptions for the future, dw dudes!
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
                Process.Start(path);
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

        public async Task ShowErrorMessages(UIElement target, string message)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = "Error! x_X",
                Content = $"An error occured: {message}",
                CloseButtonText = "OK",
                XamlRoot = target.XamlRoot

            };

            var result = await dialog.ShowAsync();
        }
    }
}
