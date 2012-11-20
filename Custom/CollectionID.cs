using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Maze.Custom
{
    public class CollectionID<T> : IEnumerable
    {
        protected Dictionary<int, T> dictionary;
        protected T defaultValue;

        public T this[int id]
        {
            get
            {
                T val;
                if (dictionary.TryGetValue(id, out val))
                {
                    return val;
                }
                else return defaultValue;
            }
            set
            {
                if (value.Equals(defaultValue)) RemoveValue(id);
                else dictionary[id] = value;
            }
        }

        public CollectionID()
        {
            dictionary = new Dictionary<int,T>();
            defaultValue = default(T);
        }

        public CollectionID(T defValue) : this()
        {
            defaultValue = defValue;
        }

        public int GetIDs(out int[] ids)
        {
            ids = dictionary.Keys.ToArray();
            return ids.Count();
        }

        /// <summary>
        /// Remove all keys and values from the dictionary
        /// </summary>
        public void Clear()
        {
            dictionary.Clear();
        }

        /// <summary>
        /// Get key value and remove it from the dictionary
        /// </summary>
        /// <param name="id">Key in the dictionary</param>
        /// <returns></returns>
        public T ExtractValue(int id)
        {
            T val = this[id];
            RemoveValue(id);
            return val;
        }

        /// <summary>
        /// Remove the key-value pair from the dictionary
        /// </summary>
        /// <param name="id">Key in the dictionary</param>
        /// <returns></returns>
        public bool RemoveValue(int id)
        {
            if (dictionary.Remove(id))
                return true;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public CollectionEnum<T> GetEnumerator()
        {
            return new CollectionEnum<T>(dictionary);
        }
    }

    public class CollectionEnum<T> : IEnumerator
    {
        Dictionary<int, T> collection;

        int position = -1;

        public CollectionEnum(Dictionary<int, T> dictionary)
        {
            collection = dictionary;
        }

        public bool MoveNext()
        {
            position++;
            return (position < collection.Count);
        }

        public void Reset()
        {
            position = -1;
        }

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
                    return collection[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
