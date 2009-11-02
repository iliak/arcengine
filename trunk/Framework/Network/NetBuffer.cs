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
using System.Text;


namespace ArcEngine.Network
{
	internal sealed class NetBuffer
	{
		public byte[] Data;

		private int m_bitLength;
		private int m_readBitPtr;

		public int LengthBytes { get { return (m_bitLength >> 3) + ((m_bitLength & 7) > 0 ? 1 : 0); } }
		public int LengthBits { get { return m_bitLength; } set { m_bitLength = value; } }

		public void ResetReadPointer() { m_readBitPtr = 0; }
		public void ResetWritePointer() { m_bitLength = 0; }
		public void ResetReadPointer(int position) { m_readBitPtr = position; }
		public void ResetWritePointer(int position) { m_bitLength = position; }
		public void ResetPointers() { m_readBitPtr = 0; m_bitLength = 0; }

		public int ReadBitsLeft { get { return m_bitLength - m_readBitPtr; } }

		internal NetBuffer()
		{
			// Data to be set later
		}

		internal void SetDataLength(int numBytes)
		{
			m_bitLength = numBytes * 8;
		}

		public NetBuffer(int initialSizeInBytes)
		{
			Data = new byte[initialSizeInBytes];
		}

		public NetBuffer(byte[] fromBuffer)
		{
			Data = fromBuffer;
			m_bitLength = (fromBuffer == null ? 0 : fromBuffer.Length * 8);
		}

		private void EnsureSizeWrite(int numberOfBits)
		{
			EnsureBufferSize(m_bitLength + numberOfBits);
		}

		public void EnsureBufferSize(int numberOfBits)
		{
			int byteLen = (numberOfBits >> 3) + ((numberOfBits & 7) > 0 ? 1 : 0);
			if (Data == null)
			{
				Data = new byte[byteLen + 4]; // overallocate 4 bytes
				return;
			}
			if (Data.Length < byteLen)
				Array.Resize<byte>(ref Data, byteLen + 4); // overallocate 4 bytes
			return;
		}

		internal byte[] ToArray()
		{
			int len = LengthBytes;
			byte[] copy = new byte[len];
			Array.Copy(Data, copy, copy.Length);
			return copy;
		}

		#region Write Methods
/*
		public void Write(byte source)
		{
			EnsureSizeWrite(8);
			NetBase.BitWriter.WriteByte(source, 8, Data, m_bitLength);
			m_bitLength += 8;
		}

		public void Write(byte[] source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			EnsureSizeWrite(source.Length * 8);

			NetBase.BitWriter.WriteBytes(source, 0, source.Length, Data, m_bitLength);
			m_bitLength += (source.Length * 8);
		}

		public void Write(byte source, int numberOfBits)
		{
			Debug.Assert((numberOfBits > 0 && numberOfBits <= 8), "Write(byte, numberOfBits) can only write between 1 and 8 bits");
			EnsureSizeWrite(numberOfBits);
			NetBase.BitWriter.WriteByte(source, numberOfBits, Data, m_bitLength);
			m_bitLength += numberOfBits;
		}

		public void Write(byte[] source, int offset, int numberOfBytes)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			EnsureSizeWrite(numberOfBytes * 8);

			NetBase.BitWriter.WriteBytes(source, offset, numberOfBytes, Data, m_bitLength);
			m_bitLength += (numberOfBytes * 8);
		}

		public void Write(ushort source)
		{
			EnsureSizeWrite(16);
			NetBase.BitWriter.WriteUInt32((uint)source, 16, Data, m_bitLength);
			m_bitLength += 16;
		}

		public void Write(short source)
		{
			EnsureSizeWrite(16);
			NetBase.BitWriter.WriteUInt32((uint)source, 16, Data, m_bitLength);
			m_bitLength += 16;
		}

		public void Write(bool y)
		{
			EnsureSizeWrite(1);
			NetBase.BitWriter.WriteByte((y ? (byte)1 : (byte)0), 1, Data, m_bitLength);
			m_bitLength += 1;
		}

		public unsafe void Write(Int32 source)
		{
			EnsureSizeWrite(32);

			// can write fast?
			if (m_bitLength % 8 == 0)
			{
				fixed (byte* numRef = &Data[m_bitLength / 8])
				{
					*((int*)numRef) = source;
				}
			}
			else
			{
				NetBase.BitWriter.WriteUInt32((UInt32)source, 32, Data, m_bitLength);
			}
			m_bitLength += 32;
		}

		public unsafe void Write(UInt32 source)
		{
			EnsureSizeWrite(32);

			// can write fast?
			if (m_bitLength % 8 == 0)
			{
				fixed (byte* numRef = &Data[m_bitLength / 8])
				{
					*((uint*)numRef) = source;
				}
			}
			else
			{
				NetBase.BitWriter.WriteUInt32(source, 32, Data, m_bitLength);
			}

			m_bitLength += 32;
		}

		public unsafe void Write(double source)
		{
			ulong val = *((ulong*)&source);
			if (!NetBase.IsLittleEndian)
				val = NetUtil.SwapByteOrder(val);
			Write(val);
		}

		public unsafe void Write(float source)
		{
			uint val = *((uint*)&source);
			if (!NetBase.IsLittleEndian)
				val = NetUtil.SwapByteOrder(val);
			Write(val);
		}

		public void Write(UInt64 source)
		{
			EnsureSizeWrite(64);
			NetBase.BitWriter.WriteUInt64(source, 64, Data, m_bitLength);
			m_bitLength += 64;
		}

		public void Write(UInt64 source, int numBits)
		{
			EnsureSizeWrite(numBits);
			NetBase.BitWriter.WriteUInt64(source, numBits, Data, m_bitLength);
			m_bitLength += numBits;
		}

		public void Write(Int64 source)
		{
			EnsureSizeWrite(64);
			ulong usource = (ulong)source;
			NetBase.BitWriter.WriteUInt64(usource, 64, Data, m_bitLength);
			m_bitLength += 64;
		}

		public void Write(ushort source, int numberOfBits)
		{
			Debug.Assert((numberOfBits > 0 && numberOfBits <= 16), "Write(ushort, numberOfBits) can only write between 1 and 16 bits");
			EnsureSizeWrite(numberOfBits);
			NetBase.BitWriter.WriteUInt32((uint)source, numberOfBits, Data, m_bitLength);
			m_bitLength += numberOfBits;
		}

		public void Write(uint source, int numberOfBits)
		{
			Debug.Assert((numberOfBits > 0 && numberOfBits <= 32), "Write(uint, numberOfBits) can only write between 1 and 32 bits");
			EnsureSizeWrite(numberOfBits);
			NetBase.BitWriter.WriteUInt32(source, numberOfBits, Data, m_bitLength);
			m_bitLength += numberOfBits;
		}

		public void Write(int source, int numberOfBits)
		{
			Debug.Assert((numberOfBits > 0 && numberOfBits <= 32), "Write(int, numberOfBits) can only write between 1 and 32 bits");
			EnsureSizeWrite(numberOfBits);

			if (numberOfBits != 32)
			{
				// make first bit sign
				int signBit = 1 << (numberOfBits - 1);
				if (source < 0)
					source = (-source - 1) | signBit;
				else
					source &= (~signBit);
			}

			NetBase.BitWriter.WriteUInt32((uint)source, numberOfBits, Data, m_bitLength);

			m_bitLength += numberOfBits;
		}

		public void Write(string str)
		{
			NetAppConfiguration config = NetBase.CurrentContext.Configuration;

			if (string.IsNullOrEmpty(str))
			{
				Write7BitEncodedUInt(0);
				return;
			}

			byte[] bytes = config.StringEncoding.GetBytes(str);

			Write7BitEncodedUInt((uint)bytes.Length);
			Write(bytes);
		}
*/
		#endregion

		#region Read Methods
/*
		public byte ReadByte()
		{
			//Debug.Assert(m_bitLength - m_readBitPtr >= 8, "tried to read past buffer size");
			byte retval = NetBase.BitWriter.ReadByte(Data, 8, m_readBitPtr);
			m_readBitPtr += 8;
			return retval;
		}

		public bool ReadBoolean()
		{
			//Debug.Assert(m_bitLength - m_readBitPtr >= 1, "tried to read past buffer size");
			byte retval = NetBase.BitWriter.ReadByte(Data, 1, m_readBitPtr);
			m_readBitPtr += 1;
			return (retval > 0 ? true : false);
		}

		public byte ReadByte(int numberOfBits)
		{
			Debug.Assert((numberOfBits > 0 && numberOfBits <= 8), "ReadByte() can only read between 1 and 8 bits");
			//Debug.Assert(m_bitLength - m_readBitPtr >= numberOfBits, "tried to read past buffer size");

			byte retval = NetBase.BitWriter.ReadByte(Data, numberOfBits, m_readBitPtr);
			m_readBitPtr += numberOfBits;
			return retval;
		}

		public byte[] ReadBytes(int numberOfBytes)
		{
			byte[] retval = new byte[numberOfBytes];
			NetBase.BitWriter.ReadBytes(Data, numberOfBytes, m_readBitPtr, retval, 0);
			m_readBitPtr += (8 * numberOfBytes);
			return retval;
		}

		public short ReadInt16()
		{
			Debug.Assert(m_bitLength - m_readBitPtr >= 16, "tried to read past buffer size");
			uint retval = NetBase.BitWriter.ReadUInt32(Data, 16, m_readBitPtr);
			m_readBitPtr += 16;
			return (short)retval;
		}

		public ushort ReadUInt16()
		{
			Debug.Assert(m_bitLength - m_readBitPtr >= 16, "tried to read past buffer size");
			uint retval = NetBase.BitWriter.ReadUInt32(Data, 16, m_readBitPtr);
			m_readBitPtr += 16;
			return (ushort)retval;
		}

		public Int32 ReadInt32()
		{
			Debug.Assert(m_bitLength - m_readBitPtr >= 32, "tried to read past buffer size");
			uint retval = NetBase.BitWriter.ReadUInt32(Data, 32, m_readBitPtr);
			m_readBitPtr += 32;
			return (Int32)retval;
		}

		public int ReadInt32(int numberOfBits)
		{
			Debug.Assert((numberOfBits > 0 && numberOfBits <= 32), "ReadInt() can only read between 1 and 32 bits");
			Debug.Assert(m_bitLength - m_readBitPtr >= numberOfBits, "tried to read past buffer size");

			uint retval = NetBase.BitWriter.ReadUInt32(Data, numberOfBits, m_readBitPtr);
			m_readBitPtr += numberOfBits;

			if (numberOfBits == 32)
				return (int)retval;

			int signBit = 1 << (numberOfBits - 1);
			if ((retval & signBit) == 0)
				return (int)retval; // positive

			// negative
			unchecked
			{
				uint mask = ((uint)-1) >> (33 - numberOfBits);
				uint tmp = (retval & mask) + 1;
				return -((int)tmp);
			}
		}

		public UInt32 ReadUInt32()
		{
			uint retval = NetBase.BitWriter.ReadUInt32(Data, 32, m_readBitPtr);
			m_readBitPtr += 32;
			return retval;
		}

		public uint ReadUInt32(int numberOfBits)
		{
			Debug.Assert((numberOfBits > 0 && numberOfBits <= 32), "ReadUInt() can only read between 1 and 32 bits");
			//Debug.Assert(m_bitLength - m_readBitPtr >= numberOfBits, "tried to read past buffer size");

			uint retval = NetBase.BitWriter.ReadUInt32(Data, numberOfBits, m_readBitPtr);
			m_readBitPtr += numberOfBits;
			return retval;
		}

		public float ReadSingle()
		{
			return ReadFloat();
		}

		public float ReadFloat()
		{
			Debug.Assert(m_bitLength - m_readBitPtr >= (4 * 8), "tried to read past buffer size");
			byte[] bytes = ReadBytes(4);
			if (!NetBase.IsLittleEndian)
			{
				// flip bytes
				byte tmp = bytes[0];
				bytes[0] = bytes[3];
				bytes[3] = tmp;

				tmp = bytes[1];
				bytes[1] = bytes[2];
				bytes[2] = tmp;
			}
			return BitConverter.ToSingle(bytes, 0);
		}

		public double ReadDouble()
		{
			Debug.Assert(m_bitLength - m_readBitPtr >= (8 * 8), "tried to read past buffer size");
			byte[] bytes = ReadBytes(8);
			if (!NetBase.IsLittleEndian)
			{
				byte tmp = bytes[0];
				bytes[0] = bytes[7];
				bytes[7] = tmp;

				tmp = bytes[1];
				bytes[1] = bytes[6];
				bytes[6] = tmp;

				tmp = bytes[2];
				bytes[2] = bytes[5];
				bytes[5] = tmp;

				tmp = bytes[3];
				bytes[3] = bytes[4];
				bytes[4] = tmp;
			}
			return BitConverter.ToDouble(bytes, 0);
		}

		public UInt64 ReadUInt64()
		{
			Debug.Assert(m_bitLength - m_readBitPtr >= 64, "tried to read past buffer size");

			ulong low = NetBase.BitWriter.ReadUInt32(Data, 32, m_readBitPtr);
			m_readBitPtr += 32;
			ulong high = NetBase.BitWriter.ReadUInt32(Data, 32, m_readBitPtr);

			ulong retval = low + (high << 32);

			m_readBitPtr += 32;
			return retval;
		}

		public Int64 ReadInt64()
		{
			Debug.Assert(m_bitLength - m_readBitPtr >= 64, "tried to read past buffer size");
			unchecked
			{
				ulong retval = ReadUInt64();
				long longRetval = (long)retval;
				return longRetval;
			}
		}

		public UInt64 ReadUInt64(int numberOfBits)
		{
			Debug.Assert((numberOfBits > 0 && numberOfBits <= 64), "ReadUInt() can only read between 1 and 64 bits");
			Debug.Assert(m_bitLength - m_readBitPtr >= numberOfBits, "tried to read past buffer size");

			ulong retval;
			if (numberOfBits <= 32)
			{
				retval = (ulong)NetBase.BitWriter.ReadUInt32(Data, numberOfBits, m_readBitPtr);
			}
			else
			{
				retval = NetBase.BitWriter.ReadUInt32(Data, 32, m_readBitPtr);
				retval |= NetBase.BitWriter.ReadUInt32(Data, numberOfBits - 32, m_readBitPtr) << 32;
			}
			m_readBitPtr += numberOfBits;
			return retval;
		}

		public Int64 ReadInt64(int numberOfBits)
		{
			Debug.Assert(((numberOfBits > 0) && (numberOfBits < 65)), "ReadInt64(bits) can only read between 1 and 64 bits");
			return (long)ReadUInt64(numberOfBits);
		}

		public string ReadString()
		{
			NetAppConfiguration config = NetBase.CurrentContext.Configuration;

			int byteLen = (int)Read7BitEncodedUInt();

			Encoding enc = config.StringEncoding;

			// verify we have enough data
			if (m_readBitPtr + (byteLen * 8) > this.LengthBits)
			{
				int rem = (this.LengthBits - m_readBitPtr) / 8;
				throw new IndexOutOfRangeException("ReadString() tried to read " + byteLen + " bytes; but remainder of message only has " + rem + " bytes left");
			}

			byte[] bytes = ReadBytes(byteLen);

			return enc.GetString(bytes, 0, bytes.Length);
		}
*/
		#endregion

		#region Peek Methods
/*
		public byte PeekByte()
		{
			return NetBase.BitWriter.ReadByte(Data, 8, m_readBitPtr);
		}

		public byte PeekByte(int numberOfBits)
		{
			Debug.Assert(((numberOfBits > 0) && (numberOfBits < 9)), "PeekByte(bits) can only read between 1 and 8 bits");
			return NetBase.BitWriter.ReadByte(Data, numberOfBits, m_readBitPtr);
		}

		public uint PeekUInt32()
		{
			return NetBase.BitWriter.ReadUInt32(Data, 32, m_readBitPtr);
		}

		public uint PeekUInt32(int numberOfBits)
		{
			Debug.Assert(((numberOfBits > 0) && (numberOfBits < 33)), "PeekUInt32(bits) can only read between 1 and 32 bits");
			return NetBase.BitWriter.ReadUInt32(Data, numberOfBits, m_readBitPtr);
		}

		public ushort PeekUInt16()
		{
			return (ushort)NetBase.BitWriter.ReadUInt32(Data, 16, m_readBitPtr);
		}

		public ushort PeekUInt16(int numberOfBits)
		{
			Debug.Assert(((numberOfBits > 0) && (numberOfBits < 17)), "PeekUInt32(bits) can only read between 1 and 16 bits");
			return (ushort)NetBase.BitWriter.ReadUInt32(Data, numberOfBits, m_readBitPtr);
		}
*/
		#endregion
	}
}
