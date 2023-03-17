# About The Project

<p align="left">
<img src="./Images~/Logo.png" width="256" height="256"/>
</p>

Sonar Viewer is a demo project that shows our aproach in unity project development. Here you can find our typical project organization, architecture and code style.

Basically, it is an app for 3D-model viewing in augmented reality. Models are downloaded from Sketchfab API in .glb format, so Sketchfab credentials needed (email and password). App allows user to filter models by name, vertices count, texture size and license type. Search results represented as a paginated list of model names. User can choose model from the list to download it. 

Augmeneted model viewing process consists of three phases:
1. Environment Scanning - process of AR planes searching;
2. Model Positioning with screen center - model placing on any visible AR Plane by screen center. It is possible to rotate model (left/right) by two-finger swipe. If user taps on screen, model anchors to the plane where it situated on;
3. Model Transform Edit - when model anchored to the plane it is possible to change model position on the plane by finger swipe, rotate model by two-finger swipe (left/right) and scale by pinch.

Information about viewed model (sketchfab model page link, author, license) is available for user in model info section.

## Built With
Project implemented with [Unity 3D](https://unity.com).

3D-models are downloaded from [Sketchfab API](https://sketchfab.com/developers).

For 3D-model import (in .glb format) [glTFast](https://github.com/atteneder/glTFast) plugin is used.

For network communication [UnityWebRequest](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html) is used.

## Usage
You can download project from a git url with command:
*`git clone https://github.com/Onix-Systems/unity-sonar-viewer.git`*

## License
Distributed under the MIT License. See [LICENSE](https://github.com/Onix-Systems/unity-sonar-viewer/blob/dev/LICENSE) for more information.

## Contact
Project Link: [https://github.com/Onix-Systems/unity-sonar-viewer](https://github.com/Onix-Systems/unity-sonar-viewer)

Contucts: [sales@onix-systems.com](https://onix-systems.com/contact-us)
