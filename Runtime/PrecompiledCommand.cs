using System;

namespace Louis.CustomPackages.CommandLineInterface {
    public class PrecompiledCommand {
        public string CommandName { get; private set; }
        public BoundArgs BoundArgs { get; private set; }

        public PrecompiledCommand(string commandName, BoundArgs args) {
            if(string.IsNullOrWhiteSpace(commandName)) throw new ArgumentException("Cannot have empty command Name");
            if(args == null) throw new ArgumentNullException("Cannot have null arguments for command");
            CommandName = commandName;
            BoundArgs = args;
        }
    }
}