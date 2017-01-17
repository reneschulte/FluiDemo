//
//   Modul:             Fluid physics
//
//   Description:       A particle emitter
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Particles/ParticleEmitter.cs $
//   Id:                $Id: ParticleEmitter.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
using OpenTK.Math;

namespace Fluid
{
   /// <summary>
   /// A particle emitter
   /// </summary>
   public sealed class ParticleEmitter
   {
      #region Members

      private Random m_randGen;
      private Vector2 m_direction;
      private double m_time;

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the position.
      /// </summary>
      /// <value>The position.</value>
      public Vector2 Position { get; set; }

      /// <summary>
      /// Gets or sets the direction.
      /// </summary>
      /// <value>The direction.</value>
      public Vector2 Direction 
      {
         get { return m_direction; }
         set
         {
            m_direction = value;
            m_direction.Normalize();
         }
      }

      /// <summary>
      /// Gets or sets the distribution along the direction.
      /// </summary>
      /// <value>The distribution.</value>
      public float Distribution { get; set; }

      /// <summary>
      /// Gets or sets the minimum initial velocity of the particles.
      /// </summary>
      /// <value>The minimum velocity.</value>
      public float VelocityMin { get; set; }

      /// <summary>
      /// Gets or sets the maximum initial velocity of the particles.
      /// </summary>
      /// <value>The maximum velocity.</value>
      public float VelocityMax { get; set; }

      /// <summary>
      /// Gets or sets the frequency in particles per second.
      /// </summary>
      /// <value>The frequency.</value>
      public double Frequency { get; set; }

      /// <summary>
      /// Gets or sets the particle mass.
      /// </summary>
      /// <value>The particle mass.</value>
      public float ParticleMass { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether this <see cref="ParticleEmitter"/> is enabled.
      /// </summary>
      /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
      public bool Enabled { get; set; }

      #endregion

      #region Contructors

      /// <summary>
      /// Initializes a new instance of the <see cref="ParticleEmitter"/> class.
      /// </summary>
      /// <param name="position">The position.</param>
      /// <param name="direction">The direction.</param>
      /// <param name="distribution">The distribution along the direction.</param>
      /// <param name="frequency">The particles per second which will be emitted.</param>
      public ParticleEmitter()
      {
         this.m_randGen       = new Random();
         this.m_time          = 0.0;
         this.Position        = Vector2.Zero;
         this.VelocityMin     = 0.0f;
         this.VelocityMax     = this.VelocityMin;
         this.Direction       = Vector2.UnitY;
         this.Distribution    = 1.0f;
         this.Frequency       = 128.0f;
         this.ParticleMass    = 1.0f;
         this.Enabled         = true;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Emits particles.
      /// </summary>
      /// <param name="dTime">The delta time.</param>
      /// <returns></returns>
      public FluidParticles Emit(double dTime)
      {
         FluidParticles particles = new FluidParticles();
         if (this.Enabled)
         {
            // Calc particle count based on frequency
            m_time += dTime;
            int nParts = (int)(this.Frequency * m_time);
            if (nParts > 0)
            {
               // Create Particles
               for (int i = 0; i < nParts; i++)
               {
                  // Calc velocity based on the distribution along the normalized direction
                  float dist = (float)m_randGen.NextDouble() * this.Distribution - this.Distribution * 0.5f;
                  Vector2 normal = this.Direction.PerpendicularRight;
                  Vector2.Mult(ref normal, dist, out normal);
                  Vector2 vel = this.Direction + normal;
                  vel.Normalize();
                  float velLen = (float)m_randGen.NextDouble() * (this.VelocityMax - this.VelocityMin) + this.VelocityMin;
                  Vector2.Mult(ref vel, velLen, out vel);

                  // Calc Oldpos (for right velocity) using simple euler
                  // oldPos = this.Position - vel * m_time;
                  Vector2 oldPos = this.Position - vel * (float)m_time;

                  particles.Add(new FluidParticle
                  {
                     Position    = this.Position,
                     PositionOld = oldPos,
                     Velocity    = vel,
                     Mass        = this.ParticleMass
                  });
               }
               // Reset time
               m_time = 0.0;
            }
         }
         return particles;
      }

      #endregion
   }
}
