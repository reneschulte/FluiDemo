//
//   Modul:             Fluid physics
//
//   Description:       Implementation of a list of fluid particles
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Particles/FluidParticles.cs $
//   Id:                $Id: FluidParticles.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
using System.Collections.Generic;
using OpenTK.Math;

namespace Fluid
{
   /// <summary>
   /// A list of fluid particles
   /// </summary>
   public sealed class FluidParticles : List<FluidParticle>
   {
      #region Contructors

      /// <summary>
      /// Initializes a new instance of the <see cref="FluidParticles"/> class.
      /// </summary>
      public FluidParticles()
         : base()
      {
      }
      /// <summary>
      /// Initializes a new instance of the <see cref="FluidParticles"/> class.
      /// </summary>
      /// <param name="capacity">The capacity.</param>
      public FluidParticles(int capacity)
         : base(capacity)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="FluidParticles"/> class.
      /// </summary>
      /// <param name="source">The source.</param>
      public FluidParticles(IEnumerable<FluidParticle> source)
         : base(source)
      {
      }

      #endregion

      #region Methods

      /// <summary>
      /// Create particles evenly spaced on ground of the boundary.
      /// </summary>
      public static FluidParticles Create(int nParticles, float cellSpace, RectangleF domain, float particleMass)
      {
         FluidParticles particles = new FluidParticles(nParticles);

         // Init. Particle positions
         float x0 = domain.X + cellSpace;
         float x = x0;
         float y = domain.Y;
         for (int i = 0; i < nParticles; i++)
         {
            if (x == x0)
            {
               y += cellSpace;
            }
            Vector2 pos = new Vector2(x, y);
            particles.Add(new FluidParticle 
            { 
               Position       = pos,
               PositionOld    = pos,
               Mass           = particleMass,
            });
            x = x + cellSpace < domain.Width ? x + cellSpace : x0;
         }

         return particles;
      }

      #endregion
   }
}