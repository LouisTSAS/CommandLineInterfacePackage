using System;
using System.Collections.Generic;
using System.Linq;

namespace Louis.CustomPackages.CommandLineInterface {
    public class CommandSchema {
        readonly List<ArgDef> _args = new();
        readonly List<FlagDef> _flags = new();
        readonly List<FlagChoiceDef> _flagChoices = new();
        readonly Dictionary<string, string> _shorthands = new(StringComparer.OrdinalIgnoreCase);

        public string Description { get; private set; }
        public IReadOnlyList<ArgDef> Args => _args;
        public IReadOnlyList<FlagDef> Flags => _flags;
        public IReadOnlyList<FlagChoiceDef> FlagChoices => _flagChoices;
        public IReadOnlyDictionary<string, string> Shorthands => _shorthands;

        public CommandSchema WithDescription(string description) {
            Description = description;
            return this;
        }

        public CommandSchema Required<T>(string name, int position, string description = null) {
            _args.Add(new ArgDef(name, typeof(T), position, required: true, description: description));
            return this;
        }

        public CommandSchema Optional<T>(string name, int position, T defaultValue, string description = null) {
            _args.Add(new ArgDef(name, typeof(T), position, required: false, defaultValue, description: description));
            return this;
        }

        public CommandSchema Flag(string name) {
            _flags.Add(new FlagDef(name));
            return this;
        }

        public CommandSchema FlagChoice<T>(string name, T defaultValue, (string flag, T value)[] options, string description = null) {
            var objectOptions = options.Select(o => (o.flag, (object)o.value)).ToArray();
            _flagChoices.Add(new FlagChoiceDef(
                name, typeof(T),
                defaultValue,
                objectOptions,
                description));
            return this;
        }

        public CommandSchema FlagShorthands(params (string shorthand, string fullFlag)[] mappings) {
            foreach(var (sh, full) in mappings) {
                _shorthands[sh] = full;
            }
            return this;
        }
    }

    public class ArgDef {
        public string Name;
        public Type Type;
        public int Position;
        public bool Required;
        public object DefaultValue = null;
        public string Description = null;

        public ArgDef(string name, Type type, int position, bool required, object defaultValue = null, string description = null) {
            Name = name;
            Type = type;
            Position = position;
            Required = required;
            DefaultValue = defaultValue;
            Description = description;
        }
    }

    public class FlagDef {
        public string Name;
        public string Description = null;

        public FlagDef(string name, string description = null) {
            Name = name;
            Description = description;
        }
    }

    public class FlagChoiceDef {
        public string Name;
        public Type Type;
        public object DefaultValue;
        public (string Flag, object Value)[] Options;
        public string Description = null;

        public FlagChoiceDef(string name, Type type, object defaultVal, (string flat, object value)[] options, string description = null) {
            Name = name;
            Type = type;
            DefaultValue = defaultVal;
            Options = options;
            Description = description;
        }
    }
}