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
using ArcEngine;
using ArcEngine.Graphic;
using ArcEngine.Asset;

namespace DungeonEye.Forms
{
	/// <summary>
	/// Square editor form
	/// </summary>
	public partial class SquareForm : Form
	{

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="maze">Maze</param>
		/// <param name="sqaure">Square handle</param>
		public SquareForm(Maze maze, Square square)
		{
			InitializeComponent();

			if (square == null)
			{
				Close();
				return;
			}

			Maze = maze;

			List<string> list = ResourceManager.GetAssets<Item>();
			ItemsBox.DataSource = list;


			#region Items

			NWBox.BeginUpdate();
			NEBox.BeginUpdate();
			SWBox.BeginUpdate();
			SEBox.BeginUpdate();

			foreach (Item item in square.Items[0])
				NWBox.Items.Add(item.Name);
			foreach (Item item in square.Items[1])
				NEBox.Items.Add(item.Name);
			foreach (Item item in square.Items[2])
				SWBox.Items.Add(item.Name);
			foreach (Item item in square.Items[3])
				SEBox.Items.Add(item.Name);

			NWBox.EndUpdate();
			NEBox.EndUpdate();
			SWBox.EndUpdate();
			SEBox.EndUpdate();
			#endregion


			#region Monsters
			SetMonster(SquarePosition.NorthWest, square.Monsters[0]);
			SetMonster(SquarePosition.NorthEast, square.Monsters[1]);
			SetMonster(SquarePosition.SouthWest, square.Monsters[2]);
			SetMonster(SquarePosition.SouthEast, square.Monsters[3]);

			#endregion

			Square = square;
		}


		/// <summary>
		/// Activate the actor tab
		/// </summary>
		public void ActivateActorTab()
		{
			TabControlBox.SelectedTab = ActorTab;
		}


		/// <summary>
		/// Render the decoration scene
		/// </summary>
		void RenderDecorationScene()
		{
			GlDecorationControl.MakeCurrent();
			Display.ClearBuffers();

			if (Batch != null)
			{
				Batch.Begin();

				Batch.DrawTile(Maze.WallTileset, 0, Point.Empty);

				TileDrawing td = DisplayCoordinates.GetWalls(ViewFieldPosition.L)[0];
				Batch.DrawTile(Maze.WallTileset, td.ID, td.Location);


				// Draw the decoration
				if (Maze.Decoration != null)
					Maze.Decoration.Draw(Batch, (int) DecorationIdBox.Value, ViewFieldPosition.L);

				Batch.End();
			}


			GlDecorationControl.SwapBuffers();
		}


		/// <summary>
		/// Decoration changed
		/// </summary>
		void ChangeDecoration()
		{
			if (Square == null)
				return;

			DecorationIdBox.Value = Square.Decorations[(int)DecorationSide];
		}


		/// <summary>
		/// Update the visual states of the decoration boxes
		/// </summary>
		void UpdateDecorationBoxes()
		{
			if (Square == null)
				return;

			NorthDecorationBox.ForeColor = Square.Decorations[(int) CardinalPoint.North] == -1 ? Color.Black : Color.Red;
			SouthDecorationBox.ForeColor = Square.Decorations[(int) CardinalPoint.South] == -1 ? Color.Black : Color.Red;
			WestDecorationBox.ForeColor = Square.Decorations[(int) CardinalPoint.West] == -1 ? Color.Black : Color.Red;
			EastDecorationBox.ForeColor = Square.Decorations[(int) CardinalPoint.East] == -1 ? Color.Black : Color.Red;
		}


		#region Items events


		/// <summary>
		/// Removes all items
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClearAllItemsBox_Click(object sender, EventArgs e)
		{
			if (Square == null)
				return;

			if (MessageBox.Show("Are you sure ?", "Remove all items", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
				return;

			Square.Items[0].Clear();
			Square.Items[1].Clear();
			Square.Items[2].Clear();
			Square.Items[3].Clear();

			NWBox.Items.Clear();
			NEBox.Items.Clear();
			SWBox.Items.Clear();
			SEBox.Items.Clear();

		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NWAddItem_Click(object sender, EventArgs e)
		{
			if (Square == null)
				return;
			
			Square.Items[0].Add(ResourceManager.CreateAsset<Item>(ItemsBox.SelectedItem as string));
			NWBox.Items.Add(ItemsBox.SelectedItem as string);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NEAddItem_Click(object sender, EventArgs e)
		{
			if (Square == null)
				return;

			Square.Items[1].Add(ResourceManager.CreateAsset<Item>(ItemsBox.SelectedItem as string));
			NEBox.Items.Add(ItemsBox.SelectedItem as string);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NERemoveItem_Click(object sender, EventArgs e)
		{
			if (NEBox.SelectedIndex == -1)
				return;

			Square.Items[1].RemoveAt(NEBox.SelectedIndex);

			NEBox.Items.Clear();
			foreach (Item item in Square.Items[1])
				NEBox.Items.Add(item.Name);

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NWRemoveItem_Click(object sender, EventArgs e)
		{
			if (Square == null || NWBox.SelectedIndex == -1)
				return;

			Square.Items[0].RemoveAt(NWBox.SelectedIndex);

			NWBox.Items.Clear();
			foreach (Item item in Square.Items[0])
				NWBox.Items.Add(item.Name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SWAddItem_Click(object sender, EventArgs e)
		{
			if (Square == null)
				return;

			Square.Items[2].Add(ResourceManager.CreateAsset<Item>(ItemsBox.SelectedItem as string));
			SWBox.Items.Add(ItemsBox.SelectedItem as string);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SEAddItem_Click(object sender, EventArgs e)
		{
			if (Square == null)
				return;

			Square.Items[3].Add(ResourceManager.CreateAsset<Item>(ItemsBox.SelectedItem as string));
			SEBox.Items.Add(ItemsBox.SelectedItem as string);

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SERemoveItem_Click(object sender, EventArgs e)
		{
			if (Square == null || SEBox.SelectedIndex == -1)
				return;

			Square.Items[3].RemoveAt(SEBox.SelectedIndex);

			SEBox.Items.Clear();
			foreach (Item item in Square.Items[3])
				SEBox.Items.Add(item.Name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SWRemoveItem_Click(object sender, EventArgs e)
		{
			if (Square == null || SWBox.SelectedIndex == -1)
				return;

			Square.Items[2].RemoveAt(SWBox.SelectedIndex);

			SWBox.Items.Clear();
			foreach (Item item in Square.Items[2])
				SWBox.Items.Add(item.Name);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NWBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (Square == null || NWBox.SelectedIndex == -1)
				return;

			Square.Items[0].RemoveAt(NWBox.SelectedIndex);
			NWBox.Items.RemoveAt(NWBox.SelectedIndex);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NEBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (Square == null || NEBox.SelectedIndex == -1)
				return;

			Square.Items[1].RemoveAt(NEBox.SelectedIndex);
			NEBox.Items.RemoveAt(NEBox.SelectedIndex);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SEBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (Square == null || SEBox.SelectedIndex == -1)
				return;

			Square.Items[3].RemoveAt(SEBox.SelectedIndex);
			SEBox.Items.RemoveAt(SEBox.SelectedIndex);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SWBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (Square == null || SWBox.SelectedIndex == -1)
				return;

			Square.Items[2].RemoveAt(SWBox.SelectedIndex);
			SWBox.Items.RemoveAt(SWBox.SelectedIndex);
		}


		#endregion


		#region Form events

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SquareForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (Batch != null)
				Batch.Dispose();
			Batch = null;
		}
		
		
		/// <summary>
		/// OnKeyDown
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MazeBlockForm_KeyDown(object sender, KeyEventArgs e)
		{
			// Escape key close the form
			if (e.KeyCode == Keys.Escape)
				Close();
		}


		#endregion


		#region Monsters

		/// <summary>
		/// Removes all monsters
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RemoveAllMonstersBox_Click(object sender, EventArgs e)
		{
			if (Square == null)
				return;

			if (MessageBox.Show("Are you sure ?", "Remove all monsters", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
				return;

			SetMonster(SquarePosition.NorthWest, null);
			SetMonster(SquarePosition.NorthEast, null);
			SetMonster(SquarePosition.SouthWest, null);
			SetMonster(SquarePosition.SouthEast, null);
		}


		/// <summary>
		/// Definies a monster
		/// </summary>
		/// <param name="position">Square position</param>
		/// <param name="monster">Monster handle or null</param>
		void SetMonster(SquarePosition position, Monster monster)
		{
			TextBox[] boxes = new TextBox[]
			{
				NWMonsterBox,
				NEMonsterBox,
				SWMonsterBox,
				SEMonsterBox,
			};

			Button[] buttons = new Button[]
			{
				DeleteNWBox,
				DeleteNEBox,
				DeleteSWBox,
				DeleteSEBox,
			};

			// Oops ! Wrong position.
			if (position == SquarePosition.Center)
				return;

			if (monster == null)
			{
				boxes[(int)position].Text = string.Empty;
				buttons[(int)position].Enabled = false;
				if (Square != null)
					Square.Monsters[(int) position] = null;
			}
			else
			{
				boxes[(int)position].Text = monster.Name;
				buttons[(int)position].Enabled = true;
				monster.Teleport(Square);
				monster.Position = position;
			}
		}


		private void EditNWBox_Click(object sender, EventArgs e)
		{
			if (Square == null)
				return;

			if (Square.Monsters[0] == null)
				Square.Monsters[0] = new Monster();

			new MonsterEditorForm(Square.Monsters[0]).ShowDialog();
			
			SetMonster(SquarePosition.NorthWest, Square.Monsters[0]);
			Square.Monsters[0].OnSpawn();

		}

		private void EditNEBox_Click(object sender, EventArgs e)
		{
			if (Square == null)
				return;


			if (Square.Monsters[1] == null)
				Square.Monsters[1] = new Monster();

			new MonsterEditorForm(Square.Monsters[1]).ShowDialog();
			Square.Monsters[1].OnSpawn();

			SetMonster(SquarePosition.NorthEast, Square.Monsters[1]);
		}

		private void EditSWBox_Click(object sender, EventArgs e)
		{
			if (Square == null)
				return;


			if (Square.Monsters[2] == null)
				Square.Monsters[2] = new Monster();

			new MonsterEditorForm(Square.Monsters[2]).ShowDialog();
			Square.Monsters[2].OnSpawn();

			SetMonster(SquarePosition.SouthWest, Square.Monsters[2]);
		}

		private void EditSEBox_Click(object sender, EventArgs e)
		{
			if (Square == null)
				return;

			if (Square.Monsters[3] == null)
				Square.Monsters[3] = new Monster();

			new MonsterEditorForm(Square.Monsters[3]).ShowDialog();
			Square.Monsters[3].OnSpawn();

			SetMonster(SquarePosition.SouthEast, Square.Monsters[3]);
		}




		private void DeleteNWBox_Click(object sender, EventArgs e)
		{
			if (Square == null)
				return;

			if (MessageBox.Show("Are you sure ?", "Remove monster", MessageBoxButtons.YesNo) == DialogResult.Yes)
				SetMonster(SquarePosition.NorthWest, null);
		}
		
		private void DeleteNEBox_Click(object sender, EventArgs e)
		{
			if (Square == null)
				return;

			if (MessageBox.Show("Are you sure ?", "Remove monster", MessageBoxButtons.YesNo) == DialogResult.Yes)
				SetMonster(SquarePosition.NorthEast, null);
		}
	
		private void DeleteSWBox_Click(object sender, EventArgs e)
		{
			if (Square == null)
				return;

			if (MessageBox.Show("Are you sure ?", "Remove monster", MessageBoxButtons.YesNo) == DialogResult.Yes)
				SetMonster(SquarePosition.SouthWest, null);
		}

		private void DeleteSEBox_Click(object sender, EventArgs e)
		{
			if (Square == null)
				return;

			if (MessageBox.Show("Are you sure ?", "Remove monster", MessageBoxButtons.YesNo) == DialogResult.Yes)
				SetMonster(SquarePosition.SouthEast, null);
		}

		#endregion


		#region Decoration

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GlDecorationControl_Load(object sender, EventArgs e)
		{
			GlDecorationControl.MakeCurrent();
			Display.Init();

			Batch = new SpriteBatch();

			ChangeDecoration();
			
			UpdateDecorationBoxes();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GlDecorationControl_Resize(object sender, EventArgs e)
		{
			GlDecorationControl.MakeCurrent();
			Display.ViewPort = new Rectangle(Point.Empty, GlDecorationControl.Size);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GlDecorationControl_Paint(object sender, PaintEventArgs e)
		{
			RenderDecorationScene();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DecorationIdBox_ValueChanged(object sender, EventArgs e)
		{
			if (Square == null)
				return;

			Square.Decorations[(int)DecorationSide] = (int)DecorationIdBox.Value;

			RenderDecorationScene();
			UpdateDecorationBoxes();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NorthDecorationBox_CheckedChanged(object sender, EventArgs e)
		{
			DecorationSide = CardinalPoint.North;
			ChangeDecoration();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WestDecorationBox_CheckedChanged(object sender, EventArgs e)
		{
			DecorationSide = CardinalPoint.West;
			ChangeDecoration();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EastDecorationBox_CheckedChanged(object sender, EventArgs e)
		{
			DecorationSide = CardinalPoint.East;
			ChangeDecoration();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SouthDecorationBox_CheckedChanged(object sender, EventArgs e)
		{
			DecorationSide = CardinalPoint.South;
			ChangeDecoration();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClearDecorationBox_Click(object sender, EventArgs e)
		{
			if (Square == null)
				return;

			Square.Decorations[0] = -1;
			Square.Decorations[1] = -1;
			Square.Decorations[2] = -1;
			Square.Decorations[3] = -1;

			DecorationIdBox.Value = -1;

			RenderDecorationScene();
			UpdateDecorationBoxes();
		}

		#endregion


		#region Tab events

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeleteActorBox_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Erase actor ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			Square.Actor = null;
			ActorPanelBox.Controls.Clear();
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ActorTab_Enter(object sender, EventArgs e)
		{
			// No actor
			if (Square.Actor == null)
			{
				ActorPanelBox.Controls.Clear();
				return;
			}

			// Control already present
			if (ActorPanelBox.Controls.Count > 0)
				return;


			UserControl control = null;


			if (Square.Actor is Door)
			{
				control = new DoorControl(Square.Actor as Door);
			}
			else if (Square.Actor is EventSquare)
			{
				control = new EventSquareControl(Square.Actor as EventSquare, Maze.Dungeon);
			}
			else if (Square.Actor is PressurePlate)
			{
				control = new PressurePlateControl(Square.Actor as PressurePlate, Maze.Dungeon);
			}
			else if (Square.Actor is ForceField)
			{
				control = new ForceFieldControl(Square.Actor as ForceField, Maze.Dungeon);
			}
			else if (Square.Actor is Pit)
			{
				control = new PitControl(Square.Actor as Pit, Maze.Dungeon);
			}
			else if (Square.Actor is Stair)
			{
				control = new StairControl(Square.Actor as Stair, Maze.Dungeon);
			}
			else if (Square.Actor is Teleporter)
			{
				control = new TeleporterControl(Square.Actor as Teleporter, Maze.Dungeon);
			}
			else if (Square.Actor is AlcoveActor)
			{
				control = new AlcoveControl(Square.Actor as AlcoveActor, Maze);
			}
			else if (Square.Actor is WallSwitch)
			{
				control = new WallSwitchControl(Square.Actor as WallSwitch, Maze);
			}
			else if (Square.Actor is Generator)
			{
				control = new GeneratorControl(Square.Actor as WallSwitch, Maze);
			}
			else if (Square.Actor is Launcher)
			{
				control = new LauncherControl(Square.Actor as WallSwitch, Maze);
			}
			else
			{
				MessageBox.Show("Unhandled actor control", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			if (control != null)
			{
				control.Dock = DockStyle.Fill;
				ActorPanelBox.Controls.Add(control);
			}
		}
		
		
		#endregion


		#region Properties

		/// <summary>
		/// Maze handle
		/// </summary>
		Maze Maze;

		/// <summary>
		/// Square to edit
		/// </summary>
		Square Square;


		/// <summary>
		/// Spritabatch handle
		/// </summary>
		SpriteBatch Batch;


		/// <summary>
		/// Current side for the decoration
		/// </summary>
		CardinalPoint DecorationSide;

		#endregion



	}
}