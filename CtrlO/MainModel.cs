using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CtrlO.Mvvm;
using CtrlO.Mvvm.Commands;

namespace CtrlO
{
    public class MainModel : BindableBase
    {
        private bool _selectNext = true;
        private OpenModel _selectedOpen;

        public MainModel()
        {
            try
            {
                Opens = File.ReadAllLines("open.txt").Select((item, index) => new OpenModel
                {
                    Index = index + 1,
                    Url = item
                }).ToArray();
            }
            catch (Exception e)
            {
                Opens = new OpenModel[0];
                MessageBox.Show(e.GetBaseException().Message);
            }
           
            SelectedOpen = Opens.FirstOrDefault();
            OpenCommand = new DelegateCommand(Open, CanOpen);
        }

        public OpenModel[] Opens { get; set; }

        public OpenModel SelectedOpen
        {
            get { return _selectedOpen; }
            set { SetProperty(ref _selectedOpen, value); }
        }

        public bool SelectNext
        {
            get { return _selectNext; }
            set { SetProperty(ref _selectNext, value); }
        }

        public ICommand OpenCommand { get; }

        private void Open()
        {
            try
            {
                Process.Start(SelectedOpen.Url);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetBaseException().Message);
            }
            
            if (SelectNext)
            {
                SelectedOpen = SelectedOpen.Index < Opens.Length ? Opens[SelectedOpen.Index] : null;
            }
        }

        private bool CanOpen()
        {
            return SelectedOpen != null;
        }
    }
}