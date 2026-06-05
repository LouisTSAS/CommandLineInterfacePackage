using Cysharp.Threading.Tasks;
using Louis.CustomPackages.CommandLineInterface.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if USE_VCONTAINER
using VContainer;
#endif

namespace Louis.CustomPackages.CommandLineInterface.CommandConfiguration {
    [AddComponentMenu("Command Line Interface/Input Command Mapper")]
    public class InputCommandMapper : MonoBehaviour {
        const string k_CONFIGURATION_FILENAME = "command_map.json";
        Command[] _runOnStart;
        readonly Dictionary<string, Command> _inputToCommandMap = new();

        [Header("Settings")]
        [SerializeField] Configuration _defaultConfiguration;


#if USE_VCONTAINER
        IInputProvider _inputProvider;
        ICommandHandler _commandHandler;
        ICommandCompiler _commandCompiler;
#else
        [SerializeField] GameObject _inputProviderComponent;
        [Header("References")]
        IInputProvider _inputProvider;
        [SerializeField] CommandManager _commandHandler;
        [SerializeField] CommandCompiler _commandCompiler;
#endif

        public IInputProvider InputProvider {
            get => _inputProvider;
            set {
                if(_inputProvider != null) _inputProvider.onInputTriggered -= OnInput;
                _inputProvider = value;
                if(_inputProvider != null) _inputProvider.onInputTriggered += OnInput;
            }
        }

#if USE_VCONTAINER
        public ICommandHandler CommandHandler {
            get => _commandHandler;
            set => _commandHandler = value;
        }

        public ICommandCompiler CommandCompiler {
            get => _commandCompiler;
            set => _commandCompiler = value;
        }
#else
        public CommandManager CommandHandler {
            get => _commandHandler;
            set => _commandHandler = value;
        }

        public CommandCompiler CommandCompiler {
            get => _commandCompiler;
            set => _commandCompiler = value;
        }
#endif

        Configuration Configuration { get; set; }

#if USE_VCONTAINER
        [Inject]
        public void Init(IInputProvider inputProvider, ICommandHandler commandHandler, ICommandCompiler commandCompiler) {
            InputProvider = inputProvider;
            CommandHandler = commandHandler;
            CommandCompiler = commandCompiler;
        }
#endif

        private void Awake() {
#if !USE_VCONTAINER
            if(_inputProviderComponent.TryGetComponent(out IInputProvider inputProvider))
                InputProvider = inputProvider;
            else
                throw new ArgumentException($"[Input Command Mapper] - Assigned Input Provider Component does not implement IInputProvider");

#endif
        }

        private void OnDestroy() {
            if(InputProvider != null)
                InputProvider.onInputTriggered -= OnInput;
        }

        private async void Start() {
            await LoadConfigurationAsync();
            CompileCommands();
            RunStartCommands();
        }

        async UniTask LoadConfigurationAsync() {
            string path = Path.Combine(Application.persistentDataPath, k_CONFIGURATION_FILENAME);

            try {
                // 1. Check if the file exists, if it doesn't, this is the "First Run"
                if(!File.Exists(path)) {
                    if(_defaultConfiguration != null) {
                        string defaultJson = JsonUtility.ToJson(_defaultConfiguration, true);

                        await File.WriteAllTextAsync(path, defaultJson);
                        Configuration = _defaultConfiguration;

                        Debug.Log($"[Input Command Mapper] First run detected. Initialized default configuration");
                    }
                } else {
                    string configJson = await File.ReadAllTextAsync(path);
                    var config = JsonUtility.FromJson<Configuration>(configJson);

                    Configuration = config ?? _defaultConfiguration;
                }
            } catch (Exception ex) {
                Debug.LogWarning($"[Input Command Mapper] Failed dealing with persistent config, falling back to default configuration. {ex.Message}");
                Configuration = _defaultConfiguration;
            }
        }

        void CompileCommands() {
            // Compile start commands
            if(Configuration.run_at_start != null) {
                _runOnStart = Configuration.run_at_start.Select(rawCommand => {
                    var command = CommandCompiler.CreateCommand(rawCommand);
                    CommandCompiler.TryCompile(command);
                    return command;
                }).ToArray();
            }
            // Compile input-based commands
            if(Configuration.input_commmand_map != null) {
                foreach(var mapping in Configuration.input_commmand_map) {
                    var command = CommandCompiler.CreateCommand(mapping.command);
                    CommandCompiler.TryCompile(command);
                    _inputToCommandMap[mapping.input] = command;
                }
            }
        }

        void RunStartCommands() {
            foreach(var command in _runOnStart) {
                CommandHandler.PushCommand(command);
            }
        }

        void OnInput(string inputId) {
            if(_inputToCommandMap.TryGetValue(inputId, out var command)) {
                CommandHandler.PushCommand(command);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Open Persistent Data Folder")]
        private void OpenPersistentDataFolder() {
            UnityEditor.EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
#endif
    }

    public interface IInputProvider {
        event Action<string> onInputTriggered;
    }
}
