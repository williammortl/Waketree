namespace Waketree.Common.Models
{
    public class ProcessCreateArgument
    {
        public string Filename { get; set; }
        public string Runtime { get; set; }
        public string[] CommandArgs { get; set; }
    }
}
