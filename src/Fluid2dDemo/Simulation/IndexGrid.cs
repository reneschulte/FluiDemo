//
//   Modul:             Fluid physics
//
//   Description:       Implementation of a evenly spaced spatial grid, used for fast fluid simulation
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Simulation/IndexGrid.cs $
//   Id:                $Id: IndexGrid.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
   /// Implementation of a evenly spaced spatial grid, used for fast fluid simulation
   /// </summary>
   public sealed class IndexGrid
   {
      #region Members

      private List<int>[,] m_grid;

      #endregion

      #region Properties

      public float CellSpace { get; private set; }

      public RectangleF Domain { get; private set; }

      public int Width { get; private set; }

      public int Height { get; private set; }

      /// <summary>
      /// Returns (3^nDim)-1 -> nDim=2 => 8
      /// </summary>
      public int NeighbourCount 
      {
         get { return 8; }
      }

      public int Count
      {
         get { return m_grid.Length; }
      }

      #endregion

      #region Contructors

      public IndexGrid()
         : this(8, new RectangleF(0, 0, 256, 256))
      {
      }

      public IndexGrid(float cellSpace, RectangleF domain)
         : this(cellSpace, domain, null)
      {
      }

      public IndexGrid(float cellSpace, RectangleF domain, FluidParticles particles)
      {
         this.CellSpace       = cellSpace;
         this.Domain          = domain;
         this.Width           = (int)(this.Domain.Width / this.CellSpace);
         this.Height          = (int)(this.Domain.Height / this.CellSpace);

         this.Refresh(particles);
      }

      #endregion

      #region Methods

      public void Refresh(FluidParticles particles)
      {
         m_grid = new List<int>[this.Width, this.Height];

         if (particles != null)
         {
            for (int i = 0; i < particles.Count; i++)
            {
               int gridIndexX = GetGridIndexX(particles[i]);
               int gridIndexY = GetGridIndexY(particles[i]); 

               // Add particle to list
               if (m_grid[gridIndexX, gridIndexY] == null)
               {
                  m_grid[gridIndexX, gridIndexY] = new List<int>();
               }
               m_grid[gridIndexX, gridIndexY].Add(i);
            }
         }
      }

      private int GetGridIndexX(FluidParticle particle)
      {
         int gridIndexX = (int)(particle.Position.X / this.CellSpace);
         // Clamp X
         if (gridIndexX < 0)
         {
            gridIndexX = 0;
         }
         if (gridIndexX >= this.Width)
         {
            gridIndexX = this.Width - 1;
         }
         return gridIndexX;
      }

      private int GetGridIndexY(FluidParticle particle)
      {
         int gridIndexY = (int)(particle.Position.Y / this.CellSpace);
         // Clamp Y
         if (gridIndexY < 0)
         {
            gridIndexY = 0;
         }
         if (gridIndexY >= this.Height)
         {
            gridIndexY = this.Height - 1;
         }
         return gridIndexY;
      }

      public IEnumerable<int> GetNeighbourIndex(FluidParticle particle)
      {
         for (int xOff = -1; xOff < 2; xOff++)
         {
            for (int yOff = -1; yOff < 2; yOff++)
            {
               // Own index
               //if (xOff == 0 && yOff == 0)
               //{
               //   continue;
               //}
               // Neighbour index
               int x = GetGridIndexX(particle) + xOff;
               int y = GetGridIndexY(particle) + yOff;
               // Clamp
               if (x > -1 && x < this.Width && y > -1 && y < this.Height)
               {
                  List<int> idxList = m_grid[x, y];
                  if (idxList != null)
                  {
                     // Return neighbours index
                     foreach (var idx in idxList)
                     {
                        yield return idx;
                     }
                  }
               }
            }
         }
      }

      #endregion
   }
}
