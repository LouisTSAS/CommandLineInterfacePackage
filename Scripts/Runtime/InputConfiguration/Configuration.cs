using System;

namespace Louis.CustomPackages.CommandLineInterface.CommandConfiguration {
    [Serializable]
    public class Configuration {
        public string[] run_at_start;
        public InputCommandMapping[] input_commmand_map;
    }

    [Serializable]
    public class InputCommandMapping {
        public string input;
        public string command;
    }
}
