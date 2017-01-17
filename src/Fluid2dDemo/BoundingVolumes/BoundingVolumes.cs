//
//   Modul:             Fluid physics
//
//   Description:       A List of bounding volumes
//
//   Changed by:        $Author: rene.schulte $
//   Changed on:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/BoundingVolumes/BoundingVolumes.cs $
//   Id:                $Id: BoundingVolumes.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
using OpenTK.Graphics.OpenGL;

namespace Fluid
{
   /// <summary>
   /// A List of bounding volumes
   /// </summary>
   public sealed class BoundingVolumes : List<BoundingVolume>
   {
      #region Constructors

      public BoundingVolumes()
         : base()
      {
      }

      public BoundingVolumes(int capacity)
         : base(capacity)
      {
      }

      public BoundingVolumes(IEnumerable<BoundingVolume> other)
         : base(other)
      {
      }

      #endregion

      #region Methods

      public BoundingVolume FindIntersect(BoundingVolume boundingVolume)
      {
         foreach (var bv in this)
         {
            if (bv.Intersects(boundingVolume))
            {
               return bv;
            }
         }
         return null;
      }

      #region Render

      public void Draw()
      {
         foreach (var bv in this)
         {
            bv.Draw();
         }
      }

      public void Draw(Color color)
      {
         GL.Color3(color);
         this.Draw();
      }

      #endregion

      #endregion
   }
}