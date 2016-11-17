using System.Linq;

namespace CtrlO.States
{
    public class State
    {
        public bool Auto { get; set; }

        public string File { get; set; }

        public UrlSate[] Urls { get; set; }

        public State Clone()
        {
            return new State
            {
                Auto = Auto,
                File = File,
                Urls = Urls.Select(url => url.Clone()).ToArray()
            };
        }

        public bool HasDiff(State state)
        {
            return Auto != state.Auto
                || File != state.File
                || Urls.Length != state.Urls.Length
                || Urls.Where((t, i) => t.HasDiff(state.Urls[i])).Any();
        }
    }
}