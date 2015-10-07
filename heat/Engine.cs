using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace heat
{
    public class Circuit {

        private string[] moves;

        public Circuit(string[] moves)
        {
            this.moves = moves;
        }
    }

    public class Coach
    {
        public Coach() { }

        public void Run(Circuit circuit, Listener listener)
        {

        }
    }

    public class Listener
    {

    }
}
