//
//   Modul:             Fluid physics
//
//   Description:       Some static utility functions
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 11:04:02 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 108 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Utils.cs $
//   Id:                $Id: Utils.cs 108 2008-04-08 09:04:02Z rene.schulte $
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
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.OpenGL.Enums;
using OpenTK.Math;

namespace Fluid
{
   public static class Utils
   {
      #region Methods

      /// <summary>
      /// Generates a texture and sets some texparameters.
      /// </summary>
      /// <returns>The generated texture.</returns>
      public static int GenerateTexture(TextureWrapMode wrapMode)
      {
         int texture;
         GL.GenTextures(1, out texture);
         GL.BindTexture(TextureTarget.Texture2D, texture);
         GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
         GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
         GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
         GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);
         return texture;
      }

      /// <summary>
      /// Generates a texture and sets some texparameters.
      /// </summary>
      /// <returns>The generated texture.</returns>
      public static int GenerateTexture()
      {
         return GenerateTexture(TextureWrapMode.Repeat);
      }

      public static int GenerateFBO(int texture)
      {
         int fbo = 0;
         if (GL.SupportsExtension(Constants.GL_EXT_framebuffer_object))
         {
            GL.Ext.GenFramebuffers(1, out fbo);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fbo);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, texture, 0);
            FramebufferErrorCode fboErrorCode = GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt);
            if (fboErrorCode != FramebufferErrorCode.FramebufferCompleteExt)
            {
               throw new InvalidOperationException("FBO could not be initialized! FramebufferErrorCode = " + fboErrorCode.ToString());
            }
         }
         return fbo;
      }


      /// <summary>
      /// Traces if a OpenGl error occured.
      /// </summary>
      public static void TraceGlError()
      {
         ErrorCode errCode = GL.GetError();
         if (errCode != ErrorCode.NoError)
         {
            System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame(1, true);
            string msg = String.Format("{0}: OGL ERROR - Code: {1} - Message: {2} @ {3}, Line {4}", DateTime.Now.ToLongTimeString(), errCode, Glu.ErrorString(errCode), stackFrame.GetFileName(), stackFrame.GetFileLineNumber());
            System.Diagnostics.Trace.WriteLine(msg);
#if DEBUG
            throw new InvalidOperationException(msg);
#endif
         }
      }

      #endregion
   }
}
