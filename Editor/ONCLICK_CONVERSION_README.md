# OnClick Event Conversion Tool

## Overview
This tool automatically converts OnClick events from old method calls to `UdonBehaviour.SendCustomEvent(string)` calls in your VRChat project.

### Example Conversion
- **Before**: Button OnClick → `ObjectBuying.Press()`
- **After**: Button OnClick → `UdonBehaviour.SendCustomEvent("Press")`

## How It Works

The script (`ConvertOnClickEvents.cs`) performs the following operations:

1. **Scans all buttons** in the current scene for OnClick event listeners
2. **Identifies old-style method calls** that point to object methods (e.g., `ObjectBuying.Press()`)
3. **Creates or finds an UdonBehaviour** on the button's GameObject (or parent)
4. **Converts the listener** to call `UdonBehaviour.SendCustomEvent()` with the method name as a parameter
5. **Logs all changes** for verification

## Usage

### Step 1: Open Your Scene
Open the scene containing buttons with OnClick events you want to convert in the Unity Editor.

### Step 2: Run the Conversion Tool
In the Unity Editor menu bar:
```
Tools → VRChat → Convert OnClick Events to UdonBehaviour.SendCustomEvent
```

### Step 3: Review the Results
- A dialog will show you the number of buttons converted
- Check the Console window for detailed logs of each conversion
- Look for lines like: `✓ Converted button 'ButtonName' event from 'ObjectName.MethodName()' to UdonBehaviour.SendCustomEvent("MethodName")`

### Step 4: Save Your Scene
The script automatically marks the scene as dirty and saves assets. Make sure to save your scene file (`Ctrl+S` or `Cmd+S`).

## How It Creates UdonBehaviour References

The script uses this priority:
1. **Reuses existing UdonBehaviour** on the button's GameObject (if present)
2. **Searches parent objects** for an existing UdonBehaviour
3. **Creates a new UdonBehaviour** on the button if none exists

You'll need to properly configure the UdonBehaviour with your custom events matching the method names used in the conversions.

## Important Notes

### UdonBehaviour Configuration
After conversion, make sure your UdonBehaviour has public methods or custom events matching the converted method names:

```csharp
public void Press()
{
    // Your implementation here
}
```

Or in UdonSharp:

```csharp
public void Press()
{
    // Your implementation here
}
```

### Backup Your Scenes
Before running the conversion tool, backup your scene files in case you need to revert changes.

### Manual Review
Always review the Console output to verify that conversions were performed as expected.

## Troubleshooting

### No Buttons Found
- Make sure the buttons are in the current open scene
- Check that Button components are properly attached

### Conversion Not Working
- Ensure the scene is not read-only
- Check Console for error messages
- Verify that the target objects and methods exist

### Already Converted Buttons
The script will skip buttons that already have `UdonBehaviour.SendCustomEvent()` listeners to avoid duplicate conversions.

## Advanced: Checking Converted Events

You can verify the conversions by:
1. Selecting a converted button in the Hierarchy
2. Looking at the Inspector
3. Expanding the Button component
4. Checking the "On Click" section to see the `UdonBehaviour.SendCustomEvent()` listener with the string parameter

## Support
If you encounter issues, check:
- The Console window for detailed error messages
- That all VRChat SDK packages are properly installed
- That the scene file is not corrupted
