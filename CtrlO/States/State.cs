using System.Linq;

namespace CtrlO.States
{
    public class State
    {
        public string File { get; set; }

        public UrlSate[] Urls { get; set; }

        public State Clone()
        {
            return new State
            {
                File = File,
                Urls = Urls.Select(url => url.Clone()).ToArray()
            };
        }

        public bool HasDiff(State state)
        {
            return File != state.File 
                || Urls.Length != state.Urls.Length
                || Urls.Where((t, i) => t.HasDiff(state.Urls[i])).Any();
        }
    }
}