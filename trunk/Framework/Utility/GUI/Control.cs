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
using System.Text;
using System.Xml;
using ArcEngine.Graphic;
using ArcEngine.Asset;
using ArcEngine.Input;


namespace ArcEngine.Utility.GUI
{

	/// <summary>
	/// Defines the base class for controls, which are components with visual representation
	/// </summary>
	public abstract class Control
	{


		/// <summary>
		/// Initializes a new instance of the Control class with default settings.
		/// </summary>
		public Control()
		{
			Rectangle = new Rectangle();
			Color = Color.White;
			Visible = true;
		}


		/// <summary>
		/// Releases the unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
			if (Font != null)
				Font.Dispose();
			Font = null;
		}


		#region Update & Draw

		/// <summary>
		/// Updates the gadget
		/// </summary>
		/// <param name="manager">Gui manager handle</param>
		/// <param name="time">Elapsed time</param>
		public virtual void Update(GuiManager manager, GameTime time)
		{

			// Mouse inside the control
			if (ScreenRectangle.Contains(Mouse.Location))
			{
				if ((Mouse.Buttons | System.Windows.Forms.MouseButtons.None) != System.Windows.Forms.MouseButtons.None)
				{
					OnClick(new System.Windows.Forms.MouseEventArgs(Mouse.Buttons, 1, Mouse.Location.X, Mouse.Location.Y, 0));
				}



			}
		}


		/// <summary>
		/// Draws the button
		/// </summary>
		/// <param name="manager">Gui manager handle</param>
		/// <param name="batch">SpriteBatch to use</param>
		public virtual void Draw(GuiManager manager, SpriteBatch batch)
		{
			OnPaint(EventArgs.Empty);
		}



		#endregion



		#region Methods


		/// <summary>
		/// Conceals the control from the user. 
		/// </summary>
		public void Hide()
		{
			Visible = false;
		}


		/// <summary>
		/// Displays the control to the user. 
		/// </summary>
		public void Show()
		{
			Visible = true;
		}


		/// <summary>
		/// Computes the location of the specified client point into screen coordinates.
		/// </summary>
		/// <param name="p">The client coordinate point to convert.</param>
		/// <returns>A Point that represents the converted point in screen coordinates. </returns>
		public Point PointToScreen(Point p)
		{


			return p;
		}


		/// <summary>
		/// Computes the location of the specified screen point into client coordinates.
		/// </summary>
		/// <param name="p">The screen coordinate Point to convert.</param>
		/// <returns>A Point that represents the converted Point, p, in client coordinates.</returns>
		public Point PointToClient(Point p)
		{

			return p;
		}



		/// <summary>
		/// Computes the size and location of the specified client rectangle in screen coordinates.
		/// </summary>
		/// <param name="r">The screen coordinate Rectangle to convert.</param>
		/// <returns>A Rectangle that represents the converted Rectangle, p, in screen coordinates.</returns>
		public Rectangle RectangleToScreen(Rectangle r)
		{


			return r;
		}


		/// <summary>
		/// Computes the size and location of the specified screen rectangle in client coordinates.
		/// </summary>
		/// <param name="r">The screen coordinate Rectangle to convert.</param>
		/// <returns>A Rectangle that represents the converted Rectangle, r, in client coordinates.</returns>
		public Rectangle RectangleToClient(Rectangle r)
		{
			return r;

		}

		#endregion


		#region

		/// <summary>
		/// Raises the Click event. 
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data</param>
		protected virtual void OnClick(EventArgs e)
		{
			if (Click != null)
				Click(this, e);
		}


		/// <summary>
		/// Raises the Paint event. 
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data</param>
		protected virtual void OnPaint(EventArgs e)
		{
			if (Paint != null)
				Paint(this, e);
		}



		/// <summary>
		/// Raises the MouseDown event. 
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data</param>
		protected virtual void OnMouseDown(EventArgs e)
		{
			if (MouseDown != null)
				MouseDown(this, e);
		}


		/// <summary>
		/// Raises the MouseEnter event. 
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data</param>
		protected virtual void OnMouseEnter(EventArgs e)
		{
			if (MouseEnter != null)
				MouseEnter(this, e);
		}



		/// <summary>
		/// Raises the MouseUp event. 
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data</param>
		protected virtual void OnMouseUp(EventArgs e)
		{
			if (MouseUp != null)
				MouseUp(this, e);
		}


		/// <summary>
		/// Raises the MouseMove event. 
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data</param>
		protected virtual void OnMouseMove(EventArgs e)
		{
			if (MouseMove != null)
				MouseMove(this, e);
		}




		#endregion


		#region Events

		/// <summary>
		/// Occurs when the Text property value changes. 
		/// </summary>
		public event EventHandler TextChanged;


		/// <summary>
		/// Occurs when the Visible property value changes. 
		/// </summary>
		public event EventHandler VisibleChanged;


		/// <summary>
		/// Occurs when the control is redrawn.  
		/// </summary>
		public event EventHandler Paint;


		/// <summary>
		/// Occurs when the control is clicked.
		/// </summary>
		public event EventHandler Click;


		/// <summary>
		/// Occurs when the mouse pointer is over the control and a mouse button is pressed.
		/// </summary>
		public event EventHandler MouseDown;


		/// <summary>
		/// Occurs when the mouse pointer is over the control and a mouse button is released.
		/// </summary>
		public event EventHandler MouseUp;


		/// <summary>
		/// 
		/// </summary>
		public event EventHandler MouseMove;


		/// <summary>
		/// 
		/// </summary>
		public event EventHandler MouseEnter;


		/// <summary>
		/// 
		/// </summary>
		public event EventHandler MouseLeave;

		#endregion



		#region IO routines
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual bool Save(XmlWriter xml)
		{
			if (xml == null)
				return false;


			xml.WriteStartElement("rectangle");
			xml.WriteAttributeString("x", Rectangle.X.ToString());
			xml.WriteAttributeString("y", Rectangle.Y.ToString());
			xml.WriteAttributeString("width", Rectangle.Width.ToString());
			xml.WriteAttributeString("height", Rectangle.Height.ToString());
			xml.WriteEndElement();


			xml.WriteStartElement("color");
			xml.WriteAttributeString("r", Color.R.ToString());
			xml.WriteAttributeString("g", Color.G.ToString());
			xml.WriteAttributeString("b", Color.B.ToString());
			xml.WriteAttributeString("a", Color.A.ToString());
			xml.WriteEndElement();

	

			xml.WriteStartElement("bgcolor");
			xml.WriteAttributeString("r", BgColor.R.ToString());
			xml.WriteAttributeString("g", BgColor.G.ToString());
			xml.WriteAttributeString("b", BgColor.B.ToString());
			xml.WriteAttributeString("a", BgColor.A.ToString());
			xml.WriteEndElement();

	
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual bool Load(XmlNode node)
		{
			if (node == null)
			return false;

			switch (node.Name.ToLower())
			{

				// Main color
				case "color":
				{
					Color = Color.FromArgb(Int32.Parse(node.Attributes["a"].Value),
											Int32.Parse(node.Attributes["r"].Value),
											Int32.Parse(node.Attributes["g"].Value),
											Int32.Parse(node.Attributes["b"].Value));														
				}
				break;

				// Background color
				case "bgcolor":
				{
					BgColor = Color.FromArgb(Int32.Parse(node.Attributes["a"].Value),
											Int32.Parse(node.Attributes["r"].Value),
											Int32.Parse(node.Attributes["g"].Value),
											Int32.Parse(node.Attributes["b"].Value));														
				}
				break;

	
				// Rectangle of the element
				case "rectangle":
				{

					Rectangle rect = Rectangle.Empty;
					rect.X = Int32.Parse(node.Attributes["x"].Value);
					rect.Y = Int32.Parse(node.Attributes["y"].Value);
					rect.Width = Int32.Parse(node.Attributes["width"].Value);
					rect.Height = Int32.Parse(node.Attributes["height"].Value);

					Rectangle = rect;
				}
				break;


				// Unknown...
				default:
				{
					return false;
				}
			}

			return true;
		}

		#endregion



		#region Properties

		/// <summary>
		/// Gets or sets the parent container of the control. 
		/// </summary>
		public Control Parent
		{
			get;
			internal set;
		}


		/// <summary>
		/// Name of the control
		/// </summary>
		public string Name;


		/// <summary>
		/// Rectangle
		/// </summary>
		public Rectangle Rectangle;


		/// <summary>
		/// Rectangle of the control in screen coordinate
		/// </summary>
		public Rectangle ScreenRectangle
		{
			get
			{
				Rectangle rect = Rectangle;

				if (Parent != null)
					rect.Offset(Parent.ScreenRectangle.Location);

				return rect;
			}
		}


		/// <summary>
		/// Size of the element
		/// </summary>
		[Browsable(false)]
		public Size Size
		{
			get
			{
				return Rectangle.Size;
			}
			set
			{
				Rectangle.Size = value;
			}
		}


		/// <summary>
		/// Location of the element
		/// </summary>
		[Browsable(false)]
		public Point Location
		{
			get
			{
				return Rectangle.Location;
			}
			set
			{
				Rectangle.Location = value;
			}
		}


		/// <summary>
		/// Gets/Sets the color of the element
		/// </summary>
		[Category("Color")]
		public Color Color
		{
			get;
			set;
		}
 
         
		/// <summary>
		/// Gets/Sets the background color of the element
		/// </summary>
		[Category("Color")]
		public Color BgColor
		{
			get;
			set;
		}


		/// <summary>
		/// Gets or sets a value indicating whether the control and all its parent controls are displayed.
		/// </summary>
		public bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;

				if (VisibleChanged != null)
					VisibleChanged(this, EventArgs.Empty);

			}
		}
		private bool visible;


		/// <summary>
		/// Font to use
		/// </summary>
		public BitmapFont Font;


		/// <summary>
		/// Gets or sets the text associated with this control.
		/// </summary>
		public string Text
		{
			get
			{
				if (text != null)
				{
					return text;
				}
				return "";
			}
			set
			{
				if (value == null)
					value = "";

				if (text == value)
					return;

				text = value;

				if (TextChanged != null)
					TextChanged(this, EventArgs.Empty);
			}
		}
		private string text;

		#endregion
	}

}
