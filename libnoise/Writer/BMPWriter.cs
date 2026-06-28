using System;
using System.IO;
using LibNoiseDotNet.Graphics.Tools.Noise.Renderer;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Writer
{
	public class BMPWriter : AbstractWriter
	{
		public Image Image
		{
			get
			{
				return this._image;
			}
			set
			{
				this._image = value;
			}
		}

		public override void WriteFile()
		{
			if (this._image == null)
			{
				throw new ArgumentException("An image map must be provided");
			}
			int width = this._image.Width;
			int height = this._image.Height;
			int num = this.CalcWidthByteCount(width);
			int num2 = num * height;
			byte[] array = new byte[num];
			base.OpenFile();
			byte[] array2 = new byte[4];
			byte[] array3 = new byte[] { 66, 77 };
			try
			{
				this._writer.Write(array3);
				this._writer.Write(Libnoise.UnpackLittleUint32(num2 + 54, ref array2));
				this._writer.Write(Libnoise.UnpackLittleUint32(0, ref array2));
				this._writer.Write(Libnoise.UnpackLittleUint32(54, ref array2));
				this._writer.Write(Libnoise.UnpackLittleUint32(40, ref array2));
				this._writer.Write(Libnoise.UnpackLittleUint32(width, ref array2));
				this._writer.Write(Libnoise.UnpackLittleUint32(height, ref array2));
				this._writer.Write(Libnoise.UnpackLittleUint16(1, ref array3));
				this._writer.Write(Libnoise.UnpackLittleUint16(24, ref array3));
				this._writer.Write(Libnoise.UnpackLittleUint32(0, ref array2));
				this._writer.Write(Libnoise.UnpackLittleUint32(num2, ref array2));
				this._writer.Write(Libnoise.UnpackLittleUint32(2834, ref array2));
				this._writer.Write(Libnoise.UnpackLittleUint32(2834, ref array2));
				this._writer.Write(Libnoise.UnpackLittleUint32(0, ref array2));
				this._writer.Write(array2);
				for (int i = 0; i < height; i++)
				{
					int num3 = 0;
					Array.Clear(array, 0, array.Length);
					for (int j = 0; j < width; j++)
					{
						Color value = this._image.GetValue(j, i);
						array[num3++] = value.Blue;
						array[num3++] = value.Green;
						array[num3++] = value.Red;
					}
					this._writer.Write(array);
				}
			}
			catch (Exception ex)
			{
				throw new IOException("Unknown IO exception", ex);
			}
			base.CloseFile();
		}

		protected int CalcWidthByteCount(int width)
		{
			return (width * 3 + 3) & -4;
		}

		public const int BMP_HEADER_SIZE = 54;

		protected Image _image;
	}
}
