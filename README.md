## App video
https://user-images.githubusercontent.com/124143253/227535925-640429d8-1562-4d69-aab7-9fbac8aa9f99.mp4

## Description
Sonar Viewer - app for 3D-model viewing in augmented reality. Models are downloaded from Sketchfab. The app allows the user to search models by name, vertices count, texture size and license type.

## Features
- Sketchfab Authentication and API communication
- .glb 3d-model viewer
- ARFoundation (ARCore/ARKit) system for AR view
- UnityWebRequest used for API communication
- Model transform control. Model Positioning with screen center - model placing on any visible AR plane by screen center. It is possible to rotate the model (left/right) by the two-finger swipe. If the user taps on the screen, the model anchors to the plane on which it is positioned.
- Model author and information: sketchfab model page link, author, license.

## Built With
- Project implemented with [Unity 3D](https://unity.com).
- 3D-models are downloaded from [Sketchfab API](https://sketchfab.com/developers).
- For 3D-model import (in .glb format) [glTFast](https://github.com/atteneder/glTFast).
- For network communication [UnityWebRequest](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html).

## License
Distributed under the MIT License. See [LICENSE](https://github.com/Onix-Systems/unity-sonar-viewer/blob/dev/LICENSE) for more information.

## Contact
Contact us: [sales@onix-systems.com](https://onix-systems.com/contact-us)
