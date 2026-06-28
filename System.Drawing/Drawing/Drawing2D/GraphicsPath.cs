using System;
using System.ComponentModel;

namespace System.Drawing.Drawing2D
{
	public sealed class GraphicsPath : MarshalByRefObject, IDisposable, ICloneable
	{
		private GraphicsPath(IntPtr ptr)
		{
			this.nativePath = ptr;
		}

		public GraphicsPath()
		{
			Status status = GDIPlus.GdipCreatePath(FillMode.Alternate, out this.nativePath);
			GDIPlus.CheckStatus(status);
		}

		public GraphicsPath(FillMode fillMode)
		{
			Status status = GDIPlus.GdipCreatePath(fillMode, out this.nativePath);
			GDIPlus.CheckStatus(status);
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
			Status status = GDIPlus.GdipCreatePath2I(pts, types, pts.Length, fillMode, out this.nativePath);
			GDIPlus.CheckStatus(status);
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
			Status status = GDIPlus.GdipCreatePath2(pts, types, pts.Length, fillMode, out this.nativePath);
			GDIPlus.CheckStatus(status);
		}

		public object Clone()
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipClonePath(this.nativePath, out intPtr);
			GDIPlus.CheckStatus(status);
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
				Status status = GDIPlus.GdipDeletePath(this.nativePath);
				GDIPlus.CheckStatus(status);
				this.nativePath = IntPtr.Zero;
			}
		}

		public FillMode FillMode
		{
			get
			{
				FillMode fillMode;
				Status status = GDIPlus.GdipGetPathFillMode(this.nativePath, out fillMode);
				GDIPlus.CheckStatus(status);
				return fillMode;
			}
			set
			{
				if (value < FillMode.Alternate || value > FillMode.Winding)
				{
					throw new InvalidEnumArgumentException("FillMode", (int)value, typeof(FillMode));
				}
				Status status = GDIPlus.GdipSetPathFillMode(this.nativePath, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public PathData PathData
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipGetPointCount(this.nativePath, out num);
				GDIPlus.CheckStatus(status);
				PointF[] array = new PointF[num];
				byte[] array2 = new byte[num];
				if (num > 0)
				{
					status = GDIPlus.GdipGetPathPoints(this.nativePath, array, num);
					GDIPlus.CheckStatus(status);
					status = GDIPlus.GdipGetPathTypes(this.nativePath, array2, num);
					GDIPlus.CheckStatus(status);
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
				Status status = GDIPlus.GdipGetPointCount(this.nativePath, out num);
				GDIPlus.CheckStatus(status);
				if (num == 0)
				{
					throw new ArgumentException("PathPoints");
				}
				PointF[] array = new PointF[num];
				status = GDIPlus.GdipGetPathPoints(this.nativePath, array, num);
				GDIPlus.CheckStatus(status);
				return array;
			}
		}

		public byte[] PathTypes
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipGetPointCount(this.nativePath, out num);
				GDIPlus.CheckStatus(status);
				if (num == 0)
				{
					throw new ArgumentException("PathTypes");
				}
				byte[] array = new byte[num];
				status = GDIPlus.GdipGetPathTypes(this.nativePath, array, num);
				GDIPlus.CheckStatus(status);
				return array;
			}
		}

		public int PointCount
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipGetPointCount(this.nativePath, out num);
				GDIPlus.CheckStatus(status);
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

		public void AddArc(Rectangle rect, float start_angle, float sweep_angle)
		{
			Status status = GDIPlus.GdipAddPathArcI(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height, start_angle, sweep_angle);
			GDIPlus.CheckStatus(status);
		}

		public void AddArc(RectangleF rect, float start_angle, float sweep_angle)
		{
			Status status = GDIPlus.GdipAddPathArc(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height, start_angle, sweep_angle);
			GDIPlus.CheckStatus(status);
		}

		public void AddArc(int x, int y, int width, int height, float start_angle, float sweep_angle)
		{
			Status status = GDIPlus.GdipAddPathArcI(this.nativePath, x, y, width, height, start_angle, sweep_angle);
			GDIPlus.CheckStatus(status);
		}

		public void AddArc(float x, float y, float width, float height, float start_angle, float sweep_angle)
		{
			Status status = GDIPlus.GdipAddPathArc(this.nativePath, x, y, width, height, start_angle, sweep_angle);
			GDIPlus.CheckStatus(status);
		}

		public void AddBezier(Point pt1, Point pt2, Point pt3, Point pt4)
		{
			Status status = GDIPlus.GdipAddPathBezierI(this.nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
			GDIPlus.CheckStatus(status);
		}

		public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		{
			Status status = GDIPlus.GdipAddPathBezier(this.nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
			GDIPlus.CheckStatus(status);
		}

		public void AddBezier(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
		{
			Status status = GDIPlus.GdipAddPathBezierI(this.nativePath, x1, y1, x2, y2, x3, y3, x4, y4);
			GDIPlus.CheckStatus(status);
		}

		public void AddBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			Status status = GDIPlus.GdipAddPathBezier(this.nativePath, x1, y1, x2, y2, x3, y3, x4, y4);
			GDIPlus.CheckStatus(status);
		}

		public void AddBeziers(params Point[] pts)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			Status status = GDIPlus.GdipAddPathBeziersI(this.nativePath, pts, pts.Length);
			GDIPlus.CheckStatus(status);
		}

		public void AddBeziers(PointF[] pts)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			Status status = GDIPlus.GdipAddPathBeziers(this.nativePath, pts, pts.Length);
			GDIPlus.CheckStatus(status);
		}

		public void AddEllipse(RectangleF r)
		{
			Status status = GDIPlus.GdipAddPathEllipse(this.nativePath, r.X, r.Y, r.Width, r.Height);
			GDIPlus.CheckStatus(status);
		}

		public void AddEllipse(float x, float y, float width, float height)
		{
			Status status = GDIPlus.GdipAddPathEllipse(this.nativePath, x, y, width, height);
			GDIPlus.CheckStatus(status);
		}

		public void AddEllipse(Rectangle r)
		{
			Status status = GDIPlus.GdipAddPathEllipseI(this.nativePath, r.X, r.Y, r.Width, r.Height);
			GDIPlus.CheckStatus(status);
		}

		public void AddEllipse(int x, int y, int width, int height)
		{
			Status status = GDIPlus.GdipAddPathEllipseI(this.nativePath, x, y, width, height);
			GDIPlus.CheckStatus(status);
		}

		public void AddLine(Point a, Point b)
		{
			Status status = GDIPlus.GdipAddPathLineI(this.nativePath, a.X, a.Y, b.X, b.Y);
			GDIPlus.CheckStatus(status);
		}

		public void AddLine(PointF a, PointF b)
		{
			Status status = GDIPlus.GdipAddPathLine(this.nativePath, a.X, a.Y, b.X, b.Y);
			GDIPlus.CheckStatus(status);
		}

		public void AddLine(int x1, int y1, int x2, int y2)
		{
			Status status = GDIPlus.GdipAddPathLineI(this.nativePath, x1, y1, x2, y2);
			GDIPlus.CheckStatus(status);
		}

		public void AddLine(float x1, float y1, float x2, float y2)
		{
			Status status = GDIPlus.GdipAddPathLine(this.nativePath, x1, y1, x2, y2);
			GDIPlus.CheckStatus(status);
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
			Status status = GDIPlus.GdipAddPathLine2I(this.nativePath, points, points.Length);
			GDIPlus.CheckStatus(status);
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
			Status status = GDIPlus.GdipAddPathLine2(this.nativePath, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void AddPie(Rectangle rect, float startAngle, float sweepAngle)
		{
			Status status = GDIPlus.GdipAddPathPie(this.nativePath, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, startAngle, sweepAngle);
			GDIPlus.CheckStatus(status);
		}

		public void AddPie(int x, int y, int width, int height, float startAngle, float sweepAngle)
		{
			Status status = GDIPlus.GdipAddPathPieI(this.nativePath, x, y, width, height, startAngle, sweepAngle);
			GDIPlus.CheckStatus(status);
		}

		public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			Status status = GDIPlus.GdipAddPathPie(this.nativePath, x, y, width, height, startAngle, sweepAngle);
			GDIPlus.CheckStatus(status);
		}

		public void AddPolygon(Point[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipAddPathPolygonI(this.nativePath, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void AddPolygon(PointF[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipAddPathPolygon(this.nativePath, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void AddRectangle(Rectangle rect)
		{
			Status status = GDIPlus.GdipAddPathRectangleI(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height);
			GDIPlus.CheckStatus(status);
		}

		public void AddRectangle(RectangleF rect)
		{
			Status status = GDIPlus.GdipAddPathRectangle(this.nativePath, rect.X, rect.Y, rect.Width, rect.Height);
			GDIPlus.CheckStatus(status);
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
			Status status = GDIPlus.GdipAddPathRectanglesI(this.nativePath, rects, rects.Length);
			GDIPlus.CheckStatus(status);
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
			Status status = GDIPlus.GdipAddPathRectangles(this.nativePath, rects, rects.Length);
			GDIPlus.CheckStatus(status);
		}

		public void AddPath(GraphicsPath addingPath, bool connect)
		{
			if (addingPath == null)
			{
				throw new ArgumentNullException("addingPath");
			}
			Status status = GDIPlus.GdipAddPathPath(this.nativePath, addingPath.nativePath, connect);
			GDIPlus.CheckStatus(status);
		}

		public PointF GetLastPoint()
		{
			PointF pointF;
			Status status = GDIPlus.GdipGetPathLastPoint(this.nativePath, out pointF);
			GDIPlus.CheckStatus(status);
			return pointF;
		}

		public void AddClosedCurve(Point[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipAddPathClosedCurveI(this.nativePath, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void AddClosedCurve(PointF[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipAddPathClosedCurve(this.nativePath, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void AddClosedCurve(Point[] points, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipAddPathClosedCurve2I(this.nativePath, points, points.Length, tension);
			GDIPlus.CheckStatus(status);
		}

		public void AddClosedCurve(PointF[] points, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipAddPathClosedCurve2(this.nativePath, points, points.Length, tension);
			GDIPlus.CheckStatus(status);
		}

		public void AddCurve(Point[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipAddPathCurveI(this.nativePath, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void AddCurve(PointF[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipAddPathCurve(this.nativePath, points, points.Length);
			GDIPlus.CheckStatus(status);
		}

		public void AddCurve(Point[] points, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipAddPathCurve2I(this.nativePath, points, points.Length, tension);
			GDIPlus.CheckStatus(status);
		}

		public void AddCurve(PointF[] points, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipAddPathCurve2(this.nativePath, points, points.Length, tension);
			GDIPlus.CheckStatus(status);
		}

		public void AddCurve(Point[] points, int offset, int numberOfSegments, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipAddPathCurve3I(this.nativePath, points, points.Length, offset, numberOfSegments, tension);
			GDIPlus.CheckStatus(status);
		}

		public void AddCurve(PointF[] points, int offset, int numberOfSegments, float tension)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Status status = GDIPlus.GdipAddPathCurve3(this.nativePath, points, points.Length, offset, numberOfSegments, tension);
			GDIPlus.CheckStatus(status);
		}

		public void Reset()
		{
			Status status = GDIPlus.GdipResetPath(this.nativePath);
			GDIPlus.CheckStatus(status);
		}

		public void Reverse()
		{
			Status status = GDIPlus.GdipReversePath(this.nativePath);
			GDIPlus.CheckStatus(status);
		}

		public void Transform(Matrix matrix)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}
			Status status = GDIPlus.GdipTransformPath(this.nativePath, matrix.nativeMatrix);
			GDIPlus.CheckStatus(status);
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
			IntPtr intPtr = ((format != null) ? format.NativeObject : IntPtr.Zero);
			Status status = GDIPlus.GdipAddPathStringI(this.nativePath, s, s.Length, family.NativeObject, style, emSize, ref layoutRect, intPtr);
			GDIPlus.CheckStatus(status);
		}

		[MonoTODO("The layoutRect and StringFormat parameters are ignored when using libgdiplus.")]
		public void AddString(string s, FontFamily family, int style, float emSize, RectangleF layoutRect, StringFormat format)
		{
			if (family == null)
			{
				throw new ArgumentException("family");
			}
			IntPtr intPtr = ((format != null) ? format.NativeObject : IntPtr.Zero);
			Status status = GDIPlus.GdipAddPathString(this.nativePath, s, s.Length, family.NativeObject, style, emSize, ref layoutRect, intPtr);
			GDIPlus.CheckStatus(status);
		}

		public void ClearMarkers()
		{
			Status status = GDIPlus.GdipClearPathMarkers(this.nativePath);
			GDIPlus.CheckStatus(status);
		}

		public void CloseAllFigures()
		{
			Status status = GDIPlus.GdipClosePathFigures(this.nativePath);
			GDIPlus.CheckStatus(status);
		}

		public void CloseFigure()
		{
			Status status = GDIPlus.GdipClosePathFigure(this.nativePath);
			GDIPlus.CheckStatus(status);
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
			IntPtr intPtr = ((matrix != null) ? matrix.nativeMatrix : IntPtr.Zero);
			Status status = GDIPlus.GdipFlattenPath(this.nativePath, intPtr, flatness);
			GDIPlus.CheckStatus(status);
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
			IntPtr intPtr = ((matrix != null) ? matrix.nativeMatrix : IntPtr.Zero);
			IntPtr intPtr2 = ((pen != null) ? pen.nativeObject : IntPtr.Zero);
			RectangleF rectangleF;
			Status status = GDIPlus.GdipGetPathWorldBounds(this.nativePath, out rectangleF, intPtr, intPtr2);
			GDIPlus.CheckStatus(status);
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
			IntPtr intPtr = ((graphics != null) ? graphics.nativeObject : IntPtr.Zero);
			bool flag;
			Status status = GDIPlus.GdipIsOutlineVisiblePathPointI(this.nativePath, x, y, pen.nativeObject, intPtr, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsOutlineVisible(float x, float y, Pen pen, Graphics graphics)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			IntPtr intPtr = ((graphics != null) ? graphics.nativeObject : IntPtr.Zero);
			bool flag;
			Status status = GDIPlus.GdipIsOutlineVisiblePathPoint(this.nativePath, x, y, pen.nativeObject, intPtr, out flag);
			GDIPlus.CheckStatus(status);
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
			IntPtr intPtr = ((graphics != null) ? graphics.nativeObject : IntPtr.Zero);
			bool flag;
			Status status = GDIPlus.GdipIsVisiblePathPointI(this.nativePath, x, y, intPtr, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(float x, float y, Graphics graphics)
		{
			IntPtr intPtr = ((graphics != null) ? graphics.nativeObject : IntPtr.Zero);
			bool flag;
			Status status = GDIPlus.GdipIsVisiblePathPoint(this.nativePath, x, y, intPtr, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public void SetMarkers()
		{
			Status status = GDIPlus.GdipSetPathMarker(this.nativePath);
			GDIPlus.CheckStatus(status);
		}

		public void StartFigure()
		{
			Status status = GDIPlus.GdipStartPathFigure(this.nativePath);
			GDIPlus.CheckStatus(status);
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
			IntPtr intPtr = ((matrix != null) ? matrix.nativeMatrix : IntPtr.Zero);
			Status status = GDIPlus.GdipWarpPath(this.nativePath, intPtr, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, warpMode, flatness);
			GDIPlus.CheckStatus(status);
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
			IntPtr intPtr = ((matrix != null) ? matrix.nativeMatrix : IntPtr.Zero);
			Status status = GDIPlus.GdipWidenPath(this.nativePath, pen.nativeObject, intPtr, flatness);
			GDIPlus.CheckStatus(status);
		}

		private const float FlatnessDefault = 0.25f;

		internal IntPtr nativePath = IntPtr.Zero;
	}
}
