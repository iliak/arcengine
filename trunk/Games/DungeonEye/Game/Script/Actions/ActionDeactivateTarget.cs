﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace DungeonEye.Script.Actions
{
	/// <summary>
	/// Deactivate a target
	/// </summary>
	public class ActionDeactivateTarget : ActionBase
	{

		/// <summary>
		/// 
		/// </summary>
		public ActionDeactivateTarget()
		{
			Name = "DeactivateTarget";
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override bool Run()
		{
			if (Target == null)
				return false;

			Square square = Target.GetSquare(GameScreen.Dungeon);
			if (square == null)
				return false;

			if (square.Actor != null)
				square.Actor.Deactivate();

			return true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override bool Load(XmlNode xml)
		{
			if (xml == null)
				return false;

			foreach (XmlNode node in xml)
			{
				switch (node.Name.ToLower())
				{

					default:
					{
						base.Load(node);
					}
					break;
				}
			}

			return true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <returns></returns>
		public override bool Save(XmlWriter writer)
		{
			if (writer == null)
				return false;


			writer.WriteStartElement(Name);

			base.Save(writer);

			writer.WriteEndElement();

			return true;
		}




		#region Properties



		#endregion
	}
}
