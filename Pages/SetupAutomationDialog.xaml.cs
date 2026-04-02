using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MoscoviumThree.Pages;

public sealed partial class SetupAutomationDialog : UserControl
{
    private List<string> _selectedWingetApps = new();

    public SetupAutomationDialog()
    {
        this.InitializeComponent();
    }

    private void App_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox cb && cb.Tag is string appId)
        {
            if (!_selectedWingetApps.Contains(appId))
            {
                _selectedWingetApps.Add(appId);
            }
        }
    }

    private void App_Unchecked(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox cb && cb.Tag is string appId)
        {
            _selectedWingetApps.Remove(appId);
        }
    }

    public List<string> GetSelectedWingetApps() => _selectedWingetApps;
    
    public bool RunWindowsUpdate => ChkWindowsUpdate.IsChecked == true;
    public bool InstallVCRuntimes => ChkVCRuntimes.IsChecked == true;
    public bool RunChrisTitus => ChkChrisTitus.IsChecked == true;
    public bool RunRaphi => ChkRaphi.IsChecked == true;
}
