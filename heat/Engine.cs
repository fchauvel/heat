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

    public class Circuit: IEnumerable<String> {

        private string[] moves;

        public Circuit(string[] moves)
        {
            this.moves = moves;
        }

        public IEnumerator<string> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public string[] GetMoves()
        {
            return this.moves;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach(string move in moves)
            {
                yield return move;
            }
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

        public void Run(Trainee listener)
        { 
            Cursor<int> rounds = new Cursor<int>(level.rounds());
            while (rounds.HasNext())
            {
                GoThroughCircuit(listener);
                rounds.Next();
                if (rounds.HasNext())
                {
                    listener.Relax();
                }
            }
        }

 
        private void GoThroughCircuit(Trainee trainee)
        {
            Cursor<string> move = new Cursor<string>(circuit.GetMoves());
            while (move.HasNext())
            {
                trainee.GoFor(circuit.GetMoves()[move.GetCurrent()]);
                move.Next();
                if (move.HasNext())
                {
                    trainee.Relax();
                }
            }
        }
    }

    public class Trainee
    {
        public virtual void GoFor(String move) { }

        public virtual void Relax() { }

    }
}
