using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Drawing
{
	public sealed class Graphics : MarshalByRefObject, IDisposable, IDeviceContext
	{
		internal Graphics(IntPtr nativeGraphics)
		{
			this.nativeObject = nativeGraphics;
		}

		~Graphics()
		{
			this.Dispose();
		}

		internal static float systemDpiX
		{
			get
			{
				if (Graphics.defDpiX == 0f)
				{
					Bitmap bitmap = new Bitmap(1, 1);
					Graphics graphics = Graphics.FromImage(bitmap);
					Graphics.defDpiX = graphics.DpiX;
					Graphics.defDpiY = graphics.DpiY;
				}
				return Graphics.defDpiX;
			}
		}

		internal static float systemDpiY
		{
			get
			{
				if (Graphics.defDpiY == 0f)
				{
					Bitmap bitmap = new Bitmap(1, 1);
					Graphics graphics = Graphics.FromImage(bitmap);
					Graphics.defDpiX = graphics.DpiX;
					Graphics.defDpiY = graphics.DpiY;
				}
				return Graphics.defDpiY;
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

		[MonoTODO("Metafiles, both WMF and EMF formats, aren't supported.")]
		public void AddMetafileComment(byte[] data)
		{
			throw new NotImplementedException();
		}

		public GraphicsContainer BeginContainer()
		{
			uint num;
			Status status = GDIPlus.GdipBeginContainer2(this.nativeObject, out num);
			GDIPlus.CheckStatus(status);
			return new GraphicsContainer(num);
		}

		[MonoTODO("The rectangles and unit parameters aren't supported in libgdiplus")]
		public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)
		{
			uint num;
			Status status = GDIPlus.GdipBeginContainerI(this.nativeObject, ref dstrect, ref srcrect, unit, out num);
			GDIPlus.CheckStatus(status);
			return new GraphicsContainer(num);
		}

		[MonoTODO("The rectangles and unit parameters aren't supported in libgdiplus")]
		public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)
		{
			uint num;
			Status status = GDIPlus.GdipBeginContainer(this.nativeObject, ref dstrect, ref srcrect, unit, out num);
			GDIPlus.CheckStatus(status);
			return new GraphicsContainer(num);
		}

		public void Clear(Color color)
		{
			Status status = GDIPlus.GdipGraphicsClear(this.nativeObject, color.ToArgb());
			GDIPlus.CheckStatus(status);
		}

		[MonoLimitation("Works on Win32 and on X11 (but not on Cocoa and Quartz)")]
		public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize)
		{
			this.CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y, blockRegionSize, CopyPixelOperation.SourceCopy);
		}

		[MonoLimitation("Works on Win32 and (for CopyPixelOperation.SourceCopy only) on X11 but not on Cocoa and Quartz")]
		public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
		{
			this.CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y, blockRegionSize, copyPixelOperation);
		}

		[MonoLimitation("Works on Win32 and on X11 (but not on Cocoa and Quartz)")]
		public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize)
		{
			this.CopyFromScreen(sourceX, sourceY, destinationX, destinationY, blockRegionSize, CopyPixelOperation.SourceCopy);
		}

		[MonoLimitation("Works on Win32 and (for CopyPixelOperation.SourceCopy only) on X11 but not on Cocoa and Quartz")]
		public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
		{
			if (!Enum.IsDefined(typeof(CopyPixelOperation), copyPixelOperation))
			{
				throw new InvalidEnumArgumentException(Locale.GetText("Enum argument value '{0}' is not valid for CopyPixelOperation", new object[] { copyPixelOperation }));
			}
			if (GDIPlus.UseX11Drawable)
			{
				this.CopyFromScreenX11(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
			}
			else if (GDIPlus.UseCarbonDrawable)
			{
				this.CopyFromScreenMac(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
			}
			else
			{
				this.CopyFromScreenWin32(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
			}
		}

		private void CopyFromScreenWin32(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
		{
			IntPtr desktopWindow = GDIPlus.GetDesktopWindow();
			IntPtr dc = GDIPlus.GetDC(desktopWindow);
			IntPtr hdc = this.GetHdc();
			GDIPlus.BitBlt(hdc, destinationX, destinationY, blockRegionSize.Width, blockRegionSize.Height, dc, sourceX, sourceY, (int)copyPixelOperation);
			GDIPlus.ReleaseDC(IntPtr.Zero, dc);
			this.ReleaseHdc(hdc);
		}

		private void CopyFromScreenMac(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
		{
			throw new NotImplementedException();
		}

		private void CopyFromScreenX11(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
		{
			int num = -1;
			int num2 = 0;
			if (copyPixelOperation != CopyPixelOperation.SourceCopy)
			{
				throw new NotImplementedException("Operation not implemented under X11");
			}
			if (GDIPlus.Display == IntPtr.Zero)
			{
				GDIPlus.Display = GDIPlus.XOpenDisplay(IntPtr.Zero);
			}
			IntPtr intPtr = GDIPlus.XRootWindow(GDIPlus.Display, 0);
			IntPtr intPtr2 = GDIPlus.XDefaultVisual(GDIPlus.Display, 0);
			XVisualInfo xvisualInfo = default(XVisualInfo);
			xvisualInfo.visualid = GDIPlus.XVisualIDFromVisual(intPtr2);
			IntPtr intPtr3 = GDIPlus.XGetVisualInfo(GDIPlus.Display, 1, ref xvisualInfo, ref num2);
			xvisualInfo = (XVisualInfo)Marshal.PtrToStructure(intPtr3, typeof(XVisualInfo));
			IntPtr intPtr4 = GDIPlus.XGetImage(GDIPlus.Display, intPtr, sourceX, sourceY, blockRegionSize.Width, blockRegionSize.Height, num, 2);
			Bitmap bitmap = new Bitmap(blockRegionSize.Width, blockRegionSize.Height);
			int num3 = (int)xvisualInfo.red_mask;
			int num4 = (int)xvisualInfo.blue_mask;
			int num5 = (int)xvisualInfo.green_mask;
			for (int i = 0; i < blockRegionSize.Height; i++)
			{
				for (int j = 0; j < blockRegionSize.Width; j++)
				{
					int num6 = GDIPlus.XGetPixel(intPtr4, j, i);
					uint depth = xvisualInfo.depth;
					int num7;
					int num8;
					int num9;
					if (depth != 16U)
					{
						if (depth != 24U && depth != 32U)
						{
							string text = Locale.GetText("{0}bbp depth not supported.", new object[] { xvisualInfo.depth });
							throw new NotImplementedException(text);
						}
						num7 = ((num6 & num3) >> 16) & 255;
						num8 = ((num6 & num5) >> 8) & 255;
						num9 = num6 & num4 & 255;
					}
					else
					{
						num7 = ((num6 & num3) >> 8) & 255;
						num8 = ((num6 & num5) >> 3) & 255;
						num9 = ((num6 & num4) << 3) & 255;
					}
					bitmap.SetPixel(j, i, Color.FromArgb(255, num7, num8, num9));
				}
			}
			this.DrawImage(bitmap, destinationX, destinationY);
			bitmap.Dispose();
			GDIPlus.XDestroyImage(intPtr4);
			GDIPlus.XFree(intPtr3);
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				if (GDIPlus.UseCarbonDrawable && this.context.ctx != IntPtr.Zero)
				{
					this.Flush();
					Carbon.CGContextSynchronize(this.context.ctx);
					Carbon.ReleaseContext(this.context.port, this.context.ctx);
				}
				Status status = GDIPlus.GdipDeleteGraphics(this.nativeObject);
				this.nativeObject = IntPtr.Zero;
				GDIPlus.CheckStatus(status);
				this.disposed = true;
			}
			GC.SuppressFinalize(this);
		}

		public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
		{
			this.DrawArc(pen, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, startAngle, sweepAngle);
		}

		public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
		{
			this.DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			Status status = GDIPlus.GdipDrawArc(this.nativeObject, pen.nativeObject, x, y, width, height, startAngle, sweepAngle);
			GDIPlus.CheckStatus(status);
		}

		public void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			Status status = GDIPlus.GdipDrawArcI(this.nativeObject, pen.nativeObject, x, y, width, height, (float)startAngle, (float)sweepAngle);
			GDIPlus.CheckStatus(status);
		}

		public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			Status status = GDIPlus.GdipDrawBezier(this.nativeObject, pen.nativeObject, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
			GDIPlus.CheckStatus(status);
		}

		public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			Status status = GDIPlus.GdipDrawBezierI(this.nativeObject, pen.nativeObject, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
			GDIPlus.CheckStatus(status);
		}

		public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			Status status = GDIPlus.GdipDrawBezier(this.nativeObject, pen.nativeObject, x1, y1, x2, y2, x3, y3, x4, y4);
			GDIPlus.CheckStatus(status);
		}

		public void DrawBeziers(Pen pen, Point[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			int num = points.Length;
			if (num < 4)
			{
				return;
			}
			for (int i = 0; i < num - 1; i += 3)
			{
				Point point = points[i];
				Point point2 = points[i + 1];
				Point point3 = points[i + 2];
				Point point4 = points[i + 3];
				Status status = GDIPlus.GdipDrawBezier(this.nativeObject, pen.nativeObject, (float)point.X, (float)point.Y, (float)point2.X, (float)point2.Y, (float)point3.X, (float)point3.Y, (float)point4.X, (float)point4.Y);
				GDIPlus.CheckStatus(status);
			}
		}

		public void DrawBeziers(Pen pen, PointF[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			int num = points.Length;
			if (num < 4)
			{
				return;
			}
			for (int i = 0; i < num - 1; i += 3)
			{
				PointF pointF = points[i];
				PointF pointF2 = points[i + 1];
				PointF pointF3 = points[i + 2];
				PointF pointF4 = points[i + 3];
				Status status = GDIPlus.GdipDrawBezier(this.nativeObject, pen.nativeObject, pointF.X, pointF.Y, pointF2.X, pointF2.Y, pointF3.X, pointF3.Y, pointF4.X, pointF4.Y);
				GDIPlus.CheckStatus(status);
			}
		}

		public void DrawClosedCurve(Pen pen, PointF[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipDrawClosedCurve(this.nativeObject, pen.nativeObject, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void DrawClosedCurve(Pen pen, Point[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipDrawClosedCurveI(this.nativeObject, pen.nativeObject, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipDrawClosedCurve2I(this.nativeObject, pen.nativeObject, points, points.Length, tension);
			GDIPlus.CheckStatus(status);
		}

		public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipDrawClosedCurve2(this.nativeObject, pen.nativeObject, points, points.Length, tension);
			GDIPlus.CheckStatus(status);
		}

		public void DrawCurve(Pen pen, Point[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipDrawCurveI(this.nativeObject, pen.nativeObject, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void DrawCurve(Pen pen, PointF[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipDrawCurve(this.nativeObject, pen.nativeObject, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void DrawCurve(Pen pen, PointF[] points, float tension)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipDrawCurve2(this.nativeObject, pen.nativeObject, points, points.Length, tension);
			GDIPlus.CheckStatus(status);
		}

		public void DrawCurve(Pen pen, Point[] points, float tension)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipDrawCurve2I(this.nativeObject, pen.nativeObject, points, points.Length, tension);
			GDIPlus.CheckStatus(status);
		}

		public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipDrawCurve3(this.nativeObject, pen.nativeObject, points, points.Length, offset, numberOfSegments, 0.5f);
			GDIPlus.CheckStatus(status);
		}

		public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipDrawCurve3I(this.nativeObject, pen.nativeObject, points, points.Length, offset, numberOfSegments, tension);
			GDIPlus.CheckStatus(status);
		}

		public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipDrawCurve3(this.nativeObject, pen.nativeObject, points, points.Length, offset, numberOfSegments, tension);
			GDIPlus.CheckStatus(status);
		}

		public void DrawEllipse(Pen pen, Rectangle rect)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			this.DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void DrawEllipse(Pen pen, RectangleF rect)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			this.DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void DrawEllipse(Pen pen, int x, int y, int width, int height)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			Status status = GDIPlus.GdipDrawEllipseI(this.nativeObject, pen.nativeObject, x, y, width, height);
			GDIPlus.CheckStatus(status);
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			Status status = GDIPlus.GdipDrawEllipse(this.nativeObject, pen.nativeObject, x, y, width, height);
			GDIPlus.CheckStatus(status);
		}

		public void DrawIcon(Icon icon, Rectangle targetRect)
		{
			if (icon == null)
			{
				throw new ArgumentNullException("icon");
			}
			this.DrawImage(icon.GetInternalBitmap(), targetRect);
		}

		public void DrawIcon(Icon icon, int x, int y)
		{
			if (icon == null)
			{
				throw new ArgumentNullException("icon");
			}
			this.DrawImage(icon.GetInternalBitmap(), x, y);
		}

		public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
		{
			if (icon == null)
			{
				throw new ArgumentNullException("icon");
			}
			this.DrawImageUnscaled(icon.GetInternalBitmap(), targetRect);
		}

		public void DrawImage(Image image, RectangleF rect)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImageRect(this.nativeObject, image.NativeObject, rect.X, rect.Y, rect.Width, rect.Height);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, PointF point)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImage(this.nativeObject, image.NativeObject, point.X, point.Y);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, Point[] destPoints)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			Status status = GDIPlus.GdipDrawImagePointsI(this.nativeObject, image.NativeObject, destPoints, destPoints.Length);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, Point point)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			this.DrawImage(image, point.X, point.Y);
		}

		public void DrawImage(Image image, Rectangle rect)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			this.DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void DrawImage(Image image, PointF[] destPoints)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			Status status = GDIPlus.GdipDrawImagePoints(this.nativeObject, image.NativeObject, destPoints, destPoints.Length);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, int x, int y)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImageI(this.nativeObject, image.NativeObject, x, y);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, float x, float y)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImage(this.nativeObject, image.NativeObject, x, y);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImageRectRectI(this.nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImageRectRect(this.nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			Status status = GDIPlus.GdipDrawImagePointsRectI(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			Status status = GDIPlus.GdipDrawImagePointsRect(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			Status status = GDIPlus.GdipDrawImagePointsRectI(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, (imageAttr == null) ? IntPtr.Zero : imageAttr.NativeObject, null, IntPtr.Zero);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, float x, float y, float width, float height)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImageRect(this.nativeObject, image.NativeObject, x, y, width, height);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			Status status = GDIPlus.GdipDrawImagePointsRect(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, (imageAttr == null) ? IntPtr.Zero : imageAttr.NativeObject, null, IntPtr.Zero);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImagePointRectI(this.nativeObject, image.NativeObject, x, y, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, int x, int y, int width, int height)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImageRectI(this.nativeObject, image.nativeObject, x, y, width, height);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImagePointRect(this.nativeObject, image.nativeObject, x, y, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			Status status = GDIPlus.GdipDrawImagePointsRect(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, (imageAttr == null) ? IntPtr.Zero : imageAttr.NativeObject, callback, IntPtr.Zero);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			Status status = GDIPlus.GdipDrawImagePointsRectI(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, (imageAttr == null) ? IntPtr.Zero : imageAttr.NativeObject, callback, IntPtr.Zero);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback, int callbackData)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			Status status = GDIPlus.GdipDrawImagePointsRectI(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, (imageAttr == null) ? IntPtr.Zero : imageAttr.NativeObject, callback, (IntPtr)callbackData);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImageRectRect(this.nativeObject, image.NativeObject, (float)destRect.X, (float)destRect.Y, (float)destRect.Width, (float)destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, IntPtr.Zero, null, IntPtr.Zero);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback, int callbackData)
		{
			Status status = GDIPlus.GdipDrawImagePointsRect(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, (imageAttr == null) ? IntPtr.Zero : imageAttr.NativeObject, callback, (IntPtr)callbackData);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImageRectRectI(this.nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, IntPtr.Zero, null, IntPtr.Zero);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImageRectRect(this.nativeObject, image.NativeObject, (float)destRect.X, (float)destRect.Y, (float)destRect.Width, (float)destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, (imageAttrs == null) ? IntPtr.Zero : imageAttrs.NativeObject, null, IntPtr.Zero);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImageRectRectI(this.nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, (imageAttr == null) ? IntPtr.Zero : imageAttr.NativeObject, null, IntPtr.Zero);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImageRectRectI(this.nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, (imageAttr == null) ? IntPtr.Zero : imageAttr.NativeObject, callback, IntPtr.Zero);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, Graphics.DrawImageAbort callback)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImageRectRect(this.nativeObject, image.NativeObject, (float)destRect.X, (float)destRect.Y, (float)destRect.Width, (float)destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, (imageAttrs == null) ? IntPtr.Zero : imageAttrs.NativeObject, callback, IntPtr.Zero);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, Graphics.DrawImageAbort callback, IntPtr callbackData)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImageRectRect(this.nativeObject, image.NativeObject, (float)destRect.X, (float)destRect.Y, (float)destRect.Width, (float)destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, (imageAttrs == null) ? IntPtr.Zero : imageAttrs.NativeObject, callback, callbackData);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, Graphics.DrawImageAbort callback, IntPtr callbackData)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Status status = GDIPlus.GdipDrawImageRectRect(this.nativeObject, image.NativeObject, (float)destRect.X, (float)destRect.Y, (float)destRect.Width, (float)destRect.Height, (float)srcX, (float)srcY, (float)srcWidth, (float)srcHeight, srcUnit, (imageAttrs == null) ? IntPtr.Zero : imageAttrs.NativeObject, callback, callbackData);
			GDIPlus.CheckStatus(status);
		}

		public void DrawImageUnscaled(Image image, Point point)
		{
			this.DrawImageUnscaled(image, point.X, point.Y);
		}

		public void DrawImageUnscaled(Image image, Rectangle rect)
		{
			this.DrawImageUnscaled(image, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void DrawImageUnscaled(Image image, int x, int y)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			this.DrawImage(image, x, y, image.Width, image.Height);
		}

		public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (width <= 0 || height <= 0)
			{
				return;
			}
			using (Image image2 = new Bitmap(width, height))
			{
				using (Graphics graphics = Graphics.FromImage(image2))
				{
					graphics.DrawImage(image, 0, 0, image.Width, image.Height);
					this.DrawImage(image2, x, y, width, height);
				}
			}
		}

		public void DrawImageUnscaledAndClipped(Image image, Rectangle rect)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			int num = ((image.Width <= rect.Width) ? image.Width : rect.Width);
			int num2 = ((image.Height <= rect.Height) ? image.Height : rect.Height);
			this.DrawImageUnscaled(image, rect.X, rect.Y, num, num2);
		}

		public void DrawLine(Pen pen, PointF pt1, PointF pt2)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			Status status = GDIPlus.GdipDrawLine(this.nativeObject, pen.nativeObject, pt1.X, pt1.Y, pt2.X, pt2.Y);
			GDIPlus.CheckStatus(status);
		}

		public void DrawLine(Pen pen, Point pt1, Point pt2)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			Status status = GDIPlus.GdipDrawLineI(this.nativeObject, pen.nativeObject, pt1.X, pt1.Y, pt2.X, pt2.Y);
			GDIPlus.CheckStatus(status);
		}

		public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			Status status = GDIPlus.GdipDrawLineI(this.nativeObject, pen.nativeObject, x1, y1, x2, y2);
			GDIPlus.CheckStatus(status);
		}

		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			Status status = GDIPlus.GdipDrawLine(this.nativeObject, pen.nativeObject, x1, y1, x2, y2);
			GDIPlus.CheckStatus(status);
		}

		public void DrawLines(Pen pen, PointF[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipDrawLines(this.nativeObject, pen.nativeObject, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void DrawLines(Pen pen, Point[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipDrawLinesI(this.nativeObject, pen.nativeObject, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void DrawPath(Pen pen, GraphicsPath path)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			Status status = GDIPlus.GdipDrawPath(this.nativeObject, pen.nativeObject, path.nativePath);
			GDIPlus.CheckStatus(status);
		}

		public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			this.DrawPie(pen, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, startAngle, sweepAngle);
		}

		public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			this.DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
		}

		public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			Status status = GDIPlus.GdipDrawPie(this.nativeObject, pen.nativeObject, x, y, width, height, startAngle, sweepAngle);
			GDIPlus.CheckStatus(status);
		}

		public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			Status status = GDIPlus.GdipDrawPieI(this.nativeObject, pen.nativeObject, x, y, width, height, (float)startAngle, (float)sweepAngle);
			GDIPlus.CheckStatus(status);
		}

		public void DrawPolygon(Pen pen, Point[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipDrawPolygonI(this.nativeObject, pen.nativeObject, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void DrawPolygon(Pen pen, PointF[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipDrawPolygon(this.nativeObject, pen.nativeObject, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void DrawRectangle(Pen pen, Rectangle rect)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			this.DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			Status status = GDIPlus.GdipDrawRectangle(this.nativeObject, pen.nativeObject, x, y, width, height);
			GDIPlus.CheckStatus(status);
		}

		public void DrawRectangle(Pen pen, int x, int y, int width, int height)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			Status status = GDIPlus.GdipDrawRectangleI(this.nativeObject, pen.nativeObject, x, y, width, height);
			GDIPlus.CheckStatus(status);
		}

		public void DrawRectangles(Pen pen, RectangleF[] rects)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("image");
			}
			if (rects == null)
			{
				throw new ArgumentNullException("rects");
			}
			Status status = GDIPlus.GdipDrawRectangles(this.nativeObject, pen.nativeObject, rects, rects.Length);
			GDIPlus.CheckStatus(status);
		}

		public void DrawRectangles(Pen pen, Rectangle[] rects)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("image");
			}
			if (rects == null)
			{
				throw new ArgumentNullException("rects");
			}
			Status status = GDIPlus.GdipDrawRectanglesI(this.nativeObject, pen.nativeObject, rects, rects.Length);
			GDIPlus.CheckStatus(status);
		}

		public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
		{
			this.DrawString(s, font, brush, layoutRectangle, null);
		}

		public void DrawString(string s, Font font, Brush brush, PointF point)
		{
			this.DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0f, 0f), null);
		}

		public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
		{
			this.DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0f, 0f), format);
		}

		public void DrawString(string s, Font font, Brush brush, float x, float y)
		{
			this.DrawString(s, font, brush, new RectangleF(x, y, 0f, 0f), null);
		}

		public void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format)
		{
			this.DrawString(s, font, brush, new RectangleF(x, y, 0f, 0f), format);
		}

		public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
		{
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (s == null || s.Length == 0)
			{
				return;
			}
			Status status = GDIPlus.GdipDrawString(this.nativeObject, s, s.Length, font.NativeObject, ref layoutRectangle, (format == null) ? IntPtr.Zero : format.NativeObject, brush.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public void EndContainer(GraphicsContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			Status status = GDIPlus.GdipEndContainer(this.nativeObject, container.NativeObject);
			GDIPlus.CheckStatus(status);
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point destPoint, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF destPoint, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF destPoint, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point destPoint, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point destPoint, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF destPoint, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		public void ExcludeClip(Rectangle rect)
		{
			Status status = GDIPlus.GdipSetClipRectI(this.nativeObject, rect.X, rect.Y, rect.Width, rect.Height, CombineMode.Exclude);
			GDIPlus.CheckStatus(status);
		}

		public void ExcludeClip(Region region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			Status status = GDIPlus.GdipSetClipRegion(this.nativeObject, region.NativeObject, CombineMode.Exclude);
			GDIPlus.CheckStatus(status);
		}

		public void FillClosedCurve(Brush brush, PointF[] points)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipFillClosedCurve(this.nativeObject, brush.NativeObject, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void FillClosedCurve(Brush brush, Point[] points)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipFillClosedCurveI(this.nativeObject, brush.NativeObject, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			this.FillClosedCurve(brush, points, fillmode, 0.5f);
		}

		public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			this.FillClosedCurve(brush, points, fillmode, 0.5f);
		}

		public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipFillClosedCurve2(this.nativeObject, brush.NativeObject, points, points.Length, tension, fillmode);
			GDIPlus.CheckStatus(status);
		}

		public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipFillClosedCurve2I(this.nativeObject, brush.NativeObject, points, points.Length, tension, fillmode);
			GDIPlus.CheckStatus(status);
		}

		public void FillEllipse(Brush brush, Rectangle rect)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			this.FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void FillEllipse(Brush brush, RectangleF rect)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			this.FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			Status status = GDIPlus.GdipFillEllipse(this.nativeObject, brush.nativeObject, x, y, width, height);
			GDIPlus.CheckStatus(status);
		}

		public void FillEllipse(Brush brush, int x, int y, int width, int height)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			Status status = GDIPlus.GdipFillEllipseI(this.nativeObject, brush.nativeObject, x, y, width, height);
			GDIPlus.CheckStatus(status);
		}

		public void FillPath(Brush brush, GraphicsPath path)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			Status status = GDIPlus.GdipFillPath(this.nativeObject, brush.NativeObject, path.NativeObject);
			GDIPlus.CheckStatus(status);
		}

		public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			Status status = GDIPlus.GdipFillPie(this.nativeObject, brush.NativeObject, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, startAngle, sweepAngle);
			GDIPlus.CheckStatus(status);
		}

		public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			Status status = GDIPlus.GdipFillPieI(this.nativeObject, brush.NativeObject, x, y, width, height, (float)startAngle, (float)sweepAngle);
			GDIPlus.CheckStatus(status);
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			Status status = GDIPlus.GdipFillPie(this.nativeObject, brush.NativeObject, x, y, width, height, startAngle, sweepAngle);
			GDIPlus.CheckStatus(status);
		}

		public void FillPolygon(Brush brush, PointF[] points)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipFillPolygon2(this.nativeObject, brush.nativeObject, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void FillPolygon(Brush brush, Point[] points)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipFillPolygon2I(this.nativeObject, brush.nativeObject, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipFillPolygonI(this.nativeObject, brush.nativeObject, points, points.Length, fillMode);
			GDIPlus.CheckStatus(status);
		}

		public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipFillPolygon(this.nativeObject, brush.nativeObject, points, points.Length, fillMode);
			GDIPlus.CheckStatus(status);
		}

		public void FillRectangle(Brush brush, RectangleF rect)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			this.FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
		}

		public void FillRectangle(Brush brush, Rectangle rect)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			this.FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
		}

		public void FillRectangle(Brush brush, int x, int y, int width, int height)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			Status status = GDIPlus.GdipFillRectangleI(this.nativeObject, brush.nativeObject, x, y, width, height);
			GDIPlus.CheckStatus(status);
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			Status status = GDIPlus.GdipFillRectangle(this.nativeObject, brush.nativeObject, x, y, width, height);
			GDIPlus.CheckStatus(status);
		}

		public void FillRectangles(Brush brush, Rectangle[] rects)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (rects == null)
			{
				throw new ArgumentNullException("rects");
			}
			Status status = GDIPlus.GdipFillRectanglesI(this.nativeObject, brush.nativeObject, rects, rects.Length);
			GDIPlus.CheckStatus(status);
		}

		public void FillRectangles(Brush brush, RectangleF[] rects)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (rects == null)
			{
				throw new ArgumentNullException("rects");
			}
			Status status = GDIPlus.GdipFillRectangles(this.nativeObject, brush.nativeObject, rects, rects.Length);
			GDIPlus.CheckStatus(status);
		}

		public void FillRegion(Brush brush, Region region)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			Status status = GDIPlus.GdipFillRegion(this.nativeObject, brush.NativeObject, region.NativeObject);
			GDIPlus.CheckStatus(status);
		}

		public void Flush()
		{
			this.Flush(FlushIntention.Flush);
		}

		public void Flush(FlushIntention intention)
		{
			if (this.nativeObject == IntPtr.Zero)
			{
				return;
			}
			Status status = GDIPlus.GdipFlush(this.nativeObject, intention);
			GDIPlus.CheckStatus(status);
			if (GDIPlus.UseCarbonDrawable && this.context.ctx != IntPtr.Zero)
			{
				Carbon.CGContextSynchronize(this.context.ctx);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static Graphics FromHdc(IntPtr hdc)
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCreateFromHDC(hdc, out intPtr);
			GDIPlus.CheckStatus(status);
			return new Graphics(intPtr);
		}

		[MonoTODO]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static Graphics FromHdc(IntPtr hdc, IntPtr hdevice)
		{
			throw new NotImplementedException();
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public static Graphics FromHdcInternal(IntPtr hdc)
		{
			GDIPlus.Display = hdc;
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static Graphics FromHwnd(IntPtr hwnd)
		{
			IntPtr intPtr;
			if (GDIPlus.UseCarbonDrawable)
			{
				CarbonContext cgcontextForView = Carbon.GetCGContextForView(hwnd);
				GDIPlus.GdipCreateFromContext_macosx(cgcontextForView.ctx, cgcontextForView.width, cgcontextForView.height, out intPtr);
				return new Graphics(intPtr)
				{
					context = cgcontextForView
				};
			}
			if (GDIPlus.UseX11Drawable)
			{
				if (GDIPlus.Display == IntPtr.Zero)
				{
					GDIPlus.Display = GDIPlus.XOpenDisplay(IntPtr.Zero);
					if (GDIPlus.Display == IntPtr.Zero)
					{
						throw new NotSupportedException("Could not open display (X-Server required. Check you DISPLAY environment variable)");
					}
				}
				if (hwnd == IntPtr.Zero)
				{
					hwnd = GDIPlus.XRootWindow(GDIPlus.Display, GDIPlus.XDefaultScreen(GDIPlus.Display));
				}
				return Graphics.FromXDrawable(hwnd, GDIPlus.Display);
			}
			Status status = GDIPlus.GdipCreateFromHWND(hwnd, out intPtr);
			GDIPlus.CheckStatus(status);
			return new Graphics(intPtr);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public static Graphics FromHwndInternal(IntPtr hwnd)
		{
			return Graphics.FromHwnd(hwnd);
		}

		public static Graphics FromImage(Image image)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if ((image.PixelFormat & PixelFormat.Indexed) != PixelFormat.DontCare)
			{
				throw new Exception(Locale.GetText("Cannot create Graphics from an indexed bitmap."));
			}
			IntPtr intPtr;
			Status status = GDIPlus.GdipGetImageGraphicsContext(image.nativeObject, out intPtr);
			GDIPlus.CheckStatus(status);
			Graphics graphics = new Graphics(intPtr);
			if (GDIPlus.RunningOnUnix())
			{
				Rectangle rectangle = new Rectangle(0, 0, image.Width, image.Height);
				GDIPlus.GdipSetVisibleClip_linux(graphics.NativeObject, ref rectangle);
			}
			return graphics;
		}

		internal static Graphics FromXDrawable(IntPtr drawable, IntPtr display)
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCreateFromXDrawable_linux(drawable, display, out intPtr);
			GDIPlus.CheckStatus(status);
			return new Graphics(intPtr);
		}

		[MonoTODO]
		public static IntPtr GetHalftonePalette()
		{
			throw new NotImplementedException();
		}

		public IntPtr GetHdc()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetDC(this.nativeObject, out this.deviceContextHdc));
			return this.deviceContextHdc;
		}

		public Color GetNearestColor(Color color)
		{
			int num;
			Status status = GDIPlus.GdipGetNearestColor(this.nativeObject, out num);
			GDIPlus.CheckStatus(status);
			return Color.FromArgb(num);
		}

		public void IntersectClip(Region region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			Status status = GDIPlus.GdipSetClipRegion(this.nativeObject, region.NativeObject, CombineMode.Intersect);
			GDIPlus.CheckStatus(status);
		}

		public void IntersectClip(RectangleF rect)
		{
			Status status = GDIPlus.GdipSetClipRect(this.nativeObject, rect.X, rect.Y, rect.Width, rect.Height, CombineMode.Intersect);
			GDIPlus.CheckStatus(status);
		}

		public void IntersectClip(Rectangle rect)
		{
			Status status = GDIPlus.GdipSetClipRectI(this.nativeObject, rect.X, rect.Y, rect.Width, rect.Height, CombineMode.Intersect);
			GDIPlus.CheckStatus(status);
		}

		public bool IsVisible(Point point)
		{
			bool flag = false;
			Status status = GDIPlus.GdipIsVisiblePointI(this.nativeObject, point.X, point.Y, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(RectangleF rect)
		{
			bool flag = false;
			Status status = GDIPlus.GdipIsVisibleRect(this.nativeObject, rect.X, rect.Y, rect.Width, rect.Height, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(PointF point)
		{
			bool flag = false;
			Status status = GDIPlus.GdipIsVisiblePoint(this.nativeObject, point.X, point.Y, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(Rectangle rect)
		{
			bool flag = false;
			Status status = GDIPlus.GdipIsVisibleRectI(this.nativeObject, rect.X, rect.Y, rect.Width, rect.Height, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(float x, float y)
		{
			return this.IsVisible(new PointF(x, y));
		}

		public bool IsVisible(int x, int y)
		{
			return this.IsVisible(new Point(x, y));
		}

		public bool IsVisible(float x, float y, float width, float height)
		{
			return this.IsVisible(new RectangleF(x, y, width, height));
		}

		public bool IsVisible(int x, int y, int width, int height)
		{
			return this.IsVisible(new Rectangle(x, y, width, height));
		}

		public Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat)
		{
			if (text == null || text.Length == 0)
			{
				return new Region[0];
			}
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}
			if (stringFormat == null)
			{
				throw new ArgumentException("stringFormat");
			}
			int measurableCharacterRangeCount = stringFormat.GetMeasurableCharacterRangeCount();
			if (measurableCharacterRangeCount == 0)
			{
				return new Region[0];
			}
			IntPtr[] array = new IntPtr[measurableCharacterRangeCount];
			Region[] array2 = new Region[measurableCharacterRangeCount];
			for (int i = 0; i < measurableCharacterRangeCount; i++)
			{
				array2[i] = new Region();
				array[i] = array2[i].NativeObject;
			}
			Status status = GDIPlus.GdipMeasureCharacterRanges(this.nativeObject, text, text.Length, font.NativeObject, ref layoutRect, stringFormat.NativeObject, measurableCharacterRangeCount, out array[0]);
			GDIPlus.CheckStatus(status);
			return array2;
		}

		private SizeF GdipMeasureString(IntPtr graphics, string text, Font font, ref RectangleF layoutRect, IntPtr stringFormat)
		{
			if (text == null || text.Length == 0)
			{
				return SizeF.Empty;
			}
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}
			RectangleF rectangleF = default(RectangleF);
			Status status = GDIPlus.GdipMeasureString(this.nativeObject, text, text.Length, font.NativeObject, ref layoutRect, stringFormat, out rectangleF, null, null);
			GDIPlus.CheckStatus(status);
			return new SizeF(rectangleF.Width, rectangleF.Height);
		}

		public SizeF MeasureString(string text, Font font)
		{
			return this.MeasureString(text, font, SizeF.Empty);
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea)
		{
			RectangleF rectangleF = new RectangleF(0f, 0f, layoutArea.Width, layoutArea.Height);
			return this.GdipMeasureString(this.nativeObject, text, font, ref rectangleF, IntPtr.Zero);
		}

		public SizeF MeasureString(string text, Font font, int width)
		{
			RectangleF rectangleF = new RectangleF(0f, 0f, (float)width, 2.1474836E+09f);
			return this.GdipMeasureString(this.nativeObject, text, font, ref rectangleF, IntPtr.Zero);
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			RectangleF rectangleF = new RectangleF(0f, 0f, layoutArea.Width, layoutArea.Height);
			IntPtr intPtr = ((stringFormat != null) ? stringFormat.NativeObject : IntPtr.Zero);
			return this.GdipMeasureString(this.nativeObject, text, font, ref rectangleF, intPtr);
		}

		public SizeF MeasureString(string text, Font font, int width, StringFormat format)
		{
			RectangleF rectangleF = new RectangleF(0f, 0f, (float)width, 2.1474836E+09f);
			IntPtr intPtr = ((format != null) ? format.NativeObject : IntPtr.Zero);
			return this.GdipMeasureString(this.nativeObject, text, font, ref rectangleF, intPtr);
		}

		public SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat)
		{
			RectangleF rectangleF = new RectangleF(origin.X, origin.Y, 0f, 0f);
			IntPtr intPtr = ((stringFormat != null) ? stringFormat.NativeObject : IntPtr.Zero);
			return this.GdipMeasureString(this.nativeObject, text, font, ref rectangleF, intPtr);
		}

		public unsafe SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
		{
			charactersFitted = 0;
			linesFilled = 0;
			if (text == null || text.Length == 0)
			{
				return SizeF.Empty;
			}
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}
			RectangleF rectangleF = default(RectangleF);
			RectangleF rectangleF2 = new RectangleF(0f, 0f, layoutArea.Width, layoutArea.Height);
			IntPtr intPtr = ((stringFormat != null) ? stringFormat.NativeObject : IntPtr.Zero);
			fixed (int* ptr = &charactersFitted, ptr2 = &linesFilled)
			{
				Status status = GDIPlus.GdipMeasureString(this.nativeObject, text, text.Length, font.NativeObject, ref rectangleF2, intPtr, out rectangleF, ptr, ptr2);
				GDIPlus.CheckStatus(status);
			}
			return new SizeF(rectangleF.Width, rectangleF.Height);
		}

		public void MultiplyTransform(Matrix matrix)
		{
			this.MultiplyTransform(matrix, MatrixOrder.Prepend);
		}

		public void MultiplyTransform(Matrix matrix, MatrixOrder order)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}
			Status status = GDIPlus.GdipMultiplyWorldTransform(this.nativeObject, matrix.nativeMatrix, order);
			GDIPlus.CheckStatus(status);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public void ReleaseHdc(IntPtr hdc)
		{
			this.ReleaseHdcInternal(hdc);
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public void ReleaseHdc()
		{
			this.ReleaseHdcInternal(this.deviceContextHdc);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[MonoLimitation("Can only be used when hdc was provided by Graphics.GetHdc() method")]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public void ReleaseHdcInternal(IntPtr hdc)
		{
			Status status = Status.InvalidParameter;
			if (hdc == this.deviceContextHdc)
			{
				status = GDIPlus.GdipReleaseDC(this.nativeObject, this.deviceContextHdc);
				this.deviceContextHdc = IntPtr.Zero;
			}
			GDIPlus.CheckStatus(status);
		}

		public void ResetClip()
		{
			Status status = GDIPlus.GdipResetClip(this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public void ResetTransform()
		{
			Status status = GDIPlus.GdipResetWorldTransform(this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public void Restore(GraphicsState gstate)
		{
			Status status = GDIPlus.GdipRestoreGraphics(this.nativeObject, gstate.nativeState);
			GDIPlus.CheckStatus(status);
		}

		public void RotateTransform(float angle)
		{
			this.RotateTransform(angle, MatrixOrder.Prepend);
		}

		public void RotateTransform(float angle, MatrixOrder order)
		{
			Status status = GDIPlus.GdipRotateWorldTransform(this.nativeObject, angle, order);
			GDIPlus.CheckStatus(status);
		}

		public GraphicsState Save()
		{
			uint num;
			Status status = GDIPlus.GdipSaveGraphics(this.nativeObject, out num);
			GDIPlus.CheckStatus(status);
			return new GraphicsState
			{
				nativeState = num
			};
		}

		public void ScaleTransform(float sx, float sy)
		{
			this.ScaleTransform(sx, sy, MatrixOrder.Prepend);
		}

		public void ScaleTransform(float sx, float sy, MatrixOrder order)
		{
			Status status = GDIPlus.GdipScaleWorldTransform(this.nativeObject, sx, sy, order);
			GDIPlus.CheckStatus(status);
		}

		public void SetClip(RectangleF rect)
		{
			this.SetClip(rect, CombineMode.Replace);
		}

		public void SetClip(GraphicsPath path)
		{
			this.SetClip(path, CombineMode.Replace);
		}

		public void SetClip(Rectangle rect)
		{
			this.SetClip(rect, CombineMode.Replace);
		}

		public void SetClip(Graphics g)
		{
			this.SetClip(g, CombineMode.Replace);
		}

		public void SetClip(Graphics g, CombineMode combineMode)
		{
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			Status status = GDIPlus.GdipSetClipGraphics(this.nativeObject, g.NativeObject, combineMode);
			GDIPlus.CheckStatus(status);
		}

		public void SetClip(Rectangle rect, CombineMode combineMode)
		{
			Status status = GDIPlus.GdipSetClipRectI(this.nativeObject, rect.X, rect.Y, rect.Width, rect.Height, combineMode);
			GDIPlus.CheckStatus(status);
		}

		public void SetClip(RectangleF rect, CombineMode combineMode)
		{
			Status status = GDIPlus.GdipSetClipRect(this.nativeObject, rect.X, rect.Y, rect.Width, rect.Height, combineMode);
			GDIPlus.CheckStatus(status);
		}

		public void SetClip(Region region, CombineMode combineMode)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			Status status = GDIPlus.GdipSetClipRegion(this.nativeObject, region.NativeObject, combineMode);
			GDIPlus.CheckStatus(status);
		}

		public void SetClip(GraphicsPath path, CombineMode combineMode)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			Status status = GDIPlus.GdipSetClipPath(this.nativeObject, path.NativeObject, combineMode);
			GDIPlus.CheckStatus(status);
		}

		public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			IntPtr intPtr = GDIPlus.FromPointToUnManagedMemory(pts);
			Status status = GDIPlus.GdipTransformPoints(this.nativeObject, destSpace, srcSpace, intPtr, pts.Length);
			GDIPlus.CheckStatus(status);
			GDIPlus.FromUnManagedMemoryToPoint(intPtr, pts);
		}

		public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			IntPtr intPtr = GDIPlus.FromPointToUnManagedMemoryI(pts);
			Status status = GDIPlus.GdipTransformPointsI(this.nativeObject, destSpace, srcSpace, intPtr, pts.Length);
			GDIPlus.CheckStatus(status);
			GDIPlus.FromUnManagedMemoryToPointI(intPtr, pts);
		}

		public void TranslateClip(int dx, int dy)
		{
			Status status = GDIPlus.GdipTranslateClipI(this.nativeObject, dx, dy);
			GDIPlus.CheckStatus(status);
		}

		public void TranslateClip(float dx, float dy)
		{
			Status status = GDIPlus.GdipTranslateClip(this.nativeObject, dx, dy);
			GDIPlus.CheckStatus(status);
		}

		public void TranslateTransform(float dx, float dy)
		{
			this.TranslateTransform(dx, dy, MatrixOrder.Prepend);
		}

		public void TranslateTransform(float dx, float dy, MatrixOrder order)
		{
			Status status = GDIPlus.GdipTranslateWorldTransform(this.nativeObject, dx, dy, order);
			GDIPlus.CheckStatus(status);
		}

		public Region Clip
		{
			get
			{
				Region region = new Region();
				Status status = GDIPlus.GdipGetClip(this.nativeObject, region.NativeObject);
				GDIPlus.CheckStatus(status);
				return region;
			}
			set
			{
				this.SetClip(value, CombineMode.Replace);
			}
		}

		public RectangleF ClipBounds
		{
			get
			{
				RectangleF rectangleF = default(RectangleF);
				Status status = GDIPlus.GdipGetClipBounds(this.nativeObject, out rectangleF);
				GDIPlus.CheckStatus(status);
				return rectangleF;
			}
		}

		public CompositingMode CompositingMode
		{
			get
			{
				CompositingMode compositingMode;
				Status status = GDIPlus.GdipGetCompositingMode(this.nativeObject, out compositingMode);
				GDIPlus.CheckStatus(status);
				return compositingMode;
			}
			set
			{
				Status status = GDIPlus.GdipSetCompositingMode(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public CompositingQuality CompositingQuality
		{
			get
			{
				CompositingQuality compositingQuality;
				Status status = GDIPlus.GdipGetCompositingQuality(this.nativeObject, out compositingQuality);
				GDIPlus.CheckStatus(status);
				return compositingQuality;
			}
			set
			{
				Status status = GDIPlus.GdipSetCompositingQuality(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public float DpiX
		{
			get
			{
				float num;
				Status status = GDIPlus.GdipGetDpiX(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
		}

		public float DpiY
		{
			get
			{
				float num;
				Status status = GDIPlus.GdipGetDpiY(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
		}

		public InterpolationMode InterpolationMode
		{
			get
			{
				InterpolationMode interpolationMode = InterpolationMode.Invalid;
				Status status = GDIPlus.GdipGetInterpolationMode(this.nativeObject, out interpolationMode);
				GDIPlus.CheckStatus(status);
				return interpolationMode;
			}
			set
			{
				Status status = GDIPlus.GdipSetInterpolationMode(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public bool IsClipEmpty
		{
			get
			{
				bool flag = false;
				Status status = GDIPlus.GdipIsClipEmpty(this.nativeObject, out flag);
				GDIPlus.CheckStatus(status);
				return flag;
			}
		}

		public bool IsVisibleClipEmpty
		{
			get
			{
				bool flag = false;
				Status status = GDIPlus.GdipIsVisibleClipEmpty(this.nativeObject, out flag);
				GDIPlus.CheckStatus(status);
				return flag;
			}
		}

		public float PageScale
		{
			get
			{
				float num;
				Status status = GDIPlus.GdipGetPageScale(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
			set
			{
				Status status = GDIPlus.GdipSetPageScale(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public GraphicsUnit PageUnit
		{
			get
			{
				GraphicsUnit graphicsUnit;
				Status status = GDIPlus.GdipGetPageUnit(this.nativeObject, out graphicsUnit);
				GDIPlus.CheckStatus(status);
				return graphicsUnit;
			}
			set
			{
				Status status = GDIPlus.GdipSetPageUnit(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		[MonoTODO("This property does not do anything when used with libgdiplus.")]
		public PixelOffsetMode PixelOffsetMode
		{
			get
			{
				PixelOffsetMode pixelOffsetMode = PixelOffsetMode.Invalid;
				Status status = GDIPlus.GdipGetPixelOffsetMode(this.nativeObject, out pixelOffsetMode);
				GDIPlus.CheckStatus(status);
				return pixelOffsetMode;
			}
			set
			{
				Status status = GDIPlus.GdipSetPixelOffsetMode(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public Point RenderingOrigin
		{
			get
			{
				int num;
				int num2;
				Status status = GDIPlus.GdipGetRenderingOrigin(this.nativeObject, out num, out num2);
				GDIPlus.CheckStatus(status);
				return new Point(num, num2);
			}
			set
			{
				Status status = GDIPlus.GdipSetRenderingOrigin(this.nativeObject, value.X, value.Y);
				GDIPlus.CheckStatus(status);
			}
		}

		public SmoothingMode SmoothingMode
		{
			get
			{
				SmoothingMode smoothingMode = SmoothingMode.Invalid;
				Status status = GDIPlus.GdipGetSmoothingMode(this.nativeObject, out smoothingMode);
				GDIPlus.CheckStatus(status);
				return smoothingMode;
			}
			set
			{
				Status status = GDIPlus.GdipSetSmoothingMode(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		[MonoTODO("This property does not do anything when used with libgdiplus.")]
		public int TextContrast
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipGetTextContrast(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
			set
			{
				Status status = GDIPlus.GdipSetTextContrast(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public TextRenderingHint TextRenderingHint
		{
			get
			{
				TextRenderingHint textRenderingHint;
				Status status = GDIPlus.GdipGetTextRenderingHint(this.nativeObject, out textRenderingHint);
				GDIPlus.CheckStatus(status);
				return textRenderingHint;
			}
			set
			{
				Status status = GDIPlus.GdipSetTextRenderingHint(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public Matrix Transform
		{
			get
			{
				Matrix matrix = new Matrix();
				Status status = GDIPlus.GdipGetWorldTransform(this.nativeObject, matrix.nativeMatrix);
				GDIPlus.CheckStatus(status);
				return matrix;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				Status status = GDIPlus.GdipSetWorldTransform(this.nativeObject, value.nativeMatrix);
				GDIPlus.CheckStatus(status);
			}
		}

		public RectangleF VisibleClipBounds
		{
			get
			{
				RectangleF rectangleF;
				Status status = GDIPlus.GdipGetVisibleClipBounds(this.nativeObject, out rectangleF);
				GDIPlus.CheckStatus(status);
				return rectangleF;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[MonoTODO]
		public object GetContextInfo()
		{
			throw new NotImplementedException();
		}

		private const string MetafileEnumeration = "Metafiles enumeration, for both WMF and EMF formats, isn't supported.";

		internal IntPtr nativeObject = IntPtr.Zero;

		internal CarbonContext context;

		private bool disposed;

		private static float defDpiX;

		private static float defDpiY;

		private IntPtr deviceContextHdc;

		public delegate bool EnumerateMetafileProc(EmfPlusRecordType recordType, int flags, int dataSize, IntPtr data, PlayRecordCallback callbackData);

		public delegate bool DrawImageAbort(IntPtr callbackData);
	}
}
