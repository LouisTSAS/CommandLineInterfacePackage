using UnityEditor;
using UnityEngine;

namespace Louis.CustomPackages.CommandLineInterface.Editor {
    public class MenuExtensions : MonoBehaviour {
        [MenuItem("GameObject/Command Line Interface/Command Manager", false, 10)]
        private static void CreateCommandManager(MenuCommand menuCommand) =>
            CreatePrefab(menuCommand, "Packages/com.louis.commandlineinterface/Prefabs/CommandManager.prefab");

        [MenuItem("GameObject/Command Line Interface/Console", false, 10)]
        private static void CreateConsole(MenuCommand menuCommand) =>
            CreatePrefab(menuCommand, "Packages/com.louis.commandlineinterface/Prefabs/Console.prefab");

        [MenuItem("GameObject/Command Line Interface/Input Command Mapper", false, 10)]
        private static void CreateInputCommandMapper(MenuCommand menuCommand) =>
            CreatePrefab(menuCommand, "Packages/com.louis.commandlineinterface/Prefabs/InputCommandMapper.prefab");

        [MenuItem("GameObject/Command Line Interface/OSC Receiver", false, 10)]
        private static void CreateOSCReceiver(MenuCommand menuCommand) =>
            CreatePrefab(menuCommand, "Packages/com.louis.commandlineinterface/Prefabs/OSCReceiver.prefab");

        static void CreatePrefab(MenuCommand menuCommand, string path) {
            // 2. Load the asset from the package cache
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if(prefab == null) {
                Debug.LogError($"[CommandLine] Could not find prefab at path: {path}");
                return;
            }

            // 3. Instantiate it properly as a prefab link (not a loose clone)
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = prefab.name; // Strip the "(Clone)" suffix

            // 4. Mirror native Unity behavior: Parent it to whatever the user has selected in the Hierarchy
            GameObjectUtility.SetParentAndAlign(instance, menuCommand.context as GameObject);

            // 5. Register the creation with Unity's Undo system so Ctrl+Z works flawlessly
            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");

            // 6. Automatically select the newly created object
            Selection.activeObject = instance;
        }
    }
}