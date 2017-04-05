using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hmm.Base;

namespace Hmm.Decoding {


    public class FBDecoding<Alphabet> : BaseHmmDecoding<Alphabet> where Alphabet : IEquatable<Alphabet>, IComparable<Alphabet> {

        /// <summary>
        /// exit probability = 3.72e-44.
        /// As proved before, any number works.
        /// </summary>
        public const double DefaultEndStateTransitionIfnotSpecified = 0;

        private double logPx;
        private Dictionary<HmmState<Alphabet>, IList<double>> logForward;
        private Dictionary<HmmState<Alphabet>, IList<double>> logBackward;

        public double LogPx {
            get { return logPx; }
        }

        public Dictionary<HmmState<Alphabet>, IList<double>> LogForward {
            get { return logForward; }
        }

        public Dictionary<HmmState<Alphabet>, IList<double>> LogBackward {
            get { return logBackward; }
        }

        public FBDecoding(BaseMarkovModel<Alphabet> model)
            : base(model) {
        }

        protected override void calculateLogProbsArray(IList<Alphabet> Sequence) {
            logForward = new Dictionary<HmmState<Alphabet>, IList<double>>(model.States.Count);
            logBackward = new Dictionary<HmmState<Alphabet>, IList<double>>(model.States.Count);

            InitializeLogTable(Sequence, logForward);
            InitializeLogTable(Sequence, logBackward);

            logPx = calculateForwardLogArray(Sequence);
            double px_2 = calculateBackwardLogArray(Sequence);

            //Console.WriteLine("==========P(X)============");
            //Console.WriteLine(px_1);
            //Console.WriteLine(px_2);

            if (Math.Abs(logPx - px_2) > 1)
                throw new ArithmeticException("P(X) from Forward and backward do not match for " + Math.Exp(logPx - px_2).ToString());
            //Console.WriteLine("P(X) from Forward and backward do not match for e^" +  Math.Exp(logPx - px_2).ToString() +" "+logPx +" "+px_2);

            foreach (var state in model.States)
                if (!(state is IHmmEndState))
                    for (int i = 0; i < Sequence.Count; i++) {
                        logProbs[state][i] = logForward[state][i] + logBackward[state][i] - logPx;
                        //if (i==5)
                        //    Console.WriteLine(state.Name+Math.Exp(logProbs[state][i]));
                    }
        }

        private double calculateForwardLogArray(IList<Alphabet> Sequence) {

            foreach (HmmState<Alphabet> state in model.States)
                if (!(state is IHmmEndState)) {
                    Alphabet symbol = Sequence[0];
                    double transition = model.Transitions[model.StartState][state].LogProbability;
                    double emission = model.Emissions[state][symbol].LogProbability;
                    logForward[state][0] = transition + emission;

                    //if(double.IsNaN(logForward[state][0]))
                    //    throw new ArithmeticException("Forward - 0");
                    
                }

            for (int i = 1; i < Sequence.Count; i++) {

                Alphabet symbol = Sequence[i];

                foreach (HmmState<Alphabet> nextState in model.States)
                    if (!(nextState is IHmmEndState)) {

                        double emission = model.Emissions[nextState][symbol].LogProbability;

                        double sumPrior = Double.NegativeInfinity;

                        foreach (HmmState<Alphabet> priorState in model.States)
                            if (!(priorState is IHmmEndState)) {

                                double f_prior = logForward[priorState][i - 1];
                                double transition = model.Transitions[priorState][nextState].LogProbability;

                                sumPrior = ProbabilityValue.addInLogSpace(sumPrior, f_prior + transition);
                            }

                        logForward[nextState][i] = sumPrior + emission;
                                            
                        //if(double.IsNaN(logForward[nextState][i] ))
                        //    throw new ArithmeticException("Forward - " + nextState + "-" + i);

                    }
            }

            double px = Double.NegativeInfinity;
            foreach (HmmState<Alphabet> state in model.States)
                if (!(state is IHmmEndState)) {
                    ///BUGFIX: Although at last, it should be multiplied by transition probabilities, 
                    ///       if the model, does not have an explicit end point, it should not be done.
                    ///       it causes 'tr' more in the P(X) of forward. 
                    ///       although it does not change anything in posterior probability.
                    double tr = 0;//DefaultEndStateTransitionIfnotSpecified;
                    if (model.HasEndState)
                        tr = model.Transitions[state][model.EndState].LogProbability;
                    px = ProbabilityValue.addInLogSpace(px, logForward[state][Sequence.Count - 1] + tr);
                }
            return px;
        }

        private double calculateBackwardLogArray(IList<Alphabet> Sequence) {

            int L = Sequence.Count - 1;

            foreach (HmmState<Alphabet> state in model.States)
                if (!(state is IHmmEndState)) {
                    double transition = DefaultEndStateTransitionIfnotSpecified;
                    if (model.HasEndState)
                        transition = model.Transitions[state][model.EndState].LogProbability;
                    logBackward[state][L] = transition;
                }

            for (int i = L - 1; i >= 0; i--) {

                Alphabet emission = Sequence[i + 1];

                foreach (HmmState<Alphabet> state in model.States)
                    if (!(state is IHmmEndState)) {

                        double sumPrior = Double.NegativeInfinity;

                        foreach (HmmState<Alphabet> nextState in model.States)
                            if (!(nextState is IHmmEndState)) {


                                double transition = model.Transitions[state][nextState].LogProbability;
                                double emitProb = model.Emissions[nextState][emission].LogProbability;
                                double b_next = logBackward[nextState][i + 1];

                                sumPrior = ProbabilityValue.addInLogSpace(sumPrior, transition + emitProb + b_next);
                            }

                        logBackward[state][i] = sumPrior;
                    }
            }

            double px = Double.NegativeInfinity;
            foreach (HmmState<Alphabet> state in model.States)
                if (!(state is IHmmEndState)) {
                    Alphabet emission = Sequence[0];

                    double transition = model.Transitions[model.StartState][state].LogProbability;
                    double emitProb = model.Emissions[state][emission].LogProbability;
                    double b_next = logBackward[state][0];

                    px = ProbabilityValue.addInLogSpace(px, transition + emitProb + b_next);
                }
            return px;

        }





    }
}
