# TinyFLVER

<img width="1609" height="1079" alt="Clipboard_12-15-2025_01" src="https://github.com/user-attachments/assets/bb167e68-9130-4789-a8d2-c625ef336c08" />
<br><br>
This program is inspired by This is inspired by FLVER_Editor by ForsakenSilver. Because I do most of the work in Blender, so I want flver editor to open .flver, import .fbx, check/set material and textures and that's all.<br>
I am working on new version, which will be heavy focused on shaders, so I didn't tested it much. I checked only ER and NR.<br>
<br>
Features and considerations:
- [x] Diffuse texturing. Most textures should work. I am looking for textures in the material, which names contains "_a" or "_d" and assume these are diffuse textures. If there is none, thatn I look texture type with "albedo" or "diffuse" in "type". All textures must be in .dds in the same folder as .flver. TPF is not supported (I don't use it).
- [x] Removal of meshes and materials. I implemented real removal of meshes and materials from FLVER data. Of course there might be errors, so beware.
- [x] *fbx Import. The algorithm is from Flver_Editor, I tried to only refactor it and don't break much... But it doesn't matter really, because I am absolutely sure this algorithm doesn't work right, judging by the fact that there are almost zero mods on the entire Nexus where normals and tangents wouldn't be broken. But I didn't have time to look at it yet.
- [x] also didn't have time to implement features like customizable controls... But it is preatty easy to do from the code.
<br>
Controls:<br>
- RMB to orbit camera, MMB to move, LMB to select meshes.<br>
- Alt + LMB - select all, Ctrl + LMB - add/remove to selection.<br>
- Delete - delete meshes.<br>
- PgUp/PgDown - rotate selected meshes for 90 degrees around X axis<br>
- Space - edit selected mesh's material<br>
- Escape - exit.<br>






