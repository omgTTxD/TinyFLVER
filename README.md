# TinyFLVER
<img src="https://i.postimg.cc/6qmz4CWF/Screenshot.png"/>
<br>
Features and considerations:<br>
- Next planned feature - PBR shading.<br>
- Direct3D9 style diffuse and normal/specular textures. To set diffuse texturing add texture with "_d" or "_a" in name and DDS format in any material slot. I will implement .matbins eventually.<br>
- Removal of meshes and materials.<br>
- FBX Importing. Right now the algorithm is from Flver_Editor 2.6, I refactored it a little, and I hope I didn't break something... I will look at it later when I know more about why almost any mod on Nexus is with broken normals, and how to fix it.<br>
- I also didn't have time to implement QoL features like customizable controls... But it is preatty easy to do from the code.<br>