// See http://www.gamasutra.com/blogs/WendelinReich/20131127/203843/C_Memory_Management_for_Unity_Developers_part_3_of_3.php
using System;
using System.Collections.Generic;

namespace ItchyOwl.Auxiliary
{
    public interface IRecyclable
    {
        void Init();
        void Reset();
    }

    public class GenericPooler<T> where T : class, IRecyclable, new()
    {
        private Stack<T> _objectStack;
        private Action<T> _resetAction;
        private Action<T> _initAction;

        public GenericPooler(int initBufferSize, Action<T> initAction = null, Action<T> resetAction = null)
        {
            _objectStack = new Stack<T>(initBufferSize);
            _resetAction = resetAction;
            _initAction = initAction;
        }

        public T New(Action<T> callback = null)
        {
            T obj;
            if (_objectStack.Count > 0)
            {
                obj = _objectStack.Pop();
            }
            else
            {
                obj = new T();
                _initAction(obj);
                obj.Init();
            }
            if (callback != null)
            {
                callback(obj);
            }
            return obj;
        }

        public void Recycle(T obj, Action<T> callback = null)
        {
            _resetAction(obj);
            obj.Reset();
            _objectStack.Push(obj);
            if (callback != null)
            {
                callback(obj);
            }
        }
    }
}

