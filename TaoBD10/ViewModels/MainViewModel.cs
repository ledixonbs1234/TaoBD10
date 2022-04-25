﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaoBD10.Manager;

namespace TaoBD10.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private Window _window;
        private Y2KeyboardHook _keyboardHook;
        private string _CountInBD;
        public string CountInBD { get => _CountInBD; set => SetProperty(ref _CountInBD, value); }

        private int _IndexTabControl = 0;

        public int IndexTabControl
        {
            get { return _IndexTabControl; }
            set { SetProperty(ref _IndexTabControl, value); }
        }

        public MainViewModel()
        {
            LoadPageCommand = new RelayCommand<Window>(LoadPage);
            TabChangedCommand = new RelayCommand<TabControl>(TabChanged);
            OnCloseWindowCommand = new RelayCommand(OnCloseWindow);

            _keyboardHook = new Y2KeyboardHook();
            _keyboardHook.OnKeyPressed += OnKeyPress;
            _keyboardHook.HookKeyboard();
            createConnection();
            SoundManager.SetUpDirectory();
            WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
            {
                if (m == "GoChiTiet")
                {
                    IndexTabControl = 2;
                }
            });
        }

        private void createConnection()
        {
            //var pathDB = System.IO.Path.Combine(Environment.CurrentDirectory, "dulieu.sqlite");
            //string _strConnect = @"DataSource=" + pathDB + ";Version=3";
            //_con = new SqliteConnection(_strConnect);
            //_con.ConnectionString = _strConnect;
            //_con.Open();
        }

        private void OnCloseWindow()
        {
            _keyboardHook.UnHookKeyboard();
        }

        private void OnKeyPress(object sender, KeyPressedArgs e)
        {
            CountInBD += e.KeyPressed.ToString();
            switch (e.KeyPressed)
            {
                case Key.F8:
                    //Thuc hien nay
                    WeakReferenceMessenger.Default.Send<MessageManager>(new MessageManager("getData"));
                    break;

                default:
                    break;
            }
        }

        private void TabChanged(TabControl control)
        {
            if (control == null)
                return;
            switch (control.SelectedIndex)
            {
                case 0:
                    //thuc hien chuyen ve
                    SetRightWindow();
                    break;

                case 1:
                    SetLargeRightWindow();

                    break;

                case 2:
                    SetCenterWindow();
                    break;

                case 3:
                    SetRightSmallWindow();
                    break;

                default:
                    break;
            }
        }

        private void SetRightSmallWindow()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 300;
            _window.Height = 500;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;
            // use 'Screen.AllScreens[1].WorkingArea' for secondary screen
            _window.Left = desktopWorkingArea.Left + width - _window.Width;
            _window.Top = desktopWorkingArea.Top + 0;
        }

        private void LoadPage(Window window)
        {
            _window = window;
            SetRightWindow();
        }

        private void SetRightWindow()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 360;
            _window.Height = 250;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;
            // use 'Screen.AllScreens[1].WorkingArea' for secondary screen
            _window.Left = desktopWorkingArea.Left + width - _window.Width;
            _window.Top = desktopWorkingArea.Top + 0;
        }

        private void SetLargeRightWindow()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 520;
            _window.Height = 500;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;
            // use 'Screen.AllScreens[1].WorkingArea' for secondary screen
            _window.Left = desktopWorkingArea.Left + width - _window.Width;
            _window.Top = desktopWorkingArea.Top + 0;
        }

        private void SetCenterWindow()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 1100;
            _window.Height = 600;
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            _window.Left = (width - 1000) / 2;
            _window.Top = (height - 600) / 2;
        }

        public ICommand LoadPageCommand { get; }
        public ICommand TabChangedCommand { get; }
        public ICommand OnCloseWindowCommand { get; }
    }
}