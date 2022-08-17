using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.IO;
using System.Collections;

namespace ViewSim
{

    //based on https://github.com/joaoportela/CircularBuffer-CSharp
    public class ListBuffer<T> : IEnumerable
    {
        public readonly T[] _buffer;

        public int _start;


        public int _end;


        private int _size;

        private int _lastIndex;

        public int _current;


        public ListBuffer(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentException(
                    "Circular buffer cannot have negative or zero capacity.", nameof(capacity));
            }
            _buffer = new T[capacity];
            _lastIndex = capacity - 1;
            Clear();
        }

        public void PrintItems()
        {
            Debug.Log("start " + _start);
            Debug.Log("end " + _end);
            Debug.Log("size " + _size);
            for (int i = 0; i < _buffer.Length; i++)
            {
                Debug.Log(_buffer[i]);
            }
        }

        // Implementation for the GetEnumerator method.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public ListBufferEnum<T> GetEnumerator()
        {
            return new ListBufferEnum<T>(this);
        }


        public T[] buffer { get { return _buffer; } }
        public int Count { get { return _size; } }

        public void Increment(ref int index)
        {
            if (++index > _lastIndex)
            {
                index = 0;
            }
        }
        public void Decrement(ref int index)
        {
            index--;
            if (index < 0)
            {
                index = _lastIndex;
            }

        }
        public bool IsEmpty()
        {
            return _size == 0;
        }
        public bool IsFull()
        {

            return _size == _lastIndex + 1;

        }
        private void ThrowIfEmpty(string message = "Cannot access an empty buffer.")
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException(message);
            }
        }

        public void PushBack(T item)
        {
            if (IsFull())
            {
                // _buffer[_end] = item;
                // Increment(ref _end);
                // _start = _end;
                Debug.LogError("size = " + _size + " max: " + (_lastIndex + 1));
                throw new InvalidOperationException("Push Back Buffer Full");
            }
            else
            {
                if (!IsEmpty())
                    Increment(ref _end);
                _buffer[_end] = item;

                ++_size;
            }
        }


        public void IncrementBackPtr()
        {
            if (IsFull())
            {
                // _buffer[_end] = item;
                // Increment(ref _end);
                // _start = _end;
                Debug.LogError("size = " + _size + " max: " + (_lastIndex + 1));
                throw new InvalidOperationException("Push Back Buffer Full");
            }
            else
            {
                if (!IsEmpty())
                    Increment(ref _end);
                ++_size;
            }
        }
		

		
        public void PushFront(T item)
        {
            if (IsFull())
            {
                // Decrement(ref _start);
                // _end = _start;
                // _buffer[_start] = item;
                throw new InvalidOperationException("Push Front Buffer Full");
            }
            else
            {
                if (!IsEmpty())
                    Decrement(ref _start);
                _buffer[_start] = item;
                ++_size;
            }
        }
        public void PopBack()
        {
            ThrowIfEmpty("Cannot take elements from an empty buffer.");
            Decrement(ref _end);
            // _buffer[_end] = default(T);
            --_size;
        }
        public void PopFront()
        {
            ThrowIfEmpty("Cannot take elements from an empty buffer.");
            // _buffer[_start] = default(T);
            Increment(ref _start);
            --_size;
        }
        public T Front()
        {
            ThrowIfEmpty();
            return _buffer[_start];
        }
        public T Back()
        {
            ThrowIfEmpty();
            return _buffer[_end];
        }

        public void CopyFrom(ListBuffer<T> l)
        {
            this._size = l._size;
            this._start = l._start;
            this._end = l._end;

            //Assume the same buffer length
            Array.Copy(l.buffer, _buffer, _lastIndex + 1);

        }

        //to match old list
        public void Add(T item)
        {
            PushBack(item);
        }

        public void Clear()
        {
            _size = 0;
            _start = _end = 0;
        }

        public void ClearToIndex(int index)
        {
            _size = 0;
            _start = _end =  InternalIndex(index);
        }

        public T Current()
        {
            return _buffer[_current];
        }

        /// <summary>
        /// Converts the index in the argument to an index in <code>_buffer</code>
        /// </summary>
        /// <returns>
        /// The transformed index.
        /// </returns>
        /// <param name='index'>
        /// External index.
        /// </param>
        private int InternalIndex(int index)
        {
            index += _start;
            index = index > _lastIndex ? index - _lastIndex - 1 : index;
            return index;
        }


        /// <summary>
        /// Index access to elements in buffer.
        /// Index does not loop around like when adding elements,
        /// valid interval is [0;Size[
        /// </summary>
        /// <param name="index">Index of element to access.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown when index is outside of [; Size[ interval.</exception>
        public T this[int index]
        {
            get
            {
                if (IsEmpty())
                {
                    throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty", index));
                }
                if (index >= _size)
                {
                    throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer size is {1}", index, _size));
                }
                int actualIndex = InternalIndex(index);
                return _buffer[actualIndex];
            }
            set
            {
                if (IsEmpty())
                {
                    throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty", index));
                }
                if (index >= _size)
                {
                    throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer size is {1}", index, _size));
                }
                int actualIndex = InternalIndex(index);
                _buffer[actualIndex] = value;
            }
        }


    }

    public class ListBufferEnum<T> : IEnumerator
    {
        public ListBuffer<T> _listBuffer;
        int steps;


        public ListBufferEnum(ListBuffer<T> listBuffer)
        {
            _listBuffer = listBuffer;
            steps = listBuffer.Count;
            //note: we always increment first, so start with -1
            _listBuffer._current = _listBuffer._start - 1;
        }

        public bool MoveNext()
        {

            _listBuffer.Increment(ref _listBuffer._current);
            steps--;
            // Debug.Log("steps " + steps);
            return (steps >= 0);
        }

        public void Reset()
        {
            _listBuffer._current = _listBuffer._start - 1;
            steps = _listBuffer.Count;
            //    Debug.Log("steps " + steps);
        }

        //object IEnumerator.Current
        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public T Current
        {
            get
            {
                try
                {
                    return _listBuffer.Current();
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }

}
