using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using CtrlO.Mvvm;
using CtrlO.States;

namespace CtrlO
{
    public class FileModel : BindableBase
    {
        private readonly UrlSate _sate;
        private UrlModel _selectedUrl;

        public FileModel(string file, UrlSate sate)
        {
            _sate = sate;
            Name = Path.GetFileNameWithoutExtension(file);
            try
            {
                var hashSet = new HashSet<string>();
                Urls = File.ReadAllLines(file).Select((item, index) =>
                {
                    var model = new UrlModel
                    {
                        Index = index + 1,
                        Value = item,
                        Bad = hashSet.Contains(item)
                    };
                    hashSet.Add(item);
                    return model;
                }).ToArray();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetBaseException().Message);
                Urls = new UrlModel[0];
            }
            SelectedUrl = Urls.FirstOrDefault(url => _sate.Value == url.Value) ?? Urls.FirstOrDefault();
        }

        public string Name { get; set; }

        public UrlModel[] Urls { get; set; }

        public UrlModel SelectedUrl
        {
            get { return _selectedUrl; }
            set
            {
                SetProperty(ref _selectedUrl, value);
                _sate.Value = value?.Value;
            }
        }

        public void SelectNext()
        {
            SelectedUrl = SelectedUrl.Index < Urls.Length ? Urls[SelectedUrl.Index] : null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}