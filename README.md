# VUSharp Reference Handler
A scene GameObject singleton to help handle references in Udon

## Features

- Runtime handling of singleton classes and PlayerObjects in Udon

- Supports objects instantiate at runtime without the need to create prewarmed prefabs

- Each new object will only ever need to perform a single Unity Find to obtain all stored references.

## How to install

### VRChat Package Manager

<https://vincil-vr.github.io/VUSharp-Reference-Handler-Listing/>

### Unity Package Manager

In your Unity project, go to `Window > Package Manager` then click the top left `+`, click on `Add package from git URL` and paste this link:
<https://github.com/Vincil-VR/VUSharp-Reference-Handler/tree/main/Packages/com.vincil.vusharp.reference-handler>

### Unity Package

Download the latest package from the [latest release](https://github.com/Vincil-VR/VUSharp-Reference-Handler/releases/latest)

Then import the contained .unitypackage

## Setup

In your project navigate to `Packages > VUSharp REference Handler > Runtime > Prefabs` and then click and drag the `ReferenceHandler` prefab into your scene.

  > Make sure there is ever only one of the `ReferenceHandler` prefab in your scene at a time.

## Usage

### Adding Singleton References

Adding a singleton reference to the `VUSharpReferenceHandler` instance is done via:

```c#
AddSingleton(/*[instance of the singleton to be added]*/);
```

You can have a singleton reference add itself in its Start.

> **NOTE:** `VUSharpReferenceHandler.Instance()` assumes that the `VUSharpReferenceHandler` instance is contained on a `GameObject` titled `ReferenceHandler` at the root of the scene hierarchy.

```C#
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using Vincil.VUSharp.ReferenceHandler;

public class ExampleSingleton : UdonSharpBehaviour
{
    VUSharpReferenceHandler referenceHandler;
    void Start()
    {
        referenceHandler = VUSharpReferenceHandler.Instance();
        referenceHandler.AddSingleton(this);
    }
}
```

If needed, you can also add a singleton reference from a different class.

```c#
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using Vincil.VUSharp.ReferenceHandler;

public class OtherScript : UdonSharpBehaviour
{
    VUSharpReferenceHandler referenceHandler;
    ExampleSingleton exampleSingleton;
    void Start()
    {
        referenceHandler = VUSharpReferenceHandler.Instance();

        // Get the ExampleSingleton instance in desired way

        referenceHandler.AddSingleton(exampleSingleton);
    }
}
```   

### Retrieving Singleton References

Retrieving a singleton reference from the `VUSharpReferenceHandler` instance is done via:

```c#
exampleSingleton = (ExampleSingleton)referenceHandler.GetSingleton(GetUdonTypeName<ExampleSingleton>());
```
  
The Udon type name of the desired singleton class is provided to `GetSingleton` method and an `UdonSharpBehaviour` is returned.  This is then cast into the singleton's type.

> **NOTE:** Udon type name of a `UdonSharpBehaviour` can sometimes be but will not always be the class name.  It appears to be the class name with the namespace appended before it.


It is possible for a class instance to try retrieving a singleton reference before that singleton has had a chance to add itself to the reference handler.  This can often occur during scene start.  In those situations it is advised to use the method of retrieving singleton instances that includes a [Unity Find](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/GameObject.Find.html).

```c#
exampleSingleton = (ExampleSingleton)referenceHandler.GetSingleton(GetUdonTypeName<ExampleSingleton>(), "[path/name of desired singleton instance]");
```

If the reference handler finds it has not been provided the singleton instance desired, it will attempt to find the singleton instance using a Unity Find using the string provided in the second parameter.

#### Example
```C#
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using Vincil.VUSharp.ReferenceHandler;

public class MyClass : UdonSharpBehaviour
{
    ExampleSingleton exampleSingleton;
    VUSharpReferenceHandler referenceHandler;
    void Start()
    {
        referenceHandler = VUSharpReferenceHandler.Instance();
        exampleSingleton = (ExampleSingleton)referenceHandler.GetSingleton(GetUdonTypeName<ExampleSingleton>(), "/ExampleSingleton");
    }
}
```

### Adding PlayerObject References

This can be performed similarly to adding a singleton.  The reference handler will automatically associate the instance with its owner.  And you can once again perform this in the PlayerObject class's Start.

```c#
referenceHandler.AddPlayerObject(/*[instance of the PlayerObject to add]*/);
```


### Retrieving PlayerObject References


This can be performed similarly to retrieving a singleton except you must also specify the owner.

```c#
VRCPlayerApi desiredPlayer;
ExamplePlayerObject playerObject = (ExamplePlayerObject)referenceHandler.GetPlayerObject(GetUdonTypeName<ExamplePlayerObject>(), desiredPlayer);
```
  
Retrieving `PlayerObjects` does not contain a backup method, but if you try retrieving them in the [`OnPlayerRestored`](https://creators.vrchat.com/worlds/udon/graph/event-nodes#onplayerrestored) event, that should give them enough time to run their start and add themselves.
