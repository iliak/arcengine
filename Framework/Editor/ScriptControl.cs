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
using System.Text;
using System.Windows.Forms;
using System.Xml;
using ArcEngine;
using ArcEngine.Asset;
using ArcEngine.Forms;
using ArcEngine.Graphic;

namespace ArcEngine.Editor
{
	/// <summary>
	/// 
	/// </summary>
	public partial class ScriptControl : UserControl
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ScriptControl()
		{
			InitializeComponent();
		}


		/// <summary>
		/// Set values
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="s"></param>
		/// <returns></returns>
		public bool SetValues<T>(ScriptInterface<T> s)
		{

			RebuildFoundScripts();


			if (s == null || string.IsNullOrEmpty(s.ScriptName) || !ScriptNameBox.Items.Contains(s.ScriptName))
				return false;

			ScriptNameBox.SelectedItem = s.ScriptName;


			if (string.IsNullOrEmpty(s.InterfaceName) || !InterfaceNameBox.Items.Contains(s.InterfaceName))
				return false;

			InterfaceNameBox.SelectedItem = s.InterfaceName;

			return true;
		}


		#region OnEvent


		/// <summary>
		/// Raises the ControlAdded event. 
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data</param>
		protected virtual void OnInterfaceChanged(EventArgs e)
		{
			if (InterfaceChanged != null)
				InterfaceChanged(this, e);
		}


		/// <summary>
		/// Raises the ControlRemoved event. 
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data</param>
		protected virtual void OnScriptChanged(EventArgs e)
		{
			if (ScriptChanged != null)
				ScriptChanged(this, e);
		}

		#endregion


		#region Events

		/// <summary>
		/// Occurs when the interface name changed.
		/// </summary>
		public event EventHandler InterfaceChanged;



		/// <summary>
		/// Occurs when the script name changed.
		/// </summary>
		public event EventHandler ScriptChanged;


		#endregion


		#region scripting events


		/// <summary>
		/// Rebuild found interfaces
		/// </summary>
		/// <param name="name"></param>
		void RebuildFoundInterfaces(string name)
		{
			InterfaceNameBox.BeginUpdate();
			InterfaceNameBox.Items.Clear();


			Script script = ResourceManager.CreateAsset<Script>(name);
			if (script != null)
			{
				List<string> list = script.GetImplementedInterfaces(null);

				InterfaceNameBox.Items.AddRange(list.ToArray());

			}
	
			InterfaceNameBox.Items.Insert(0, "");
			InterfaceNameBox.EndUpdate();
		}

		/// <summary>
		/// 
		/// </summary>
		void RebuildFoundScripts()
		{
			ScriptNameBox.BeginUpdate();
			ScriptNameBox.Items.Clear();

			ScriptNameBox.Items.Add("");
			foreach (string name in ResourceManager.GetAssets<Script>())
			{
				ScriptNameBox.Items.Add(name);
			}

			ScriptNameBox.EndUpdate();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScriptNameBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			RebuildFoundInterfaces((string) ScriptNameBox.SelectedItem);

			OnScriptChanged(EventArgs.Empty);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void InterfaceNameBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnInterfaceChanged(EventArgs.Empty);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RefreshBox_Click(object sender, EventArgs e)
		{
			RebuildFoundScripts();
		}


		#endregion


		#region Properties

		/// <summary>
		/// Gets or sets the control text
		/// </summary>
		public string ControlText
		{
			get
			{
				return groupBox2.Text;
			}

			set
			{
				groupBox2.Text = value;
			}
		}


		/// <summary>
		/// Gets or sets the control text
		/// </summary>
		public string InterfaceName
		{
			get
			{
				return InterfaceNameBox.Text;
			}
		}


		/// <summary>
		/// Gets or sets the control text
		/// </summary>
		public string ScriptName
		{
			get
			{
				return ScriptNameBox.Text;
			}
		}


		#endregion
	}

}
