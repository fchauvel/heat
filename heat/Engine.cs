using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heat
{
    public class Level {

        private int roundCount;

        public Level(int roundCount)
        {
            this.roundCount = roundCount;
        }

        public ICollection<int> rounds()
        {
            return Enumerable.Range(0, roundCount).ToArray();
        }

    }

    public class Circuit {

        private string[] moves;

        public Circuit(string[] moves)
        {
            this.moves = moves;
        }

        public string[] GetMoves()
        {
            return this.moves;
        }

    }

    public class Session
    {
        private Circuit circuit;
        private Level level;
            
        public Session(Circuit circuit, Level level)
        {
            this.circuit = circuit;
            this.level = level; 
        }

        public void Run(Trainee trainee)
        { 
            Cursor<int> rounds = new Cursor<int>(level.rounds());
            while (rounds.HasNext())
            {
                GoThroughCircuit(trainee);
                rounds.Next();
                if (rounds.HasNext()) { trainee.Break(); }
            }
        }
         
        private void GoThroughCircuit(Trainee trainee)
        {
            Cursor<string> move = new Cursor<string>(circuit.GetMoves());
            while (move.HasNext())
            {
                String currentMove = circuit.GetMoves()[move.GetCurrent()];
                trainee.GoFor(currentMove);
                move.Next();
                if (move.HasNext()) { trainee.SwitchTo(); }
            }
        }
    }

    public class Trainee
    {
        public virtual void GoFor(String move) { }

        public virtual void Break() { }
        
        public virtual void SwitchTo() { }

    }
}
