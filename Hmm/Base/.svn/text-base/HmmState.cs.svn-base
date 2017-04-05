using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hmm.Base {

    public class HmmState<Alphabet> : IEquatable<HmmState<Alphabet>>, IComparable<HmmState<Alphabet>> where Alphabet : IEquatable<Alphabet>, IComparable<Alphabet> {

        protected string name;

        public virtual string Name { get { return name; } }

        public HmmState(string Name) {
            this.name = Name;
        }

        public override string ToString() {
            return name;
        }

        public override int GetHashCode() {
            return name.GetHashCode();
        }

        public int GetHashCode(HmmState<Alphabet> obj) {
            return obj.Name.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj is HmmState<Alphabet>)
                return name.Equals(((HmmState<Alphabet>)obj).name);
            return false;
        }

        public bool Equals(HmmState<Alphabet> other) {
            return name.Equals(other.name);
        }

        public int CompareTo(HmmState<Alphabet> other) {
            return name.CompareTo(other.name);
        }
    }

    public class HmmStartState<Alphabet> : HmmState<Alphabet> where Alphabet : IEquatable<Alphabet>, IComparable<Alphabet> {
        public HmmStartState()
            : base("Start") { }
    }

    interface IHmmEndState {
    }

    public class HmmEndState<Alphabet> : HmmState<Alphabet>, IHmmEndState
        where Alphabet : IEquatable<Alphabet>, IComparable<Alphabet> {
        public HmmEndState()
            : base("End") { }
    }

}
