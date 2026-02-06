# TETR04o
A Tetris parody game made for VRChat using UdonSharp

![TETR04o arcade machine](https://github.com/Andrey04o/TETR04o-vpm/blob/main/screenshot1.png?raw=true)

[Trailer](https://youtu.be/SP11tQ6hIlU)

[VRChat world](https://vrchat.com/home/launch?worldId=wrld_2af7b8d2-ed8f-4e51-9fab-b8ddf98ade14)

# Features

- VR, PC, Touch control
- Rotation 90 and -90 degrees
- Hold pieces
- Ghost piece
- Wall kicks
- Lock delay
- Synchronization with other players
(it's not syncing the whole board every time, only the changed lines)
- Competition with other players
- Control settings
- Handling settings

# Installation

<details><summary>

### Import with [VRChat Creator Companion](https://vcc.docs.vrchat.com/vpm/packages#user-packages):</summary>

> 1. In VRChat Creator Companion, navigate to `Settings` > `Packages` > `Add Repository`
> 2. Enter <pre lang="md">https://andrey04o.github.io/TETR04o-vpm/index.json</pre>
> 3. `TETR04o` should now be visible under `Community Repositories`

</details><details><summary>

### Import from [Unitypackage](https://docs.unity3d.com/2019.4/Documentation/Manual/AssetPackagesImport.html):</summary>

> 1. Download latest package from [here](https://github.com/Andrey04o/TETR04o-vpm/releases)
> 2. Import the downloaded .unitypackage into your Unity project

</details>

# How to use

1. Find in the `Prefabs` folder 
- `TETR04o` prefab
- `Controls settings` prefab 
- `Multiplayer` prefab<br/>
Put them to your world.

2. In the `Controls settings` prefab, 
- select the `Hotkey` gameobject and in the inspector window press the `Set hotkeys reference to all arcade machines` button
- select the `Handling` gameobject and in the inspector window press the `Set handling reference to all arcade machines` button
- select the `Extra` gameobject and in the inspector window press the `Get all T04oGStations` button

3. In the `Multiplayer` prefab, in the inspector window press the `Get all arcade machines and set ids` button

That's it, now it should work fine.</br>
If you add another `TETR04o` prefab to your world, please repeat steps 2 and 3

You can find an example in `Packages/TETR04o/Runtime/Scenes/ExampleScene`

## Developed by Andrey04o

[VRChat profile](https://vrchat.com/home/user/usr_fee3cab2-a580-4da8-a002-b5f37c7c7b6f)

[Patreon](https://patreon.com/04o)

[Boosty](https://boosty.to/04o)

[itch.io](https://andrey04o.itch.io/)

