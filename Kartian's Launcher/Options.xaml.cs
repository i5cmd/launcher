using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Kartian_s_Launcher;

public class ProgramConfiguration
{
    public bool WindowXButton { get; set; } = true;
    public bool AltHome { get; set; } = true;
}

public sealed partial class Options : Window
{
    private JsonManagement json;
    private ProgramConfiguration config;
    private ProgramConfiguration backup;
    private bool saved = false;
    public event EventHandler<ProgramConfiguration> SendConfig;
    public Options(ProgramConfiguration pc)
    {
        json = new JsonManagement();
        InitializeComponent();
        this.AppWindow.MoveAndResize(new RectInt32
        {
            Height = 450,
            Width = 490
        });
        var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
        appWindowPresenter.IsResizable = false;
        appWindowPresenter.IsAlwaysOnTop = true;
        appWindowPresenter.IsMaximizable = false;
        appWindowPresenter.IsMinimizable = false;
        config = pc;
        backup = pc;
        this.Closed += Options_Closed;
        LoadUp();
    }

    private void Options_Closed(object sender, WindowEventArgs args)
    {
        if (!saved)
        {
            config = backup;
        }
    }

    private void SaveEditedContent_Click(object sender, RoutedEventArgs e)
    {
        ProgramConfiguration newConfig = new ProgramConfiguration()
        {
            WindowXButton = CloseWin.IsOn,
            AltHome = HotkeyWin.IsOn
        };
        SendConfig?.Invoke(this, newConfig);
        json.SaveOptions(newConfig);
        this.Close();
    }

    private void LoadUp() // i'll do the same for edit window!!!
    {
        CloseWin.IsOn = config.WindowXButton;
        HotkeyWin.IsOn = config.AltHome;
    }
}
