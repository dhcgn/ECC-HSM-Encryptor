using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.HockeyApp;

namespace EHF.Presentation
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Microsoft.HockeyApp.HockeyClient.Current.Configure("9fb8c20ccd9b45f8aad2c9c192bf92f2");
        }
    }
}
