using System.Linq;

namespace CtrlO.States
{
    internal static class StateExtentions
    {
        public static State ToEntity(this StateData data)
        {
            return new State
            {
                File = data.File,
                Urls = data.Urls.Select(url => url.ToEntity()).ToArray()
            };
        }

        public static StateData ToData(this State state)
        {
            return new StateData
            {
                File = state.File,
                Urls = state.Urls.Select(url => url.ToData()).ToArray()
            };
        }

        public static UrlSate ToEntity(this UrlSateData data)
        {
            return new UrlSate
            {
                File = data.File,
                Value = data.Value
            };
        }

        public static UrlSateData ToData(this UrlSate state)
        {
            return new UrlSateData
            {
                File = state.File,
                Value = state.Value
            };
        }

        public static StateData CreateDefault()
        {
            return new StateData
            {
                File = "",
                Urls = new UrlSateData[0]
            };
        }
    }
}