using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UdonSharp;

/// <summary>
/// Adds a context menu for C# script assets that:
/// - Replaces occurrences of the word "MonoBehaviour" with "MultiBehaviour" in the selected .cs file
/// - Creates a `UdonSharpProgramAsset` in `Assets/UdonSharpProgramAssets` with the same name and links
///   its `sourceCsScript` field to the selected `MonoScript`.
///
/// Usage: Right-click a C# script in the Project window, then choose
/// `U# Tools/Create UdonSharp Program Asset and Rename To MultiBehaviour`.
/// </summary>
public static class CreateUdonSharpAssetAndRename
{
    private const string MenuPath = "Assets/U# Tools/Create UdonSharp Program Asset and Rename To UdonSharpBehaviour";
    private const string TargetFolder = "Assets/TETR04o/UdonSharpProgramAssets";

    [MenuItem(MenuPath, true)]
    private static bool Validate_CreateProgramAssetAndRename()
    {
        var obj = Selection.activeObject as MonoScript;
        if (obj == null) return false;
        string path = AssetDatabase.GetAssetPath(obj);
        return !string.IsNullOrEmpty(path) && path.EndsWith(".cs", System.StringComparison.OrdinalIgnoreCase);
    }

    [MenuItem(MenuPath, false, 2000)]
    private static void CreateProgramAssetAndRenameSelectedScript()
    {
        MonoScript script = Selection.activeObject as MonoScript;
        if (script == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a C# script (MonoScript) in the Project window.", "OK");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(script);
        string fullPath = Path.GetFullPath(assetPath);

        if (!File.Exists(fullPath))
        {
            EditorUtility.DisplayDialog("Error", $"File not found: {fullPath}", "OK");
            return;
        }

        string originalText = File.ReadAllText(fullPath);

        // Replace only whole-word occurrences of MonoBehaviour -> MultiBehaviour
        string newText = Regex.Replace(originalText, "\\bMonoBehaviour\\b", "UdonSharpBehaviour");

        bool didReplace = newText != originalText;

        if (didReplace)
        {
            File.WriteAllText(fullPath, newText);
            Debug.Log($"[U#] Replaced 'MonoBehaviour' -> 'UdonSharpBehaviour' in {assetPath}");
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }
        else
        {
            Debug.Log($"[U#] No occurrences of 'MonoBehaviour' found in {assetPath}");
        }

        // Ensure target folder exists
        if (!AssetDatabase.IsValidFolder(TargetFolder))
        {
            string parent = "Assets/TETR04o";
            AssetDatabase.CreateFolder(parent, "UdonSharpProgramAssets");
            AssetDatabase.Refresh();
        }

        // Create (or reuse) an UdonSharpProgramAsset with the same name as the script
        string scriptName = Path.GetFileNameWithoutExtension(assetPath);
        string programAssetPath = Path.Combine(TargetFolder, scriptName + ".asset").Replace("\\", "/");

        // If an asset already exists, prompt to overwrite or use existing
        UdonSharp.UdonSharpProgramAsset programAsset = AssetDatabase.LoadAssetAtPath<UdonSharp.UdonSharpProgramAsset>(programAssetPath);

        if (programAsset == null)
        {
            programAsset = ScriptableObject.CreateInstance<UdonSharp.UdonSharpProgramAsset>();
            AssetDatabase.CreateAsset(programAsset, programAssetPath);
            Debug.Log($"[U#] Created UdonSharpProgramAsset at {programAssetPath}");
        }
        else
        {
            Debug.Log($"[U#] Found existing UdonSharpProgramAsset at {programAssetPath}, updating reference.");
        }

        // Link the MonoScript to the program asset
        programAsset.sourceCsScript = script;
        EditorUtility.SetDirty(programAsset);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Focus the created asset in the Project window
        var created = AssetDatabase.LoadAssetAtPath<UdonSharp.UdonSharpProgramAsset>(programAssetPath);
        if (created != null)
            Selection.activeObject = created;

        string msg = didReplace
            ? $"Converted script and created/updated UdonSharpProgramAsset: {programAssetPath}"
            : $"No replacements were necessary; created/updated UdonSharpProgramAsset: {programAssetPath}";

        EditorUtility.DisplayDialog("UdonSharp Program Asset", msg, "OK");
    }
}
