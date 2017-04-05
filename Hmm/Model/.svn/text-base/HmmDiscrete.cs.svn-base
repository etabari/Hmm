using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Hmm.Base;

namespace Hmm.Model {


    public class DiscreteModelEmissions<Alphabet> : BaseModelEmissions<Alphabet>
       where Alphabet : IEquatable<Alphabet>, IComparable<Alphabet> {

        IList<Alphabet> symbols;

        public DiscreteModelEmissions(IList<HmmState<Alphabet>> states, IList<Alphabet> symbols)
            : base(states) {
            this.symbols = symbols;
            foreach (HmmState<Alphabet> state in states)
                data[state] = new DiscreteStateEmissions<Alphabet>(symbols);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            foreach (var symbol in symbols)
                sb.Append("\t").Append(symbol.ToString());
            sb.Append("\n");
            foreach (var source in data.Keys)
                if (!(source is IHmmEndState)) {
                    sb.Append(source.Name);
                    foreach (var symbol in symbols)
                        sb.Append("\t").AppendFormat("{0:0.###}", this[source][symbol].Probability);
                    sb.Append("\n");
                }
            return sb.ToString(0, sb.Length - 1);
        }

    }

    public class DiscreteStateEmissions<Alphabet> : IBaseStateEmisions<Alphabet>
       where Alphabet : IEquatable<Alphabet>, IComparable<Alphabet> {

        Dictionary<Alphabet, ProbabilityValue> data;
        Dictionary<Alphabet, double> cdf;
        static Random random = new Random();
        public DiscreteStateEmissions(IList<Alphabet> symbols) {
            data = new Dictionary<Alphabet, ProbabilityValue>(symbols.Count);
            cdf = new Dictionary<Alphabet, double>(symbols.Count);
            foreach (Alphabet s in symbols)
                data[s] = new ProbabilityValue();
        }

        public ProbabilityValue this[Alphabet obs] {
            get { return data[obs]; }
        }

        public void setProbability(Alphabet s, double p) {
            double sum = (from e in data where e.Key.CompareTo(s) <= 0 select e.Value.Probability).Sum();
            ProbabilityValue pv = new ProbabilityValue(p);
            data[s] = pv;
            cdf[s] = p + sum;
        }

        public bool IsCorrect() {
            double sum = (from e in data select e.Value.Probability).Sum();
            return Math.Abs(sum - 1.0) < ProbabilityValue.Epsilon;
        }

        public Alphabet getRandomObservation() {
            var a = from e in data
                    where e.Value.Probability > 0.0 && cdf[e.Key] >= random.NextDouble()
                    orderby e.Key
                    select e.Key;

            if (a.Count() > 0)
                return a.First();
            a = from e in data
                where e.Value.Probability > 0.0
                orderby e.Key descending
                select e.Key;
            Console.WriteLine("HOHOOOOOOOOOOO");
            return a.First();
        }
    }


    public abstract class DiscreteMarkovModel<Alphabet> : BaseMarkovModel<Alphabet>
        where Alphabet : IEquatable<Alphabet>, IComparable<Alphabet> {

        public class SymbolEmission : IComparable<SymbolEmission> {
            public HmmState<Alphabet> State;
            public Alphabet Signal;
            public double Probability;

            public SymbolEmission(HmmState<Alphabet> State, Alphabet Signal, double Probability) {
                this.State = State;
                this.Signal = Signal;
                this.Probability = Probability;
            }

            public SymbolEmission() {
            }

            public int CompareTo(SymbolEmission other) {
                int r = State.CompareTo(other.State);
                if (r != 0)
                    return r;
                return Signal.CompareTo(other.Signal);
            }
        }

        IList<Alphabet> _symbols;

        public abstract IList<Alphabet> Symbols { get; }

        protected abstract IList<SymbolEmission> SetupEmissions { get; }

        public DiscreteMarkovModel() {

            _symbols = Symbols;
            _emissions = new DiscreteModelEmissions<Alphabet>(_states, _symbols);

            HmmState<Alphabet> s = CheckEmissions(SetupEmissions);
            if (s!=null)
                throw new ArgumentException(this + ": Emissions in all state '"+s+ "' do not add up to 1.0");
        }

        private HmmState<Alphabet> CheckEmissions(IList<SymbolEmission> SetupEmissions) {

            var emissionStates =
                from e in SetupEmissions
                group e by e.State into g
                select new { State = g.Key, Emissions = g };

            foreach (var g in emissionStates) {

                foreach (var e in g.Emissions)
                    ((DiscreteStateEmissions<Alphabet>)_emissions[g.State]).setProbability(e.Signal, e.Probability);

                if (!_emissions[g.State].IsCorrect())
                    return g.State;
            }
            return null;
        }



    }


}
