using Waketree.Neptune.Common.Bases;
using Waketree.Neptune.Common.Models;

namespace Waketree.Neptune.Common.Interfaces
{
    public interface IFrame
    {
        void Push(NeptuneValueBase data);

        NeptuneValueBase Pop();

        void StoreGlobal(long name, NeptuneValueBase? data);

        NeptuneValueBase? GetGlobal(long name);

        void StoreLocal(long name, NeptuneValueBase? data);

        NeptuneValueBase? GetLocal(long name);

        IFrame GetChildFrame();

        int LookupLabel(string label);

        bool EnterCriticalSection(long name);

        void ExitCriticalSection(long name);

        NeptuneFrameData GetFrameData();
    }
}
