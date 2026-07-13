using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[NativeHeader("Modules/Animation/Constraints/Constraint.bindings.h")]
	[NativeHeader("Modules/Animation/Constraints/ParentConstraint.h")]
	[RequireComponent(typeof(Transform))]
	[UsedByNativeCode]
	public sealed class ParentConstraint : Behaviour, IConstraint, IConstraintInternal
	{
		private ParentConstraint()
		{
			ParentConstraint.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] ParentConstraint self);

		public float weight
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParentConstraint.get_weight_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParentConstraint.set_weight_Injected(intPtr, value);
			}
		}

		public bool constraintActive
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParentConstraint.get_constraintActive_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParentConstraint.set_constraintActive_Injected(intPtr, value);
			}
		}

		public bool locked
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParentConstraint.get_locked_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParentConstraint.set_locked_Injected(intPtr, value);
			}
		}

		public int sourceCount
		{
			get
			{
				return ParentConstraint.GetSourceCountInternal(this);
			}
		}

		[FreeFunction("ConstraintBindings::GetSourceCount")]
		private static int GetSourceCountInternal([NotNull] ParentConstraint self)
		{
			if (self == null)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(self);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			return ParentConstraint.GetSourceCountInternal_Injected(intPtr);
		}

		public Vector3 translationAtRest
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ParentConstraint.get_translationAtRest_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParentConstraint.set_translationAtRest_Injected(intPtr, ref value);
			}
		}

		public Vector3 rotationAtRest
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ParentConstraint.get_rotationAtRest_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParentConstraint.set_rotationAtRest_Injected(intPtr, ref value);
			}
		}

		public unsafe Vector3[] translationOffsets
		{
			get
			{
				Vector3[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					ParentConstraint.get_translationOffsets_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					Vector3[] array;
					blittableArrayWrapper.Unmarshal<Vector3>(ref array);
					array2 = array;
				}
				return array2;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<Vector3> span = new Span<Vector3>(value);
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					ParentConstraint.set_translationOffsets_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		public unsafe Vector3[] rotationOffsets
		{
			get
			{
				Vector3[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					ParentConstraint.get_rotationOffsets_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					Vector3[] array;
					blittableArrayWrapper.Unmarshal<Vector3>(ref array);
					array2 = array;
				}
				return array2;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<Vector3> span = new Span<Vector3>(value);
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					ParentConstraint.set_rotationOffsets_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		public Axis translationAxis
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParentConstraint.get_translationAxis_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParentConstraint.set_translationAxis_Injected(intPtr, value);
			}
		}

		public Axis rotationAxis
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParentConstraint.get_rotationAxis_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParentConstraint.set_rotationAxis_Injected(intPtr, value);
			}
		}

		public Vector3 GetTranslationOffset(int index)
		{
			this.ValidateSourceIndex(index);
			return this.GetTranslationOffsetInternal(index);
		}

		public void SetTranslationOffset(int index, Vector3 value)
		{
			this.ValidateSourceIndex(index);
			this.SetTranslationOffsetInternal(index, value);
		}

		[NativeName("GetTranslationOffset")]
		private Vector3 GetTranslationOffsetInternal(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			ParentConstraint.GetTranslationOffsetInternal_Injected(intPtr, index, out vector);
			return vector;
		}

		[NativeName("SetTranslationOffset")]
		private void SetTranslationOffsetInternal(int index, Vector3 value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParentConstraint.SetTranslationOffsetInternal_Injected(intPtr, index, ref value);
		}

		public Vector3 GetRotationOffset(int index)
		{
			this.ValidateSourceIndex(index);
			return this.GetRotationOffsetInternal(index);
		}

		public void SetRotationOffset(int index, Vector3 value)
		{
			this.ValidateSourceIndex(index);
			this.SetRotationOffsetInternal(index, value);
		}

		[NativeName("GetRotationOffset")]
		private Vector3 GetRotationOffsetInternal(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			ParentConstraint.GetRotationOffsetInternal_Injected(intPtr, index, out vector);
			return vector;
		}

		[NativeName("SetRotationOffset")]
		private void SetRotationOffsetInternal(int index, Vector3 value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParentConstraint.SetRotationOffsetInternal_Injected(intPtr, index, ref value);
		}

		private void ValidateSourceIndex(int index)
		{
			bool flag = this.sourceCount == 0;
			if (flag)
			{
				throw new InvalidOperationException("The ParentConstraint component has no sources.");
			}
			bool flag2 = index < 0 || index >= this.sourceCount;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Constraint source index {0} is out of bounds (0-{1}).", index, this.sourceCount));
			}
		}

		[FreeFunction(Name = "ConstraintBindings::GetSources", HasExplicitThis = true)]
		public void GetSources([NotNull] List<ConstraintSource> sources)
		{
			if (sources == null)
			{
				ThrowHelper.ThrowArgumentNullException(sources, "sources");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParentConstraint.GetSources_Injected(intPtr, sources);
		}

		public void SetSources(List<ConstraintSource> sources)
		{
			bool flag = sources == null;
			if (flag)
			{
				throw new ArgumentNullException("sources");
			}
			ParentConstraint.SetSourcesInternal(this, sources);
		}

		[FreeFunction("ConstraintBindings::SetSources", ThrowsException = true)]
		private static void SetSourcesInternal([NotNull] ParentConstraint self, List<ConstraintSource> sources)
		{
			if (self == null)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(self);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			ParentConstraint.SetSourcesInternal_Injected(intPtr, sources);
		}

		public int AddSource(ConstraintSource source)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ParentConstraint.AddSource_Injected(intPtr, ref source);
		}

		public void RemoveSource(int index)
		{
			this.ValidateSourceIndex(index);
			this.RemoveSourceInternal(index);
		}

		[NativeName("RemoveSource")]
		private void RemoveSourceInternal(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParentConstraint.RemoveSourceInternal_Injected(intPtr, index);
		}

		public ConstraintSource GetSource(int index)
		{
			this.ValidateSourceIndex(index);
			return this.GetSourceInternal(index);
		}

		[NativeName("GetSource")]
		private ConstraintSource GetSourceInternal(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ConstraintSource constraintSource;
			ParentConstraint.GetSourceInternal_Injected(intPtr, index, out constraintSource);
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
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParentConstraint>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParentConstraint.SetSourceInternal_Injected(intPtr, index, ref source);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_weight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_weight_Injected(IntPtr _unity_self, float value);

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
		private static extern void get_translationAtRest_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_translationAtRest_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rotationAtRest_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotationAtRest_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_translationOffsets_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_translationOffsets_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rotationOffsets_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotationOffsets_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Axis get_translationAxis_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_translationAxis_Injected(IntPtr _unity_self, Axis value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Axis get_rotationAxis_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotationAxis_Injected(IntPtr _unity_self, Axis value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTranslationOffsetInternal_Injected(IntPtr _unity_self, int index, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTranslationOffsetInternal_Injected(IntPtr _unity_self, int index, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRotationOffsetInternal_Injected(IntPtr _unity_self, int index, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRotationOffsetInternal_Injected(IntPtr _unity_self, int index, [In] ref Vector3 value);

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
