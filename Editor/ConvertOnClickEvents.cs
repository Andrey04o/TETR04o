using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Events;
using VRC.Udon;

/// <summary>
/// Converts OnClick events from old method calls to UdonBehaviour.SendCustomEvent(string) calls.
/// Example: ObjectBuying.Press() -> UdonBehaviour.SendCustomEvent("Press")
/// </summary>
public class ConvertOnClickEvents
{
    [MenuItem("Tools/VRChat/Convert OnClick Events to UdonBehaviour.SendCustomEvent")]
    public static void ConvertAllOnClickEvents()
    {
        int convertedCount = 0;
        int skippedCount = 0;

        // Find all buttons in the current scene
        Button[] allButtons = Object.FindObjectsOfType<Button>(true);

        if (allButtons.Length == 0)
        {
            EditorUtility.DisplayDialog("Info", "No buttons found in the scene.", "OK");
            return;
        }

        Debug.Log($"[OnClick Conversion] Found {allButtons.Length} button(s) in scene. Starting conversion...");

        foreach (Button button in allButtons)
        {
            if (ConvertButtonOnClickEvents(button))
            {
                convertedCount++;
            }
            else
            {
                skippedCount++;
            }
        }

        // Mark scene as dirty to save changes
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Conversion Complete",
            $"Successfully converted: {convertedCount} button(s)\nSkipped (already converted): {skippedCount} button(s)",
            "OK");

        Debug.Log($"[OnClick Conversion] Conversion complete! Converted {convertedCount} button(s). Skipped {skippedCount} button(s).");
    }

    private static bool ConvertButtonOnClickEvents(Button button)
    {
        Button.ButtonClickedEvent clickEvent = button.onClick;
        return ConvertUnityEvent(clickEvent, button);
    }

    private static bool ConvertUnityEvent(Button.ButtonClickedEvent clickEvent, Button button)
    {
        bool hasConverted = false;
        int persistentCount = clickEvent.GetPersistentEventCount();

        // Process listeners in reverse to avoid index issues when removing
        for (int i = persistentCount - 1; i >= 0; i--)
        {
            Object target = clickEvent.GetPersistentTarget(i);
            string methodName = clickEvent.GetPersistentMethodName(i);

            Debug.Log($"[OnClick Conversion] Button '{button.gameObject.name}' - Found listener: Target='{target?.name ?? "null"}', Method='{methodName}'");

            // Check if this is calling a method on an object (not already UdonBehaviour.SendCustomEvent)
            if (target != null && !IsAlreadyUdonBehaviourSendCustomEvent(target, methodName))
            {
                // Get or create UdonBehaviour
                UdonBehaviour udonBehaviour = GetOrCreateUdonBehaviour(button);

                if (udonBehaviour != null)
                {
                    string oldTargetName = target.name;
                    
                    // Clear the old listener
                    UnityEventTools.RemovePersistentListener(clickEvent, i);

                    // Add new listener calling UdonBehaviour.SendCustomEvent
                    AddUdonSendCustomEventListener(clickEvent, udonBehaviour, methodName);

                    hasConverted = true;
                    Debug.Log($"[OnClick Conversion] ✓ Converted button '{button.gameObject.name}' event from '{oldTargetName}.{methodName}()' to UdonBehaviour.SendCustomEvent(\"{methodName}\")");
                }
            }
        }

        return hasConverted;
    }

    private static bool IsAlreadyUdonBehaviourSendCustomEvent(Object target, string methodName)
    {
        if (target is UdonBehaviour && methodName == "SendCustomEvent")
        {
            return true;
        }
        return false;
    }

    private static UdonBehaviour GetOrCreateUdonBehaviour(Button button)
    {
        // Try to get existing UdonBehaviour on the button's gameobject
        UdonBehaviour udonBehaviour = button.GetComponent<UdonBehaviour>();

        if (udonBehaviour == null)
        {
            // Try to find it on a parent
            udonBehaviour = button.GetComponentInParent<UdonBehaviour>();
        }

        if (udonBehaviour == null)
        {
            // Create a new UdonBehaviour on the button's gameobject
            udonBehaviour = button.gameObject.AddComponent<UdonBehaviour>();
            Debug.Log($"[OnClick Conversion] Created new UdonBehaviour on '{button.gameObject.name}'");
        }

        return udonBehaviour;
    }

    private static void AddUdonSendCustomEventListener(Button.ButtonClickedEvent clickEvent, UdonBehaviour udonBehaviour, string methodName)
    {
        // Add persistent listener using UnityEventTools
        // The string parameter will be the method name to call on the UdonBehaviour
        UnityEventTools.AddStringPersistentListener(clickEvent, udonBehaviour.SendCustomEvent, methodName);
    }
}

