using Louis.CustomPackages.CommandLineInterface.Exceptions;
using System;
using UnityEngine;

namespace Louis.CustomPackages.CommandLineInterface {
    [AddComponentMenu("Command Line Interface/Command Compiler")]
    [RequireComponent(typeof(ICommandRegistry))]
    internal class CommandCompiler : MonoBehaviour, ICommandCompiler {
        ICommandRegistry _registry;

        private void Awake() {
            _registry = GetComponent<ICommandRegistry>();
        }

        public PrecompiledCommand CompileCommand(string command) {
            if(string.IsNullOrWhiteSpace(command)) throw new ArgumentException("Cannot have empty command");
            // Step 0: Tokenize input
            string[] tokens = CommandTokenizer.Tokenize(command);
            var keyword = tokens[0];
            var parameters = tokens[1..];
            if(!_registry.Schemas.ContainsKey(keyword)) throw new CommandException($"Command {keyword} not recognised");
            if(!_registry.Callbacks.ContainsKey(keyword)) throw new CommandException($"Internal Error, not function bound for command {keyword}");

            // Step 1: Identify which function we are compiling for
            var schema = _registry.Schemas[keyword];

            // Step 2: Parse Tokens according to function schema
            var parsedTokens = new ParsedTokens(parameters, schema.Shorthands);

            // Step 3: Bind the tokens as arguments to the schema
            var boundArgs = CommandBinder.Bind(schema, parsedTokens);

            // Step 4: Return compiled command with keyword and bound arguments
            return new PrecompiledCommand(keyword, boundArgs);
        }
    }
}