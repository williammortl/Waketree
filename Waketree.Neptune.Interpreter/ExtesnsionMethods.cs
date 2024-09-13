namespace Waketree.Neptune.Interpreter
{
    public static class ExtesnsionMethods
    {
        public static Stack<T> DeepCloneStack<T>(this Stack<T> stackToClone) where T : ICloneable
        {
            var stackList = stackToClone.ToList<T>();
            stackList.Reverse();
            var clonedStack = new Stack<T>();
            foreach(var item in stackList)
            {
                clonedStack.Push((T)((ICloneable)item).Clone());
            }
            return clonedStack;
        }
    }
}
