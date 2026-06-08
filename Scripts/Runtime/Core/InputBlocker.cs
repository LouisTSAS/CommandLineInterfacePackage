namespace Louis.CustomPackages.CommandLineInterface.Core {
    public interface IInputBlocker {
        bool IsInputBlocked { get; }
    }

    public class InputBlocker : IInputBlocker {
        public bool IsInputBlocked { get; set; }
    }
}