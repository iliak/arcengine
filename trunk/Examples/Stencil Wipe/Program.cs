﻿#region Licence
//
//This file is part of ArcEngine.
//Copyright (C)2008-2009 Adrien Hémery ( iliak@mimicprod.net )
//
//ArcEngine is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//any later version.
//
//ArcEngine is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion
using System;
using System.Drawing;
using System.Windows.Forms;
using ArcEngine;
using ArcEngine.Asset;
using ArcEngine.Graphic;
using ArcEngine.Input;


using OpenTK.Graphics.OpenGL;




// http://www.videotutorialsrock.com/opengl_tutorial/reflections/text.php
// http://jerome.jouvie.free.fr/OpenGl/Tutorials/Tutorial23.php

namespace StencilWipe
{
    /// <summary>
    /// Main game class
    /// </summary>
    public class StencilWipe : Game
    {

        /// <summary>
        /// Main entry point.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                using (StencilWipe game = new StencilWipe())
                    game.Run();
            }
            catch (Exception e)
            {
                // Oops, an error happened !
                MessageBox.Show(e.StackTrace, e.Message);
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public StencilWipe()
        {
            CreateGameWindow(new Size(800, 600));
			Window.Text = "Wipe effect";
        }



        /// <summary>
        /// Load contents 
        /// </summary>
        public override void LoadContent()
        {
            Display.ClearColor = Color.CornflowerBlue;

			Mask = new Texture("data/mask.png");


			GL.ClearStencil(0);


			// Check if Stencil buffer is ok
			int value = 0;
			GL.GetInteger(GetPName.StencilBits, out value);
			if (value == 0)
			{
				MessageBox.Show("No stencil buffer found !!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Exit();
			}
	


      }


        /// <summary>
        /// Unload contents
        /// </summary>
        public override void UnloadContent()
        {
			Mask.Dispose();
        }


        /// <summary>
        /// Update the game logic
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Check if the Excape key is pressed
            if (Keyboard.IsKeyPress(Keys.Escape))
                Exit();

			// Copy stencil buffer to a texture
			if (Keyboard.IsNewKeyPress(Keys.F1))
			{
				// Temp texture
				Texture tmp = new Texture(Display.ViewPort.Size);
				Display.Texture = tmp;


				// Collect pixels
				byte[] pixels = new byte[Display.ViewPort.Width * Display.ViewPort.Height*4];
				GL.ReadPixels(
					0, 0, Display.ViewPort.Width, Display.ViewPort.Height,
					PixelFormat.Bgra, PixelType.UnsignedByte, pixels);




				ErrorCode code = GL.GetError();

				// Copy to the texture
				tmp.LockTextureBits(ImageLockMode.WriteOnly);
				tmp.Data = pixels;
				tmp.UnlockTextureBits();
				tmp.SaveToDisk("stencil.png");

				tmp.Dispose();
			}
        }



        /// <summary>
        /// Called when it is time to draw a frame.
        /// </summary>
        /// <param name="device"></param>
        public override void Draw()
        {
            // Clears the background
			GL.Clear(ClearBufferMask.AccumBufferBit | ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
			Display.Color = Color.White;


			GL.AlphaFunc(AlphaFunction.Greater, 0);
			Display.AlphaTest = true;
			Display.StencilTest = true;

			GL.ColorMask(false, false, false, false);
			GL.StencilFunc(StencilFunction.Always, 1, 1);
			GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);
			Mask.Blit(new Point(100, 100));




			GL.ColorMask(true, true, true, true);
			GL.StencilFunc(StencilFunction.Equal, 1, 1);
			GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
			Display.Rectangle(new Rectangle(10, 10, 600, 600), true);

			Display.StencilTest = false;


        }




        #region Properties


		/// <summary>
		/// Mask texture
		/// </summary>
		Texture Mask;


        #endregion

    }

}
