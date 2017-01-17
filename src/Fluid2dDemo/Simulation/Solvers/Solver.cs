//
//   Modul:             Fluid physics
//
//   Description:       Base class of a ordinary differential equation solver
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Simulation/Solvers/Solver.cs $
//   Id:                $Id: Solver.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
   /// Base class of a numeric ordinary differential equation solver
   /// </summary>
   public abstract class Solver
   {
      #region Properties

      public float Damping { get; set; }

      #endregion

      #region Contructors

      public Solver()
      {
         this.Damping = 0.0f;
      }

      #endregion

      #region Methods

      public abstract void Solve(ref Vector2 position, ref Vector2 positionOld, ref Vector2 velocity, Vector2 acceleration, float timeStep);

      public virtual void Solve(ref Vector2 position, ref Vector2 positionOld, ref Vector2 velocity, Vector2 force, float mass, float timeStep)
      {
         this.Solve(ref position, ref positionOld, ref velocity, force / mass, timeStep);
      }

      #endregion
   }
}
