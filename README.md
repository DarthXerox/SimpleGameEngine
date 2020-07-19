Forest  
==============

Hello,  
this a simple 3D game created using OpenGL (exactly OpenTK + C#)  
It runs on my *simple* game engine, which offers these features:  
- Normal mapping technique
- Add a new 3D object to the game, this requires:
	- an .obj file containing a single-mesh 3D model and assigning it multiple positions, scales and rotations within the game world  
	- any 2D diffuse texture and any 2D normal texture  
	- any material using an .mtl file  
- Create an arbitrary text GUI with clickable text boxes that get highlighted and play a sound  
  when a mouse hovers over them  
- Play .wav sounds (supports playing only one sound at a time)
- Detect collisions of ingame objects on 1 to N scheme (e.g. player and trees) using simple hitboxes
- Specify the world's surface based on a heatmap


Requirements  
--------------
**To launch Forest game**  
 - Download the repository  
 - Head to SimpleGameEngine/bin/Release and launch Forest.exe (Windows OS is necessary)  
 - Ingame controls can be found in HELP in Main menu  
  
**To open the code in VS**  
 - .NET Framework 4.7.2

:warning: If any problem occurs with nuget packages (I included them in the repository but just in case)  
download these packages:
 - OpenTK 3.2.0
 - AssimpNet 4.1.0
 - SharpFont.Dependecies 2.6.0
 - SharpFont 4.0.1


