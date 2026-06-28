using System;

namespace System.Drawing.Drawing2D
{
	public sealed class GraphicsPathIterator : MarshalByRefObject, IDisposable
	{
		internal GraphicsPathIterator(IntPtr native)
		{
			this.nativeObject = native;
		}

		public GraphicsPathIterator(GraphicsPath path)
		{
			if (path != null)
			{
				Status status = GDIPlus.GdipCreatePathIter(out this.nativeObject, path.NativeObject);
				GDIPlus.CheckStatus(status);
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

		public int Count
		{
			get
			{
				if (this.nativeObject == IntPtr.Zero)
				{
					return 0;
				}
				int num;
				Status status = GDIPlus.GdipPathIterGetCount(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
		}

		public int SubpathCount
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipPathIterGetSubpathCount(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
		}

		internal void Dispose(bool disposing)
		{
			if (this.nativeObject != IntPtr.Zero)
			{
				Status status = GDIPlus.GdipDeletePathIter(this.nativeObject);
				GDIPlus.CheckStatus(status);
				this.nativeObject = IntPtr.Zero;
			}
		}

		public int CopyData(ref PointF[] points, ref byte[] types, int startIndex, int endIndex)
		{
			if (points.Length != types.Length)
			{
				throw new ArgumentException("Invalid arguments passed. Both arrays should have the same length.");
			}
			int num;
			Status status = GDIPlus.GdipPathIterCopyData(this.nativeObject, out num, points, types, startIndex, endIndex);
			GDIPlus.CheckStatus(status);
			return num;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~GraphicsPathIterator()
		{
			this.Dispose(false);
		}

		public int Enumerate(ref PointF[] points, ref byte[] types)
		{
			int num = points.Length;
			if (num != types.Length)
			{
				throw new ArgumentException("Invalid arguments passed. Both arrays should have the same length.");
			}
			int num2;
			Status status = GDIPlus.GdipPathIterEnumerate(this.nativeObject, out num2, points, types, num);
			GDIPlus.CheckStatus(status);
			return num2;
		}

		public bool HasCurve()
		{
			bool flag;
			Status status = GDIPlus.GdipPathIterHasCurve(this.nativeObject, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}

		public int NextMarker(GraphicsPath path)
		{
			IntPtr intPtr = ((path != null) ? path.NativeObject : IntPtr.Zero);
			int num;
			Status status = GDIPlus.GdipPathIterNextMarkerPath(this.nativeObject, out num, intPtr);
			GDIPlus.CheckStatus(status);
			return num;
		}

		public int NextMarker(out int startIndex, out int endIndex)
		{
			int num;
			Status status = GDIPlus.GdipPathIterNextMarker(this.nativeObject, out num, out startIndex, out endIndex);
			GDIPlus.CheckStatus(status);
			return num;
		}

		public int NextPathType(out byte pathType, out int startIndex, out int endIndex)
		{
			int num;
			Status status = GDIPlus.GdipPathIterNextPathType(this.nativeObject, out num, out pathType, out startIndex, out endIndex);
			GDIPlus.CheckStatus(status);
			return num;
		}

		public int NextSubpath(GraphicsPath path, out bool isClosed)
		{
			IntPtr intPtr = ((path != null) ? path.NativeObject : IntPtr.Zero);
			int num;
			Status status = GDIPlus.GdipPathIterNextSubpathPath(this.nativeObject, out num, intPtr, out isClosed);
			GDIPlus.CheckStatus(status);
			return num;
		}

		public int NextSubpath(out int startIndex, out int endIndex, out bool isClosed)
		{
			int num;
			Status status = GDIPlus.GdipPathIterNextSubpath(this.nativeObject, out num, out startIndex, out endIndex, out isClosed);
			GDIPlus.CheckStatus(status);
			return num;
		}

		public void Rewind()
		{
			Status status = GDIPlus.GdipPathIterRewind(this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		private IntPtr nativeObject = IntPtr.Zero;
	}
}
