using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hmm.Base;
using Hmm.Model;

namespace BaseHmm {




    class DishonestCasino : DiscreteMarkovModel<int> {

        public static HmmState<int> Fair = new HmmState<int>("F");
        public static HmmState<int> Loaded = new HmmState<int>("L");
        public static HmmEndState<int> End = new HmmEndState<int>();

        HmmState<int>[] stateList = { Fair, Loaded , End};

        int[] symbolList = { 1, 2, 3, 4, 5, 6 };


        private IList<SymbolEmission> emitList = new[] {
            new SymbolEmission {State=Fair, Signal=1, Probability=1.0/6},
            new SymbolEmission {State=Fair, Signal=2, Probability=1.0/6},
            new SymbolEmission {State=Fair, Signal=3, Probability=1.0/6},
            new SymbolEmission {State=Fair, Signal=4, Probability=1.0/6},
            new SymbolEmission {State=Fair, Signal=5, Probability=1.0/6},
            new SymbolEmission {State=Fair, Signal=6, Probability=1-5.0/6},

            new SymbolEmission {State=Loaded, Signal=1, Probability=0.1},  
            new SymbolEmission {State=Loaded, Signal=2, Probability=0.1},  
            new SymbolEmission {State=Loaded, Signal=3, Probability=0.1},  
            new SymbolEmission {State=Loaded, Signal=4, Probability=0.1},  
            new SymbolEmission {State=Loaded, Signal=5, Probability=0.1},  
            new SymbolEmission {State=Loaded, Signal=6, Probability=0.5},  
            
        };

        private IList<StartTransition> startTransList = new[] {
            new StartTransition {Target=Loaded , Probability=0.5},
            new StartTransition {Target=Fair , Probability=.5},
        };

        private IList<SingleTransition> transList = new[] {
            new SingleTransition {Source=Fair, Target=Loaded, Probability= .05},
            new SingleTransition {Source=Fair, Target=Fair, Probability = .9499},
            new SingleTransition {Source=Loaded, Target=Fair, Probability = .1},
            new SingleTransition {Source=Loaded, Target=Loaded, Probability = .8999},
            new SingleTransition {Source=Loaded, Target=End, Probability= .0001},
            new SingleTransition {Source=Fair, Target=End, Probability= .0001},
        };



        public override IList<HmmState<int>> States {
            get { return stateList; }
        }

        public override IList<int> Symbols {
            get { return symbolList; }
        }



        protected override IList<SingleTransition> SetupTransitions {
            get { return transList; }
        }

        protected override IList<SymbolEmission> SetupEmissions {
            get { return emitList; }
        }


       protected override IList<StartTransition> SetupStartTransistions {
            get { return startTransList; }
        }
    }
}
