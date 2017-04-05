using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hmm.Model;
using Hmm.Base;

namespace BaseHmm {
    class IonChannel : ContinuousMarkovModel {

        public static HmmState<double> Closed = new HmmState<double>("0");

        public static HmmState<double> Bursting = new HmmState<double>("1");
        public static HmmState<double> BurstingNoise = new HmmState<double>("2");

        public static HmmState<double> Opened = new HmmState<double>("3");
        public static HmmState<double> OpenedNoise = new HmmState<double>("4");
        
        
        ////Bursting,
        HmmState<double>[] stateList = { Closed, Bursting, BurstingNoise, Opened, OpenedNoise };

        private IList<StartTransition> startTransList = new[] {
            new StartTransition {Target=Closed , Probability=.2},
            new StartTransition {Target=Bursting , Probability=.2},
            new StartTransition {Target=BurstingNoise , Probability=.2},
            new StartTransition {Target=Opened , Probability=.2},
            new StartTransition {Target=OpenedNoise , Probability=.2},
        };

        private IList<SingleTransition> transList = new[] {
            // Close state transitions 
            new SingleTransition {Source=Closed, Target=Closed, Probability= .9994},
            new SingleTransition {Source=Closed, Target=BurstingNoise, Probability= .0001},
            new SingleTransition {Source=Closed, Target=Bursting, Probability= .0002},
            new SingleTransition {Source=Closed, Target=OpenedNoise, Probability= .0001},
            new SingleTransition {Source=Closed, Target=Opened, Probability= .0002},


            // Bursting

            new SingleTransition {Source=Bursting, Target=Bursting, Probability= .899},
            new SingleTransition {Source=Bursting, Target=BurstingNoise, Probability= .1},
            new SingleTransition {Source=Bursting, Target=Closed, Probability= .001},

            // Bursting Noise
            new SingleTransition {Source=BurstingNoise, Target=BurstingNoise, Probability= .899},
            new SingleTransition {Source=BurstingNoise, Target=Bursting, Probability= .1},
            new SingleTransition {Source=BurstingNoise, Target=Closed, Probability= .001},



            // Open state transitions
            new SingleTransition {Source=Opened, Target=Opened, Probability= .899},
            new SingleTransition {Source=Opened, Target=OpenedNoise, Probability= .1},
            new SingleTransition {Source=Opened, Target=Closed, Probability= .001},

            // Open noise 
            new SingleTransition {Source=OpenedNoise, Target=OpenedNoise, Probability= .4},
            new SingleTransition {Source=OpenedNoise, Target=Opened, Probability= .599},
            new SingleTransition {Source=OpenedNoise, Target=Closed, Probability= .001},
          };

        //static double[] mean = { 0, 20, 75, 95 };
        //static double[] var = { 4, 20, 7, 95 };

        //static double[] mean = { 0, 20, 75, 100 };
        //static double[] var = { 4, 36, 9, 25 };

        private IList<GaussianEmission> emitList = new[] {
            new GaussianEmission {State=Closed, MixCoef=.97, Mean=0, Var=4  },
            new GaussianEmission {State=Closed, MixCoef=.01, Mean=20, Var=25  },
            new GaussianEmission {State=Closed, MixCoef=.01, Mean=75, Var=9  },
            new GaussianEmission {State=Closed, MixCoef=.01, Mean=95, Var=100  },

             new GaussianEmission {State=Bursting, MixCoef=.7, Mean=0, Var=4  },
            new GaussianEmission {State=Bursting, MixCoef=.1, Mean=20, Var=25  },
            new GaussianEmission {State=Bursting, MixCoef=.1, Mean=75, Var=9  },
            new GaussianEmission {State=Bursting, MixCoef=.1, Mean=95, Var=100  },


             new GaussianEmission {State=BurstingNoise, MixCoef=.1, Mean=0, Var=4  },
            new GaussianEmission {State=BurstingNoise, MixCoef=.7, Mean=20, Var=25  },
            new GaussianEmission {State=BurstingNoise, MixCoef=.1, Mean=75, Var=9  },
            new GaussianEmission {State=BurstingNoise, MixCoef=.1, Mean=95, Var=100  },


             new GaussianEmission {State=Opened, MixCoef=.01, Mean=0, Var=4  },
            new GaussianEmission {State=Opened, MixCoef=.01, Mean=20, Var=25  },
            new GaussianEmission {State=Opened, MixCoef=.97, Mean=75, Var=9  },
            new GaussianEmission {State=Opened, MixCoef=.01, Mean=95, Var=100  },


             new GaussianEmission {State=OpenedNoise, MixCoef=.01, Mean=0, Var=4  },
            new GaussianEmission {State=OpenedNoise, MixCoef=.01, Mean=20, Var=25  },
            new GaussianEmission {State=OpenedNoise, MixCoef=.01, Mean=75, Var=9  },
            new GaussianEmission {State=OpenedNoise, MixCoef=.97, Mean=95, Var=100  },

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
