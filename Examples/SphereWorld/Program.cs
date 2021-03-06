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
using ArcEngine.Asset;
using ArcEngine.Audio;
using ArcEngine.Graphic;
using ArcEngine.Input;

namespace ArcEngine.Examples.SphereWorld
{
	/// <summary>
	/// Main game class
	/// </summary>
	public class Example : GameBase
	{

		/// <summary>
		/// Main entry point.
		/// </summary>
		[STAThread]
		static void Main()
		{
			using (Example game = new Example())
				game.Run();
		}


		/// <summary>
		/// Constructor
		/// </summary>
		public Example()
		{
			CreateGameWindow(new Size(1024, 768));
		}


		/// <summary>
		/// Load contents 
		/// </summary>
		public override void LoadContent()
		{
			Display.RenderState.ClearColor = Color.Black;
			Display.RenderState.DepthTest = true;
			Display.RenderState.Culling = true;


			#region Audio

			AudioManager.Create();
			Stream = new AudioStream();
			Stream.LoadOgg("data/Hydrate-Kenny_Beltrey.ogg");
			Stream.Play();

			#endregion


			#region Matrices

			CameraPostion = new Vector3(0.0f, 0.1f, 3.0f);

			float aspectRatio = (float)Display.ViewPort.Width / (float)Display.ViewPort.Height;
			ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), aspectRatio, 0.1f, 100.0f);

			#endregion


			#region Shader

			Shader = new Shader();
			Shader.LoadSource(ShaderType.VertexShader, "data/shader.vert");
			Shader.LoadSource(ShaderType.FragmentShader, "data/shader.frag");
			Shader.Compile();
			Display.Shader = Shader;

			#endregion


			#region Textures

			Texture2D.DefaultMagFilter = TextureMagFilter.Linear;
			Texture2D.DefaultMinFilter = TextureMinFilter.Linear;
			Texture2D.DefaultHorizontalWrapFilter = TextureWrapFilter.Repeat;
			Texture2D.DefaultVerticalWrapFilter = TextureWrapFilter.Repeat;

			Marble = new Texture2D("data/Marble.png");
			Marble.MinFilter = TextureMinFilter.Linear;
			Marble.MagFilter = TextureMagFilter.Linear;
			Marble.HorizontalWrap = TextureWrapFilter.Repeat;
			Marble.VerticalWrap = TextureWrapFilter.Repeat;

			Moon = new Texture2D("data/Moon.png");
			Moon.MinFilter = TextureMinFilter.Linear;
			Moon.MagFilter = TextureMagFilter.Linear;

			Mars = new Texture2D("data/Mars.png");
			Mars.MinFilter = TextureMinFilter.Linear;
			Mars.MagFilter = TextureMagFilter.Linear;

			#endregion


			#region Mesh
			Torus = Mesh.CreateTorus(0.15f, 0.50f, 40, 20);

			Sphere = Mesh.CreateSphere(0.1f, 26);
			Sphere.Position = new Vector3(1.0f, 0.4f, 0.0f);

			float[] data = new float[]
			{
				// Vertex					Texture
				-10.0f, 0.0f,  20.0f,		0.0f, 0.0f,
				 10.0f, 0.0f,  20.0f,		20.0f, 0.0f,
				 10.0f, 0.0f, -20.0f,		20.0f, 20.0f,
				-10.0f, 0.0f, -20.0f,		0.0f, 20.0f,
			};
			Floor = new Mesh();
			Floor.SetVertices(data);
			Floor.SetIndices(new uint[] { 0, 1, 2, 3});
			Floor.Buffer.AddDeclaration("in_position", 3);
			Floor.Buffer.AddDeclaration("in_texcoord", 2);
			Floor.PrimitiveType = PrimitiveType.TriangleFan;
			Floor.Position = new Vector3(0.0f, -1.0f, 0.0f);

			#endregion


			Batch = new Graphic.SpriteBatch();
			Font = BitmapFont.CreateFromTTF(@"c:\windows\fonts\verdana.ttf", 10, FontStyle.Regular);
		}


		/// <summary>
		/// Unload contents
		/// </summary>
		public override void UnloadContent()
		{
			if (Torus != null)
				Torus.Dispose();
			Torus = null;

			if (Sphere != null)
				Sphere.Dispose();
			Sphere = null;

			if (Floor != null)
				Floor.Dispose();
			Floor = null;

			if (Shader != null)
				Shader.Dispose();
			Shader = null;

			if (Moon != null)
				Moon.Dispose();
			Moon = null;

			if (Mars != null)
				Mars.Dispose();
			Mars = null;

			if (Marble != null)
				Marble.Dispose();
			Marble = null;

			if (Batch != null)
				Batch.Dispose();
			Batch = null;

			if (Font != null)
				Font.Dispose();
			Font = null;

			if (Stream != null)
				Stream.Dispose();
			Stream = null;
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

			if (Keyboard.IsKeyPress(Keys.Down) || Keyboard.IsKeyPress(Keys.S))
				CameraPostion = Vector3.Add(CameraPostion, new Vector3(0.0f, 0.0f, CameraSpeed));

			if (Keyboard.IsKeyPress(Keys.Up) || Keyboard.IsKeyPress(Keys.Z))
				CameraPostion = Vector3.Add(CameraPostion, new Vector3(0.0f, 0.0f, -CameraSpeed));


			if (Keyboard.IsKeyPress(Keys.Left) || Keyboard.IsKeyPress(Keys.Q))
				CameraPostion = Vector3.Add(CameraPostion, new Vector3(-CameraSpeed, 0.0f, 0.0f));

			if (Keyboard.IsKeyPress(Keys.Right) || Keyboard.IsKeyPress(Keys.D))
				CameraPostion = Vector3.Add(CameraPostion, new Vector3(CameraSpeed, 0.0f, 0.0f));

			Torus.Rotation += new Vector3(0.0f, 0.025f, 0.0f);
			Sphere.Rotation -= new Vector3(0.0f, 0.02f, 0.0f);
		}


		/// <summary>
		/// Called when it is time to draw a frame.
		/// </summary>
		public override void Draw()
		{
			// Clears the background
			Display.ClearBuffers();

			Vector3 target = Vector3.Add(CameraPostion, new Vector3(0.0f, 0.0f, -1.0f));
			Vector4 lightPos = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
			Vector4 diffuseColor = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);

			// Bind the shader and the texture
			Display.Shader = Shader;
			Shader.SetUniform("textureUnit0", 0);
			Shader.SetUniform("diffuseColor", diffuseColor);

	
			#region Mirror

			// ModelViewProjection matrix
			ModelViewMatrix = Matrix4.CreateTranslation(0.0f, 1.7f, 0.0f) * Matrix4.Scale(1.0f, -1.0f, 1.0f) * Matrix4.LookAt(CameraPostion, target, Vector3.UnitY);
			Matrix4 mvp = ModelViewMatrix * ProjectionMatrix;
			Shader.SetUniform("mvMatrix", ModelViewMatrix);

			// Tranform light position
			Shader.SetUniform("lightPosition", Vector4.Transform(lightPos, ModelViewMatrix));

			Display.RenderState.FrontFace = FrontFace.ClockWise;

			Display.Texture = Mars;
			Shader.SetUniform("mvpMatrix", Torus.Matrix * mvp);
			Shader.SetUniform("color", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
			Torus.Draw();

			Display.Texture = Moon;
			Shader.SetUniform("mvpMatrix", Matrix4.CreateTranslation(Sphere.Position) * Matrix4.CreateRotationY(Sphere.Rotation.Y) * mvp);
			Sphere.Draw();

			Display.RenderState.FrontFace = FrontFace.CounterClockWise;

			#endregion


			#region Normal view

			ModelViewMatrix = Matrix4.LookAt(CameraPostion, target, Vector3.UnitY);
			mvp = ModelViewMatrix * ProjectionMatrix;


			// Tranform light position
			Shader.SetUniform("lightPosition", Vector4.Transform(lightPos, ModelViewMatrix));
			
			Display.RenderState.Blending = true;
			Display.BlendingFunction(BlendingFactorSource.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			Display.Texture = Marble;
			Shader.SetUniform("mvpMatrix", Floor.Matrix * mvp);
			Shader.SetUniform("color", new Vector4(1.0f, 1.0f, 1.0f, 0.75f));
			Floor.Draw();
			Display.RenderState.Blending = false;
			Shader.SetUniform("color", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));


			Display.Texture = Mars;
			Shader.SetUniform("mvpMatrix", Torus.Matrix * mvp);
			Torus.Draw();

			Display.Texture = Moon;
			Shader.SetUniform("mvpMatrix", Matrix4.CreateTranslation(Sphere.Position) * Matrix4.CreateRotationY(Sphere.Rotation.Y) * mvp);
			Sphere.Draw();

			#endregion


			Batch.Begin();
			Batch.DrawString(Font, new Vector2(10, 50), Color.White, "Camera position : {0}", CameraPostion.ToString());
			Batch.End();
		}



		#region Properties


		/// <summary>
		/// Camera rotation speed
		/// </summary>
		const float RotationSpeed = 0.3f;


		/// <summary>
		/// Camera move speed
		/// </summary>
		const float CameraSpeed = 0.1f;


		/// <summary>
		/// Position of the camera
		/// </summary>
		Vector3 CameraPostion;


		/// <summary>
		/// 
		/// </summary>
		Vector3 CameraRotation;


		/// <summary>
		/// Torus mesh
		/// </summary>
		Mesh Torus;


		/// <summary>
		/// Sphere mesh
		/// </summary>
		Mesh Sphere;


		/// <summary>
		/// Floor mesh
		/// </summary>
		Mesh Floor;


		/// <summary>
		/// Shader
		/// </summary>
		Shader Shader;


		/// <summary>
		/// Model view matrix
		/// </summary>
		Matrix4 ModelViewMatrix;


		/// <summary>
		/// Projection matrix
		/// </summary>
		Matrix4 ProjectionMatrix;


		/// <summary>
		/// Marble texture
		/// </summary>
		Texture2D Marble;


		/// <summary>
		/// Mars texture
		/// </summary>
		Texture2D Mars;


		/// <summary>
		/// Moon texture
		/// </summary>
		Texture2D Moon;


		/// <summary>
		/// Bitmap font
		/// </summary>
		BitmapFont Font;
		

		/// <summary>
		/// Spritebatch
		/// </summary>
		SpriteBatch Batch;


		/// <summary>
		/// Audio stream from http://www.vorbis.com/music/
		/// </summary>
		AudioStream Stream;

		#endregion

	}

}
