using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Louis.CustomPackages.CommandLineInterface.Core {
    internal static class HelpGenerator {
        public static string GenerateHelpString(IReadOnlyDictionary<string, CommandSchema> commands) {
            var sb = new StringBuilder();
            sb.AppendLine($"Available Commands:");

            foreach(var command in commands.Keys) {

                sb.AppendLine($"<indent=5%>- {command}</indent>");
                if(!string.IsNullOrWhiteSpace((commands[command].Description)))
                    sb.AppendLine($"<indent=10%>{commands[command].Description}</indent>");
            }
            return sb.ToString();
        }

        public static string GenerateUsage(string commandName, CommandSchema schema) {
            var sb = new StringBuilder();
            sb.AppendLine($"Purpose : " + schema.Description);
            sb.AppendLine($"Usage   : {commandName} {(schema.Args.Count > 0 ? "[args]" : "")} {(schema.Flags.Count > 0 || schema.FlagChoices.Count > 0 ? "[flags]" : "")}");

            // Arguments
            if(schema.Args.Count > 0) {
                sb.AppendLine("\nArguments:");
                foreach(var arg in schema.Args) {
                    string req = arg.Required ? "(required)" : $"(optional, default={arg.DefaultValue})";
                    sb.AppendLine($"  {arg.Name} [{arg.Type.Name}] {req} " + arg.Description);
                    if(arg.Type.IsEnum) sb.AppendLine("    values: " + string.Join("/", Enum.GetNames(arg.Type)));
                }
            }

            // Boolean flags
            if(schema.Flags.Count > 0) {
                sb.AppendLine("\nFlags:");
                foreach(var flag in schema.Flags) {
                    sb.AppendLine($"  --{flag.Name}" +
                                  (string.IsNullOrWhiteSpace(flag.Description) ? "" : $": {flag.Description}"));
                }
            }

            // Flag choices
            if(schema.FlagChoices.Count > 0) {
                sb.AppendLine("\nFlag choices:");
                foreach(var choice in schema.FlagChoices) {
                    string opts = string.Join(", ", choice.Options.Select(o => $"--{o.Flag} ({o.Value})"));
                    sb.AppendLine($"  {choice.Name} [{choice.Type.Name}] default={choice.DefaultValue}: {opts}" +
                                  (string.IsNullOrWhiteSpace(choice.Description) ? "" : $" - {choice.Description}"));
                }
            }

            // Shorthands
            if(schema.Shorthands.Count > 0) {
                sb.AppendLine("\nShorthands:");
                foreach(var kv in schema.Shorthands)
                    sb.AppendLine($"  -{kv.Key} → --{kv.Value}");
            }

            return sb.ToString();
        }
    }
}