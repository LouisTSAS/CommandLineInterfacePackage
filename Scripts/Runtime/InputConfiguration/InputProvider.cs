using System;
using UnityEngine;

namespace Louis.CustomPackages.CommandLineInterface.CommandConfiguration {
    public abstract class InputProvider : MonoBehaviour, IInputProvider {
        public abstract event Action<string> onInputTriggered;
    }

    public interface IInputProvider {
        event Action<string> onInputTriggered;
    }
}
