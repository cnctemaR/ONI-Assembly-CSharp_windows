using System;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Drawing
{
	public sealed class Region : MarshalByRefObject, IDisposable
	{
		public Region()
		{
			Status status = GDIPlus.GdipCreateRegion(out this.nativeRegion);
			GDIPlus.CheckStatus(status);
		}

		internal Region(IntPtr native)
		{
			this.nativeRegion = native;
		}

		public Region(GraphicsPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			Status status = GDIPlus.GdipCreateRegionPath(path.NativeObject, out this.nativeRegion);
			GDIPlus.CheckStatus(status);
		}

		public Region(Rectangle rect)
		{
			Status status = GDIPlus.GdipCreateRegionRectI(ref rect, out this.nativeRegion);
			GDIPlus.CheckStatus(status);
		}

		public Region(RectangleF rect)
		{
			Status status = GDIPlus.GdipCreateRegionRect(ref rect, out this.nativeRegion);
			GDIPlus.CheckStatus(status);
		}

		public Region(RegionData rgnData)
		{
			if (rgnData == null)
			{
				throw new ArgumentNullException("rgnData");
			}
			if (rgnData.Data.Length == 0)
			{
				throw new ArgumentException("rgnData");
			}
			Status status = GDIPlus.GdipCreateRegionRgnData(rgnData.Data, rgnData.Data.Length, out this.nativeRegion);
			GDIPlus.CheckStatus(status);
		}

		public void Union(GraphicsPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			Status status = GDIPlus.GdipCombineRegionPath(this.nativeRegion, path.NativeObject, CombineMode.Union);
			GDIPlus.CheckStatus(status);
		}

		public void Union(Rectangle rect)
		{
			Status status = GDIPlus.GdipCombineRegionRectI(this.nativeRegion, ref rect, CombineMode.Union);
			GDIPlus.CheckStatus(status);
		}

		public void Union(RectangleF rect)
		{
			Status status = GDIPlus.GdipCombineRegionRect(this.nativeRegion, ref rect, CombineMode.Union);
			GDIPlus.CheckStatus(status);
		}

		public void Union(Region region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			Status status = GDIPlus.GdipCombineRegionRegion(this.nativeRegion, region.NativeObject, CombineMode.Union);
			GDIPlus.CheckStatus(status);
		}

		public void Intersect(GraphicsPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			Status status = GDIPlus.GdipCombineRegionPath(this.nativeRegion, path.NativeObject, CombineMode.Intersect);
			GDIPlus.CheckStatus(status);
		}

		public void Intersect(Rectangle rect)
		{
			Status status = GDIPlus.GdipCombineRegionRectI(this.nativeRegion, ref rect, CombineMode.Intersect);
			GDIPlus.CheckStatus(status);
		}

		public void Intersect(RectangleF rect)
		{
			Status status = GDIPlus.GdipCombineRegionRect(this.nativeRegion, ref rect, CombineMode.Intersect);
			GDIPlus.CheckStatus(status);
		}

		public void Intersect(Region region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			Status status = GDIPlus.GdipCombineRegionRegion(this.nativeRegion, region.NativeObject, CombineMode.Intersect);
			GDIPlus.CheckStatus(status);
		}

		public void Complement(GraphicsPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			Status status = GDIPlus.GdipCombineRegionPath(this.nativeRegion, path.NativeObject, CombineMode.Complement);
			GDIPlus.CheckStatus(status);
		}

		public void Complement(Rectangle rect)
		{
			Status status = GDIPlus.GdipCombineRegionRectI(this.nativeRegion, ref rect, CombineMode.Complement);
			GDIPlus.CheckStatus(status);
		}

		public void Complement(RectangleF rect)
		{
			Status status = GDIPlus.GdipCombineRegionRect(this.nativeRegion, ref rect, CombineMode.Complement);
			GDIPlus.CheckStatus(status);
		}

		public void Complement(Region region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			Status status = GDIPlus.GdipCombineRegionRegion(this.nativeRegion, region.NativeObject, CombineMode.Complement);
			GDIPlus.CheckStatus(status);
		}

		public void Exclude(GraphicsPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			Status status = GDIPlus.GdipCombineRegionPath(this.nativeRegion, path.NativeObject, CombineMode.Exclude);
			GDIPlus.CheckStatus(status);
		}

		public void Exclude(Rectangle rect)
		{
			Status status = GDIPlus.GdipCombineRegionRectI(this.nativeRegion, ref rect, CombineMode.Exclude);
			GDIPlus.CheckStatus(status);
		}

		public void Exclude(RectangleF rect)
		{
			Status status = GDIPlus.GdipCombineRegionRect(this.nativeRegion, ref rect, CombineMode.Exclude);
			GDIPlus.CheckStatus(status);
		}

		public void Exclude(Region region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			Status status = GDIPlus.GdipCombineRegionRegion(this.nativeRegion, region.NativeObject, CombineMode.Exclude);
			GDIPlus.CheckStatus(status);
		}

		public void Xor(GraphicsPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			Status status = GDIPlus.GdipCombineRegionPath(this.nativeRegion, path.NativeObject, CombineMode.Xor);
			GDIPlus.CheckStatus(status);
		}

		public void Xor(Rectangle rect)
		{
			Status status = GDIPlus.GdipCombineRegionRectI(this.nativeRegion, ref rect, CombineMode.Xor);
			GDIPlus.CheckStatus(status);
		}

		public void Xor(RectangleF rect)
		{
			Status status = GDIPlus.GdipCombineRegionRect(this.nativeRegion, ref rect, CombineMode.Xor);
			GDIPlus.CheckStatus(status);
		}

		public void Xor(Region region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			Status status = GDIPlus.GdipCombineRegionRegion(this.nativeRegion, region.NativeObject, CombineMode.Xor);
			GDIPlus.CheckStatus(status);
		}

		public RectangleF GetBounds(Graphics g)
		{
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			RectangleF rectangleF = default(Rectangle);
			Status status = GDIPlus.GdipGetRegionBounds(this.nativeRegion, g.NativeObject, ref rectangleF);
			GDIPlus.CheckStatus(status);
			return rectangleF;
		}

		public void Translate(int dx, int dy)
		{
			Status status = GDIPlus.GdipTranslateRegionI(this.nativeRegion, dx, dy);
			GDIPlus.CheckStatus(status);
		}

		public void Translate(float dx, float dy)
		{
			Status status = GDIPlus.GdipTranslateRegion(this.nativeRegion, dx, dy);
			GDIPlus.CheckStatus(status);
		}

		public bool IsVisible(int x, int y, Graphics g)
		{
			IntPtr intPtr = ((g != null) ? g.NativeObject : IntPtr.Zero);
			bool flag;
			Status status = GDIPlus.GdipIsVisibleRegionPointI(this.nativeRegion, x, y, intPtr, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(int x, int y, int width, int height)
		{
			bool flag;
			Status status = GDIPlus.GdipIsVisibleRegionRectI(this.nativeRegion, x, y, width, height, IntPtr.Zero, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(int x, int y, int width, int height, Graphics g)
		{
			IntPtr intPtr = ((g != null) ? g.NativeObject : IntPtr.Zero);
			bool flag;
			Status status = GDIPlus.GdipIsVisibleRegionRectI(this.nativeRegion, x, y, width, height, intPtr, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(Point point)
		{
			bool flag;
			Status status = GDIPlus.GdipIsVisibleRegionPointI(this.nativeRegion, point.X, point.Y, IntPtr.Zero, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(PointF point)
		{
			bool flag;
			Status status = GDIPlus.GdipIsVisibleRegionPoint(this.nativeRegion, point.X, point.Y, IntPtr.Zero, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(Point point, Graphics g)
		{
			IntPtr intPtr = ((g != null) ? g.NativeObject : IntPtr.Zero);
			bool flag;
			Status status = GDIPlus.GdipIsVisibleRegionPointI(this.nativeRegion, point.X, point.Y, intPtr, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(PointF point, Graphics g)
		{
			IntPtr intPtr = ((g != null) ? g.NativeObject : IntPtr.Zero);
			bool flag;
			Status status = GDIPlus.GdipIsVisibleRegionPoint(this.nativeRegion, point.X, point.Y, intPtr, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(Rectangle rect)
		{
			bool flag;
			Status status = GDIPlus.GdipIsVisibleRegionRectI(this.nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, IntPtr.Zero, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(RectangleF rect)
		{
			bool flag;
			Status status = GDIPlus.GdipIsVisibleRegionRect(this.nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, IntPtr.Zero, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(Rectangle rect, Graphics g)
		{
			IntPtr intPtr = ((g != null) ? g.NativeObject : IntPtr.Zero);
			bool flag;
			Status status = GDIPlus.GdipIsVisibleRegionRectI(this.nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, intPtr, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(RectangleF rect, Graphics g)
		{
			IntPtr intPtr = ((g != null) ? g.NativeObject : IntPtr.Zero);
			bool flag;
			Status status = GDIPlus.GdipIsVisibleRegionRect(this.nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, intPtr, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(float x, float y)
		{
			bool flag;
			Status status = GDIPlus.GdipIsVisibleRegionPoint(this.nativeRegion, x, y, IntPtr.Zero, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(float x, float y, Graphics g)
		{
			IntPtr intPtr = ((g != null) ? g.NativeObject : IntPtr.Zero);
			bool flag;
			Status status = GDIPlus.GdipIsVisibleRegionPoint(this.nativeRegion, x, y, intPtr, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(float x, float y, float width, float height)
		{
			bool flag;
			Status status = GDIPlus.GdipIsVisibleRegionRect(this.nativeRegion, x, y, width, height, IntPtr.Zero, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsVisible(float x, float y, float width, float height, Graphics g)
		{
			IntPtr intPtr = ((g != null) ? g.NativeObject : IntPtr.Zero);
			bool flag;
			Status status = GDIPlus.GdipIsVisibleRegionRect(this.nativeRegion, x, y, width, height, intPtr, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsEmpty(Graphics g)
		{
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			bool flag;
			Status status = GDIPlus.GdipIsEmptyRegion(this.nativeRegion, g.NativeObject, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public bool IsInfinite(Graphics g)
		{
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			bool flag;
			Status status = GDIPlus.GdipIsInfiniteRegion(this.nativeRegion, g.NativeObject, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public void MakeEmpty()
		{
			Status status = GDIPlus.GdipSetEmpty(this.nativeRegion);
			GDIPlus.CheckStatus(status);
		}

		public void MakeInfinite()
		{
			Status status = GDIPlus.GdipSetInfinite(this.nativeRegion);
			GDIPlus.CheckStatus(status);
		}

		public bool Equals(Region region, Graphics g)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			bool flag;
			Status status = GDIPlus.GdipIsEqualRegion(this.nativeRegion, region.NativeObject, g.NativeObject, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public static Region FromHrgn(IntPtr hrgn)
		{
			if (hrgn == IntPtr.Zero)
			{
				throw new ArgumentException("hrgn");
			}
			IntPtr intPtr;
			Status status = GDIPlus.GdipCreateRegionHrgn(hrgn, out intPtr);
			GDIPlus.CheckStatus(status);
			return new Region(intPtr);
		}

		public IntPtr GetHrgn(Graphics g)
		{
			if (g == null)
			{
				return this.nativeRegion;
			}
			IntPtr zero = IntPtr.Zero;
			Status status = GDIPlus.GdipGetRegionHRgn(this.nativeRegion, g.NativeObject, ref zero);
			GDIPlus.CheckStatus(status);
			return zero;
		}

		public RegionData GetRegionData()
		{
			int num;
			Status status = GDIPlus.GdipGetRegionDataSize(this.nativeRegion, out num);
			GDIPlus.CheckStatus(status);
			byte[] array = new byte[num];
			int num2;
			status = GDIPlus.GdipGetRegionData(this.nativeRegion, array, num, out num2);
			GDIPlus.CheckStatus(status);
			return new RegionData
			{
				Data = array
			};
		}

		public RectangleF[] GetRegionScans(Matrix matrix)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}
			int num;
			Status status = GDIPlus.GdipGetRegionScansCount(this.nativeRegion, out num, matrix.NativeObject);
			GDIPlus.CheckStatus(status);
			if (num == 0)
			{
				return new RectangleF[0];
			}
			RectangleF[] array = new RectangleF[num];
			int num2 = Marshal.SizeOf(array[0]);
			IntPtr intPtr = Marshal.AllocHGlobal(num2 * num);
			try
			{
				status = GDIPlus.GdipGetRegionScans(this.nativeRegion, intPtr, out num, matrix.NativeObject);
				GDIPlus.CheckStatus(status);
			}
			finally
			{
				GDIPlus.FromUnManagedMemoryToRectangles(intPtr, array);
			}
			return array;
		}

		public void Transform(Matrix matrix)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}
			Status status = GDIPlus.GdipTransformRegion(this.nativeRegion, matrix.NativeObject);
			GDIPlus.CheckStatus(status);
		}

		public Region Clone()
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCloneRegion(this.nativeRegion, out intPtr);
			GDIPlus.CheckStatus(status);
			return new Region(intPtr);
		}

		public void Dispose()
		{
			this.DisposeHandle();
			GC.SuppressFinalize(this);
		}

		private void DisposeHandle()
		{
			if (this.nativeRegion != IntPtr.Zero)
			{
				GDIPlus.GdipDeleteRegion(this.nativeRegion);
				this.nativeRegion = IntPtr.Zero;
			}
		}

		~Region()
		{
			this.DisposeHandle();
		}

		internal IntPtr NativeObject
		{
			get
			{
				return this.nativeRegion;
			}
			set
			{
				this.nativeRegion = value;
			}
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public void ReleaseHrgn(IntPtr regionHandle)
		{
			if (regionHandle == IntPtr.Zero)
			{
				throw new ArgumentNullException("regionHandle");
			}
			Status status = Status.Ok;
			if (GDIPlus.RunningOnUnix())
			{
				status = GDIPlus.GdipDeleteRegion(regionHandle);
			}
			else if (!GDIPlus.DeleteObject(regionHandle))
			{
				status = Status.InvalidParameter;
			}
			GDIPlus.CheckStatus(status);
		}

		private IntPtr nativeRegion = IntPtr.Zero;
	}
}
