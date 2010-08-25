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
using System.ComponentModel;
using System.Drawing;
using System.Xml;

namespace DungeonEye
{


	/// <summary>
	/// Location in the dungeon
	/// </summary>
//	[EditorAttribute(typeof(DungeonLocationEditor), typeof(UITypeEditor))]
	[TypeConverter(typeof(DungeonLocationConverter))]
	public class DungeonLocation
	{
		#region constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="dungeon">Initial dungeon</param>
		public DungeonLocation(Dungeon dungeon)
		{
			Dungeon = dungeon;
			Compass = new Compass();
			Position = Point.Empty;
			GroundPosition = GroundPosition.NorthEast;
		}



		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="loc">Location</param>
		public DungeonLocation(DungeonLocation loc)
		{
			Dungeon = loc.Dungeon;
			Compass = new Compass(loc.Compass);
			Position = loc.Position;
			GroundPosition = loc.GroundPosition;
			SetMaze(loc.MazeName);
		}



		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">Name of the maze</param>
		/// <param name="location">Location</param>
		/// <param name="direction">Direction</param>
		public DungeonLocation(string name, Point location, CardinalPoint direction)
		{
			Position = location;
			Compass = new Compass();
			Direction = direction;
			SetMaze(name);
		}


		#endregion



		/// <summary>
		/// Offset the location
		/// </summary>
		/// <param name="direction">Direction of the move</param>
		/// <param name="range">Number of block to move</param>
		public void Offset(CardinalPoint direction, int range)
		{
			switch (direction)
			{
				case CardinalPoint.North:
				Position.Y -= range;
				break;
				case CardinalPoint.South:
				Position.Y += range;
				break;
				case CardinalPoint.West:
				Position.X -= range;
				break;
				case CardinalPoint.East:
				Position.X += range;
				break;
			}

	
		}


		/// <summary>
		/// Facing a given location
		/// </summary>
		/// <param name="location">Location to check</param>
		/// <returns>True if facing, or false</returns>
		public bool IsFacing(DungeonLocation location)
		{
			Point offset = new Point(location.Position.X - Position.X, location.Position.Y - Position.Y);

			switch (Direction)
			{
				case CardinalPoint.North:
				{
					if (offset.X <= 0 && offset.Y <= 0)
					return true;
				}
				break;
				case CardinalPoint.South:
				{
					if (offset.X >= 0 && offset.Y >= 0)
						return true;
				}
				break;
				case CardinalPoint.West:
				{
					if (offset.X <= 0 && offset.Y >= 0)
						return true;
				}
				break;
				case CardinalPoint.East:
				{
					if (offset.X >= 0 && offset.Y <= 0)
						return true;
				}
				break;
			}

			return false;
		}


		#region I/O


		/// <summary>
		/// 
		/// </summary>
		/// <param name="node">Xml node</param>
		/// <returns></returns>
		public bool Load(XmlNode xml)
		{
			if (xml == null)
				return false;

			if (xml.Attributes["maze"] != null)
				SetMaze(xml.Attributes["maze"].Value);
			Position = new Point(int.Parse(xml.Attributes["x"].Value), int.Parse(xml.Attributes["y"].Value));

			if (xml.Attributes["direction"] != null)
				Direction = (CardinalPoint)Enum.Parse(typeof(CardinalPoint), xml.Attributes["direction"].Value, true);

			if (xml.Attributes["position"] != null)
				GroundPosition = (GroundPosition) Enum.Parse(typeof(GroundPosition), xml.Attributes["position"].Value, true);

			return true;
		}


		/// <summary>
		/// Saves location
		/// </summary>
		/// <param name="name">Node's name</param>
		/// <param name="writer">XmlWriter handle</param>
		/// <returns></returns>
		public bool Save(string name, XmlWriter writer)
		{
			if (writer == null || string.IsNullOrEmpty(name))
				return false;



			writer.WriteStartElement(name);
			writer.WriteAttributeString("maze", MazeName);
			writer.WriteAttributeString("x", Position.X.ToString());
			writer.WriteAttributeString("y", Position.Y.ToString());
			writer.WriteAttributeString("direction", Direction.ToString());
			writer.WriteAttributeString("position", GroundPosition.ToString());
			writer.WriteEndElement();

			return true;
		}



		#endregion


		/// <summary>
		/// Changes the current maze
		/// </summary>
		/// <param name="name">Desired maze name</param>
		/// <returns>True if maze found</returns>
		public bool SetMaze(string name)
		{
			if (string.IsNullOrEmpty(name))
				return false;

			if (Dungeon == null || Dungeon.GetMaze(name) == null)
				return false;

			MazeName = name;
			Maze = Dungeon.GetMaze(name);

			return true;
		}



		/// <summary>
		/// Returns a String that represents the current location
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0}x{1} {2} {3}", Position.X, Position.Y, MazeName, Direction.ToString());
		}



		#region Properties


		/// <summary>
		/// Dungeon handle
		/// </summary>
		public Dungeon Dungeon
		{
			get;
			private set;
		}

	
		/// <summary>
		/// Handle of the maze
		/// </summary>
		public Maze Maze
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets current maze name
		/// </summary>
		public string MazeName
		{
			get;
			private set;
		}


		/// <summary>
		/// Location in the maze
		/// </summary>
		public Point Position;


		/// <summary>
		/// Facing direction
		/// </summary>
		public CardinalPoint Direction
		{
			get
			{
				return Compass.Direction;
			}
			set
			{
				Compass.Direction = value;
			}
		}


		/// <summary>
		/// TurnLeft
		/// </summary>
		[Browsable(false)]
		public Compass Compass;


		/// <summary>
		/// Position on the ground
		/// </summary>
		public GroundPosition GroundPosition
		{
			get;
			set;
		}


		/// <summary>
		/// 
		/// </summary>
		public MazeBlock Block
		{
			get
			{
				if (Maze == null)
					return null;

				return Maze.GetBlock(Position);
			}
		}

		#endregion

	}




	/// <summary>
	/// Convert DungeonLocation to a human readable string
	/// </summary>
	internal class DungeonLocationConverter : ExpandableObjectConverter
	{

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
		{
			if (destType == typeof(string) && value is DungeonLocation)
			{
				DungeonLocation loc = value as DungeonLocation;

				if (string.IsNullOrEmpty(loc.MazeName))
					return string.Empty;

				return loc.MazeName + " " + loc.Position.ToString() + "-" + loc.Direction.ToString();
			}

			return base.ConvertTo(context, culture, value, destType);
		}

	}

/*

	/// <summary>
	/// WinForm editor for the DungeonLocation
	/// </summary>
	internal class DungeonLocationEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(
		  ITypeDescriptorContext context)
		{
			if (context != null)
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if ((context != null) && (provider != null))
			{
				// Access the Property Browser's UI display service
				IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (editorService == null)
					return new DungeonLocation();


				// Create an instance of the UI editor form
				DungeonLocationForm modalEditor = new DungeonLocationForm(null, value as DungeonLocation);


				// Display the UI editor dialog
				if (editorService.ShowDialog(modalEditor) == DialogResult.OK)
				{
					return modalEditor.DungeonLocation;
				}

				return new DungeonLocation();
			}
			return base.EditValue(context, provider, value);
		}
	}

	*/
}
