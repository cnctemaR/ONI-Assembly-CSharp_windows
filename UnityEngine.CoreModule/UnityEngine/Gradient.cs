using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Export/Math/Gradient.bindings.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class Gradient : IEquatable<Gradient>
	{
		[FreeFunction(Name = "Gradient_Bindings::Init", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Init();

		[FreeFunction(Name = "Gradient_Bindings::Cleanup", IsThreadSafe = true, HasExplicitThis = true)]
		private void Cleanup()
		{
			IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Gradient.Cleanup_Injected(intPtr);
		}

		[FreeFunction("Gradient_Bindings::Internal_Equals", IsThreadSafe = true, HasExplicitThis = true)]
		private bool Internal_Equals(IntPtr other)
		{
			IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Gradient.Internal_Equals_Injected(intPtr, other);
		}

		[RequiredByNativeCode]
		public Gradient()
		{
			this.m_Ptr = Gradient.Init();
			this.m_RequiresNativeCleanup = true;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.ParticleSystemModule" })]
		internal Gradient(IntPtr ptr)
		{
			this.m_Ptr = ptr;
			this.m_RequiresNativeCleanup = false;
		}

		protected override void Finalize()
		{
			try
			{
				bool requiresNativeCleanup = this.m_RequiresNativeCleanup;
				if (requiresNativeCleanup)
				{
					this.Cleanup();
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		[FreeFunction(Name = "Gradient_Bindings::Evaluate", IsThreadSafe = true, HasExplicitThis = true)]
		public Color Evaluate(float time)
		{
			IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Color color;
			Gradient.Evaluate_Injected(intPtr, time, out color);
			return color;
		}

		public unsafe GradientColorKey[] colorKeys
		{
			[FreeFunction("Gradient_Bindings::GetColorKeysArray", IsThreadSafe = true, HasExplicitThis = true)]
			get
			{
				GradientColorKey[] array2;
				try
				{
					IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					Gradient.get_colorKeys_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					GradientColorKey[] array;
					blittableArrayWrapper.Unmarshal<GradientColorKey>(ref array);
					array2 = array;
				}
				return array2;
			}
			[FreeFunction("Gradient_Bindings::SetColorKeysWithSpan", IsThreadSafe = true, HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<GradientColorKey> span = new Span<GradientColorKey>(value);
				fixed (GradientColorKey* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					Gradient.set_colorKeys_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		public unsafe GradientAlphaKey[] alphaKeys
		{
			[FreeFunction("Gradient_Bindings::GetAlphaKeysArray", IsThreadSafe = true, HasExplicitThis = true)]
			get
			{
				GradientAlphaKey[] array2;
				try
				{
					IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					Gradient.get_alphaKeys_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					GradientAlphaKey[] array;
					blittableArrayWrapper.Unmarshal<GradientAlphaKey>(ref array);
					array2 = array;
				}
				return array2;
			}
			[FreeFunction("Gradient_Bindings::SetAlphaKeysWithSpan", IsThreadSafe = true, HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<GradientAlphaKey> span = new Span<GradientAlphaKey>(value);
				fixed (GradientAlphaKey* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					Gradient.set_alphaKeys_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		public int colorKeyCount
		{
			[FreeFunction("Gradient_Bindings::GetColorKeyCount", IsThreadSafe = true, HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Gradient.get_colorKeyCount_Injected(intPtr);
			}
		}

		public int alphaKeyCount
		{
			[FreeFunction("Gradient_Bindings::GetAlphaKeyCount", IsThreadSafe = true, HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Gradient.get_alphaKeyCount_Injected(intPtr);
			}
		}

		public void GetColorKeys(Span<GradientColorKey> keys)
		{
			bool flag = this.colorKeyCount > keys.Length;
			if (flag)
			{
				throw new ArgumentException("Destination array must be large enough to store the keys", "keys");
			}
			this.GetColorKeysWithSpan(keys);
		}

		public void GetAlphaKeys(Span<GradientAlphaKey> keys)
		{
			bool flag = this.alphaKeyCount > keys.Length;
			if (flag)
			{
				throw new ArgumentException("Destination array must be large enough to store the keys", "keys");
			}
			this.GetAlphaKeysWithSpan(keys);
		}

		[FreeFunction(Name = "Gradient_Bindings::SetColorKeysWithSpan", HasExplicitThis = true, IsThreadSafe = true)]
		public unsafe void SetColorKeys(ReadOnlySpan<GradientColorKey> keys)
		{
			IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<GradientColorKey> readOnlySpan = keys;
			fixed (GradientColorKey* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				Gradient.SetColorKeys_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		[FreeFunction(Name = "Gradient_Bindings::SetAlphaKeysWithSpan", HasExplicitThis = true, IsThreadSafe = true)]
		public unsafe void SetAlphaKeys(ReadOnlySpan<GradientAlphaKey> keys)
		{
			IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<GradientAlphaKey> readOnlySpan = keys;
			fixed (GradientAlphaKey* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				Gradient.SetAlphaKeys_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		[SecurityCritical]
		[FreeFunction(Name = "Gradient_Bindings::GetColorKeysWithSpan", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe void GetColorKeysWithSpan(Span<GradientColorKey> keys)
		{
			IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<GradientColorKey> span = keys;
			fixed (GradientColorKey* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Gradient.GetColorKeysWithSpan_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		[SecurityCritical]
		[FreeFunction(Name = "Gradient_Bindings::GetAlphaKeysWithSpan", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe void GetAlphaKeysWithSpan(Span<GradientAlphaKey> keys)
		{
			IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<GradientAlphaKey> span = keys;
			fixed (GradientAlphaKey* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Gradient.GetAlphaKeysWithSpan_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		[NativeProperty(IsThreadSafe = true)]
		public GradientMode mode
		{
			get
			{
				IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Gradient.get_mode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Gradient.set_mode_Injected(intPtr, value);
			}
		}

		[NativeProperty(IsThreadSafe = true)]
		public ColorSpace colorSpace
		{
			get
			{
				IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Gradient.get_colorSpace_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Gradient.set_colorSpace_Injected(intPtr, value);
			}
		}

		public void SetKeys(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys)
		{
			this.SetKeys(colorKeys.AsSpan<GradientColorKey>(), alphaKeys.AsSpan<GradientAlphaKey>());
		}

		[FreeFunction(Name = "Gradient_Bindings::SetKeysWithSpans", HasExplicitThis = true, IsThreadSafe = true)]
		public unsafe void SetKeys(ReadOnlySpan<GradientColorKey> colorKeys, ReadOnlySpan<GradientAlphaKey> alphaKeys)
		{
			IntPtr intPtr = Gradient.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<GradientColorKey> readOnlySpan = colorKeys;
			fixed (GradientColorKey* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				ReadOnlySpan<GradientAlphaKey> readOnlySpan2 = alphaKeys;
				fixed (GradientAlphaKey* pinnableReference = readOnlySpan2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan2.Length);
					Gradient.SetKeys_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public override bool Equals(object o)
		{
			Gradient gradient = o as Gradient;
			bool flag = gradient != null;
			return flag && this.Equals(gradient);
		}

		public bool Equals(Gradient other)
		{
			bool flag = other == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this == other;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool flag4 = this.m_Ptr.Equals(other.m_Ptr);
					flag2 = flag4 || this.Internal_Equals(other.m_Ptr);
				}
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			return this.m_Ptr.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Cleanup_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Equals_Injected(IntPtr _unity_self, IntPtr other);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Evaluate_Injected(IntPtr _unity_self, float time, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_colorKeys_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_colorKeys_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_alphaKeys_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_alphaKeys_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_colorKeyCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_alphaKeyCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetColorKeys_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keys);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAlphaKeys_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keys);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetColorKeysWithSpan_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keys);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAlphaKeysWithSpan_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keys);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GradientMode get_mode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_mode_Injected(IntPtr _unity_self, GradientMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ColorSpace get_colorSpace_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_colorSpace_Injected(IntPtr _unity_self, ColorSpace value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetKeys_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colorKeys, ref ManagedSpanWrapper alphaKeys);

		[VisibleToOtherModules(new string[] { "UnityEngine.ParticleSystemModule" })]
		internal IntPtr m_Ptr;

		private bool m_RequiresNativeCleanup;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(Gradient graident)
			{
				return graident.m_Ptr;
			}

			public static Gradient ConvertToManaged(IntPtr ptr)
			{
				return new Gradient(ptr);
			}
		}
	}
}
