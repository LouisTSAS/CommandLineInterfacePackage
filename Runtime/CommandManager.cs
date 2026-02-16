using Cysharp.Threading.Tasks;
using Louis.CustomPackages.CommandLineInterface.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
[assembly: InternalsVisibleTo("com.Louis.CommandLineInterfaceTests")]

namespace Louis.CustomPackages.CommandLineInterface {
    [AddComponentMenu("Command Line Interface/Command Manager")]
    [RequireComponent(typeof(CommandLogger))]
    [RequireComponent(typeof(CommandRegistry))]
    [RequireComponent(typeof(CommandCompiler))]
    internal class CommandManager : MonoBehaviour, ICommandHandler {
        CancellationTokenSource _cts = new();
        readonly Queue<PrecompiledCommand> _commandQueue = new();
        ICommandLogger _logger;
        ICommandRegistry _registry;
        ICommandCompiler _compiler;

        private void Awake() {
            _logger = GetComponent<ICommandLogger>();
            _registry = GetComponent<ICommandRegistry>();
            _compiler = GetComponent<ICommandCompiler>();
        }

        public void Start() {
            ProcessCommands(_cts.Token);
        }

        public void OnDestroy() {
            _cts.Cancel();
        }

        async void ProcessCommands(CancellationToken token) {
            _logger.Log("CommandManager", $"Command Processing Started: {_commandQueue.Count} unhandled commands pushed");
            while(true) {
                if(_commandQueue.Count > 0) {
                    try {
                        await RunCommand(_commandQueue.Dequeue(), token);
                    } catch(Exception e) {
                        _logger.LogError(e.Message);
                        Debug.LogException(e);
                    }
                } else {
                    await UniTask.Yield();
                }

                if(token.IsCancellationRequested) {
                    _logger.Log("CommandManager", $"Cancellation Requested: {_commandQueue.Count} unhandled commands remaining");
                    return;
                }
            }
        }

        async void CancelAll() {
            _logger.Log("CommandManager", "Cancelling all current actions and clearing command buffer");
            _cts.Cancel();
            _cts = new();
            _commandQueue.Clear();
            await UniTask.Yield();
            await UniTask.Delay(250);
            ProcessCommands(_cts.Token);
        }

        async UniTask RunCommand(PrecompiledCommand command, CancellationToken token) {
            if(!_registry.Schemas.ContainsKey(command.CommandName)) throw new CommandException($"Internal Error, not function bound for command {command.CommandName}");
            var callback = _registry.Callbacks[command.CommandName];

            await callback.Invoke(_logger, command.BoundArgs, token);
        }

        void ShowHelpText(string command) {
            string[] tokens = CommandTokenizer.Tokenize(command);
            if(tokens[0] != "help") return;
            string helpText;
            if(tokens.Length == 1) {
                // Output all currently registered functions
                helpText = HelpGenerator.GenerateHelpString(_registry.Schemas);
            } else {
                var func = tokens[1];
                if(!_registry.Schemas.TryGetValue(func, out var schema)) throw new CommandArgumentException($"Function {func} is not a registered function");
                helpText = HelpGenerator.GenerateUsage(func, schema);
            }
            _logger.Log("CommandManager", helpText);
        }

        public void PushCommand(string command) {
            // Special Function which overrides other commands and runs instantly
            if(command == "cancelAll") {
                CancelAll();
                return;
            } else if(command.StartsWith("help")) {
                ShowHelpText(command);
                return;
            }
            PrecompiledCommand compiled = _compiler.CompileCommand(command);
            _commandQueue.Enqueue(compiled);
        }

        public void PushCommand(PrecompiledCommand command) {
            _commandQueue.Enqueue(command);
        }
    }
}