using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[NativeHeader("Modules/Animation/Constraints/Constraint.bindings.h")]
	[NativeHeader("Modules/Animation/Constraints/LookAtConstraint.h")]
	[RequireComponent(typeof(Transform))]
	[UsedByNativeCode]
	public sealed class LookAtConstraint : Behaviour, IConstraint, IConstraintInternal
	{
		private LookAtConstraint()
		{
			LookAtConstraint.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] LookAtConstraint self);

		public float weight
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LookAtConstraint.get_weight_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LookAtConstraint.set_weight_Injected(intPtr, value);
			}
		}

		public float roll
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LookAtConstraint.get_roll_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LookAtConstraint.set_roll_Injected(intPtr, value);
			}
		}

		public bool constraintActive
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LookAtConstraint.get_constraintActive_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LookAtConstraint.set_constraintActive_Injected(intPtr, value);
			}
		}

		public bool locked
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LookAtConstraint.get_locked_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LookAtConstraint.set_locked_Injected(intPtr, value);
			}
		}

		public Vector3 rotationAtRest
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				LookAtConstraint.get_rotationAtRest_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LookAtConstraint.set_rotationAtRest_Injected(intPtr, ref value);
			}
		}

		public Vector3 rotationOffset
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				LookAtConstraint.get_rotationOffset_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LookAtConstraint.set_rotationOffset_Injected(intPtr, ref value);
			}
		}

		public Transform worldUpObject
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Transform>(LookAtConstraint.get_worldUpObject_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LookAtConstraint.set_worldUpObject_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Transform>(value));
			}
		}

		public bool useUpObject
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LookAtConstraint.get_useUpObject_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LookAtConstraint.set_useUpObject_Injected(intPtr, value);
			}
		}

		public int sourceCount
		{
			get
			{
				return LookAtConstraint.GetSourceCountInternal(this);
			}
		}

		[FreeFunction("ConstraintBindings::GetSourceCount")]
		private static int GetSourceCountInternal([NotNull] LookAtConstraint self)
		{
			if (self == null)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(self);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			return LookAtConstraint.GetSourceCountInternal_Injected(intPtr);
		}

		[FreeFunction(Name = "ConstraintBindings::GetSources", HasExplicitThis = true)]
		public void GetSources([NotNull] List<ConstraintSource> sources)
		{
			if (sources == null)
			{
				ThrowHelper.ThrowArgumentNullException(sources, "sources");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			LookAtConstraint.GetSources_Injected(intPtr, sources);
		}

		public void SetSources(List<ConstraintSource> sources)
		{
			bool flag = sources == null;
			if (flag)
			{
				throw new ArgumentNullException("sources");
			}
			LookAtConstraint.SetSourcesInternal(this, sources);
		}

		[FreeFunction("ConstraintBindings::SetSources", ThrowsException = true)]
		private static void SetSourcesInternal([NotNull] LookAtConstraint self, List<ConstraintSource> sources)
		{
			if (self == null)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(self);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			LookAtConstraint.SetSourcesInternal_Injected(intPtr, sources);
		}

		public int AddSource(ConstraintSource source)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return LookAtConstraint.AddSource_Injected(intPtr, ref source);
		}

		public void RemoveSource(int index)
		{
			this.ValidateSourceIndex(index);
			this.RemoveSourceInternal(index);
		}

		[NativeName("RemoveSource")]
		private void RemoveSourceInternal(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			LookAtConstraint.RemoveSourceInternal_Injected(intPtr, index);
		}

		public ConstraintSource GetSource(int index)
		{
			this.ValidateSourceIndex(index);
			return this.GetSourceInternal(index);
		}

		[NativeName("GetSource")]
		private ConstraintSource GetSourceInternal(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ConstraintSource constraintSource;
			LookAtConstraint.GetSourceInternal_Injected(intPtr, index, out constraintSource);
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
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LookAtConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			LookAtConstraint.SetSourceInternal_Injected(intPtr, index, ref source);
		}

		private void ValidateSourceIndex(int index)
		{
			bool flag = this.sourceCount == 0;
			if (flag)
			{
				throw new InvalidOperationException("The LookAtConstraint component has no sources.");
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
		private static extern float get_roll_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_roll_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_constraintActive_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_constraintActive_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_locked_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_locked_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rotationAtRest_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotationAtRest_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rotationOffset_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotationOffset_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_worldUpObject_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_worldUpObject_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useUpObject_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useUpObject_Injected(IntPtr _unity_self, bool value);

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
