using System;
using System.Collections.Generic;
using System.Text;

namespace Louis.CustomPackages.CommandLineInterface.Core {
    internal static class CommandTokenizer {
        /// <summary>
        /// Tokenizes a command string into tokens. Supports:
        /// - Quoted strings
        /// - Multiline commands
        /// - Arrays ([...]) and objects ({...}) as single tokens
        /// - Nested arrays/objects
        /// - Normalizes internal whitespace in arrays/objects
        /// </summary>
        /// <param name="input">The input command string</param>
        /// <returns>Array of tokens</returns>
        /// <exception cref="CommandParseException"></exception>
        public static string[] Tokenize(string input) {
            var tokens = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;
            var stack = new Stack<char>(); // Track [ ] and { }

            for(int i = 0; i < input.Length; i++) {
                char c = input[i];

                if(c == '"') {
                    sb.Append(c);
                    inQuotes = !inQuotes;
                    continue;
                }

                // Track nested brackets/braces
                if(!inQuotes) {
                    if(c == '[' || c == '{') {
                        stack.Push(c);
                        sb.Append(c);
                        continue;
                    }
                    if(c == ']' || c == '}') {
                        if(stack.Count == 0)
                            throw new CommandParseException($"Unmatched closing {c}");

                        char open = stack.Pop();
                        if((open == '[' && c != ']') || (open == '{' && c != '}'))
                            throw new CommandParseException($"Mismatched {open} and {c}");

                        sb.Append(c);
                        continue;
                    }

                    // Token separator: whitespace only if not in quotes or nested structure
                    if(char.IsWhiteSpace(c) && stack.Count == 0) {
                        if(sb.Length > 0) {
                            tokens.Add(NormalizeToken(sb.ToString()));
                            sb.Clear();
                        }

                        // Skip consecutive whitespace/newlines
                        while(i + 1 < input.Length && char.IsWhiteSpace(input[i + 1]))
                            i++;

                        continue;
                    }
                }

                sb.Append(c);
            }

            if(sb.Length > 0)
                tokens.Add(NormalizeToken(sb.ToString()));

            if(inQuotes)
                throw new CommandParseException("Unterminated quote");
            if(stack.Count > 0)
                throw new CommandParseException("Unterminated bracket or brace");

            return tokens.ToArray();
        }

        /// <summary>
        /// Normalizes a token by removing extra whitespace and line breaks
        /// inside arrays/objects, but preserves quoted strings and outer structure
        /// </summary>
        private static string NormalizeToken(string token) {
            int start = token.IndexOfAny(new[] { '{', '[' });
            if(start == -1)
                return token;

            string prefix = token.Substring(0, start);
            string body = token.Substring(start);

            var sb = new StringBuilder();
            bool inQuotes = false;
            bool pendingCommaSpace = false;

            foreach(char c in body) {
                if(c == '"') {
                    if(pendingCommaSpace) {
                        sb.Append(' ');
                        pendingCommaSpace = false;
                    }

                    sb.Append(c);
                    inQuotes = !inQuotes;
                    continue;
                }

                if(inQuotes) {
                    sb.Append(c);
                    continue;
                }

                switch(c) {
                    case ' ':
                    case '\r':
                    case '\n':
                    case '\t':
                        // discard all whitespace outside quotes
                        break;

                    case ',':
                        sb.Append(',');
                        pendingCommaSpace = true;
                        break;

                    case ':':
                        sb.Append(':');
                        break;

                    case ']':
                    case '}':
                        // never allow space before closing bracket/brace
                        pendingCommaSpace = false;
                        sb.Append(c);
                        break;

                    default:
                        if(pendingCommaSpace) {
                            sb.Append(' ');
                            pendingCommaSpace = false;
                        }
                        sb.Append(c);
                        break;
                }
            }

            return prefix + sb.ToString();
        }
    }

    public class CommandArgumentException : Exception {
        public CommandArgumentException() : base() { }
        public CommandArgumentException(string message) : base(message) { }
    }


    public class CommandParseException : Exception {
        public CommandParseException() : base() { }
        public CommandParseException(string message) : base(message) { }
    }
}