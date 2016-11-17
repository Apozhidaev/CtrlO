using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly string _file;
        private readonly UrlSate _sate;
        private UrlModel _selectedUrl;
        private readonly List<UrlModel> _history = new List<UrlModel>();
        private int _historyIndex = 0;
        private UrlModel _curentUrl = null;

        public FileModel(MainModel parent, string file, UrlSate sate)
        {
            Parent = parent;
            _file = file;
            _sate = sate;
            Name = Path.GetFileNameWithoutExtension(file);
            try
            {
                Urls = new ObservableCollection<UrlModel>(File.ReadAllLines(file)
                    .Distinct()
                    .Where(line => !string.IsNullOrEmpty(line))
                    .Select((item, index) =>
                {
                    var model = new UrlModel(this)
                    {
                        Index = index + 1,
                        Value = item
                    };
                    return model;
                }));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetBaseException().Message);
                Urls = new ObservableCollection<UrlModel>();
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

        public ObservableCollection<UrlModel> Urls { get; set; }

        public UrlModel SelectedUrl
        {
            get { return _selectedUrl; }
            set
            {
                if (_selectedUrl != value)
                {
                    _history.Add(_selectedUrl);
                    _historyIndex = _history.Count;
                    _curentUrl = value;
                    SelectUrl(value);
                }
            }
        }

        public void SelectNext()
        {
            SelectedUrl = SelectedUrl.Index < Urls.Count
                ? Urls[SelectedUrl.Index]
                : null;
        }

        private void Back()
        {
            --_historyIndex;
            SelectUrl(_history[_historyIndex]);
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
            SelectUrl(url);
        }

        private bool CanForward()
        {

            return _historyIndex < _history.Count;
        }

        public void Remove()
        {
            var url = SelectedUrl;
            _history.RemoveAll(u => u == url);
            _historyIndex = _history.Count;
            _curentUrl = null;
            SelectUrl(null);
            Urls.Remove(url);
            try
            {
                var lines = File.ReadAllLines(_file).Where(line => line != url.Value);
                File.WriteAllLines(_file, lines);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetBaseException().Message);
            }
           
        }

        public override string ToString()
        {
            return Name;
        }

        private void SelectUrl(UrlModel url)
        {
            _selectedUrl = url;
            _sate.Value = url?.Value;
            OnPropertyChanged(() => SelectedUrl);
        }
    }
}