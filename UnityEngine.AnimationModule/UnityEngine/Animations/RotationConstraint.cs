using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[UsedByNativeCode]
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Modules/Animation/Constraints/RotationConstraint.h")]
	[NativeHeader("Modules/Animation/Constraints/Constraint.bindings.h")]
	public sealed class RotationConstraint : Behaviour, IConstraint, IConstraintInternal
	{
		private RotationConstraint()
		{
			RotationConstraint.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] RotationConstraint self);

		public float weight
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RotationConstraint.get_weight_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RotationConstraint.set_weight_Injected(intPtr, value);
			}
		}

		public Vector3 rotationAtRest
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				RotationConstraint.get_rotationAtRest_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RotationConstraint.set_rotationAtRest_Injected(intPtr, ref value);
			}
		}

		public Vector3 rotationOffset
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				RotationConstraint.get_rotationOffset_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RotationConstraint.set_rotationOffset_Injected(intPtr, ref value);
			}
		}

		public Axis rotationAxis
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RotationConstraint.get_rotationAxis_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RotationConstraint.set_rotationAxis_Injected(intPtr, value);
			}
		}

		public bool constraintActive
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RotationConstraint.get_constraintActive_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RotationConstraint.set_constraintActive_Injected(intPtr, value);
			}
		}

		public bool locked
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RotationConstraint.get_locked_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RotationConstraint.set_locked_Injected(intPtr, value);
			}
		}

		public int sourceCount
		{
			get
			{
				return RotationConstraint.GetSourceCountInternal(this);
			}
		}

		[FreeFunction("ConstraintBindings::GetSourceCount")]
		private static int GetSourceCountInternal([NotNull] RotationConstraint self)
		{
			if (self == null)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(self);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			return RotationConstraint.GetSourceCountInternal_Injected(intPtr);
		}

		[FreeFunction(Name = "ConstraintBindings::GetSources", HasExplicitThis = true)]
		public void GetSources([NotNull] List<ConstraintSource> sources)
		{
			if (sources == null)
			{
				ThrowHelper.ThrowArgumentNullException(sources, "sources");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RotationConstraint.GetSources_Injected(intPtr, sources);
		}

		public void SetSources(List<ConstraintSource> sources)
		{
			bool flag = sources == null;
			if (flag)
			{
				throw new ArgumentNullException("sources");
			}
			RotationConstraint.SetSourcesInternal(this, sources);
		}

		[FreeFunction("ConstraintBindings::SetSources", ThrowsException = true)]
		private static void SetSourcesInternal([NotNull] RotationConstraint self, List<ConstraintSource> sources)
		{
			if (self == null)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(self);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			RotationConstraint.SetSourcesInternal_Injected(intPtr, sources);
		}

		public int AddSource(ConstraintSource source)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RotationConstraint.AddSource_Injected(intPtr, ref source);
		}

		public void RemoveSource(int index)
		{
			this.ValidateSourceIndex(index);
			this.RemoveSourceInternal(index);
		}

		[NativeName("RemoveSource")]
		private void RemoveSourceInternal(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RotationConstraint.RemoveSourceInternal_Injected(intPtr, index);
		}

		public ConstraintSource GetSource(int index)
		{
			this.ValidateSourceIndex(index);
			return this.GetSourceInternal(index);
		}

		[NativeName("GetSource")]
		private ConstraintSource GetSourceInternal(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ConstraintSource constraintSource;
			RotationConstraint.GetSourceInternal_Injected(intPtr, index, out constraintSource);
			return constraintSource;
		}

		public void SetSource(int index, ConstraintSource source)
		{
			this.ValidateSourceIndex(index);
			this.SetSourceInternal(index, source);
		}

		[NativeName("SetSource")]
		private void SetSourceInternal(int index, ConstraintSource source)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RotationConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RotationConstraint.SetSourceInternal_Injected(intPtr, index, ref source);
		}

		private void ValidateSourceIndex(int index)
		{
			bool flag = this.sourceCount == 0;
			if (flag)
			{
				throw new InvalidOperationException("The RotationConstraint component has no sources.");
			}
			bool flag2 = index < 0 || index >= this.sourceCount;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Constraint source index {0} is out of bounds (0-{1}).", index, this.sourceCount));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_weight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_weight_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rotationAtRest_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotationAtRest_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rotationOffset_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotationOffset_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Axis get_rotationAxis_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotationAxis_Injected(IntPtr _unity_self, Axis value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_constraintActive_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_constraintActive_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_locked_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_locked_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSourceCountInternal_Injected(IntPtr self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSources_Injected(IntPtr _unity_self, List<ConstraintSource> sources);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSourcesInternal_Injected(IntPtr self, List<ConstraintSource> sources);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddSource_Injected(IntPtr _unity_self, [In] ref ConstraintSource source);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveSourceInternal_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSourceInternal_Injected(IntPtr _unity_self, int index, out ConstraintSource ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSourceInternal_Injected(IntPtr _unity_self, int index, [In] ref ConstraintSource source);
	}
}
