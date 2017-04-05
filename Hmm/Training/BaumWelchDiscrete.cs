using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hmm.Model;
using Hmm.Base;

namespace Hmm.Training {
    class BaumWelchDiscrete<Alphabet> : BaseBaumWelch<Alphabet>
        where Alphabet : IEquatable<Alphabet>, IComparable<Alphabet> {

        DiscreteModelEmissions<Alphabet> E;

        public BaumWelchDiscrete(DiscreteMarkovModel<Alphabet> model)
            : base(model) {
           
        }

        protected override void ReestimateEmissionModelParameters(IList<Alphabet> trainset) {
            E = new DiscreteModelEmissions<Alphabet>(model.States, ((DiscreteMarkovModel<Alphabet>)model).Symbols);

            for (int i = 0; i < trainset.Count; i++)
                foreach (HmmState<Alphabet> k in model.States)
                    if (!(k is IHmmEndState)) {
                        Alphabet obs = trainset[i];

                        double eki_log =
                            fb.LogForward[k][i] +
                            fb.LogBackward[k][i] -
                            fb.LogPx;
                        double eki = E[k][obs].Probability + Math.Exp(eki_log);
                        ((DiscreteStateEmissions<Alphabet>)E[k]).setProbability(obs, eki);
                    }
        }

        protected override void SetNewModelEmissionParameters(IList<Alphabet> trainset) {
            DiscreteMarkovModel<Alphabet> dm = (DiscreteMarkovModel<Alphabet>)model;

            foreach (HmmState<Alphabet> k in dm.States)
                if (!(k is IHmmEndState)) {
                    double sum = 0.0;

                    foreach (Alphabet s in dm.Symbols)
                        sum += E[k][s].Probability;

                    foreach (Alphabet s in dm.Symbols)
                        ((DiscreteStateEmissions<Alphabet>)model.Emissions[k]).setProbability(s, E[k][s].Probability / sum);

                }

        }
    }
}
