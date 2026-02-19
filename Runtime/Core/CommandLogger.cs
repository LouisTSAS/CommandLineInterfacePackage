using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Louis.CustomPackages.CommandLineInterface.Core {
    [AddComponentMenu("Command Line Interface/Command Logger")]
    internal class CommandLogger : MonoBehaviour, ICommandOutputProvider, ICommandLogger {
        [Header("Settings")]
        [SerializeField] int _messageIndent = 20;
        [SerializeField] bool _outputToConsole = true;
        readonly HashSet<IOutput> _outputSet = new();
        public void AttachOutput(IOutput output) {
            _outputSet.Add(output);
        }

        public void DetachOutput(IOutput output) {
            _outputSet.Remove(output);
        }

        public void Log(Object sender, string message) => Output($"<b><color=green>{sender.name}: </color></b><indent={_messageIndent}%>" + message + $"</indent>");
        public void Log(string sender, string message) => Output($"<b><color=green>{sender}: </color></b><indent={_messageIndent}%>" + message + $"</indent>");
        public void Log(string message) => Output($"<indent={_messageIndent}%>" + message + $"</indent>");
        public void LogError(string message) => Output($"<b><color=red>Error: </color></b><indent={_messageIndent}%>" + message + $"</indent>");

        void Output(string output) {
            if(_outputToConsole) Debug.Log($"<b>CommandManager: </b>{output}");
            if(_outputSet == null) return;
            foreach(var outputChannel in _outputSet) {
                outputChannel.Write(output);
            }
        }
    }
}