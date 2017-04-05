using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hmm.Base;



namespace Hmm.Decoding {

    class ViterbiDecoding<Alphabet> : BaseHmmDecoding<Alphabet> where Alphabet : IEquatable<Alphabet>, IComparable<Alphabet> {

        public ViterbiDecoding(BaseMarkovModel<Alphabet> Model)
            : base(Model) {
        }

        protected override void calculateLogProbsArray(IList<Alphabet> Sequence) {

            HmmStartState<Alphabet> start = model.StartState;

            foreach (HmmState<Alphabet> state in model.States)
                if (!(state is IHmmEndState)) {
                    double a = model.Transitions[start][state].LogProbability;
                    logProbs[state][0] = a;
                }

            for (int i = 1; i < Sequence.Count; i++) {

                Alphabet emission = Sequence[i - 1];

                foreach (HmmState<Alphabet> nextState in model.States)
                    if (!(nextState is IHmmEndState)) {

                        double emitProb = model.Emissions[nextState][emission].LogProbability;

                        double maxProp = Double.NegativeInfinity;

                        foreach (HmmState<Alphabet> priorState in model.States)
                            if (!(priorState is IHmmEndState)) {
                                double prevV = logProbs[priorState][i - 1];
                                double transition = model.Transitions[priorState][nextState].LogProbability;

                                maxProp = Math.Max(maxProp, prevV + transition);
                            }

                        logProbs[nextState][i] = maxProp + emitProb;
                    }
            }
        }



    }

}
