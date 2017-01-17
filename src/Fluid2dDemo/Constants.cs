//
//   Modul:             Fluid physics
//
//   Description:       Some constants
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Constants.cs $
//   Id:                $Id: Constants.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
using System.Drawing;

namespace Fluid
{
   public static class Constants
   {
      #region Physic

      public static readonly Vector2 GRAVITY                            = new Vector2(0.0f, -9.81f);
      public static readonly float DENSITY_OFFSET                       = 100f;
      public static readonly float GAS_K                                = 0.1f;
      public static readonly float VISC0SITY                            = 0.002f;

      public static readonly RectangleF SIM_DOMAIN                      = new RectangleF(0.1f, 0.1f, 6.1f, 6.1f);
      public static readonly float CELL_SPACE                           = (SIM_DOMAIN.Width + SIM_DOMAIN.Height) / 64;
      public static readonly float DELTA_TIME_SEC                       = 0.01f;
      public static readonly float PARTICLE_MASS                        = CELL_SPACE * 20.0f;

      #endregion

      #region Common

      public static readonly int PRIME_1                                = 73856093;
      public static readonly int PRIME_2                                = 19349663;
      public static readonly int PRIME_3                                = 83492791;

      public static readonly float FLOAT_EPSILON                        = 1.192092896e-07f;

      #endregion

      #region OpenGL Extensions

      public static readonly string GL_EXT_framebuffer_object                 = "GL_EXT_framebuffer_object";
      public static readonly string GL_ARB_texture_float                      = "GL_ARB_texture_float";

      #endregion
   }
}
