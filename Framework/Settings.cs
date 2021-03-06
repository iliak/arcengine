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
using System.IO;
using System.Text;
using System.Xml;



namespace ArcEngine
{


	/// <summary>
	/// Settings class holding all settings of the game
	/// </summary>
	static public class Settings
	{

		/// <summary>
		/// Constructor
		/// </summary>
		static Settings()
		{
			Tokens = new Dictionary<string, string>();
			FileName = "settings.xml";
		}


		/// <summary>
		/// Remove all settings
		/// </summary>
		public static void Clear()
		{
			Tokens.Clear();
		}




		#region Tokens

		/// <summary>
		/// Sets a string token
		/// </summary>
		/// <param name="name">Token</param>
		/// <param name="value">Value</param>
		static public void SetToken(string name, object value)
		{
			Tokens[name] = value.ToString();
		}


		/// <summary>
		/// Gets a string token
		/// </summary>
		/// <param name="name">Token's name</param>
		/// <param name="defaultvalue">Default value</param>
		/// <returns>String value or defaultvalue if not found</returns>
		static public string GetString(string name, string defaultvalue)
		{
			if (Tokens.ContainsKey(name))
				return Tokens[name];

			return defaultvalue;
		}


		/// <summary>
		/// Gets a string token
		/// </summary>
		/// <param name="name">Token's name</param>
		/// <returns>String value or string.Empty if not found</returns>
		static public string GetString(string name)
		{
			return GetString(name, string.Empty);
		}


		/// <summary>
		/// Gets an int token
		/// </summary>
		/// <param name="name">Token's name</param>
		/// <param name="defaultvalue">Default value</param>
		/// <returns>Int value, or defaultvalue if not found</returns>
		static public int GetInt(string name, int defaultvalue)
		{
			if (Tokens.ContainsKey(name))
				return int.Parse(Tokens[name]);

			return defaultvalue;
		}


		/// <summary>
		/// Gets an int token
		/// </summary>
		/// <param name="name">Token's name</param>
		/// <returns>Int value, or 0 if not found</returns>
		static public int GetInt(string name)
		{
			return GetInt(name, 0);
		}


		/// <summary>
		/// Gets a float token
		/// </summary>
		/// <param name="name">Token's name</param>
		/// <returns>Float value, or 0.0f if not found</returns>
		static public float GetFloat(string name)
		{
			return GetFloat(name, 0.0f);
		}


		/// <summary>
		/// Gets a float token
		/// </summary>
		/// <param name="name">Token's name</param>
		/// <param name="defaultvalue">Default value if not found</param>
		/// <returns>Float value, or 0.0f if not found</returns>
		static public float GetFloat(string name, float defaultvalue)
		{
			if (Tokens.ContainsKey(name))
				return float.Parse(Tokens[name]);

			return defaultvalue;
		}


		/// <summary>
		/// Gets a boolean token
		/// </summary>
		/// <param name="name">Token's name</param>
		/// <returns>Boolean value, or false if not found</returns>
		static public bool GetBool(string name)
		{
			return GetBool(name, false);
		}


		/// <summary>
		/// Gets a boolean token
		/// </summary>
		/// <param name="name">Token's name</param>
		/// <param name="defaultvalue">Default value if not found</param>
		/// <returns>Boolean value, or defaultvalue if not found</returns>
		static public bool GetBool(string name, bool defaultvalue)
		{
			if (Tokens.ContainsKey(name))
				return bool.Parse(Tokens[name]);

			return defaultvalue;
		}


		#endregion



		#region IO


		/// <summary>
		/// Saves settings
		/// </summary>
		/// <returns>True if successful</returns>
		static public bool Save()
		{
			return Save(FileName);
		}


		/// <summary>
		/// Saves settings
		/// </summary>
		/// <param name="filename">Filename</param>
		/// <returns></returns>
		static public bool Save(string filename)
		{
			if (string.IsNullOrEmpty(filename))
				return false;

			FileName = filename;

			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.OmitXmlDeclaration = false;
			settings.IndentChars = "\t";
			settings.Encoding = ASCIIEncoding.ASCII;

			XmlWriter writer = XmlWriter.Create(filename, settings);
			writer.WriteStartDocument(true);
			writer.WriteStartElement("settings");
			foreach (KeyValuePair<string, string> kvp in Tokens)
			{
				writer.WriteElementString(kvp.Key, kvp.Value);
			}
			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Flush();
			writer.Close();


			return true;
		}


		/// <summary>
		/// Loads settings from a file
		/// </summary>
		/// <returns>True on success</returns>
		static public bool Load()
		{
			return Load(FileName);
		}


		/// <summary>
		/// Loads settings from a file
		/// </summary>
		/// <param name="filename">Filename to load</param>
		/// <returns></returns>
		static public bool Load(string filename)
		{
			if (string.IsNullOrEmpty(filename))
				return false;

			FileName = filename;

			Trace.Write("Loading settings from file \"" + filename + "\"...");

			// File exists ??
			if (File.Exists(filename) == false)
			{
				Trace.WriteLine("File not found !!!");
				return false;
			}

			
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);

			// Check the root node
			XmlElement xml = doc.DocumentElement;
			if (xml.Name.ToLower() != "settings")
			{
				Trace.WriteLine("\"" + filename + "\" is not a valid settings file !");
				return false;
			}



			// For each nodes, process it
			XmlNodeList nodes = xml.ChildNodes;
			foreach (XmlNode node in nodes)
			{
				if (node.NodeType == XmlNodeType.Comment)
					continue;

				Tokens[node.Name] = node.InnerText;
			}


			Trace.WriteLine("OK");
			return true;
		}

		#endregion



		#region Properties

		/// <summary>
		/// List of tokens
		/// </summary>
		static Dictionary<string, string> Tokens;


		/// <summary>
		/// Filename
		/// </summary>
		static public string FileName
		{
			get;
			set;
		}

		#endregion

	}
}


