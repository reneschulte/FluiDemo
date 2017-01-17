//
//   Modul:             Fluid physics
//
//   Description:       MainWindow of the application
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-09 08:54:38 +0200 (Mi, 09 Apr 2008) $
//   Changed in:        $Revision: 114 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/MainWindow.cs $
//   Id:                $Id: MainWindow.cs 114 2008-04-09 06:54:38Z rene.schulte $
//
//
//   Copyright (c) 2008 Rene Schulte
//
//   This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License 
//   as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
//   This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
//   See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with this program; ("License.txt").
//   if not, see <http://www.gnu.org/licenses/>. 
//


using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.OpenGL.Enums;
using OpenTK.Math;
using OpenTK.Input;
using OpenTK.Platform;

namespace Fluid
{
   /// <summary>
   /// MainWindow of the application
   /// </summary>
   public sealed class MainWindow : GameWindow
   {
      #region Static Members

      private static string WINDOW_TXT          = "Flui°D°emo - A 2D Fluid Simulation - © 2008 Rene Schulte";
      private static int TEX_SIZE               = 512;
      private static int BLOB_RADIUS            = 8;

      #endregion

      #region Members

      // Sim
      private SPHSimulation m_fluidSim;
      private Vector2 m_gravity;
      private ParticleSystem m_particleSystem;
      private bool m_pause;

      // BoundingVolumes
      private CollisionResolver m_collisionSolver;
      private BoundingVolume m_selectedBoundingVolume;
      private List<Vector2> m_pointList;

      // Blobs
      private BlobRenderer m_blobs;
      private bool m_useBlobs;
      private bool m_drawVel;

      // OpenTK text
      private ITextPrinter m_textPrinter;
      private TextHandle m_textHandleHelp;
      private TextHandle m_textHandleStats;
      private TextureFont m_textFont;

      // Misc.
      private Random m_randGen;
      private bool m_showHelp;
      private int m_mouseWheelLastValue;

      #endregion

      #region Constructor, Main

      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main()
      {
         // The 'using' idiom guarantees proper resource cleanup.
         // We request 30 UpdateFrame events per second, and unlimited
         // RenderFrame events (as fast as the computer can handle).
         using (MainWindow main = new MainWindow())
         {
            main.VSync = VSyncMode.Off;
            main.Run(200.0f);
         }
      }

      /// <summary>Creates a window with the specified title.</summary>
      public MainWindow()
         : base(TEX_SIZE, TEX_SIZE, GraphicsMode.Default, WINDOW_TXT)
      {
         m_randGen   = new Random();
         m_pointList = null;
      }

      #endregion

      #region GameWindow Methods

      /// <summary>Load resources here.</summary>
      /// <param name="e">Not used.</param>
      public override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         //
         // Init. sim
         //
         m_pause = false;
         m_gravity = Constants.GRAVITY * Constants.PARTICLE_MASS;
         m_fluidSim = new SPHSimulation(Constants.CELL_SPACE, Constants.SIM_DOMAIN);
         m_collisionSolver = new CollisionResolver
         {
            BoundingVolumes = new BoundingVolumes
            {
               new OBB
               { 
                  Position    = new Vector2(Constants.SIM_DOMAIN.Width / 3, Constants.SIM_DOMAIN.Height / 2),
                  Extents     = new Vector2(Constants.SIM_DOMAIN.Width / 6 , Constants.SIM_DOMAIN.Height / 30),
               }
            },
            Bounciness     = 0.2f,
            Friction       = 0.01f,
         };
         
         // Init. particle system
         double freq = 30;
         int maxPart = 2000;
         m_particleSystem = new ParticleSystem
         {
            Emitters = new ParticleEmitters 
            { 
               new ParticleEmitter
               {
                  Position       = new Vector2(Constants.SIM_DOMAIN.X, Constants.SIM_DOMAIN.Bottom),
                  VelocityMin    = Constants.PARTICLE_MASS * 0.30f,
                  VelocityMax    = Constants.PARTICLE_MASS * 0.35f,
                  Direction      = new Vector2(0.8f, -0.25f),
                  Distribution   = Constants.SIM_DOMAIN.Width * 0.0001f,
                  Frequency      = freq,
                  ParticleMass   = Constants.PARTICLE_MASS,
               },
            },

            MaxParticles   = maxPart,
            MaxLife        = (int)((double)maxPart / freq / Constants.DELTA_TIME_SEC),
            TestMaxLife    = false,
         };


         //
         // Init blobs
         //
         m_blobs = new BlobRenderer(1.0f, 0.4f, 0.8f, BLOB_RADIUS, TEX_SIZE, Color.LightSkyBlue);
         m_useBlobs = true;
         m_drawVel = false;


         //
         // Init OpenGL
         //
         GL.ClearColor(System.Drawing.Color.Black);
         GL.PointSize(5.0f);
         GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
         GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

         // Init OpenTK text
         m_showHelp = false;
         m_textPrinter = new TextPrinter();
         m_textFont = new TextureFont(new Font(FontFamily.GenericMonospace, 12.0f));
         m_textPrinter.Prepare(
           "Press [F1] to hide / show this text\r\n"
         + "Press [R] to change particle rendering\r\n"
         + "Press [D] to draw velocity direction\r\n"
         + "Press [E] to switch emitter on / off\r\n"
         + "Press [P] to pause\r\n"
         + "Press [Space] to tilt (add random impulse)\r\n" 
         + "Press [Esc] to close the program\r\n" 
         + "\r\n"
         + "Use left mouse button <LMB> to select a box\r\n"
         + "Select a box + <LMB> to move the selected box\r\n"
         + "Hold [Ctrl] + <RMB> to remove a box\r\n"
         + "Hold [Ctrl] + <LMB> to draw a new box (AABB)\r\n"
         + "(Release <LMB> to add the drawn box (AABB))\r\n"
         + "\r\n"
         + "Hold [Alt] + <LMB> to exert a negative force field\r\n"
         + "Hold [Alt] + <RMB> to exert a positive force field\r\n"
         + "\r\n"
         + "Use mousewheel <MW> to change some values\r\n"
         + "(Hold [Shift] to change smaller steps)\r\n"
         + "Select a box + <MW> to rotate the selected box\r\n"
         + "Hold [V] + <MW> to change viscosity\r\n"
         + "Hold [B] + <MW> to change bounciness\r\n"
         + "Hold [F] + <MW> to change friction\r\n"
         + "\r\n"
         + "This program is free software (GPLv3) -> License.txt",
         m_textFont, out m_textHandleHelp);

         // Add Keyboard- and Mouse-Handler
         Keyboard.KeyUp    += new KeyUpEvent(Keyboard_KeyUp);
         Mouse.ButtonDown  += new MouseButtonDownEvent(Mouse_ButtonDown);
         Mouse.ButtonUp    += new MouseButtonUpEvent(Mouse_ButtonUp);

         // Init. misc.
         m_mouseWheelLastValue = Mouse.Wheel;
      }

      /// <summary>
      /// Occurs after after calling GameWindow.Exit, but before destroying the OpenGL context.
      /// Override to unload application resources.
      /// </summary>
      /// <param name="e">Not used.</param>
      public override void OnUnload(EventArgs e)
      {
         if (m_textHandleHelp != null)
         {
            m_textHandleHelp.Dispose();
         }
         if (m_textHandleStats != null)
         {
            m_textHandleStats.Dispose();
         }
         if (m_textFont != null)
         {
            m_textFont.Dispose();
         }
         if (m_blobs != null)
         {
            m_blobs.Dispose();
         }
      }

      /// <summary>
      /// Called when your window is resized. Set your viewport here. It is also
      /// a good place to set up your projection matrix (which probably changes
      /// along when the aspect ratio of your window).
      /// </summary>
      /// <param name="e">Contains information on the new Width and Size of the GameWindow.</param>
      protected override void OnResize(ResizeEventArgs e)
      {
         if (e.Height > 0 && e.Width > 0)
         {
            GL.Viewport(0, 0, Width, Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Glu.Ortho2D(Constants.SIM_DOMAIN.Left, Constants.SIM_DOMAIN.Right, Constants.SIM_DOMAIN.Top, Constants.SIM_DOMAIN.Bottom);
         }

         base.OnResize(e);
      }

      #region GameLoop

      /// <summary>
      /// Called when it is time to setup the next frame. Add you game logic here.
      /// </summary>
      /// <param name="e">Contains timing information for framerate independent logic.</param>
      public override void OnUpdateFrame(UpdateFrameEventArgs e)
      {
         base.OnUpdateFrame(e);

         // Hopefully will be fixed in OpenTK 0.9.1
         //// Set default mouse cursor
         //System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;

         // Keyboard-Handling
         if (Keyboard[Key.Escape])
         {
            Exit();
         }

         // Handle mouse move for obbs
         Mouse_Move();
         Mouse_WheelMove();

         // Solve collisions only for obbs (not particles)
         m_collisionSolver.Solve();

         // Pause
         if (m_pause)
         {
            return;
         }

         // Update particle system
         this.m_particleSystem.Update(Constants.DELTA_TIME_SEC);

         // Interaction handling
         AddInteractionForces();

         // Solve collisions only for particles
         m_collisionSolver.Solve(this.m_particleSystem.Particles);

         // Do simulation
         this.m_fluidSim.Calculate(this.m_particleSystem.Particles, this.m_gravity, Constants.DELTA_TIME_SEC);
      }

      /// <summary>
      /// Called when it is time to render the next frame. Add your rendering code here.
      /// </summary>
      /// <param name="e">Contains timing information.</param>
      public override void OnRenderFrame(RenderFrameEventArgs e)
      {
         base.OnRenderFrame(e);

         GL.Clear(ClearBufferMask.ColorBufferBit);

         //
         // Draw particles as meta-circles (meta-balls / blobs)
         //
         if (m_useBlobs)
         {
            m_blobs.Render(this.m_particleSystem.Particles, m_fluidSim.Domain);
         }
         // Draw particles as points and a velocity line
         else
         {
            GL.Disable(EnableCap.Texture2D);
            GL.Color4(m_blobs.Color);
            foreach (var particle in this.m_particleSystem.Particles)
            {
               GL.Begin(BeginMode.Points);
                  GL.Vertex2(particle.Position.X, particle.Position.Y);
               GL.End();
               if (m_drawVel)
               {
                  Vector2 vel = particle.Position + particle.Velocity * 0.1f;
                  GL.Begin(BeginMode.Lines);
                     GL.Vertex2(particle.Position.X, particle.Position.Y);
                     GL.Vertex2(vel.X, vel.Y);
                  GL.End();
               }
            }
         }


         //
         // Draw Bounding Volumes
         //
         m_collisionSolver.BoundingVolumes.Draw(Color.LightGreen);
         if (m_selectedBoundingVolume != null)
         {
            GL.Color3(Color.GreenYellow);
            m_selectedBoundingVolume.Draw();
         }

         // Draw PointList
         if (m_pointList != null)
         {
            GL.Color4(Color.Red);
            GL.Begin(BeginMode.LineStrip);
            foreach (var point in m_pointList)
            {
               GL.Vertex2(point.X, point.Y);
            }
            GL.End();
         }


         //
         // Draw OpenTK text
         //

         // Text Background
         if (m_showHelp)
         {
            GL.PushAttrib(AttribMask.EnableBit);
            GL.Enable(EnableCap.Blend);
            GL.Color4(0.0f, 0.0f, 0.0f, 0.8f);
            GL.Begin(BeginMode.Quads);
               GL.Vertex2(m_fluidSim.Domain.Left, m_fluidSim.Domain.Bottom);
               GL.Vertex2(m_fluidSim.Domain.Right, m_fluidSim.Domain.Bottom);
               GL.Vertex2(m_fluidSim.Domain.Right, m_fluidSim.Domain.Top);
               GL.Vertex2(m_fluidSim.Domain.Left, m_fluidSim.Domain.Top);
            GL.End();
            GL.PopAttrib();
         }

         GL.Color4(Color.White);
         m_textPrinter.Begin();

         // FPS & Co.
         // Show fps (Frames per second)
         m_textPrinter.Prepare(CreateFPSText(e.Time), m_textFont, out m_textHandleStats);
         m_textPrinter.Draw(m_textHandleStats);

         // Help
         if (m_showHelp)
         {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Translate(0.0f, m_textFont.Height, 0.0f);
            m_textPrinter.Draw(m_textHandleHelp);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
         }
         m_textPrinter.End();


         // Present
         SwapBuffers();
         
         // Check OGL errors
         Utils.TraceGlError();
      }

      #endregion

      /// <summary>
      /// Gets the mouse coordinates in domain space.
      /// </summary>
      /// <returns></returns>
      private Vector2 GetMouseInDomainSpace()
      {
         return new Vector2(((float)Mouse.X / this.Width) * m_fluidSim.Domain.Width + m_fluidSim.Domain.X,
                            (m_fluidSim.Domain.Height - (((float)Mouse.Y / this.Height) * m_fluidSim.Domain.Height) + m_fluidSim.Domain.Y));
      }

      /// <summary>
      /// Creates the FPS text.
      /// </summary>
      /// <param name="elapsedTime">The elapsed time.</param>
      /// <returns></returns>
      private string CreateFPSText(double elapsedTime)
      {
         string txt = String.Format("{0:0000} pa. @ {1:000.00} fps", this.m_particleSystem.Particles.Count, 1.0 / elapsedTime);
         if (Keyboard[Key.V])
         {
            txt = String.Format("{0} - Viscosity = {1}", txt, this.m_fluidSim.Viscosity);
         }
         else if (Keyboard[Key.B])
         {
            txt = String.Format("{0} - Bounciness = {1}", txt, this.m_collisionSolver.Bounciness);
         }
         else if (Keyboard[Key.F])
         {
            txt = String.Format("{0} - Friction = {1}", txt, this.m_collisionSolver.Friction);
         }
         return txt;
      }

      #endregion

      #region Eventhandler

      void Keyboard_KeyUp(KeyboardDevice sender, Key key)
      {
         // Particle style
         if (key == Key.R)
         {
            m_useBlobs = !m_useBlobs;
         }
         // Draw velocity direction
         else if (key == Key.D)
         {
            m_drawVel = !m_drawVel;
         }         
         // Switch Emitters on / off
         else if (key == Key.E)
         {
            m_particleSystem.TestMaxLife = !m_particleSystem.TestMaxLife;
            if (m_particleSystem.HasConsumers)
            {
               foreach (var consumer in m_particleSystem.Consumers)
               {
                  consumer.Enabled = !consumer.Enabled;
               }
            }
         }
         // Show Help ?
         else if (key == Key.F1)
         {
            m_showHelp = !m_showHelp;
         }      
         // Pause
         else if (key == Key.P)
         {
            m_pause = !m_pause;
         }       
      }

      /// <summary>
      /// Adds the interaction (mouse, keyboard) forces to the particles.
      /// </summary>
      private void AddInteractionForces()
      {
         foreach (var particle in this.m_particleSystem.Particles)
         {
            if (Keyboard[Key.Space])
            {
               particle.Force.X += ((float)m_randGen.NextDouble() - 0.5f) * m_fluidSim.Domain.Width * 100.0f * Constants.PARTICLE_MASS;
               particle.Force.Y += ((float)m_randGen.NextDouble() - 0.5f) * m_fluidSim.Domain.Height * 100.0f * Constants.PARTICLE_MASS;
            }
            if ((Keyboard[Key.AltLeft] && Mouse[MouseButton.Left])
            || (Keyboard[Key.AltLeft] && Mouse[MouseButton.Right]))
            {
               Vector2 diff = GetMouseInDomainSpace() - particle.Position;
               float forceRange = (m_fluidSim.Domain.Width + m_fluidSim.Domain.Height) * 0.1f;
               if (diff.LengthSquared < forceRange * forceRange)
               {
                  diff.Normalize();
                  diff.X *= m_fluidSim.Domain.Width * Constants.PARTICLE_MASS * Constants.PARTICLE_MASS;
                  diff.Y *= m_fluidSim.Domain.Height * Constants.PARTICLE_MASS * Constants.PARTICLE_MASS;
                  particle.Force += Mouse[MouseButton.Left] ? -diff : diff;
               }
            }
         }
      }

      /// <summary>
      /// Handles the interaction with the BoundingVolumes and other stuff (Mouse move)
      /// </summary>
      private void Mouse_Move()
      {
         if (Mouse[MouseButton.Left])
         {
            // Record point for PointList
            if (Keyboard[Key.ControlLeft])
            {
               if (m_pointList != null)
               {
                  m_pointList.Add(GetMouseInDomainSpace());
               }
            }
            // Move the selected obb
            else
            {
               if (m_selectedBoundingVolume != null)
               {
                  m_selectedBoundingVolume.Position = GetMouseInDomainSpace();
               }
            }

         }
      }

      /// <summary>
      ///  Handle mouse wheel
      /// </summary>
      private void Mouse_WheelMove()
      {
         int mouseWheelDelta = Mouse.Wheel - m_mouseWheelLastValue;
         m_mouseWheelLastValue = Mouse.Wheel;
         if (mouseWheelDelta != 0)
         {
            float change = mouseWheelDelta * 0.1f;
            // Change smaller values
            if (Keyboard[Key.ShiftLeft])
            {
               change *= 0.01f;
            }
            // increase, decrease viscosity
            if (Keyboard[Key.V])
            {
               if (m_fluidSim.Viscosity + change > 0.0f)
               {
                  m_fluidSim.Viscosity += change;
               }
            }
            else if (Keyboard[Key.B])
            {
               m_collisionSolver.Bounciness += change;
            }
            else if (Keyboard[Key.F])
            {
               m_collisionSolver.Friction += change;
            }
            // Rotate selected OBB
            else
            {
               if (m_selectedBoundingVolume is OBB)
               {
                  double angle = -Functions.DTOR * mouseWheelDelta * 10.0;
                  // Rotate smaller values
                  if (Keyboard[Key.ShiftLeft])
                  {
                     angle *= 0.1;
                  }
                  (m_selectedBoundingVolume as OBB).Rotate(angle);
               }
            }
         }
      }

      void Mouse_ButtonDown(MouseDevice sender, MouseButton button)
      {
         if (button == MouseButton.Left)
         {
            // Reset old sele
            if (m_selectedBoundingVolume != null)
            {
               m_selectedBoundingVolume.IsFixed = false;
               m_selectedBoundingVolume = null;
            }
            // Start recording the PointList
            if (Keyboard[Key.ControlLeft])
            {
               m_pointList = new List<Vector2>();
            }
            // Select the obb
            else
            {
               BoundingVolume bv = new PointVolume { Position = GetMouseInDomainSpace() };
               m_selectedBoundingVolume = m_collisionSolver.BoundingVolumes.FindIntersect(bv);
               if (m_selectedBoundingVolume != null)
               {
                  m_selectedBoundingVolume.IsFixed = true;
               }
            }
         }
         else if (button == MouseButton.Right)
         {
            // Remove OBB at mouse position
            if (Keyboard[Key.ControlLeft])
            {
               BoundingVolume bv = new PointVolume { Position = GetMouseInDomainSpace() };
               m_selectedBoundingVolume = m_collisionSolver.BoundingVolumes.FindIntersect(bv);
               if (m_selectedBoundingVolume != null)
               {
                  m_collisionSolver.BoundingVolumes.Remove(m_selectedBoundingVolume);
                  m_selectedBoundingVolume = null;
               }
            }
         }
      }

      void Mouse_ButtonUp(MouseDevice sender, MouseButton button)
      {
         if (button == MouseButton.Left)
         {
            // Stop recording the PointList and add OBB defined by the PointLists Min, Max (AABB)
            if (Keyboard[Key.ControlLeft])
            {
               if (m_pointList != null)
               {
                  // Find min, max
                  Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
                  Vector2 max = new Vector2(float.MinValue, float.MinValue);
                  foreach (var point in m_pointList)
                  {
                     if (point.X < min.X)
                     {
                        min.X = point.X;
                     }
                     if (point.Y < min.Y)
                     {
                        min.Y = point.Y;
                     }                     
                     if (point.X > max.X)
                     {
                        max.X = point.X;
                     }
                     if (point.Y > max.Y)
                     {
                        max.Y = point.Y;
                     }                   
                  }

                  // Build OBB
                  Vector2 extents = new Vector2((max.X - min.X) * 0.5f, (max.Y - min.Y) * 0.5f);
                  if (extents.X > 0.0f && extents.Y > 0.0f)
                  {
                     m_selectedBoundingVolume = new OBB
                     {
                        Position = min + extents,
                        Extents = extents,
                     };
                     m_collisionSolver.BoundingVolumes.Add(m_selectedBoundingVolume);
                  }
                  m_pointList = null;
               }
            }
         }
      }

      #endregion
   }
}