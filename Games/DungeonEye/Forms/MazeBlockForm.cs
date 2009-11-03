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
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Windows.Forms;
using ArcEngine;
using ArcEngine.Forms;
using ArcEngine.Graphic;
using ArcEngine.Providers;
using ArcEngine.Asset;


namespace DungeonEye.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public partial class MazeBlockForm : AssetEditor
	{

		/// <summary>
		/// 
		/// </summary>
		/// <param name="maze"></param>
		/// <param name="position"></param>
		public MazeBlockForm(Maze maze, MazeBlock block)
		{
			InitializeComponent();
			MonsterBox.Init();

			if (block == null)
			{
				Close();
				return;
			}
			MazeBlock = block;
			Maze = maze;

			#region Ground items

			Itemset = ResourceManager.CreateAsset<ItemSet>("");

			NWBox.BeginUpdate();
			NWBox.Items.Clear();
			NEBox.BeginUpdate();
			NEBox.Items.Clear();
			SWBox.BeginUpdate();
			SWBox.Items.Clear();
			SEBox.BeginUpdate();
			SEBox.Items.Clear();

			foreach (Item item in MazeBlock.GroundItems[0])
				NWBox.Items.Add(item.Name);
			foreach (Item item in MazeBlock.GroundItems[1])
				NEBox.Items.Add(item.Name);
			foreach (Item item in MazeBlock.GroundItems[2])
				SWBox.Items.Add(item.Name);
			foreach (Item item in MazeBlock.GroundItems[3])
				SEBox.Items.Add(item.Name);

			NWBox.EndUpdate();
			NEBox.EndUpdate();
			SWBox.EndUpdate();
			SEBox.EndUpdate();


			NWItemsBox.BeginUpdate();
			NWItemsBox.Items.Clear();
			NEItemsBox.BeginUpdate();
			NEItemsBox.Items.Clear();
			SWItemsBox.BeginUpdate();
			SWItemsBox.Items.Clear();
			SEItemsBox.BeginUpdate();
			SEItemsBox.Items.Clear();

			foreach (string item in Itemset.Items.Keys)
			{
				NWItemsBox.Items.Add(item);
				NEItemsBox.Items.Add(item);
				SWItemsBox.Items.Add(item);
				SEItemsBox.Items.Add(item);
			}

			NWItemsBox.EndUpdate();
			NEItemsBox.EndUpdate();
			SWItemsBox.EndUpdate();
			SEItemsBox.EndUpdate();

			#endregion


			#region Monsters



			// Add templates
			MonsterTemplateBox.BeginUpdate();
			MonsterTemplateBox.Items.Clear();

			foreach (string name in ResourceManager.GetAssets<Monster>())
				MonsterTemplateBox.Items.Add(name);

			MonsterTemplateBox.EndUpdate();


			#endregion


			#region Wall decoration

			GlWallControl.MakeCurrent();
			Display.Init();

			#endregion


			#region Specials

			#region Door

			DoorStateBox.BeginUpdate();
			DoorStateBox.Items.Clear();
			foreach (string name in Enum.GetNames(typeof(DoorState)))
				DoorStateBox.Items.Add(name);
			DoorStateBox.EndUpdate();

			DoorTypeBox.BeginUpdate();
			DoorTypeBox.Items.Clear();
			foreach (string name in Enum.GetNames(typeof(DoorType)))
				DoorTypeBox.Items.Add(name);
			DoorTypeBox.EndUpdate();

			#endregion 


			#region Floor plate

			List<string> scripts = ResourceManager.GetAssets<Script>();
			FloorPlateScriptBox.BeginUpdate();
			FloorPlateScriptBox.Items.Clear();
			foreach (string name in scripts)
				FloorPlateScriptBox.Items.Add(name);
			FloorPlateScriptBox.EndUpdate();

			if (MazeBlock.FloorPlate != null)
			{
				if (FloorPlateScriptBox.Items.Contains(MazeBlock.FloorPlate.ScriptName))
					FloorPlateScriptBox.SelectedItem = MazeBlock.FloorPlate.ScriptName;

				if (OnEnterFloorPlateBox.Items.Contains(MazeBlock.FloorPlate.OnEnterScript))
					OnEnterFloorPlateBox.SelectedItem = MazeBlock.FloorPlate.OnEnterScript;

				if (OnLeaveFloorPlateBox.Items.Contains(MazeBlock.FloorPlate.OnLeaveScript))
					OnLeaveFloorPlateBox.SelectedItem = MazeBlock.FloorPlate.OnLeaveScript;
			}



			#endregion

			#region Force Field
			ForceFieldTypeBox.BeginUpdate();
			ForceFieldTypeBox.Items.Clear();
			foreach (string name in Enum.GetNames(typeof(ForceFieldType)))
				ForceFieldTypeBox.Items.Add(name);
			ForceFieldTypeBox.EndUpdate();

			ForceFieldRotationBox.BeginUpdate();
			ForceFieldRotationBox.Items.Clear();
			foreach (string name in Enum.GetNames(typeof(CompassRotation)))
				ForceFieldRotationBox.Items.Add(name);
			ForceFieldRotationBox.EndUpdate();


			ForceFieldMoveBox.BeginUpdate();
			ForceFieldMoveBox.Items.Clear();
			foreach (string name in Enum.GetNames(typeof(CardinalPoint)))
				ForceFieldMoveBox.Items.Add(name);
			ForceFieldMoveBox.EndUpdate();

			#endregion

			#endregion

		}


		#region Ground Items events


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NWAddItem_Click(object sender, EventArgs e)
		{
			MazeBlock.GroundItems[0].Add(Itemset.GetItem(NWItemsBox.SelectedItem as string));
			NWBox.Items.Add(NWItemsBox.SelectedItem as string);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NEAddItem_Click(object sender, EventArgs e)
		{
			MazeBlock.GroundItems[1].Add(Itemset.GetItem(NEItemsBox.SelectedItem as string));
			NEBox.Items.Add(NEItemsBox.SelectedItem as string);
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

			MazeBlock.GroundItems[1].RemoveAt(NEBox.SelectedIndex);

			NEBox.Items.Clear();
			foreach (Item item in MazeBlock.GroundItems[1])
				NEBox.Items.Add(item.Name);

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NWRemoveItem_Click(object sender, EventArgs e)
		{
			if (NWBox.SelectedIndex == -1)
				return;

			MazeBlock.GroundItems[0].RemoveAt(NWBox.SelectedIndex);

			NWBox.Items.Clear();
			foreach (Item item in MazeBlock.GroundItems[0])
				NWBox.Items.Add(item.Name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SWAddItem_Click(object sender, EventArgs e)
		{
			MazeBlock.GroundItems[2].Add(Itemset.GetItem(SWItemsBox.SelectedItem as string));
			SWBox.Items.Add(SWItemsBox.SelectedItem as string);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SEAddItem_Click(object sender, EventArgs e)
		{
			MazeBlock.GroundItems[3].Add(Itemset.GetItem(SEItemsBox.SelectedItem as string));
			SEBox.Items.Add(SEItemsBox.SelectedItem as string);

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SERemoveItem_Click(object sender, EventArgs e)
		{
			if (SEBox.SelectedIndex == -1)
				return;

			MazeBlock.GroundItems[3].RemoveAt(SEBox.SelectedIndex);

			SEBox.Items.Clear();
			foreach (Item item in MazeBlock.GroundItems[3])
				SEBox.Items.Add(item.Name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SWRemoveItem_Click(object sender, EventArgs e)
		{
			if (SWBox.SelectedIndex == -1)
				return;

			MazeBlock.GroundItems[2].RemoveAt(SWBox.SelectedIndex);

			SWBox.Items.Clear();
			foreach (Item item in MazeBlock.GroundItems[2])
				SWBox.Items.Add(item.Name);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NWBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (NWBox.SelectedIndex == -1)
				return;

			MazeBlock.GroundItems[0].RemoveAt(NWBox.SelectedIndex);
			NWBox.Items.RemoveAt(NWBox.SelectedIndex);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NEBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (NEBox.SelectedIndex == -1)
				return;

			MazeBlock.GroundItems[1].RemoveAt(NEBox.SelectedIndex);
			NEBox.Items.RemoveAt(NEBox.SelectedIndex);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SEBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (SEBox.SelectedIndex == -1)
				return;

			MazeBlock.GroundItems[3].RemoveAt(SEBox.SelectedIndex);
			SEBox.Items.RemoveAt(SEBox.SelectedIndex);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SWBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (SWBox.SelectedIndex == -1)
				return;

			MazeBlock.GroundItems[2].RemoveAt(SWBox.SelectedIndex);
			SWBox.Items.RemoveAt(SWBox.SelectedIndex);
		}



		#endregion


		#region Form events


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


		#region Monster events


		/// <summary>
		/// Applies a template
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ApplyMonsterTemplateBox_Click(object sender, EventArgs e)
		{
			if (GroundLocationBox.SelectedIndex == -1 || MonsterTemplateBox.SelectedItem == null)
				return;
			

			Monster monster = ResourceManager.CreateAsset<Monster>(MonsterTemplateBox.SelectedItem as string);
			monster.Location = new DungeonLocation();
			monster.Location.Position = MazeBlock.Location;
			monster.Location.GroundPosition = (GroundPosition)Enum.ToObject(typeof(GroundPosition), GroundLocationBox.SelectedIndex);
			monster.Location.Maze = Maze.Name;
			monster.Init();


			Maze.Monsters.Add(monster);
			MonsterBox.Monster = monster;
		}


		/// <summary>
		/// Selects an existing monster
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GroundLocationBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (GroundLocationBox.SelectedIndex == -1)
				return;

			Monster[] monsters = Maze.GetMonsters(MazeBlock.Location);
			MonsterBox.Monster = monsters[GroundLocationBox.SelectedIndex];
		}



		/// <summary>
		/// Removes a monster 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button2_Click(object sender, EventArgs e)
		{
			if (GroundLocationBox.SelectedIndex == -1)
				return;

			Maze.Monsters.Remove(MonsterBox.Monster);
			MonsterBox.Monster = null;

		}


		#endregion


		#region Wall decoration

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GlWallControl_Resize(object sender, EventArgs e)
		{
			GlWallControl.MakeCurrent();
			Display.ViewPort = new Rectangle(Point.Empty, GlWallControl.Size);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GlWallControl_Paint(object sender, PaintEventArgs e)
		{
			GlWallControl.MakeCurrent();
			Display.ClearBuffers();



			GlWallControl.SwapBuffers();
		}


		#endregion


		#region Specials

		/// <summary>
		/// Change special type of block
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SpecialTypeBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (SpecialTypeBox.SelectedIndex)
			{
				// None
				case 0:
				{
					StairGroupBox.Enabled = false;
					PitGroupBox.Enabled = false;
					DoorGroupBox.Enabled = false;
					ForceFieldGroupBox.Enabled = false;
					TeleporterGroupBox.Enabled = false;
					PlateGroupBox.Enabled = false;

					MazeBlock.RemoveSpecials();
				}
				break;

				// Door
				case 1:
				{
					StairGroupBox.Enabled = false;
					PitGroupBox.Enabled = false;
					DoorGroupBox.Enabled = true;
					ForceFieldGroupBox.Enabled = false;
					TeleporterGroupBox.Enabled = false;
					PlateGroupBox.Enabled = false;


					// If no door present, then add one
					if (MazeBlock.Door == null)
					{
						MazeBlock.RemoveSpecials();
						MazeBlock.Door = new Door();
						MazeBlock.Door.Init();
					}
				}
				break;

				// Floor Plate
				case 2:
				{
					StairGroupBox.Enabled = false;
					PitGroupBox.Enabled = false;
					DoorGroupBox.Enabled = false;
					ForceFieldGroupBox.Enabled = false;
					TeleporterGroupBox.Enabled = false;
					PlateGroupBox.Enabled = true;


					if (MazeBlock.FloorPlate == null)
					{
						MazeBlock.RemoveSpecials();
						MazeBlock.FloorPlate = new FloorPlate();
					}

				}
				break;

				// Force field
				case 3:
				{
					StairGroupBox.Enabled = false;
					PitGroupBox.Enabled = false;
					DoorGroupBox.Enabled = false;
					ForceFieldGroupBox.Enabled = true;
					TeleporterGroupBox.Enabled = false;
					PlateGroupBox.Enabled = false;

					if (MazeBlock.ForceField == null)
					{
						MazeBlock.RemoveSpecials();
						MazeBlock.ForceField = new ForceField();
					}
				}
				break;


				// Pit
				case 4:
				{
					StairGroupBox.Enabled = false;
					PitGroupBox.Enabled = true;
					DoorGroupBox.Enabled = false;
					ForceFieldGroupBox.Enabled = false;
					TeleporterGroupBox.Enabled = false;
					PlateGroupBox.Enabled = false;

					if (MazeBlock.Pit == null)
					{
						MazeBlock.RemoveSpecials();
						MazeBlock.Pit = new Pit();
					}
				}
				break;

				// Stair
				case 5:
				{
					StairGroupBox.Enabled = true;
					PitGroupBox.Enabled = false;
					DoorGroupBox.Enabled = false;
					ForceFieldGroupBox.Enabled = false;
					TeleporterGroupBox.Enabled = false;
					PlateGroupBox.Enabled = false;

					if (MazeBlock.Stair == null)
					{
						MazeBlock.RemoveSpecials();
						MazeBlock.Stair = new Stair();
					}
				}
				break;

				// Teleporter
				case 6:
				{
					StairGroupBox.Enabled = false;
					PitGroupBox.Enabled = false;
					DoorGroupBox.Enabled = false;
					ForceFieldGroupBox.Enabled = false;
					TeleporterGroupBox.Enabled = true;
					PlateGroupBox.Enabled = false;

					if (MazeBlock.Teleporter == null)
					{
						MazeBlock.RemoveSpecials();
						MazeBlock.Teleporter = new Teleporter();
					}
				}
				break;

			}
		}

		/// <summary>
		/// OnShow
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabPage1_Enter(object sender, EventArgs e)
		{
			if (MazeBlock.Door != null)
			{
				SpecialTypeBox.SelectedItem = "Door";
				HasButtonBox.Checked = MazeBlock.Door.HasButton;
				DoorStateBox.SelectedItem = MazeBlock.Door.State.ToString();
				DoorTypeBox.SelectedItem = MazeBlock.Door.Type.ToString();

			}
			else if (MazeBlock.FloorPlate != null)
			{
				SpecialTypeBox.SelectedItem = "Floor Plate";
				HiddenPlateBox.Checked = MazeBlock.FloorPlate.Invisible;
			}
			else if (MazeBlock.ForceField != null)
			{
				SpecialTypeBox.SelectedItem = "Force Field";

				ForceFieldTypeBox.SelectedItem = MazeBlock.ForceField.Type.ToString();
				ForceFieldRotationBox.SelectedItem = MazeBlock.ForceField.Rotation.ToString();
				ForceFieldMoveBox.SelectedItem = MazeBlock.ForceField.Move.ToString();
			}
			else if (MazeBlock.Pit != null)
			{
				SpecialTypeBox.SelectedItem = "Pit";

			}
			else if (MazeBlock.Stair != null)
			{
				SpecialTypeBox.SelectedItem = "Stair";

			}
			else if (MazeBlock.Teleporter != null)
			{
				SpecialTypeBox.SelectedItem = "Teleporter";

				
			}
			else
			{
				SpecialTypeBox.SelectedItem = "None";
			}
			
		}

		#endregion


		#region Doors

		/// <summary>
		/// Door has button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HasButtonBox_CheckedChanged(object sender, EventArgs e)
		{
			if (MazeBlock.Door == null)
				return;

			MazeBlock.Door.HasButton = HasButtonBox.Checked;
		}


		/// <summary>
		/// On Door state change
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DoorStateBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (MazeBlock.Door == null)
				return;

			MazeBlock.Door.State = (DoorState)Enum.Parse(typeof(DoorState), DoorStateBox.SelectedItem.ToString());
		}


		/// <summary>
		/// On Door type change
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DoorTypeBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (MazeBlock.Door == null)
				return;

			MazeBlock.Door.Type = (DoorType)Enum.Parse(typeof(DoorType), DoorTypeBox.SelectedItem.ToString());
		}


		#endregion


		#region Floor Plate

		/// <summary>
		/// Shows / hides floor plate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HiddenPlateBox_CheckedChanged(object sender, EventArgs e)
		{
			if (MazeBlock.FloorPlate == null)
				return;


			MazeBlock.FloorPlate.Invisible = HiddenPlateBox.Checked;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FloorPlateScriptBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (FloorPlateScriptBox.SelectedIndex == -1)
				return;

			MazeBlock.FloorPlate.ScriptName = FloorPlateScriptBox.SelectedItem as string;

			OnEnterFloorPlateBox.Items.Clear();
			OnLeaveFloorPlateBox.Items.Clear();

			Script script = ResourceManager.CreateAsset<Script>(FloorPlateScriptBox.SelectedItem as string);
			if (script == null)
				return;

			OnEnterFloorPlateBox.BeginUpdate();
			OnLeaveFloorPlateBox.BeginUpdate();

			List<string> methods = script.GetMethods();
			foreach (string name in methods)
			{
				OnEnterFloorPlateBox.Items.Add(name);
				OnLeaveFloorPlateBox.Items.Add(name);
			}

			OnEnterFloorPlateBox.EndUpdate();
			OnLeaveFloorPlateBox.EndUpdate();

		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEnterFloorPlateBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (OnEnterFloorPlateBox.SelectedIndex == -1)
				return;

			MazeBlock.FloorPlate.OnEnterScript = OnEnterFloorPlateBox.SelectedItem as string;

		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLeaveFloorPlateBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (OnLeaveFloorPlateBox.SelectedIndex == -1)
				return;

			MazeBlock.FloorPlate.OnLeaveScript = OnLeaveFloorPlateBox.SelectedItem as string;

		}



		#endregion


		#region Pit

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PitTargetBox_Click(object sender, EventArgs e)
		{
			if (MazeBlock.Pit == null)
				return;

			DungeonLocationForm form = new DungeonLocationForm(Maze.Dungeon, MazeBlock.Pit.Target);
			form.ShowDialog();
		}

		#endregion


		#region Force Field

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ForceFieldTypeBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (MazeBlock.ForceField == null)
				return;
			MazeBlock.ForceField.Type = (ForceFieldType)Enum.Parse(typeof(ForceFieldType), ForceFieldTypeBox.SelectedItem as string);

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ForceFieldRotationBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (MazeBlock.ForceField == null)
				return;

			MazeBlock.ForceField.Rotation = (CompassRotation)Enum.Parse(typeof(CompassRotation), ForceFieldRotationBox.SelectedItem as string);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ForceFieldMoveBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (MazeBlock.ForceField == null)
				return;

			MazeBlock.ForceField.Move = (CardinalPoint)Enum.Parse(typeof(CardinalPoint), ForceFieldMoveBox.SelectedItem as string);
		}


		#endregion


		#region Properties

		/// <summary>
		/// 
		/// </summary>
		Maze Maze;

		/// <summary>
		/// 
		/// </summary>
		MazeBlock MazeBlock;


		/// <summary>
		/// Item set
		/// </summary>
		ItemSet Itemset;


		#endregion


	}
}