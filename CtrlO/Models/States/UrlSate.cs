namespace CtrlO.Models.States
{
    public class UrlSate
    {
        public string File { get; set; }

        public string Value { get; set; }

        public UrlSate Clone()
        {
            return new UrlSate
            {
                File = File,
                Value = Value
            };
        }

        public bool HasDiff(UrlSate state)
        {
            return File != state.File
                   || Value != state.Value;
        }
    }
}