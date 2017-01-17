Description
°°°°°°°°°°°

Flui°D°emo is an interactive 2D physically based fluid simulation using a Smoothed Particle Hydrodynamics (SPH) approach as described in the paper "Particle-Based Fluid Simulation for Interactive Applications" by M. Müller et al., which utilizes the Navier-Stokes equation and SPH to simulate the behaviour of the fluid. The resulting differential equation is solved with a basic verlet integration.
The whole project is written in C# 3.0 against the .Net Runtime 2.0 and is released under the GPLv3 license. The visualization is done with OpenGL, using the OpenTK library (http://www.opentk.com).
As a by product a little framework for a flexible 2D particle systems, including emitters, consumers and a renderer for Meta-Balls (Blobs) was developed. 
The Meta-Balls are drawn with a Render-To-Texture technique using OpenGLs framebuffer extension (FBO), a procedural generated gaussian distribution texture and alpha-testing. 
The collision handling is done with the Separating-Axis-Theorem (SAT) and Object Bounded Boxes (OBBs).

You can also watch a video at YouTube (http://www.youtube.com). Search for "Flui°D°emo" or "FluiDemo" to see a video or use this link http://www.youtube.com/watch?v=0bL80G1HX9w.


Requirements
°°°°°°°°°°°°

- An OpenGL 1.1 capable graphics card
- A mouse with a wheel to change some constants and rotate the boxes
- A .Net Runtime 2.0
  - Windows: Microsoft .Net Framework http://www.microsoft.com/downloads/details.aspx?FamilyID=0856eacb-4362-4b0d-8edd-aab15c5e04f5
  - Linux / (MacOS) / Windows / ...: Mono http://www.go-mono.com/mono-downloads/download.html


Instructions
°°°°°°°°°°°°

Press [F1] to hide / show the following instructions in the program.

Press [R] to change particle rendering (Meta-Balls or Points).
Press [D] to draw velocity direction, if the particles are rendered as points.
Press [E] to switch emitter on / off.
Press [P] to pause.
Press [Space] to tilt (add random impulse to all particles).
Press [Esc] to close the program.

Use left mouse button <LMB> to select a box (OBB).
Select a box + <LMB> to move the selected box.
Hold [Ctrl] + <RMB> to remove a box.
Hold [Ctrl] + <LMB> to draw a new box, release <LMB> to add the drawn box. The box is added as the Axis Aligned Bounding Box of the drawn path.

Hold [Alt] + <LMB> to exert a negative force field to the particles in a certain range.
Hold [Alt] + <RMB> to exert a positive force field to the particles in a certain range.

Use mousewheel <MW> to change some values (hold [Shift] to change smaller steps):
Select a box + <MW> to rotate the selected box.
Hold [V] + <MW> to change the viscosity of the fluid.
Hold [B] + <MW> to change the bounciness for collision resolving between the particles and the boxes (OBBs).
Hold [F] + <MW> to change the friction for collision resolving between the particles and the boxes (OBBs).


Legal Stuff
°°°°°°°°°°°

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with this program ("License.txt"); if not, see <http://www.gnu.org/licenses/>. 

See also the OpenTK license "OpenTK_License.txt".


Author
°°°°°°

Name:   Rene Schulte
E-mail: fluid@rene-schulte.info
WWW:    http://www.rene-schulte.info

Copyright © 2008 Rene Schulte