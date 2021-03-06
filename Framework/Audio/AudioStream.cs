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
using System.IO;
using System.Xml;
using ArcEngine.Asset;
using OpenAL = OpenTK.Audio.OpenAL;
using ArcEngine.Interface;


// http://www.marek-knows.com/
// http://www.xiph.org/vorbis/doc/vorbisfile/example.html
// http://loulou.developpez.com/tutoriels/openal/flux-ogg/
	


namespace ArcEngine.Audio
{
	/// <summary>
	///Audio streaming class
	/// </summary>
	public class AudioStream : IDisposable, IAsset
	{

		/// <summary>
		/// Constructor
		/// </summary>
		public AudioStream()
		{
			if (!AudioManager.IsInit)
			{
				Trace.WriteLine("[AudioStream] AudioStream() : No audio context available.");
				return;
			}

			// Add to the known list of audio streams
			if (Streams == null)
				Streams = new List<AudioStream>();
			Streams.Add(this);

			Buffers = OpenAL.AL.GenBuffers(2);
			AudioManager.Check();

			Source = OpenAL.AL.GenSource();
			AudioManager.Check();

			// Stream buffers
			BufferData = new Dictionary<int, byte[]>();
			BufferData[Buffers[0]] = new byte[44100];
			BufferData[Buffers[1]] = new byte[44100];

		}


			/// <summary>
		/// Destructor
		/// </summary>
		~AudioStream()
		{
			if (!IsDisposed)
				System.Windows.Forms.MessageBox.Show("[AudioStream] : Call Dispose() !!");
				//throw new Exception ("[AudioStream] : Call Dispose() !!");
		}

	
		/// <summary>
		/// Dispose resources
		/// </summary>
		public void Dispose()
		{
			// Remove from known streams
			if (Streams != null)
				Streams.Remove(this);

			if (oggStream != null)
				oggStream.Dispose();
			oggStream = null;
			Stream = null;

			if (Buffers != null)
				OpenAL.AL.DeleteBuffers(Buffers);
			Buffers = null;

			if (Source != 0)
				OpenAL.AL.DeleteSource(Source);
			Source = 0;

			FileName = string.Empty;
			BufferData = null;

			IsDisposed = true;
		}



		/// <summary>
		/// Loads an Ogg Vorbis file
		/// </summary>
		/// <param name="filename">File to open</param>
		/// <returns></returns>
		public bool LoadOgg(string filename)
		{
			FileName = filename;
			return LoadOgg(ResourceManager.Load(filename));
		}


		/// <summary>
		/// Loads an Ogg Vorbis from a stream
		/// </summary>
		/// <param name="stream">Stream handle</param>
		/// <returns>True on success</returns>
		public bool LoadOgg(Stream stream)
		{
			if (stream == null || !AudioManager.IsInit)
				return false;

			Stream = stream;
			oggStream = new OggInputStream(Stream);

			Position = Vector3.Zero;
			Direction = Vector3.Zero;
			Velocity = Vector3.Zero;
			RolloffFactor = 0.0f;
			SourceRelative = true;

			return true;
		}

	
		/// <summary>
		/// Play the sound
		/// </summary>
		public void Play()
		{
			if (!AudioManager.IsInit || State == AudioSourceState.Playing || Stream == null)
				return;


			// Fill first buffer
			if (!StreamBuffer(Buffers[0], BufferData[Buffers[0]]))
				return;

			// Fill second buffer
			if (!StreamBuffer(Buffers[1], BufferData[Buffers[1]]))
				return;

			// Add buffers to the queue
			OpenAL.AL.SourceQueueBuffers(Source, Buffers.Length, Buffers);
			AudioManager.Check();

			// Play source
			OpenAL.AL.SourcePlay(Source);
		}


		/// <summary>
		/// Stop the sound
		/// </summary>
		public void Stop()
		{
			if (!AudioManager.IsInit)
				return;

			// Stop source
			OpenAL.AL.SourceStop(Source);

			Rewind();
		}


		/// <summary>
		/// Pause sound
		/// </summary>
		public void Pause()
		{
			if (!AudioManager.IsInit)
				return;
			
			OpenAL.AL.SourcePause(Source);
		}


		/// <summary>
		/// Update a sound buffer
		/// </summary>
		internal void Process()
		{
			if (State != AudioSourceState.Playing || !AudioManager.IsInit)
				return;

			// Get empty buffers
			int processed = 0;
			OpenAL.AL.GetSource(Source, OpenAL.ALGetSourcei.BuffersProcessed, out processed);

			// Update each empty buffer
			while (processed-- != 0)
			{
				// Enqueue buffer
				int buffer = OpenAL.AL.SourceUnqueueBuffer(Source);
				AudioManager.Check();

				// Update buffer
				StreamBuffer(buffer, BufferData[buffer]);

				// Queue buffer
				OpenAL.AL.SourceQueueBuffer(Source, buffer);
				AudioManager.Check();
			}

		}


		/// <summary>
		/// Fill a buffer with audio data
		/// </summary>
		/// <param name="bufferid">Buffer id</param>
		/// <param name="data">Buffer to fill</param>
		/// <returns>True if no errors</returns>
		bool StreamBuffer(int bufferid, byte[] data)
		{
			if (data == null | data.Length == 0)
				return false;

			int size = 0;
			while (size < data.Length)
			{
				int result = oggStream.Read(data, size, data.Length - size);
				if (result > 0)
				{
					size += result;
				}
				else
				{
					Trace.WriteLine("[AudioStream] : End of stream");
	
					// End of stream
					//if (Stream.Position == Stream.Length)
					if (oggStream.Available == 0)
					{
						if (Loop)
						{
							Rewind();
						}
						else
							Stop();
					}

					return false;
				}
			}

			if (size == 0)
			{
				return false;
			}

			// Fill OpenAL buffer
			OpenAL.AL.BufferData(bufferid, (OpenAL.ALFormat)oggStream.Format, data, data.Length, oggStream.Rate);
			AudioManager.Check();

			return true;
		}


		/// <summary>
		/// Rewind the stream
		/// </summary>
		public void Rewind()
		{
			if (Stream == null)
				return;
			
			// Begin of the stream
			Stream.Seek(0, SeekOrigin.Begin);
			
			// Reload stream
			LoadOgg(Stream);
		}


		#region statics

		/// <summary>
		/// Update 
		/// </summary>
		static internal void Update()
		{
			if (Streams == null)
				return;

			foreach (AudioStream audio in Streams)
			{
				audio.Process();
			}
		}


		/// <summary>
		/// Registred streams
		/// </summary>
		static List<AudioStream> Streams;

		#endregion



		#region	IO operations


		///<summary>
		/// Save the collection to a xml node
		/// </summary>
		public bool Save(XmlWriter xml)
		{
			if (xml == null)
				return false;

			/*
						xml.WriteStartElement("tileset");
						xml.WriteAttributeString("name", Name);



						if (!string.IsNullOrEmpty(TextureName))
						{
							xml.WriteStartElement("texture");
							xml.WriteAttributeString("file", TextureName);
							xml.WriteEndElement();
						}

						// Loops throughs tile
						foreach (KeyValuePair<int, Tile> val in tiles)
						{
							xml.WriteStartElement("tile");

							xml.WriteAttributeString("id", val.Key.ToString());

							xml.WriteStartElement("rectangle");
							xml.WriteAttributeString("x", val.Value.Rectangle.X.ToString());
							xml.WriteAttributeString("y", val.Value.Rectangle.Y.ToString());
							xml.WriteAttributeString("width", val.Value.Rectangle.Width.ToString());
							xml.WriteAttributeString("height", val.Value.Rectangle.Height.ToString());
							xml.WriteEndElement();

							xml.WriteStartElement("hotspot");
							xml.WriteAttributeString("x", val.Value.HotSpot.X.ToString());
							xml.WriteAttributeString("y", val.Value.HotSpot.Y.ToString());
							xml.WriteEndElement();

							xml.WriteStartElement("collisionbox");
							xml.WriteAttributeString("x", val.Value.CollisionBox.X.ToString());
							xml.WriteAttributeString("y", val.Value.CollisionBox.Y.ToString());
							xml.WriteAttributeString("width", val.Value.CollisionBox.Width.ToString());
							xml.WriteAttributeString("height", val.Value.CollisionBox.Height.ToString());
							xml.WriteEndElement();

							xml.WriteEndElement();
						}


						xml.WriteEndElement();
			*/
			return false;
		}


		/// <summary>
		/// Loads the collection from a xml node
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public bool Load(XmlNode xml)
		{
			if (xml == null)
				return false;

			Name = xml.Attributes["name"].Value;


			// Process datas
			XmlNodeList nodes = xml.ChildNodes;
			foreach (XmlNode node in nodes)
			{
				if (node.NodeType == XmlNodeType.Comment)
				{
					//base.LoadComment(node);
					continue;
				}


				switch (node.Name.ToLower())
				{
					// file name
					case "file":
					{
						//LoadSound(node.Attributes["name"].Value);
					}
					break;

				}
			}

			return true;
		}

		#endregion
	


		#region Properties


		/// <summary>
		/// Sound position
		/// </summary>
		public Vector3 Position
		{
			get
			{
				if (Source == 0)
					return Vector3.Zero;

				Vector3 v = Vector3.Zero;
				OpenAL.AL.GetSource(Source, OpenAL.ALSource3f.Position, out v.X, out v.Y, out v.Z);
				return v;
			}
			set
			{
				if (Source == 0)
					return;

				OpenAL.AL.Source(Source, OpenAL.ALSource3f.Position, value.X, value.Y, value.Z);
			}
		}


		/// <summary>
		/// Sound direction
		/// </summary>
		public Vector3 Direction
		{
			get
			{
				if (Source == 0)
					return Vector3.Zero;

				Vector3 v = Vector3.Zero;
				OpenAL.AL.GetSource(Source, OpenAL.ALSource3f.Direction, out v.X, out v.Y, out v.Z);
				return v;
			}
			set
			{
				if (Source == 0)
					return;

				OpenAL.AL.Source(Source, OpenAL.ALSource3f.Direction, value.X, value.Y, value.Z);
			}
		}


		/// <summary>
		/// Sound velocity
		/// </summary>
		public Vector3 Velocity
		{
			get
			{
				if (Source == 0)
					return Vector3.Zero;

				Vector3 v = Vector3.Zero;
				OpenAL.AL.GetSource(Source, OpenAL.ALSource3f.Velocity, out v.X, out v.Y, out v.Z);
				return v;
			}
			set
			{
				if (Source == 0)
					return;

				OpenAL.AL.Source(Source, OpenAL.ALSource3f.Velocity, value.X, value.Y, value.Z);
			}
		}


		/// <summary>
		/// Rolloff factor
		/// </summary>
		public float RolloffFactor
		{
			get
			{
				if (Source == 0)
					return 0.0f;

				float f;
				OpenAL.AL.GetSource(Source, OpenAL.ALSourcef.RolloffFactor, out f);
				return f;
			}
			set
			{
				if (Source == 0)
					return;

				OpenAL.AL.Source(Source, OpenAL.ALSourcef.RolloffFactor, value);
			}
		}


		/// <summary>
		/// Rolloff factor
		/// </summary>
		public bool SourceRelative
		{
			get
			{
				if (Source == 0)
					return false;

				bool b;
				OpenAL.AL.GetSource(Source, OpenAL.ALSourceb.SourceRelative, out b);
				return b;
			}
			set
			{
				if (Source == 0)
					return;

				OpenAL.AL.Source(Source, OpenAL.ALSourceb.SourceRelative, value);
			}
		}


		/// <summary>
		/// Ogg stream
		/// </summary>
		OggInputStream oggStream;


		/// <summary>
		/// File stream
		/// </summary>
		Stream Stream;


		/// <summary>
		/// File name of the audio stream
		/// </summary>
		string FileName;


		/// <summary>
		/// Loop playing
		/// </summary>
		public bool Loop;


		/// <summary>
		/// Name of the sound
		/// </summary>
		public string Name
		{
			get;
			set;
		}


		/// <summary>
		/// Is asset disposed
		/// </summary>
		public bool IsDisposed
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
				return "audiostream";
			}
		}


		/// <summary>
		/// Audio buffers
		/// </summary>
		int[] Buffers;


		/// <summary>
		/// Buffer data
		/// </summary>
		Dictionary<int, byte[]> BufferData;
		

		/// <summary>
		/// Audio source
		/// </summary>
		int Source;


		/// <summary>
		/// State of the stream
		/// </summary>
		public AudioSourceState State
		{
			get
			{
				if (Source == 0)
					return AudioSourceState.Initial;

				return (AudioSourceState) OpenAL.AL.GetSourceState(Source);
			}
		}

		#endregion

	}
}
