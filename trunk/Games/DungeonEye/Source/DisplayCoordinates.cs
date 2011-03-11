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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using ArcEngine;
using ArcEngine.Graphic;
using ArcEngine.Asset;


//
//
// http://dmweb.free.fr/?q=node/1394
//
//
// Type of coordinates
//		lighting bolts
//		clouds
//		creatures
//		items
//		stack of items
//		ThrownItems
//		Doors
//		Floor decorations (foot steps)
//		Wall decorations
//		Unreadable texts
//		Wall texts
//		Door buttons
//		Door decorations
//
//

namespace DungeonEye
{
	/// <summary>
	/// Visual display informations
	/// </summary>
	public static class DisplayCoordinates
	{

		/// <summary>
		/// Default constructor 
		/// </summary>
		static DisplayCoordinates()
		{
			int viewcount = Enum.GetValues(typeof(ViewFieldPosition)).Length;

			Doors = new TileDrawing[viewcount];
			FloorPlates = new TileDrawing[viewcount];
			
			Ground = new Point[viewcount, 5];
			for (int i = 0 ; i < viewcount ; i++)
				for (int j = 0 ; j < 5 ; j++)
					Ground[i,j] = new Point(-999, -999);

			Decorations = new TileDrawing[viewcount, 3];
			FlyingItems = new Point[viewcount, 5];
			Walls = new List<TileDrawing>[viewcount];
			for (int i = 0; i < viewcount; i++)
				Walls[i] = new List<TileDrawing>();

			Pits = new TileDrawing[viewcount];
			CeilingPits = new TileDrawing[viewcount];
			Stairs = new List<TileDrawing>[viewcount];
			for (int i = 0; i < viewcount; i++)
				Stairs[i] = new List<TileDrawing>();


			ThrowRight =  new Rectangle(176, 0, 176, 144);
			ThrowLeft = new Rectangle(0, 0, 176, 144);
			CampButton = new Rectangle(578, 354, 62, 42);
			FrontSquare = new Rectangle(48, 14, 256, 192);
			Alcove = new Rectangle(130, 64, 128, 44);
			LeftFeetTeam = new Rectangle(0, 202, 176, 38);
			LeftFrontTeamGround = new Rectangle(0, 144, 176, 58);
			RightFeetTeam = new Rectangle(176, 202, 176, 38);
			RightFrontTeamGround = new Rectangle(176, 144, 176, 58);

			ScriptedDialog = new Rectangle(0, 242, 640, 158);
			ScriptedDialogChoices = new Rectangle[]
			{
				// 1 choice
				new Rectangle(442,	378, 190, 18),
				Rectangle.Empty,
				Rectangle.Empty,

				// 2 choices
				new Rectangle(118,	378, 190, 18),
				new Rectangle(332,	378, 190, 18),
				Rectangle.Empty,

				// 3 choices
				new Rectangle(8,	378, 190, 18),
				new Rectangle(224,	378, 190, 18),
				new Rectangle(440,	378, 190, 18),
			};

			Scroll = new Rectangle(0, 0, 352, 350);
			ScrollOk = new Rectangle(152, 324, 190, 18);
		}


		#region Getters

		/// <summary>
		/// Gets a draw order information
		/// </summary>
		/// <param name="position">Block position in the view field</param>
		/// <returns></returns>
		static public List<TileDrawing> GetWalls(ViewFieldPosition position)
		{
			return Walls[(int)position];
		}


		/// <summary>
		/// Gets a ground item coordinate
		/// </summary>
		/// <param name="view">Block position in the view field</param>
		/// <param name="position">ground position</param>
		/// <returns></returns>
		static public Point GetGroundPosition(ViewFieldPosition view, SquarePosition position)
		{
			return Ground[(int)view, (int)position];
		}


		/// <summary>
		/// Gets a flying item coordinate
		/// </summary>
		/// <param name="view">Block position in the view field</param>
		/// <param name="ground">ground position</param>
		/// <returns></returns>
		static public Point GetFlyingItem(ViewFieldPosition view, SquarePosition ground)
		{
			return FlyingItems[(int)view, (int)ground];
		}


		/// <summary>
		/// Gets a decoration coordinate
		/// </summary>
		/// <param name="view">Block position in the view field</param>
		/// <param name="point">Wall side</param>
		/// <returns></returns>
		static public TileDrawing GetDecoration(ViewFieldPosition view, CardinalPoint point)
		{
			if (point == CardinalPoint.North)
				return null;

			return Decorations[(int)view, (int)point - 1];
		}


		/// <summary>
		/// Get stair
		/// </summary>
		/// <param name="view">Block position in the view field</param>
		/// <returns></returns>
		static public List<TileDrawing> GetStairs(ViewFieldPosition view)
		{
			return Stairs[(int)view];
		}


		/// <summary>
		/// Get pit
		/// </summary>
		/// <param name="view">Block position in the view field</param>
		/// <returns></returns>
		static public TileDrawing GetPit(ViewFieldPosition view)
		{
			return Pits[(int)view];
		}


		/// <summary>
		/// Get ceiling pit
		/// </summary>
		/// <param name="view">Block position in the view field</param>
		/// <returns></returns>
		static public TileDrawing GetCeilingPit(ViewFieldPosition view)
		{
			return CeilingPits[(int)view];
		}


		/// <summary>
		/// Gets floor plate
		/// </summary>
		/// <param name="view">Block position in the view field</param>
		/// <returns></returns>
		static public TileDrawing GetFloorPlate(ViewFieldPosition view)
		{
			return FloorPlates[(int)view];
		}


		/// <summary>
		/// Gets door
		/// </summary>
		/// <param name="view">Block position in the view field</param>
		/// <returns></returns>
		static public TileDrawing GetDoor(ViewFieldPosition view)
		{
			return Doors[(int)view];
		}

		#endregion


		#region IO

		/// <summary>
		/// Loads the maze definition
		/// </summary>
		/// <returns></returns>
		static public bool Load()
		{
			
			// Load file definition
			using (Stream stream = ResourceManager.Load("MazeElements.xml"))
			{
				if (stream == null)
					throw new FileNotFoundException("Can not find maze element coordinate file !!! Aborting.");

				XmlDocument doc = new XmlDocument();
				doc.Load(stream);
				XmlNode xml = doc.DocumentElement;
				if (xml.Name != "displaycoordinate")
				{
					Trace.Write("Wrong header for MazeElements file");
					return false;
				}


				foreach (XmlNode node in xml)
				{
					if (node.NodeType == XmlNodeType.Comment)
						continue;


					switch (node.Name.ToLower())
					{
						case "decoration":
						{
							ViewFieldPosition view = (ViewFieldPosition) Enum.Parse(typeof(ViewFieldPosition), node.Attributes["position"].Value, true);
							CardinalPoint side = (CardinalPoint) Enum.Parse(typeof(CardinalPoint), node.Attributes["side"].Value, true);
							if (side == CardinalPoint.North)
								throw new ArgumentOutOfRangeException("side", "No north wall side decoration !");

							Decorations[(int) view, (int) side - 1] = GetTileDrawing(node);
							//new Point(int.Parse(node.Attributes["x"].Value), int.Parse(node.Attributes["y"].Value));
						}
						break;


						case "wall":
						{
							ViewFieldPosition view = (ViewFieldPosition) Enum.Parse(typeof(ViewFieldPosition), node.Attributes["position"].Value, true);
							Walls[(int) view].Add(GetTileDrawing(node));
						}
						break;


						case "stair":
						{
							ViewFieldPosition view = (ViewFieldPosition) Enum.Parse(typeof(ViewFieldPosition), node.Attributes["position"].Value, true);
							Stairs[(int) view].Add(GetTileDrawing(node));
						}
						break;


						case "ground":
						{
							ViewFieldPosition view = (ViewFieldPosition) Enum.Parse(typeof(ViewFieldPosition), node.Attributes["position"].Value, true);
							SquarePosition ground = (SquarePosition) Enum.Parse(typeof(SquarePosition), node.Attributes["coordinate"].Value, true);

							Ground[(int) view, (int) ground] = new Point(int.Parse(node.Attributes["x"].Value), int.Parse(node.Attributes["y"].Value));
						}
						break;


						case "flyingitem":
						{
							ViewFieldPosition view = (ViewFieldPosition) Enum.Parse(typeof(ViewFieldPosition), node.Attributes["position"].Value, true);
							SquarePosition ground = (SquarePosition) Enum.Parse(typeof(SquarePosition), node.Attributes["coordinate"].Value, true);

							FlyingItems[(int) view, (int) ground] = new Point(int.Parse(node.Attributes["x"].Value), int.Parse(node.Attributes["y"].Value));
						}
						break;


						case "pit":
						{
							ViewFieldPosition view = (ViewFieldPosition) Enum.Parse(typeof(ViewFieldPosition), node.Attributes["position"].Value, true);
							Pits[(int) view] = GetTileDrawing(node);
						}
						break;


						case "ceilingpit":
						{
							ViewFieldPosition view = (ViewFieldPosition) Enum.Parse(typeof(ViewFieldPosition), node.Attributes["position"].Value, true);
							CeilingPits[(int) view] = GetTileDrawing(node);
						}
						break;


						case "floorplate":
						{
							ViewFieldPosition view = (ViewFieldPosition) Enum.Parse(typeof(ViewFieldPosition), node.Attributes["position"].Value, true);
							FloorPlates[(int) view] = GetTileDrawing(node);
						}
						break;

						case "door":
						{
							ViewFieldPosition view = (ViewFieldPosition) Enum.Parse(typeof(ViewFieldPosition), node.Attributes["position"].Value, true);
							Doors[(int) view] = GetTileDrawing(node);
						}
						break;

						default:
						{
							Trace.WriteLine("[MazeDisplayCoordinates] Load() : Unknown element \"" + node.Name + "\".");
						}
						break;

					}
				}

			}

			return true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		static TileDrawing GetTileDrawing(XmlNode node)
		{
			if (node == null)
				return null;

			// Tile id
			int id = int.Parse(node.Attributes["tile"].Value);

			// Location
			Point location = new Point(int.Parse(node.Attributes["x"].Value), int.Parse(node.Attributes["y"].Value));

			// effect
			SpriteEffects effect = SpriteEffects.None;
			if (node.Attributes["effect"] != null)
				effect = (SpriteEffects)Enum.Parse(typeof(SpriteEffects), node.Attributes["effect"].Value);

			return new TileDrawing(id, location, effect);
		}

		#endregion


		#region Properties

		/// <summary>
		/// Pits
		/// </summary>
		static TileDrawing[] Pits;

		/// <summary>
		/// Ceiling pits
		/// </summary>
		static TileDrawing[] CeilingPits;


		/// <summary>
		/// Floorplates
		/// </summary>
		static TileDrawing[] FloorPlates;


		/// <summary>
		/// Doors
		/// </summary>
		static TileDrawing[] Doors;


		/// <summary>
		/// Walls
		/// </summary>
		static List<TileDrawing>[] Walls;


		/// <summary>
		/// Stairs
		/// </summary>
		static List<TileDrawing>[] Stairs;


		/// <summary>
		/// Ground items
		/// </summary>
		static Point[,] Ground;


		/// <summary>
		/// Decorations
		/// </summary>
		static TileDrawing[,] Decorations;


		/// <summary>
		/// Flying items
		/// </summary>
		static Point[,] FlyingItems;


		/// <summary>
		/// Alcoves zone
		/// </summary>
		static public Rectangle Alcove
		{
			get;
			private set;
		}


		/// <summary>
		/// Scroll display zone
		/// </summary>
		static public Rectangle Scroll
		{
			get;
			private set;
		}


		/// <summary>
		/// Scroll Ok button zone
		/// </summary>
		static public Rectangle ScrollOk
		{
			get;
			private set;
		}


		/// <summary>
		/// Front block zone
		/// </summary>
		static public Rectangle FrontSquare
		{
			get;
			private set;
		}


		/// <summary>
		/// Camp button zone
		/// </summary>
		static public Rectangle CampButton
		{
			get;
			private set;
		}


		/// <summary>
		/// Throw left zone
		/// </summary>
		static public Rectangle ThrowLeft
		{
			get;
			private set;
		}


		/// <summary>
		/// Scripted dialog rectangle
		/// </summary>
		static public Rectangle ScriptedDialog
		{
			get;
			private set;
		}


		/// <summary>
		/// Scripted dialog choices rectangle
		/// </summary>
		static public Rectangle[] ScriptedDialogChoices
		{
			get;
			private set;
		}


		/// <summary>
		/// Throw right zone
		/// </summary>
		static public Rectangle ThrowRight
		{
			get;
			private set;
		}


		/// <summary>
		/// Left team ground item zone
		/// </summary>
		static public Rectangle LeftFeetTeam
		{
			get;
			private set;
		}


		/// <summary>
		/// Right team ground item zone
		/// </summary>
		static public Rectangle RightFeetTeam
		{
			get;
			private set;
		}


		/// <summary>
		/// Left front team ground item zone
		/// </summary>
		static public Rectangle LeftFrontTeamGround
		{
			get;
			private set;
		}


		/// <summary>
		/// Right front team ground item zone
		/// </summary>
		static public Rectangle RightFrontTeamGround
		{
			get;
			private set;
		}



		#endregion

	}


	/// <summary>
	/// Interface panel coordinates
	/// </summary>
	static class InterfaceCoord
	{

		#region Main window

		/// <summary>
		/// TurnLeft button
		/// </summary>
		static public Rectangle TurnLeft = new Rectangle(10, 256, 38, 34);

		/// <summary>
		/// Move Forward button
		/// </summary>
		static public Rectangle MoveForward = new Rectangle(48, 256, 40, 34);


		/// <summary>
		/// Turn right button
		/// </summary>
		static public Rectangle TurnRight = new Rectangle(88, 256, 40, 34);


		/// <summary>
		/// Move left button
		/// </summary>
		static public Rectangle MoveLeft = new Rectangle(10, 290, 38, 34);


		/// <summary>
		/// Move backward button
		/// </summary>
		static public Rectangle MoveBackward = new Rectangle(48, 290, 40, 34);


		/// <summary>
		/// Move right button
		/// </summary>
		static public Rectangle MoveRight = new Rectangle(88, 290, 40, 34);


		/// <summary>
		/// Select hero's rectangle
		/// </summary>
		static public Rectangle[] SelectHero = new Rectangle[]
		{
			new Rectangle(368      ,       2, 126, 20),		// Hero 1
			new Rectangle(368 + 144,       2, 126, 20),		// Hero 2

			new Rectangle(368      , 104 + 2, 126, 20),		// Hero 3
			new Rectangle(368 + 144, 104 + 2, 126, 20),		// Hero 4

			new Rectangle(368      , 208 + 2, 126, 20),		// Hero 5
			new Rectangle(368 + 144, 208 + 2, 126, 20),		// Hero 6
		};


		/// <summary>
		/// Hero's Face rectangle
		/// </summary>
		static public Rectangle[] HeroFace = new Rectangle[]
		{
			new Rectangle(368      ,       22, 64, 64),		// Hero 1
			new Rectangle(368 + 144,       22, 64, 64),		// Hero 2

			new Rectangle(368      , 104 + 22, 64, 64),		// Hero 3
			new Rectangle(368 + 144, 104 + 22, 64, 64),		// Hero 4
			
			new Rectangle(368      , 208 + 22, 64, 64),		// Hero 5
			new Rectangle(368 + 144, 208 + 22, 64, 64),		// Hero 6
		};


		/// <summary>
		/// Hero's primary hand rectangle
		/// </summary>
		static public Rectangle[] PrimaryHand = new Rectangle[]
		{
			new Rectangle(432      ,       22, 62, 32),		// Hero 1
			new Rectangle(432 + 144,       22, 62, 32),		// Hero 2

			new Rectangle(432      , 104 + 22, 62, 32),		// Hero 3
			new Rectangle(432 + 144, 104 + 22, 62, 32),		// Hero 4

			new Rectangle(432      , 208 + 22, 62, 32),		// Hero 5
			new Rectangle(432 + 144, 208 + 22, 60, 32),		// Hero 6
		};


		/// <summary>
		/// Hero's primary hand rectangle
		/// </summary>
		static public Rectangle[] SecondaryHand = new Rectangle[]
		{
			new Rectangle(432      ,       54, 62, 32),		// Hero 1
			new Rectangle(432 + 144,       54, 62, 32),		// Hero 2

			new Rectangle(432      , 104 + 54, 62, 32),		// Hero 3
			new Rectangle(432 + 144, 104 + 54, 62, 32),		// Hero 4

			new Rectangle(432      , 208 + 54, 62, 32),		// Hero 5
			new Rectangle(432 + 144, 208 + 54, 62, 32),		// Hero 6
		};


		#endregion



		#region Inventory screen

		/// <summary>
		/// Close inventory button
		/// </summary>
		static public Rectangle CloseInventory = new Rectangle(360, 4, 64, 64);


		/// <summary>
		/// Show statistics button
		/// </summary>
		static public Rectangle ShowStatistics = new Rectangle(602, 296, 36, 36);


		/// <summary>
		/// Close inventory button
		/// </summary>
		static public Rectangle PreviousHero = new Rectangle(546, 68, 40, 30);


		/// <summary>
		/// Close inventory button
		/// </summary>
		static public Rectangle NextHero = new Rectangle(592, 68, 40, 30);


		/// <summary>
		/// Backpack buttons
		/// </summary>
		static public Rectangle[] BackPack = new Rectangle[]
		{
			new Rectangle(358     , 76     ,  36, 36),
			new Rectangle(358 + 36, 76     ,  36, 36),
			new Rectangle(358     , 76 + 36,  36, 36),
			new Rectangle(358 + 36, 76 + 36,  36, 36),
			new Rectangle(358     , 76 + 72,  36, 36),
			new Rectangle(358 + 36, 76 + 72,  36, 36),
			new Rectangle(358     , 76 + 108, 36, 36),
			new Rectangle(358 + 36, 76 + 108, 36, 36),
			new Rectangle(358     , 76 + 144, 36, 36),
			new Rectangle(358 + 36, 76 + 144, 36, 36),
			new Rectangle(358     , 76 + 180, 36, 36),
			new Rectangle(358 + 36, 76 + 180, 36, 36),
			new Rectangle(358     , 76 + 216, 36, 36),
			new Rectangle(358 + 36, 76 + 216, 36, 36),
		};

		/// <summary>
		/// Rings buttons
		/// </summary>
		static public Rectangle[] Rings = new Rectangle[]
		{
			new Rectangle(452     , 268, 20, 20),
			new Rectangle(452 + 24, 268, 20, 20),
		};


		/// <summary>
		/// Belt buttons
		/// </summary>
		static public Rectangle[] Waist = new Rectangle[]
		{
			new Rectangle(596, 184     , 36, 36),
			new Rectangle(596, 184 + 36, 36, 36),
			new Rectangle(596, 184 + 72, 36, 36),
		};


		/// <summary>
		/// Quiver button
		/// </summary>
		static public Rectangle Quiver = new Rectangle(446, 108, 36, 36);


		/// <summary>
		/// Quiver button
		/// </summary>
		static public Rectangle Armor = new Rectangle(444, 148, 36, 36);


		/// <summary>
		/// Food button
		/// </summary>
		static public Rectangle Food = new Rectangle(470, 72, 62, 30);


		/// <summary>
		/// Wrists button
		/// </summary>
		static public Rectangle Wrist = new Rectangle(446, 188, 36, 36);


		/// <summary>
		/// Primary Hand inventory button
		/// </summary>
		static public Rectangle PrimaryHandInventory = new Rectangle(456, 228, 36, 36);


		/// <summary>
		/// Feet button
		/// </summary>
		static public Rectangle Feet = new Rectangle(550, 270, 36, 36);


		/// <summary>
		/// Food button
		/// </summary>
		static public Rectangle SecondaryHandInventory = new Rectangle(552, 228, 36, 36);


		/// <summary>
		/// Neck button
		/// </summary>
		static public Rectangle Neck = new Rectangle(570, 146, 36, 36);


		/// <summary>
		/// Head button
		/// </summary>
		static public Rectangle Head = new Rectangle(592, 106, 36, 36);

		#endregion
	}


	/// <summary>
	/// Location on the screen of a tile
	/// </summary>
	/// <remarks>This class is used for drawing maze blocks</remarks>
	public class TileDrawing
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">ID of the tile</param>
		/// <param name="location">Location</param>
		public TileDrawing(int id, Point location) : this(id, location, SpriteEffects.None)
		{
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">ID of the tile</param>
		/// <param name="location">Location</param>
		/// <param name="effect">Display effect</param>
		public TileDrawing(int id, Point location, SpriteEffects effect)
		{
			ID = id;
			Location = location;
			Effect = effect;
		}

		/// <summary>
		/// Tile id
		/// </summary>
		public int ID
		{
			get;
			private set;
		}

		/// <summary>
		/// Location on the screen
		/// </summary>
		public Point Location
		{
			get;
			private set;
		}

		/// <summary>
		/// Display effect
		/// </summary>
		public SpriteEffects Effect
		{
			get;
			private set;
		}

	}



}
