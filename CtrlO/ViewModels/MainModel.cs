﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CtrlO.Models.States;
using CtrlO.Mvvm;
using CtrlO.Mvvm.Commands;

namespace CtrlO.ViewModels
{
    public class MainModel : BindableBase
    {
        private FileModel _selectedFile;

        public MainModel(State state)
        {
            State = state;
            try
            {
                var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.txt");
                var urlSateMap = files.ToDictionary(key => key, file =>
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    return State.Urls.SingleOrDefault(url => url.File == name) ?? new UrlSate {File = name};
                });
                Files = files.Select(file => new FileModel(this, file, urlSateMap[file])).ToArray();
                State.Urls = urlSateMap.Values.OrderBy(us => us.File).ToArray();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetBaseException().Message);
                Application.Current.Shutdown();
            }

            SelectedFile = Files.SingleOrDefault(file => file.Name == State.File) ?? Files.FirstOrDefault();
            NextCommand = new DelegateCommand(Next, CanOpen);
        }

        public State State { get; }

        public FileModel[] Files { get; }

        public FileModel SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                SetProperty(ref _selectedFile, value);
                State.File = value?.Name;
            }
        }

        public ICommand NextCommand { get; }

        private void Next()
        {
            Open();
            SelectedFile.SelectNext();
        }

        public void Open()
        {
            try
            {
                Process.Start(SelectedFile.SelectedUrl.Value);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetBaseException().Message);
            }

        }

        private bool CanOpen()
        {
            return SelectedFile?.SelectedUrl != null;
        }
    }
}