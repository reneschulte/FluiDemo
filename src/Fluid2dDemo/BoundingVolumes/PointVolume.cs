//
//   Modul:             Fluid physics
//
//   Description:       Point / particle (contains only the position)
//
//   Changed by:        $Author: rene.schulte $
//   Changed on:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/BoundingVolumes/PointVolume.cs $
//   Id:                $Id: PointVolume.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
using OpenTK.Graphics.OpenGL;

namespace Fluid
{
   /// <summary>
   /// Point / particle (contains only the position)
   /// </summary>
   public sealed class PointVolume : BoundingVolume
   {
      #region Properties

      /// <summary>
      /// Gets the axis.
      /// </summary>
      /// <value>The axis.</value>
      public override Vector2[] Axis
      {
         get { return null; }
      }

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="PointVolume"/> class.
      /// </summary>
      public PointVolume()
         : base()
      {
      }

      #endregion

      #region Methods

      /// <summary>
      /// Projects an axis of this bounding volume 
      /// </summary>
      /// <param name="axis"></param>
      /// <param name="min"></param>
      /// <param name="max"></param>
      public override void Project(Vector2 axis, out float min, out float max)
      {
         min = max = Vector2.Dot(this.Position, axis);
      }

      #endregion

      #region Render

      /// <summary>
      /// Draws this instance.
      /// </summary>
      public override void Draw()
      {
         GL.Begin(BeginMode.Points);
            GL.Vertex2(this.Position.X, this.Position.Y);
         GL.End();
      }

      #endregion
   }
}