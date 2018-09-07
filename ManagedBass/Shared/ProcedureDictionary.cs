using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagedBass
{
    class ReferenceHolder
    {
        readonly Dictionary<Tuple<int, int>, object> _procedures = new Dictionary<Tuple<int, int>, object>();
        readonly SyncProcedure _freeproc;

        public ReferenceHolder(bool Free = true)
        {
            if (Free)
                _freeproc = Callback;
        }

        public void Add(int Handle, int SpecificHandle, object proc)
        {
            if (proc.Equals(_freeproc))
                return;

            var key = Tuple.Create(Handle, SpecificHandle);

            var contains = _procedures.ContainsKey(key);

            if (proc == null)
            {
                if (contains)
                    _procedures.Remove(key);

                return;
            }

            if (_freeproc != null && !_procedures.Any(pair => pair.Key.Item1 == Handle))
                Bass.ChannelSetSync(Handle, SyncFlags.Free, 0, _freeproc);

            if (contains)
                _procedures[key] = proc;
            else _procedures.Add(key, proc);
        }

        public void Remove<T>(int Handle, int SpecialHandle)
        {
            var key = Tuple.Create(Handle, SpecialHandle);
            
            if (_procedures.ContainsKey(key) && _procedures[key].GetType() == typeof(T))
                _procedures.Remove(key);
        }

        void Callback(int Handle, int Channel, int Data, IntPtr User)
        {
            var toRemove = new List<Tuple<int, int>>();
            
            foreach (var pair in _procedures.Where(Pair => Pair.Key.Item1 == Channel))
                toRemove.Add(pair.Key);
            
            foreach (var key in toRemove)
                _procedures.Remove(key);
        }
    }
}
