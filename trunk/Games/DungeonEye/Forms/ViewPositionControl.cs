﻿#region Licence
//
//This file is part of ArcEngine.
//Copyright (C)2008-2011 Adrien Hémery ( iliak@mimicprod.net )
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DungeonEye;


namespace DungeonEye.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public partial class ViewPositionControl : UserControl
	{
		/// <summary>
		/// 
		/// </summary>
		public ViewPositionControl()
		{
			InitializeComponent();

			TeamBox.Checked = true;
		}


		/// <summary>
		/// Uncheck all checkboxes
		/// </summary>
		/// <param name="exclude">Control to exclude</param>
		void UncheckButtons(CheckBox exclude)
		{
			CheckBox[] boxes = new CheckBox[]
			{
				ABox,
				BBox,
				CBox,
				DBox,
				EBox,
				FBox,
				GBox,
				HBox,
				IBox,
				JBox,
				KBox,
				LBox,
				MBox,
				NBox,
				OBox,
				PBox,
				TeamBox,
				QBox,
			};

			foreach (CheckBox box in boxes)
			{
				if (box != exclude)
					box.Checked = false;
			}
		}


		/// <summary>
		/// Check for at least one position is checked
		/// </summary>
		void CheckForValidity()
		{
			CheckBox[] boxes = new CheckBox[]
			{
				ABox,
				BBox,
				CBox,
				DBox,
				EBox,
				FBox,
				GBox,
				HBox,
				IBox,
				JBox,
				KBox,
				LBox,
				MBox,
				NBox,
				OBox,
				PBox,
				TeamBox,
				QBox,
			};


			foreach (CheckBox box in boxes)
			{
				if (box.Checked)
					return;
			}

			TeamBox.Checked = true;
		}


		#region Form events


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Box_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox box = sender as CheckBox;

			// Uncheck others
			if (box.Checked)
				UncheckButtons(box);

			if (box.Text == "^")
				position = ViewFieldPosition.Team;
			else
				position = (ViewFieldPosition)Enum.Parse(typeof(ViewFieldPosition), box.Text);


			CheckForValidity();

			OnPositionChanged(EventArgs.Empty);
		}

		#endregion


		#region Events

		/// <summary>
		/// Selected view position changer
		/// </summary>
		/// <param name="sender">Controler handle</param>
		/// <param name="position">Selected position</param>
		public delegate void ChangedEventHandler(object sender, ViewFieldPosition position);


		/// <summary>
		/// 
		/// </summary>
		public event ChangedEventHandler PositionChanged;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnPositionChanged(EventArgs e)
		{
			if (PositionChanged != null)
				PositionChanged(this, position);
		}

		#endregion


		#region Properties


		/// <summary>
		/// Select view field position
		/// </summary>
		public ViewFieldPosition Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}
		ViewFieldPosition position;

		#endregion

	}
}
