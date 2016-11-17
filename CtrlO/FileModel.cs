using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CtrlO.Mvvm;
using CtrlO.Mvvm.Commands;
using CtrlO.States;

namespace CtrlO
{
    public class FileModel : BindableBase
    {
        private readonly UrlSate _sate;
        private UrlModel _selectedUrl;
        private readonly List<UrlModel> _history = new List<UrlModel>();
        private int _historyIndex = 0;
        private UrlModel _curentUrl = null;

        public FileModel(MainModel parent, string file, UrlSate sate)
        {
            Parent = parent;
            _sate = sate;
            Name = Path.GetFileNameWithoutExtension(file);
            try
            {
                Urls = File.ReadAllLines(file).Distinct().Select((item, index) =>
                {
                    var model = new UrlModel(this)
                    {
                        Index = index + 1,
                        Value = item
                    };
                    return model;
                }).ToArray();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetBaseException().Message);
                Urls = new UrlModel[0];
            }
            _selectedUrl = Urls.FirstOrDefault(url => _sate.Value == url.Value) ?? Urls.FirstOrDefault();
            _curentUrl = _selectedUrl;
            _sate.Value = _selectedUrl?.Value;
            BackCommand = new DelegateCommand(Back, CanBack);
            ForwardCommand = new DelegateCommand(Forward, CanForward);
        }

        public ICommand BackCommand { get; }
        public ICommand ForwardCommand { get; }

        public MainModel Parent { get; }

        public string Name { get; set; }

        public UrlModel[] Urls { get; set; }

        public UrlModel SelectedUrl
        {
            get { return _selectedUrl; }
            set
            {
                _history.Add(_selectedUrl);
                _historyIndex = _history.Count;
                _curentUrl = value;
                SetProperty(ref _selectedUrl, value);
                _sate.Value = value?.Value;
                
            }
        }

        public void SelectNext()
        {
            SelectedUrl = SelectedUrl.Index < Urls.Length ? Urls[SelectedUrl.Index] : null;
        }

        private void Back()
        {
            --_historyIndex;
            _selectedUrl = _history[_historyIndex];
            _sate.Value = _history[_historyIndex]?.Value;
            OnPropertyChanged(() => SelectedUrl);
        }

        private bool CanBack()
        {
            return _historyIndex > 0;
        }

        private void Forward()
        {
            ++_historyIndex;
            var url = _historyIndex == _history.Count 
                ? _curentUrl 
                : _history[_historyIndex];
            _selectedUrl = url;
            _sate.Value = url?.Value;
            OnPropertyChanged(() => SelectedUrl);
        }

        private bool CanForward()
        {
            
            return _historyIndex < _history.Count;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}