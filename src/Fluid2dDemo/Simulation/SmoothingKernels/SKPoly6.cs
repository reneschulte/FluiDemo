//
//   Modul:             Fluid physics
//
//   Description:       Implementation of the Poly6 Smoothing-Kernel for SPH-based fluid simulation
//                      based on the paper:
//                      M. Müller et al., "Particle-Based Fluid Simulation for Interactive Applications",
//                      SCA 03, pages 154-159.
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Simulation/SmoothingKernels/SKPoly6.cs $
//   Id:                $Id: SKPoly6.cs 108 2008-04-08 09:04:02Z rene.schulte $
//
//   Copyright (c) 2008 Rene Schulte
//
//

using System;
using OpenTK.Math;

namespace Fluid
{
   /// <summary>
   /// Implementation of the Poly6 Smoothing-Kernel for SPH-based fluid simulation
   /// </summary>
   public sealed class SKPoly6 : SmoothingKernel
   {
      #region Contructors

      public SKPoly6()
         : base()
      {
      }

      public SKPoly6(float kernelSize)
         : base(kernelSize)
      {
      }

      #endregion

      #region Methods

      protected override void CalculateFactor()
      {
         double kernelRad9 = Math.Pow((double)m_kernelSize, 9.0);
         m_factor = (float)(315.0 / (64.0 * Math.PI * kernelRad9));
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
         float diffSq = m_kernelSizeSq - lenSq;
         return m_factor * diffSq * diffSq * diffSq;
      }

      public override Vector2 CalculateGradient(ref Vector2 r)
      {
         float lenSq = r.LengthSquared;
         if (lenSq > m_kernelSizeSq)
         {
            return new Vector2(0.0f, 0.0f);
         }
         if (lenSq < Constants.FLOAT_EPSILON)
         {
            lenSq = Constants.FLOAT_EPSILON;
         }
         float diffSq = m_kernelSizeSq - lenSq;
         float f = -m_factor * 6.0f * diffSq * diffSq;
         return new Vector2(r.X * f, r.Y * f);
      }

      public override float CalculateLaplacian(ref Vector2 r)
      {
         throw new NotImplementedException();
      }

      #endregion
   }
}
