using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Drawing
{
	[ImmutableObject(true)]
	[TypeConverter(typeof(ImageConverter))]
	[Editor("System.Drawing.Design.ImageEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[ComVisible(true)]
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
			GDIPlus.CheckStatus(GDIPlus.GdipCreateBitmapFromHBITMAP(hbitmap, hpalette, out intPtr));
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

		internal static Image CreateImageObject(IntPtr nativeImage)
		{
			return Image.CreateFromHandle(nativeImage);
		}

		internal static Image CreateFromHandle(IntPtr handle)
		{
			ImageType imageType;
			GDIPlus.CheckStatus(GDIPlus.GdipGetImageType(handle, out imageType));
			if (imageType == ImageType.Bitmap)
			{
				return new Bitmap(handle);
			}
			if (imageType != ImageType.Metafile)
			{
				throw new NotSupportedException(Locale.GetText("Unknown image type."));
			}
			return new Metafile(handle);
		}

		public static int GetPixelFormatSize(PixelFormat pixfmt)
		{
			int num = 0;
			if (pixfmt <= PixelFormat.Format8bppIndexed)
			{
				if (pixfmt <= PixelFormat.Format32bppRgb)
				{
					if (pixfmt - PixelFormat.Format16bppRgb555 > 1)
					{
						if (pixfmt == PixelFormat.Format24bppRgb)
						{
							return 24;
						}
						if (pixfmt != PixelFormat.Format32bppRgb)
						{
							return num;
						}
						goto IL_00A7;
					}
				}
				else
				{
					if (pixfmt == PixelFormat.Format1bppIndexed)
					{
						return 1;
					}
					if (pixfmt == PixelFormat.Format4bppIndexed)
					{
						return 4;
					}
					if (pixfmt != PixelFormat.Format8bppIndexed)
					{
						return num;
					}
					return 8;
				}
			}
			else
			{
				if (pixfmt > PixelFormat.Format16bppGrayScale)
				{
					if (pixfmt <= PixelFormat.Format64bppPArgb)
					{
						if (pixfmt == PixelFormat.Format48bppRgb)
						{
							return 48;
						}
						if (pixfmt != PixelFormat.Format64bppPArgb)
						{
							return num;
						}
					}
					else
					{
						if (pixfmt == PixelFormat.Format32bppArgb)
						{
							goto IL_00A7;
						}
						if (pixfmt != PixelFormat.Format64bppArgb)
						{
							return num;
						}
					}
					return 64;
				}
				if (pixfmt != PixelFormat.Format16bppArgb1555)
				{
					if (pixfmt == PixelFormat.Format32bppPArgb)
					{
						goto IL_00A7;
					}
					if (pixfmt != PixelFormat.Format16bppGrayScale)
					{
						return num;
					}
				}
			}
			return 16;
			IL_00A7:
			num = 32;
			return num;
		}

		public static bool IsAlphaPixelFormat(PixelFormat pixfmt)
		{
			bool flag = false;
			if (pixfmt > PixelFormat.Format8bppIndexed)
			{
				if (pixfmt <= PixelFormat.Format16bppGrayScale)
				{
					if (pixfmt != PixelFormat.Format16bppArgb1555 && pixfmt != PixelFormat.Format32bppPArgb)
					{
						if (pixfmt != PixelFormat.Format16bppGrayScale)
						{
							return flag;
						}
						goto IL_0098;
					}
				}
				else if (pixfmt <= PixelFormat.Format64bppPArgb)
				{
					if (pixfmt == PixelFormat.Format48bppRgb)
					{
						goto IL_0098;
					}
					if (pixfmt != PixelFormat.Format64bppPArgb)
					{
						return flag;
					}
				}
				else if (pixfmt != PixelFormat.Format32bppArgb && pixfmt != PixelFormat.Format64bppArgb)
				{
					return flag;
				}
				return true;
			}
			if (pixfmt <= PixelFormat.Format32bppRgb)
			{
				if (pixfmt - PixelFormat.Format16bppRgb555 > 1 && pixfmt != PixelFormat.Format24bppRgb && pixfmt != PixelFormat.Format32bppRgb)
				{
					return flag;
				}
			}
			else if (pixfmt != PixelFormat.Format1bppIndexed && pixfmt != PixelFormat.Format4bppIndexed && pixfmt != PixelFormat.Format8bppIndexed)
			{
				return flag;
			}
			IL_0098:
			flag = false;
			return flag;
		}

		public static bool IsCanonicalPixelFormat(PixelFormat pixfmt)
		{
			return (pixfmt & PixelFormat.Canonical) > PixelFormat.Undefined;
		}

		public static bool IsExtendedPixelFormat(PixelFormat pixfmt)
		{
			return (pixfmt & PixelFormat.Extended) > PixelFormat.Undefined;
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
			if (status != Status.Ok)
			{
				return IntPtr.Zero;
			}
			return intPtr;
		}

		public RectangleF GetBounds(ref GraphicsUnit pageUnit)
		{
			RectangleF rectangleF;
			GDIPlus.CheckStatus(GDIPlus.GdipGetImageBounds(this.nativeObject, out rectangleF, ref pageUnit));
			return rectangleF;
		}

		public EncoderParameters GetEncoderParameterList(Guid encoder)
		{
			uint num;
			GDIPlus.CheckStatus(GDIPlus.GdipGetEncoderParameterListSize(this.nativeObject, ref encoder, out num));
			IntPtr intPtr = Marshal.AllocHGlobal((int)num);
			EncoderParameters encoderParameters;
			try
			{
				Status status = GDIPlus.GdipGetEncoderParameterList(this.nativeObject, ref encoder, num, intPtr);
				encoderParameters = EncoderParameters.ConvertFromMemory(intPtr);
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
			GDIPlus.CheckStatus(GDIPlus.GdipImageGetFrameCount(this.nativeObject, ref guid, out num));
			return (int)num;
		}

		public PropertyItem GetPropertyItem(int propid)
		{
			PropertyItem propertyItem = new PropertyItem();
			int num;
			GDIPlus.CheckStatus(GDIPlus.GdipGetPropertyItemSize(this.nativeObject, propid, out num));
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			try
			{
				GDIPlus.CheckStatus(GDIPlus.GdipGetPropertyItem(this.nativeObject, propid, num, intPtr));
				GdipPropertyItem.MarshalTo((GdipPropertyItem)Marshal.PtrToStructure(intPtr, typeof(GdipPropertyItem)), propertyItem);
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
				GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRectI(graphics.nativeObject, this.nativeObject, 0, 0, thumbWidth, thumbHeight, 0, 0, this.Width, this.Height, GraphicsUnit.Pixel, IntPtr.Zero, null, IntPtr.Zero));
			}
			return image;
		}

		public void RemovePropertyItem(int propid)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipRemovePropertyItem(this.nativeObject, propid));
		}

		public void RotateFlip(RotateFlipType rotateFlipType)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipImageRotateFlip(this.nativeObject, rotateFlipType));
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
					throw new ArgumentException(Locale.GetText("No codec available for saving format '{0}'.", new object[] { format.Guid }), "format");
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
				IntPtr intPtr = encoderParams.ConvertToMemory();
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
				throw new ArgumentException("No codec available for format:" + format.Guid.ToString());
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
				intPtr = encoderParams.ConvertToMemory();
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
			IntPtr intPtr = encoderParams.ConvertToMemory();
			Status status = GDIPlus.GdipSaveAdd(this.nativeObject, intPtr);
			Marshal.FreeHGlobal(intPtr);
			GDIPlus.CheckStatus(status);
		}

		public void SaveAdd(Image image, EncoderParameters encoderParams)
		{
			IntPtr intPtr = encoderParams.ConvertToMemory();
			Status status = GDIPlus.GdipSaveAddImage(this.nativeObject, image.NativeObject, intPtr);
			Marshal.FreeHGlobal(intPtr);
			GDIPlus.CheckStatus(status);
		}

		public int SelectActiveFrame(FrameDimension dimension, int frameIndex)
		{
			Guid guid = dimension.Guid;
			GDIPlus.CheckStatus(GDIPlus.GdipImageSelectActiveFrame(this.nativeObject, ref guid, frameIndex));
			return frameIndex;
		}

		public unsafe void SetPropertyItem(PropertyItem propitem)
		{
			if (propitem == null)
			{
				throw new ArgumentNullException("propitem");
			}
			int num = Marshal.SizeOf<byte>(propitem.Value[0]) * propitem.Value.Length;
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			try
			{
				GdipPropertyItem gdipPropertyItem = default(GdipPropertyItem);
				gdipPropertyItem.id = propitem.Id;
				gdipPropertyItem.len = propitem.Len;
				gdipPropertyItem.type = propitem.Type;
				Marshal.Copy(propitem.Value, 0, intPtr, num);
				gdipPropertyItem.value = intPtr;
				GDIPlus.CheckStatus(GDIPlus.GdipSetPropertyItem(this.nativeObject, &gdipPropertyItem));
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}

		[Browsable(false)]
		public int Flags
		{
			get
			{
				int num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetImageFlags(this.nativeObject, out num));
				return num;
			}
		}

		[Browsable(false)]
		public Guid[] FrameDimensionsList
		{
			get
			{
				uint num;
				GDIPlus.CheckStatus(GDIPlus.GdipImageGetFrameDimensionsCount(this.nativeObject, out num));
				Guid[] array = new Guid[num];
				GDIPlus.CheckStatus(GDIPlus.GdipImageGetFrameDimensionsList(this.nativeObject, array, num));
				return array;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[DefaultValue(false)]
		public int Height
		{
			get
			{
				uint num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetImageHeight(this.nativeObject, out num));
				return (int)num;
			}
		}

		public float HorizontalResolution
		{
			get
			{
				float num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetImageHorizontalResolution(this.nativeObject, out num));
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
			GDIPlus.CheckStatus(GDIPlus.GdipGetImagePaletteSize(this.nativeObject, out num));
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			ColorPalette colorPalette2;
			try
			{
				GDIPlus.CheckStatus(GDIPlus.GdipGetImagePalette(this.nativeObject, intPtr, num));
				colorPalette.ConvertFromMemory(intPtr);
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
			IntPtr intPtr = palette.ConvertToMemory();
			if (intPtr == IntPtr.Zero)
			{
				return;
			}
			try
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetImagePalette(this.nativeObject, intPtr));
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}

		public SizeF PhysicalDimension
		{
			get
			{
				float num;
				float num2;
				GDIPlus.CheckStatus(GDIPlus.GdipGetImageDimension(this.nativeObject, out num, out num2));
				return new SizeF(num, num2);
			}
		}

		public PixelFormat PixelFormat
		{
			get
			{
				PixelFormat pixelFormat;
				GDIPlus.CheckStatus(GDIPlus.GdipGetImagePixelFormat(this.nativeObject, out pixelFormat));
				return pixelFormat;
			}
		}

		[Browsable(false)]
		public int[] PropertyIdList
		{
			get
			{
				uint num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPropertyCount(this.nativeObject, out num));
				int[] array = new int[num];
				GDIPlus.CheckStatus(GDIPlus.GdipGetPropertyIdList(this.nativeObject, num, array));
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
				GDIPlus.CheckStatus(GDIPlus.GdipGetPropertySize(this.nativeObject, out num, out num2));
				PropertyItem[] array = new PropertyItem[num2];
				if (num2 == 0)
				{
					return array;
				}
				IntPtr intPtr = Marshal.AllocHGlobal(num * num2);
				try
				{
					GDIPlus.CheckStatus(GDIPlus.GdipGetAllPropertyItems(this.nativeObject, num, num2, intPtr));
					int num3 = Marshal.SizeOf<GdipPropertyItem>(gdipPropertyItem);
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
				GDIPlus.CheckStatus(GDIPlus.GdipGetImageRawFormat(this.nativeObject, out guid));
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

		[DefaultValue(null)]
		[Localizable(false)]
		[Bindable(true)]
		[TypeConverter(typeof(StringConverter))]
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
				GDIPlus.CheckStatus(GDIPlus.GdipGetImageVerticalResolution(this.nativeObject, out num));
				return num;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(false)]
		public int Width
		{
			get
			{
				uint num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetImageWidth(this.nativeObject, out num));
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

		internal IntPtr nativeImage
		{
			get
			{
				return this.nativeObject;
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
					this.stream.Dispose();
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
			GDIPlus.CheckStatus(GDIPlus.GdipCloneImage(this.NativeObject, out zero));
			if (this is Bitmap)
			{
				return new Bitmap(zero);
			}
			return new Metafile(zero);
		}

		private object CloneFromStream()
		{
			MemoryStream memoryStream = new MemoryStream(new byte[this.stream.Length]);
			int num = ((this.stream.Length < 4096L) ? ((int)this.stream.Length) : 4096);
			byte[] array = new byte[num];
			this.stream.Position = 0L;
			do
			{
				num = this.stream.Read(array, 0, num);
				memoryStream.Write(array, 0, num);
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
