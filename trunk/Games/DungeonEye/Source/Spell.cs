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
using System.Xml;
using ArcEngine.Asset;
using DungeonEye.Interfaces;


// http://dmreference.com/SRD/Magic.htm
// http://www.d20srd.org/srd/magicOverview/spellDescriptions.htm
//
//
namespace DungeonEye
{
	/// <summary>
	/// Spell class
	/// </summary>
	public class Spell : IAsset
	{

		/// <summary>
		/// Default constructor
		/// </summary>
		public Spell()
		{
			IsDisposed = false;
			Script = new ScriptInterface<ISpell>();
		}


		/// <summary>
		/// Initializes the item
		/// </summary>
		/// <returns></returns>
		public bool Init()
		{
			return true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0}, {1} level {2}", Name, Class, Level);
		}


		#region Load & Save

		/// <summary>
		/// Loads a spell definition
		/// </summary>
		/// <param name="xml">XmlNode</param>
		/// <returns></returns>
		public bool Load(XmlNode xml)
		{
			if (xml == null || xml.Name != "spell")
				return false;

			Name = xml.Attributes["name"].Value;

			foreach (XmlNode node in xml)
			{
				if (node.NodeType == XmlNodeType.Comment)
					continue;


				switch (node.Name.ToLower())
				{
					case "range":
					{
						Range =(SpellRange) Enum.Parse(typeof(SpellRange), node.Attributes["value"].Value, true);
					}
					break;

					case "level":
					{
						Level = int.Parse(node.Attributes["value"].Value);
					}
					break;

					case "duration":
					{
						Duration = TimeSpan.Parse(node.Attributes["value"].Value);
					}
					break;

					case "castingtime":
					{
						CastingTime = TimeSpan.Parse(node.Attributes["value"].Value);
					}
					break;

					case "description":
					{
						Description = node.InnerText;
					}
					break;

					case "script":
					{
						Script.Load(node);
					}
					break;

				}
			}



			return true;
		}



		/// <summary>
		/// Saves a spell definition
		/// </summary>
		/// <param name="writer">XmlWriter</param>
		/// <returns></returns>
		public bool Save(XmlWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.WriteStartElement("spell");
			writer.WriteAttributeString("name", Name);

			writer.WriteElementString("description", Description);



			writer.WriteStartElement("duration");
			writer.WriteAttributeString("value", Duration.ToString());
			writer.WriteEndElement();



			writer.WriteStartElement("castingtime");
			writer.WriteAttributeString("value", CastingTime.ToString());
			writer.WriteEndElement();


			writer.WriteStartElement("range");
			writer.WriteAttributeString("value", Range.ToString());
			writer.WriteEndElement();



			writer.WriteStartElement("level");
			writer.WriteAttributeString("value", Level.ToString());
			writer.WriteEndElement();


			Script.Save("script", writer);


			writer.WriteEndElement();

			return true;
		}

		#endregion



		#region Properties

		/// <summary>
		/// Name of the spell
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Is asset disposed
		/// </summary>
		public bool IsDisposed { get; private set; }


		/// <summary>
		/// Script
		/// </summary>
		public ScriptInterface<ISpell> Script
		{
			get;
			private set;
		}


		/// <summary>
		/// Xml tag of the asset in bank
		/// </summary>
		public string XmlTag
		{
			get
			{
				return "spell";
			}
		}


		/// <summary>
		/// Description of the spell
		/// </summary>
		public string Description
		{
			get;
			set;
		}



		/// <summary>
		/// How long the magical energy of the spell lasts
		/// </summary>
		public TimeSpan Duration
		{
			get;
			set;
		}


		/// <summary>
		/// Time to cast the spell
		/// </summary>
		public TimeSpan CastingTime
		{
			get;
			set;
		}


		/// <summary>
		/// Action range
		/// </summary>
		public SpellRange Range
		{
			get;
			set;
		}


		/// <summary>
		/// Required level for this spell
		/// </summary>
		public int Level
		{
			get;
			set;
		}


		/// <summary>
		/// Class
		/// </summary>
		public HeroClass Class
		{
			get;
			set;
		}
		
		#endregion
	}


	/// <summary>
	/// Indicates how far from the entity a spell can reach
	/// </summary>
	public enum SpellRange
	{
		/// <summary>
		/// The spell affects only the caster.
		/// </summary>
		Personal,

		/// <summary>
		/// Affect the whole team
		/// </summary>
		Team,

		/// <summary>
		/// You must touch a creature or object to affect it. 
		/// </summary>
		Touch,


		/// <summary>
		/// The spell reaches as far as 1 block
		/// </summary>
		Close,


		/// <summary>
		/// The spell reaches as far as 2 blocks
		/// </summary>
		Medium,


		/// <summary>
		/// The spell reaches as far as 3 blocks
		/// </summary>
		Long,


		/// <summary>
		/// The spell reaches far as possible
		/// </summary>
		Unlimited,

	}
}
