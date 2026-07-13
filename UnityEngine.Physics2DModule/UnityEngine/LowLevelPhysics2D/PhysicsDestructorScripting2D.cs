using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Bindings;

namespace UnityEngine.LowLevelPhysics2D
{
	[StaticAccessor("PhysicsDestructor2D", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/Physics2D/LowLevel/PhysicsDestructor2D.h")]
	internal static class PhysicsDestructorScripting2D
	{
		[NativeMethod(Name = "Fragment", IsThreadSafe = true)]
		internal unsafe static PhysicsDestructor.FragmentResult PhysicsDestructor_Fragment(PhysicsDestructor.FragmentGeometry target, ReadOnlySpan<Vector2> fragmentPoints, Allocator allocator)
		{
			ReadOnlySpan<Vector2> readOnlySpan = fragmentPoints;
			PhysicsDestructor.FragmentResult fragmentResult;
			fixed (Vector2* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				PhysicsDestructorScripting2D.PhysicsDestructor_Fragment_Injected(ref target, ref managedSpanWrapper, allocator, out fragmentResult);
			}
			return fragmentResult;
		}

		[NativeMethod(Name = "FragmentMasked", IsThreadSafe = true)]
		internal unsafe static PhysicsDestructor.FragmentResult PhysicsDestructor_FragmentMasked(PhysicsDestructor.FragmentGeometry target, PhysicsDestructor.FragmentGeometry mask, ReadOnlySpan<Vector2> fragmentPoints, Allocator allocator)
		{
			ReadOnlySpan<Vector2> readOnlySpan = fragmentPoints;
			PhysicsDestructor.FragmentResult fragmentResult;
			fixed (Vector2* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				PhysicsDestructorScripting2D.PhysicsDestructor_FragmentMasked_Injected(ref target, ref mask, ref managedSpanWrapper, allocator, out fragmentResult);
			}
			return fragmentResult;
		}

		[NativeMethod(Name = "Slice", IsThreadSafe = true)]
		internal static PhysicsDestructor.SliceResult PhysicsDestructor_Slice(PhysicsDestructor.FragmentGeometry target, Vector2 origin, Vector2 translation, Allocator allocator)
		{
			PhysicsDestructor.SliceResult sliceResult;
			PhysicsDestructorScripting2D.PhysicsDestructor_Slice_Injected(ref target, ref origin, ref translation, allocator, out sliceResult);
			return sliceResult;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PhysicsDestructor_Fragment_Injected([In] ref PhysicsDestructor.FragmentGeometry target, ref ManagedSpanWrapper fragmentPoints, Allocator allocator, out PhysicsDestructor.FragmentResult ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PhysicsDestructor_FragmentMasked_Injected([In] ref PhysicsDestructor.FragmentGeometry target, [In] ref PhysicsDestructor.FragmentGeometry mask, ref ManagedSpanWrapper fragmentPoints, Allocator allocator, out PhysicsDestructor.FragmentResult ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PhysicsDestructor_Slice_Injected([In] ref PhysicsDestructor.FragmentGeometry target, [In] ref Vector2 origin, [In] ref Vector2 translation, Allocator allocator, out PhysicsDestructor.SliceResult ret);
	}
}
