//
//   Modul:             Fluid physics
//
//   Description:       Implementation of a fluid particle   
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Particles/FluidParticle.cs $
//   Id:                $Id: FluidParticle.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
   /// A fluid particle
   /// </summary>
   public sealed class FluidParticle
   {
      #region Members

      public int Life;
      public float Mass;
      public Vector2 Position;
      public Vector2 PositionOld;
      public Vector2 Velocity;
      public Vector2 Force;
      public float Density;
      public float Pressure;

      #endregion

      #region Properties

      public Solver Solver { get; set; }

      public BoundingVolume BoundingVolume { get; private set; }

      #endregion

      #region Contructors

      /// <summary>
      /// Initializes a new instance of the <see cref="FluidParticle"/> class.
      /// </summary>
      public FluidParticle()
      {
         this.Life            = 0;
         this.Mass            = 1.0f;
         this.Position        = Vector2.Zero;
         this.PositionOld     = this.Position;
         this.Velocity        = Vector2.Zero;
         this.Force           = Vector2.Zero;
         this.Density         = Constants.DENSITY_OFFSET;
         // update (integrate) using basic verlet with small drag
         this.Solver          = new Verlet
         {
            Damping     = 0.01f
         };
         this.BoundingVolume  = new PointVolume
         {
            Position    = this.Position,
            Margin   = Constants.CELL_SPACE * 0.25f,
         };
        
         this.UpdatePressure();
      }

      #endregion

      #region Methods

      /// <summary>
      /// Updates the pressure using a modified ideal gas state equation 
      /// (see the paper "Smoothed particles: A new paradigm for animating highly deformable bodies." by Desbrun)
      /// </summary>
      public void UpdatePressure()
      {
         this.Pressure = Constants.GAS_K * (this.Density - Constants.DENSITY_OFFSET);
      }

      /// <summary>
      /// Updates the particle.
      /// </summary>
      /// <param name="dTime">The time step.</param>
      public void Update(float dTime)
      {
         this.Life++;
         // integrate
         this.Solver.Solve(ref this.Position, ref this.PositionOld, ref this.Velocity, this.Force, this.Mass, dTime);
         // update bounding volume
         this.BoundingVolume.Position = this.Position;
      }

      public override int GetHashCode()
      {
         return (int)(this.Position.X * Constants.PRIME_1) ^ (int)(this.Position.Y * Constants.PRIME_2);
      }

      public override bool Equals(object obj)
      {
         if (obj is FluidParticle)
         {
            return (obj as FluidParticle).GetHashCode().Equals(this.GetHashCode());
         }
         return base.Equals(obj);
      }

      #endregion
   }
}
