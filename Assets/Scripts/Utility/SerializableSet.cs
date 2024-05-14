using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    [Serializable]
    public class SerializableSet<T> : IEnumerable<T>
    {
        [SerializeField] private List<T> items;
        
        public SerializableSet()
        {
            items = new List<T>();
        }

        public SerializableSet(IEnumerable<T> collection)
        {
            items = new List<T>(collection);
        }
        
        public bool Add(T item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                return true;
            }

            return false;
        }

        public bool Remove(T item)
        {
            return items.Remove(item);
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public int Count
        {
            get { return items.Count; }
        }

        public T this[int index]
        {
            get => items[index];
            set => items[index] = value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}