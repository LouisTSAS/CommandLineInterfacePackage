using System;

namespace Louis.CustomPackages.CommandLineInterface.Core {
    public class Command {
        public string CommandName { get; private set; }
        public bool IsCompiled { get; private set; }
        public string[] RawTokens { get; private set; }
        public BoundArgs BoundArgs { get; private set; }

        public Command(string commandName, string[] rawArgs) {
            if(string.IsNullOrWhiteSpace(commandName)) throw new ArgumentException("Cannot have empty command Name");
            CommandName = commandName;
            RawTokens = rawArgs;
            IsCompiled = false;
        }

        internal void SetBoundArgs(BoundArgs args) {
            if(args == null) throw new ArgumentNullException("Args cannot be null");
            BoundArgs = args;
            IsCompiled = true;
        }
    }
}