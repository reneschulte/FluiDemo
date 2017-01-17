//
//   Modul:             Fluid physics
//
//   Description:       Solves collisions
//
//   Changed by:        $Author: rene.schulte $
//   Changed on:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Simulation/CollisionResolver.cs $
//   Id:                $Id: CollisionResolver.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
   /// Solves collisions
   /// </summary>
   public sealed class CollisionResolver
   {
      #region Properties

      /// <summary>
      /// Gets or sets the bounding volumes.
      /// </summary>
      /// <value>The bounding volumes.</value>
      public BoundingVolumes BoundingVolumes { get; set; }

      /// <summary>
      /// Gets or sets the bounciness.
      /// </summary>
      /// <value>The bounciness.</value>
      public float Bounciness { get; set; }

      /// <summary>
      /// Gets or sets the friction.
      /// </summary>
      /// <value>The friction.</value>
      public float Friction { get; set; }

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="CollisionResolver"/> class.
      /// </summary>
      public CollisionResolver()
      {
         this.BoundingVolumes = new BoundingVolumes();
         this.Bounciness      = 1.0f;
         this.Friction        = 0.0f;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Solves collisions for the bounding volumes among each other associated with this instance.
      /// </summary>
      /// <returns>True, if a collision occured.</returns>
      public bool Solve()
      {
         bool hasCollided = false;
         Vector2 penetration;
         float penLen;
         foreach (var bv1 in this.BoundingVolumes)
         {
            foreach (var bv2 in this.BoundingVolumes)
            {
               if (bv1 != bv2)
               {
                  if (bv1.Intersects(bv2, out penetration, out penLen))
                  {
                     hasCollided = true;
                     Vector2.Mult(ref penetration, penLen, out penetration);
                     if (bv2.IsFixed)
                     {
                        bv1.Position += penetration;
                     }
                     else
                     {
                        bv2.Position -= penetration;
                     }
                  }
               }
            }
         }
         return hasCollided;
      }

      /// <summary>
      /// Solves collisions only for the particles and the bounding volumes associated with this instance.
      /// </summary>
      /// <param name="particles">The particles.</param>
      /// <returns>True, if a collision occured.</returns>
      public bool Solve(FluidParticles particles)
      {
         bool hasCollided = false;
         Vector2 penetration, penNormal, v, vn, vt;
         float penLen, dp;
         foreach (var bv in this.BoundingVolumes)
         {
            foreach (var particle in particles)
            {
               if (bv.Intersects(particle.BoundingVolume, out penNormal, out penLen))
               {
                  hasCollided = true;
                  Vector2.Mult(ref penNormal, penLen, out penetration);
                  if (particle.BoundingVolume.IsFixed)
                  {
                     bv.Position                      += penetration;
                  }
                  else
                  {
                     particle.BoundingVolume.Position -= penetration;


                     // Calc new velocity using elastic collision with friction
                     // -> Split oldVelocity in normal and tangential component, revert normal component and add it afterwards
                     // v = pos - oldPos;
                     //vn = n * Vector2.Dot(v, n) * -Bounciness;
                     //vt = t * Vector2.Dot(v, t) * (1.0f - Friction);
                     //v = vn + vt;
                     //oldPos = pos - v;

                     Vector2.Sub(ref particle.Position, ref particle.PositionOld, out v);
                     Vector2 tangent = penNormal.PerpendicularRight;
                     dp = Vector2.Dot(v, penNormal);
                     Vector2.Mult(ref penNormal, dp * -this.Bounciness, out vn);
                     dp = Vector2.Dot(v, tangent);
                     Vector2.Mult(ref tangent, dp * (1.0f - this.Friction), out vt);
                     Vector2.Add(ref vn, ref vt, out v);
                     particle.Position -= penetration;
                     Vector2.Sub(ref particle.Position, ref v, out particle.PositionOld);
                  }
               }
            }
         }
         return hasCollided;
      }

      #endregion
   }
}