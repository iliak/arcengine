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
using System.Windows.Forms;
using System.Xml;
using ArcEngine;
using ArcEngine.Asset;
using ArcEngine.Graphic;
using ArcEngine.Input;
using ArcEngine.Utility.ScreenManager;
using DungeonEye.Gui;


namespace DungeonEye
{

	/// <summary>
	/// Dungeon crawler game screen class
	/// </summary>
	public class GameScreen : GameScreenBase
	{

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="heroes">Heroes in the team</param>
		public GameScreen(Hero[] heroes)
		{

			SpellBook = new SpellBook();

			DrawHPAsBar = true;

		}


		/// <summary>
		/// Initialize the team
		/// </summary>
		public override void LoadContent()
		{
			DrawHPAsBar = Settings.GetBool("HPAsBar");

			Batch = new SpriteBatch();

			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
			watch.Start();

			Trace.WriteLine("Content loaded ({0} ms)", watch.ElapsedMilliseconds);

			// Keyboard input scheme
			InputScheme = ResourceManager.CreateAsset<InputScheme>(Game.InputSchemeName);
			if (InputScheme == null)
			{
				Trace.WriteLine("ERROR !!! No InputSchema detected !!!");
				InputScheme = new InputScheme();
				InputScheme["MoveForward"] = Keys.Z;
				InputScheme["MoveBackward"] = Keys.S;
				InputScheme["StrafeLeft"] = Keys.Q;
				InputScheme["StrafeRight"] = Keys.D;
				InputScheme["TurnLeft"] = Keys.A;
				InputScheme["TurnRight"] = Keys.E;
				InputScheme["Inventory"] = Keys.I;
				InputScheme["SelectHero1"] = Keys.D1;
				InputScheme["SelectHero2"] = Keys.D2;
				InputScheme["SelectHero3"] = Keys.D3;
				InputScheme["SelectHero4"] = Keys.D4;
				InputScheme["SelectHero5"] = Keys.D5;
				InputScheme["SelectHero6"] = Keys.D6;
			}
			Trace.WriteLine("InputScheme ({0} ms)", watch.ElapsedMilliseconds);


			// Interface tileset
			TileSet = ResourceManager.CreateSharedAsset<TileSet>("Interface", "Interface");

			// Heroe's heads
			Heads = ResourceManager.CreateAsset<TileSet>("Heads");

			// Items tileset
			Items = ResourceManager.CreateAsset<TileSet>("Items");

			// Fonts
			Font = ResourceManager.CreateSharedAsset<BitmapFont>("inventory", "inventory");
			OutlinedFont = ResourceManager.CreateSharedAsset<BitmapFont>("outline", "outline");

			// Misc init
			SpellBook.LoadContent();
			GameMessage.Init();


			// Loads a saved game
			if (!LoadParty())
			{
				Team.Init();
			}


			watch.Stop();
			Trace.WriteLine("Team::LoadContent() finished ! ({0} ms)", watch.ElapsedMilliseconds);
		}


		/// <summary>
		/// Dispose
		/// </summary>
		public override void UnloadContent()
		{
			Trace.WriteDebugLine("[Team] : UnloadContent");

			Team.Dispose();
			SpellBook.Dispose();
			SpellBook = null;

			if (OutlinedFont != null)
				OutlinedFont.Dispose();
			OutlinedFont = null;

			if (Font != null)
				Font.Dispose();
			Font = null;

			if (Items != null)
				Items.Dispose();
			Items = null;

			if (Heads != null)
				Heads.Dispose();
			Heads = null;

			ResourceManager.UnlockSharedAsset<TileSet>(TileSet);
			TileSet = null;

			if (Batch != null)
				Batch.Dispose();
			Batch = null;


			Dialog = null;
			InputScheme = null;
		}


		#region IO


		/// <summary>
		/// Loads a team party
		/// </summary>
		/// <param name="filename">File name to load</param>
		/// <returns>True if loaded</returns>
		public bool LoadParty()
		{

			Team.LoadParty();
			GameMessage.AddMessage("Party Loaded...", GameColors.Yellow);
			return true;
		}


		/// <summary>
		/// Saves a party progress
		/// </summary>
		/// <param name="filename">File name</param>
		/// <returns></returns>
		public bool SaveParty(string filename)
		{
			try
			{
				Team.SaveParty();
				GameMessage.AddMessage("Party saved...", GameColors.Yellow);
			}
			catch (Exception e)
			{
				Trace.WriteLine("[Team] SaveParty() : Failed to save the party (filename = '{0}') => {1} !", filename, e.Message);
				GameMessage.AddMessage("Party NOT saved...", GameColors.Red);
				return false;
			}
			return true;
		}



		#endregion


		#region Draws



		/// <summary>
		/// Display team informations
		/// </summary>
		public override void Draw()
		{
			Display.ClearBuffers();

			Batch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, false);


			// Draw the current maze
			if (Team.Maze != null)
				Team.Maze.Draw(Batch, Team.Location);

			
			// The interface
			Batch.DrawTile(TileSet, 0, Point.Empty);


			// Display the compass
			Batch.DrawTile(TileSet, 5 + (int)Team.Location.Direction * 3, new Point(228, 262));
			Batch.DrawTile(TileSet, 6 + (int)Team.Location.Direction * 3, new Point(158, 316));
			Batch.DrawTile(TileSet, 7 + (int)Team.Location.Direction * 3, new Point(302, 316));


			// Ingame interfaces
			if (Interface == TeamInterface.Inventory)
				DrawInventory(Batch);

			else if (Interface == TeamInterface.Statistic)
				DrawStatistics(Batch);

			else
			{
				DrawMain(Batch);


				// Action zones
				//for (int id = 0 ; id < 6 ; id++)
				//{
				//    Batch.FillRectangle(InterfaceCoord.PrimaryHand[id], Color.FromArgb(128, Color.Red));
				//    Batch.FillRectangle(InterfaceCoord.SecondaryHand[id], Color.FromArgb(128, Color.Red));
				//    Batch.FillRectangle(InterfaceCoord.SelectHero[id], Color.FromArgb(128, Color.Red));
				//    Batch.FillRectangle(InterfaceCoord.HeroFace[id], Color.FromArgb(128, Color.Red));
				//}
				//Batch.FillRectangle(InterfaceCoord.TurnLeft, Color.FromArgb(128, Color.Red));
				//Batch.FillRectangle(InterfaceCoord.TurnRight, Color.FromArgb(128, Color.Red));
				//Batch.FillRectangle(InterfaceCoord.MoveForward, Color.FromArgb(128, Color.Red));
				//Batch.FillRectangle(InterfaceCoord.MoveBackward, Color.FromArgb(128, Color.Red));
				//Batch.FillRectangle(InterfaceCoord.MoveLeft, Color.FromArgb(128, Color.Red));
				//Batch.FillRectangle(InterfaceCoord.MoveRight, Color.FromArgb(128, Color.Red));
			}


			// Draw game messages
			GameMessage.Draw(Batch, Font);


			// Draw the spell window
			SpellBook.Draw(Batch);


			// Draws the dialog window
			if (Dialog != null)
				Dialog.Draw(Batch);


			Batch.End();
		}




		/// <summary>
		/// Draws the right side of the panel
		/// </summary>
		/// <param name="batch"></param>
		private void DrawMain(SpriteBatch batch)
		{
			// Draw heroes
			Point pos;
			for (int y = 0 ; y < 3 ; y++)
			{
				for (int x = 0 ; x < 2 ; x++)
				{
					Hero hero = Team.Heroes[y * 2 + x];
					if (hero == null)
						continue;

					pos = new Point(366 + 144 * x, y * 104 + 2);

					// Backdrop
					batch.DrawTile(TileSet, 17, pos);

					// Head
					if (hero.IsDead)
						batch.DrawTile(TileSet, 4, new Point(pos.X + 2, pos.Y + 20));
					else
						batch.DrawTile(Heads, hero.Head, new Point(pos.X + 2, pos.Y + 20));

					// Hero uncouncious
					if (hero.IsUnconscious)
						batch.DrawTile(TileSet, 2, new Point(pos.X + 4, pos.Y + 20));

					// Name
					if (HeroToSwap == hero)
					{
						batch.DrawString(Font, new Point(pos.X + 6, pos.Y + 6), GameColors.Red, " Swapping");
					}
					else if (Team.SelectedHero == hero)
					{
						batch.DrawString(Font, new Point(pos.X + 6, pos.Y + 6), GameColors.White, hero.Name);
					}
					else
					{
						batch.DrawString(Font, new Point(pos.X + 6, pos.Y + 6), GameColors.Black, hero.Name);
					}

					// HP
					if (DrawHPAsBar)
					{
						float percent = (float) hero.HitPoint.Current / (float) hero.HitPoint.Max;
						Color color = GameColors.Green;
						if (percent < 0.15)
							color = GameColors.Red;
						else if (percent < 0.4)
							color = GameColors.Yellow;

						batch.DrawString(Font, new Point(pos.X + 6, pos.Y + 88), GameColors.Black, "HP");
						DrawProgressBar(batch, hero.HitPoint.Current, hero.HitPoint.Max, new Rectangle(pos.X + 30, pos.Y + 88, 92, 10), color);
					}
					else
						batch.DrawString(Font, new Point(pos.X + 6, pos.Y + 88), GameColors.Black, hero.HitPoint.Current + " of " + hero.HitPoint.Max);


					// Hands
					for (int i = 0 ; i < 2 ; i++)
					{
						HeroHand hand = (HeroHand) i;
						int yoffset = i * 32;

						// Primary
						Item item = hero.GetInventoryItem(hand == HeroHand.Primary ? InventoryPosition.Primary : InventoryPosition.Secondary);
						batch.DrawTile(Items, item != null ? item.TileID : 86, new Point(pos.X + 96, pos.Y + 36 + yoffset));

						if (!hero.CanUseHand(hand))
							batch.DrawTile(TileSet, 3, new Point(pos.X + 66, pos.Y + 20 + yoffset));


						// Hero hit a monster a few moment ago
						Attack attack = hero.GetLastAttack(hand);
						if (attack != null)
						{

							if (!hero.CanUseHand(hand))
							{
								// Ghost item
								batch.DrawTile(TileSet, 3, new Point(pos.X + 66, pos.Y + 20 + yoffset));

								// Monster hit ?
								if (attack.Target != null && !attack.IsOutdated(DateTime.Now, 1000))
								{
									batch.DrawTile(TileSet, 21, new Point(pos.X + 64, pos.Y + 20 + yoffset));

									if (attack.IsAHit)
										batch.DrawString(Font, new Point(pos.X + 90, pos.Y + 32 + yoffset), GameColors.White, attack.Hit.ToString());
									else if (attack.IsAMiss)
										batch.DrawString(Font, new Point(pos.X + 76, pos.Y + 32 + yoffset), GameColors.White, "MISS");
								}
							}


						}



						HandAction action = hero.GetLastActionResult(hand);
						if (action.Result != ActionResult.Ok && !action.IsOutdated(DateTime.Now, 1000))
						{
							batch.DrawTile(TileSet, 22, new Point(pos.X + 66, pos.Y + 20 + yoffset));

							switch (action.Result)
							{
								case ActionResult.NoAmmo:
								{
									batch.DrawString(Font, new Point(pos.X + 86, pos.Y + 24 + yoffset), GameColors.White, "NO");
									batch.DrawString(Font, new Point(pos.X + 74, pos.Y + 38 + yoffset), GameColors.White, "AMMO");
								}
								break;
								case ActionResult.CantReach:
								{
									batch.DrawString(Font, new Point(pos.X + 68, pos.Y + 24 + yoffset), GameColors.White, "CAN'T");
									batch.DrawString(Font, new Point(pos.X + 68, pos.Y + 38 + yoffset), GameColors.White, "REACH");
								}
								break;
							}
						}
					}


					// Dead or uncounscious
					if (hero.IsUnconscious || hero.IsDead)
					{
						batch.DrawTile(TileSet, 3, new Point(pos.X + 66, pos.Y + 52));
						batch.DrawTile(TileSet, 3, new Point(pos.X + 66, pos.Y + 20));
					}


					// Hero was hit
					if (hero.LastAttack != null && !hero.LastAttack.IsOutdated(DateTime.Now, 1000))
					{
						batch.DrawTile(TileSet, 20, new Point(pos.X + 24, pos.Y + 66));
						batch.DrawString(Font, new Point(pos.X + 52, pos.Y + 86), GameColors.White, hero.LastAttack.Hit.ToString());
					}

				}
			}


			// Mini map
			if (Debug)
			{
				Team.Maze.DrawMiniMap(batch, this, new Point(500, 220));

				// Team location
				batch.DrawString(Font, new Point(10, 340), GameColors.White, Team.Location.ToString());
			}
		}




		/// <summary>
		/// Draws a progress bar
		/// </summary>
		/// <param name="batch">SpriteBatch to use</param>
		/// <param name="value">Current value</param>
		/// <param name="max">Maximum value</param>
		/// <param name="rectangle">Rectangle</param>
		/// <param name="color">Bar color</param>
		public void DrawProgressBar(SpriteBatch batch, int value, int max, Rectangle rectangle, Color color)
		{
			if (value > 0)
			{
				Vector4 zone = new Vector4(
					rectangle.Left + 1,
					rectangle.Top + 1,
					((float) value / (float) max * (rectangle.Width - 1)),
					rectangle.Height - 2
					);
				batch.FillRectangle(zone, color);
			}

			batch.DrawLine(rectangle.Left, rectangle.Top, rectangle.Left, rectangle.Bottom, GameColors.LightBlue);
			batch.DrawLine(rectangle.Left, rectangle.Bottom, rectangle.Right + 2, rectangle.Bottom, GameColors.LightBlue);


			batch.DrawLine(rectangle.Left + 1, rectangle.Top, rectangle.Right + 1, rectangle.Top, GameColors.DarkBlue);
			batch.DrawLine(rectangle.Right + 1, rectangle.Top, rectangle.Right + 1, rectangle.Bottom, GameColors.DarkBlue);
		}



		/// <summary>
		/// Draws the inventory
		/// </summary>
		/// <param name="batch"></param>
		void DrawInventory(SpriteBatch batch)
		{
			// Background
			batch.DrawTile(TileSet, 18, new Point(352, 0));

			// Name
			batch.DrawString(OutlinedFont, new Point(430, 12), GameColors.White, Team.SelectedHero.Name);

			// HP and Food
			batch.DrawString(Font, new Point(500, 30), GameColors.Black, Team.SelectedHero.HitPoint.Current + " of " + Team.SelectedHero.HitPoint.Max);

			// Dead or uncounscious
			if (Team.SelectedHero.IsUnconscious)
			{
				batch.DrawString(OutlinedFont, new Point(450, 316), GameColors.Yellow, "UNCONSCIOUS");
				batch.DrawTile(TileSet, 2, new Point(360, 4));
			}
			else if (Team.SelectedHero.IsDead)
			{
				batch.DrawString(OutlinedFont, new Point(500, 316), GameColors.Red, "DEAD");
				batch.DrawTile(TileSet, 4, new Point(360, 4));
			}
			else
				batch.DrawTile(Heads, Team.SelectedHero.Head, new Point(360, 4));


			// Food
			if (Team.SelectedHero.Food > 0)
			{
				Color color;
				if (Team.SelectedHero.Food > 50)
					color = GameColors.Green;
				else if (Team.SelectedHero.Food > 25)
					color = GameColors.Yellow;
				else
					color = GameColors.Red;

				batch.FillRectangle(new Rectangle(500, 48, Team.SelectedHero.Food, 10), color);
			}

			// Draw inventory
			int pos = 0;
			for (int y = 94 ; y < 346 ; y += 36)
				for (int x = 376 ; x < 448 ; x += 36)
				{
					if (Team.SelectedHero.GetBackPackItem(pos) != null)
						batch.DrawTile(Items, Team.SelectedHero.GetBackPackItem(pos).TileID, new Point(x, y));

					pos++;
				}


			// Quiver count
			if (Team.SelectedHero.Quiver > 99)
				batch.DrawString(Font, new Point(452, 128), GameColors.White, "++");
			else
				batch.DrawString(Font, new Point(452, 128), GameColors.White, Team.SelectedHero.Quiver.ToString());

			// Armor
			if (Team.SelectedHero.GetInventoryItem(InventoryPosition.Armor) != null)
				batch.DrawTile(Items, Team.SelectedHero.GetInventoryItem(InventoryPosition.Armor).TileID, new Point(462, 166));

			// Wrists
			if (Team.SelectedHero.GetInventoryItem(InventoryPosition.Wrist) != null)
				batch.DrawTile(Items, Team.SelectedHero.GetInventoryItem(InventoryPosition.Wrist).TileID, new Point(464, 206));

			// Primary
			if (Team.SelectedHero.GetInventoryItem(InventoryPosition.Primary) != null)
				batch.DrawTile(Items, Team.SelectedHero.GetInventoryItem(InventoryPosition.Primary).TileID, new Point(474, 244));

			// Fingers 1
			if (Team.SelectedHero.GetInventoryItem(InventoryPosition.Ring_Left) != null)
				batch.DrawTile(Items, Team.SelectedHero.GetInventoryItem(InventoryPosition.Ring_Left).TileID, new Point(462, 278));

			// Fingers 2
			if (Team.SelectedHero.GetInventoryItem(InventoryPosition.Ring_Right) != null)
				batch.DrawTile(Items, Team.SelectedHero.GetInventoryItem(InventoryPosition.Ring_Right).TileID, new Point(486, 278));

			// Feet
			if (Team.SelectedHero.GetInventoryItem(InventoryPosition.Feet) != null)
				batch.DrawTile(Items, Team.SelectedHero.GetInventoryItem(InventoryPosition.Feet).TileID, new Point(568, 288));

			// Secondary
			if (Team.SelectedHero.GetInventoryItem(InventoryPosition.Secondary) != null)
				batch.DrawTile(Items, Team.SelectedHero.GetInventoryItem(InventoryPosition.Secondary).TileID, new Point(568, 246));

			// Back 1 598,184,36,36
			if (Team.SelectedHero.GetWaistPackItem(0) != null)
				batch.DrawTile(Items, Team.SelectedHero.GetWaistPackItem(0).TileID, new Point(614, 202));

			// Back 2 598,220,36,36
			if (Team.SelectedHero.GetWaistPackItem(1) != null)
				batch.DrawTile(Items, Team.SelectedHero.GetWaistPackItem(1).TileID, new Point(614, 238));

			// Back 3 598,256,36,36
			if (Team.SelectedHero.GetWaistPackItem(2) != null)
				batch.DrawTile(Items, Team.SelectedHero.GetWaistPackItem(2).TileID, new Point(614, 272));

			// Neck 572,146,36,36
			if (Team.SelectedHero.GetInventoryItem(InventoryPosition.Neck) != null)
				batch.DrawTile(Items, Team.SelectedHero.GetInventoryItem(InventoryPosition.Neck).TileID, new Point(588, 164));

			// Head 594,106,36,36
			if (Team.SelectedHero.GetInventoryItem(InventoryPosition.Helmet) != null)
				batch.DrawTile(Items, Team.SelectedHero.GetInventoryItem(InventoryPosition.Helmet).TileID, new Point(610, 124));

			/* 
						// Debug draw
						Batch.FillRectangle(InterfaceCoord.PreviousHero, Color.FromArgb(128, Color.Red));
						Batch.FillRectangle(InterfaceCoord.CloseInventory, Color.FromArgb(128, Color.Red));
						Batch.FillRectangle(InterfaceCoord.NextHero, Color.FromArgb(128, Color.Red));
						Batch.FillRectangle(InterfaceCoord.ShowStatistics, Color.FromArgb(128, Color.Red));
						foreach (Rectangle rectangle in InterfaceCoord.BackPack)
							Batch.FillRectangle(rectangle, Color.FromArgb(128, Color.Red));
						foreach (Rectangle rectangle in InterfaceCoord.Rings)
							Batch.FillRectangle(rectangle, Color.FromArgb(128, Color.Red));
						foreach (Rectangle rectangle in InterfaceCoord.Belt)
							Batch.FillRectangle(rectangle, Color.FromArgb(128, Color.Red));

						Batch.FillRectangle(InterfaceCoord.Head, Color.FromArgb(128, Color.Red));
						Batch.FillRectangle(InterfaceCoord.Neck, Color.FromArgb(128, Color.Red));
						Batch.FillRectangle(InterfaceCoord.SecondaryHandInventory, Color.FromArgb(128, Color.Red));
						Batch.FillRectangle(InterfaceCoord.PrimaryHandInventory, Color.FromArgb(128, Color.Red));
						Batch.FillRectangle(InterfaceCoord.Feet, Color.FromArgb(128, Color.Red));
						Batch.FillRectangle(InterfaceCoord.Wrists, Color.FromArgb(128, Color.Red));
						Batch.FillRectangle(InterfaceCoord.Food, Color.FromArgb(128, Color.Red));
						Batch.FillRectangle(InterfaceCoord.Armor, Color.FromArgb(128, Color.Red));
						Batch.FillRectangle(InterfaceCoord.Quiver, Color.FromArgb(128, Color.Red));
			*/
		}



		/// <summary>
		/// Draws hero statistic
		/// </summary>
		/// <param name="batch"></param>
		void DrawStatistics(SpriteBatch batch)
		{
			// Background
			batch.DrawTile(TileSet, 18, new Point(352, 0));
			batch.FillRectangle(new Rectangle(360, 70, 182, 30), GameColors.LightGrey);
			batch.FillRectangle(new Rectangle(360, 100, 276, 194), GameColors.LightGrey);
			batch.FillRectangle(new Rectangle(360, 294, 242, 36), GameColors.LightGrey);


			// Hero head
			batch.DrawTile(Heads, Team.SelectedHero.Head, new Point(360, 4));


			batch.DrawString(OutlinedFont, new Point(430, 12), GameColors.White, Team.SelectedHero.Name);
			batch.DrawString(OutlinedFont, new Point(370, 80), GameColors.White, "Character info");

			// HP and Food
			batch.DrawString(Font, new Point(500, 30), GameColors.Black, Team.SelectedHero.HitPoint.Current + " of " + Team.SelectedHero.HitPoint.Max);

			// Food
			Color color;
			if (Team.SelectedHero.Food > 50)
				color = GameColors.Green;
			else if (Team.SelectedHero.Food > 25)
				color = GameColors.Yellow;
			else
				color = GameColors.Red;

			batch.FillRectangle(new Rectangle(498, 48, Team.SelectedHero.Food, 10), color);

			string txt = string.Empty;
			foreach (Profession prof in Team.SelectedHero.Professions)
				txt += prof.Class.ToString() + "/";
			txt = txt.Substring(0, txt.Length - 1);

			batch.DrawString(Font, new Point(366, 110), GameColors.Black, txt);
			batch.DrawString(Font, new Point(366, 124), GameColors.Black, Team.SelectedHero.Alignment.ToString());
			batch.DrawString(Font, new Point(366, 138), GameColors.Black, Team.SelectedHero.Race.ToString());

			batch.DrawString(Font, new Point(366, 166), GameColors.Black, "Strength");
			batch.DrawString(Font, new Point(366, 180), GameColors.Black, "Intelligence");
			batch.DrawString(Font, new Point(366, 194), GameColors.Black, "Wisdom");
			batch.DrawString(Font, new Point(366, 208), GameColors.Black, "Dexterity");
			batch.DrawString(Font, new Point(366, 222), GameColors.Black, "Constitution");
			batch.DrawString(Font, new Point(366, 236), GameColors.Black, "Charisma");
			batch.DrawString(Font, new Point(366, 250), GameColors.Black, "Armor class");


			batch.DrawString(Font, new Point(552, 166), GameColors.Black, Team.SelectedHero.Strength.Value.ToString());// + "/" + Team.SelectedHero.MaxStrength.ToString());
			batch.DrawString(Font, new Point(552, 180), GameColors.Black, Team.SelectedHero.Intelligence.Value.ToString());
			batch.DrawString(Font, new Point(552, 194), GameColors.Black, Team.SelectedHero.Wisdom.Value.ToString());
			batch.DrawString(Font, new Point(552, 208), GameColors.Black, Team.SelectedHero.Dexterity.Value.ToString());
			batch.DrawString(Font, new Point(552, 222), GameColors.Black, Team.SelectedHero.Constitution.Value.ToString());
			batch.DrawString(Font, new Point(552, 236), GameColors.Black, Team.SelectedHero.Charisma.Value.ToString());
			batch.DrawString(Font, new Point(552, 250), GameColors.Black, Team.SelectedHero.ArmorClass.ToString());


			batch.DrawString(Font, new Point(470, 270), GameColors.Black, "EXP");
			batch.DrawString(Font, new Point(550, 270), GameColors.Black, "LVL");
			int y = 0;
			foreach (Profession prof in Team.SelectedHero.Professions)
			{
				batch.DrawString(Font, new Point(366, 290 + y), GameColors.Black, prof.Class.ToString());
				batch.DrawString(Font, new Point(460, 290 + y), GameColors.White, prof.Experience.ToString());
				batch.DrawString(Font, new Point(560, 290 + y), GameColors.White, prof.Level.ToString());

				y += 12;
			}

		}




		#endregion


		#region Updates

		/// <summary>
		/// Update the Team status
		/// </summary>
		/// <param name="time">Time passed since the last call to the last update.</param>
		public override void Update(GameTime time, bool hasFocus, bool isCovered)
		{

			#region Dialog

			if (Dialog != null)
			{
				if (Dialog.Quit)
					Dialog = null;
				else
				{
					Dialog.Update(time);
					return;
				}
			}

			#endregion


		//	Team.HasMoved = false;


			#region Keyboard

			// Bye bye
			if (Keyboard.IsNewKeyPress(Keys.Escape))
			{
				ExitScreen();
				return;
			}

			// Reload data banks
			if (Keyboard.IsNewKeyPress(Keys.W))
			{
				DisplayCoordinates.Load();
				GameMessage.AddMessage("MazeDisplayCoordinates reloaded...");
			}



			if (Keyboard.IsNewKeyPress(Keys.V))
			{
				Team.ReorderHeroes();
			}


			// Reload data banks
			if (Keyboard.IsNewKeyPress(Keys.R))
			{
				//Team.Dungeon = ResourceManager.CreateAsset<Dungeon>("Eye");
				//Dungeon.Team = this;
				//Team.Dungeon.Init();
				GameMessage.AddMessage("Dungeon reloaded...");
			}


			// AutoMap
			if (Keyboard.IsNewKeyPress(Keys.Tab))
			{
				ScreenManager.AddScreen(new AutoMap(Batch));
			}


			// Debug
			if (Keyboard.IsNewKeyPress(Keys.Space))
				Debug = !Debug;

			// Save team
			if (Keyboard.IsNewKeyPress(Keys.J))
			{
				SaveParty(@"z:\data\savegame.xml");
			}

			// Load team
			if (Keyboard.IsNewKeyPress(Keys.L))
			{
				LoadParty();
			}


			#region Change maze
			// Change maze
			for (int i = 0 ; i < 12 ; i++)
			{
				if (Keyboard.IsNewKeyPress(Keys.F1 + i))
				{
					int id = i + 1;
					string lvl = "0" + id.ToString();
					lvl = "Catacomb - " + lvl.Substring(lvl.Length - 2, 2);

					if (Team.Teleport(lvl))
						GameMessage.AddMessage("Loading " + lvl + ":" + Team.Maze.Description);

					break;
				}
			}

			// Test maze
			if (Keyboard.IsNewKeyPress(Keys.T))
			{
				if (Team.Teleport("test"))
					GameMessage.AddMessage("Loading maze test", GameColors.Blue);
			}

			// Forest maze
			if (Keyboard.IsNewKeyPress(Keys.F))
			{
				if (Team.Teleport("Forest"))
					GameMessage.AddMessage("Loading maze forest", Color.Blue);
			}

			#endregion


			#region Team move & managment

			// Display inventory
			if (Keyboard.IsNewKeyPress(InputScheme["Inventory"]))
			{
				if (Interface == TeamInterface.Inventory)
					Interface = TeamInterface.Main;
				else
					Interface = TeamInterface.Inventory;
			}


			// Turn left
			if (Keyboard.IsNewKeyPress(InputScheme["TurnLeft"]))
				Team.Location.Direction = Compass.Rotate(Team.Location.Direction, CompassRotation.Rotate270);


			// Turn right
			if (Keyboard.IsNewKeyPress(InputScheme["TurnRight"]))
				Team.Location.Direction = Compass.Rotate(Team.Location.Direction, CompassRotation.Rotate90);


			// Move forward
			if (Keyboard.IsNewKeyPress(InputScheme["MoveForward"]))
				Team.Walk(0, -1);


			// Move backward
			if (Keyboard.IsNewKeyPress(InputScheme["MoveBackward"]))
				Team.Walk(0, 1);


			// Strafe left
			if (Keyboard.IsNewKeyPress(InputScheme["StrafeLeft"]))
				Team.Walk(-1, 0);

			// Strafe right
			if (Keyboard.IsNewKeyPress(InputScheme["StrafeRight"]))
				Team.Walk(1, 0);

			// Select Hero 1
			if (Keyboard.IsNewKeyPress(InputScheme["SelectHero1"]))
				Team.SelectedHero = Team.Heroes[0];

			// Select Hero 2
			if (Keyboard.IsNewKeyPress(InputScheme["SelectHero2"]))
				Team.SelectedHero = Team.Heroes[1];

			// Select Hero 3
			if (Keyboard.IsNewKeyPress(InputScheme["SelectHero3"]))
				Team.SelectedHero = Team.Heroes[2];

			// Select Hero 4
			if (Keyboard.IsNewKeyPress(InputScheme["SelectHero4"]))
				Team.SelectedHero = Team.Heroes[3];

			// Select Hero 5
			if (Keyboard.IsNewKeyPress(InputScheme["SelectHero5"]) && Team.HeroCount >= 5)
				Team.SelectedHero = Team.Heroes[4];

			// Select Hero 6
			if (Keyboard.IsNewKeyPress(InputScheme["SelectHero6"]) && Team.HeroCount >= 6)
				Team.SelectedHero = Team.Heroes[5];
			#endregion


			#endregion


			SquarePosition groundpos = SquarePosition.NorthEast;
			Point mousePos = Mouse.Location;
			Point pos = Point.Empty;

			// Get the square at team position
			Square square = Team.Maze.GetSquare(Team.Location.Coordinate);


			#region Mouse

			#region Left mouse button
			if (Mouse.IsNewButtonDown(MouseButtons.Left) && Dialog == null)
			{

				#region Direction buttons
				// Turn left
				if (InterfaceCoord.TurnLeft.Contains(mousePos))
					Team.Location.Direction = Compass.Rotate(Team.Location.Direction, CompassRotation.Rotate270);

				// MoveForward
				else if (InterfaceCoord.MoveForward.Contains(mousePos))
					Team.Walk(0, -1);

				// Turn right
				else if (InterfaceCoord.TurnRight.Contains(mousePos))
					Team.Location.Direction = Compass.Rotate(Team.Location.Direction, CompassRotation.Rotate90);

				// Move left
				else if (InterfaceCoord.MoveLeft.Contains(mousePos))
					Team.Walk(-1, 0);

				// Backward
				else if (InterfaceCoord.MoveBackward.Contains(mousePos))
				{
					if (!Team.Walk(0, 1))
						GameMessage.AddMessage("You can't go that way.");
				}
				// Move right
				else if (InterfaceCoord.MoveRight.Contains(mousePos))
				{
					if (!Team.Walk(1, 0))
						GameMessage.AddMessage("You can't go that way.");
				}
				#endregion

				#region Camp button
				else if (DisplayCoordinates.CampButton.Contains(mousePos))
				{
					SpellBook.Close();
					Dialog = new CampDialog();
					Interface = TeamInterface.Main;
				}
				#endregion


				#region Gather item on the ground Left

				// Team's feet
				else if (DisplayCoordinates.LeftFeetTeam.Contains(mousePos))
				{
					switch (Team.Direction)
					{
						case CardinalPoint.North:
						groundpos = SquarePosition.NorthWest;
						break;
						case CardinalPoint.East:
						groundpos = SquarePosition.NorthEast;
						break;
						case CardinalPoint.South:
						groundpos = SquarePosition.SouthEast;
						break;
						case CardinalPoint.West:
						groundpos = SquarePosition.SouthWest;
						break;
					}
					if (Team.ItemInHand != null)
					{
						if (square.DropItem(groundpos, Team.ItemInHand))
							Team.SetItemInHand(null);
					}
					else
					{
						Team.SetItemInHand(square.CollectItem(groundpos));
					}
				}

				// In front of the team
				else if (!Team.FrontSquare.IsWall && DisplayCoordinates.LeftFrontTeamGround.Contains(mousePos))
				{
					// Ground position
					switch (Team.Location.Direction)
					{
						case CardinalPoint.North:
						groundpos = SquarePosition.SouthWest;
						break;
						case CardinalPoint.East:
						groundpos = SquarePosition.NorthWest;
						break;
						case CardinalPoint.South:
						groundpos = SquarePosition.NorthEast;
						break;
						case CardinalPoint.West:
						groundpos = SquarePosition.SouthEast;
						break;
					}


					if (Team.ItemInHand != null)
					{
						if (Team.FrontSquare.DropItem(groundpos, Team.ItemInHand))
							Team.SetItemInHand(null);
					}
					else
						Team.SetItemInHand(Team.FrontSquare.CollectItem(groundpos));
				}


				#endregion

				#region Gather item on the ground right
				else if (DisplayCoordinates.RightFeetTeam.Contains(mousePos))
				{
					switch (Team.Location.Direction)
					{
						case CardinalPoint.North:
						groundpos = SquarePosition.NorthEast;
						break;
						case CardinalPoint.East:
						groundpos = SquarePosition.SouthEast;
						break;
						case CardinalPoint.South:
						groundpos = SquarePosition.SouthWest;
						break;
						case CardinalPoint.West:
						groundpos = SquarePosition.NorthWest;
						break;
					}

					if (Team.ItemInHand != null)
					{
						if (square.DropItem(groundpos, Team.ItemInHand))
							Team.SetItemInHand(null);
					}
					else
					{
						Team.SetItemInHand(square.CollectItem(groundpos));
						//if (ItemInHand != null)
						//    AddMessage(Language.BuildMessage(2, ItemInHand.Name));

					}
				}

				// In front of the team
				else if (!Team.FrontSquare.IsWall && DisplayCoordinates.RightFrontTeamGround.Contains(mousePos))
				{

					// Ground position
					switch (Team.Location.Direction)
					{
						case CardinalPoint.North:
						groundpos = SquarePosition.SouthEast;
						break;
						case CardinalPoint.East:
						groundpos = SquarePosition.SouthWest;
						break;
						case CardinalPoint.South:
						groundpos = SquarePosition.NorthWest;
						break;
						case CardinalPoint.West:
						groundpos = SquarePosition.NorthEast;
						break;
					}


					if (Team.ItemInHand != null)
					{
						if (Team.FrontSquare.DropItem(groundpos, Team.ItemInHand))
							Team.SetItemInHand(null);
					}
					else
					{
						Team.SetItemInHand(Team.FrontSquare.CollectItem(groundpos));
						//if (ItemInHand != null)
						//    AddMessage(Language.BuildMessage(2, ItemInHand.Name));
					}
				}

				#endregion

				#region Alcove
				else if (DisplayCoordinates.Alcove.Contains(mousePos) && Team.FrontSquare.IsWall)
				{

					if (Team.ItemInHand != null)
					{
						if (Team.FrontSquare.DropAlcoveItem(Team.FrontWallSide, Team.ItemInHand))
							Team.SetItemInHand(null);
					}
					else
					{
						Team.SetItemInHand(Team.FrontSquare.CollectAlcoveItem(Team.FrontWallSide));
					}
				}
				#endregion

				#region Action to process on the front square

				// Click on the square in front of the team
				else if (DisplayCoordinates.FrontSquare.Contains(mousePos))
				{
					if (!Team.FrontSquare.OnClick(mousePos, Team.FrontWallSide))
					{
						#region Throw an object in the left side
						if (DisplayCoordinates.ThrowLeft.Contains(mousePos) && Team.ItemInHand != null)
						{
							DungeonLocation loc = new DungeonLocation(Team.Location);
							switch (Team.Location.Direction)
							{
								case CardinalPoint.North:
								loc.Position = SquarePosition.SouthWest;
								break;
								case CardinalPoint.East:
								loc.Position = SquarePosition.NorthWest;
								break;
								case CardinalPoint.South:
								loc.Position = SquarePosition.NorthEast;
								break;
								case CardinalPoint.West:
								loc.Position = SquarePosition.SouthEast;
								break;
							}
							Team.Maze.ThrownItems.Add(new ThrownItem(Team.SelectedHero, Team.ItemInHand, loc, TimeSpan.FromSeconds(0.25), 4));
							Team.SetItemInHand(null);
						}
						#endregion

						#region Throw an object in the right side
						else if (DisplayCoordinates.ThrowRight.Contains(mousePos) && Team.ItemInHand != null)
						{

							DungeonLocation loc = new DungeonLocation(Team.Location);

							switch (Team.Location.Direction)
							{
								case CardinalPoint.North:
								loc.Position = SquarePosition.SouthEast;
								break;
								case CardinalPoint.East:
								loc.Position = SquarePosition.SouthWest;
								break;
								case CardinalPoint.South:
								loc.Position = SquarePosition.NorthWest;
								break;
								case CardinalPoint.West:
								loc.Position = SquarePosition.NorthEast;
								break;
							}

							Team.Maze.ThrownItems.Add(new ThrownItem(Team.SelectedHero, Team.ItemInHand, loc, TimeSpan.FromSeconds(0.25), 4));
							Team.SetItemInHand(null);
						}
						#endregion
					}
				}
				#endregion
			}

			#endregion

			#region Right mouse button

			else if (Mouse.IsNewButtonDown(MouseButtons.Right))
			{
				#region Alcove
				if (DisplayCoordinates.Alcove.Contains(mousePos) && Team.FrontSquare.IsWall)
				{
					Team.SelectedHero.AddToInventory(Team.FrontSquare.CollectAlcoveItem(Team.FrontWallSide));
				}
				#endregion

				#region Action to process on the front square

				#endregion

				#region Gather item on the ground Left

				// Team's feet
				else if (DisplayCoordinates.LeftFeetTeam.Contains(mousePos))
				{
					switch (Team.Direction)
					{
						case CardinalPoint.North:
						groundpos = SquarePosition.NorthWest;
						break;
						case CardinalPoint.East:
						groundpos = SquarePosition.NorthEast;
						break;
						case CardinalPoint.South:
						groundpos = SquarePosition.SouthEast;
						break;
						case CardinalPoint.West:
						groundpos = SquarePosition.SouthWest;
						break;
					}
					if (Team.ItemInHand == null)
					{
						Item item = square.CollectItem(groundpos);
						if (item != null)
						{
							if (Team.SelectedHero.AddToInventory(item))
								GameMessage.BuildMessage(2, item.Name);
							else
								square.DropItem(groundpos, item);
						}
					}
				}

				// In front of the team
				else if (DisplayCoordinates.LeftFrontTeamGround.Contains(mousePos) && !Team.FrontSquare.IsWall)
				{
					// Ground position
					switch (Team.Location.Direction)
					{
						case CardinalPoint.North:
						groundpos = SquarePosition.SouthWest;
						break;
						case CardinalPoint.East:
						groundpos = SquarePosition.NorthWest;
						break;
						case CardinalPoint.South:
						groundpos = SquarePosition.NorthEast;
						break;
						case CardinalPoint.West:
						groundpos = SquarePosition.SouthEast;
						break;
					}

					if (Team.ItemInHand == null)
					{
						Item item = Team.FrontSquare.CollectItem(groundpos);

						if (item != null)
						{
							if (Team.SelectedHero.AddToInventory(item))
								GameMessage.BuildMessage(2, item.Name);
							else
								Team.FrontSquare.DropItem(groundpos, item);
						}
					}
				}


				#endregion

				#region Gather item on the ground right
				else if (DisplayCoordinates.RightFeetTeam.Contains(mousePos))
				{
					switch (Team.Location.Direction)
					{
						case CardinalPoint.North:
						groundpos = SquarePosition.NorthEast;
						break;
						case CardinalPoint.East:
						groundpos = SquarePosition.SouthEast;
						break;
						case CardinalPoint.South:
						groundpos = SquarePosition.SouthWest;
						break;
						case CardinalPoint.West:
						groundpos = SquarePosition.NorthWest;
						break;
					}

					if (Team.ItemInHand == null)
					{
						Item item = square.CollectItem(groundpos);
						if (item != null)
						{
							if (Team.SelectedHero.AddToInventory(item))
								GameMessage.BuildMessage(2, item.Name);
							else
								square.DropItem(groundpos, item);
						}
					}
				}

				// In front of the team
				else if (DisplayCoordinates.RightFrontTeamGround.Contains(mousePos) && !Team.FrontSquare.IsWall)
				{

					// Ground position
					switch (Team.Location.Direction)
					{
						case CardinalPoint.North:
						groundpos = SquarePosition.SouthEast;
						break;
						case CardinalPoint.East:
						groundpos = SquarePosition.SouthWest;
						break;
						case CardinalPoint.South:
						groundpos = SquarePosition.NorthWest;
						break;
						case CardinalPoint.West:
						groundpos = SquarePosition.NorthEast;
						break;
					}

					if (Team.ItemInHand == null)
					{
						Item item = Team.FrontSquare.CollectItem(groundpos);
						if (item != null)
						{
							if (Team.SelectedHero.AddToInventory(item))
								GameMessage.BuildMessage(2, item.Name);
							else
								Team.FrontSquare.DropItem(groundpos, item);
						}
					}
				}

				#endregion
			}

			#endregion

			#endregion


			#region Interface actions

			if (Interface == TeamInterface.Inventory)
			{
				UpdateInventory(time);
			}
			else if (Interface == TeamInterface.Statistic)
			{
				UpdateStatistics(time);
			}
			else
			{
				#region Left mouse button action


				// Left mouse button down
				if (Mouse.IsNewButtonDown(MouseButtons.Left) && Dialog == null)
				{

					Item item = null;

					// Update each hero interface
					for (int id = 0 ; id < 6 ; id++)
					{
						// Get the hero
						Hero hero = Team.Heroes[id];
						if (hero == null)
							continue;

						#region Select hero
						if (InterfaceCoord.SelectHero[id].Contains(mousePos))
							Team.SelectedHero = hero;
						#endregion

						#region Swap hero
						if (InterfaceCoord.HeroFace[id].Contains(mousePos))
						{
							Team.SelectedHero = hero;
							HeroToSwap = null;
							Interface = TeamInterface.Inventory;
						}
						#endregion

						#region Take object in primary hand
						if (InterfaceCoord.PrimaryHand[id].Contains(mousePos)) // && hero.CanUseHand(HeroHand.Primary))
						{
							item = hero.GetInventoryItem(InventoryPosition.Primary);

							if (Team.ItemInHand != null && ((Team.ItemInHand.Slot & BodySlot.Primary) == BodySlot.Primary))
							{
								Item swap = Team.ItemInHand;
								Team.SetItemInHand(hero.GetInventoryItem(InventoryPosition.Primary));
								hero.SetInventoryItem(InventoryPosition.Primary, swap);
							}
							else if (Team.ItemInHand == null && item != null)
							{
								Team.SetItemInHand(item);
								hero.SetInventoryItem(InventoryPosition.Primary, null);
							}
						}
						#endregion

						#region Take object in secondary hand
						if (InterfaceCoord.SecondaryHand[id].Contains(mousePos)) // && hero.CanUseHand(HeroHand.Secondary))
						{
							item = hero.GetInventoryItem(InventoryPosition.Secondary);

							if (Team.ItemInHand != null && ((Team.ItemInHand.Slot & BodySlot.Secondary) == BodySlot.Secondary))
							{
								Item swap = Team.ItemInHand;
								Team.SetItemInHand(hero.GetInventoryItem(InventoryPosition.Secondary));
								hero.SetInventoryItem(InventoryPosition.Secondary, swap);

							}
							else if (Team.ItemInHand == null && item != null)
							{
								Team.SetItemInHand(item);
								hero.SetInventoryItem(InventoryPosition.Secondary, null);
							}
						}
						#endregion
					}


				}

				#endregion

				#region Right mouse button action

				// Right mouse button down
				if (Mouse.IsNewButtonDown(MouseButtons.Right) && Dialog == null)
				{

					// Update each hero interface
					for (int id = 0 ; id < 6 ; id++)
					{
						// Get the hero
						Hero hero = Team.Heroes[id];
						if (hero == null)
							continue;


						#region Swap hero
						if (InterfaceCoord.SelectHero[id].Contains(mousePos))
						{
							if (HeroToSwap == null)
							{
								HeroToSwap = hero;
							}
							else
							{
								Team.Heroes[(int)Team.GetHeroPosition(HeroToSwap)] = hero;
								Team.Heroes[id] = HeroToSwap;


								HeroToSwap = null;
							}
						}
						#endregion

						#region Show Hero inventory
						if (InterfaceCoord.HeroFace[id].Contains(mousePos))
						{
							Team.SelectedHero = hero;
							Interface = TeamInterface.Inventory;
						}
						#endregion

						#region Use object in primary hand
						if (InterfaceCoord.PrimaryHand[id].Contains(mousePos) && hero.CanUseHand(HeroHand.Primary))
							hero.UseHand(HeroHand.Primary);

						#endregion

						#region Use object in secondary hand
						if (InterfaceCoord.SecondaryHand[id].Contains(mousePos) && hero.CanUseHand(HeroHand.Secondary))
							hero.UseHand(HeroHand.Secondary);
						#endregion
					}
				}
				#endregion
			}

			#endregion


			#region Heros update

			// Update all heroes
			foreach (Hero hero in Team.Heroes)
			{
				if (hero != null)
					hero.Update(time);
			}
			#endregion


			#region Spell window

			SpellBook.Update(time);

			#endregion


			// Update messages
			GameMessage.Update(time);


			// Update the dungeon
			Team.Dungeon.Update(time);


			if (Team.CanMove && Team.Square != null)
				Team.Square.OnTeamStand();
		}


		/// <summary>
		/// Updates inventory
		/// </summary>
		/// <param name="time">Elapsed game time</param>
		void UpdateInventory(GameTime time)
		{
			Point mousePos = Mouse.Location;
			Item item = null;


			if (Mouse.IsNewButtonDown(MouseButtons.Left))
			{
				// Close inventory
				if (InterfaceCoord.CloseInventory.Contains(mousePos))
					Interface = TeamInterface.Main;


				// Show statistics
				if (InterfaceCoord.ShowStatistics.Contains(mousePos))
					Interface = TeamInterface.Statistic;


				// Previous Hero
				if (InterfaceCoord.PreviousHero.Contains(mousePos))
					Team.SelectedHero = Team.GetPreviousHero();

				// Next Hero
				if (InterfaceCoord.NextHero.Contains(mousePos))
					Team.SelectedHero = Team.GetNextHero();



				#region Manage bag pack items
				for (int id = 0 ; id < 14 ; id++)
				{
					if (InterfaceCoord.BackPack[id].Contains(mousePos))
					{
						Item swap = Team.ItemInHand;
						Team.SetItemInHand(Team.SelectedHero.GetBackPackItem(id));
						Team.SelectedHero.SetBackPackItem(id, swap);
					}
				}
				#endregion

				#region Quiver
				if (InterfaceCoord.Quiver.Contains(mousePos))
				{
					if (Team.ItemInHand == null && Team.SelectedHero.Quiver > 0)
					{
						Team.SelectedHero.Quiver--;
						Team.SetItemInHand(ResourceManager.CreateAsset<Item>("Arrow"));
					}
					else if (Team.ItemInHand != null && (Team.ItemInHand.Slot & BodySlot.Quiver) == BodySlot.Quiver)
					{
						Team.SelectedHero.Quiver++;
						Team.SetItemInHand(null);
					}
				}
				#endregion

				#region Armor
				else if (InterfaceCoord.Armor.Contains(mousePos))
				{
					item = Team.SelectedHero.GetInventoryItem(InventoryPosition.Armor);

					if (Team.ItemInHand != null && Team.SelectedHero.SetInventoryItem(InventoryPosition.Armor, Team.ItemInHand))
						Team.SetItemInHand(item);
					else if (Team.ItemInHand == null && item != null)
					{
						Team.SetItemInHand(item);
						Team.SelectedHero.SetInventoryItem(InventoryPosition.Armor, null);
					}
				}
				#endregion

				#region Food
				else if (InterfaceCoord.Food.Contains(mousePos))
				{
					if (Team.ItemInHand != null && Team.ItemInHand.Type == ItemType.Consumable)
					{
						if (Team.ItemInHand.Script.Instance != null)
							Team.ItemInHand.Script.Instance.OnUse(Team.ItemInHand, Team.SelectedHero);
						Team.SetItemInHand(null);
					}
				}
				#endregion

				#region Wrists
				else if (InterfaceCoord.Wrist.Contains(mousePos))
				{
					item = Team.SelectedHero.GetInventoryItem(InventoryPosition.Wrist);

					if (Team.ItemInHand != null && Team.SelectedHero.SetInventoryItem(InventoryPosition.Wrist, Team.ItemInHand))
						Team.SetItemInHand(item);
					else if (Team.ItemInHand == null && item != null)
					{
						Team.SetItemInHand(item);
						Team.SelectedHero.SetInventoryItem(InventoryPosition.Wrist, null);
					}
				}
				#endregion

				#region Primary
				else if (InterfaceCoord.PrimaryHandInventory.Contains(mousePos))
				{
					item = Team.SelectedHero.GetInventoryItem(InventoryPosition.Primary);

					if (Team.ItemInHand != null && Team.SelectedHero.SetInventoryItem(InventoryPosition.Primary, Team.ItemInHand))
						Team.SetItemInHand(item);
					else if (Team.ItemInHand == null && item != null)
					{
						Team.SetItemInHand(item);
						Team.SelectedHero.SetInventoryItem(InventoryPosition.Primary, null);
					}
				}
				#endregion

				#region Feet
				else if (InterfaceCoord.Feet.Contains(mousePos))
				{
					item = Team.SelectedHero.GetInventoryItem(InventoryPosition.Feet);

					if (Team.ItemInHand != null && Team.SelectedHero.SetInventoryItem(InventoryPosition.Feet, Team.ItemInHand))
						Team.SetItemInHand(item);
					else if (Team.ItemInHand == null && item != null)
					{
						Team.SetItemInHand(item);
						Team.SelectedHero.SetInventoryItem(InventoryPosition.Feet, null);
					}
				}
				#endregion

				#region Secondary
				else if (InterfaceCoord.SecondaryHandInventory.Contains(mousePos))
				{
					item = Team.SelectedHero.GetInventoryItem(InventoryPosition.Secondary);

					if (Team.ItemInHand != null && Team.SelectedHero.SetInventoryItem(InventoryPosition.Secondary, Team.ItemInHand))
						Team.SetItemInHand(item);
					else if (Team.ItemInHand == null && item != null)
					{
						Team.SetItemInHand(item);
						Team.SelectedHero.SetInventoryItem(InventoryPosition.Secondary, null);
					}
				}
				#endregion

				#region Neck
				else if (InterfaceCoord.Neck.Contains(mousePos))
				{
					item = Team.SelectedHero.GetInventoryItem(InventoryPosition.Neck);

					if (Team.ItemInHand != null && Team.SelectedHero.SetInventoryItem(InventoryPosition.Neck, Team.ItemInHand))
						Team.SetItemInHand(item);
					else if (Team.ItemInHand == null && item != null)
					{
						Team.SetItemInHand(item);
						Team.SelectedHero.SetInventoryItem(InventoryPosition.Neck, null);
					}
				}
				#endregion

				#region Head
				else if (InterfaceCoord.Head.Contains(mousePos))
				{
					item = Team.SelectedHero.GetInventoryItem(InventoryPosition.Helmet);

					if (Team.ItemInHand != null && Team.SelectedHero.SetInventoryItem(InventoryPosition.Helmet, Team.ItemInHand))
						Team.SetItemInHand(item);
					else if (Team.ItemInHand == null && item != null)
					{
						Team.SetItemInHand(item);
						Team.SelectedHero.SetInventoryItem(InventoryPosition.Helmet, null);
					}
				}
				#endregion

				else
				{
					#region Belt
					for (int id = 0 ; id < 3 ; id++)
					{
						if (InterfaceCoord.Waist[id].Contains(mousePos))
						{
							item = Team.SelectedHero.GetWaistPackItem(id);

							if (Team.ItemInHand != null && Team.SelectedHero.SetWaistPackItem(id, Team.ItemInHand))
								Team.SetItemInHand(item);
							else if (Team.ItemInHand == null && item != null)
							{
								Team.SetItemInHand(item);
								Team.SelectedHero.SetWaistPackItem(id, null);
							}
						}
					}
					#endregion

					#region Rings
					for (int id = 0 ; id < 2 ; id++)
					{
						item = Team.SelectedHero.GetInventoryItem(InventoryPosition.Ring_Left + id);

						if (InterfaceCoord.Rings[id].Contains(mousePos))
						{
							if (Team.ItemInHand != null && Team.SelectedHero.SetInventoryItem(InventoryPosition.Ring_Left + id, Team.ItemInHand))
								Team.SetItemInHand(item);
							else if (Team.ItemInHand == null && item != null)
							{
								Team.SetItemInHand(item);
								Team.SelectedHero.SetInventoryItem(InventoryPosition.Ring_Left + id, null);
							}
						}
					}
					#endregion
				}
			}



		}


		/// <summary>
		/// Updates statistics
		/// </summary>
		/// <param name="time">Elapsed game time</param>
		void UpdateStatistics(GameTime time)
		{
			Point mousePos = Mouse.Location;

			if (Mouse.IsNewButtonDown(System.Windows.Forms.MouseButtons.Left))
			{

				// Close inventory
				if (InterfaceCoord.CloseInventory.Contains(mousePos))
					Interface = TeamInterface.Main;


				// Show statistics
				if (InterfaceCoord.ShowStatistics.Contains(mousePos))
					Interface = TeamInterface.Inventory;



				// Previous Hero
				if (InterfaceCoord.PreviousHero.Contains(mousePos))
					Team.SelectedHero = Team.GetPreviousHero();

				// Next Hero
				if (InterfaceCoord.NextHero.Contains(mousePos))
					Team.SelectedHero = Team.GetNextHero();

			}
		}


		#endregion


		#region Properties


		/// <summary>
		/// Heads of the Heroes
		/// </summary>
		TileSet Heads;


		/// <summary>
		/// Debug mode
		/// </summary>
		public bool Debug
		{
			get;
			set;
		}


		/// <summary>
		/// Drawing Tileset
		/// </summary>
		TileSet TileSet;


		/// <summary>
		/// Items tilesets
		/// </summary>
		TileSet Items;


		/// <summary>
		/// Interface to display
		/// </summary>
		static public TeamInterface Interface
		{
			get;
			private set;
		}


		/// <summary>
		/// Spritebatch
		/// </summary>
		SpriteBatch Batch;


		/// <summary>
		/// Display font
		/// </summary>
		BitmapFont Font;


		/// <summary>
		/// Outlined font
		/// </summary>
		BitmapFont OutlinedFont;


		/// <summary>
		/// Dialog GUI
		/// </summary>
		static public DialogBase Dialog
		{
			get
			{
				return dialog;
			}
			set
			{
				if (dialog != null)
					dialog.Dispose();

				dialog = value;
			}
		}
		static DialogBase dialog;


		/// <summary>
		/// Allow the player to personalize keyboard input shceme
		/// </summary>
		InputScheme InputScheme;


		/// <summary>
		/// Spell book window
		/// </summary>
		static public SpellBook SpellBook
		{
			get;
			private set;
		}


		/// <summary>
		/// Draw HP as bar
		/// </summary>
		static public bool DrawHPAsBar
		{
			get;
			set;
		}


		/// <summary>
		/// Hero swapping
		/// </summary>
		Hero HeroToSwap;


		#endregion
	}


	#region Enums


	/// <summary>
	/// Interface panel to display
	/// </summary>
	public enum TeamInterface
	{
		/// <summary>
		/// Display the main interface
		/// </summary>
		Main,


		/// <summary>
		/// Display the inventory
		/// </summary>
		Inventory,


		/// <summary>
		/// Display statistic page
		/// </summary>
		Statistic
	}


	#endregion


}
