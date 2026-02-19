using System.Collections.Generic;

namespace Louis.CustomPackages.CommandLineInterface.Core {
    public class BoundArgs {
        readonly Dictionary<string, object> _values;
        readonly HashSet<string> _flags;

        public BoundArgs(Dictionary<string, object> values, HashSet<string> flags) {
            _values = values;
            _flags = flags;
        }
        public T Get<T>(string name) => (T)_values[name];
        public bool HasFlag(string name) => _flags.Contains(name);
    }
}