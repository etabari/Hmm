using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hmm.Base;


namespace Hmm {

    class MarkovChane<Alphabet> where Alphabet : IEquatable<Alphabet>, IComparable<Alphabet> {
        BaseMarkovModel<Alphabet> model;
        Random random;

        IList<HmmState<Alphabet>> trueStates;
        IList<Alphabet> sequence;

        public IList<Alphabet> Sequence {
            get { return sequence; }
        }

        public IList<HmmState<Alphabet>> TrueStates {
            get { return trueStates; }
        }

        public MarkovChane(BaseMarkovModel<Alphabet> Model) {
            this.model = Model;
            this.random = new Random(1);
        }

        public int Generate(int length) {
            sequence = new List<Alphabet>(length);
            trueStates = new List<HmmState<Alphabet>>(length);

            int c = 0;
            HmmState<Alphabet> state = model.StartState;
            state = model.Transitions[state].getNextTransition(random.NextDouble());

            while(c<length && !(state is IHmmEndState)) {
                Alphabet symbol = model.Emissions[state].getRandomObservation();
                trueStates.Add(state);
                sequence.Add(symbol);
                state = model.Transitions[state].getNextTransition(random.NextDouble());
                c++;
                if(c%1000 ==0)
                    Console.WriteLine(c);
            }


            return sequence.Count();
        }

    }
}
