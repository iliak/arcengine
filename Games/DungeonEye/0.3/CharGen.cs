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
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ArcEngine;
using ArcEngine.Asset;
using ArcEngine.Graphic;
using ArcEngine.Input;
using ArcEngine.Utility.ScreenManager;
using DungeonEye.Gui;

namespace DungeonEye
{
	/// <summary>
	/// Charactere generation
	/// </summary>
	public class CharGen : GameScreen
	{

		/// <summary>
		/// Constructor
		/// </summary>
		public CharGen()
		{
			Heroes = new Hero[4];

			HeroeBoxes = new Rectangle[] 
			{
				new Rectangle(32,  128, 64, 64),
				new Rectangle(160, 128, 64, 64),
				new Rectangle(32,  256, 64, 64),
				new Rectangle(160, 256, 64, 64),
			};
			NameLocations = new Point[]
			{
				new Point(4, 212),
				new Point(132, 212),
				new Point(4, 340),
				new Point(132, 340),
			};
			HeroID = -1;

			BackButton = new Rectangle(528, 344, 76, 32);
		}




		/// <summary>
		/// Loads content
		/// </summary>
		public override void LoadContent()
		{
			ResourceManager.LoadBank("data/chargen.bnk");

			Tileset = ResourceManager.CreateAsset<TileSet>("CharGen");
			Tileset.Scale = new SizeF(2.0f, 2.0f);

			Heads = ResourceManager.CreateAsset<TileSet>("Heads");
			Heads.Scale = new SizeF(2.0f, 2.0f);

			Font = ResourceManager.CreateAsset<Font2d>("intro");
			Font.GlyphTileset.Scale = new SizeF(2.0f, 2.0f);

			NameFont = ResourceManager.CreateAsset<Font2d>("name");
			NameFont.GlyphTileset.Scale = new SizeF(2.0f, 2.0f);

			PlayButton = new ScreenButton(string.Empty, new Rectangle(48, 362, 166, 32));
			PlayButton.Selected += new EventHandler(PlayButton_Selected);

			StringTable = ResourceManager.CreateAsset<StringTable>("Chargen");
			StringTable.LanguageName = Game.LanguageName;

			Anims = ResourceManager.CreateAsset<Animation>("Animations");
			Anims.TileSet.Scale = new SizeF(2.0f, 2.0f);
			Anims.Play();

			CurrentState = CharGenStates.SelectHero;
		}


		/// <summary>
		/// Unload contents
		/// </summary>
		public override void UnloadContent()
		{
			if (Tileset != null)
				Tileset.Dispose();
			if (Font != null)
				Font.Dispose();
			if (Anims != null)
				Anims.Dispose();

			Tileset = null;
			Font = null;
			Anims = null;
		}


		#region Events


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void PlayButton_Selected(object sender, EventArgs e)
		{

			ScreenManager.AddScreen(new Team(Heroes));
			ExitScreen();
		}


		#endregion


		#region Updates & Draws


		/// <summary>
		/// Update the scene
		/// </summary>
		/// <param name="time"></param>
		/// <param name="hasFocus"></param>
		/// <param name="isCovered"></param>
		public override void Update(GameTime time, bool hasFocus, bool isCovered)
		{
			// Go back to the main menu
			if (Keyboard.IsNewKeyPress(Keys.Escape))
				ExitScreen();



			switch (CurrentState)
			{
				#region Select hero
				case CharGenStates.SelectHero:
				if (Mouse.IsButtonDown(MouseButtons.Left))
				{
					for (int id = 0; id < 4; id++)
					{
						if (HeroeBoxes[id].Contains(Mouse.Location))
						{
							HeroID = id;

							// Create a new hero or remove it
							if (Heroes[id] == null)
							{
								Heroes[id] = new Hero(null);
								CurrentState = CharGenStates.SelectRace;
							}
							else
								CurrentState = CharGenStates.Delete;
						}
					}
				}
				break;
				#endregion

				#region Select race
				case CharGenStates.SelectRace:
				{
					Point point = new Point(300, 140);
					for (int i = 0; i < 12; i++)
					{
						point.Y += 18;
						if (new Rectangle(point.X, point.Y, 324, 16).Contains(Mouse.Location) && Mouse.IsNewButtonDown(MouseButtons.Left))
						{
							CurrentHero.Race = (HeroRace)i;
							CurrentState = CharGenStates.SelectClass;
						}
					}
				}
				break;
				#endregion

				#region Select class
				case CharGenStates.SelectClass:
				{
					Point point = new Point(304, 0);
					for (int i = 0; i < 9; i++)
					{
						point.Y = 176 + i *18;
						if (new Rectangle(286, 176 + i * 18, 324, 16).Contains(Mouse.Location) && Mouse.IsNewButtonDown(MouseButtons.Left))
						{
							CurrentHero.Professions.Clear();

							switch (i)
							{
								case 0:
								CurrentHero.Professions.Add(new Profession(0, HeroClass.Fighter));
								break;
								case 1:
								CurrentHero.Professions.Add(new Profession(0, HeroClass.Ranger));
								break;
								case 2:
								CurrentHero.Professions.Add(new Profession(0, HeroClass.Mage));
								break;
								case 3:
								CurrentHero.Professions.Add(new Profession(0, HeroClass.Cleric));
								break;
								case 4:
								CurrentHero.Professions.Add(new Profession(0, HeroClass.Thief));
								break;
								case 5:
								CurrentHero.Professions.Add(new Profession(0, HeroClass.Fighter));
								CurrentHero.Professions.Add(new Profession(0, HeroClass.Thief));
								break;
								case 6:
								CurrentHero.Professions.Add(new Profession(0, HeroClass.Fighter));
								CurrentHero.Professions.Add(new Profession(0, HeroClass.Mage));
								break;
								case 7:
								CurrentHero.Professions.Add(new Profession(0, HeroClass.Fighter));
								CurrentHero.Professions.Add(new Profession(0, HeroClass.Mage));
								CurrentHero.Professions.Add(new Profession(0, HeroClass.Thief));
								break;
								case 8:
								CurrentHero.Professions.Add(new Profession(0, HeroClass.Thief));
								CurrentHero.Professions.Add(new Profession(0, HeroClass.Mage));
								break;
							}

							CurrentState = CharGenStates.SelectAlignment;
						}


						// Back
						if (BackButton.Contains(Mouse.Location) && Mouse.IsNewButtonDown(MouseButtons.Left))
							CurrentState = CharGenStates.SelectRace;
					}
				
					
				}
				break;
				#endregion

				#region Select alignment
				case CharGenStates.SelectAlignment:
				{
					Point point = new Point(304, 0);
					for (int i = 0; i < 9; i++)
					{
						point.Y = 176 + i * 18;
						if (new Rectangle(286, 176 + i * 18, 324, 16).Contains(Mouse.Location) && Mouse.IsNewButtonDown(MouseButtons.Left))
						{
							EntityAlignment[] alignments = new EntityAlignment[]
							{
								EntityAlignment.LawfulGood,
								EntityAlignment.NeutralGood,
								EntityAlignment.ChaoticGood,
								EntityAlignment.LawfulNeutral,
								EntityAlignment.TrueNeutral,
								EntityAlignment.ChoaticNeutral,
								EntityAlignment.LawfulEvil,
								EntityAlignment.NeutralEvil,
								EntityAlignment.ChaoticEvil,
							};

							CurrentHero.Alignment = alignments[i];
							CurrentState = CharGenStates.SelectFace;
						}
					}

					// Back
					if (BackButton.Contains(Mouse.Location) && Mouse.IsNewButtonDown(MouseButtons.Left))
						CurrentState = CharGenStates.SelectClass;
				}
				break;
				#endregion

				#region Select face
				case CharGenStates.SelectFace:
				{

					if (Mouse.IsNewButtonDown(MouseButtons.Left))
					{
						if (new Rectangle(288, 132, 64, 32).Contains(Mouse.Location))
							FaceOffset--;

						if (new Rectangle(288, 164, 64, 32).Contains(Mouse.Location))
							FaceOffset++;

						// Select a face
						for (int x = 0; x < 4; x++)
						{
							if (new Rectangle(352 + x * 64, 132, 64, 64).Contains(Mouse.Location))
							{
								CurrentHero.Head = FaceOffset + x;
								CurrentState = CharGenStates.Confirm;
								break;
							}
						}
					}


					// Limit value
					if (CurrentHero.Gender == HeroGender.Male)
					{
						if (FaceOffset < 0)
							FaceOffset = 0;

						if (FaceOffset > 25)
							FaceOffset = 25;
					}
					else
					{
						if (FaceOffset < 29)
							FaceOffset = 29;

						if (FaceOffset > 40)
							FaceOffset = 40;
					}

					// Back
					if (BackButton.Contains(Mouse.Location) && Mouse.IsNewButtonDown(MouseButtons.Left))
						CurrentState = CharGenStates.SelectAlignment;
				}
				break;
				#endregion

				#region Confirm
				case CharGenStates.Confirm:
				{
					if (Mouse.IsNewButtonDown(MouseButtons.Left))
					{
						// Reroll
						if (new Rectangle(448, 318, 76, 32).Contains(Mouse.Location))
						{
							CurrentHero.RollAbilities();
						}

						// Faces
						if (new Rectangle(448, 350, 76, 32).Contains(Mouse.Location))
						{
							CurrentHero.Head = -1;
							CurrentState = CharGenStates.SelectFace;
						}

						// Modify
						if (new Rectangle(528, 316, 76, 32).Contains(Mouse.Location))
						{
						}

						// Keep
						if (new Rectangle(528, 350, 76, 32).Contains(Mouse.Location))
						{
							CurrentState = CharGenStates.SelectName;
							Keyboard.OnKeyDown += new EventHandler<PreviewKeyDownEventArgs>(Keyboard_OnKeyDown);
						}
					}
				}
				break;
				#endregion

				#region Select name
				case CharGenStates.SelectName:
				{
				}
				break;
				#endregion

				#region Delete hero
				case CharGenStates.Delete:
				{
					if (Mouse.IsNewButtonDown(MouseButtons.Left))
					{
						// Delete
						if (new Rectangle(448, 344, 76, 32).Contains(Mouse.Location))
						{
							Heroes[HeroID] = null;
							CurrentState = CharGenStates.SelectHero;
						}

						// Ok
						if (new Rectangle(528, 344, 76, 32).Contains(Mouse.Location))
						{
							CurrentState = CharGenStates.SelectHero;
						}
					}
				}
				break;
				#endregion
			}

			// Update anim
			if (Anims != null)
				Anims.Update(time);


			// If the team is ready, let's go !
			if (PlayButton.Rectangle.Contains(Mouse.Location) && Mouse.IsNewButtonDown(System.Windows.Forms.MouseButtons.Left) && IsTeamReadyToPlay)
				PlayButton.OnSelectEntry();
		}



		/// <summary>
		/// Draws the scene
		/// </summary>
		public override void Draw()
		{
			// Clears the background
			Display.ClearBuffers();
			Display.Color = Color.White;


			// Background
			Tileset.Draw(0, Point.Empty);


			// Heroes faces and names
			for (int i = 0; i < 4; i++)
			{
				Hero hero = Heroes[i];
				if (hero == null)
					continue;

				Heads.Draw(hero.Head, HeroeBoxes[i].Location);
				NameFont.DrawText(NameLocations[i], Color.Blue, hero.Name);
			}

			switch (CurrentState)
			{
				#region Select hero
				case CharGenStates.SelectHero:
				{
					Font.DrawText(new Rectangle(304, 160, 300, 64), Color.White, StringTable.GetString(1));

					// Team is ready, game can begin...
					if (IsTeamReadyToPlay)
						Tileset.Draw(1, new Point(48, 362));
				}
				break;
				#endregion

				#region Select race
				case CharGenStates.SelectRace:
				{
					Anims.Draw(HeroeBoxes[HeroID].Location);
					Font.DrawText(new Rectangle(294, 134, 300, 64), Color.FromArgb(85, 255, 255), StringTable.GetString(34));

					Point point = new Point(300, 140);
					Color color;
					for (int i = 0; i < 12; i++)
					{
						point.Y += 18;
						if (new Rectangle(point.X, point.Y, 324, 16).Contains(Mouse.Location))
							color = Color.FromArgb(255, 85, 85);
						else
							color = Color.White;

						Font.DrawText(point, color, StringTable.GetString(i + 22));
					}

				}
				break;
				#endregion

				#region Select class
				case CharGenStates.SelectClass:
				{
					Anims.Draw(HeroeBoxes[HeroID].Location);
					Font.DrawText(new Rectangle(304, 140, 300, 64), Color.FromArgb(85, 255, 255), StringTable.GetString(2));

					Point point = new Point(304, 0);
					Color color;
					for (int i = 0; i < 9; i++)
					{
						point.Y = 176 + i *18;
						if (new Rectangle(286, 176 + i * 18, 324, 16).Contains(Mouse.Location))
							color = Color.FromArgb(255, 85, 85);
						else
							color = Color.White;

						Font.DrawText(point, color, StringTable.GetString(i + 3));
					}

					// Back
					Tileset.Draw(3, BackButton.Location);
					Tileset.Draw(12, new Point(BackButton.Location.X + 12, BackButton.Location.Y + 12));
				}
				break;
				#endregion

				#region Select alignment
				case CharGenStates.SelectAlignment:
				{
					Anims.Draw(HeroeBoxes[HeroID].Location);
					Font.DrawText(new Rectangle(304, 140, 300, 64), Color.FromArgb(85, 255, 255), StringTable.GetString(12));

					Point point = new Point(304, 0);
					Color color;
					for (int i = 0; i < 9; i++)
					{
						point.Y = 176 + i * 18;
						if (new Rectangle(286, 176 + i * 18, 324, 16).Contains(Mouse.Location))
							color = Color.FromArgb(255, 85, 85);
						else
							color = Color.White;

						Font.DrawText(point, color, StringTable.GetString(i + 13));
					}

					// Back
					Tileset.Draw(3, BackButton.Location);
					Tileset.Draw(12, new Point(BackButton.Location.X + 12, BackButton.Location.Y + 12));
				}
				break;
				#endregion

				#region Select face
				case CharGenStates.SelectFace:
				{
					Anims.Draw(HeroeBoxes[HeroID].Location);

					// Class and professions
					Font.DrawText(new Rectangle(300, 210, 300, 64), Color.White, CurrentHero.Race.ToString());
					string txt = string.Empty;
					foreach (Profession prof in CurrentHero.Professions)
						txt += prof.Class.ToString() + "/";
					txt = txt.Substring(0, txt.Length - 1);
					Font.DrawText(new Rectangle(300, 228, 300, 64), Color.White, txt);

					Font.DrawText(new Rectangle(294, 256, 300, 64), Color.White, "STR " + CurrentHero.Strength.Value.ToString());
					Font.DrawText(new Rectangle(294, 276, 300, 64), Color.White, "INT " + CurrentHero.Intelligence.Value.ToString());
					Font.DrawText(new Rectangle(294, 296, 300, 64), Color.White, "WIS " + CurrentHero.Wisdom.Value.ToString());
					Font.DrawText(new Rectangle(294, 316, 300, 64), Color.White, "DEX " + CurrentHero.Dexterity.Value.ToString());
					Font.DrawText(new Rectangle(294, 336, 300, 64), Color.White, "CON " + CurrentHero.Constitution.Value.ToString());
					Font.DrawText(new Rectangle(294, 356, 300, 64), Color.White, "CHA " + CurrentHero.Charisma.Value.ToString());
					Font.DrawText(new Rectangle(462, 256, 300, 64), Color.White, "AC  " + CurrentHero.ArmorClass.ToString());
					Font.DrawText(new Rectangle(462, 276, 300, 64), Color.White, "HP  " + CurrentHero.HitPoint.Max.ToString());
					Font.DrawText(new Rectangle(462, 296, 300, 64), Color.White, "LVL ");


					// Left/right box
					Tileset.Draw(3, new Point(288, 132));
					Tileset.Draw(18, new Point(300, 140));
					Tileset.Draw(3, new Point(288, 164));
					Tileset.Draw(19, new Point(300, 172));

					// Faces
					for (int i = 0; i < 4; i++)
						Heads.Draw(i + FaceOffset, new Point(354 + i * 64, 132));

				}
				break;
				#endregion

				#region Confirm
				case CharGenStates.Confirm:
				{
					// Class and professions
					Font.DrawText(new Rectangle(300, 210, 300, 64), Color.White, CurrentHero.Race.ToString());
					string txt = string.Empty;
					foreach (Profession prof in CurrentHero.Professions)
						txt += prof.Class.ToString() + "/";
					txt = txt.Substring(0, txt.Length - 1);
					Font.DrawText(new Rectangle(300, 228, 300, 64), Color.White, txt);

					Font.DrawText(new Rectangle(294, 256, 300, 64), Color.White, "STR " + CurrentHero.Strength.Value.ToString());
					Font.DrawText(new Rectangle(294, 276, 300, 64), Color.White, "INT " + CurrentHero.Intelligence.Value.ToString());
					Font.DrawText(new Rectangle(294, 296, 300, 64), Color.White, "WIS " + CurrentHero.Wisdom.Value.ToString());
					Font.DrawText(new Rectangle(294, 316, 300, 64), Color.White, "DEX " + CurrentHero.Dexterity.Value.ToString());
					Font.DrawText(new Rectangle(294, 336, 300, 64), Color.White, "CON " + CurrentHero.Constitution.Value.ToString());
					Font.DrawText(new Rectangle(294, 356, 300, 64), Color.White, "CHA " + CurrentHero.Charisma.Value.ToString());
					Font.DrawText(new Rectangle(462, 256, 300, 64), Color.White, "AC  " + CurrentHero.ArmorClass.ToString());
					Font.DrawText(new Rectangle(462, 276, 300, 64), Color.White, "HP  " + CurrentHero.HitPoint.Max.ToString());
					Font.DrawText(new Rectangle(462, 296, 300, 64), Color.White, "LVL ");

					Heads.Draw(CurrentHero.Head, new Point(438, 132));

					// Reroll
					Tileset.Draw(5, new Point(448, 318));
					Tileset.Draw(11, new Point(462, 330));

					// Faces
					Tileset.Draw(5, new Point(448, 350));
					Tileset.Draw(20, new Point(466, 362));

					// Modify
					Tileset.Draw(5, new Point(528, 316));
					Tileset.Draw(14, new Point(540, 328));

					// Keep
					Tileset.Draw(5, new Point(528, 350));
					Tileset.Draw(13, new Point(550, 360));
				}
				break;
				#endregion

				#region Select name
				case CharGenStates.SelectName:
				{
					//
					Font.DrawText(new Rectangle(296, 200, 300, 64), Color.FromArgb(85, 255, 255), "Name: " + CurrentHero.Name);

					Font.DrawText(new Rectangle(294, 256, 300, 64), Color.White, "STR " + CurrentHero.Strength.Value.ToString());
					Font.DrawText(new Rectangle(294, 276, 300, 64), Color.White, "INT " + CurrentHero.Intelligence.Value.ToString());
					Font.DrawText(new Rectangle(294, 296, 300, 64), Color.White, "WIS " + CurrentHero.Wisdom.Value.ToString());
					Font.DrawText(new Rectangle(294, 316, 300, 64), Color.White, "DEX " + CurrentHero.Dexterity.Value.ToString());
					Font.DrawText(new Rectangle(294, 336, 300, 64), Color.White, "CON " + CurrentHero.Constitution.Value.ToString());
					Font.DrawText(new Rectangle(294, 356, 300, 64), Color.White, "CHA " + CurrentHero.Charisma.Value.ToString());
					Font.DrawText(new Rectangle(462, 256, 300, 64), Color.White, "AC  " + CurrentHero.ArmorClass.ToString());
					Font.DrawText(new Rectangle(462, 276, 300, 64), Color.White, "HP  " + CurrentHero.HitPoint.Max.ToString());
					Font.DrawText(new Rectangle(462, 296, 300, 64), Color.White, "LVL ");

					Heads.Draw(CurrentHero.Head, new Point(438, 132));
				}
				break;
				#endregion

				#region Delete hero
				case CharGenStates.Delete:
				{
					Heads.Draw(CurrentHero.Head, new Point(438, 132));
					Font.DrawText(new Rectangle(292, 190, 300, 64), Color.White, CurrentHero.Name);

					// Class and professions
					Font.DrawText(new Rectangle(300, 214, 300, 64), Color.White, CurrentHero.Race.ToString());
					string txt = string.Empty;
					foreach (Profession prof in CurrentHero.Professions)
						txt += prof.Class.ToString() + "/";
					txt = txt.Substring(0, txt.Length - 1);
					Font.DrawText(new Rectangle(300, 232, 300, 64), Color.White, txt);

					Font.DrawText(new Rectangle(294, 256, 300, 64), Color.White, "STR " + CurrentHero.Strength.Value.ToString());
					Font.DrawText(new Rectangle(294, 276, 300, 64), Color.White, "INT " + CurrentHero.Intelligence.Value.ToString());
					Font.DrawText(new Rectangle(294, 296, 300, 64), Color.White, "WIS " + CurrentHero.Wisdom.Value.ToString());
					Font.DrawText(new Rectangle(294, 316, 300, 64), Color.White, "DEX " + CurrentHero.Dexterity.Value.ToString());
					Font.DrawText(new Rectangle(294, 336, 300, 64), Color.White, "CON " + CurrentHero.Constitution.Value.ToString());
					Font.DrawText(new Rectangle(294, 356, 300, 64), Color.White, "CHA " + CurrentHero.Charisma.Value.ToString());
					Font.DrawText(new Rectangle(462, 256, 300, 64), Color.White, "AC  " + CurrentHero.ArmorClass.ToString());
					Font.DrawText(new Rectangle(462, 276, 300, 64), Color.White, "HP  " + CurrentHero.HitPoint.Max.ToString());
					Font.DrawText(new Rectangle(462, 296, 300, 64), Color.White, "LVL ");

					// Delete
					Tileset.Draw(7, new Point(448, 350));

					// OK
					Tileset.Draw(5, new Point(528, 350));
					Tileset.Draw(15, new Point(558, 360));

				}
				break;
				#endregion
			}





			// Draw the cursor or the item in the hand
			Display.Color = Color.White;
			Tileset.Draw(999, Mouse.Location);
		}


		#endregion


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Keyboard_OnKeyDown(object sender, PreviewKeyDownEventArgs e)
		{

			switch (e.KeyCode)
			{
				case Keys.Back:
				if (CurrentHero.Name.Length > 0)
					CurrentHero.Name = CurrentHero.Name.Substring(0, CurrentHero.Name.Length - 1);
				break;

				case Keys.Space:
				CurrentHero.Name += " ";
				break;

				case Keys.Add:
				case Keys.Alt:
				case Keys.Apps:
				case Keys.Attn:
				case Keys.BrowserBack:
				case Keys.BrowserFavorites:
				case Keys.BrowserForward:
				case Keys.BrowserHome:
				case Keys.BrowserRefresh:
				case Keys.BrowserSearch:
				case Keys.BrowserStop:
				case Keys.Cancel:
				case Keys.Capital:
				case Keys.Clear:
				case Keys.Control:
				case Keys.ControlKey:
				case Keys.Crsel:
				case Keys.D:
				case Keys.D0:
				case Keys.D1:
				case Keys.D2:
				case Keys.D3:
				case Keys.D4:
				case Keys.D5:
				case Keys.D6:
				case Keys.D7:
				case Keys.D8:
				case Keys.D9:
				case Keys.Decimal:
				case Keys.Delete:
				case Keys.Divide:
				case Keys.Down:
				case Keys.End:
				case Keys.EraseEof:
				case Keys.Escape:
				case Keys.Execute:
				case Keys.Exsel:
				case Keys.F1:
				case Keys.F10:
				case Keys.F11:
				case Keys.F12:
				case Keys.F13:
				case Keys.F14:
				case Keys.F15:
				case Keys.F16:
				case Keys.F17:
				case Keys.F18:
				case Keys.F19:
				case Keys.F2:
				case Keys.F20:
				case Keys.F21:
				case Keys.F22:
				case Keys.F23:
				case Keys.F24:
				case Keys.F3:
				case Keys.F4:
				case Keys.F5:
				case Keys.F6:
				case Keys.F7:
				case Keys.F8:
				case Keys.F9:
				case Keys.FinalMode:
				case Keys.HanguelMode:
				case Keys.HanjaMode:
				case Keys.Help:
				case Keys.Home:
				case Keys.IMEAccept:
				case Keys.IMEConvert:
				case Keys.IMEModeChange:
				case Keys.IMENonconvert:
				case Keys.Insert:
				case Keys.JunjaMode:
				case Keys.KeyCode:
				case Keys.LButton:
				case Keys.LControlKey:
				case Keys.LMenu:
				case Keys.LShiftKey:
				case Keys.LWin:
				case Keys.LaunchApplication1:
				case Keys.LaunchApplication2:
				case Keys.LaunchMail:
				case Keys.Left:
				case Keys.LineFeed:
				case Keys.MButton:
				case Keys.MediaNextTrack:
				case Keys.MediaPlayPause:
				case Keys.MediaPreviousTrack:
				case Keys.MediaStop:
				case Keys.Menu:
				case Keys.Modifiers:
				case Keys.Multiply:
				case Keys.Next:
				case Keys.NoName:
				case Keys.None:
				case Keys.NumLock:
				case Keys.Oem1:
				case Keys.Oem102:
				case Keys.Oem2:
				case Keys.Oem3:
				case Keys.Oem4:
				case Keys.Oem5:
				case Keys.Oem6:
				case Keys.Oem7:
				case Keys.Oem8:
				case Keys.OemClear:
				case Keys.OemMinus:
				case Keys.OemPeriod:
				case Keys.Oemcomma:
				case Keys.Oemplus:
				case Keys.Pa1:
				case Keys.Packet:
				case Keys.PageUp:
				case Keys.Pause:
				case Keys.Play:
				case Keys.Print:
				case Keys.PrintScreen:
				case Keys.ProcessKey:
				case Keys.RButton:
				case Keys.RControlKey:
				case Keys.RMenu:
				case Keys.RShiftKey:
				case Keys.RWin:
				case Keys.Scroll:
				case Keys.Select:
				case Keys.SelectMedia:
				case Keys.Separator:
				case Keys.Shift:
				case Keys.ShiftKey:
				case Keys.Sleep:
				case Keys.Subtract:
				case Keys.VolumeDown:
				case Keys.VolumeMute:
				case Keys.VolumeUp:
				case Keys.XButton1:
				case Keys.XButton2:
				break;


				case Keys.Return:

				Keyboard.OnKeyDown -= new EventHandler<PreviewKeyDownEventArgs>(Keyboard_OnKeyDown);
				CurrentState = CharGenStates.SelectHero;
				break;

				default:
				{
					if (CurrentHero.Name != null && CurrentHero.Name.Length > 10)
						break;

					if (Keyboard.IsKeyPress(Keys.ShiftKey))
						CurrentHero.Name += e.KeyCode;
					else
						CurrentHero.Name += (char)(e.KeyCode + 32);
				}
				break;
			}


		}


		#region Properties


		/// <summary>
		/// Current state
		/// </summary>
		CharGenStates CurrentState;


		/// <summary>
		/// Returns true if the team is ready to play
		/// </summary>
		bool IsTeamReadyToPlay
		{
			get
			{
				for (int id = 0; id < 4; id++)
				{
					if (Heroes[id] == null)
						return false;
				}

				return true;
			}
		}


		/// <summary>
		/// Face id offset
		/// </summary>
		int FaceOffset;


		/// <summary>
		/// Tileset
		/// </summary>
		TileSet Tileset;


		/// <summary>
		/// Bitmap font
		/// </summary>
		Font2d Font;


		/// <summary>
		/// Play button
		/// </summary>
		ScreenButton PlayButton;



		/// <summary>
		/// Heroes in the team
		/// </summary>
		Hero[] Heroes;


		/// <summary>
		/// Currently selected hero
		/// </summary>
		Hero CurrentHero
		{
			get
			{
				if (HeroID == -1)
					return null;

				return Heroes[HeroID];
			}
		}


		/// <summary>
		/// ID of the current hero
		/// </summary>
		int HeroID;


		/// <summary>
		/// String table for translations
		/// </summary>
		StringTable StringTable;


		/// <summary>
		/// Animations
		/// </summary>
		Animation Anims;


		/// <summary>
		/// Heroe's box
		/// </summary>
		Rectangle[] HeroeBoxes;

		/// <summary>
		/// 
		/// </summary>
		Point[] NameLocations;


		/// <summary>
		/// Back button
		/// </summary>
		Rectangle BackButton;


		/// <summary>
		/// Hero's heads
		/// </summary>
		TileSet Heads;


		/// <summary>
		/// 
		/// </summary>
		Font2d NameFont;

		#endregion
	}


	/// <summary>
	/// Differents states of the team generation
	/// </summary>
	enum CharGenStates
	{
		SelectRace,
		SelectHero,
		SelectClass,
		SelectAlignment,
		SelectFace,
		Confirm,
		SelectName,
		Delete,
	}
}