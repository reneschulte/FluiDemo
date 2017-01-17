//
//   Modul:             Fluid physics
//
//   Description:       A List of ParticleEmitters
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Particles/ParticleEmitters.cs $
//   Id:                $Id: ParticleEmitters.cs 108 2008-04-08 09:04:02Z rene.schulte $            
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
   /// A List of ParticleEmitters
   /// </summary>
   public sealed class ParticleEmitters : List<ParticleEmitter>
   {
      #region Contructors

      /// <summary>
      /// Initializes a new instance of the <see cref="ParticleEmitters"/> class.
      /// </summary>
      public ParticleEmitters()
         : base()
      {
      }
      /// <summary>
      /// Initializes a new instance of the <see cref="ParticleEmitters"/> class.
      /// </summary>
      /// <param name="capacity">The capacity.</param>
      public ParticleEmitters(int capacity)
         : base(capacity)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ParticleEmitters"/> class.
      /// </summary>
      /// <param name="source">The source.</param>
      public ParticleEmitters(IEnumerable<ParticleEmitter> source)
         : base(source)
      {
      }

      #endregion
   }
}
