using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Drawing
{
	[TypeConverter(typeof(ImageConverter))]
	[ComVisible(true)]
	[Editor("System.Drawing.Design.ImageEditor, System.Drawing.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[ImmutableObject(true)]
	[Serializable]
	public abstract class Image : MarshalByRefObject, IDisposable, ICloneable, ISerializable
	{
		internal Image()
		{
		}

		internal Image(SerializationInfo info, StreamingContext context)
		{
			foreach (SerializationEntry serializationEntry in info)
			{
				if (string.Compare(serializationEntry.Name, "Data", true) == 0)
				{
					byte[] array = (byte[])serializationEntry.Value;
					if (array != null)
					{
						MemoryStream memoryStream = new MemoryStream(array);
						this.nativeObject = Image.InitFromStream(memoryStream);
						if (GDIPlus.RunningOnWindows())
						{
							this.stream = memoryStream;
						}
					}
				}
			}
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				if (this.RawFormat.Equals(ImageFormat.Icon))
				{
					this.Save(memoryStream, ImageFormat.Png);
				}
				else
				{
					this.Save(memoryStream, this.RawFormat);
				}
				si.AddValue("Data", memoryStream.ToArray());
			}
		}

		public static Image FromFile(string filename)
		{
			return Image.FromFile(filename, false);
		}

		public static Image FromFile(string filename, bool useEmbeddedColorManagement)
		{
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException(filename);
			}
			IntPtr intPtr;
			Status status;
			if (useEmbeddedColorManagement)
			{
				status = GDIPlus.GdipLoadImageFromFileICM(filename, out intPtr);
			}
			else
			{
				status = GDIPlus.GdipLoadImageFromFile(filename, out intPtr);
			}
			GDIPlus.CheckStatus(status);
			return Image.CreateFromHandle(intPtr);
		}

		public static Bitmap FromHbitmap(IntPtr hbitmap)
		{
			return Image.FromHbitmap(hbitmap, IntPtr.Zero);
		}

		public static Bitmap FromHbitmap(IntPtr hbitmap, IntPtr hpalette)
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCreateBitmapFromHBITMAP(hbitmap, hpalette, out intPtr);
			GDIPlus.CheckStatus(status);
			return new Bitmap(intPtr);
		}

		public static Image FromStream(Stream stream)
		{
			return Image.LoadFromStream(stream, false);
		}

		[MonoLimitation("useEmbeddedColorManagement  isn't supported.")]
		public static Image FromStream(Stream stream, bool useEmbeddedColorManagement)
		{
			return Image.LoadFromStream(stream, false);
		}

		[MonoLimitation("useEmbeddedColorManagement  and validateImageData aren't supported.")]
		public static Image FromStream(Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
		{
			return Image.LoadFromStream(stream, false);
		}

		internal static Image LoadFromStream(Stream stream, bool keepAlive)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			Image image = Image.CreateFromHandle(Image.InitFromStream(stream));
			if (keepAlive && GDIPlus.RunningOnWindows())
			{
				image.stream = stream;
			}
			return image;
		}

		internal static Image CreateFromHandle(IntPtr handle)
		{
			ImageType imageType;
			GDIPlus.CheckStatus(GDIPlus.GdipGetImageType(handle, out imageType));
			ImageType imageType2 = imageType;
			if (imageType2 == ImageType.Bitmap)
			{
				return new Bitmap(handle);
			}
			if (imageType2 != ImageType.Metafile)
			{
				throw new NotSupportedException(Locale.GetText("Unknown image type."));
			}
			return new Metafile(handle);
		}

		public static int GetPixelFormatSize(PixelFormat pixfmt)
		{
			int num = 0;
			if (pixfmt != PixelFormat.Format16bppRgb555 && pixfmt != PixelFormat.Format16bppRgb565)
			{
				if (pixfmt != PixelFormat.Format24bppRgb)
				{
					if (pixfmt != PixelFormat.Format32bppRgb)
					{
						if (pixfmt == PixelFormat.Format1bppIndexed)
						{
							return 1;
						}
						if (pixfmt == PixelFormat.Format4bppIndexed)
						{
							return 4;
						}
						if (pixfmt == PixelFormat.Format8bppIndexed)
						{
							return 8;
						}
						if (pixfmt == PixelFormat.Format16bppArgb1555)
						{
							goto IL_00A3;
						}
						if (pixfmt != PixelFormat.Format32bppPArgb)
						{
							if (pixfmt == PixelFormat.Format16bppGrayScale)
							{
								goto IL_00A3;
							}
							if (pixfmt != PixelFormat.Format48bppRgb)
							{
								if (pixfmt != PixelFormat.Format64bppPArgb)
								{
									if (pixfmt == PixelFormat.Format32bppArgb)
									{
										goto IL_00BA;
									}
									if (pixfmt != PixelFormat.Format64bppArgb)
									{
										return num;
									}
								}
								return 64;
							}
							return 48;
						}
					}
					IL_00BA:
					return 32;
				}
				return 24;
			}
			IL_00A3:
			num = 16;
			return num;
		}

		public static bool IsAlphaPixelFormat(PixelFormat pixfmt)
		{
			bool flag = false;
			if (pixfmt != PixelFormat.Format16bppRgb555 && pixfmt != PixelFormat.Format16bppRgb565 && pixfmt != PixelFormat.Format24bppRgb && pixfmt != PixelFormat.Format32bppRgb && pixfmt != PixelFormat.Format1bppIndexed && pixfmt != PixelFormat.Format4bppIndexed && pixfmt != PixelFormat.Format8bppIndexed)
			{
				if (pixfmt != PixelFormat.Format16bppArgb1555 && pixfmt != PixelFormat.Format32bppPArgb)
				{
					if (pixfmt == PixelFormat.Format16bppGrayScale || pixfmt == PixelFormat.Format48bppRgb)
					{
						goto IL_00AA;
					}
					if (pixfmt != PixelFormat.Format64bppPArgb && pixfmt != PixelFormat.Format32bppArgb && pixfmt != PixelFormat.Format64bppArgb)
					{
						return flag;
					}
				}
				return true;
			}
			IL_00AA:
			flag = false;
			return flag;
		}

		public static bool IsCanonicalPixelFormat(PixelFormat pixfmt)
		{
			return (pixfmt & PixelFormat.Canonical) != PixelFormat.DontCare;
		}

		public static bool IsExtendedPixelFormat(PixelFormat pixfmt)
		{
			return (pixfmt & PixelFormat.Extended) != PixelFormat.DontCare;
		}

		internal static IntPtr InitFromStream(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentException("stream");
			}
			if (!stream.CanSeek)
			{
				byte[] array = new byte[256];
				int num = 0;
				int num2;
				do
				{
					if (array.Length < num + 256)
					{
						byte[] array2 = new byte[array.Length * 2];
						Array.Copy(array, array2, array.Length);
						array = array2;
					}
					num2 = stream.Read(array, num, 256);
					num += num2;
				}
				while (num2 != 0);
				stream = new MemoryStream(array, 0, num);
			}
			IntPtr intPtr;
			Status status;
			if (GDIPlus.RunningOnUnix())
			{
				GDIPlus.GdiPlusStreamHelper gdiPlusStreamHelper = new GDIPlus.GdiPlusStreamHelper(stream, true);
				status = GDIPlus.GdipLoadImageFromDelegate_linux(gdiPlusStreamHelper.GetHeaderDelegate, gdiPlusStreamHelper.GetBytesDelegate, gdiPlusStreamHelper.PutBytesDelegate, gdiPlusStreamHelper.SeekDelegate, gdiPlusStreamHelper.CloseDelegate, gdiPlusStreamHelper.SizeDelegate, out intPtr);
			}
			else
			{
				status = GDIPlus.GdipLoadImageFromStream(new ComIStreamWrapper(stream), out intPtr);
			}
			GDIPlus.CheckStatus(status);
			return intPtr;
		}

		public RectangleF GetBounds(ref GraphicsUnit pageUnit)
		{
			RectangleF rectangleF;
			Status status = GDIPlus.GdipGetImageBounds(this.nativeObject, out rectangleF, ref pageUnit);
			GDIPlus.CheckStatus(status);
			return rectangleF;
		}

		public EncoderParameters GetEncoderParameterList(Guid encoder)
		{
			uint num;
			Status status = GDIPlus.GdipGetEncoderParameterListSize(this.nativeObject, ref encoder, out num);
			GDIPlus.CheckStatus(status);
			IntPtr intPtr = Marshal.AllocHGlobal((int)num);
			EncoderParameters encoderParameters;
			try
			{
				status = GDIPlus.GdipGetEncoderParameterList(this.nativeObject, ref encoder, num, intPtr);
				encoderParameters = EncoderParameters.FromNativePtr(intPtr);
				GDIPlus.CheckStatus(status);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return encoderParameters;
		}

		public int GetFrameCount(FrameDimension dimension)
		{
			Guid guid = dimension.Guid;
			uint num;
			Status status = GDIPlus.GdipImageGetFrameCount(this.nativeObject, ref guid, out num);
			GDIPlus.CheckStatus(status);
			return (int)num;
		}

		public PropertyItem GetPropertyItem(int propid)
		{
			PropertyItem propertyItem = new PropertyItem();
			GdipPropertyItem gdipPropertyItem = default(GdipPropertyItem);
			int num;
			Status status = GDIPlus.GdipGetPropertyItemSize(this.nativeObject, propid, out num);
			GDIPlus.CheckStatus(status);
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			try
			{
				status = GDIPlus.GdipGetPropertyItem(this.nativeObject, propid, num, intPtr);
				GDIPlus.CheckStatus(status);
				gdipPropertyItem = (GdipPropertyItem)Marshal.PtrToStructure(intPtr, typeof(GdipPropertyItem));
				GdipPropertyItem.MarshalTo(gdipPropertyItem, propertyItem);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return propertyItem;
		}

		public Image GetThumbnailImage(int thumbWidth, int thumbHeight, Image.GetThumbnailImageAbort callback, IntPtr callbackData)
		{
			if (thumbWidth <= 0 || thumbHeight <= 0)
			{
				throw new OutOfMemoryException("Invalid thumbnail size");
			}
			Image image = new Bitmap(thumbWidth, thumbHeight);
			using (Graphics graphics = Graphics.FromImage(image))
			{
				Status status = GDIPlus.GdipDrawImageRectRectI(graphics.nativeObject, this.nativeObject, 0, 0, thumbWidth, thumbHeight, 0, 0, this.Width, this.Height, GraphicsUnit.Pixel, IntPtr.Zero, null, IntPtr.Zero);
				GDIPlus.CheckStatus(status);
			}
			return image;
		}

		public void RemovePropertyItem(int propid)
		{
			Status status = GDIPlus.GdipRemovePropertyItem(this.nativeObject, propid);
			GDIPlus.CheckStatus(status);
		}

		public void RotateFlip(RotateFlipType rotateFlipType)
		{
			Status status = GDIPlus.GdipImageRotateFlip(this.nativeObject, rotateFlipType);
			GDIPlus.CheckStatus(status);
		}

		internal ImageCodecInfo findEncoderForFormat(ImageFormat format)
		{
			ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
			ImageCodecInfo imageCodecInfo = null;
			if (format.Guid.Equals(ImageFormat.MemoryBmp.Guid))
			{
				format = ImageFormat.Png;
			}
			for (int i = 0; i < imageEncoders.Length; i++)
			{
				if (imageEncoders[i].FormatID.Equals(format.Guid))
				{
					imageCodecInfo = imageEncoders[i];
					break;
				}
			}
			return imageCodecInfo;
		}

		public void Save(string filename)
		{
			this.Save(filename, this.RawFormat);
		}

		public void Save(string filename, ImageFormat format)
		{
			ImageCodecInfo imageCodecInfo = this.findEncoderForFormat(format);
			if (imageCodecInfo == null)
			{
				imageCodecInfo = this.findEncoderForFormat(this.RawFormat);
				if (imageCodecInfo == null)
				{
					string text = Locale.GetText("No codec available for saving format '{0}'.", new object[] { format.Guid });
					throw new ArgumentException(text, "format");
				}
			}
			this.Save(filename, imageCodecInfo, null);
		}

		public void Save(string filename, ImageCodecInfo encoder, EncoderParameters encoderParams)
		{
			Guid clsid = encoder.Clsid;
			Status status;
			if (encoderParams == null)
			{
				status = GDIPlus.GdipSaveImageToFile(this.nativeObject, filename, ref clsid, IntPtr.Zero);
			}
			else
			{
				IntPtr intPtr = encoderParams.ToNativePtr();
				status = GDIPlus.GdipSaveImageToFile(this.nativeObject, filename, ref clsid, intPtr);
				Marshal.FreeHGlobal(intPtr);
			}
			GDIPlus.CheckStatus(status);
		}

		public void Save(Stream stream, ImageFormat format)
		{
			ImageCodecInfo imageCodecInfo = this.findEncoderForFormat(format);
			if (imageCodecInfo == null)
			{
				throw new ArgumentException("No codec available for format:" + format.Guid);
			}
			this.Save(stream, imageCodecInfo, null);
		}

		public void Save(Stream stream, ImageCodecInfo encoder, EncoderParameters encoderParams)
		{
			Guid clsid = encoder.Clsid;
			IntPtr intPtr;
			if (encoderParams == null)
			{
				intPtr = IntPtr.Zero;
			}
			else
			{
				intPtr = encoderParams.ToNativePtr();
			}
			Status status;
			try
			{
				if (GDIPlus.RunningOnUnix())
				{
					GDIPlus.GdiPlusStreamHelper gdiPlusStreamHelper = new GDIPlus.GdiPlusStreamHelper(stream, false);
					status = GDIPlus.GdipSaveImageToDelegate_linux(this.nativeObject, gdiPlusStreamHelper.GetBytesDelegate, gdiPlusStreamHelper.PutBytesDelegate, gdiPlusStreamHelper.SeekDelegate, gdiPlusStreamHelper.CloseDelegate, gdiPlusStreamHelper.SizeDelegate, ref clsid, intPtr);
				}
				else
				{
					status = GDIPlus.GdipSaveImageToStream(new HandleRef(this, this.nativeObject), new ComIStreamWrapper(stream), ref clsid, new HandleRef(encoderParams, intPtr));
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			GDIPlus.CheckStatus(status);
		}

		public void SaveAdd(EncoderParameters encoderParams)
		{
			IntPtr intPtr = encoderParams.ToNativePtr();
			Status status = GDIPlus.GdipSaveAdd(this.nativeObject, intPtr);
			Marshal.FreeHGlobal(intPtr);
			GDIPlus.CheckStatus(status);
		}

		public void SaveAdd(Image image, EncoderParameters encoderParams)
		{
			IntPtr intPtr = encoderParams.ToNativePtr();
			Status status = GDIPlus.GdipSaveAddImage(this.nativeObject, image.NativeObject, intPtr);
			Marshal.FreeHGlobal(intPtr);
			GDIPlus.CheckStatus(status);
		}

		public int SelectActiveFrame(FrameDimension dimension, int frameIndex)
		{
			Guid guid = dimension.Guid;
			Status status = GDIPlus.GdipImageSelectActiveFrame(this.nativeObject, ref guid, frameIndex);
			GDIPlus.CheckStatus(status);
			return frameIndex;
		}

		public void SetPropertyItem(PropertyItem propitem)
		{
			throw new NotImplementedException();
		}

		[Browsable(false)]
		public int Flags
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipGetImageFlags(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
		}

		[Browsable(false)]
		public Guid[] FrameDimensionsList
		{
			get
			{
				uint num;
				Status status = GDIPlus.GdipImageGetFrameDimensionsCount(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				Guid[] array = new Guid[num];
				status = GDIPlus.GdipImageGetFrameDimensionsList(this.nativeObject, array, num);
				GDIPlus.CheckStatus(status);
				return array;
			}
		}

		[Browsable(false)]
		[DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Height
		{
			get
			{
				uint num;
				Status status = GDIPlus.GdipGetImageHeight(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return (int)num;
			}
		}

		public float HorizontalResolution
		{
			get
			{
				float num;
				Status status = GDIPlus.GdipGetImageHorizontalResolution(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
		}

		[Browsable(false)]
		public ColorPalette Palette
		{
			get
			{
				return this.retrieveGDIPalette();
			}
			set
			{
				this.storeGDIPalette(value);
			}
		}

		internal ColorPalette retrieveGDIPalette()
		{
			ColorPalette colorPalette = new ColorPalette();
			int num;
			Status status = GDIPlus.GdipGetImagePaletteSize(this.nativeObject, out num);
			GDIPlus.CheckStatus(status);
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			ColorPalette colorPalette2;
			try
			{
				status = GDIPlus.GdipGetImagePalette(this.nativeObject, intPtr, num);
				GDIPlus.CheckStatus(status);
				colorPalette.setFromGDIPalette(intPtr);
				colorPalette2 = colorPalette;
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return colorPalette2;
		}

		internal void storeGDIPalette(ColorPalette palette)
		{
			if (palette == null)
			{
				throw new ArgumentNullException("palette");
			}
			IntPtr gdipalette = palette.getGDIPalette();
			if (gdipalette == IntPtr.Zero)
			{
				return;
			}
			try
			{
				Status status = GDIPlus.GdipSetImagePalette(this.nativeObject, gdipalette);
				GDIPlus.CheckStatus(status);
			}
			finally
			{
				Marshal.FreeHGlobal(gdipalette);
			}
		}

		public SizeF PhysicalDimension
		{
			get
			{
				float num;
				float num2;
				Status status = GDIPlus.GdipGetImageDimension(this.nativeObject, out num, out num2);
				GDIPlus.CheckStatus(status);
				return new SizeF(num, num2);
			}
		}

		public PixelFormat PixelFormat
		{
			get
			{
				PixelFormat pixelFormat;
				Status status = GDIPlus.GdipGetImagePixelFormat(this.nativeObject, out pixelFormat);
				GDIPlus.CheckStatus(status);
				return pixelFormat;
			}
		}

		[Browsable(false)]
		public int[] PropertyIdList
		{
			get
			{
				uint num;
				Status status = GDIPlus.GdipGetPropertyCount(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				int[] array = new int[num];
				status = GDIPlus.GdipGetPropertyIdList(this.nativeObject, num, array);
				GDIPlus.CheckStatus(status);
				return array;
			}
		}

		[Browsable(false)]
		public PropertyItem[] PropertyItems
		{
			get
			{
				GdipPropertyItem gdipPropertyItem = default(GdipPropertyItem);
				int num;
				int num2;
				Status status = GDIPlus.GdipGetPropertySize(this.nativeObject, out num, out num2);
				GDIPlus.CheckStatus(status);
				PropertyItem[] array = new PropertyItem[num2];
				if (num2 == 0)
				{
					return array;
				}
				IntPtr intPtr = Marshal.AllocHGlobal(num * num2);
				try
				{
					status = GDIPlus.GdipGetAllPropertyItems(this.nativeObject, num, num2, intPtr);
					GDIPlus.CheckStatus(status);
					int num3 = Marshal.SizeOf(gdipPropertyItem);
					IntPtr intPtr2 = intPtr;
					int i = 0;
					while (i < num2)
					{
						gdipPropertyItem = (GdipPropertyItem)Marshal.PtrToStructure(intPtr2, typeof(GdipPropertyItem));
						array[i] = new PropertyItem();
						GdipPropertyItem.MarshalTo(gdipPropertyItem, array[i]);
						i++;
						intPtr2 = new IntPtr(intPtr2.ToInt64() + (long)num3);
					}
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
				return array;
			}
		}

		public ImageFormat RawFormat
		{
			get
			{
				Guid guid;
				Status status = GDIPlus.GdipGetImageRawFormat(this.nativeObject, out guid);
				GDIPlus.CheckStatus(status);
				return new ImageFormat(guid);
			}
		}

		public Size Size
		{
			get
			{
				return new Size(this.Width, this.Height);
			}
		}

		[TypeConverter(typeof(StringConverter))]
		[Bindable(true)]
		[DefaultValue(null)]
		[Localizable(false)]
		public object Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				this.tag = value;
			}
		}

		public float VerticalResolution
		{
			get
			{
				float num;
				Status status = GDIPlus.GdipGetImageVerticalResolution(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
		}

		[DefaultValue(false)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Width
		{
			get
			{
				uint num;
				Status status = GDIPlus.GdipGetImageWidth(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return (int)num;
			}
		}

		internal IntPtr NativeObject
		{
			get
			{
				return this.nativeObject;
			}
			set
			{
				this.nativeObject = value;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Image()
		{
			this.Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (GDIPlus.GdiPlusToken != 0UL && this.nativeObject != IntPtr.Zero)
			{
				Status status = GDIPlus.GdipDisposeImage(this.nativeObject);
				if (this.stream != null)
				{
					this.stream.Close();
					this.stream = null;
				}
				this.nativeObject = IntPtr.Zero;
				GDIPlus.CheckStatus(status);
			}
		}

		public object Clone()
		{
			if (GDIPlus.RunningOnWindows() && this.stream != null)
			{
				return this.CloneFromStream();
			}
			IntPtr zero = IntPtr.Zero;
			Status status = GDIPlus.GdipCloneImage(this.NativeObject, out zero);
			GDIPlus.CheckStatus(status);
			if (this is Bitmap)
			{
				return new Bitmap(zero);
			}
			return new Metafile(zero);
		}

		private object CloneFromStream()
		{
			byte[] array = new byte[this.stream.Length];
			MemoryStream memoryStream = new MemoryStream(array);
			int num = ((this.stream.Length >= 4096L) ? 4096 : ((int)this.stream.Length));
			byte[] array2 = new byte[num];
			this.stream.Position = 0L;
			do
			{
				num = this.stream.Read(array2, 0, num);
				memoryStream.Write(array2, 0, num);
			}
			while (num == 4096);
			IntPtr intPtr = IntPtr.Zero;
			intPtr = Image.InitFromStream(memoryStream);
			if (this is Bitmap)
			{
				return new Bitmap(intPtr, memoryStream);
			}
			return new Metafile(intPtr, memoryStream);
		}

		private object tag;

		internal IntPtr nativeObject = IntPtr.Zero;

		internal Stream stream;

		public delegate bool GetThumbnailImageAbort();
	}
}
