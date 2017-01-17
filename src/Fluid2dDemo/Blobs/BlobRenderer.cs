//
//   Modul:             Fluid physics
//
//   Description:       Renders Meta-Circles (Meta-Balls / Blobs in 2D)
//
//   Changed by:        $Author: rene.schulte $
//   Changed at:        $Date: 2008-04-08 15:11:29 +0200 (Di, 08 Apr 2008) $
//   Changed in:        $Revision: 110 $
//   Project:           $URL: file:///U:/Data/Development/SVN/Fluid2dDemo/trunk/Fluid2dDemo/Blobs/BlobRenderer.cs $
//   Id:                $Id: BlobRenderer.cs 110 2008-04-08 13:11:29Z rene.schulte $
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
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using OpenTK.Math;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.OpenGL.Enums;

namespace Fluid
{
   /// <summary>
   /// Renders Meta-Circles (Blobs)
   /// </summary>
   public sealed class BlobRenderer : IDisposable
   {
      #region Members

      private int m_texAttentuation;
      private int m_texRender;
      private int m_fboRender;
      private int m_texSize;
      private float m_radiusSquared;

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the energy.
      /// </summary>
      /// <value>The energy.</value>
      public float Energy { get; set; }

      /// <summary>
      /// Gets or sets the falloff for the gaussian energy function.
      /// </summary>
      /// <value>The falloff.</value>
      public float Falloff { get; set; }

      /// <summary>
      /// Gets or sets the energy threshold.
      /// </summary>
      /// <value>The energy threshold.</value>
      public float EnergyThreshold { get; set; }

      /// <summary>
      /// Gets or sets the radius.
      /// Internally the squared radius is stored, 
      /// so be aware of calling the getter to often (uses Math.Sqrt(m_radiusSquared)).
      /// </summary>
      /// <value>The radius.</value>
      public int Radius 
      {
         get { return (int)Math.Sqrt(m_radiusSquared); }
         set
         {
            m_radiusSquared = value * value;
         }
      }

      /// <summary>
      /// Gets or sets the color.
      /// </summary>
      /// <value>The color.</value>
      public Color Color { get; set; }

      #endregion
      
      #region Contructors

      /// <summary>
      /// Initializes a new instance of the <see cref="BlobRenderer"/> class.
      /// </summary>
      public BlobRenderer()
         : this(0.9f, 1.0f, 0.9f, 4, 512, Color.Green)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BlobRenderer"/> class.
      /// </summary>
      /// <param name="energy">The energy.</param>
      /// <param name="falloff">The falloff for the gaussian energy function.</param>
      /// <param name="energyThreshold">The energy threshold.</param>
      /// <param name="radius">The radius.</param>
      /// <param name="texSize">Size of the texture.</param>
      /// <param name="color">The color.</param>
      public BlobRenderer(float energy, float falloff, float energyThreshold, int radius, int texSize, Color color)
      {
         this.m_texAttentuation     = 0;
         this.m_texRender           = 0;
         this.m_fboRender           = 0;
         this.Energy                = energy;
         this.Falloff               = falloff;
         this.EnergyThreshold       = energyThreshold;
         this.Radius                = radius;
         this.m_texSize             = texSize;
         this.Color                 = color;
         this.m_texAttentuation     = 0;
         this.m_texRender           = 0;
         Generate();
      }

      #endregion

      #region Methods

      #region Init

      /// <summary>
      /// Generates this instance.
      /// </summary>
      public void Generate()
      {
         this.Dispose();

         // Generate render target texture
         m_texRender = Utils.GenerateTexture(TextureWrapMode.ClampToEdge);
         GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, m_texSize, m_texSize, 0, 
                       OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, null);
         // Use a FBO if available
         m_fboRender = Utils.GenerateFBO(m_texRender);
         if (m_fboRender != 0)
         {
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
         }

         // Generate attentutaion texture
         m_texAttentuation = Utils.GenerateTexture(TextureWrapMode.Clamp);
         if (GL.SupportsExtension(Constants.GL_ARB_texture_float))
         {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            int size = (int)m_radiusSquared * 2;
            float[] data = CreateAttentuationImageData(size);
            unsafe
            {
               fixed (void* pData = data)
               {
                  GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)ArbTextureFloat.Alpha32fArb, size, size, 0,
                                OpenTK.Graphics.OpenGL.PixelFormat.Alpha, PixelType.Float, new IntPtr(pData));
               }
            }
         }
         else
         {
            // Build Meta-Circle (blob) attentuation texture and set texture data
            Bitmap bitmap = CreateAttentuationImage((int)m_radiusSquared * 2);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.BindTexture(TextureTarget.Texture2D, m_texAttentuation);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                          OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);
         }
      }

      /// <summary>
      /// Creates a attentuation image.
      /// </summary>
      /// <param name="size">The size.</param>
      /// <returns></returns>
      private Bitmap CreateAttentuationImage(int texSize)
      {
         // Find nearest 2^n -> fast texture rendering
         double exp = (int)Math.Log((double)texSize, 2.0);
         texSize = (int)Math.Pow(2.0, exp);

         // Create Image
         Bitmap bitmap = new Bitmap(texSize, texSize);
         float[] data = CreateAttentuationImageData(texSize);
         for (int x = 0; x < bitmap.Width; x++)
         {
            for (int y = 0; y < bitmap.Height; y++)
            {
               // Map energy to alpha
               int alpha = (int)(data[x * texSize + y] * 255.0f);
               // Write value to bitmap
               bitmap.SetPixel(x, y, Color.FromArgb(alpha, Color.White));
            }
         }

#if DEBUG
         bitmap.Save(@"atten_tex.png", ImageFormat.Png);
#endif
         return bitmap;
      }

      /// <summary>
      /// Creates a attentuation image.
      /// </summary>
      /// <param name="size">The size.</param>
      /// <returns></returns>
      private float[] CreateAttentuationImageData(int texSize)
      {
         // Find nearest 2^n -> fast texture rendering
         double exp = (int)Math.Log((double)texSize, 2.0);
         texSize = (int)Math.Pow(2.0, exp);

         // Create Image
         float[] result = new float[texSize * texSize];
         int center = texSize / 2;
         double centerHalfSq = (center / 2.0) * this.Falloff;
         centerHalfSq = centerHalfSq * centerHalfSq;
         float threshMax = this.EnergyThreshold - (this.EnergyThreshold * 0.1f);
         for (int x = 0; x < texSize; x++)
         {
            for (int y = 0; y < texSize; y++)
            {
               // Calculate the squared distance from the center of the meta-circle
               Vector2 dist = new Vector2(x - center, y - center);
               // Use gaussian as falloff functian: e^-(d / (center/2))^2 * this.Energy
               float en = (float)Math.Exp(-dist.LengthSquared / centerHalfSq) * this.Energy;

               // Clamp
               if (en < 0.0f)
               {
                  en = 0.0f;
               }
               else if (en > threshMax)
               {
                  en = threshMax;
               }
               result[x * texSize + y] = en;
            }
         }
         return result;
      }

      #endregion

      #region Graphic methods

      /// <summary>
      /// Push OpenGL states and set state for blob rendering.
      /// </summary>
      public void BeginDraw()
      {
         GL.MatrixMode(MatrixMode.Modelview);
         GL.PushMatrix();
         GL.LoadIdentity();

         GL.PushAttrib(AttribMask.EnableBit | AttribMask.ColorBufferBit | AttribMask.TextureBit);

         GL.Enable(EnableCap.Blend);
         GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
         GL.AlphaFunc(AlphaFunction.Gequal, EnergyThreshold);

         GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvModeCombine.Replace);
      }

      /// <summary>
      /// Restore old OpenGL states.
      /// </summary>
      public void EndDraw()
      {
         GL.PopAttrib();

         GL.MatrixMode(MatrixMode.Modelview);
         GL.PopMatrix();
      }

      /// <summary>
      /// Renders to texture.
      /// </summary>
      /// <param name="particles">The particles.</param>
      /// <param name="domain">The source rectangle (min, max of particle position).</param>

      private void RenderToTex(FluidParticles particles, RectangleF domain)
      {
         //
         // Render-To-Texture
         //

         // Bind FBO if available and clear it
         if (m_fboRender != 0)
         {
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, m_fboRender);
            GL.Clear(ClearBufferMask.ColorBufferBit);
         }
         // Set correct viewport
         GL.PushAttrib(AttribMask.ViewportBit);
         GL.Viewport(0, 0, m_texSize, m_texSize);

         GL.Disable(EnableCap.AlphaTest);
         GL.Enable(EnableCap.Texture2D);
         GL.BindTexture(TextureTarget.Texture2D, m_texAttentuation);

         // Trasnform radius from screen to domain
         float rat  = m_radiusSquared / m_texSize;
         float radW = rat * domain.Width;
         float radH = rat * domain.Height;

         // Draw particles as quads with the size of the squared radius
         GL.Begin(BeginMode.Quads);
         foreach (var particle in particles)
         {
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(particle.Position.X - radW, particle.Position.Y - radH);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(particle.Position.X + radW, particle.Position.Y - radH);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(particle.Position.X + radW, particle.Position.Y + radH);
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(particle.Position.X - radW, particle.Position.Y + radH);
         }
         GL.End();

         // Copy framebuffer to texture if fbo isn't available
         if (m_fboRender == 0)
         {
            BindRenderTexture();
            GL.CopyTexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 0, 0, m_texSize, m_texSize, 0);
         }
         else
         {
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
         }

         GL.PopAttrib();
      }

      /// <summary>
      /// Binds the render texture.
      /// </summary>
      private void BindRenderTexture()
      {
         GL.BindTexture(TextureTarget.Texture2D, m_texRender);
      }

      /// <summary>
      /// Renders the specified particles.
      /// </summary>
      /// <param name="particles">The particles.</param>
      /// <param name="domain">The source rectangle (min, max of possible particle position).</param>
      /// <param name="destCoordinates">The destination coordinates, lenght of the array has to be 4.
      /// Index 0: upper left corner.
      /// Index 1: upper right corner.
      /// Index 2: lower right corner.
      /// Index 3: lower left corner</param>
      public void Render(FluidParticles particles, RectangleF domain, Vector2[] destCoordinates)
      {
         if (destCoordinates.Length != 4)
         {
            throw new ArgumentOutOfRangeException("destCoordinates.Length != 4");
         }

         BeginDraw();

         //
         // Render-To-Texture
         //

         RenderToTex(particles, domain);

         //
         // Render-To-Framebuffer
         //

         //// Clear Frame-Buffer
         //if (m_fboRender == 0)
         //{
         //   GL.Clear(ClearBufferMask.ColorBufferBit);
         //}
         
         // AlphaTest checks if the energy (alpha) is greater or equal than the threshold
         GL.Enable(EnableCap.AlphaTest);

         // Draw single blue quad (background)
         GL.Disable(EnableCap.Texture2D);
         GL.Color4(Color);
         GL.Begin(BeginMode.Quads);
            GL.Vertex2(destCoordinates[0].X, destCoordinates[0].Y);
            GL.Vertex2(destCoordinates[1].X, destCoordinates[1].Y);
            GL.Vertex2(destCoordinates[2].X, destCoordinates[2].Y);
            GL.Vertex2(destCoordinates[3].X, destCoordinates[3].Y);
         GL.End();

         // Draw single quad with rendered alpha texture
         GL.Enable(EnableCap.Texture2D);
         BindRenderTexture();
         GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(destCoordinates[0].X, destCoordinates[0].Y);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(destCoordinates[1].X, destCoordinates[1].Y);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(destCoordinates[2].X, destCoordinates[2].Y);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(destCoordinates[3].X, destCoordinates[3].Y);
         GL.End();

         EndDraw();
      }

      /// <summary>
      /// Renders the specified particles using the size of the texture (specified in ctor).
      /// </summary>
      /// <param name="particles">The particles.</param>
      /// <param name="domain">The source rectangle (min, max of particle position).</param>
      public void Render(FluidParticles particles, RectangleF domain)
      {
         Vector2[] destCoordinates = new Vector2[]
         {
            new Vector2(domain.Left,   domain.Bottom),
            new Vector2(domain.Right,  domain.Bottom),
            new Vector2(domain.Right,  domain.Top),
            new Vector2(domain.Left,   domain.Top)
         };
         this.Render(particles, domain, destCoordinates);
      }

      #endregion

      #region IDisposable Member

      /// <summary>
      /// Disposes this instance.
      /// </summary>
      public void Dispose()
      {
         if (m_texAttentuation != 0)
         {
            GL.DeleteTextures(1, ref m_texAttentuation);
            this.m_texAttentuation = 0;
         }
         if (m_texRender != 0)
         {
            GL.DeleteTextures(1, ref m_texRender);
            this.m_texRender = 0;
         }
         if (m_fboRender != 0)
         {
            GL.Ext.DeleteFramebuffers(1, ref m_fboRender);
            m_fboRender = 0;
         }
      }

      #endregion

      #endregion
   }
}