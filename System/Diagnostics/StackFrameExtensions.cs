using System;
using System.ComponentModel;
using Unity;

namespace System.Diagnostics
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class StackFrameExtensions
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IntPtr GetNativeImageBase(this StackFrame stackFrame)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return 0;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IntPtr GetNativeIP(this StackFrame stackFrame)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return 0;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool HasILOffset(this StackFrame stackFrame)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool HasMethod(this StackFrame stackFrame)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool HasNativeImage(this StackFrame stackFrame)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool HasSource(this StackFrame stackFrame)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}
	}
}
