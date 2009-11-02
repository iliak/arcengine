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
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using ArcEngine.Forms;
using ArcEngine.Graphic;
using ArcEngine.Input;

namespace ArcEngine
{
	/// <summary>
	/// Provides basic graphics device initialization, game logic, and rendering code.
	/// </summary>
	public class Game : IDisposable
	{

		#region ctor

		/// <summary>
		/// Constructor
		/// </summary>
		public Game()
		{
			Trace.TraceInventory();

			Clock = new GameClock();
			GameTime = new GameTime();
			MaximumElapsedTime = TimeSpan.FromMilliseconds(500.0);
			TargetElapsedTime = TimeSpan.FromTicks(166667L);
			IsFixedTimeStep = true;

			Random = new Random((int)DateTime.Now.Ticks);


		}


		/// <summary>
		/// Destructor
		/// </summary>
		~Game()
		{
			Dispose(false);
		}

		#endregion


		#region GameWindow creation

		/// <summary>
		/// Create the GameWindow, with the highest compatible OpenGL version
		/// </summary>
		/// <param name="size">Size of the window</param>
		public void CreateGameWindow(Size size)
		{
			CreateGameWindow(size, 99, 99);
		}


		/// <summary>
		/// Create the GameWindow, and use a specific OpenGL version
		/// </summary>
		/// <param name="size">Size of the window</param>
		/// <param name="major">Major version</param>
		/// <param name="minor">Minor version</param>
		public void CreateGameWindow(Size size, int major, int minor)
		{
			// Close the previous game window first
			if (Window != null)
			{
				Window.Dispose();
				Window = null;
			}

			Window = new GameWindow(size, major, minor);
			Window.Show();



			Mouse.Init(Window);
		}


		#endregion


		#region Game logic


		private bool ForceElapsedTimeToZero;
		private TimeSpan LastFrameElapsedGameTime;
		private TimeSpan LastFrameElapsedRealTime;
		private TimeSpan AccumulatedElapsedGameTime;
		private int UpdatesSinceRunningSlowly1;
		private int UpdatesSinceRunningSlowly2;
		private bool DrawRunningSlowly;
		private TimeSpan TotalGameTime;
		private bool SuppressDraw;


		/// <summary>
		/// Performs one complete frame for the game.
		/// </summary>
		void Tick()
		{


			// if we are exiting or in editor mode, do nothing
			if (IsExiting || EditorMode)
			{
				Thread.Sleep(10);
				return;
			}

			// if we are inactive, sleep for a bit
			if (!IsActive)
				Thread.Sleep(20);


			// Update the clock
			Clock.Step();

			bool flag = true;
			GameTime.TotalRealTime = Clock.CurrentTime;
			GameTime.ElapsedRealTime = Clock.ElapsedTime;
			LastFrameElapsedRealTime += Clock.ElapsedTime;
			TimeSpan elapsedAdjustedTime = Clock.ElapsedAdjustedTime;
			if (elapsedAdjustedTime < TimeSpan.Zero)
				elapsedAdjustedTime = TimeSpan.Zero;

			if (ForceElapsedTimeToZero)
			{
				GameTime.ElapsedRealTime = LastFrameElapsedRealTime = elapsedAdjustedTime = TimeSpan.Zero;
				ForceElapsedTimeToZero = false;
			}

			if (elapsedAdjustedTime > MaximumElapsedTime)
				elapsedAdjustedTime = MaximumElapsedTime;


			// Fixed time step clock
			if (IsFixedTimeStep)
			{
				if (Math.Abs((long)(elapsedAdjustedTime.Ticks - TargetElapsedTime.Ticks)) < (TargetElapsedTime.Ticks >> 6))
					elapsedAdjustedTime = TargetElapsedTime;

				AccumulatedElapsedGameTime += elapsedAdjustedTime;
				long num = AccumulatedElapsedGameTime.Ticks / TargetElapsedTime.Ticks;
				AccumulatedElapsedGameTime = TimeSpan.FromTicks(AccumulatedElapsedGameTime.Ticks % TargetElapsedTime.Ticks);
				LastFrameElapsedGameTime = TimeSpan.Zero;
				if (num == 0)
				{
					return;
				}
				TimeSpan targetElapsedTime = TargetElapsedTime;

				if (num > 1)
				{
					UpdatesSinceRunningSlowly2 = UpdatesSinceRunningSlowly1;
					UpdatesSinceRunningSlowly1 = 0;
				}
				else
				{
					if (UpdatesSinceRunningSlowly1 < int.MaxValue)
						UpdatesSinceRunningSlowly1++;

					if (UpdatesSinceRunningSlowly2 < int.MaxValue)
						UpdatesSinceRunningSlowly2++;

				}
				DrawRunningSlowly = UpdatesSinceRunningSlowly2 < 20;


				// update until it's time to draw the next frame
				while ((num > 0) && !IsExiting)
				{
					num -= 1;

					GameTime.ElapsedGameTime = TargetElapsedTime;
					GameTime.TotalGameTime = TotalGameTime;
					GameTime.IsRunningSlowly = DrawRunningSlowly;

					if (Window.HasFocus)
					{
						Keyboard.Update();
						Mouse.Update();
						GamePad.Update();
					}

				//	Trace.WriteLine("#############################");
					Update(GameTime);

					flag &= SuppressDraw;
					SuppressDraw = false;

					LastFrameElapsedGameTime += TargetElapsedTime;
					TotalGameTime += TargetElapsedTime;
				}
			}
			else
			{
				TimeSpan span3 = elapsedAdjustedTime;
				DrawRunningSlowly = false;
				UpdatesSinceRunningSlowly1 = int.MaxValue;
				UpdatesSinceRunningSlowly2 = int.MaxValue;
				if (!IsExiting)
				{
					try
					{
						GameTime.ElapsedGameTime = LastFrameElapsedGameTime = span3;
						GameTime.TotalGameTime = TotalGameTime;
						GameTime.IsRunningSlowly = false;

						if (Window.HasFocus)
						{
							Keyboard.Update();
							Mouse.Update();
							GamePad.Update();
						}

						Update(GameTime);
						flag &= SuppressDraw;
						SuppressDraw = false;
					}
					finally
					{
						TotalGameTime += span3;
					}
				}
			}



			if (!flag)
			{
				if (IsExiting)
					return;

				GameTime.TotalRealTime = Clock.CurrentTime;
				GameTime.ElapsedRealTime = LastFrameElapsedRealTime;
				GameTime.TotalGameTime = TotalGameTime;
				GameTime.ElapsedGameTime = LastFrameElapsedGameTime;
				GameTime.IsRunningSlowly = DrawRunningSlowly;

				Display.RenderStats.Reset();

				Draw();

				Window.SwapBuffers();
			}


		}


		/// <summary>
		/// Called when the game has determined that game logic needs to be processed.
		/// </summary>
		/// <param name="gameTime">The time passed since the last update.</param>
		public virtual void Update(GameTime gameTime)
		{
			// Check if the Escape key is pressed
			if (Keyboard.IsKeyPress(Keys.Escape))
				Exit();
		}


		/// <summary>
		/// Called when the game determines it is time to draw a frame.
		/// </summary>
		public virtual void Draw()
		{
			Display.ClearColor = Color.CornflowerBlue;
			Display.ClearBuffers();
		}




		/// <summary>
		/// Called when graphics resources need to be loaded.
		/// </summary>
		public virtual void LoadContent()
		{
		}


		/// <summary>
		/// Called when graphics resources need to be unloaded.
		/// </summary>
		public virtual void UnloadContent()
		{
		}



		/// <summary>
		/// Runs the game
		/// </summary>
		public void Run()
		{
			Trace.WriteLine("Running the game");

			// Initializations
			//Audio.Init();
			GamePad.Init(Window);


			LoadContent();


			IsRunning = true;
			
			try
			{
				GameTime.ElapsedGameTime = TimeSpan.Zero;
				GameTime.ElapsedRealTime = TimeSpan.Zero;
				GameTime.TotalGameTime = TotalGameTime;
				GameTime.TotalRealTime = Clock.CurrentTime;
				GameTime.IsRunningSlowly = false;

				Update(GameTime);

				Application.Idle += Application_Idle;
				Application.Run(Window);
			}
			finally
			{
				Application.Idle -= Application_Idle;
				IsRunning = false;
				//OnExiting(EventArgs.Empty);

				UnloadContent();
			}

			//Audio.Release();
			GamePad.Release();

			ResourceManager.Close();
		}

		#endregion




		#region Events


		/// <summary>
		/// Raised when the game gains focus.
		/// </summary>
		public event EventHandler Activated;


		/// <summary>
		/// Raised when the game loses focus.
		/// </summary>
		public event EventHandler Deactivated;


		#endregion



		/// <summary>
		/// Message pump
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Application_Idle(object sender, EventArgs e)
		{
			NativeMessage message;
			while (!PeekMessage(out message, IntPtr.Zero, 0, 0, 0))
			{
				if (IsExiting)
					Window.Close();
				else
					Tick();
			}
		}




		/// <summary>
		/// Exits the game.
		/// </summary>
		public void Exit()
		{
			IsExiting = true;

			Trace.WriteLine("Exit requested !");
		}


	
		#region Time management

		/// <summary>
		/// Is the game running at a fixed time setp
		/// </summary>
		public bool IsFixedTimeStep
		{
			get;
			set;
		}


		/// <summary>
		/// Desired time step in ms
		/// </summary>
		public TimeSpan TargetElapsedTime
		{
			get;
			set;
		}



		/// <summary>
		/// Maximum elapsed time allowed
		/// </summary>
		public TimeSpan MaximumElapsedTime
		{
			get;
			internal set;
		}


		/// <summary>
		/// 
		/// </summary>
		public GameTime GameTime
		{
			get;
			private set;
		}



		/// <summary>
		/// Game clock
		/// </summary>
		GameClock Clock;



		#endregion




		#region Editor

		/// <summary>
		/// Launch the editor
		/// </summary>
		public void RunEditor()
		{
			bool mousestate = Mouse.Visible;
			EditorMode = true;
			Mouse.Visible = true;

			Window.Hide();
	
			new Editor.EditorForm().ShowDialog();
			EditorMode = false;


			Window.MakeCurrent();
			Mouse.Visible = mousestate;
			Window.Show();
			Window.BringToFront();
		}

		#endregion



		#region Properties



		/// <summary>
		/// Gets the game window.
		/// </summary>
		/// <value>The game window.</value>
		public GameWindow Window
		{
			get;
			private set;
		}



		/// <summary>
		/// Gets a value indicating whether this Game is exiting.
		/// </summary>
		/// <value>true if exiting; otherwise, false.</value>
		public bool IsExiting
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets or sets a value indicating whether this instance is running.
		/// </summary>
		/// <value>true if this instance is running; otherwise, false.</value>
		public bool IsRunning
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets or sets a value indicating whether this Game is active.
		/// </summary>
		/// <value>true if active; otherwise, false.</value>
		public bool IsActive
		{
			get;
			internal set;
		}


		/// <summary>
		/// True if in editor mode
		/// </summary>
		public bool EditorMode
		{
			get;
			private set;
		}


		/// <summary>
		/// Random number
		/// </summary>
		static public Random Random
		{
			get;
			private set;
		}


		#endregion



		#region Win32 Interop


		/// <summary>
		/// 
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		struct NativeMessage
		{
			public IntPtr hWnd;
			public uint msg;
			public IntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public Point p;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="hwnd"></param>
		/// <param name="messageFilterMin"></param>
		/// <param name="messageFilterMax"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		[SuppressUnmanagedCodeSecurityAttribute]
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool PeekMessage(out NativeMessage message, IntPtr hwnd, uint messageFilterMin, uint messageFilterMax, uint flags);

		#endregion



		#region Dispose



		/// <summary>
		/// Disposing resources
		/// </summary>
		/// <param name="disposing">true if disposing managed resources</param>
		// http://www.dotnet2themax.com/blogs/fbalena/SearchView.aspx?q=Dispose/Finalize%20
		// http://www.devx.com/dotnet/Article/33167/0/page/3
		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Code to dispose the managed resources of the class
			}


			// Code to dispose the un-managed resources of the class

			isDisposed = true;
		}


		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// 
		/// </summary>
		private bool isDisposed = false;

		#endregion

	}
}
