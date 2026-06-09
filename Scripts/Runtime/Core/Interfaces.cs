using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Object = UnityEngine.Object;

namespace Louis.CustomPackages.CommandLineInterface.Core {
    public interface ICommandCompiler {
        Command CreateCommand(string command, string[] args);
        Command CreateCommand(string rawCommand);
        CompilationResult TryCompile(Command command);
    }

    public interface ICommandLogger {
        void Log(Object sender, string message, LogLevel level = LogLevel.Info);
        void Log(object sender, string message, LogLevel level = LogLevel.Info);
        void Log(string message, LogLevel level = LogLevel.Info);
    }

    public interface ICommandRegistry {
        void RegisterCommand(string keyword, CommandSchema schema, Func<ICommandLogger, BoundArgs, CancellationToken, UniTask> callback);
        void UnregisterCommand(string keyword);
        public IReadOnlyDictionary<string, CommandSchema> Schemas { get; }
        public IReadOnlyDictionary<string, Func<ICommandLogger, BoundArgs, CancellationToken, UniTask>> Callbacks { get; }
    }

    public interface ICommandOutputProvider {
        void AttachOutput(IOutput output);
        void DetachOutput(IOutput output);
    }

    public interface ICommandHandler {
        void PushCommand(Command command);
        void PushCommand(string command);
        void PushCommand(string keyword, params string[] args);
    }

    public interface IOutput {
        void Write(Log log);
    }

    [Flags]
    public enum LogLevel {
        Debug       = 0,
        Info        = 1,
        Analytics   = 2,
        Success     = 4,
        Warning     = 8,
        Error       = 16
    }
}