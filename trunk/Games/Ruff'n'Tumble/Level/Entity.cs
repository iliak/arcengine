#region Licence
//
//This file is part of ArcEngine.
//Copyright (C)2008-2009 Adrien H�mery ( iliak@mimicprod.net )
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
using ArcEngine;
using ArcEngine.Games.RuffnTumble.Interface;
using System.ComponentModel;
using System;
using System.Drawing;
using System.Xml;
using ArcEngine.Graphic;
using ArcEngine.Asset;

//
//- Colision detection
//  http://supertux.lethargik.org/wiki/Collision_detection
//  http://www.harveycartel.org/metanet/tutorials/tutorialA.html
//  http://www.indielib.com/wiki/index.php?title=Tutorial_08_Collisions
//
//
//
//
//
//
//  Current_Annimation  Get Or Set The Current Animation 
//  Current_Frame  Get Or Set The Curent Frame 
//
//
//
//
namespace ArcEngine.Games.RuffnTumble.Asset
{

	/// <summary>
	/// 
	/// </summary>
	public class Entity// : ResourceBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">Name of the entity</param>
		/// <param name="layer">Layer of the entity</param>
		public Entity(Layer layer)// : base(name)
		{
			parentLayer = layer;
		}


		/// <summary>
		/// Initialize the entity
		/// </summary>
		/// <returns>Success or not</returns>
		public bool Init()
		{

			Model = ResourceManager.CreateAsset<Model>(ModelName);


			if (ScriptInterface != null)
				return ScriptInterface.Init(this);



			return true;
		}


		#region IO


		/// <summary>
		/// Saves the entity
		/// </summary>
		/// <param name="xml">xml node</param>
		/// <returns></returns>
		public bool Save(XmlWriter xml)
		{

			xml.WriteStartElement("entity");
	//		xml.WriteAttributeString("name", Name);


	//		base.SaveComment(xml);


			xml.WriteStartElement("location");
			xml.WriteAttributeString("x", location.X.ToString());
			xml.WriteAttributeString("y", location.Y.ToString());
			xml.WriteEndElement();


			xml.WriteStartElement("model");
			xml.WriteAttributeString("name", ModelName);
			xml.WriteEndElement();
			xml.WriteEndElement();


			if (ScriptInterface != null)
				ScriptInterface.Save(this, xml);

			return true;
		}



		/// <summary>
		/// Loads the entity
		/// </summary>
		/// <param name="xml">xml node</param>
		/// <returns></returns>
		public bool Load(XmlNode xml)
		{
			foreach (XmlNode node in xml)
			{
				if (node.NodeType == XmlNodeType.Comment)
				{
				//	base.LoadComment(node);
					continue;
				}

				switch (node.Name.ToLower())
				{
					case "location":
					{
						location = new Point(Int32.Parse(node.Attributes["x"].Value), Int32.Parse(node.Attributes["y"].Value));
					}
					break;

					case "model":
					{
						ModelName = node.Attributes["name"].Value;
					}
					break;

					default:
					{
						Trace.WriteLine("Entity : Unknown node element found (" + node.Name + ")");
					}
					break;
				}
			}


			if (ScriptInterface != null)
				ScriptInterface.Load(this, xml);

			return true;
		}

		#endregion


		#region Rendering  & update

		/// <summary>
		/// Update the entity
		/// </summary>
		public void Update(GameTime time)
		{
			// If script present, then call it's Update method
			if (ScriptInterface != null)
				ScriptInterface.Update(this, time);
		}


		/// <summary>
		/// Renders the entity
		/// </summary>
		///<param name="loc">Location of the entity relative to the layer coordinate</param>
		public void Draw(Point loc)
		{
			if (tileSet != null)
			{
				tileSet.Scale = zoom;
				//tileSet.Draw(TileId, loc);
				tileSet.Draw(tileId, loc);
			}


			// Is entity in debug mode ?
			if (debug)
			{
                Rectangle coll = CollisionBoxLocation;
                coll.Location = parentLayer.Level.LevelToScreen(coll.Location);
					 Display.Rectangle(coll, false);
			}

		}

		#endregion


		#region Properties


		/// <summary>
		/// Entity TileSet name
		/// </summary>
		[TypeConverter(typeof(TileSetEnumerator))]
		public string TileSetName
		{
			get
			{
				//if (tileSet != null) return tileSet.Name;
				//else return null;

				return string.Empty;
			}
			set
			{
				//if (tileSet != null)
				//   tileSet.UnlockResource();

				tileSet = ResourceManager.CreateAsset<TileSet>(value);
				//if (tileSet != null)
				//   tileSet.LockResource();
			}

		}


		/// <summary>
		/// Gets the current tileset of the entity
		/// </summary>
		[Browsable(false)]
		public TileSet TileSet
		{
			get
			{
				return tileSet;
			}
		}
		TileSet tileSet;


		/// <summary>
		/// Gets/sets the current sequence
		/// </summary>
		public int TileId
		{
			get
			{
				return tileId;
			}
			set
			{
				tileId = value;

				if (tileSet != null)
					tile = tileSet.GetTile(tileId);
				else
					tile = null;
			}
		}
		int tileId;


		/// <summary>
		/// Current tile
		/// </summary>
		[Browsable(false)]
		public Tile Tile
		{
			get
			{
				if (tile == null)
					return null;

				return tile;
			}
		}
		Tile tile;

		/// <summary>
		/// Gets the original collision box (without zoom)
		/// </summary>
		[Browsable(false)]
		public Rectangle CollisionBox
		{
			get
			{
				if (Tile == null)
					return Rectangle.Empty;

				Rectangle coll = Tile.CollisionBox;
				//coll.Width *= (int)zoom.Width;
				//coll.Height *= (int)zoom.Height;

				return coll;
			}
		}


		/// <summary>
		/// Gets the location of the collision box of the entity (CollisionBox translated
		/// to the entity location in screen coordinate)
		/// </summary>
		[Browsable(false)]
		public Rectangle CollisionBoxLocation
		{
			get
			{
				if (Tile == null)
					return Rectangle.Empty;


				return new Rectangle(
					Location.X - (int) (Tile.HotSpot.X * Zoom.Width) + (int)(CollisionBox.X * Zoom.Width),
					Location.Y - (int) (Tile.HotSpot.Y * Zoom.Height) + (int)(CollisionBox.Y * Zoom.Height),
					(int) (Tile.CollisionBox.Width * Zoom.Width),
					(int) (Tile.CollisionBox.Height * Zoom.Height));
			}
		}


								

		/// <summary>
		/// Entity model reference
		/// </summary>
		[Description("Model name to use")]
		[Category("Model")]
		[TypeConverter(typeof(ModelEnumerator))]
		public string ModelName
		{
			get
			{
				return modelName;
			}
			set
			{
				//Model = ResourceManager.GetModel(value);
				modelName = value;

				Model = ResourceManager.CreateAsset<Model>(value);
			}
		}
		string modelName;




		/// <summary>
		/// Handle to the model
		/// </summary>
		[Browsable(false)]
		public Model Model
		{
			get
			{
				return model;
			}
			private set
			{
				model = value;
				if (model == null) return;

				// tileset
				tileSet = ResourceManager.CreateAsset<TileSet>(model.TileSetName);

				// script
				Script script = ResourceManager.CreateAsset<Script>(model.ScriptName);
				if (script != null)
				{
					//if (script.Compile())
					//	ScriptInterface = (IEntity)script.FindInterface("RuffnTumble.Interface.IEntity");
				}

				// zoom
				zoom = model.Zoom;

				// Movements
				Velocity = model.MaxVelocity;
				JumpHeight = model.JumpHeight;
				Gravity = model.Gravity;
			}
		}
		Model model;




		/// <summary>
		/// Zoom factor of the entity
		/// </summary>
		[Category("Display")]
		[Description("Zoom factor of the entity")]
		public SizeF Zoom
		{
			get
			{
				if (zoom.IsEmpty)
					return new SizeF(1.0f, 1.0f);
				else
					return zoom;
			}
			set
			{
				zoom = value;
			}
		}
		SizeF zoom = new SizeF(1,1);

									

		/// <summary>
		/// Compiled script handle
		/// </summary>
		IEntity ScriptInterface;


		/// <summary>
		/// The layer where the entity is
		/// </summary>
		[Browsable(false)]
		public Layer ParentLayer
		{
			get
			{
				return parentLayer;
			}
		}
		Layer parentLayer;



		/// <summary>
		/// Gets / sets the entity debug mode
		/// </summary>
		[Browsable(false)]
		public bool Debug
		{
			get
			{
				return debug;
			}
			set
			{
				debug = value;
			}
		}
		bool debug;




		/// <summary>
		/// Gets/sets the entity a god (cheating)
		/// </summary>
		[Category("Cheat")]
		[Description("Entity is like a God")]
		public bool God
		{
			get
			{
				return god;
			}
			set
			{
				god = value;
			}
		}
		bool god;


		/// <summary>
		/// Gets the hotspot of the entity
		/// </summary>
		[Browsable(false)]
		public Point HotSpot
		{
			get
			{
				if (Tile == null)
					return Point.Empty;

				return Tile.HotSpot;
			}
		}




		/// <summary>
		/// Gets/sets the maxVelocity of the entity
		/// (pixels per second)
		/// </summary>
		[Browsable(false)]
		public int Velocity
		{
			get
			{
				return velocity;
			}
			set
			{
				velocity = value;
			}
		}
		int velocity;



		/// <summary>
		/// Gets / sets entity location in the layer.
		/// </summary>
		[Category("Entity")]
		[Description("Offset of the entity")]
		public Point Location
		{
			get
			{
				return location;
			}
			set
			{
				location = value;
			}
		}
		Point location;



		/// <summary>
		/// Gets / sets the acceleration
		/// </summary>
		public int Acceleration
		{
			get
			{
				return acceleration;
			}
			set
			{
				acceleration = value;
			}
		}
		int acceleration;

									


		/// <summary>
		/// Gravity of the entity
		/// (pixels per second)
		/// </summary>
		[Browsable(false)]
		public Point Gravity
		{
			get
			{
				return gravity;
			}
			set
			{
				gravity = value;
			}
		}
		Point gravity;

									


		/// <summary>
		/// Initial jump height in pixels
		/// </summary>
		[Browsable(false)]
		public int JumpHeight
		{
			get
			{
				return jumpHeight;
			}
			set
			{
				jumpHeight = value;
			}
		}
		int jumpHeight;



		/// <summary>
		/// Is the entity is jumping
		/// </summary>
		[Browsable(false)]
		public bool IsJumping
		{
			get
			{
				return isJumping;
			}
            internal set
            {
                isJumping = value;
            }
		}
		bool isJumping;



		/// <summary>
		/// Is entity falling ?
		/// </summary>
		[Browsable(false)]
		public bool IsFalling
		{
			get
			{
				return isFalling;
			}
			set
			{
				isFalling = value;
			}
		}
		bool isFalling = false;

									

		/// <summary>
		/// Current jump height of the entity
		/// </summary>
		[Browsable(false)]
		public int Jump
		{
			get
			{
				return jump;
			}
			set
			{
				jump = value;
//				if (jump > parentLayer.BlockSize.Height * (int)parentLayer.Zoom.Height)
//					jump = parentLayer.BlockSize.Height * (int)parentLayer.Zoom.Height;

				if (jump < 0)
					jump = 0;
			}
		}
		int jump;

									

		#endregion
	}





}
