//
//   Modul:             Fluid physics
//
//   Description:       A particle consumer, which removes particles in a certain radius.
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Particles/ParticleConsumer.cs $
//   Id:                $Id: ParticleConsumer.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
   /// A particle consumer, which removes particles in a certain radius.
   /// </summary>
   public sealed class ParticleConsumer
   {
      #region Members

      private float m_radiusSquared;

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the position.
      /// </summary>
      /// <value>The position.</value>
      public Vector2 Position { get; set; }

      /// <summary>
      /// Gets or sets the radius.
      /// Internally the squared radius is stored, 
      /// so be aware of calling the getter to often (uses Math.Sqrt(m_radiusSquared)).
      /// </summary>
      /// <value>The radius.</value>
      public float Radius
      {
         get { return (float)Math.Sqrt(m_radiusSquared); }
         set
         {
            m_radiusSquared = value * value;
         }
      }

      public bool Enabled { get; set; }

      #endregion

      #region Contructors

      /// <summary>
      /// Initializes a new instance of the <see cref="ParticleConsumer"/> class.
      /// </summary>
      public ParticleConsumer()
      {
         this.Position     = Vector2.Zero;
         this.Radius       = 1.0f;
         this.Enabled      = true;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Consumes the specified particles if they are in in the radius.
      /// </summary>
      /// <param name="particles">The particles.</param>
      public void Consume(FluidParticles particles)
      {
         if (this.Enabled)
         {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
               float distSq = (particles[i].Position - this.Position).LengthSquared;
               if (distSq < m_radiusSquared)
               {
                  particles.RemoveAt(i);
               }
            }
         }
      }

      #endregion
   }
}
