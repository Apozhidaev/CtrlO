using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CtrlO.Mvvm;
using CtrlO.Mvvm.Commands;
using CtrlO.States;

namespace CtrlO
{
    public class MainModel : BindableBase
    {
        private bool _auto = true;
        private bool _playing = false;
        private string _actionName = "Play";
        private FileModel _selectedFile;

        private static readonly string TempChrome = Path.Combine(Environment.CurrentDirectory, @"Temp\Chrome");

        static MainModel()
        {
            if (!Directory.Exists(TempChrome))
            {
                Directory.CreateDirectory(TempChrome);
            }
        }

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

        public bool Auto
        {
            get { return _auto; }
            set
            {
                SetProperty(ref _auto, value);
                ActionName = value ? "Play" : "Next";
            }
        }

        public bool Playing
        {
            get { return _playing; }
            set
            {
                SetProperty(ref _playing, value);
                ActionName = value ? "Stop" : "Play";
            }
        }

        public string ActionName
        {
            get { return _actionName; }
            set { SetProperty(ref _actionName, value); }
        }

        public ICommand NextCommand { get; }

        private async void Next()
        {
            if (!Auto)
            {
                try
                {
                    var url = SelectedFile.SelectedUrl;
                    SelectedFile.SelectNext();
                    await Run(url);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.GetBaseException().Message);
                }
            }
            else if(!Playing)
            {
                Play();
            }
            else
            {
                Playing = false;
            }
        }

        private async void Play()
        {
            Playing = true;
            while (CanOpen())
            {
                var wait = await Run(SelectedFile.SelectedUrl);
                if (Playing)
                {
                    SelectedFile.SelectNext();
                    if (wait < 100) break;
                }
                else
                {
                    return;
                }
                
            }
            Playing = false;
        }

        private Task<int> Run(UrlModel model)
        {
            return Task.Run(() =>
            {
                //var process = Process.Start(model.Value);

                var process = Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                    $"--user-data-dir=\"{TempChrome}\"  {model.Value}");
                var sw = new Stopwatch();
                sw.Start();
                process?.WaitForExit();
                sw.Stop();
                return sw.Elapsed.Milliseconds;
            });
        }

        public async void Open()
        {
            await Run(SelectedFile.SelectedUrl);
        }

        private bool CanOpen()
        {
            return SelectedFile?.SelectedUrl != null;
        }
    }
}