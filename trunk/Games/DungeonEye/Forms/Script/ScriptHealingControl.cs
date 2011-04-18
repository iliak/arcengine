﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DungeonEye.Script;

namespace DungeonEye.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public partial class ScriptHealingControl : ScriptActionControlBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="script"></param>
		public ScriptHealingControl(ScriptHealing script)
		{
			InitializeComponent();


			if (script != null)
				Action = script;
			else
				Action = new ScriptHealing();
		}


		#region Properties


		#endregion



	}
}