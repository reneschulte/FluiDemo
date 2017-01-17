//
//   Modul:             Fluid physics
//
//   Description:       Implementation of a Smoothed Particle Hydrodynamics (SPH) based simulation
//                      based on the paper:
//                      M. Müller et al., "Particle-Based Fluid Simulation for Interactive Applications",
//                      SCA 03, pages 154-159.
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Simulation/SPHSimulation.cs $
//   Id:                $Id: SPHSimulation.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
using System.Collections.Generic;
using System.Drawing;

using OpenTK.Math;

namespace Fluid
{
   /// <summary>
   /// Implementation of a SPH-based fluid simulation
   /// </summary>
   public sealed class SPHSimulation
   {
      #region Members

      private IndexGrid m_grid;

      #endregion

      #region Properties

      public float CellSpace { get; private set; }

      public RectangleF Domain { get; set; }

      public SmoothingKernel SKGeneral { get; set; }

      public SmoothingKernel SKPressure { get; set; }

      public SmoothingKernel SKViscosity { get; set; }

      public float Viscosity { get; set; }

      #endregion

      #region Contructors

      public SPHSimulation()
         : this(8, new RectangleF(0, 0, 256, 256))
      {
      }

      public SPHSimulation(float cellSpace, RectangleF domain)
      {
         this.CellSpace    = cellSpace;
         this.Domain       = domain;
         this.Viscosity    = Constants.VISC0SITY;
         this.m_grid       = new IndexGrid(cellSpace, domain);
         this.SKGeneral    = new SKPoly6(cellSpace);
         this.SKPressure   = new SKSpiky(cellSpace);
         this.SKViscosity  = new SKViscosity(cellSpace);
      }

      #endregion

      #region Methods

      /// <summary>
      /// Simulates the specified particles.
      /// </summary>
      /// <param name="particles">The particles.</param>
      /// <param name="globalForce">The global force.</param>
      /// <param name="dTime">The time step.</param>
      public void Calculate(FluidParticles particles, Vector2 globalForce, float dTime)
      {
         m_grid.Refresh(particles);
         CalculatePressureAndDensities(particles, m_grid);
         CalculateForces(particles, m_grid, globalForce);
         UpdateParticles(particles, dTime);
         CheckParticleDistance(particles, m_grid);
      }

      /// <summary>
      /// Calculates the pressure and densities.
      /// </summary>
      /// <param name="particles">The particles.</param>
      /// <param name="grid">The grid.</param>
      private void CalculatePressureAndDensities(FluidParticles particles, IndexGrid grid)
      {
         Vector2 dist;
         foreach (var particle in particles)
         {
            particle.Density = 0.0f;
            foreach (var nIdx in grid.GetNeighbourIndex(particle))
            {
               Vector2.Sub(ref particle.Position, ref particles[nIdx].Position, out dist);
               particle.Density += particle.Mass * this.SKGeneral.Calculate(ref dist);
            }
            particle.UpdatePressure();
         }
      }

      /// <summary>
      /// Calculates the pressure and viscosity forces.
      /// </summary>
      /// <param name="particles">The particles.</param>
      /// <param name="grid">The grid.</param>
      /// <param name="globalForce">The global force.</param>
      private void CalculateForces(FluidParticles particles, IndexGrid grid, Vector2 globalForce)
      {
         Vector2 f, dist;
         float scalar;
         for (int i = 0; i < particles.Count; i++)
         {
            // Add global force to every particle
            particles[i].Force += globalForce;

            foreach (var nIdx in grid.GetNeighbourIndex(particles[i]))
            {
               // Prevent double tests
               if (nIdx < i)
               {
                  if (particles[nIdx].Density > Constants.FLOAT_EPSILON)
                  {
                     Vector2.Sub(ref particles[i].Position, ref particles[nIdx].Position, out dist);

                     // pressure
                     // f = particles[nIdx].Mass * ((particles[i].Pressure + particles[nIdx].Pressure) / (2.0f * particles[nIdx].Density)) * WSpikyGrad(ref dist);
                     scalar   = particles[nIdx].Mass * (particles[i].Pressure + particles[nIdx].Pressure) / (2.0f * particles[nIdx].Density);
                     f        = this.SKPressure.CalculateGradient(ref dist);
                     Vector2.Mult(ref f, scalar, out f);
                     particles[i].Force      -= f;
                     particles[nIdx].Force   += f;

                     // viscosity
                     // f = particles[nIdx].Mass * ((particles[nIdx].Velocity - particles[i].Velocity) / particles[nIdx].Density) * WViscosityLap(ref dist) * Constants.VISC0SITY;
                     scalar   = particles[nIdx].Mass * this.SKViscosity.CalculateLaplacian(ref dist) * this.Viscosity * 1 / particles[nIdx].Density;
                     f        = particles[nIdx].Velocity - particles[i].Velocity;
                     Vector2.Mult(ref f, scalar, out f);
                     particles[i].Force      += f;
                     particles[nIdx].Force   -= f;
                  }
               }
            }
         }
      }

      /// <summary>
      /// Updates the particles posotions using integration and clips them to the domain space.
      /// </summary>
      /// <param name="particles">The particles.</param>
      /// <param name="dTime">The time step.</param>
      private void UpdateParticles(FluidParticles particles, float dTime)
      {
         float r = this.Domain.Right;
         float l = this.Domain.X;
         // Rectangle contains coordinates inverse on y
         float t = this.Domain.Bottom;
         float b = this.Domain.Y;

         foreach (var particle in particles)
         {
            // Clip positions to domain space
            if (particle.Position.X < l)
            {
               particle.Position.X = l + Constants.FLOAT_EPSILON;
            }
            else if (particle.Position.X > r)
            {
               particle.Position.X = r - Constants.FLOAT_EPSILON;
            }
            if (particle.Position.Y < b)
            {
               particle.Position.Y = b + Constants.FLOAT_EPSILON;
            }
            else if (particle.Position.Y > t)
            {
               particle.Position.Y = t - Constants.FLOAT_EPSILON;
            }

            // Update velocity + position using forces
            particle.Update(dTime);
            // Reset force
            particle.Force = Vector2.Zero;
         }
      }

      /// <summary>
      /// Checks the distance between the particles and corrects it, if they are to near.
      /// </summary>
      /// <param name="particles">The particles.</param>
      /// <param name="grid">The grid.</param>
      private void CheckParticleDistance(FluidParticles particles, IndexGrid grid)
      {
         float minDist = 0.5f * CellSpace;
         float minDistSq = minDist * minDist;
         Vector2 dist;
         for (int i = 0; i < particles.Count; i++)
         {
            foreach (var nIdx in grid.GetNeighbourIndex(particles[i]))
            {
               Vector2.Sub(ref particles[nIdx].Position, ref particles[i].Position, out dist);
               float distLenSq = dist.LengthSquared;
               if (distLenSq < minDistSq)
               {
                  if (distLenSq > Constants.FLOAT_EPSILON)
                  {
                     float distLen = (float)Math.Sqrt((double)distLenSq);
                     Vector2.Mult(ref dist, 0.5f * (distLen - minDist) / distLen, out dist);
                     Vector2.Sub(ref particles[nIdx].Position, ref dist, out particles[nIdx].Position);
                     Vector2.Sub(ref particles[nIdx].PositionOld, ref dist, out particles[nIdx].PositionOld);
                     Vector2.Add(ref particles[i].Position, ref dist, out particles[i].Position);
                     Vector2.Add(ref particles[i].PositionOld, ref dist, out particles[i].PositionOld);
                  }
                  else
                  {
                     float diff = 0.5f * minDist;
                     particles[nIdx].Position.Y       -= diff;
                     particles[nIdx].PositionOld.Y    -= diff;
                     particles[i].Position.Y          += diff;
                     particles[i].PositionOld.Y       += diff;
                  }
               }
            }
         }
      }

      #endregion
   }
}
