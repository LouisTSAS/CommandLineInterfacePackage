using Louis.CustomPackages.CommandLineInterface.Core.Exceptions;
using System;
using UnityEngine;

namespace Louis.CustomPackages.CommandLineInterface.Core {
    [AddComponentMenu("Command Line Interface/Command Compiler")]
    [RequireComponent(typeof(ICommandRegistry))]
    internal class CommandCompiler : MonoBehaviour, ICommandCompiler {
        ICommandRegistry _registry;

        private void Awake() {
            _registry = GetComponent<ICommandRegistry>();
        }

        public Command CreateCommand(string keyword, string[] args) {
            if (string.IsNullOrWhiteSpace(keyword)) throw new ArgumentException("Cannot have empty command");
            return new Command(keyword, args);
        }

        public Command CreateCommand(string rawCommand) {
            if(string.IsNullOrWhiteSpace(rawCommand)) throw new ArgumentException("Cannot have empty command");
            // Step 0: Tokenize Input
            string[] tokens = CommandTokenizer.Tokenize(rawCommand);
            string keyword = tokens[0];
            string[] args = tokens[1..];

            return new Command(keyword, args);
        }

        public void Compile(Command command) {
            if(!_registry.Schemas.ContainsKey(command.CommandName)) throw new CommandException($"Command {command.CommandName} not recognised");
            if(!_registry.Callbacks.ContainsKey(command.CommandName)) throw new CommandException($"Internal Error, not function bound for command {command.CommandName}");
            if(command.IsCompiled) return;

            // Step 1: Identify which function we are compiling for
            var schema = _registry.Schemas[command.CommandName];

            // Step 2: Parse Tokens according to function schema
            var parsedTokens = new ParsedTokens(command.RawTokens, schema.Shorthands);

            // Step 3: Bind the tokens as arguments to the schema
            var boundArgs = CommandBinder.Bind(schema, parsedTokens);

            // Step 4: Return compiled command with keyword and bound arguments
            command.SetBoundArgs(boundArgs);
        }
    }
}