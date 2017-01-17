//
//   Modul:             Fluid physics
//
//   Description:       Ordinary differential equation solver using a basic verlet integration
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Simulation/Solvers/Verlet.cs $
//   Id:                $Id: Verlet.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
   /// Ordinary differential equation solver using a basic verlet integration
   /// </summary>
   public sealed class Verlet : Solver
   {
      #region Contructors

      public Verlet()
         : base()
      {
      }

      #endregion

      #region Methods

      public override void Solve(ref Vector2 position, ref Vector2 positionOld, ref Vector2 velocity, Vector2 acceleration, float timeStep)
      {
         Vector2 t;
         Vector2 oldPos = position;
         // Position = Position + (1.0f - Damping) * (Position - PositionOld) + dt * dt * a;
         Vector2.Mult(ref acceleration, timeStep * timeStep, out acceleration);
         Vector2.Sub(ref position, ref positionOld, out t);
         Vector2.Mult(ref t, 1.0f - Damping, out t);
         Vector2.Add(ref t, ref acceleration, out t);
         Vector2.Add(ref position, ref t, out position);
         positionOld = oldPos;

         // calculate velocity
         // Velocity = (Position - PositionOld) / dt;
         Vector2.Sub(ref position, ref positionOld, out t);
         Vector2.Div(ref t, timeStep, out velocity);
      }

      #endregion
   }
}
