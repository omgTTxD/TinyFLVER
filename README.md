# TinyFLVER
<img src="https://github.com/omgTTxD/TinyFLVER/blob/v0.1/Screenshot.png"/>
<br>
This tool is inspired by https://github.com/asasasasasbc/FLVER_Editor. I tried to make it faster and remove all unneccesary funtionality. I am doing most of the work in Blender, so this tool needs only to import .fbx, set materials and textures and nothing more. Basic support for texturing (only diffuse at the moment). Right now I am working on the new version which will focus more on shaders.<br>
- Mesh deleting should work, but I didn't test it much.<br>
- Texturing works as follows: to find diffuse texture I check all texture path in material, to find texure with "_a" or "_d" in the path, then I search it in folder which contains flver. This isn't reliable, but I will rework it in the next version anyway.<br>
- FBX importing code is mostly untouched from FLVER_Editor, and I think there are errors with tangents and normals. I will try to rewrite it in the next version.<br>