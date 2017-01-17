//
//   Modul:             Fluid physics
//
//   Description:       Implementation of the Viscosity Smoothing-Kernel for SPH-based fluid simulation
//                      based on the paper:
//                      M. Müller et al., "Particle-Based Fluid Simulation for Interactive Applications",
//                      SCA 03, pages 154-159.
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Simulation/SmoothingKernels/SKViscosity.cs $
//   Id:                $Id: SKViscosity.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
   /// Implementation of the Viscosity Smoothing-Kernel for SPH-based fluid simulation
   /// </summary>
   public sealed class SKViscosity : SmoothingKernel
   {
      #region Contructors

      public SKViscosity()
         : base()
      {
      }

      public SKViscosity(float kernelSize)
         : base(kernelSize)
      {
      }

      #endregion

      #region Methods

      protected override void CalculateFactor()
      {
         m_factor = (float)(15.0 / (2.0f * Math.PI * m_kernelSize3));
      }

      public override float Calculate(ref Vector2 r)
      {
         float lenSq = r.LengthSquared;
         if (lenSq > m_kernelSizeSq)
         {
            return 0.0f;
         }
         if (lenSq < Constants.FLOAT_EPSILON)
         {
            lenSq = Constants.FLOAT_EPSILON;
         }
         float len = (float)Math.Sqrt((double)lenSq);
         float len3 = len * len * len;
         return m_factor * (((-len3 / (2.0f * m_kernelSize3)) + (lenSq / m_kernelSizeSq) + (m_kernelSize / (2.0f * len))) - 1.0f);
      }

      public override Vector2 CalculateGradient(ref Vector2 r)
      {
         throw new NotImplementedException();
      }

      public override float CalculateLaplacian(ref Vector2 r)
      {
         float lenSq = r.LengthSquared;
         if (lenSq > m_kernelSizeSq)
         {
            return 0.0f;
         }
         if (lenSq < Constants.FLOAT_EPSILON)
         {
            lenSq = Constants.FLOAT_EPSILON;
         }
         float len = (float)Math.Sqrt((double)lenSq);
         return m_factor * (6.0f / m_kernelSize3) * (m_kernelSize - len);
      }

      #endregion
   }
}
