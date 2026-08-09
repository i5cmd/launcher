using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Kartian_s_Launcher
{
    public class ShortcutDetails
    {
        public AppDetails app { get; set; }
        public List<string> inputs { get; set; } // like alt, t or alt, space, r
        public string inputsOne { get; set; }
    }
    public sealed partial class Settings : Window
    {
        private List<ShortcutDetails> shortcuters;
        public Settings()
        {
            InitializeComponent();
            shortcuters = new List<ShortcutDetails>();
        }
    }
}
