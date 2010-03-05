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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using ArcEngine.Graphic;


namespace ArcEngine.Asset
{
	/// <summary>
	/// 
	/// </summary>
	class Skin: IAsset
	{






		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool Init()
		{


			return true;
		}

		#region IO


		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public bool Load(XmlNode xml)
		{


			return true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <returns></returns>
		public bool Save(XmlWriter writer)
		{


			return true;
		}

		#endregion




		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get;
			set;
		}


		/// <summary>
		/// 
		/// </summary>
		public string XmlTag
		{
			get { return "skin"; }
		}

		#endregion

	}


	/// <summary>
	/// Skin position
	/// </summary>
	enum SkinPosition
	{
		TopLeft = 0,
		Top,
		TopRight,

		Left,
		Background,
		Right,

		BottomLeft,
		Bottom,
		BottomRight
	}
}
