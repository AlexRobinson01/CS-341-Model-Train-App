﻿using ModelTrain.Screens;
namespace ModelTrain
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Login());

            //MainPage = new AppShell();

            //// Navigate to the login page when the app starts
            //Shell.Current.GoToAsync("//screen");
        }
    }
}
