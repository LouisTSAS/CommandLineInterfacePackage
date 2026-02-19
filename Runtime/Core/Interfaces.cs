using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Object = UnityEngine.Object;

namespace Louis.CustomPackages.CommandLineInterface.Core {
    public interface ICommandCompiler {
        Command CreateCommand(string rawCommand);
        void Compile(Command command);
    }

    public interface ICommandLogger {
        void Log(Object sender, string message);
        void Log(string sender, string message);
        void Log(string message);
        void LogError(string message);
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
    }

    public interface IOutput {
        void Write(string output);
    }
}