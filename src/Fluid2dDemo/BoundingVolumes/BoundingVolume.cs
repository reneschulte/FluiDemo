//
//   Modul:             Fluid physics
//
//   Description:       Base class of a bounding volume
//
//   Changed by:        $Author: rene.schulte $
//   Changed on:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/BoundingVolumes/BoundingVolume.cs $
//   Id:                $Id: BoundingVolume.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
   /// Base class of a bounding volume
   /// </summary>
   public abstract class BoundingVolume
   {
      #region Staric Members

      private static int ID_COUNTER;

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the position.
      /// </summary>
      /// <value>The position.</value>
      public virtual Vector2 Position { get; set; }

      /// <summary>
      /// Gets the axis.
      /// </summary>
      /// <value>The axis.</value>
      public abstract Vector2[] Axis { get; }

      /// <summary>
      /// Gets or sets a value indicating whether this instance is fixed.
      /// </summary>
      /// <value><c>true</c> if this instance is fixed; otherwise, <c>false</c>.</value>
      public bool IsFixed { get; set; }

      /// <summary>
      /// Gets or sets the id.
      /// </summary>
      /// <value>The id.</value>
      public int Id { get; private set; }

      /// <summary>
      /// Gets or sets the margin (safety distance).
      /// </summary>
      public float Margin { get; set; }

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes the <see cref="BoundingVolume"/> class.
      /// </summary>
      static BoundingVolume()
      {
         ID_COUNTER = 0;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BoundingVolume"/> class.
      /// </summary>
      public BoundingVolume()
      {
         this.Id        = ++ID_COUNTER;
         this.Position  = Vector2.Zero;
         this.IsFixed   = false;
         this.Margin    = Constants.FLOAT_EPSILON;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Projects an axis of this bounding volume.
      /// </summary>
      /// <param name="axis">The axis.</param>
      /// <param name="min">The min.</param>
      /// <param name="max">The max.</param>
      public abstract void Project(Vector2 axis, out float min, out float max);

      /// <summary>
      /// Tests if this bounding volume intersects the other bounding volume,
      /// using the "Separating Axis Test" (SAT).
      /// </summary>
      /// <param name="other">The bounding volume to test against</param>
      /// <returns>
      /// True, if the both bounding volumes intersect.
      /// </returns>
      public bool Intersects(BoundingVolume other)
      {
         Vector2 penetrationNormal;
         float penetrationLength;
         return this.Intersects(other, out penetrationNormal, out penetrationLength);
      }

      /// <summary>
      /// Tests if this bounding volume intersects the other bounding volume,
      /// using the "Separating Axis Test" (SAT).
      /// </summary>
      /// <param name="other">The bounding volume to test against</param>
      /// <param name="penetrationNormal">The penetration vector (direction of the least penetration).</param>
      /// <param name="penetrationLength">Length of the penetration.</param>
      /// <returns>
      /// True, if the both bounding volumes intersect.
      /// </returns>
      public bool Intersects(BoundingVolume other, out Vector2 penetrationNormal, out float penetrationLength)
      {
         penetrationNormal       = Vector2.Zero;
         penetrationLength       = float.MaxValue;

         // Axis of this
         if (this.Axis != null)
         {
            foreach (var axis in this.Axis)
            {
               if (!FindLeastPenetrating(axis, other, ref penetrationNormal, ref penetrationLength))
               {
                  return false;
               }
            }
         }

         // Axis of other
         if (other.Axis != null)
         {
            foreach (var axis in other.Axis)
            {
               if (!FindLeastPenetrating(axis, other, ref penetrationNormal, ref penetrationLength))
               {
                  return false;
               }
            }
         }

         // Flip penetrationDirection to point away from this
         if (Vector2.Dot(other.Position - this.Position, penetrationNormal) > 0.0f)
         {
            Vector2.Mult(ref penetrationNormal, -1.0f, out penetrationNormal);
         }

         return true;
      }

      /// <summary>
      /// Finds a least penetrating vector.
      /// </summary>
      /// <param name="axis">The axis.</param>
      /// <param name="other">The other.</param>
      /// <param name="penetrationNormal">The penetration normal.</param>
      /// <param name="penetrationLength">Length of the penetration.</param>
      /// <returns>
      /// True, if a least penetrating vector could be found (no Axis separates the bounding volumes).
      /// </returns>
      private bool FindLeastPenetrating(Vector2 axis, BoundingVolume other, ref Vector2 penetrationNormal, ref float penetrationLength)
      {
         float minThis, maxThis, minOther, maxOther;

         // Tests if separating axis exists
         if (TestSeparatingAxis(axis, other, out minThis, out maxThis, out minOther, out maxOther))
         {
            return false;
         }

         // Find least penetrating axis
         float diff = Math.Min(maxOther, maxThis) - Math.Max(minOther, minThis);
         // Store penetration vector
         if (diff < penetrationLength)
         {
            penetrationLength    = diff;
            penetrationNormal    = axis;
         }
         return true;
      }

      /// <summary>
      /// Tests if a separating axis can be found between this bounding volume and the other.
      /// </summary>
      /// <param name="axis">The axis to test against</param>
      /// <param name="other">The bounding volume to test against</param>
      /// <param name="minThis">The min this.</param>
      /// <param name="maxThis">The max this.</param>
      /// <param name="minOther">The min other.</param>
      /// <param name="maxOther">The max other.</param>
      /// <returns>
      /// True, if the Axis separates the bounding volumes
      /// </returns>
      private bool TestSeparatingAxis(Vector2 axis, BoundingVolume other, out float minThis, out float maxThis, out float minOther, out float maxOther)
      {
         this.Project(axis, out minThis, out maxThis);
         other.Project(axis, out minOther, out maxOther);

         // Add safety margin distance
         minThis  -= this.Margin;
         maxThis  += this.Margin;
         minOther -= other.Margin;
         maxOther += other.Margin;

         if (minThis >= maxOther || minOther >= maxThis)
         {
            return true;
         }
         return false;
      }

      #endregion

      #region Render

      /// <summary>
      /// Draws this instance.
      /// </summary>
      public abstract void Draw();

      #endregion
   }
}