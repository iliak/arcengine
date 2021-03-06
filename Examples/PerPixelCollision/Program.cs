﻿#region Licence
//
//This file is part of ArcEngine.
//Copyright (C)2008-2010 Adrien Hémery ( iliak@mimicprod.net )
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
using ArcEngine.Asset;
using ArcEngine.Graphic;
using ArcEngine.Input;
using ArcEngine.Utility;

namespace ArcEngine.Examples.PerPixelCollision
{
	/// <summary>
	/// Main game class
	/// </summary>
	public class Program : GameBase
	{

		/// <summary>
		/// Main entry point.
		/// </summary>
		[STAThread]
		static void Main()
		{
			using (Program game = new Program())
				game.Run();
		}


		/// <summary>
		/// Constructor
		/// </summary>
		public Program()
		{
			// Create the game window
			GameWindowParams param = new GameWindowParams();
			param.Size = new Size(1024, 768);
			param.Major = 3;
			param.Minor = 0;
			CreateGameWindow(param);

			// Change the window title
			Window.Text = "Per pixel perfect collision test";

		}


		/// <summary>
		/// Load contents 
		/// </summary>
		public override void LoadContent()
		{
			// Clear color of the screen
			Display.RenderState.ClearColor = Color.LightGray;
			//	Mouse.Visible = false;

			if (!PixelCollision.Init())
			{
				MessageBox.Show("GL_ARB_occlusion_query not found !", "Unsupported extension");
				Exit();
			}

			// Textures
			Logo = new Texture2D("data/logo.png");
			Star = new Texture2D("data/star.png");

			// Font
			Sprite = new SpriteBatch();
			Font = BitmapFont.CreateFromTTF(@"c:\windows\fonts\verdana.ttf", 14, FontStyle.Regular);
		}


		/// <summary>
		/// Unload contents
		/// </summary>
		public override void UnloadContent()
		{
			PixelCollision.Dispose();

			if (Logo != null)
				Logo.Dispose();

			if (Star != null)
				Star.Dispose();

			if (Font != null)
				Font.Dispose();

			if (Sprite != null)
				Sprite.Dispose();
			Sprite = null;
		}


		/// <summary>
		/// Update the game logic
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			// Check if the Escape key is pressed
			if (Keyboard.IsKeyPress(Keys.Escape))
				Exit();


			Angle += 0.5f;
		}



		/// <summary>
		/// Called when it is time to draw a frame.
		/// </summary>
		public override void Draw()
		{
			// Clears the background
			Display.ClearBuffers();


			#region First draw
			//	Sprite.Begin(SpriteBlendMode.Additive, SpriteSortMode.Deferred, false);
			Sprite.Begin();

			// Draw the logo
			DrawLogo();

			// Draw the star
			if (PixelCollision.Count > 0)
				StarColor = Color.Red;
			else
				StarColor = Color.White;


			// Draws the star
			Sprite.Draw(Star, new Vector2(Mouse.Location.X, Mouse.Location.Y), StarColor);

			Sprite.End();

			#endregion


			#region Occlusion query

			PixelCollision.Begin(0.1f);

			Sprite.Begin();
			DrawLogo();
			Sprite.End();

			// Begin query
			PixelCollision.BeginQuery();

			Sprite.Begin();
			Sprite.Draw(Star, new Vector2(Mouse.Location.X, Mouse.Location.Y), StarColor);
			Sprite.End();

			PixelCollision.EndQuery();


			PixelCollision.End();

			#endregion


			// Some text
			Sprite.Begin();
			Sprite.DrawString(Font, new Vector2(10, 30), Color.Red, "Count {0}", PixelCollision.Count);
			Sprite.DrawString(Font, new Vector2(10, 45), Color.Red, "Mouse {0}", new Vector2(Mouse.Location.X, Mouse.Location.Y));
			Sprite.End();

		}


		/// <summary>
		/// Draws the collision logo
		/// </summary>
		private void DrawLogo()
		{
			Vector2 dst = new Vector2(Display.ViewPort.Width / 2.0f, Display.ViewPort.Height / 2.0f);
			//		dst = new Vector2(200, 200);
			//		dst = new Vector2(Mouse.Location.X, Mouse.Location.Y);

			Sprite.Draw(Logo, dst, Vector4.Zero, Color.White, Angle, new Vector2(Logo.Size.Width / 2, Logo.Size.Height / 2), Vector2.One, SpriteEffects.None, 0.0f);
		}



		#region Properties

		/// <summary>
		/// Sprite batch
		/// </summary>
		SpriteBatch Sprite;

		/// <summary>
		/// Texture
		/// </summary>
		Texture2D Logo;


		/// <summary>
		/// Texture
		/// </summary>
		Texture2D Star;


		/// <summary>
		/// Font
		/// </summary>
		BitmapFont Font;


		/// <summary>
		/// Star drawing color
		/// </summary>
		Color StarColor;


		/// <summary>
		/// Rotation angle of the logo
		/// </summary>
		float Angle;


		#endregion

	}
}
