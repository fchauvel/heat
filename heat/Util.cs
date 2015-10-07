using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heat
{
    class Cursor<T>
    {
        private ICollection<T> source;
        private int current;

        public Cursor(ICollection<T> source)
        {
            this.source = source;
            this.current = 0;
        }

        public bool HasNext()
        {
            return current < source.Count;
        }

        public int GetCurrent()
        {
            return current;
        }

        public void Next()
        {
            current = current + 1;
        }
    }
}
