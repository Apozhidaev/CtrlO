using System.Windows;
using System.Windows.Input;
using CtrlO.Mvvm.Commands;

namespace CtrlO
{
    public class UrlModel
    {
        public UrlModel(FileModel parent)
        {
            OpenCommand = new DelegateCommand(parent.Parent.Open);
            CopyCommand = new DelegateCommand(Copy);
            RemoveCommand = new DelegateCommand(parent.Remove);
        }

        public int Index { get; set; }
        public string Value { get; set; }

        public ICommand OpenCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand RemoveCommand { get; }

        private void Copy()
        {
            Clipboard.SetText(Value);
        }

        public override string ToString()
        {
            return $"{Index}. {Value}";
        }
    }
}