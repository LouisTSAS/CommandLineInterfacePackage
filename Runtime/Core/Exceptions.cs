using System;

namespace Louis.CustomPackages.CommandLineInterface.Core.Exceptions {
    public class KeywordAlreadyExistsException : Exception {
        public KeywordAlreadyExistsException() : base() { }
        public KeywordAlreadyExistsException(string message) : base(message) { }
    }

    public class KeywordNotFoundException : Exception {
        public KeywordNotFoundException() : base() { }
        public KeywordNotFoundException(string message) : base(message) { }
    }

    public class CommandException : Exception {
        public CommandException() : base() { }
        public CommandException(string message) : base(message) { }
    }
}