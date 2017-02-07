using System.IO;
using NuGet.Modules;

namespace CtrlO.Models.States
{
    internal class StateProvider
    {
        private static readonly string FileName = "state.xml";

        public State Load()
        {
            if (File.Exists(FileName))
            {
                return XmlHelper.Deserialize<StateData>(FileName).ToEntity();
            }
            return CreateDefault();
        }

        public void Save(State state)
        {
            XmlHelper.Serialize(FileName, state.ToData());
        }

        public State CreateDefault()
        {
            return StateExtentions.CreateDefault().ToEntity();
        }
    }
}