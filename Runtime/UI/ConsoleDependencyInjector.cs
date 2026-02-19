using Louis.CustomPackages.CommandLineInterface.Core;
using UnityEngine;

namespace Louis.CustomPackages.CommandLineInterface.UI {
    [RequireComponent(typeof(Console))]
    public class ConsoleDependencyInjector : MonoBehaviour {
        [Header("References")]
        [SerializeField] CommandManager _commandHandler;
        [SerializeField] CommandRegistry _commandRegistry;
        [SerializeField] CommandLogger _commandLogger;

        private void Awake() {
            Console c = GetComponent<Console>();
            c.Bind(_commandHandler, _commandLogger, _commandRegistry);
        }
    }
}
