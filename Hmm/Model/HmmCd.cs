
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CenterSpace.Free;
using Hmm.Base;

namespace Hmm.Model {

    public class ContinuousModelEmissions : BaseModelEmissions<double> {

        public ContinuousModelEmissions(IList<HmmState<double>> states)
            : base(states) {
            foreach (HmmState<double> state in states)
                data[state] = new ContinuousStateEmissions();
        }
        
        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            sb.Append("\tcoef\tmean\tvar\n");
            
            foreach (var source in data.Keys)
                if (!(source is IHmmEndState)) {
                    ContinuousStateEmissions se = (ContinuousStateEmissions)data[source];
                for(int m=0; m<se.mixtureCount; m++)
                {

                    sb.Append(source.Name);
                    sb.Append("\t");
                    sb.Append(se.getMixtureCoefficient(m));
                    sb.Append("\t");
                    sb.Append(se.getMixtureMean(m));
                    sb.Append("\t");
                    sb.Append(se.getMixtureVariance(m));
                    sb.Append("\n");
                }
        }
            return sb.ToString(0, sb.Length - 1);
        }
    }

    public class ContinuousStateEmissions : IBaseStateEmisions<double> {
        static NormalDist standardNormal = new NormalDist(0, 1);
        static Random rand = new Random(); 
        List<double> coefficients;
        List<NormalDist> normalDists;
        NormalDist mixedNormal;

        public int mixtureCount {
            get { return coefficients.Count; }
        }

        public ContinuousStateEmissions() {
            coefficients = new List<double>();
            normalDists = new List<NormalDist>();
            mixedNormal = null;
        }

        public ProbabilityValue this[double obs] {
            get {
                
                //double p = coefficients[0] * normalDists[0].CDF(obs);

                //double p = mixedNormal.PDF(obs);
                double p = 0.0;
                for (int m = 0; m < coefficients.Count; m++) {
                   // double z = (obs - normalDists[m].Mean) / normalDists[m].Sigma;
                   //p += coefficients[m] * standardNormal.CDF(-Math.Abs(z));
                    p += coefficients[m] * normalDists[m].PDF(obs);
                }
                return new ProbabilityValue(p);

            }
        }

        public bool IsCorrect() {
            double sum = coefficients.Sum();
            return Math.Abs(sum - 1.0) < ProbabilityValue.Epsilon;
        }

        public double getRandomObservation()
        {
            double p = 0.0;
            for (int m = 0; m < coefficients.Count; m++) {
                p += coefficients[m] * GetObservationForMixture(m);
            }
            return p;
        }

        private double GetObservationForMixture(int m) {
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = normalDists[m].Mean + normalDists[m].Sigma * randStdNormal;
            return randNormal;
        }

        internal void addMixture(double mixCoef, double mean, double var) {
            coefficients.Add(mixCoef);
            normalDists.Add(new NormalDist(mean, var));
            updateMixNormal();
        }

        internal void updateMixture(int mixId, double mean, double var) {
            normalDists[mixId].Mean = mean;
            normalDists[mixId].Variance = var;
        }

        internal void updateMixture(int mixId, double mixCoef) {
            coefficients[mixId] = mixCoef;
        }

        private void updateMixNormal() {
            double mixMean = 0.0;
            double mixVar = 0.0;
            for (int m = 0; m < coefficients.Count; m++) {
                mixMean += coefficients[m] * normalDists[m].Mean;
                mixVar += coefficients[m] * coefficients[m] * normalDists[m].Variance;
            }
            mixedNormal = new NormalDist(mixMean, mixVar);
        }

        public double getMixtureValue(int mixture, double obs) {
            return coefficients[mixture] * normalDists[mixture].PDF(obs);
        }

        public double getMixtureCoefficient(int mixture) {
            return coefficients[mixture];
        }

        public double getMixtureMean(int mixture) {
            return normalDists[mixture].Mean;
        }

        public double getMixtureVariance(int mixture) {
            return normalDists[mixture].Variance;
        }

        
    }

    public abstract class ContinuousMarkovModel : BaseMarkovModel<double> {
        

        public class GaussianEmission : IComparable<GaussianEmission> {
            public HmmState<double> State;
            public double MixCoef;
            public double Mean;
            public double Var;

            public GaussianEmission(HmmState<double> state, double mixCoefficient, double mean, double var) {
                this.State = state;
                this.MixCoef = mixCoefficient;
                this.Mean = mean;
                this.Var = var;
            }

            public GaussianEmission() {
            }

            public int CompareTo(GaussianEmission other) {
                int r = State.CompareTo(other.State);
                if (r != 0)
                    return r;
                r = Mean.CompareTo(other.Mean);
                if (r != 0)
                    return r;
                return Var.CompareTo(other.Var);
            }
        }


        private int _mC;
        public int mixtureCount {
            get { return _mC; }
        }

        protected abstract IList<GaussianEmission> SetupEmissions { get; }

        public ContinuousMarkovModel() {
            _emissions = new ContinuousModelEmissions(_states);

            HmmState<double> s = CheckEmissions(SetupEmissions);
            if (s!=null)
                throw new ArgumentException(this + ":Emissions in state '" + s + "' does not add up to 1.0.");

            _mC = -1;
            foreach (var state in States)
                if (!(state is IHmmEndState)) {
                    if (_mC == -1)
                        _mC = ((ContinuousStateEmissions)Emissions[state]).mixtureCount;
                    else
                        if (_mC != ((ContinuousStateEmissions)Emissions[state]).mixtureCount)
                            throw new ArgumentException("Not all states have the same number of mixtures.");
                }
        }

        private HmmState<double> CheckEmissions(IList<GaussianEmission> SetupEmissions) {

            var emissionStates =
                from e in SetupEmissions
                group e by e.State into g
                select new { State = g.Key, Emissions = g };

            foreach (var g in emissionStates) {
                foreach (var e in g.Emissions)
                    ((ContinuousStateEmissions)_emissions[g.State]).addMixture(e.MixCoef, e.Mean, e.Var);
                if (!_emissions[g.State].IsCorrect())
                    return g.State;
            }
            return null;
        }

    }


}
