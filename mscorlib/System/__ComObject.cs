using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Mono.Interop;

namespace System
{
	[StructLayout(LayoutKind.Sequential)]
	internal class __ComObject : MarshalByRefObject
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern __ComObject CreateRCW(Type t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ReleaseInterfaces();

		~__ComObject()
		{
			if (this.hash_table != IntPtr.Zero)
			{
				if (this.synchronization_context != null)
				{
					this.synchronization_context.Post(delegate(object state)
					{
						this.ReleaseInterfaces();
					}, this);
				}
				else
				{
					this.ReleaseInterfaces();
				}
			}
			this.proxy = null;
		}

		public __ComObject()
		{
			this.Initialize(base.GetType());
		}

		internal __ComObject(Type t)
		{
			this.Initialize(t);
		}

		internal __ComObject(IntPtr pItf, ComInteropProxy p)
		{
			this.proxy = p;
			this.InitializeApartmentDetails();
			Guid iid_IUnknown = __ComObject.IID_IUnknown;
			Marshal.ThrowExceptionForHR(Marshal.QueryInterface(pItf, ref iid_IUnknown, out this.iunknown));
		}

		internal void Initialize(IntPtr pUnk, ComInteropProxy p)
		{
			this.proxy = p;
			this.InitializeApartmentDetails();
			this.iunknown = pUnk;
		}

		internal void Initialize(Type t)
		{
			this.InitializeApartmentDetails();
			if (this.iunknown != IntPtr.Zero)
			{
				return;
			}
			this.iunknown = __ComObject.CreateIUnknown(t);
		}

		internal static IntPtr CreateIUnknown(Type t)
		{
			RuntimeHelpers.RunClassConstructor(t.TypeHandle);
			ObjectCreationDelegate objectCreationCallback = ExtensibleClassFactory.GetObjectCreationCallback(t);
			IntPtr intPtr;
			if (objectCreationCallback != null)
			{
				intPtr = objectCreationCallback(IntPtr.Zero);
				if (intPtr == IntPtr.Zero)
				{
					throw new COMException(string.Format("ObjectCreationDelegate for type {0} failed to return a valid COM object", t));
				}
			}
			else
			{
				Marshal.ThrowExceptionForHR(__ComObject.CoCreateInstance(__ComObject.GetCLSID(t), IntPtr.Zero, 21U, __ComObject.IID_IUnknown, out intPtr));
			}
			return intPtr;
		}

		private void InitializeApartmentDetails()
		{
			if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
			{
				return;
			}
			this.synchronization_context = SynchronizationContext.Current;
			if (this.synchronization_context != null && this.synchronization_context.GetType() == typeof(SynchronizationContext))
			{
				this.synchronization_context = null;
			}
		}

		private static Guid GetCLSID(Type t)
		{
			if (t.IsImport)
			{
				return t.GUID;
			}
			Type type = t.BaseType;
			while (type != typeof(object))
			{
				if (type.IsImport)
				{
					return type.GUID;
				}
				type = type.BaseType;
			}
			throw new COMException("Could not find base COM type for type " + t.ToString());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr GetInterfaceInternal(Type t, bool throwException);

		internal IntPtr GetInterface(Type t, bool throwException)
		{
			this.CheckIUnknown();
			return this.GetInterfaceInternal(t, throwException);
		}

		internal IntPtr GetInterface(Type t)
		{
			return this.GetInterface(t, true);
		}

		private void CheckIUnknown()
		{
			if (this.iunknown == IntPtr.Zero)
			{
				throw new InvalidComObjectException("COM object that has been separated from its underlying RCW cannot be used.");
			}
		}

		internal IntPtr IUnknown
		{
			get
			{
				if (this.iunknown == IntPtr.Zero)
				{
					throw new InvalidComObjectException("COM object that has been separated from its underlying RCW cannot be used.");
				}
				return this.iunknown;
			}
		}

		internal IntPtr IDispatch
		{
			get
			{
				IntPtr @interface = this.GetInterface(typeof(IDispatch));
				if (@interface == IntPtr.Zero)
				{
					throw new InvalidComObjectException("COM object that has been separated from its underlying RCW cannot be used.");
				}
				return @interface;
			}
		}

		internal static Guid IID_IUnknown
		{
			get
			{
				return new Guid("00000000-0000-0000-C000-000000000046");
			}
		}

		internal static Guid IID_IDispatch
		{
			get
			{
				return new Guid("00020400-0000-0000-C000-000000000046");
			}
		}

		public override bool Equals(object obj)
		{
			this.CheckIUnknown();
			if (obj == null)
			{
				return false;
			}
			__ComObject _ComObject = obj as __ComObject;
			return _ComObject != null && this.iunknown == _ComObject.IUnknown;
		}

		public override int GetHashCode()
		{
			this.CheckIUnknown();
			return this.iunknown.ToInt32();
		}

		[DllImport("ole32.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
		private static extern int CoCreateInstance([MarshalAs(UnmanagedType.LPStruct)] [In] Guid rclsid, IntPtr pUnkOuter, uint dwClsContext, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid riid, out IntPtr pUnk);

		private IntPtr iunknown;

		private IntPtr hash_table;

		private SynchronizationContext synchronization_context;

		private ComInteropProxy proxy;
	}
}
