//
//   Modul:             Fluid physics
//
//   Description:       Oriented bounded box
//
//   Changed by:        $Author: rene.schulte $
//   Changed on:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/BoundingVolumes/OBB.cs $
//   Id:                $Id: OBB.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
   /// Oriented bounded box
   /// </summary>
   public sealed class OBB : BoundingVolume
   {
      #region Members

      private Vector2[] m_Axis;

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the extents (half width of the box).
      /// </summary>
      /// <value>The extents.</value>
      public Vector2 Extents { get; set; }

      /// <summary>
      /// Gets the axis.
      /// </summary>
      /// <value>The axis.</value>
      public override Vector2[] Axis
      {
         get { return m_Axis; }
      }

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="OBB"/> class.
      /// </summary>
      public OBB()
         : base()
      {
         this.Extents      = new Vector2(1.0f, 1.0f);
         this.m_Axis       = new Vector2[]
         {
            Vector2.UnitX,
            Vector2.UnitY
         };
      }

      #endregion

      #region Methods

      /// <summary>
      /// Projects an axis of this bounding volume
      /// </summary>
      /// <param name="axis">The axis.</param>
      /// <param name="min">The min.</param>
      /// <param name="max">The max.</param>
      public override void Project(Vector2 axis, out float min, out float max)
      {
         float pos = Vector2.Dot(this.Position, axis);
         float radius = Math.Abs(Vector2.Dot(axis, this.Axis[0])) * this.Extents.X 
                      + Math.Abs(Vector2.Dot(axis, this.Axis[1])) * this.Extents.Y;
         min = pos - radius;
         max = pos + radius;
      }

      /// <summary>
      /// Rotates the obb by the specified angle.
      /// </summary>
      /// <param name="angle">The angle in radians.</param>
      public void Rotate(double angle)
      {         
         Axis[0] = RotateAxis(angle, Axis[0]);
         Axis[1] = RotateAxis(angle, Axis[1]);
      }

      /// <summary>
      /// Rotates the obb axis.
      /// </summary>
      /// <param name="angle">The angle in radians.</param>
      /// <param name="axis">The axis.</param>
      private Vector2 RotateAxis(double angle, Vector2 axis)
      {
         return new Vector2(  axis.X * (float)Math.Cos(angle) + axis.Y * (float)Math.Sin(angle),
                              axis.Y * (float)Math.Cos(angle) - axis.X * (float)Math.Sin(angle));
      }

      #endregion

      #region Render

      /// <summary>
      /// Draws this instance.
      /// </summary>
      public override void Draw()
      {
         Vector2 exX = new Vector2(this.Axis[0] * this.Extents.X);
         Vector2 exY = new Vector2(this.Axis[1] * this.Extents.Y);
         GL.Begin(BeginMode.Quads);
            GL.Vertex2(this.Position.X + exX.X + exY.X, this.Position.Y + exX.Y + exY.Y);
            GL.Vertex2(this.Position.X - exX.X + exY.X, this.Position.Y - exX.Y + exY.Y);
            GL.Vertex2(this.Position.X - exX.X - exY.X, this.Position.Y - exX.Y - exY.Y);
            GL.Vertex2(this.Position.X + exX.X - exY.X, this.Position.Y + exX.Y - exY.Y);
         GL.End();
      }

      #endregion
   }
}