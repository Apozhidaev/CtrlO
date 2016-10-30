using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CtrlO.States;

namespace CtrlO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private State _state;
        private MainModel _model;

        private void OnExit(object sender, ExitEventArgs e)
        {
            if (_model.State.HasDiff(_state))
            {
                var stateProvider = new StateProvider();
                stateProvider.Save(_model.State);
            }
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var stateProvider = new StateProvider();
            _state = stateProvider.Load();
            _model = new MainModel(_state.Clone());
            var main = new MainWindow
            {
                DataContext = _model
            };
            main.Show();
        }
    }
}
