﻿using System.Windows.Input;
using CtrlO.Mvvm.Commands;

namespace CtrlO
{
    public class UrlModel
    {
        public UrlModel(FileModel parent)
        {
            OpenCommand = new DelegateCommand(parent.Parent.Open);
            RemoveCommand = new DelegateCommand(parent.Remove);
        }

        public int Index { get; set; }
        public string Value { get; set; }

        public ICommand OpenCommand { get; }
        public ICommand RemoveCommand { get; }

        public override string ToString()
        {
            return $"{Index}. {Value}";
        }
    }
}