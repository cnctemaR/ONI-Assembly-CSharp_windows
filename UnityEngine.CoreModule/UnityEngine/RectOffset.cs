using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/IMGUI/GUIStyle.h")]
	[UsedByNativeCode]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class RectOffset : IFormattable
	{
		public RectOffset()
		{
			this.m_Ptr = RectOffset.InternalCreate();
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
		internal RectOffset(object sourceStyle, IntPtr source)
		{
			this.m_SourceStyle = sourceStyle;
			this.m_Ptr = source;
		}

		protected override void Finalize()
		{
			try
			{
				bool flag = this.m_SourceStyle == null;
				if (flag)
				{
					this.Destroy();
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		public RectOffset(int left, int right, int top, int bottom)
		{
			this.m_Ptr = RectOffset.InternalCreate();
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return this.ToString(null, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			bool flag = formatProvider == null;
			if (flag)
			{
				formatProvider = CultureInfo.InvariantCulture.NumberFormat;
			}
			return string.Format("RectOffset (l:{0} r:{1} t:{2} b:{3})", new object[]
			{
				this.left.ToString(format, formatProvider),
				this.right.ToString(format, formatProvider),
				this.top.ToString(format, formatProvider),
				this.bottom.ToString(format, formatProvider)
			});
		}

		private void Destroy()
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				RectOffset.InternalDestroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InternalCreate();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalDestroy(IntPtr ptr);

		[NativeProperty("left", false, TargetType.Field)]
		public int left
		{
			get
			{
				IntPtr intPtr = RectOffset.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RectOffset.get_left_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = RectOffset.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RectOffset.set_left_Injected(intPtr, value);
			}
		}

		[NativeProperty("right", false, TargetType.Field)]
		public int right
		{
			get
			{
				IntPtr intPtr = RectOffset.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RectOffset.get_right_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = RectOffset.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RectOffset.set_right_Injected(intPtr, value);
			}
		}

		[NativeProperty("top", false, TargetType.Field)]
		public int top
		{
			get
			{
				IntPtr intPtr = RectOffset.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RectOffset.get_top_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = RectOffset.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RectOffset.set_top_Injected(intPtr, value);
			}
		}

		[NativeProperty("bottom", false, TargetType.Field)]
		public int bottom
		{
			get
			{
				IntPtr intPtr = RectOffset.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RectOffset.get_bottom_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = RectOffset.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RectOffset.set_bottom_Injected(intPtr, value);
			}
		}

		public int horizontal
		{
			get
			{
				IntPtr intPtr = RectOffset.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RectOffset.get_horizontal_Injected(intPtr);
			}
		}

		public int vertical
		{
			get
			{
				IntPtr intPtr = RectOffset.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RectOffset.get_vertical_Injected(intPtr);
			}
		}

		public Rect Add(Rect rect)
		{
			IntPtr intPtr = RectOffset.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rect rect2;
			RectOffset.Add_Injected(intPtr, ref rect, out rect2);
			return rect2;
		}

		public Rect Remove(Rect rect)
		{
			IntPtr intPtr = RectOffset.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rect rect2;
			RectOffset.Remove_Injected(intPtr, ref rect, out rect2);
			return rect2;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_left_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_left_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_right_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_right_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_top_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_top_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_bottom_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bottom_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_horizontal_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_vertical_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Add_Injected(IntPtr _unity_self, [In] ref Rect rect, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Remove_Injected(IntPtr _unity_self, [In] ref Rect rect, out Rect ret);

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
		[NonSerialized]
		internal IntPtr m_Ptr;

		private readonly object m_SourceStyle;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(RectOffset rectOffset)
			{
				return rectOffset.m_Ptr;
			}
		}
	}
}
