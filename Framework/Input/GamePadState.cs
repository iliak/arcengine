﻿using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Input;


namespace ArcEngine.Input
{
	/// <summary>
	/// Describes the state of a gamepad device. 
	/// </summary>
	public class GamePadState
	{

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="joystick"></param>
		internal GamePadState(Joystick joystick)
		{
			Trace.WriteDebugLine("[GamePadState] Constructor");

			if (joystick == null)
			{
				Trace.WriteDebugLine("[GamePadState] joystick == null !");
			}

			Joystick = joystick;
			States = new JoystickState[]
			{
				new JoystickState(),
				new JoystickState()
			};
		}



		/// <summary>
		/// Update gamepad status
		/// </summary>
		internal void Update()
		{
			// No joystick (unplugged ?)
			if (Joystick == null)
				return;
/*
			// Collect device state
			try
			{
				if (Joystick.Acquire().IsFailure)
					return;

				if (Joystick.Poll().IsFailure)
					return;
			}
			catch (DirectInputException e)
			{
				Trace.WriteDebugLine("[GamePadState] Update() : DirectInputException : {0}", e.Message);
				Gamepad.UnplugDevice(Joystick);
				Joystick = null;
				return;
			}

			// Get current state
			Joystick.GetCurrentState(ref States[StateIndex == 0 ? 1 : 0]);


			// Next state index
			StateIndex = (byte)((StateIndex + 1) % 2);
			//StateIndex++;
			//if (StateIndex > 1)
			//   StateIndex = 0;

		//	Trace.WriteLine(StateIndex.ToString());
*/
		}




		#region States


		/// <summary>
		/// Gets if a button is down
		/// </summary>
		/// <param name="id">ID of the button</param>
		/// <returns></returns>
		public bool IsButtonDown(int id)
		{
			//return CurrentState.IsPressed(id);
			return false;
		}


		/// <summary>
		/// Gets if a button is up
		/// </summary>
		/// <param name="id">Id of the button</param>
		/// <returns></returns>
		public bool IsButtonUp(int id)
		{
			//return CurrentState.IsReleased(id);
			return false;
		}



		/// <summary>
		/// Gets if a new button is down
		/// </summary>
		/// <param name="id">ID of the button</param>
		/// <returns></returns>
		public bool IsNewButtonDown(int id)
		{
			//return CurrentState.IsPressed(id) && PreviousState.IsReleased(id);
			return false;
		}


		/// <summary>
		/// Gets if a new button is up
		/// </summary>
		/// <param name="id">Id of the button</param>
		/// <returns></returns>
		public bool IsNewButtonUp(int id)
		{
			//return CurrentState.IsReleased(id) && PreviousState.IsPressed(id);
			return false;
		}



		#endregion



		#region Properties


		/// <summary>
		/// Gets current state
		/// </summary>
		JoystickState CurrentState
		{
			get
			{
				return States[StateIndex];
			}
		}


		/// <summary>
		/// Gets previous state
		/// </summary>
		JoystickState PreviousState
		{
			get
			{
				return States[StateIndex == 0 ? 1 : 0];
			}
		}


		/// <summary>
		/// Current state of the device
		/// </summary>
		JoystickState[] States;


		/// <summary>
		/// Joystick device
		/// </summary>
		internal Joystick Joystick
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets if the Gamepad is unplugged
		/// </summary>
		public bool IsUnplugged
		{
			get
			{
				return Joystick == null;
			}
		}
		
		/// <summary>
		/// Current state index
		/// </summary>
		public byte StateIndex;


		/// <summary>
		/// Gets the state of each button
		/// </summary>
		public bool[] GetButtons
		{
			get
			{
				//return States[StateIndex].GetButtons();
				return new bool[8];
			}
		}


		/// <summary>
		/// Gets the state of each point-of-view controller on the joystick. 
		/// </summary>
		public int[] PovControllers
		{
			get
			{
				//return CurrentState.GetPointOfViewControllers();
				return new int[4];
			}
		}

		/// <summary>
		/// Gets the X-axis, usually the left-right movement of a stick. 
		/// </summary>
		public int X
		{
			get
			{
				//return CurrentState.X;
				return -1;
			}
		}

		/// <summary>
		/// Gets the Y-axis, usually the forward-backward movement of a stick. 
		/// </summary>
		public int Y
		{
			get
			{
				//return CurrentState.Y;
				return -1;
			}
		}

		/// <summary>
		/// Gets the Z-axis, often the throttle control. 
		/// </summary>
		public int Z
		{
			get
			{
				//return CurrentState.Z;
				return -1;
			}
		}

/*
		/// <summary>
		/// Gets the X-axis velocity.
		/// </summary>
		public int VelocityX
		{
			get
			{
				return CurrentState.VelocityX;
			}
		}


		/// <summary>
		/// Gets the Y-axis velocity.
		/// </summary>
		public int VelocityY
		{
			get
			{
				return CurrentState.VelocityY;
			}
		}


		/// <summary>
		/// Gets the Z-axis velocity.
		/// </summary>
		public int VelocityZ
		{
			get
			{
				return CurrentState.VelocityZ;
			}
		}
*/

/*
		/// <summary>
		/// Gets the X-axis torque. 
		/// </summary>
		public int TorqueX
		{
			get
			{
				return CurrentState.TorqueX;
			}
		}


		/// <summary>
		/// Gets the Y-axis torque. 
		/// </summary>
		public int TorqueY
		{
			get
			{
				return CurrentState.TorqueY;
			}
		}


		/// <summary>
		/// Gets the Z-axis torque. 
		/// </summary>
		public int TorqueZ
		{
			get
			{
				return CurrentState.TorqueZ;
			}
		}
*/

		/// <summary>
		/// Gets the X-axis rotation. 
		/// </summary>
		public int RotationX
		{
			get
			{
				//return CurrentState.RotationX;
				return -1;
			}
		}


		/// <summary>
		/// Gets the Y-axis rotation. 
		/// </summary>
		public int RotationY
		{
			get
			{
				//return CurrentState.RotationY;
				return -1;
			}
		}


		/// <summary>
		/// Gets the Z-axis rotation. 
		/// </summary>
		public int RotationZ
		{
			get
			{
				//return CurrentState.RotationZ;
				return -1;
			}
		}

/*
		/// <summary>
		/// Gets the X-axis force. 
		/// </summary>
		public int ForceX
		{
			get
			{
				return CurrentState.ForceX;
			}
		}


		/// <summary>
		/// Gets the Y-axis force. 
		/// </summary>
		public int ForceY
		{
			get
			{
				return CurrentState.ForceY;
			}
		}


		/// <summary>
		/// Gets the Z-axis force. 
		/// </summary>
		public int ForceZ
		{
			get
			{
				return CurrentState.ForceZ;
			}
		}
*/


		#endregion
	}
}
