# FluiDemo

This was originally developed by Rene Schulte in 2008 but still works with Visual Studio 2015!

Flui°D°emo is an interactive 2D physically based fluid simulation engine using a Smoothed Particle Hydrodynamics (SPH) approach as described in the paper "Particle-Based Fluid Simulation for Interactive Applications" by M. Mueller et al. This approach utilizes the Navier-Stokes equation and SPH to simulate the behaviour of a fluid. The resulting differential equation is solved with a basic Verlet integration.
This all is implemented in a flexible and small 2D particle system engine, including emitters, consumers and a renderer for Meta-Balls (Blobs). The Meta-Balls are drawn with a Render-To-Texture technique using OpenGLs framebuffer extension (FBO), a procedural generated gaussian distribution texture and alpha-testing. The collision handling is done with the Separating-Axis-Theorem (SAT) and Oriented Bounding Boxes (OBBs).

The whole project is written in C# 3.0 against the .Net Runtime 2.0 and released under the GPLv3 license. The visualization is done with OpenGL (OpenTK library). The download includes the binaries, the source code (including Visual C# Express 2008 solution) and the OpenTK library. See the "Readme.txt" for further instructions and requirements.

By now my SPH code was ported to several other platforms and an awesome iPhone game called Splash !!! uses it. The platforms and languages include Unity, C++, D, JS, ...

There's also a video at Vimeo: http://vimeo.com/4391370
