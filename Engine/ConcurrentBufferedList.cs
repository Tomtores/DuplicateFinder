using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace Engine
{
    internal class ConcurrentBufferedList<T> : IDuplicateResults<T>
    {
        private ConcurrentBag<T> items = new ConcurrentBag<T>();
        private bool listTouched;
        private IEnumerable<T> listBuffer;

        public IEnumerable<T> Results
        {
            get
            {
                if (listTouched)
                {
                    listBuffer = this.items.ToList();    //todo make sure we return a deep copy so no one outside can mess with contents. todo2: this sometimes throws an array not long enough exception(?); 
                    listTouched = false;
                }

                return listBuffer;
            }
        }
        public void AddItems(IEnumerable<T> items)
        {
            foreach (var item in items)
            {                
                this.items.Add(item);                
            }

            listTouched = true;
        }

        public void Clear()
        {
            items = new ConcurrentBag<T>();
            listTouched = true;
        }

        public void Replace(IEnumerable<T> items)
        {
            this.items = new ConcurrentBag<T>(items);
            listTouched = true;
        }
    }
}
