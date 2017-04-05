using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hmm.Base;
using Hmm.Model;

namespace BaseHmm {


    class DnaGenerator : DiscreteMarkovModel<char> {

        public static HmmState<char> Bg = new HmmState<char>("1");
        public static HmmState<char> CpG = new HmmState<char>("2");
        public static HmmState<char> ApT = new HmmState<char>("3");
        public static HmmEndState<char> End = new HmmEndState<char>();

        HmmState<char>[] stateList = { Bg, CpG, ApT };

        char[] symbolList = { 'A', 'C', 'G', 'T' };


        private IList<SymbolEmission> emitList = new[] {
            new SymbolEmission {State=Bg, Signal='A', Probability=.25},
            new SymbolEmission {State=Bg, Signal='C', Probability=.25},
            new SymbolEmission {State=Bg, Signal='G', Probability=.25},
            new SymbolEmission {State=Bg, Signal='T', Probability=.25},

            new SymbolEmission {State=CpG, Signal='A', Probability=.1},
            new SymbolEmission {State=CpG, Signal='C', Probability=.5},
            new SymbolEmission {State=CpG, Signal='G', Probability=.3},
            new SymbolEmission {State=CpG, Signal='T', Probability=.1},
            
            new SymbolEmission {State=ApT, Signal='A', Probability=.4},
            new SymbolEmission {State=ApT, Signal='C', Probability=.15},
            new SymbolEmission {State=ApT, Signal='G', Probability=.2},
            new SymbolEmission {State=ApT, Signal='T', Probability=.25},
        };

        private IList<StartTransition> startTransList = new[] {
            new StartTransition {Target=ApT , Probability=.1},
            new StartTransition {Target=CpG , Probability=.1},
            new StartTransition {Target=Bg , Probability=.8},
        };

        private IList<SingleTransition> transList = new[] {
            new SingleTransition {Source=Bg, Target=Bg, Probability = .98},
            new SingleTransition {Source=Bg, Target=CpG, Probability= .01},
            new SingleTransition {Source=Bg, Target=ApT, Probability= .01},
            //new SingleTransition {Source=Bg, Target=End, Probability= .000000001},

            new SingleTransition {Source=CpG, Target=Bg, Probability = .01},
            new SingleTransition {Source=CpG, Target=CpG, Probability = .99},
            //new SingleTransition {Source=CpG, Target=ApT, Probability= .0001},
            //new SingleTransition {Source=CpG, Target=End, Probability= .000000001},

            new SingleTransition {Source=ApT, Target=Bg, Probability = .01},
            new SingleTransition {Source=ApT, Target=ApT, Probability = .99},
           // new SingleTransition {Source=ApT, Target=CpG, Probability= .0001},
            //new SingleTransition {Source=ApT, Target=End, Probability= .000000001},

        };



        public override IList<HmmState<char>> States {
            get { return stateList; }
        }

        public override IList<char> Symbols {
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
