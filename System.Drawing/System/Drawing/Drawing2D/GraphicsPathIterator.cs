using System;
using System.Drawing.Internal;
using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D
{
	public sealed class GraphicsPathIterator : MarshalByRefObject, IDisposable
	{
		public GraphicsPathIterator(GraphicsPath path)
		{
			IntPtr zero = IntPtr.Zero;
			int num = GDIPlus.GdipCreatePathIter(out zero, new HandleRef(path, (path == null) ? IntPtr.Zero : path.nativePath));
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			this.nativeIter = zero;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (this.nativeIter != IntPtr.Zero)
			{
				try
				{
					GDIPlus.GdipDeletePathIter(new HandleRef(this, this.nativeIter));
				}
				catch (Exception ex)
				{
					if (ClientUtils.IsSecurityOrCriticalException(ex))
					{
						throw;
					}
				}
				finally
				{
					this.nativeIter = IntPtr.Zero;
				}
			}
		}

		~GraphicsPathIterator()
		{
			this.Dispose(false);
		}

		public int NextSubpath(out int startIndex, out int endIndex, out bool isClosed)
		{
			int num2;
			int num3;
			int num4;
			int num = GDIPlus.GdipPathIterNextSubpath(new HandleRef(this, this.nativeIter), out num2, out num3, out num4, out isClosed);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			startIndex = num3;
			endIndex = num4;
			return num2;
		}

		public int NextSubpath(GraphicsPath path, out bool isClosed)
		{
			int num2;
			int num = GDIPlus.GdipPathIterNextSubpathPath(new HandleRef(this, this.nativeIter), out num2, new HandleRef(path, (path == null) ? IntPtr.Zero : path.nativePath), out isClosed);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return num2;
		}

		public int NextPathType(out byte pathType, out int startIndex, out int endIndex)
		{
			int num2;
			int num = GDIPlus.GdipPathIterNextPathType(new HandleRef(this, this.nativeIter), out num2, out pathType, out startIndex, out endIndex);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return num2;
		}

		public int NextMarker(out int startIndex, out int endIndex)
		{
			int num2;
			int num = GDIPlus.GdipPathIterNextMarker(new HandleRef(this, this.nativeIter), out num2, out startIndex, out endIndex);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return num2;
		}

		public int NextMarker(GraphicsPath path)
		{
			int num2;
			int num = GDIPlus.GdipPathIterNextMarkerPath(new HandleRef(this, this.nativeIter), out num2, new HandleRef(path, (path == null) ? IntPtr.Zero : path.nativePath));
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return num2;
		}

		public int Count
		{
			get
			{
				int num2;
				int num = GDIPlus.GdipPathIterGetCount(new HandleRef(this, this.nativeIter), out num2);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
				return num2;
			}
		}

		public int SubpathCount
		{
			get
			{
				int num2;
				int num = GDIPlus.GdipPathIterGetSubpathCount(new HandleRef(this, this.nativeIter), out num2);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
				return num2;
			}
		}

		public bool HasCurve()
		{
			bool flag;
			int num = GDIPlus.GdipPathIterHasCurve(new HandleRef(this, this.nativeIter), out flag);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return flag;
		}

		public void Rewind()
		{
			int num = GDIPlus.GdipPathIterRewind(new HandleRef(this, this.nativeIter));
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		public int Enumerate(ref PointF[] points, ref byte[] types)
		{
			if (points.Length != types.Length)
			{
				throw SafeNativeMethods.Gdip.StatusException(2);
			}
			int num = 0;
			int num2 = Marshal.SizeOf(typeof(GPPOINTF));
			int num3 = points.Length;
			byte[] array = new byte[num3];
			IntPtr intPtr = Marshal.AllocHGlobal(checked(num3 * num2));
			try
			{
				int num4 = GDIPlus.GdipPathIterEnumerate(new HandleRef(this, this.nativeIter), out num, intPtr, array, num3);
				if (num4 != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num4);
				}
				if (num < num3)
				{
					SafeNativeMethods.ZeroMemory(checked((long)intPtr + num * num2), (ulong)((long)((num3 - num) * num2)));
				}
				points = SafeNativeMethods.Gdip.ConvertGPPOINTFArrayF(intPtr, num3);
				array.CopyTo(types, 0);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return num;
		}

		public int CopyData(ref PointF[] points, ref byte[] types, int startIndex, int endIndex)
		{
			if (points.Length != types.Length || endIndex - startIndex + 1 > points.Length)
			{
				throw SafeNativeMethods.Gdip.StatusException(2);
			}
			int num = 0;
			int num2 = Marshal.SizeOf(typeof(GPPOINTF));
			int num3 = points.Length;
			byte[] array = new byte[num3];
			IntPtr intPtr = Marshal.AllocHGlobal(checked(num3 * num2));
			try
			{
				int num4 = GDIPlus.GdipPathIterCopyData(new HandleRef(this, this.nativeIter), out num, intPtr, array, startIndex, endIndex);
				if (num4 != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num4);
				}
				if (num < num3)
				{
					SafeNativeMethods.ZeroMemory(checked((long)intPtr + num * num2), (ulong)((long)((num3 - num) * num2)));
				}
				points = SafeNativeMethods.Gdip.ConvertGPPOINTFArrayF(intPtr, num3);
				array.CopyTo(types, 0);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return num;
		}

		internal IntPtr nativeIter;
	}
}
