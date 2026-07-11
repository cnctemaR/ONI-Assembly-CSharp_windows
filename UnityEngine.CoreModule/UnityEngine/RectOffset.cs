using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/IMGUI/GUIStyle.h")]
	[NativeHeader("Runtime/Camera/RenderLayers/GUILayer.h")]
	[UsedByNativeCode]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class RectOffset
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

		public RectOffset(int left, int right, int top, int bottom)
		{
			this.m_Ptr = RectOffset.InternalCreate();
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
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

		~RectOffset()
		{
			if (this.m_SourceStyle == null)
			{
				this.Destroy();
			}
		}

		public override string ToString()
		{
			return UnityString.Format("RectOffset (l:{0} r:{1} t:{2} b:{3})", new object[] { this.left, this.right, this.top, this.bottom });
		}

		private void Destroy()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				RectOffset.InternalDestroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
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
