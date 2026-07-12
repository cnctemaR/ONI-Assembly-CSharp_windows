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
	[ComVisible(true)]
	[Editor("System.Drawing.Design.BitmapEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[Serializable]
	public sealed class Bitmap : Image
	{
		private Bitmap()
		{
		}

		internal Bitmap(IntPtr ptr)
		{
			this.nativeObject = ptr;
		}

		internal Bitmap(IntPtr ptr, Stream stream)
		{
			if (GDIPlus.RunningOnWindows())
			{
				this.stream = stream;
			}
			this.nativeObject = ptr;
		}

		public Bitmap(int width, int height)
			: this(width, height, PixelFormat.Format32bppArgb)
		{
		}

		public Bitmap(int width, int height, Graphics g)
		{
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateBitmapFromGraphics(width, height, g.nativeObject, out intPtr));
			this.nativeObject = intPtr;
		}

		public Bitmap(int width, int height, PixelFormat format)
		{
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateBitmapFromScan0(width, height, 0, format, IntPtr.Zero, out intPtr));
			this.nativeObject = intPtr;
		}

		public Bitmap(Image original)
			: this(original, original.Width, original.Height)
		{
		}

		public Bitmap(Stream stream)
			: this(stream, false)
		{
		}

		public Bitmap(string filename)
			: this(filename, false)
		{
		}

		public Bitmap(Image original, Size newSize)
			: this(original, newSize.Width, newSize.Height)
		{
		}

		public Bitmap(Stream stream, bool useIcm)
		{
			this.nativeObject = Image.InitFromStream(stream);
		}

		public Bitmap(string filename, bool useIcm)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			IntPtr intPtr;
			Status status;
			if (useIcm)
			{
				status = GDIPlus.GdipCreateBitmapFromFileICM(filename, out intPtr);
			}
			else
			{
				status = GDIPlus.GdipCreateBitmapFromFile(filename, out intPtr);
			}
			GDIPlus.CheckStatus(status);
			this.nativeObject = intPtr;
		}

		public Bitmap(Type type, string resource)
		{
			if (resource == null)
			{
				throw new ArgumentException("resource");
			}
			if (type == null)
			{
				throw new NullReferenceException();
			}
			Stream manifestResourceStream = type.GetTypeInfo().Assembly.GetManifestResourceStream(type, resource);
			if (manifestResourceStream == null)
			{
				throw new FileNotFoundException(Locale.GetText("Resource '{0}' was not found.", new object[] { resource }));
			}
			this.nativeObject = Image.InitFromStream(manifestResourceStream);
			if (GDIPlus.RunningOnWindows())
			{
				this.stream = manifestResourceStream;
			}
		}

		public Bitmap(Image original, int width, int height)
			: this(width, height, PixelFormat.Format32bppArgb)
		{
			Graphics graphics = Graphics.FromImage(this);
			graphics.DrawImage(original, 0, 0, width, height);
			graphics.Dispose();
		}

		public Bitmap(int width, int height, int stride, PixelFormat format, IntPtr scan0)
		{
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateBitmapFromScan0(width, height, stride, format, scan0, out intPtr));
			this.nativeObject = intPtr;
		}

		private Bitmap(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public Color GetPixel(int x, int y)
		{
			int num;
			GDIPlus.CheckStatus(GDIPlus.GdipBitmapGetPixel(this.nativeObject, x, y, out num));
			return Color.FromArgb(num);
		}

		public void SetPixel(int x, int y, Color color)
		{
			Status status = GDIPlus.GdipBitmapSetPixel(this.nativeObject, x, y, color.ToArgb());
			if (status == Status.InvalidParameter && (base.PixelFormat & PixelFormat.Indexed) != PixelFormat.Undefined)
			{
				throw new InvalidOperationException(Locale.GetText("SetPixel cannot be called on indexed bitmaps."));
			}
			GDIPlus.CheckStatus(status);
		}

		public Bitmap Clone(Rectangle rect, PixelFormat format)
		{
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipCloneBitmapAreaI(rect.X, rect.Y, rect.Width, rect.Height, format, this.nativeObject, out intPtr));
			return new Bitmap(intPtr);
		}

		public Bitmap Clone(RectangleF rect, PixelFormat format)
		{
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipCloneBitmapArea(rect.X, rect.Y, rect.Width, rect.Height, format, this.nativeObject, out intPtr));
			return new Bitmap(intPtr);
		}

		public static Bitmap FromHicon(IntPtr hicon)
		{
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateBitmapFromHICON(hicon, out intPtr));
			return new Bitmap(intPtr);
		}

		public static Bitmap FromResource(IntPtr hinstance, string bitmapName)
		{
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateBitmapFromResource(hinstance, bitmapName, out intPtr));
			return new Bitmap(intPtr);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IntPtr GetHbitmap()
		{
			return this.GetHbitmap(Color.Gray);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IntPtr GetHbitmap(Color background)
		{
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateHBITMAPFromBitmap(this.nativeObject, out intPtr, background.ToArgb()));
			return intPtr;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IntPtr GetHicon()
		{
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateHICONFromBitmap(this.nativeObject, out intPtr));
			return intPtr;
		}

		public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format)
		{
			BitmapData bitmapData = new BitmapData();
			return this.LockBits(rect, flags, format, bitmapData);
		}

		public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format, BitmapData bitmapData)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipBitmapLockBits(this.nativeObject, ref rect, flags, format, bitmapData));
			return bitmapData;
		}

		public void MakeTransparent()
		{
			Color pixel = this.GetPixel(0, 0);
			this.MakeTransparent(pixel);
		}

		public void MakeTransparent(Color transparentColor)
		{
			Bitmap bitmap = new Bitmap(base.Width, base.Height, PixelFormat.Format32bppArgb);
			Graphics graphics = Graphics.FromImage(bitmap);
			Rectangle rectangle = new Rectangle(0, 0, base.Width, base.Height);
			ImageAttributes imageAttributes = new ImageAttributes();
			imageAttributes.SetColorKey(transparentColor, transparentColor);
			graphics.DrawImage(this, rectangle, 0, 0, base.Width, base.Height, GraphicsUnit.Pixel, imageAttributes);
			IntPtr nativeObject = this.nativeObject;
			this.nativeObject = bitmap.nativeObject;
			bitmap.nativeObject = nativeObject;
			graphics.Dispose();
			bitmap.Dispose();
			imageAttributes.Dispose();
		}

		public void SetResolution(float xDpi, float yDpi)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipBitmapSetResolution(this.nativeObject, xDpi, yDpi));
		}

		public void UnlockBits(BitmapData bitmapdata)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipBitmapUnlockBits(this.nativeObject, bitmapdata));
		}
	}
}
