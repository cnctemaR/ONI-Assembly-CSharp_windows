using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	public struct GCHandle
	{
		private GCHandle(IntPtr h)
		{
			this.handle = h;
		}

		private GCHandle(object obj)
		{
			this = new GCHandle(obj, GCHandleType.Normal);
		}

		internal GCHandle(object value, GCHandleType type)
		{
			if (type < GCHandleType.Weak || type > GCHandleType.Pinned)
			{
				type = GCHandleType.Normal;
			}
			this.handle = GCHandle.GetTargetHandle(value, IntPtr.Zero, type);
		}

		public bool IsAllocated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.handle != IntPtr.Zero;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static object GetRef(IntPtr handle)
		{
			return *Unsafe.As<IntPtr, object>(ref *(IntPtr*)(void*)handle);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void SetRef(IntPtr handle, object value)
		{
			*Unsafe.As<IntPtr, object>(ref *(IntPtr*)(void*)handle) = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool CanDereferenceHandle(IntPtr handle)
		{
			return (handle & (IntPtr)1) == (IntPtr)0;
		}

		public object Target
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (!this.IsAllocated)
				{
					throw new InvalidOperationException("Handle is not allocated");
				}
				if (GCHandle.CanDereferenceHandle(this.handle))
				{
					return GCHandle.GetRef(this.handle);
				}
				return GCHandle.GetTarget(this.handle);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				if (GCHandle.CanDereferenceHandle(this.handle))
				{
					GCHandle.SetRef(this.handle, value);
					return;
				}
				this.handle = GCHandle.GetTargetHandle(value, this.handle, (GCHandleType)(-1));
			}
		}

		public IntPtr AddrOfPinnedObject()
		{
			IntPtr addrOfPinnedObject = GCHandle.GetAddrOfPinnedObject(this.handle);
			if (addrOfPinnedObject == (IntPtr)(-1))
			{
				throw new ArgumentException("Object contains non-primitive or non-blittable data.");
			}
			if (addrOfPinnedObject == (IntPtr)(-2))
			{
				throw new InvalidOperationException("Handle is not pinned.");
			}
			return addrOfPinnedObject;
		}

		public static GCHandle Alloc(object value)
		{
			return new GCHandle(value);
		}

		public static GCHandle Alloc(object value, GCHandleType type)
		{
			return new GCHandle(value, type);
		}

		public void Free()
		{
			IntPtr intPtr = this.handle;
			if (intPtr != IntPtr.Zero && Interlocked.CompareExchange(ref this.handle, IntPtr.Zero, intPtr) == intPtr)
			{
				GCHandle.FreeHandle(intPtr);
				return;
			}
			throw new InvalidOperationException("Handle is not initialized.");
		}

		public static explicit operator IntPtr(GCHandle value)
		{
			return value.handle;
		}

		public static explicit operator GCHandle(IntPtr value)
		{
			if (value == IntPtr.Zero)
			{
				throw new InvalidOperationException("GCHandle value cannot be zero");
			}
			if (!GCHandle.CheckCurrentDomain(value))
			{
				throw new ArgumentException("GCHandle value belongs to a different domain");
			}
			return new GCHandle(value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CheckCurrentDomain(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetTarget(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetTargetHandle(object obj, IntPtr handle, GCHandleType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FreeHandle(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetAddrOfPinnedObject(IntPtr handle);

		public static bool operator ==(GCHandle a, GCHandle b)
		{
			return a.handle == b.handle;
		}

		public static bool operator !=(GCHandle a, GCHandle b)
		{
			return !(a == b);
		}

		public override bool Equals(object o)
		{
			return o is GCHandle && this == (GCHandle)o;
		}

		public override int GetHashCode()
		{
			return this.handle.GetHashCode();
		}

		public static GCHandle FromIntPtr(IntPtr value)
		{
			return (GCHandle)value;
		}

		public static IntPtr ToIntPtr(GCHandle value)
		{
			return (IntPtr)value;
		}

		private IntPtr handle;
	}
}
