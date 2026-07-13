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
			return UnityString.Format("RectOffset (l:{0} r:{1} t:{2} b:{3})", new object[]
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
		public extern int left
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("right", false, TargetType.Field)]
		public extern int right
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("top", false, TargetType.Field)]
		public extern int top
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("bottom", false, TargetType.Field)]
		public extern int bottom
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int horizontal
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int vertical
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Rect Add(Rect rect)
		{
			Rect rect2;
			this.Add_Injected(ref rect, out rect2);
			return rect2;
		}

		public Rect Remove(Rect rect)
		{
			Rect rect2;
			this.Remove_Injected(ref rect, out rect2);
			return rect2;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Add_Injected(ref Rect rect, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Remove_Injected(ref Rect rect, out Rect ret);

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
		[NonSerialized]
		internal IntPtr m_Ptr;

		private readonly object m_SourceStyle;
	}
}
