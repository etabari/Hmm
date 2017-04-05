using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Hmm.Decoding;

namespace Hmm.Base {

    public struct ProbabilityValue {
        public const double Epsilon = 1.0e-12;
        double probability;
        double logProbability;

        public ProbabilityValue(double p) {
            probability = p;
            logProbability = Math.Log(p);
        }

        public double Probability {
            get { return probability; }
        }

        public double LogProbability {
            get { return logProbability; }
        }

        public override string ToString() {
            return "p:" + probability + "\tlog:" + logProbability;
        }

        public static double addInLogSpace(double logP, double logQ) {
            if (Double.IsNegativeInfinity(logP))
                return logQ;
            else if (Double.IsNegativeInfinity(logQ))
                return logP;
            if (logQ < logP)
                return logP + Math.Log(1 + Math.Exp(logQ - logP));
            else
                return logQ + Math.Log(1 + Math.Exp(logP - logQ));
        }
    }

    public abstract class BaseModelEmissions<Alphabet>
       where Alphabet : IEquatable<Alphabet>, IComparable<Alphabet> {

        protected Dictionary<HmmState<Alphabet>, IBaseStateEmisions<Alphabet>> data;

        public BaseModelEmissions(IList<HmmState<Alphabet>> states) {
            data = new Dictionary<HmmState<Alphabet>, IBaseStateEmisions<Alphabet>>(states.Count);
        }

        public virtual IBaseStateEmisions<Alphabet> this[HmmState<Alphabet> state] {
            get { return data[state]; }
        }

    }


    public interface IBaseStateEmisions<Alphabet>
        where Alphabet : IEquatable<Alphabet>, IComparable<Alphabet> {
        ProbabilityValue this[Alphabet obs] { get; }
        bool IsCorrect();
        Alphabet getRandomObservation();
    }



    public abstract class BaseMarkovModel<Alphabet>
        where Alphabet : IEquatable<Alphabet>, IComparable<Alphabet> {

        public class ModelTransitions {

            Dictionary<HmmState<Alphabet>, StateTransition> data;

            public ModelTransitions(HmmState<Alphabet> startState, IList<HmmState<Alphabet>> states) {
                data = new Dictionary<HmmState<Alphabet>, StateTransition>(states.Count + 1);
                data[startState] = new StateTransition(states);
                foreach (HmmState<Alphabet> state in states)
                    data[state] = new StateTransition(states);

            }

            public StateTransition this[HmmState<Alphabet> Source] {
                get { return data[Source]; }
            }

            public override string ToString() {
                StringBuilder sb = new StringBuilder();

                foreach (var state in data.Keys)
                    if (!(state is HmmStartState<Alphabet>))
                        sb.Append("\t").Append(state.Name);
                sb.Append("\n");
                foreach (var source in data.Keys)
                    if (!(source is IHmmEndState)) {
                        sb.Append(source.Name);
                        foreach (var target in data.Keys)
                            if (!(target is HmmStartState<Alphabet>))
                                sb.Append("\t").AppendFormat("{0:0.####}", this[source][target].Probability);
                        sb.Append("\n");
                    }
                return sb.ToString(0, sb.Length - 1);
            }

            public class StateTransition {
                Dictionary<HmmState<Alphabet>, ProbabilityValue> data;
                Dictionary<HmmState<Alphabet>, double> cdf;

                public StateTransition(IList<HmmState<Alphabet>> states) {
                    data = new Dictionary<HmmState<Alphabet>, ProbabilityValue>(states.Count);
                    cdf = new Dictionary<HmmState<Alphabet>, double>(states.Count);
                    foreach (HmmState<Alphabet> s in states)
                        data[s] = new ProbabilityValue(0);
                }

                public ProbabilityValue this[HmmState<Alphabet> Target] {
                    get { return data[Target]; }
                }

                public void setProbability(HmmState<Alphabet> s, double p) {
                    double sum = (from e in data where e.Key.CompareTo(s) <= 0 select e.Value.Probability).Sum();
                    ProbabilityValue pv = new ProbabilityValue(p);
                    data[s] = pv;
                    cdf[s] = p + sum;
                }

                internal bool IsCorrect() {
                    double sum = (from e in data select e.Value.Probability).Sum();
                    return Math.Abs(sum - 1.0) < ProbabilityValue.Epsilon;
                }

                public HmmState<Alphabet> getNextTransition(double prob) {
                    var a = from e in data
                            where e.Value.Probability > 0.0 && cdf[e.Key] >= prob
                            orderby e.Key
                            select e.Key;

                    if (a.Count() > 0)
                        return a.First();
                    a = from e in data
                        where e.Value.Probability > 0.0
                        orderby e.Key descending
                        select e.Key;

                    return a.First();
                }
            }
        }


        public class SingleTransition : IComparable<SingleTransition> {
            public HmmState<Alphabet> Source;
            public HmmState<Alphabet> Target;
            public double Probability;

            public SingleTransition(HmmState<Alphabet> Source, HmmState<Alphabet> Target, double Probability) {
                this.Source = Source;
                this.Target = Target;
                this.Probability = Probability;
            }

            public SingleTransition() {

            }

            public int CompareTo(SingleTransition other) {
                int r = Source.CompareTo(other.Source);
                if (r != 0)
                    return r;
                return Target.CompareTo(other.Target);
            }
        }

        public class StartTransition : SingleTransition {

            public StartTransition(HmmState<Alphabet> Target, double Probability)
                : base(null, Target, Probability) {
            }

            public StartTransition() {

            }

        }


        protected HmmStartState<Alphabet> _startState;
        protected IList<HmmState<Alphabet>> _states;

        protected ModelTransitions _transition;
        protected BaseModelEmissions<Alphabet> _emissions;


        protected bool _hasEndState;
        protected HmmEndState<Alphabet> _endState;

        public HmmStartState<Alphabet> StartState { get { return _startState; } }
        public HmmEndState<Alphabet> EndState { get { return _endState; } }
        public bool HasEndState { get { return _hasEndState; } }

        public ModelTransitions Transitions { get { return _transition; } }
        public BaseModelEmissions<Alphabet> Emissions { get { return _emissions; } }
        public abstract IList<HmmState<Alphabet>> States { get; }

        protected abstract IList<StartTransition> SetupStartTransistions { get; }
        protected abstract IList<SingleTransition> SetupTransitions { get; }


        public BaseMarkovModel() {
            _startState = new HmmStartState<Alphabet>();
            _states = States;

            _transition = new ModelTransitions(_startState, _states);


            if (!CheckStates())
                throw new ArgumentException("States are not unique or there are more than one end states.");

            if (!CheckStartTransitions(SetupStartTransistions))
                throw new ArgumentException(this + ": Start transitions do not add up to 1.0");

            HmmState<Alphabet> s = CheckTransition(SetupTransitions);
            if (s!= null)
                throw new ArgumentException(this + ": Transitions in state '"+s+"' does not add up to 1.0.");
        }


        protected virtual bool CheckStates() {
            var endStates = from s in _states where s is IHmmEndState select s;
            int endStatesCount = endStates.Count();
            _hasEndState = endStatesCount == 1;
            if (_hasEndState)
                _endState = (HmmEndState<Alphabet>)endStates.First();

            if (endStatesCount > 1)
                return false;

            var distinctStates = (from s in _states select s.Name).Distinct();
            if (distinctStates.Count() != _states.Count)
                return false;

            return true;
        }

        protected virtual bool CheckStartTransitions(IList<StartTransition> SetupStartTransistions) {
            foreach (var e in SetupStartTransistions)
                _transition[_startState].setProbability(e.Target, e.Probability);

            if (!_transition[_startState].IsCorrect())
                return false;
            return true;
        }

        protected virtual HmmState<Alphabet> CheckTransition(IList<SingleTransition> SetupTransition) {
            var transitionStates =
                  from e in SetupTransition
                  group e by e.Source into g
                  select new { State = g.Key, Emissions = g };

            foreach (var g in transitionStates) {

                foreach (var e in g.Emissions)
                    _transition[g.State].setProbability(e.Target, e.Probability);

                if (!_transition[g.State].IsCorrect())
                    return g.State;
            }
            return null;
        }

    }


}
