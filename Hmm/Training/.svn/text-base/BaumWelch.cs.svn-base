using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hmm.Base;
using Hmm.Decoding;

namespace Hmm.Training {
    public abstract class BaseBaumWelch<Alphabet>
       where Alphabet : IEquatable<Alphabet>, IComparable<Alphabet> {

        protected BaseMarkovModel<Alphabet> model;
        protected FBDecoding<Alphabet> fb;
        protected List<IList<Alphabet>> TrainSets;
        BaseMarkovModel<Alphabet>.ModelTransitions A;

        public double PValueLog { 
            get { return fb.LogPx; } 
        }

        public BaseBaumWelch(BaseMarkovModel<Alphabet> model) {
            this.model = model;
            fb = new FBDecoding<Alphabet>(model);
            TrainSets = new List<IList<Alphabet>>();
           
            
        }

        public void AddTrainSet(IList<Alphabet> TrainSet) {
            this.TrainSets.Add(TrainSet);
        }

        public void Train(int Iterations) {
            for (int i = 0; i < Iterations; i++)
                foreach (IList<Alphabet> trainset in TrainSets) {
                    fb.Decode(trainset);
                    
                    ReestimateTransitionModelParameters(trainset);
                    ReestimateEmissionModelParameters(trainset);

                    SetNewModelTransitionParameters();
                    SetNewModelEmissionParameters(trainset);
                }
            
        }

        protected virtual void ReestimateTransitionModelParameters(IList<Alphabet> trainset) {
            A = new BaseMarkovModel<Alphabet>.ModelTransitions(model.StartState, model.States);

            for (int i = 0; i < trainset.Count - 1; i++)
                foreach (HmmState<Alphabet> k in model.States)
                    foreach (HmmState<Alphabet> l in model.States)
                        if (!(k is IHmmEndState || l is IHmmEndState)) {
                            Alphabet obs = trainset[i + 1];
                            double akl_log =
                                fb.LogForward[k][i] +
                                model.Transitions[k][l].LogProbability +
                                model.Emissions[l][obs].LogProbability +
                                fb.LogBackward[k][i + 1] -
                                fb.LogPx;

                            double akl = A[k][l].Probability + Math.Exp(akl_log);
                            A[k].setProbability(l, akl);
                        }

        }
        
        protected virtual void SetNewModelTransitionParameters() {
            foreach (HmmState<Alphabet> k in model.States)
                if (!(k is IHmmEndState)) {
                    double sum = 0.0;
                    foreach (HmmState<Alphabet> l in model.States)
                        if (!(l is IHmmEndState))
                            sum += A[k][l].Probability;

                    foreach (HmmState<Alphabet> l in model.States)
                        if (!(l is IHmmEndState))
                            model.Transitions[k].setProbability(l, A[k][l].Probability / sum);

                }
        }

        protected abstract void ReestimateEmissionModelParameters(IList<Alphabet> trainset);
        protected abstract void SetNewModelEmissionParameters(IList<Alphabet> trainset);


    }
}
