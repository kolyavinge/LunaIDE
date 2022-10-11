﻿using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.Commands;

public interface IMainWindowClosedCommand : ICommand { }

public class MainWindowClosedCommand : Command, IMainWindowClosedCommand
{
    private readonly IEnvironmentWindowsManager _windowsManager;

    public MainWindowClosedCommand(IEnvironmentWindowsManager windowsManager)
    {
        _windowsManager = windowsManager;
    }

    public override void Execute(object parameter)
    {
        _windowsManager.CloseAllWindows();
    }
}