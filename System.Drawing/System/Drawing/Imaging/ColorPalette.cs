using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	public sealed class ColorPalette
	{
		public int Flags
		{
			get
			{
				return this._flags;
			}
		}

		public Color[] Entries
		{
			get
			{
				return this._entries;
			}
		}

		internal ColorPalette(int count)
		{
			this._entries = new Color[count];
		}

		internal ColorPalette()
		{
			this._entries = new Color[1];
		}

		internal void ConvertFromMemory(IntPtr memory)
		{
			this._flags = Marshal.ReadInt32(memory);
			int num = Marshal.ReadInt32((IntPtr)((long)memory + 4L));
			this._entries = new Color[num];
			for (int i = 0; i < num; i++)
			{
				int num2 = Marshal.ReadInt32((IntPtr)((long)memory + 8L + (long)(i * 4)));
				this._entries[i] = Color.FromArgb(num2);
			}
		}

		internal IntPtr ConvertToMemory()
		{
			int num = this._entries.Length;
			IntPtr intPtr;
			checked
			{
				intPtr = Marshal.AllocHGlobal(4 * (2 + num));
				Marshal.WriteInt32(intPtr, 0, this._flags);
				Marshal.WriteInt32((IntPtr)((long)intPtr + 4L), 0, num);
			}
			for (int i = 0; i < num; i++)
			{
				Marshal.WriteInt32((IntPtr)((long)intPtr + (long)(4 * (i + 2))), 0, this._entries[i].ToArgb());
			}
			return intPtr;
		}

		private int _flags;

		private Color[] _entries;
	}
}
