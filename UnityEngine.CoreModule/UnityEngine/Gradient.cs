using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Gradient.bindings.h")]
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class Gradient : IEquatable<Gradient>
	{
		[RequiredByNativeCode]
		public Gradient()
		{
			this.m_Ptr = Gradient.Init();
		}

		[FreeFunction(Name = "Gradient_Bindings::Init", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Init();

		[FreeFunction(Name = "Gradient_Bindings::Cleanup", IsThreadSafe = true, HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Cleanup();

		[FreeFunction("Gradient_Bindings::Internal_Equals", IsThreadSafe = true, HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_Equals(IntPtr other);

		~Gradient()
		{
			this.Cleanup();
		}

		[FreeFunction(Name = "Gradient_Bindings::Evaluate", IsThreadSafe = true, HasExplicitThis = true)]
		public Color Evaluate(float time)
		{
			Color color;
			this.Evaluate_Injected(time, out color);
			return color;
		}

		public extern GradientColorKey[] colorKeys
		{
			[FreeFunction("Gradient_Bindings::GetColorKeys", IsThreadSafe = true, HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("Gradient_Bindings::SetColorKeys", IsThreadSafe = true, HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern GradientAlphaKey[] alphaKeys
		{
			[FreeFunction("Gradient_Bindings::GetAlphaKeys", IsThreadSafe = true, HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("Gradient_Bindings::SetAlphaKeys", IsThreadSafe = true, HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern GradientMode mode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction(Name = "Gradient_Bindings::SetKeys", IsThreadSafe = true, HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetKeys(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys);

		public override bool Equals(object o)
		{
			return !object.ReferenceEquals(null, o) && (object.ReferenceEquals(this, o) || (o.GetType() == base.GetType() && this.Equals((Gradient)o)));
		}

		public bool Equals(Gradient other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || this.m_Ptr.Equals(other.m_Ptr) || this.Internal_Equals(other.m_Ptr));
		}

		public override int GetHashCode()
		{
			return this.m_Ptr.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Evaluate_Injected(float time, out Color ret);

		internal IntPtr m_Ptr;
	}
}
