//
//   Modul:             Fluid physics
//
//   Description:       A particle system, containing a List of ParticleEmitters
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Particles/ParticleSystem.cs $
//   Id:                $Id: ParticleSystem.cs 108 2008-04-08 09:04:02Z rene.schulte $            
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

namespace Fluid
{
   /// <summary>
   /// A particle system, containing a List of ParticleEmitters
   /// </summary>
   public sealed class ParticleSystem
   {
      #region Members

      private bool m_wasMaxReached;

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the particles.
      /// </summary>
      /// <value>The particles.</value>
      public FluidParticles Particles { get; private set; }

      /// <summary>
      /// Gets or sets the emitters.
      /// </summary>
      /// <value>The emitters.</value>
      public ParticleEmitters Emitters { get; set; }

      public bool HasEmitters
      {
         get { return this.Emitters != null && this.Emitters.Count > 0; }
      }

      /// <summary>
      /// Gets or sets the consumers.
      /// </summary>
      /// <value>The consumers.</value>
      public ParticleConsumers Consumers { get; set; }

      public bool HasConsumers
      {
         get { return this.Consumers != null && this.Consumers.Count > 0; }
      }

      /// <summary>
      /// Gets or sets the max life.
      /// </summary>
      /// <value>The max life.</value>
      public int MaxLife { get; set; }

      /// <summary>
      /// Gets or sets the max particles.
      /// </summary>
      /// <value>The max particles.</value>
      public int MaxParticles { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether [do rebirth].
      /// </summary>
      /// <value><c>true</c> if [do rebirth]; otherwise, <c>false</c>.</value>
      public bool DoRebirth { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether [test max life].
      /// </summary>
      /// <value><c>true</c> if [test max life]; otherwise, <c>false</c>.</value>
      public bool TestMaxLife { get; set; }

      #endregion

      #region Contructors

      /// <summary>
      /// Initializes a new instance of the <see cref="ParticleSystem"/> class.
      /// </summary>
      public ParticleSystem()
      {
         this.Emitters        = new ParticleEmitters();
         this.Consumers       = new ParticleConsumers();
         this.MaxLife         = 1024;
         this.MaxParticles    = 4096;
         this.DoRebirth       = true;
         this.TestMaxLife     = true;
         Reset();
      }

      #endregion

      #region Methods

      /// <summary>
      /// Resets this instance.
      /// </summary>
      private void Reset()
      {
         this.Particles       = new FluidParticles(this.MaxParticles);
         this.m_wasMaxReached = false;
      }

      /// <summary>
      /// Updates the particles (remove, emit, ...).
      /// </summary>
      /// <param name="dTime">The delta time.</param>
      public FluidParticles Update(double dTime)
      {
         FluidParticles emitted = null;

         // Consume particles in a certain range
         if (this.HasConsumers)
         {
            foreach (var consumer in this.Consumers)
            {
               consumer.Consume(this.Particles);
            }
         }

         // Remove old particles
         if (this.TestMaxLife)
         {
            for (int i = this.Particles.Count - 1; i >= 0; i--)
            {
               if (this.Particles[i].Life >= this.MaxLife)
               {
                  this.Particles.RemoveAt(i);
               }
            }
         }

         // Check if emit is allowed
         if (m_wasMaxReached && !this.DoRebirth)
         {
            // NOP
         }
         else if (this.Particles.Count < this.MaxParticles)
         {
            if (this.HasEmitters)
            {
               // Emit new particles
               foreach (var emitter in this.Emitters)
               {
                  emitted = emitter.Emit(dTime);
                  this.Particles.AddRange(emitted);
               }
            }
         }
         else
         {
            m_wasMaxReached = true;
         }

         return emitted;
      }

      #endregion
   }
}
