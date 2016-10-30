namespace CtrlO
{
    public class UrlModel
    {
        public int Index { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Index}. {Value}";
        }
    }
}