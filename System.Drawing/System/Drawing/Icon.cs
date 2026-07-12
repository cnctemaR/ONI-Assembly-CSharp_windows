using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Drawing
{
	[Editor("System.Drawing.Design.IconEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[TypeConverter(typeof(IconConverter))]
	[Serializable]
	public sealed class Icon : MarshalByRefObject, ISerializable, ICloneable, IDisposable
	{
		private Icon()
		{
		}

		private Icon(IntPtr handle)
		{
			this.handle = handle;
			this.bitmap = Bitmap.FromHicon(handle);
			this.iconSize = new Size(this.bitmap.Width, this.bitmap.Height);
			if (GDIPlus.RunningOnUnix())
			{
				this.bitmap = Bitmap.FromHicon(handle);
				this.iconSize = new Size(this.bitmap.Width, this.bitmap.Height);
			}
			else
			{
				IconInfo iconInfo;
				GDIPlus.GetIconInfo(handle, out iconInfo);
				if (!iconInfo.IsIcon)
				{
					throw new NotImplementedException(Locale.GetText("Handle doesn't represent an ICON."));
				}
				this.iconSize = new Size(iconInfo.xHotspot * 2, iconInfo.yHotspot * 2);
				this.bitmap = Image.FromHbitmap(iconInfo.hbmColor);
			}
			this.undisposable = true;
		}

		public Icon(Icon original, int width, int height)
			: this(original, new Size(width, height))
		{
		}

		public Icon(Icon original, Size size)
		{
			if (original == null)
			{
				throw new ArgumentException("original");
			}
			this.iconSize = size;
			this.iconDir = original.iconDir;
			int idCount = (int)this.iconDir.idCount;
			if (idCount > 0)
			{
				this.imageData = original.imageData;
				this.id = ushort.MaxValue;
				ushort num = 0;
				while ((int)num < idCount)
				{
					Icon.IconDirEntry iconDirEntry = this.iconDir.idEntries[(int)num];
					if (((int)iconDirEntry.height == size.Height || (int)iconDirEntry.width == size.Width) && !iconDirEntry.ignore)
					{
						this.id = num;
						break;
					}
					num += 1;
				}
				if (this.id == 65535)
				{
					int num2 = Math.Min(size.Height, size.Width);
					Icon.IconDirEntry? iconDirEntry2 = null;
					ushort num3 = 0;
					while ((int)num3 < idCount)
					{
						Icon.IconDirEntry iconDirEntry3 = this.iconDir.idEntries[(int)num3];
						if (((int)iconDirEntry3.height < num2 || (int)iconDirEntry3.width < num2) && !iconDirEntry3.ignore)
						{
							if (iconDirEntry2 == null)
							{
								iconDirEntry2 = new Icon.IconDirEntry?(iconDirEntry3);
								this.id = num3;
							}
							else if (iconDirEntry3.height > iconDirEntry2.Value.height || iconDirEntry3.width > iconDirEntry2.Value.width)
							{
								iconDirEntry2 = new Icon.IconDirEntry?(iconDirEntry3);
								this.id = num3;
							}
						}
						num3 += 1;
					}
				}
				if (this.id == 65535)
				{
					int num4 = idCount;
					while (this.id == 65535 && num4 > 0)
					{
						num4--;
						if (!this.iconDir.idEntries[num4].ignore)
						{
							this.id = (ushort)num4;
						}
					}
				}
				if (this.id == 65535)
				{
					throw new ArgumentException("Icon", "No valid icon image found");
				}
				this.iconSize.Height = (int)this.iconDir.idEntries[(int)this.id].height;
				this.iconSize.Width = (int)this.iconDir.idEntries[(int)this.id].width;
			}
			else
			{
				this.iconSize.Height = size.Height;
				this.iconSize.Width = size.Width;
			}
			if (original.bitmap != null)
			{
				this.bitmap = (Bitmap)original.bitmap.Clone();
			}
		}

		public Icon(Stream stream)
			: this(stream, 32, 32)
		{
		}

		public Icon(Stream stream, int width, int height)
		{
			this.InitFromStreamWithSize(stream, width, height);
		}

		public Icon(string fileName)
		{
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				this.InitFromStreamWithSize(fileStream, 32, 32);
			}
		}

		public Icon(Type type, string resource)
		{
			if (resource == null)
			{
				throw new ArgumentException("resource");
			}
			if (type == null)
			{
				throw new NullReferenceException();
			}
			using (Stream manifestResourceStream = type.GetTypeInfo().Assembly.GetManifestResourceStream(type, resource))
			{
				if (manifestResourceStream == null)
				{
					throw new FileNotFoundException(Locale.GetText("Resource '{0}' was not found.", new object[] { resource }));
				}
				this.InitFromStreamWithSize(manifestResourceStream, 32, 32);
			}
		}

		private Icon(SerializationInfo info, StreamingContext context)
		{
			MemoryStream memoryStream = null;
			int num = 0;
			int num2 = 0;
			foreach (SerializationEntry serializationEntry in info)
			{
				if (string.Compare(serializationEntry.Name, "IconData", true) == 0)
				{
					memoryStream = new MemoryStream((byte[])serializationEntry.Value);
				}
				if (string.Compare(serializationEntry.Name, "IconSize", true) == 0)
				{
					Size size = (Size)serializationEntry.Value;
					num = size.Width;
					num2 = size.Height;
				}
			}
			if (memoryStream != null)
			{
				memoryStream.Seek(0L, SeekOrigin.Begin);
				this.InitFromStreamWithSize(memoryStream, num, num2);
			}
		}

		internal Icon(string resourceName, bool undisposable)
		{
			using (Stream manifestResourceStream = typeof(Icon).GetTypeInfo().Assembly.GetManifestResourceStream(resourceName))
			{
				if (manifestResourceStream == null)
				{
					throw new FileNotFoundException(Locale.GetText("Resource '{0}' was not found.", new object[] { resourceName }));
				}
				this.InitFromStreamWithSize(manifestResourceStream, 32, 32);
			}
			this.undisposable = true;
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			MemoryStream memoryStream = new MemoryStream();
			this.Save(memoryStream);
			si.AddValue("IconSize", this.Size, typeof(Size));
			si.AddValue("IconData", memoryStream.ToArray());
		}

		public Icon(Stream stream, Size size)
			: this(stream, size.Width, size.Height)
		{
		}

		public Icon(string fileName, int width, int height)
		{
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				this.InitFromStreamWithSize(fileStream, width, height);
			}
		}

		public Icon(string fileName, Size size)
		{
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				this.InitFromStreamWithSize(fileStream, size.Width, size.Height);
			}
		}

		[MonoLimitation("The same icon, SystemIcons.WinLogo, is returned for all file types.")]
		public static Icon ExtractAssociatedIcon(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentException(Locale.GetText("Null or empty path."), "filePath");
			}
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException(Locale.GetText("Couldn't find specified file."), filePath);
			}
			return SystemIcons.WinLogo;
		}

		public void Dispose()
		{
			if (this.undisposable)
			{
				return;
			}
			if (!this.disposed)
			{
				if (GDIPlus.RunningOnWindows() && this.handle != IntPtr.Zero)
				{
					GDIPlus.DestroyIcon(this.handle);
					this.handle = IntPtr.Zero;
				}
				if (this.bitmap != null)
				{
					this.bitmap.Dispose();
					this.bitmap = null;
				}
				GC.SuppressFinalize(this);
			}
			this.disposed = true;
		}

		public object Clone()
		{
			return new Icon(this, this.Size);
		}

		public static Icon FromHandle(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentException("handle");
			}
			return new Icon(handle);
		}

		private void SaveIconImage(BinaryWriter writer, Icon.IconImage ii)
		{
			Icon.BitmapInfoHeader iconHeader = ii.iconHeader;
			writer.Write(iconHeader.biSize);
			writer.Write(iconHeader.biWidth);
			writer.Write(iconHeader.biHeight);
			writer.Write(iconHeader.biPlanes);
			writer.Write(iconHeader.biBitCount);
			writer.Write(iconHeader.biCompression);
			writer.Write(iconHeader.biSizeImage);
			writer.Write(iconHeader.biXPelsPerMeter);
			writer.Write(iconHeader.biYPelsPerMeter);
			writer.Write(iconHeader.biClrUsed);
			writer.Write(iconHeader.biClrImportant);
			int num = ii.iconColors.Length;
			for (int i = 0; i < num; i++)
			{
				writer.Write(ii.iconColors[i]);
			}
			writer.Write(ii.iconXOR);
			writer.Write(ii.iconAND);
		}

		private void SaveIconDump(BinaryWriter writer, Icon.IconDump id)
		{
			writer.Write(id.data);
		}

		private void SaveIconDirEntry(BinaryWriter writer, Icon.IconDirEntry ide, uint offset)
		{
			writer.Write(ide.width);
			writer.Write(ide.height);
			writer.Write(ide.colorCount);
			writer.Write(ide.reserved);
			writer.Write(ide.planes);
			writer.Write(ide.bitCount);
			writer.Write(ide.bytesInRes);
			writer.Write((offset == uint.MaxValue) ? ide.imageOffset : offset);
		}

		private void SaveAll(BinaryWriter writer)
		{
			writer.Write(this.iconDir.idReserved);
			writer.Write(this.iconDir.idType);
			ushort idCount = this.iconDir.idCount;
			writer.Write(idCount);
			for (int i = 0; i < (int)idCount; i++)
			{
				this.SaveIconDirEntry(writer, this.iconDir.idEntries[i], uint.MaxValue);
			}
			for (int j = 0; j < (int)idCount; j++)
			{
				while (writer.BaseStream.Length < (long)((ulong)this.iconDir.idEntries[j].imageOffset))
				{
					writer.Write(0);
				}
				if (this.imageData[j] is Icon.IconDump)
				{
					this.SaveIconDump(writer, (Icon.IconDump)this.imageData[j]);
				}
				else
				{
					this.SaveIconImage(writer, (Icon.IconImage)this.imageData[j]);
				}
			}
		}

		private void SaveBestSingleIcon(BinaryWriter writer, int width, int height)
		{
			writer.Write(this.iconDir.idReserved);
			writer.Write(this.iconDir.idType);
			writer.Write(1);
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < (int)this.iconDir.idCount; i++)
			{
				Icon.IconDirEntry iconDirEntry = this.iconDir.idEntries[i];
				if (width == (int)iconDirEntry.width && height == (int)iconDirEntry.height && (int)iconDirEntry.bitCount >= num2)
				{
					num2 = (int)iconDirEntry.bitCount;
					num = i;
				}
			}
			this.SaveIconDirEntry(writer, this.iconDir.idEntries[num], 22U);
			this.SaveIconImage(writer, (Icon.IconImage)this.imageData[num]);
		}

		private void SaveBitmapAsIcon(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(1);
			writer.Write(1);
			Icon.IconDirEntry iconDirEntry = default(Icon.IconDirEntry);
			iconDirEntry.width = (byte)this.bitmap.Width;
			iconDirEntry.height = (byte)this.bitmap.Height;
			iconDirEntry.colorCount = 0;
			iconDirEntry.reserved = 0;
			iconDirEntry.planes = 0;
			iconDirEntry.bitCount = 32;
			iconDirEntry.imageOffset = 22U;
			Icon.BitmapInfoHeader bitmapInfoHeader = default(Icon.BitmapInfoHeader);
			bitmapInfoHeader.biSize = (uint)Marshal.SizeOf(typeof(Icon.BitmapInfoHeader));
			bitmapInfoHeader.biWidth = this.bitmap.Width;
			bitmapInfoHeader.biHeight = 2 * this.bitmap.Height;
			bitmapInfoHeader.biPlanes = 1;
			bitmapInfoHeader.biBitCount = 32;
			bitmapInfoHeader.biCompression = 0U;
			bitmapInfoHeader.biSizeImage = 0U;
			bitmapInfoHeader.biXPelsPerMeter = 0;
			bitmapInfoHeader.biYPelsPerMeter = 0;
			bitmapInfoHeader.biClrUsed = 0U;
			bitmapInfoHeader.biClrImportant = 0U;
			Icon.IconImage iconImage = new Icon.IconImage();
			iconImage.iconHeader = bitmapInfoHeader;
			iconImage.iconColors = new uint[0];
			int num = ((((int)bitmapInfoHeader.biBitCount * this.bitmap.Width + 31) & -32) >> 3) * this.bitmap.Height;
			iconImage.iconXOR = new byte[num];
			int num2 = 0;
			for (int i = this.bitmap.Height - 1; i >= 0; i--)
			{
				for (int j = 0; j < this.bitmap.Width; j++)
				{
					Color pixel = this.bitmap.GetPixel(j, i);
					iconImage.iconXOR[num2++] = pixel.B;
					iconImage.iconXOR[num2++] = pixel.G;
					iconImage.iconXOR[num2++] = pixel.R;
					iconImage.iconXOR[num2++] = pixel.A;
				}
			}
			int num3 = (((this.Width + 31) & -32) >> 3) * this.bitmap.Height;
			iconImage.iconAND = new byte[num3];
			iconDirEntry.bytesInRes = (uint)((ulong)bitmapInfoHeader.biSize + (ulong)((long)num) + (ulong)((long)num3));
			this.SaveIconDirEntry(writer, iconDirEntry, uint.MaxValue);
			this.SaveIconImage(writer, iconImage);
		}

		private void Save(Stream outputStream, int width, int height)
		{
			BinaryWriter binaryWriter = new BinaryWriter(outputStream);
			if (this.iconDir.idEntries != null)
			{
				if (width == -1 && height == -1)
				{
					this.SaveAll(binaryWriter);
				}
				else
				{
					this.SaveBestSingleIcon(binaryWriter, width, height);
				}
			}
			else if (this.bitmap != null)
			{
				this.SaveBitmapAsIcon(binaryWriter);
			}
			binaryWriter.Flush();
		}

		public void Save(Stream outputStream)
		{
			if (outputStream == null)
			{
				throw new NullReferenceException("outputStream");
			}
			this.Save(outputStream, -1, -1);
		}

		internal Bitmap BuildBitmapOnWin32()
		{
			if (this.imageData == null)
			{
				return new Bitmap(32, 32);
			}
			Icon.IconImage iconImage = (Icon.IconImage)this.imageData[(int)this.id];
			Icon.BitmapInfoHeader iconHeader = iconImage.iconHeader;
			int num = iconHeader.biHeight / 2;
			if (iconHeader.biClrUsed == 0U)
			{
				ushort biBitCount = iconHeader.biBitCount;
			}
			ushort biBitCount2 = iconHeader.biBitCount;
			Bitmap bitmap;
			if (biBitCount2 <= 4)
			{
				if (biBitCount2 == 1)
				{
					bitmap = new Bitmap(iconHeader.biWidth, num, PixelFormat.Format1bppIndexed);
					goto IL_00FB;
				}
				if (biBitCount2 == 4)
				{
					bitmap = new Bitmap(iconHeader.biWidth, num, PixelFormat.Format4bppIndexed);
					goto IL_00FB;
				}
			}
			else
			{
				if (biBitCount2 == 8)
				{
					bitmap = new Bitmap(iconHeader.biWidth, num, PixelFormat.Format8bppIndexed);
					goto IL_00FB;
				}
				if (biBitCount2 == 24)
				{
					bitmap = new Bitmap(iconHeader.biWidth, num, PixelFormat.Format24bppRgb);
					goto IL_00FB;
				}
				if (biBitCount2 == 32)
				{
					bitmap = new Bitmap(iconHeader.biWidth, num, PixelFormat.Format32bppArgb);
					goto IL_00FB;
				}
			}
			throw new Exception(Locale.GetText("Unexpected number of bits: {0}", new object[] { iconHeader.biBitCount }));
			IL_00FB:
			if (iconHeader.biBitCount < 24)
			{
				ColorPalette palette = bitmap.Palette;
				for (int i = 0; i < iconImage.iconColors.Length; i++)
				{
					palette.Entries[i] = Color.FromArgb((int)(iconImage.iconColors[i] | 4278190080U));
				}
				bitmap.Palette = palette;
			}
			int num2 = ((iconHeader.biWidth * (int)iconHeader.biBitCount + 31) & -32) >> 3;
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
			for (int j = 0; j < num; j++)
			{
				Marshal.Copy(iconImage.iconXOR, num2 * j, (IntPtr)(bitmapData.Scan0.ToInt64() + (long)(bitmapData.Stride * (num - 1 - j))), num2);
			}
			bitmap.UnlockBits(bitmapData);
			bitmap = new Bitmap(bitmap);
			num2 = ((iconHeader.biWidth + 31) & -32) >> 3;
			for (int k = 0; k < num; k++)
			{
				for (int l = 0; l < iconHeader.biWidth / 8; l++)
				{
					for (int m = 7; m >= 0; m--)
					{
						if (((iconImage.iconAND[k * num2 + l] >> m) & 1) != 0)
						{
							bitmap.SetPixel(l * 8 + 7 - m, num - k - 1, Color.Transparent);
						}
					}
				}
			}
			return bitmap;
		}

		internal Bitmap GetInternalBitmap()
		{
			if (this.bitmap == null)
			{
				if (GDIPlus.RunningOnUnix())
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						this.Save(memoryStream, this.Width, this.Height);
						memoryStream.Position = 0L;
						this.bitmap = (Bitmap)Image.LoadFromStream(memoryStream, false);
						goto IL_005A;
					}
				}
				this.bitmap = this.BuildBitmapOnWin32();
			}
			IL_005A:
			return this.bitmap;
		}

		public Bitmap ToBitmap()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(Locale.GetText("Icon instance was disposed."));
			}
			return new Bitmap(this.GetInternalBitmap());
		}

		public override string ToString()
		{
			return "<Icon>";
		}

		[Browsable(false)]
		public IntPtr Handle
		{
			get
			{
				if (!this.disposed && this.handle == IntPtr.Zero)
				{
					if (GDIPlus.RunningOnUnix())
					{
						this.handle = this.GetInternalBitmap().NativeObject;
					}
					else
					{
						IconInfo iconInfo = default(IconInfo);
						iconInfo.IsIcon = true;
						iconInfo.hbmColor = this.ToBitmap().GetHbitmap();
						iconInfo.hbmMask = iconInfo.hbmColor;
						this.handle = GDIPlus.CreateIconIndirect(ref iconInfo);
					}
				}
				return this.handle;
			}
		}

		[Browsable(false)]
		public int Height
		{
			get
			{
				return this.iconSize.Height;
			}
		}

		public Size Size
		{
			get
			{
				return this.iconSize;
			}
		}

		[Browsable(false)]
		public int Width
		{
			get
			{
				return this.iconSize.Width;
			}
		}

		~Icon()
		{
			this.Dispose();
		}

		private void InitFromStreamWithSize(Stream stream, int width, int height)
		{
			if (stream == null || stream.Length == 0L)
			{
				throw new ArgumentException("The argument 'stream' must be a picture that can be used as a Icon", "stream");
			}
			BinaryReader binaryReader = new BinaryReader(stream);
			this.iconDir.idReserved = binaryReader.ReadUInt16();
			if (this.iconDir.idReserved != 0)
			{
				throw new ArgumentException("Invalid Argument", "stream");
			}
			this.iconDir.idType = binaryReader.ReadUInt16();
			if (this.iconDir.idType != 1)
			{
				throw new ArgumentException("Invalid Argument", "stream");
			}
			ushort num = binaryReader.ReadUInt16();
			this.imageData = new Icon.ImageData[(int)num];
			this.iconDir.idCount = num;
			this.iconDir.idEntries = new Icon.IconDirEntry[(int)num];
			bool flag = false;
			for (int i = 0; i < (int)num; i++)
			{
				Icon.IconDirEntry iconDirEntry;
				iconDirEntry.width = binaryReader.ReadByte();
				iconDirEntry.height = binaryReader.ReadByte();
				iconDirEntry.colorCount = binaryReader.ReadByte();
				iconDirEntry.reserved = binaryReader.ReadByte();
				iconDirEntry.planes = binaryReader.ReadUInt16();
				iconDirEntry.bitCount = binaryReader.ReadUInt16();
				iconDirEntry.bytesInRes = binaryReader.ReadUInt32();
				iconDirEntry.imageOffset = binaryReader.ReadUInt32();
				if (iconDirEntry.width == 0 && iconDirEntry.height == 0)
				{
					iconDirEntry.ignore = true;
				}
				else
				{
					iconDirEntry.ignore = false;
				}
				this.iconDir.idEntries[i] = iconDirEntry;
				if (!flag && ((int)iconDirEntry.height == height || (int)iconDirEntry.width == width) && !iconDirEntry.ignore)
				{
					this.id = (ushort)i;
					flag = true;
					this.iconSize.Height = (int)iconDirEntry.height;
					this.iconSize.Width = (int)iconDirEntry.width;
				}
			}
			int num2 = 0;
			for (int j = 0; j < (int)num; j++)
			{
				if (!this.iconDir.idEntries[j].ignore)
				{
					num2++;
				}
			}
			if (num2 == 0)
			{
				throw new Win32Exception(0, "No valid icon entry were found.");
			}
			if (!flag)
			{
				uint num3 = 0U;
				for (int k = 0; k < (int)num; k++)
				{
					if (this.iconDir.idEntries[k].bytesInRes >= num3 && !this.iconDir.idEntries[k].ignore)
					{
						num3 = this.iconDir.idEntries[k].bytesInRes;
						this.id = (ushort)k;
						this.iconSize.Height = (int)this.iconDir.idEntries[k].height;
						this.iconSize.Width = (int)this.iconDir.idEntries[k].width;
					}
				}
			}
			for (int l = 0; l < (int)num; l++)
			{
				if (this.iconDir.idEntries[l].ignore)
				{
					Icon.IconDump iconDump = new Icon.IconDump();
					stream.Seek((long)((ulong)this.iconDir.idEntries[l].imageOffset), SeekOrigin.Begin);
					iconDump.data = new byte[this.iconDir.idEntries[l].bytesInRes];
					stream.Read(iconDump.data, 0, iconDump.data.Length);
					this.imageData[l] = iconDump;
				}
				else
				{
					Icon.IconImage iconImage = new Icon.IconImage();
					Icon.BitmapInfoHeader bitmapInfoHeader = default(Icon.BitmapInfoHeader);
					stream.Seek((long)((ulong)this.iconDir.idEntries[l].imageOffset), SeekOrigin.Begin);
					byte[] array = new byte[this.iconDir.idEntries[l].bytesInRes];
					stream.Read(array, 0, array.Length);
					BinaryReader binaryReader2 = new BinaryReader(new MemoryStream(array));
					bitmapInfoHeader.biSize = binaryReader2.ReadUInt32();
					bitmapInfoHeader.biWidth = binaryReader2.ReadInt32();
					bitmapInfoHeader.biHeight = binaryReader2.ReadInt32();
					bitmapInfoHeader.biPlanes = binaryReader2.ReadUInt16();
					bitmapInfoHeader.biBitCount = binaryReader2.ReadUInt16();
					bitmapInfoHeader.biCompression = binaryReader2.ReadUInt32();
					bitmapInfoHeader.biSizeImage = binaryReader2.ReadUInt32();
					bitmapInfoHeader.biXPelsPerMeter = binaryReader2.ReadInt32();
					bitmapInfoHeader.biYPelsPerMeter = binaryReader2.ReadInt32();
					bitmapInfoHeader.biClrUsed = binaryReader2.ReadUInt32();
					bitmapInfoHeader.biClrImportant = binaryReader2.ReadUInt32();
					iconImage.iconHeader = bitmapInfoHeader;
					ushort biBitCount = bitmapInfoHeader.biBitCount;
					int num4;
					if (biBitCount != 1)
					{
						if (biBitCount != 4)
						{
							if (biBitCount != 8)
							{
								num4 = 0;
							}
							else
							{
								num4 = 256;
							}
						}
						else
						{
							num4 = 16;
						}
					}
					else
					{
						num4 = 2;
					}
					iconImage.iconColors = new uint[num4];
					for (int m = 0; m < num4; m++)
					{
						iconImage.iconColors[m] = binaryReader2.ReadUInt32();
					}
					int num5 = bitmapInfoHeader.biHeight / 2;
					int num6 = (bitmapInfoHeader.biWidth * (int)bitmapInfoHeader.biPlanes * (int)bitmapInfoHeader.biBitCount + 31 >> 5 << 2) * num5;
					iconImage.iconXOR = new byte[num6];
					int num7 = binaryReader2.Read(iconImage.iconXOR, 0, num6);
					if (num7 != num6)
					{
						throw new ArgumentException(Locale.GetText("{0} data length expected {1}, read {2}", new object[] { "XOR", num6, num7 }), "stream");
					}
					int num8 = (((bitmapInfoHeader.biWidth + 31) & -32) >> 3) * num5;
					iconImage.iconAND = new byte[num8];
					num7 = binaryReader2.Read(iconImage.iconAND, 0, num8);
					if (num7 != num8)
					{
						throw new ArgumentException(Locale.GetText("{0} data length expected {1}, read {2}", new object[] { "AND", num8, num7 }), "stream");
					}
					this.imageData[l] = iconImage;
					binaryReader2.Dispose();
				}
			}
			binaryReader.Dispose();
		}

		private Size iconSize;

		private IntPtr handle = IntPtr.Zero;

		private Icon.IconDir iconDir;

		private ushort id;

		private Icon.ImageData[] imageData;

		private bool undisposable;

		private bool disposed;

		private Bitmap bitmap;

		internal struct IconDirEntry
		{
			internal byte width;

			internal byte height;

			internal byte colorCount;

			internal byte reserved;

			internal ushort planes;

			internal ushort bitCount;

			internal uint bytesInRes;

			internal uint imageOffset;

			internal bool ignore;
		}

		internal struct IconDir
		{
			internal ushort idReserved;

			internal ushort idType;

			internal ushort idCount;

			internal Icon.IconDirEntry[] idEntries;
		}

		internal struct BitmapInfoHeader
		{
			internal uint biSize;

			internal int biWidth;

			internal int biHeight;

			internal ushort biPlanes;

			internal ushort biBitCount;

			internal uint biCompression;

			internal uint biSizeImage;

			internal int biXPelsPerMeter;

			internal int biYPelsPerMeter;

			internal uint biClrUsed;

			internal uint biClrImportant;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal abstract class ImageData
		{
		}

		[StructLayout(LayoutKind.Sequential)]
		internal class IconImage : Icon.ImageData
		{
			internal Icon.BitmapInfoHeader iconHeader;

			internal uint[] iconColors;

			internal byte[] iconXOR;

			internal byte[] iconAND;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal class IconDump : Icon.ImageData
		{
			internal byte[] data;
		}
	}
}
