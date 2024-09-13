namespace Waketree.Neptune.Interpreter
{
    public class CriticalSection
    {
        public bool Value { get; set; }
        public object Lock { get; set; }

        public CriticalSection() 
        { 
            this.Value = false;
            this.Lock = new object();
        }
    }
}
