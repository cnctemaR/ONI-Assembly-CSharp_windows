using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Drawing
{
	[ComVisible(true)]
	[Editor("System.Drawing.Design.BitmapEditor, System.Drawing.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
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
			Status status = GDIPlus.GdipCreateBitmapFromGraphics(width, height, g.nativeObject, out intPtr);
			GDIPlus.CheckStatus(status);
			this.nativeObject = intPtr;
		}

		public Bitmap(int width, int height, PixelFormat format)
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCreateBitmapFromScan0(width, height, 0, format, IntPtr.Zero, out intPtr);
			GDIPlus.CheckStatus(status);
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
			Stream manifestResourceStream = type.Assembly.GetManifestResourceStream(type, resource);
			if (manifestResourceStream == null)
			{
				string text = Locale.GetText("Resource '{0}' was not found.", new object[] { resource });
				throw new FileNotFoundException(text);
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
			Status status = GDIPlus.GdipCreateBitmapFromScan0(width, height, stride, format, scan0, out intPtr);
			GDIPlus.CheckStatus(status);
			this.nativeObject = intPtr;
		}

		private Bitmap(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public Color GetPixel(int x, int y)
		{
			int num;
			Status status = GDIPlus.GdipBitmapGetPixel(this.nativeObject, x, y, out num);
			GDIPlus.CheckStatus(status);
			return Color.FromArgb(num);
		}

		public void SetPixel(int x, int y, Color color)
		{
			Status status = GDIPlus.GdipBitmapSetPixel(this.nativeObject, x, y, color.ToArgb());
			if (status == Status.InvalidParameter && (base.PixelFormat & PixelFormat.Indexed) != PixelFormat.DontCare)
			{
				string text = Locale.GetText("SetPixel cannot be called on indexed bitmaps.");
				throw new InvalidOperationException(text);
			}
			GDIPlus.CheckStatus(status);
		}

		public Bitmap Clone(Rectangle rect, PixelFormat format)
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCloneBitmapAreaI(rect.X, rect.Y, rect.Width, rect.Height, format, this.nativeObject, out intPtr);
			GDIPlus.CheckStatus(status);
			return new Bitmap(intPtr);
		}

		public Bitmap Clone(RectangleF rect, PixelFormat format)
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCloneBitmapArea(rect.X, rect.Y, rect.Width, rect.Height, format, this.nativeObject, out intPtr);
			GDIPlus.CheckStatus(status);
			return new Bitmap(intPtr);
		}

		public static Bitmap FromHicon(IntPtr hicon)
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCreateBitmapFromHICON(hicon, out intPtr);
			GDIPlus.CheckStatus(status);
			return new Bitmap(intPtr);
		}

		public static Bitmap FromResource(IntPtr hinstance, string bitmapName)
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCreateBitmapFromResource(hinstance, bitmapName, out intPtr);
			GDIPlus.CheckStatus(status);
			return new Bitmap(intPtr);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public IntPtr GetHbitmap()
		{
			return this.GetHbitmap(Color.Gray);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public IntPtr GetHbitmap(Color background)
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCreateHBITMAPFromBitmap(this.nativeObject, out intPtr, background.ToArgb());
			GDIPlus.CheckStatus(status);
			return intPtr;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public IntPtr GetHicon()
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCreateHICONFromBitmap(this.nativeObject, out intPtr);
			GDIPlus.CheckStatus(status);
			return intPtr;
		}

		public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format)
		{
			BitmapData bitmapData = new BitmapData();
			return this.LockBits(rect, flags, format, bitmapData);
		}

		public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format, BitmapData bitmapData)
		{
			Status status = GDIPlus.GdipBitmapLockBits(this.nativeObject, ref rect, flags, format, bitmapData);
			GDIPlus.CheckStatus(status);
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
			Status status = GDIPlus.GdipBitmapSetResolution(this.nativeObject, xDpi, yDpi);
			GDIPlus.CheckStatus(status);
		}

		public void UnlockBits(BitmapData bitmapdata)
		{
			Status status = GDIPlus.GdipBitmapUnlockBits(this.nativeObject, bitmapdata);
			GDIPlus.CheckStatus(status);
		}
	}
}
