namespace CtrlO
{
    public class OpenModel
    {
        public int Index { get; set; }
        public string Url { get; set; }

        public override string ToString()
        {
            return $"{Index}. {Url}";
        }
    }
}