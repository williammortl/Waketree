using Waketree.Neptune.Common.Bases;

namespace Waketree.Neptune.Common.Models
{
    public class NeptuneFrameData
    {
        public string ProcessID { get; set; }
        public Dictionary<string, int> LabelLookup { get; set; }
        public Dictionary<long, NeptuneValueBase> LocalVariables { get; set; }
        public Stack<NeptuneValueBase> Stack { get; set; }
    }
}
