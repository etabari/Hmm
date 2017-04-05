using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hmm.Model;
using Hmm.Base;

namespace Hmm.Training {
    class BaumWelchContinuous : BaseBaumWelch<double> {

        ////Dictionary<HmmState<double>, double[][]> Gamma;

        Dictionary<HmmState<double>, double[]> L;

        public BaumWelchContinuous(ContinuousMarkovModel model)
            : base(model) {
                if (model.mixtureCount != 1)
                    throw new ArgumentException("Only single mixture models!");
            
            ////Gamma = new Dictionary<HmmState<double>, double[][]>(model.States.Count);
            ////foreach (var state in model.States)
            ////    if (!(state is IHmmEndState)) {
            ////        double[][] d = new double[model.mixtureCount + 1][];
            ////        Gamma.Add(state, d);
            ////    }

        }

        protected override void ReestimateEmissionModelParameters(IList<double> trainset) {
            L = new Dictionary<HmmState<double>, double[]>(model.States.Count);
            foreach (var state in model.States)
                if (!(state is IHmmEndState)) {
                    double[] d = new double[trainset.Count];
                    for (int t = 0; t < trainset.Count; t++)
                        d[t] = fb.LogForward[state][t] + fb.LogBackward[state][t] - fb.LogPx;
                    L.Add(state, d);
                }
        }

        ////protected override void ReestimateEmissionModelParameters(IList<double> trainset) {

        ////    //Dictionary<HmmState<double>, double[]> Gamma1 = new Dictionary<HmmState<double>, double[]>(model.States.Count);
        ////    ContinuousMarkovModel cm = (ContinuousMarkovModel)model;
        ////    int KKK = cm.mixtureCount;

        ////    foreach (HmmState<double> state in model.States)
        ////        if (!(state is IHmmEndState))
        ////            for (int k = 0; k <= KKK; k++)
        ////                Gamma[state][k] = new double[trainset.Count];


        ////    for (int t = 0; t < trainset.Count; t++) {

        ////        double Gamma1Bott = double.NegativeInfinity;
        ////        foreach (HmmState<double> j in model.States)
        ////            if (!(j is IHmmEndState)) {
        ////                Gamma[j][KKK][t] = fb.LogForward[j][t] + fb.LogBackward[j][t];

        ////                Gamma1Bott = ProbabilityValue.addInLogSpace(Gamma1Bott, Gamma[j][KKK][t]);
        ////            }

        ////        foreach (HmmState<double> j in model.States)
        ////            if (!(j is IHmmEndState)) {
        ////                Gamma[j][KKK][t] -= Gamma1Bott;

        ////                ContinuousStateEmissions ej = (ContinuousStateEmissions)cm.Emissions[j];
        ////                for (int k = 0; k < KKK; k++) {
        ////                    double ejMix = Math.Log(ej.getMixtureValue(k, trainset[t]));
        ////                    Gamma[j][k][t] = Gamma[j][KKK][t] + ejMix - ej[trainset[t]].LogProbability;
        ////                }
        ////            }
        ////    }
        ////}
        
        protected override void SetNewModelEmissionParameters(IList<double> trainset) {
            ContinuousMarkovModel cm = (ContinuousMarkovModel)model;
            foreach (HmmState<double> j in cm.States)
                if (!(j is IHmmEndState)) {
                    ContinuousStateEmissions ej = (ContinuousStateEmissions)cm.Emissions[j];
                    double mean_top = 0.0;
                    double var_top = 0.0;
                    double bot = 0.0;
                    for (int t = 0; t < trainset.Count; t++) {
                        double ljt = Math.Exp(L[j][t]);
                        bot += ljt;
                        mean_top += ljt * trainset[t];
                        double otm = trainset[t] - ej.getMixtureMean(0);
                        var_top += ljt * otm * otm;
                    }
                    ej.updateMixture(0, mean_top / bot, var_top/bot);
                }
        }

        ////protected override void SetNewModelEmissionParameters(IList<double> trainset) {
        ////    ContinuousMarkovModel cm = (ContinuousMarkovModel)model;
        ////    double[] cj = new double[cm.mixtureCount];

        ////    foreach (HmmState<double> j in cm.States)
        ////        if (!(j is IHmmEndState)) {

        ////            ContinuousStateEmissions ej = (ContinuousStateEmissions)cm.Emissions[j];

        ////            // Calculate Top and Bottom of C
        ////            double cjk = 0.0;
        ////            for (int k = 0; k < cm.mixtureCount; k++) {
                        
        ////                cj[k] = 0.0;
        ////                double var = 0.0;
        ////                double mean = 0.0;
        ////                for (int t = 0; t < Gamma[j][k].Length; t++) {
        ////                    double Gammajkt =  Math.Exp(Gamma[j][k][t]);
        ////                    cj[k] += Gammajkt;

        ////                    mean += Gammajkt * trainset[t];

        ////                    ////////////////////////////
        ////                    //double ot = 0.0;
        ////                    //for (int m = 0; m < cm.mixtureCount; m++) {
        ////                    //    double om = ej.getMixtureValue(m, trainset[t]);
        ////                    //    om -= ej.getMixtureMean(m);
        ////                    //    om *= om;
        ////                    //    ot += om;
        ////                    //}
        ////                    double otm = trainset[t] - ej.getMixtureMean(k);
        ////                    var += Gammajkt * otm * otm;
        ////                }
        ////                cjk += cj[k];
        ////                double aaaa = var / cj[k]
        ////                ej.updateMixture(k, mean / cj[k], aaaa);
        ////            }

        ////            for (int k = 0; k < cm.mixtureCount; k++)
        ////                ej.updateMixture(k, cj[k] / cjk);
        ////        }
        ////}
    }
}
