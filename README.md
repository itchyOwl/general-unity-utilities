# general-unity-utilities

## About the repository
This is a collection of general Unity code that I reuse from project to project.

Most of the code is written by myself. Whenever I've used the work of someone else, I've tried to provide references to the source.

The code is licenced with [MIT](https://opensource.org/licenses/MIT). Feel free to use it as you wish in your projects.

Also check out my blog at [https://itchyowl.com](https://itchyowl.com) and assets that I've published in [Unity Asset Store](https://assetstore.unity.com/publishers/34622).

## Dependencies
Unfortunately this repository is not currently very modular. But that's partly by design. The reason for not being modular here, is that much of the code is reused in multiple places. However, I may try to change this in future.

Many scripts are depend on the extension methods found in the [Extensions](Extensions) folder and the files in the [General](General) folder. Moreover, many UI scripts in the [UI] folder depend on the [Tweener](Auxiliary/Tweener.cs) that is found in the [Auxiliary](Auxiliary) folder.

## Important classes
[Tweener](Auxiliary/Tweener.cs) is used as a base class by many scripts that rely on animating values and properties. It provides tweening functionality for just about anything. It's not meant to replace the popular animation libraries like [iTween](http://itween.pixelplacement.com) or [DOTween](https://dotween.demigiant.com/). Instead, it's meant to provide component oriented tweening behaviour that can be extended by using inheritance. I've used it for example to tween [animation layers](Auxiliary/AnimationLayerTweener) and to [move objects in trajectories](Auxiliary/BezierPositionTweener.cs).

In the [UI](UI) folder you can find [GUIManager](UI/GUIManager.cs), which is a [singleton](Auxiliary/Singleton) class. It's designed to work as a simple manager that can be thrown into any project. Currently if provides methods for displaying screen space notifications and floating messages. It also manages all the windows. For most projects, I've created a dedicated GUIManager that inherits this base class.

I'm not a great fan of the singleton pattern, but in some cases it's just unavoidable. [Singleton](Auxiliary/Singleton) provides GetInstance<TDerivate>() method that updates the instance to a more specific type, when that's required, so you don't have to manually throw managers into the scene.

GUI settings should be defined in [GUIPreset](UI/GUIPreset.cs) or a ScriptableObject that inherits that class. New ScriptableObjects can be created with [ScriptableObjectFactory](ScriptableObjectFactory/Editor/ScriptableObjectFactory.cs), which uses reflection to find all the ScriptableObjects in the project. 

The purpose of [UIElementFactory](UIElementFactory.cs) and [UIDictionaryElementFactory](UIDictionaryElementFactory.cs) is to manage dynamically created UI elements for example in a [Scroll View](https://docs.unity3d.com/Manual/script-ScrollRect.html). The elements can be anything. However, these factories are designed to work together with [UISelector](UISelector.cs), which handles the selection logic of [toggleable](General/IToggleable.cs) ui elements. [UIToggleIcon](UI/UIToggleIcon.cs) is an example of toggleable ui element.

There's also [UIDraggableElement](UI/UIDraggableElement.cs) to provide draggable functionality, useful for example in inventories. If you'd like to create an inventory with these classes, consider creating a custom icon class that inherits UIToggleIcon and define a reference to the icon data in that class.

For saving and loading files, there's the static [FileManager](DataManagement/FileManager.cs) class.

## Notes
The code is not by any means complete and it will change in due time. Also note that some parts are quite old and have not been tested for a while. If you spot any issues, feel free to create a pull request or [contact me by mail](mailto:contact@itchyowl.com).
