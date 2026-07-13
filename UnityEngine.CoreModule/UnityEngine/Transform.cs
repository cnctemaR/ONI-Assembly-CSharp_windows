using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Transform/Transform.h")]
	[NativeHeader("Runtime/Transform/ScriptBindings/TransformScriptBindings.h")]
	[RequiredByNativeCode]
	[NativeHeader("Configuration/UnityConfigure.h")]
	public class Transform : Component, IEnumerable
	{
		protected Transform()
		{
		}

		public Vector3 position
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Transform.get_position_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Transform.set_position_Injected(intPtr, ref value);
			}
		}

		public Vector3 localPosition
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Transform.get_localPosition_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Transform.set_localPosition_Injected(intPtr, ref value);
			}
		}

		internal Vector3 GetLocalEulerAngles(RotationOrder order)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Transform.GetLocalEulerAngles_Injected(intPtr, order, out vector);
			return vector;
		}

		internal void SetLocalEulerAngles(Vector3 euler, RotationOrder order)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.SetLocalEulerAngles_Injected(intPtr, ref euler, order);
		}

		[NativeConditional("UNITY_EDITOR")]
		internal void SetLocalEulerHint(Vector3 euler)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.SetLocalEulerHint_Injected(intPtr, ref euler);
		}

		public Vector3 eulerAngles
		{
			get
			{
				return this.rotation.eulerAngles;
			}
			set
			{
				this.rotation = Quaternion.Euler(value);
			}
		}

		public Vector3 localEulerAngles
		{
			get
			{
				return this.localRotation.eulerAngles;
			}
			set
			{
				this.localRotation = Quaternion.Euler(value);
			}
		}

		public Vector3 right
		{
			get
			{
				return this.rotation * Vector3.right;
			}
			set
			{
				this.rotation = Quaternion.FromToRotation(Vector3.right, value);
			}
		}

		public Vector3 up
		{
			get
			{
				return this.rotation * Vector3.up;
			}
			set
			{
				this.rotation = Quaternion.FromToRotation(Vector3.up, value);
			}
		}

		public Vector3 forward
		{
			get
			{
				return this.rotation * Vector3.forward;
			}
			set
			{
				this.rotation = Quaternion.LookRotation(value);
			}
		}

		public Quaternion rotation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Quaternion quaternion;
				Transform.get_rotation_Injected(intPtr, out quaternion);
				return quaternion;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Transform.set_rotation_Injected(intPtr, ref value);
			}
		}

		public Quaternion localRotation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Quaternion quaternion;
				Transform.get_localRotation_Injected(intPtr, out quaternion);
				return quaternion;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Transform.set_localRotation_Injected(intPtr, ref value);
			}
		}

		[NativeConditional("UNITY_EDITOR")]
		internal RotationOrder rotationOrder
		{
			get
			{
				return (RotationOrder)this.GetRotationOrderInternal();
			}
			set
			{
				this.SetRotationOrderInternal(value);
			}
		}

		[NativeMethod("GetRotationOrder")]
		[NativeConditional("UNITY_EDITOR")]
		internal int GetRotationOrderInternal()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Transform.GetRotationOrderInternal_Injected(intPtr);
		}

		[NativeMethod("SetRotationOrder")]
		[NativeConditional("UNITY_EDITOR")]
		internal void SetRotationOrderInternal(RotationOrder rotationOrder)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.SetRotationOrderInternal_Injected(intPtr, rotationOrder);
		}

		public Vector3 localScale
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Transform.get_localScale_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Transform.set_localScale_Injected(intPtr, ref value);
			}
		}

		public Transform parent
		{
			get
			{
				return this.parentInternal;
			}
			set
			{
				bool flag = this is RectTransform;
				if (flag)
				{
					Debug.LogWarning("Parent of RectTransform is being set with parent property. Consider using the SetParent method instead, with the worldPositionStays argument set to false. This will retain local orientation and scale rather than world orientation and scale, which can prevent common UI scaling issues.", this);
				}
				this.parentInternal = value;
			}
		}

		internal Transform parentInternal
		{
			get
			{
				return this.GetParent();
			}
			set
			{
				this.SetParent(value);
			}
		}

		private Transform GetParent()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Transform>(Transform.GetParent_Injected(intPtr));
		}

		public void SetParent(Transform p)
		{
			this.SetParent(p, true);
		}

		[FreeFunction("SetParent", HasExplicitThis = true)]
		public void SetParent(Transform parent, bool worldPositionStays)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.SetParent_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Transform>(parent), worldPositionStays);
		}

		public Matrix4x4 worldToLocalMatrix
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				Transform.get_worldToLocalMatrix_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
		}

		public Matrix4x4 localToWorldMatrix
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				Transform.get_localToWorldMatrix_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
		}

		public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.SetPositionAndRotation_Injected(intPtr, ref position, ref rotation);
		}

		public void SetLocalPositionAndRotation(Vector3 localPosition, Quaternion localRotation)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.SetLocalPositionAndRotation_Injected(intPtr, ref localPosition, ref localRotation);
		}

		public void GetPositionAndRotation(out Vector3 position, out Quaternion rotation)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.GetPositionAndRotation_Injected(intPtr, out position, out rotation);
		}

		public void GetLocalPositionAndRotation(out Vector3 localPosition, out Quaternion localRotation)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.GetLocalPositionAndRotation_Injected(intPtr, out localPosition, out localRotation);
		}

		public void Translate(Vector3 translation, [DefaultValue("Space.Self")] Space relativeTo)
		{
			bool flag = relativeTo == Space.World;
			if (flag)
			{
				this.position += translation;
			}
			else
			{
				this.position += this.TransformDirection(translation);
			}
		}

		public void Translate(Vector3 translation)
		{
			this.Translate(translation, Space.Self);
		}

		public void Translate(float x, float y, float z, [DefaultValue("Space.Self")] Space relativeTo)
		{
			this.Translate(new Vector3(x, y, z), relativeTo);
		}

		public void Translate(float x, float y, float z)
		{
			this.Translate(new Vector3(x, y, z), Space.Self);
		}

		public void Translate(Vector3 translation, Transform relativeTo)
		{
			bool flag = relativeTo;
			if (flag)
			{
				this.position += relativeTo.TransformDirection(translation);
			}
			else
			{
				this.position += translation;
			}
		}

		public void Translate(float x, float y, float z, Transform relativeTo)
		{
			this.Translate(new Vector3(x, y, z), relativeTo);
		}

		public void Rotate(Vector3 eulers, [DefaultValue("Space.Self")] Space relativeTo)
		{
			Quaternion quaternion = Quaternion.Euler(eulers.x, eulers.y, eulers.z);
			bool flag = relativeTo == Space.Self;
			if (flag)
			{
				this.localRotation *= quaternion;
			}
			else
			{
				this.rotation *= Quaternion.Inverse(this.rotation) * quaternion * this.rotation;
			}
		}

		public void Rotate(Vector3 eulers)
		{
			this.Rotate(eulers, Space.Self);
		}

		public void Rotate(float xAngle, float yAngle, float zAngle, [DefaultValue("Space.Self")] Space relativeTo)
		{
			this.Rotate(new Vector3(xAngle, yAngle, zAngle), relativeTo);
		}

		public void Rotate(float xAngle, float yAngle, float zAngle)
		{
			this.Rotate(new Vector3(xAngle, yAngle, zAngle), Space.Self);
		}

		[NativeMethod("RotateAround")]
		internal void RotateAroundInternal(Vector3 axis, float angle)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.RotateAroundInternal_Injected(intPtr, ref axis, angle);
		}

		public void Rotate(Vector3 axis, float angle, [DefaultValue("Space.Self")] Space relativeTo)
		{
			bool flag = relativeTo == Space.Self;
			if (flag)
			{
				this.RotateAroundInternal(base.transform.TransformDirection(axis), angle * 0.017453292f);
			}
			else
			{
				this.RotateAroundInternal(axis, angle * 0.017453292f);
			}
		}

		public void Rotate(Vector3 axis, float angle)
		{
			this.Rotate(axis, angle, Space.Self);
		}

		public void RotateAround(Vector3 point, Vector3 axis, float angle)
		{
			Vector3 vector = this.position;
			Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
			Vector3 vector2 = vector - point;
			vector2 = quaternion * vector2;
			vector = point + vector2;
			this.position = vector;
			this.RotateAroundInternal(axis, angle * 0.017453292f);
		}

		public void LookAt(Transform target, [DefaultValue("Vector3.up")] Vector3 worldUp)
		{
			bool flag = target;
			if (flag)
			{
				this.LookAt(target.position, worldUp);
			}
		}

		public void LookAt(Transform target)
		{
			bool flag = target;
			if (flag)
			{
				this.LookAt(target.position, Vector3.up);
			}
		}

		public void LookAt(Vector3 worldPosition, [DefaultValue("Vector3.up")] Vector3 worldUp)
		{
			this.Internal_LookAt(worldPosition, worldUp);
		}

		public void LookAt(Vector3 worldPosition)
		{
			this.Internal_LookAt(worldPosition, Vector3.up);
		}

		[FreeFunction("Internal_LookAt", HasExplicitThis = true)]
		private void Internal_LookAt(Vector3 worldPosition, Vector3 worldUp)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.Internal_LookAt_Injected(intPtr, ref worldPosition, ref worldUp);
		}

		public Vector3 TransformDirection(Vector3 direction)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Transform.TransformDirection_Injected(intPtr, ref direction, out vector);
			return vector;
		}

		public Vector3 TransformDirection(float x, float y, float z)
		{
			return this.TransformDirection(new Vector3(x, y, z));
		}

		[NativeMethod(Name = "TransformDirections")]
		internal unsafe void TransformDirectionsInternal(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<Vector3> readOnlySpan = directions;
			fixed (Vector3* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<Vector3> span = transformedDirections;
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					Transform.TransformDirectionsInternal_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public void TransformDirections(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
		{
			bool flag = directions.Length != transformedDirections.Length;
			if (flag)
			{
				throw new InvalidOperationException("Both spans passed to Transform.TransformDirections() must be the same length");
			}
			this.TransformDirectionsInternal(directions, transformedDirections);
		}

		public void TransformDirections(Span<Vector3> directions)
		{
			this.TransformDirectionsInternal(directions, directions);
		}

		public Vector3 InverseTransformDirection(Vector3 direction)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Transform.InverseTransformDirection_Injected(intPtr, ref direction, out vector);
			return vector;
		}

		public Vector3 InverseTransformDirection(float x, float y, float z)
		{
			return this.InverseTransformDirection(new Vector3(x, y, z));
		}

		[NativeMethod(Name = "InverseTransformDirections")]
		internal unsafe void InverseTransformDirectionsInternal(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<Vector3> readOnlySpan = directions;
			fixed (Vector3* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<Vector3> span = transformedDirections;
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					Transform.InverseTransformDirectionsInternal_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public void InverseTransformDirections(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
		{
			bool flag = directions.Length != transformedDirections.Length;
			if (flag)
			{
				throw new InvalidOperationException("Both spans passed to Transform.InverseTransformDirections() must be the same length");
			}
			this.InverseTransformDirectionsInternal(directions, transformedDirections);
		}

		public void InverseTransformDirections(Span<Vector3> directions)
		{
			this.InverseTransformDirectionsInternal(directions, directions);
		}

		public Vector3 TransformVector(Vector3 vector)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector2;
			Transform.TransformVector_Injected(intPtr, ref vector, out vector2);
			return vector2;
		}

		public Vector3 TransformVector(float x, float y, float z)
		{
			return this.TransformVector(new Vector3(x, y, z));
		}

		[NativeMethod(Name = "TransformVectors")]
		internal unsafe void TransformVectorsInternal(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<Vector3> readOnlySpan = vectors;
			fixed (Vector3* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<Vector3> span = transformedVectors;
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					Transform.TransformVectorsInternal_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public void TransformVectors(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
		{
			bool flag = vectors.Length != transformedVectors.Length;
			if (flag)
			{
				throw new InvalidOperationException("Both spans passed to Transform.TransformVectors() must be the same length");
			}
			this.TransformVectorsInternal(vectors, transformedVectors);
		}

		public void TransformVectors(Span<Vector3> vectors)
		{
			this.TransformVectorsInternal(vectors, vectors);
		}

		public Vector3 InverseTransformVector(Vector3 vector)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector2;
			Transform.InverseTransformVector_Injected(intPtr, ref vector, out vector2);
			return vector2;
		}

		public Vector3 InverseTransformVector(float x, float y, float z)
		{
			return this.InverseTransformVector(new Vector3(x, y, z));
		}

		[NativeMethod(Name = "InverseTransformVectors")]
		internal unsafe void InverseTransformVectorsInternal(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<Vector3> readOnlySpan = vectors;
			fixed (Vector3* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<Vector3> span = transformedVectors;
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					Transform.InverseTransformVectorsInternal_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public void InverseTransformVectors(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
		{
			bool flag = vectors.Length != transformedVectors.Length;
			if (flag)
			{
				throw new InvalidOperationException("Both spans passed to Transform.InverseTransformVectors() must be the same length");
			}
			this.InverseTransformVectorsInternal(vectors, transformedVectors);
		}

		public void InverseTransformVectors(Span<Vector3> vectors)
		{
			this.InverseTransformVectorsInternal(vectors, vectors);
		}

		public Vector3 TransformPoint(Vector3 position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Transform.TransformPoint_Injected(intPtr, ref position, out vector);
			return vector;
		}

		public Vector3 TransformPoint(float x, float y, float z)
		{
			return this.TransformPoint(new Vector3(x, y, z));
		}

		[NativeMethod(Name = "TransformPoints")]
		internal unsafe void TransformPointsInternal(ReadOnlySpan<Vector3> positions, Span<Vector3> transformedPositions)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<Vector3> readOnlySpan = positions;
			fixed (Vector3* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<Vector3> span = transformedPositions;
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					Transform.TransformPointsInternal_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public void TransformPoints(ReadOnlySpan<Vector3> positions, Span<Vector3> transformedPositions)
		{
			bool flag = positions.Length != transformedPositions.Length;
			if (flag)
			{
				throw new InvalidOperationException("Both spans passed to Transform.TransformPoints() must be the same length");
			}
			this.TransformPointsInternal(positions, transformedPositions);
		}

		public void TransformPoints(Span<Vector3> positions)
		{
			this.TransformPointsInternal(positions, positions);
		}

		public Vector3 InverseTransformPoint(Vector3 position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Transform.InverseTransformPoint_Injected(intPtr, ref position, out vector);
			return vector;
		}

		public Vector3 InverseTransformPoint(float x, float y, float z)
		{
			return this.InverseTransformPoint(new Vector3(x, y, z));
		}

		[NativeMethod(Name = "InverseTransformPoints")]
		internal unsafe void InverseTransformPointsInternal(ReadOnlySpan<Vector3> positions, Span<Vector3> transformedPositions)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<Vector3> readOnlySpan = positions;
			fixed (Vector3* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<Vector3> span = transformedPositions;
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					Transform.InverseTransformPointsInternal_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public void InverseTransformPoints(ReadOnlySpan<Vector3> positions, Span<Vector3> transformedPositions)
		{
			bool flag = positions.Length != transformedPositions.Length;
			if (flag)
			{
				throw new InvalidOperationException("Both spans passed to Transform.InverseTransformPoints() must be the same length");
			}
			this.InverseTransformPointsInternal(positions, transformedPositions);
		}

		public void InverseTransformPoints(Span<Vector3> positions)
		{
			this.InverseTransformPoints(positions, positions);
		}

		public Transform root
		{
			get
			{
				return this.GetRoot();
			}
		}

		private Transform GetRoot()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Transform>(Transform.GetRoot_Injected(intPtr));
		}

		public int childCount
		{
			[NativeMethod("GetChildrenCount")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Transform.get_childCount_Injected(intPtr);
			}
		}

		[FreeFunction("DetachChildren", HasExplicitThis = true)]
		public void DetachChildren()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.DetachChildren_Injected(intPtr);
		}

		public void SetAsFirstSibling()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.SetAsFirstSibling_Injected(intPtr);
		}

		public void SetAsLastSibling()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.SetAsLastSibling_Injected(intPtr);
		}

		public void SetSiblingIndex(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.SetSiblingIndex_Injected(intPtr, index);
		}

		[NativeMethod("MoveAfterSiblingInternal")]
		internal void MoveAfterSibling(Transform transform, bool notifyEditorAndMarkDirty)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.MoveAfterSibling_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Transform>(transform), notifyEditorAndMarkDirty);
		}

		public int GetSiblingIndex()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Transform.GetSiblingIndex_Injected(intPtr);
		}

		[FreeFunction(HasExplicitThis = true)]
		private unsafe Transform FindRelativeTransformWithPath(string path, [DefaultValue("false")] bool isActiveOnly)
		{
			Transform transform;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = path.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr2 = Transform.FindRelativeTransformWithPath_Injected(intPtr, ref managedSpanWrapper, isActiveOnly);
			}
			finally
			{
				IntPtr intPtr2;
				transform = Unmarshal.UnmarshalUnityObject<Transform>(intPtr2);
				char* ptr = null;
			}
			return transform;
		}

		public Transform Find(string n)
		{
			bool flag = n == null;
			if (flag)
			{
				throw new ArgumentNullException("Name cannot be null");
			}
			return this.FindRelativeTransformWithPath(n, false);
		}

		[NativeConditional("UNITY_EDITOR")]
		internal void SendTransformChangedScale()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.SendTransformChangedScale_Injected(intPtr);
		}

		public Vector3 lossyScale
		{
			[NativeMethod("GetWorldScaleLossy")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Transform.get_lossyScale_Injected(intPtr, out vector);
				return vector;
			}
		}

		[FreeFunction("Internal_IsChildOrSameAsOtherTransform", HasExplicitThis = true)]
		public bool IsChildOf([NotNull] Transform parent)
		{
			if (parent == null)
			{
				ThrowHelper.ThrowArgumentNullException(parent, "parent");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Transform>(parent);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(parent, "parent");
			}
			return Transform.IsChildOf_Injected(intPtr, intPtr2);
		}

		[NativeProperty("HasChangedDeprecated")]
		public bool hasChanged
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Transform.get_hasChanged_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Transform.set_hasChanged_Injected(intPtr, value);
			}
		}

		[Obsolete("FindChild has been deprecated. Use Find instead (UnityUpgradable) -> Find([mscorlib] System.String)", false)]
		public Transform FindChild(string n)
		{
			return this.Find(n);
		}

		public IEnumerator GetEnumerator()
		{
			return new Transform.Enumerator(this);
		}

		[Obsolete("warning use Transform.Rotate instead.")]
		public void RotateAround(Vector3 axis, float angle)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.RotateAround_Injected(intPtr, ref axis, angle);
		}

		[Obsolete("warning use Transform.Rotate instead.")]
		public void RotateAroundLocal(Vector3 axis, float angle)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.RotateAroundLocal_Injected(intPtr, ref axis, angle);
		}

		[FreeFunction("GetChild", HasExplicitThis = true)]
		[NativeThrows]
		public Transform GetChild(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Transform>(Transform.GetChild_Injected(intPtr, index));
		}

		[Obsolete("warning use Transform.childCount instead (UnityUpgradable) -> Transform.childCount", false)]
		[NativeMethod("GetChildrenCount")]
		public int GetChildCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Transform.GetChildCount_Injected(intPtr);
		}

		public int hierarchyCapacity
		{
			get
			{
				return this.internal_getHierarchyCapacity();
			}
			set
			{
				this.internal_setHierarchyCapacity(value);
			}
		}

		[FreeFunction("GetHierarchyCapacity", HasExplicitThis = true)]
		private int internal_getHierarchyCapacity()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Transform.internal_getHierarchyCapacity_Injected(intPtr);
		}

		[FreeFunction("SetHierarchyCapacity", HasExplicitThis = true)]
		private void internal_setHierarchyCapacity(int value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.internal_setHierarchyCapacity_Injected(intPtr, value);
		}

		public int hierarchyCount
		{
			get
			{
				return this.internal_getHierarchyCount();
			}
		}

		[FreeFunction("GetHierarchyCount", HasExplicitThis = true)]
		private int internal_getHierarchyCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Transform.internal_getHierarchyCount_Injected(intPtr);
		}

		[NativeConditional("UNITY_EDITOR")]
		[FreeFunction("IsNonUniformScaleTransform", HasExplicitThis = true)]
		internal bool IsNonUniformScaleTransform()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Transform.IsNonUniformScaleTransform_Injected(intPtr);
		}

		[NativeConditional("UNITY_EDITOR")]
		internal bool constrainProportionsScale
		{
			get
			{
				return this.IsConstrainProportionsScale();
			}
			set
			{
				this.SetConstrainProportionsScale(value);
			}
		}

		[NativeConditional("UNITY_EDITOR")]
		private void SetConstrainProportionsScale(bool isLinked)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Transform.SetConstrainProportionsScale_Injected(intPtr, isLinked);
		}

		[NativeConditional("UNITY_EDITOR")]
		private bool IsConstrainProportionsScale()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Transform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Transform.IsConstrainProportionsScale_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_position_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_position_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_localPosition_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_localPosition_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalEulerAngles_Injected(IntPtr _unity_self, RotationOrder order, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLocalEulerAngles_Injected(IntPtr _unity_self, [In] ref Vector3 euler, RotationOrder order);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLocalEulerHint_Injected(IntPtr _unity_self, [In] ref Vector3 euler);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rotation_Injected(IntPtr _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_localRotation_Injected(IntPtr _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_localRotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRotationOrderInternal_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRotationOrderInternal_Injected(IntPtr _unity_self, RotationOrder rotationOrder);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_localScale_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_localScale_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetParent_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetParent_Injected(IntPtr _unity_self, IntPtr parent, bool worldPositionStays);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_worldToLocalMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_localToWorldMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPositionAndRotation_Injected(IntPtr _unity_self, [In] ref Vector3 position, [In] ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLocalPositionAndRotation_Injected(IntPtr _unity_self, [In] ref Vector3 localPosition, [In] ref Quaternion localRotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPositionAndRotation_Injected(IntPtr _unity_self, out Vector3 position, out Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalPositionAndRotation_Injected(IntPtr _unity_self, out Vector3 localPosition, out Quaternion localRotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RotateAroundInternal_Injected(IntPtr _unity_self, [In] ref Vector3 axis, float angle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_LookAt_Injected(IntPtr _unity_self, [In] ref Vector3 worldPosition, [In] ref Vector3 worldUp);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TransformDirection_Injected(IntPtr _unity_self, [In] ref Vector3 direction, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TransformDirectionsInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper directions, ref ManagedSpanWrapper transformedDirections);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InverseTransformDirection_Injected(IntPtr _unity_self, [In] ref Vector3 direction, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InverseTransformDirectionsInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper directions, ref ManagedSpanWrapper transformedDirections);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TransformVector_Injected(IntPtr _unity_self, [In] ref Vector3 vector, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TransformVectorsInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper vectors, ref ManagedSpanWrapper transformedVectors);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InverseTransformVector_Injected(IntPtr _unity_self, [In] ref Vector3 vector, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InverseTransformVectorsInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper vectors, ref ManagedSpanWrapper transformedVectors);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TransformPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TransformPointsInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper positions, ref ManagedSpanWrapper transformedPositions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InverseTransformPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InverseTransformPointsInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper positions, ref ManagedSpanWrapper transformedPositions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetRoot_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_childCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DetachChildren_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAsFirstSibling_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAsLastSibling_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSiblingIndex_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MoveAfterSibling_Injected(IntPtr _unity_self, IntPtr transform, bool notifyEditorAndMarkDirty);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSiblingIndex_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr FindRelativeTransformWithPath_Injected(IntPtr _unity_self, ref ManagedSpanWrapper path, [DefaultValue("false")] bool isActiveOnly);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SendTransformChangedScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_lossyScale_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsChildOf_Injected(IntPtr _unity_self, IntPtr parent);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasChanged_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_hasChanged_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RotateAround_Injected(IntPtr _unity_self, [In] ref Vector3 axis, float angle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RotateAroundLocal_Injected(IntPtr _unity_self, [In] ref Vector3 axis, float angle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetChild_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetChildCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int internal_getHierarchyCapacity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void internal_setHierarchyCapacity_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int internal_getHierarchyCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsNonUniformScaleTransform_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetConstrainProportionsScale_Injected(IntPtr _unity_self, bool isLinked);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsConstrainProportionsScale_Injected(IntPtr _unity_self);

		private class Enumerator : IEnumerator
		{
			internal Enumerator(Transform outer)
			{
				this.outer = outer;
			}

			public object Current
			{
				get
				{
					return this.outer.GetChild(this.currentIndex);
				}
			}

			public bool MoveNext()
			{
				int childCount = this.outer.childCount;
				int num = this.currentIndex + 1;
				this.currentIndex = num;
				return num < childCount;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}

			private Transform outer;

			private int currentIndex = -1;
		}
	}
}
