using Cysharp.Threading.Tasks;
using Louis.CustomPackages.CommandLineInterface.Core;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using VContainer;


namespace Louis.CustomPackages.CommandLineInterface.UI {
    public class Console : MonoBehaviour, IOutput {
        VisualElement _rootContainer;
        ScrollView _outputScroll;
        TextField _inputField;

        [Header("Settings")]
        [SerializeField] int _maxLogEntries = 100;
        [SerializeField] float _timeBeforeHideOutput = 3f;
        [SerializeField] ConsoleMode _defaultMode = ConsoleMode.OpenOnMessage;

        [Header("UI Settings")]
        [SerializeField] VisualTreeAsset _consoleLayout;
        [SerializeField] PanelSettings _panelSettings;
        [SerializeField] int sortOrder = 1000;
        [Space(10)]
        [SerializeField] FontAsset _consoleFont;

        [Inject] readonly ICommandHandler _commandHandler;
        [Inject] readonly ICommandOutputProvider _outputProvider;
        [Inject] readonly ICommandRegistry _commandRegistry;

        ConsoleMode _mode;
        ConsoleMode Mode {
            get => _mode;
            set {
                _mode = value;
                if(_mode == ConsoleMode.AlwaysOpen) {
                    SetOutputVisibility(true);
                } else if(_mode == ConsoleMode.AlwaysClosed) {
                    SetOutputVisibility(false);
                }
            }
        }

        float _hideTimer;

        private void Awake() {
            // 1. Configure the UI Document at runtime
            UIDocument uiDoc = gameObject.AddComponent<UIDocument>();
            uiDoc.panelSettings = _panelSettings;
            uiDoc.visualTreeAsset = _consoleLayout;
            uiDoc.sortingOrder = sortOrder;

            // 2. Query elements
            var root = uiDoc.rootVisualElement;
            _rootContainer = root.Q<VisualElement>("console-container");
            _outputScroll = root.Q<ScrollView>("output-box");
            _inputField = root.Q<TextField>("input-field");

            // 3. Setup Events
            _inputField.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);

            // 4. Setup Initial State: Hidden
            ApplyFont();
            SetInputVisibility(false);
            SetOutputVisibility(false);
        }

        private void OnEnable() {
            _outputProvider.AttachOutput(this);
            _commandRegistry.RegisterCommand(
                "echo",
                new CommandSchema()
                    .WithDescription("Outputs some text to the console")
                    .Required<string>("output", 0, "The text you want to output to the console"),
                Echo);
            _commandRegistry.RegisterCommand(
                "clear",
                new CommandSchema()
                    .WithDescription("Clears the Console"),
                Clear);
            _commandRegistry.RegisterCommand(
                "setConsoleMode",
                new CommandSchema()
                    .WithDescription("Set the Console to be Visible, Hidden, or to Appear when a message is sent")
                    .FlagChoice(
                        name: "mode",
                        defaultValue: _defaultMode,
                        options: new[] {
                            ("alwaysOpen", ConsoleMode.AlwaysOpen),
                            ("alwaysClosed", ConsoleMode.AlwaysClosed),
                            ("openOnMessage", ConsoleMode.OpenOnMessage)
                        },
                        description: "Which mode the console should be set to")
                    .FlagShorthands(
                        ("o", "alwaysOpen"),
                        ("c", "alwaysClosed"),
                        ("m", "openOnMessage")),
                SetConsoleMode);
        }

        private void OnDisable() {
            _outputProvider.DetachOutput(this);
            _commandRegistry.UnregisterCommand("echo");
            _commandRegistry.UnregisterCommand("clear");
            _commandRegistry.UnregisterCommand("setConsoleMode");
        }

        private void Update() {
            if(Keyboard.current.backquoteKey.wasPressedThisFrame) {
                // Show the command line
                SetInputVisibility(!_inputVisible);
            }

            if(Keyboard.current.escapeKey.wasPressedThisFrame) {
                // Hide the command line
                SetInputVisibility(false);
            }

            if(_outputVisible && _hideTimer > 0f && Mode == ConsoleMode.OpenOnMessage) {
                _hideTimer -= Time.deltaTime;
                if(_hideTimer <= 0f) {
                    SetOutputVisibility(false);
                }
            }
        }

        void ApplyFont() {
            if(_consoleFont != null && _rootContainer != null) {
                FontDefinition fontDef = FontDefinition.FromSDFFont(_consoleFont);
                _rootContainer.style.unityFontDefinition = fontDef;
            }
        }

        void OnKeyDown(KeyDownEvent evt) {
            if(evt.keyCode != KeyCode.Return && evt.keyCode != KeyCode.KeypadEnter) return;
            // 1. Get the value
            string command = _inputField.value;

            // 2. Only process if it's not empty (prevents spamming empty enters)
            if(!string.IsNullOrEmpty(command)) {
                _commandHandler.PushCommand(command);
                _inputField.value = "";
            }

            // 3. THE FIX: Prevent the text field from "consuming" the enter key 
            // as a functional input.

            SetInputVisibility(false);
            evt.StopImmediatePropagation();
        }

        bool _outputVisible;
        void SetOutputVisibility(bool visible) {
            // Check current mode for visibility override rules
            if(Mode == ConsoleMode.AlwaysOpen) visible = true;
            else if(Mode == ConsoleMode.AlwaysClosed) visible = false;

            if(visible) _hideTimer = _timeBeforeHideOutput; // Reset the hide timer when shown
            _outputVisible = visible;
            _outputScroll.EnableInClassList("hidden", !visible);

        }

        bool _inputVisible;
        void SetInputVisibility(bool visible) {
            _inputField.EnableInClassList("hidden", !visible);
            _inputVisible = visible;

            if(visible) {
                _inputField.Focus();
                _inputField.schedule.Execute(() => _inputField.value = string.Empty).ExecuteLater(1);
            } else {
                _inputField.Blur();
            }
        }

        public void Write(string output) {
            // 0. Set the Output log to visible if it isn't already
            SetOutputVisibility(true);

            // 1. Create the element
            Label newEntry = new (output);
            newEntry.AddToClassList("log-entry");

            // 2. Add to the scroll view
            _outputScroll.Add(newEntry);

            // 3. Keep log size under control
            if(_outputScroll.childCount > _maxLogEntries) {
                _outputScroll.RemoveAt(0); // Remove the oldest entry
            }

            // 4. Force scroll to the bottom (using schedule to ensure layout update)
            _outputScroll.RegisterCallback<GeometryChangedEvent>(ScrollToBottom);
        }

        void ScrollToBottom(GeometryChangedEvent evt) {
            _outputScroll.scrollOffset = new Vector2(0, _outputScroll.contentRect.height);
            _outputScroll.UnregisterCallback<GeometryChangedEvent>(ScrollToBottom);
        }

        #region Command Line Functions
        UniTask Echo(ICommandLogger logger, BoundArgs args, CancellationToken token) {
            Write($"> {args.Get<string>("output")}");
            return UniTask.CompletedTask;
        }

        UniTask Clear(ICommandLogger logger, BoundArgs args, CancellationToken token) {
            SetOutputVisibility(true);
            _outputScroll.Clear();
            return UniTask.CompletedTask;
        }

        UniTask SetConsoleMode(ICommandLogger logger, BoundArgs args, CancellationToken token) {
            var mode = args.Get<ConsoleMode>("mode");
            Mode = mode;
            logger.Log(this, $"Set Console Mode to {Mode}");
            return UniTask.CompletedTask;
        }
        #endregion
    }

    public enum ConsoleMode {
        AlwaysOpen,
        AlwaysClosed,
        OpenOnMessage
    }
}