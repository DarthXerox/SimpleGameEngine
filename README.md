Simple Game Engine
==============
Hello, this my *very simple* game engine created using OpenGL (exactly OpenTK + C#).  
3D game creator is probably more accurate, as it offers no UI to interact with objects, lights etc.  
and add them directly to a specific place on a map, but rather all changes have to made in code,
but this process is very simple.
These features can be easily played around with:  
- Normal mapping technique
- Adding a new 3D object to the game, using:
	- an .obj file containing a single-mesh 3D model and assigning it multiple positions, scales and rotations within the game world  
	- any 2D diffuse texture and any 2D normal texture  
	- any material using an .mtl file  
- Creating an arbitrary text GUI with clickable text boxes that get highlighted and play a sound  
  when a mouse hovers over them  
- Playing .wav sounds (supports playing only one sound at a time)
- Detecting collisions of ingame objects on 1 to N scheme (e.g. player and trees) using simple hitboxes
- Specifying the world's surface based on a heatmap
- Adding multiple light sources with diferent light casting types (:grey_exclamation: right now this might lead to FPS drops)

Forest 
--------------
This is a sample game to show the features of the program.  
All objects have increased shininess level to make the normal mapping
more visible. These 2 pictures show the difference between normal mapping technique which determines normals based on normal texture as opposed to
determining normals from the surface of the given 3D model.  

![Without normal mapping](../Pictures/no_normal_mapping.png?raw=true =100x100)  
![With normal mapping](../Pictures/normal_mapping.png?raw=true =100x100)  

![](https://gyazo.com/eb5c5741b6a9a16c692170a41a49c858.png =250x25)  


Requirements  
--------------
**To launch Forest game**  
 - Download the repository  
 - Head to SimpleGameEngine/bin/Release and launch Forest.exe (Windows OS is necessary)  
 - Ingame controls can be found in HELP option in the Main menu  
  
**To open the code in VS**  
 - .NET Framework 4.7.2

:warning: If any problem occurs with nuget packages (I included them in the repository but just in case)  
download these packages:
 - OpenTK 3.2.0
 - AssimpNet 4.1.0
 - SharpFont.Dependecies 2.6.0
 - SharpFont 4.0.1


