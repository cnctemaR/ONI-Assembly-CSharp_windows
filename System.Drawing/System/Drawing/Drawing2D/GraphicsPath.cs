using System;
using System.ComponentModel;

namespace System.Drawing.Drawing2D
{
	public sealed class GraphicsPath : MarshalByRefObject, ICloneable, IDisposable
	{
		private GraphicsPath(IntPtr ptr)
		{
			this.nativePath = ptr;
		}

		public GraphicsPath()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCreatePath(FillMode.Alternate, out this.nativePath));
		}

		public GraphicsPath(FillMode fillMode)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCreatePath(fillMode, out this.nativePath));
		}

		public GraphicsPath(Point[] pts, byte[] types)
			: this(pts, types, FillMode.Alternate)
		{
		}

		public GraphicsPath(PointF[] pts, byte[] types)
			: this(pts, types, FillMode.Alternate)
		{
		}

		public GraphicsPath(Point[] pts, byte[] types, FillMode fillMode)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			if (pts.Length != types.Length)
			{
				throw new ArgumentException("Invalid parameter passed. Number of points and types must be same.");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCreatePath2I(pts, types, pts.Length, fillMode, out this.nativePath));
		}

		public GraphicsPath(PointF[] pts, byte[] types, FillMode fillMode)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			if (pts.Length != types.Length)
			{
				throw new ArgumentException("Invalid parameter passed. Number of points and types must be same.");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCreatePath2(pts, types, pts.Length, fillMode, out this.nativePath));
		}

		public object Clone()
		{
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipClonePath(this.nativePath, out intPtr));
			return new GraphicsPath(intPtr);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~GraphicsPath()
		{
			this.Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (this.nativePath != IntPtr.Zero)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipDeletePath(this.nativePath));
				this.nativePath = IntPtr.Zero;
			}
		}

		public FillMode FillMode
		{
			get
			{
				FillMode fillMode;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPathFillMode(this.nativePath, out fillMode));
				return fillMode;
			}
			set
			{
				if (value < FillMode.Alternate || value > FillMode.Winding)
				{
					throw new InvalidEnumArgumentException("FillMode", (int)value, typeof(FillMode));
				}
				GDIPlus.CheckStatus(GDIPlus.GdipSetPathFillMode(this.nativePath, value));
			}
		}

		public PathData PathData
		{
			get
			{
				int num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPointCount(this.nativePath, out num));
				PointF[] array = new PointF[num];
				byte[] array2 = new byte[num];
				if (num > 0)
				{
					GDIPlus.CheckStatus(GDIPlus.GdipGetPathPoints(this.nativePath, array, num));
					GDIPlus.CheckStatus(GDIPlus.GdipGetPathTypes(this.nativePath, array2, num));
				}
				return new PathData
				{
					Points = array,
					Types = array2
				};
			}
		}

		public PointF[] PathPoints
		{
			get
			{
				int num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPointCount(this.nativePath, out num));
				if (num == 0)
				{
					throw new ArgumentException("PathPoints");
				}
				PointF[] array = new PointF[num];
				GDIPlus.CheckStatus(GDIPlus.GdipGetPathPoints(this.nativePath, array, num));
				return array;
			}
		}

		public byte[] PathTypes
		{
			get
			{
				int num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPointCount(this.nativePath, out num));
				if (num == 0)
				{
					throw new ArgumentException("PathTypes");
				}
				byte[] array = new byte[num];
				GDIPlus.CheckStatus(GDIPlus.GdipGetPathTypes(this.nativePath, array, num));
				return array;
			}
		}

		public int PointCount
		{
			get
			{
				int num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPointCount(this.nativePath, out num));
				return num;
			}
		}

		internal IntPtr NativeObject
		{
			get
			{
				return this.nativePath;
			}
			set
			{
				this.nativePath = value;
			}
		}

		public void AddArc(Rectangle rect, float startAngle, float sweepAngle)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathArcI(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle));
		}

		public void AddArc(RectangleF rect, float startAngle, float sweepAngle)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathArc(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle));
		}

		public void AddArc(int x, int y, int width, int height, float startAngle, float sweepAngle)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathArcI(this.nativePath, x, y, width, height, startAngle, sweepAngle));
		}

		public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathArc(this.nativePath, x, y, width, height, startAngle, sweepAngle));
		}

		public void AddBezier(Point pt1, Point pt2, Point pt3, Point pt4)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathBezierI(this.nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y));
		}

		public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathBezier(this.nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y));
		}

		public void AddBezier(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathBezierI(this.nativePath, x1, y1, x2, y2, x3, y3, x4, y4));
		}

		public void AddBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathBezier(this.nativePath, x1, y1, x2, y2, x3, y3, x4, y4));
		}

		public void AddBeziers(params Point[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathBeziersI(this.nativePath, points, points.Length));
		}

		public void AddBeziers(PointF[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathBeziers(this.nativePath, points, points.Length));
		}

		public void AddEllipse(RectangleF rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathEllipse(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height));
		}

		public void AddEllipse(float x, float y, float width, float height)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathEllipse(this.nativePath, x, y, width, height));
		}

		public void AddEllipse(Rectangle rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathEllipseI(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height));
		}

		public void AddEllipse(int x, int y, int width, int height)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathEllipseI(this.nativePath, x, y, width, height));
		}

		public void AddLine(Point pt1, Point pt2)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathLineI(this.nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y));
		}

		public void AddLine(PointF pt1, PointF pt2)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathLine(this.nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y));
		}

		public void AddLine(int x1, int y1, int x2, int y2)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathLineI(this.nativePath, x1, y1, x2, y2));
		}

		public void AddLine(float x1, float y1, float x2, float y2)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathLine(this.nativePath, x1, y1, x2, y2));
		}

		public void AddLines(Point[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			if (points.Length == 0)
			{
				throw new ArgumentException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathLine2I(this.nativePath, points, points.Length));
		}

		public void AddLines(PointF[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			if (points.Length == 0)
			{
				throw new ArgumentException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathLine2(this.nativePath, points, points.Length));
		}

		public void AddPie(Rectangle rect, float startAngle, float sweepAngle)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathPie(this.nativePath, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, startAngle, sweepAngle));
		}

		public void AddPie(int x, int y, int width, int height, float startAngle, float sweepAngle)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathPieI(this.nativePath, x, y, width, height, startAngle, sweepAngle));
		}

		public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathPie(this.nativePath, x, y, width, height, startAngle, sweepAngle));
		}

		public void AddPolygon(Point[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathPolygonI(this.nativePath, points, points.Length));
		}

		public void AddPolygon(PointF[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathPolygon(this.nativePath, points, points.Length));
		}

		public void AddRectangle(Rectangle rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathRectangleI(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height));
		}

		public void AddRectangle(RectangleF rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathRectangle(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height));
		}

		public void AddRectangles(Rectangle[] rects)
		{
			if (rects == null)
			{
				throw new ArgumentNullException("rects");
			}
			if (rects.Length == 0)
			{
				throw new ArgumentException("rects");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathRectanglesI(this.nativePath, rects, rects.Length));
		}

		public void AddRectangles(RectangleF[] rects)
		{
			if (rects == null)
			{
				throw new ArgumentNullException("rects");
			}
			if (rects.Length == 0)
			{
				throw new ArgumentException("rects");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathRectangles(this.nativePath, rects, rects.Length));
		}

		public void AddPath(GraphicsPath addingPath, bool connect)
		{
			if (addingPath == null)
			{
				throw new ArgumentNullException("addingPath");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathPath(this.nativePath, addingPath.nativePath, connect));
		}

		public PointF GetLastPoint()
		{
			PointF pointF;
			GDIPlus.CheckStatus(GDIPlus.GdipGetPathLastPoint(this.nativePath, out pointF));
			return pointF;
		}

		public void AddClosedCurve(Point[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathClosedCurveI(this.nativePath, points, points.Length));
		}

		public void AddClosedCurve(PointF[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathClosedCurve(this.nativePath, points, points.Length));
		}

		public void AddClosedCurve(Point[] points, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathClosedCurve2I(this.nativePath, points, points.Length, tension));
		}

		public void AddClosedCurve(PointF[] points, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathClosedCurve2(this.nativePath, points, points.Length, tension));
		}

		public void AddCurve(Point[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurveI(this.nativePath, points, points.Length));
		}

		public void AddCurve(PointF[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurve(this.nativePath, points, points.Length));
		}

		public void AddCurve(Point[] points, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurve2I(this.nativePath, points, points.Length, tension));
		}

		public void AddCurve(PointF[] points, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurve2(this.nativePath, points, points.Length, tension));
		}

		public void AddCurve(Point[] points, int offset, int numberOfSegments, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurve3I(this.nativePath, points, points.Length, offset, numberOfSegments, tension));
		}

		public void AddCurve(PointF[] points, int offset, int numberOfSegments, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurve3(this.nativePath, points, points.Length, offset, numberOfSegments, tension));
		}

		public void Reset()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipResetPath(this.nativePath));
		}

		public void Reverse()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipReversePath(this.nativePath));
		}

		public void Transform(Matrix matrix)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipTransformPath(this.nativePath, matrix.nativeMatrix));
		}

		[MonoTODO("The StringFormat parameter is ignored when using libgdiplus.")]
		public void AddString(string s, FontFamily family, int style, float emSize, Point origin, StringFormat format)
		{
			this.AddString(s, family, style, emSize, new Rectangle
			{
				X = origin.X,
				Y = origin.Y
			}, format);
		}

		[MonoTODO("The StringFormat parameter is ignored when using libgdiplus.")]
		public void AddString(string s, FontFamily family, int style, float emSize, PointF origin, StringFormat format)
		{
			this.AddString(s, family, style, emSize, new RectangleF
			{
				X = origin.X,
				Y = origin.Y
			}, format);
		}

		[MonoTODO("The layoutRect and StringFormat parameters are ignored when using libgdiplus.")]
		public void AddString(string s, FontFamily family, int style, float emSize, Rectangle layoutRect, StringFormat format)
		{
			if (family == null)
			{
				throw new ArgumentException("family");
			}
			IntPtr intPtr = ((format == null) ? IntPtr.Zero : format.NativeObject);
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathStringI(this.nativePath, s, s.Length, family.NativeFamily, style, emSize, ref layoutRect, intPtr));
		}

		[MonoTODO("The layoutRect and StringFormat parameters are ignored when using libgdiplus.")]
		public void AddString(string s, FontFamily family, int style, float emSize, RectangleF layoutRect, StringFormat format)
		{
			if (family == null)
			{
				throw new ArgumentException("family");
			}
			IntPtr intPtr = ((format == null) ? IntPtr.Zero : format.NativeObject);
			GDIPlus.CheckStatus(GDIPlus.GdipAddPathString(this.nativePath, s, s.Length, family.NativeFamily, style, emSize, ref layoutRect, intPtr));
		}

		public void ClearMarkers()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipClearPathMarkers(this.nativePath));
		}

		public void CloseAllFigures()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipClosePathFigures(this.nativePath));
		}

		public void CloseFigure()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipClosePathFigure(this.nativePath));
		}

		public void Flatten()
		{
			this.Flatten(null, 0.25f);
		}

		public void Flatten(Matrix matrix)
		{
			this.Flatten(matrix, 0.25f);
		}

		public void Flatten(Matrix matrix, float flatness)
		{
			IntPtr intPtr = ((matrix == null) ? IntPtr.Zero : matrix.nativeMatrix);
			GDIPlus.CheckStatus(GDIPlus.GdipFlattenPath(this.nativePath, intPtr, flatness));
		}

		public RectangleF GetBounds()
		{
			return this.GetBounds(null, null);
		}

		public RectangleF GetBounds(Matrix matrix)
		{
			return this.GetBounds(matrix, null);
		}

		public RectangleF GetBounds(Matrix matrix, Pen pen)
		{
			IntPtr intPtr = ((matrix == null) ? IntPtr.Zero : matrix.nativeMatrix);
			IntPtr intPtr2 = ((pen == null) ? IntPtr.Zero : pen.NativePen);
			RectangleF rectangleF;
			GDIPlus.CheckStatus(GDIPlus.GdipGetPathWorldBounds(this.nativePath, out rectangleF, intPtr, intPtr2));
			return rectangleF;
		}

		public bool IsOutlineVisible(Point point, Pen pen)
		{
			return this.IsOutlineVisible(point.X, point.Y, pen, null);
		}

		public bool IsOutlineVisible(PointF point, Pen pen)
		{
			return this.IsOutlineVisible(point.X, point.Y, pen, null);
		}

		public bool IsOutlineVisible(int x, int y, Pen pen)
		{
			return this.IsOutlineVisible(x, y, pen, null);
		}

		public bool IsOutlineVisible(float x, float y, Pen pen)
		{
			return this.IsOutlineVisible(x, y, pen, null);
		}

		public bool IsOutlineVisible(Point pt, Pen pen, Graphics graphics)
		{
			return this.IsOutlineVisible(pt.X, pt.Y, pen, graphics);
		}

		public bool IsOutlineVisible(PointF pt, Pen pen, Graphics graphics)
		{
			return this.IsOutlineVisible(pt.X, pt.Y, pen, graphics);
		}

		public bool IsOutlineVisible(int x, int y, Pen pen, Graphics graphics)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			IntPtr intPtr = ((graphics == null) ? IntPtr.Zero : graphics.nativeObject);
			bool flag;
			GDIPlus.CheckStatus(GDIPlus.GdipIsOutlineVisiblePathPointI(this.nativePath, x, y, pen.NativePen, intPtr, out flag));
			return flag;
		}

		public bool IsOutlineVisible(float x, float y, Pen pen, Graphics graphics)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			IntPtr intPtr = ((graphics == null) ? IntPtr.Zero : graphics.nativeObject);
			bool flag;
			GDIPlus.CheckStatus(GDIPlus.GdipIsOutlineVisiblePathPoint(this.nativePath, x, y, pen.NativePen, intPtr, out flag));
			return flag;
		}

		public bool IsVisible(Point point)
		{
			return this.IsVisible(point.X, point.Y, null);
		}

		public bool IsVisible(PointF point)
		{
			return this.IsVisible(point.X, point.Y, null);
		}

		public bool IsVisible(int x, int y)
		{
			return this.IsVisible(x, y, null);
		}

		public bool IsVisible(float x, float y)
		{
			return this.IsVisible(x, y, null);
		}

		public bool IsVisible(Point pt, Graphics graphics)
		{
			return this.IsVisible(pt.X, pt.Y, graphics);
		}

		public bool IsVisible(PointF pt, Graphics graphics)
		{
			return this.IsVisible(pt.X, pt.Y, graphics);
		}

		public bool IsVisible(int x, int y, Graphics graphics)
		{
			IntPtr intPtr = ((graphics == null) ? IntPtr.Zero : graphics.nativeObject);
			bool flag;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisiblePathPointI(this.nativePath, x, y, intPtr, out flag));
			return flag;
		}

		public bool IsVisible(float x, float y, Graphics graphics)
		{
			IntPtr intPtr = ((graphics == null) ? IntPtr.Zero : graphics.nativeObject);
			bool flag;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisiblePathPoint(this.nativePath, x, y, intPtr, out flag));
			return flag;
		}

		public void SetMarkers()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetPathMarker(this.nativePath));
		}

		public void StartFigure()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipStartPathFigure(this.nativePath));
		}

		[MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
		public void Warp(PointF[] destPoints, RectangleF srcRect)
		{
			this.Warp(destPoints, srcRect, null, WarpMode.Perspective, 0.25f);
		}

		[MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
		public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix)
		{
			this.Warp(destPoints, srcRect, matrix, WarpMode.Perspective, 0.25f);
		}

		[MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
		public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix, WarpMode warpMode)
		{
			this.Warp(destPoints, srcRect, matrix, warpMode, 0.25f);
		}

		[MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
		public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix, WarpMode warpMode, float flatness)
		{
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			IntPtr intPtr = ((matrix == null) ? IntPtr.Zero : matrix.nativeMatrix);
			GDIPlus.CheckStatus(GDIPlus.GdipWarpPath(this.nativePath, intPtr, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, warpMode, flatness));
		}

		[MonoTODO("GdipWidenPath isn't implemented in libgdiplus")]
		public void Widen(Pen pen)
		{
			this.Widen(pen, null, 0.25f);
		}

		[MonoTODO("GdipWidenPath isn't implemented in libgdiplus")]
		public void Widen(Pen pen, Matrix matrix)
		{
			this.Widen(pen, matrix, 0.25f);
		}

		[MonoTODO("GdipWidenPath isn't implemented in libgdiplus")]
		public void Widen(Pen pen, Matrix matrix, float flatness)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (this.PointCount == 0)
			{
				return;
			}
			IntPtr intPtr = ((matrix == null) ? IntPtr.Zero : matrix.nativeMatrix);
			GDIPlus.CheckStatus(GDIPlus.GdipWidenPath(this.nativePath, pen.NativePen, intPtr, flatness));
		}

		private const float FlatnessDefault = 0.25f;

		internal IntPtr nativePath = IntPtr.Zero;
	}
}
