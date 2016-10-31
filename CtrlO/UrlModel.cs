using System.Windows;
using System.Windows.Media;

namespace CtrlO
{
    public class UrlModel
    {
        public int Index { get; set; }
        public string Value { get; set; }
        public bool Bad { get; set; }

        public Visibility BadVisibility => Bad ? Visibility.Visible : Visibility.Collapsed;

        public override string ToString()
        {
            return $"{Index}. {Value}";
        }
    }
}