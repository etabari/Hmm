using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hmm.Base {


    public abstract class BaseHmmDecoding<Alphabet> where Alphabet : IEquatable<Alphabet>, IComparable<Alphabet> {
        protected BaseMarkovModel<Alphabet> model;

        protected IList<HmmState<Alphabet>> pi;
        protected Dictionary<HmmState<Alphabet>, IList<double>> logProbs;


        public IList<HmmState<Alphabet>> PredictedStatePath {
            get { return pi; }
        }

        public Dictionary<HmmState<Alphabet>, IList<double>> LogPobabilities {
            get { return logProbs; }
        }

        public BaseHmmDecoding(BaseMarkovModel<Alphabet> Model) {
            this.model = Model;
            logProbs = new Dictionary<HmmState<Alphabet>, IList<double>>(model.States.Count);

        }

        public virtual void Decode(IList<Alphabet> Sequence) {
            pi = new HmmState<Alphabet>[Sequence.Count];
            InitializeLogTable(Sequence, logProbs);
            calculateLogProbsArray(Sequence);
            calculatePathFromLogArray();
        }


        protected void InitializeLogTable(IList<Alphabet> Sequence, Dictionary<HmmState<Alphabet>, IList<double>> logTable) {
            foreach (var state in model.States)
                if (!(state is IHmmEndState))
                    logTable[state] = new double[Sequence.Count];

        }


        protected abstract void calculateLogProbsArray(IList<Alphabet> Sequence);

        protected virtual void calculatePathFromLogArray() {
            for (int x = 0; x < pi.Count; x++) {

                var maxState = model.States.First();
                foreach (var state in model.States)

                    if (!(state is IHmmEndState) && logProbs[maxState][x] < logProbs[state][x])
                        maxState = state;

                pi[x] = maxState;

            }
        }


    }
}
