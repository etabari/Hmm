using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hmm.Model;
using Hmm.Base;

namespace BaseHmm {
    class IonChannel2 : ContinuousMarkovModel {

        public static HmmState<double> Closed = new HmmState<double>("0");
      public static HmmState<double> Opened = new HmmState<double>("1");

        
        
        ////Bursting,
        HmmState<double>[] stateList = { Closed,  Opened };

        private IList<StartTransition> startTransList = new[] {
            new StartTransition {Target=Closed , Probability=.5},
          
            new StartTransition {Target=Opened , Probability=.5},

        };

        private IList<SingleTransition> transList = new[] {
            // Close state transitions 
            new SingleTransition {Source=Closed, Target=Closed, Probability= .99},
            new SingleTransition {Source=Closed, Target=Opened, Probability= .01},

            // Open state transitions
            new SingleTransition {Source=Opened, Target=Opened, Probability= .99},
            new SingleTransition {Source=Opened, Target=Closed, Probability= .01},

          };


        private IList<GaussianEmission> emitList = new[] {
            new GaussianEmission {State=Closed, MixCoef=1, Mean=0, Var=4  },
            new GaussianEmission {State=Opened, MixCoef=1, Mean=20, Var=25  },
        };


        protected override IList<GaussianEmission> SetupEmissions {
            get { return emitList; }
        }

        public override IList<HmmState<double>> States {
            get { return stateList; }
        }

        protected override IList<BaseMarkovModel<double>.StartTransition> SetupStartTransistions {
            get { return startTransList; }
        }

        protected override IList<BaseMarkovModel<double>.SingleTransition> SetupTransitions {
            get { return transList; }
        }
    }
}
