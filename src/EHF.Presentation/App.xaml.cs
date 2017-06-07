using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
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
            EmbeddedLibsResolver.Init();
            InitHockeyClient();
            InitializeDispatcherHelper();
            CheckInstalled();
        }

        private  void CheckInstalled()
        {
            // Todo move to lib
            var filepath = Environment.Is64BitProcess 
                ? @"C:\Windows\System32\opensc-pkcs11.dll" 
                : @"C:\Windows\syswow64\opensc-pkcs11.dll";

            if (!File.Exists(filepath))
            {
                MessageBox.Show($"File \"{filepath}\"doen't exist, but is necessary to communicate with nitro key hsm. Please download OpenSC from \"https://sourceforge.net/projects/opensc/files/OpenSC/opensc-0.16.0/\"");
                this.Shutdown(-1);
            }
        }

        private static void InitializeDispatcherHelper()
        {
            DispatcherHelper.Initialize();
        }

        private static void InitHockeyClient()
        {
            Microsoft.HockeyApp.HockeyClient.Current.Configure("9fb8c20ccd9b45f8aad2c9c192bf92f2");
        }

        private static class EmbeddedLibsResolver
        {
            public static void Init()
            {
                var assemblies = new Dictionary<string, Assembly>();
                var executingAssembly = Assembly.GetExecutingAssembly();
                var resources = executingAssembly.GetManifestResourceNames().Where(n => n.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase));

                foreach (var resource in resources)
                {
                    using (var stream = executingAssembly.GetManifestResourceStream(resource))
                    {
                        using (var memstream = new MemoryStream())
                        {
                            stream.CopyTo(memstream);

                            assemblies.Add(resource, Assembly.Load(memstream.ToArray()));
                        }
                    }
                }

                AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
                {
                    Console.Out.WriteLine("AssemblyResolve Name: " + e.Name);

                    var assemblyName = new AssemblyName(e.Name);
                    var path = $"{assemblyName.Name}.dll";

                    return assemblies.ContainsKey(path) ? assemblies[path] : null;
                };
            }
        }
    }
}
