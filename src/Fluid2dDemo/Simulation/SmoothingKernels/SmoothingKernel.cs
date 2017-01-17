//
//   Modul:             Fluid physics
//
//   Description:       Abstract base class of a Smoothing-Kernel for SPH-based fluid simulation
//                      based on the paper:
//                      M. Müller et al., "Particle-Based Fluid Simulation for Interactive Applications",
//                      SCA 03, pages 154-159.
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Simulation/SmoothingKernels/SmoothingKernel.cs $
//   Id:                $Id: SmoothingKernel.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
   /// Abstract base class of a Smoothing-Kernel for SPH-based fluid simulation
   /// </summary>
   public abstract class SmoothingKernel
   {
      #region Members

      protected float m_factor;
      protected float m_kernelSize;
      protected float m_kernelSizeSq;
      protected float m_kernelSize3;

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the size of the kernel.
      /// </summary>
      /// <value>The size of the kernel.</value>
      public float KernelSize
      {
         get { return m_kernelSize; }
         set
         {
            m_kernelSize      = value;
            m_kernelSizeSq    = m_kernelSize * m_kernelSize;
            m_kernelSize3     = m_kernelSize * m_kernelSize * m_kernelSize;
            CalculateFactor();
         }
      }
      #endregion

      #region Contructors

      /// <summary>
      /// Initializes a new instance of the <see cref="SmoothingKernel"/> class.
      /// </summary>
      public SmoothingKernel()
         : this(1.0f)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="SmoothingKernel"/> class.
      /// </summary>
      /// <param name="kernelSize">Size of the kernel.</param>
      public SmoothingKernel(float kernelSize)
      {
         this.m_factor     = 1.0f;
         this.KernelSize   = kernelSize;
      }

      #endregion

      #region Methods

      protected abstract void CalculateFactor();

      public abstract float Calculate(ref Vector2 r);

      public abstract Vector2 CalculateGradient(ref Vector2 r);

      public abstract float CalculateLaplacian(ref Vector2 r);


      public float Calculate(Vector2 r)
      {
         return Calculate(ref r);
      }

      public Vector2 CalculateGradient(Vector2 r)
      {
         return CalculateGradient(ref r);
      }

      public float CalculateLaplacian(Vector2 r)
      {
         return CalculateLaplacian(ref r);
      }

      #endregion
   }
}
