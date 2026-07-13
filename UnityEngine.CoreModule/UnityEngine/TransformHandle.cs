using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeClass("TransformHandle")]
	[Serializable]
	public struct TransformHandle : IEquatable<TransformHandle>, IComparable<TransformHandle>
	{
		public static TransformHandle None
		{
			get
			{
				return default(TransformHandle);
			}
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is TransformHandle)
			{
				TransformHandle transformHandle = (TransformHandle)obj;
				flag = this.Equals(transformHandle);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool Equals(TransformHandle other)
		{
			return this.id == other.id;
		}

		public int CompareTo(TransformHandle other)
		{
			return this.id.CompareTo(other.id);
		}

		public static bool operator ==(TransformHandle lhs, TransformHandle rhs)
		{
			return lhs.id == rhs.id && lhs.pTransformData == rhs.pTransformData;
		}

		public static bool operator !=(TransformHandle lhs, TransformHandle rhs)
		{
			return lhs.id != rhs.id || lhs.pTransformData != rhs.pTransformData;
		}

		public TransformHandle.DirectChildrenEnumerable DirectChildren
		{
			get
			{
				return new TransformHandle.DirectChildrenEnumerable(this);
			}
		}

		public TransformHandle.DirectChildrenEnumerator GetDirectChildrenEnumerator()
		{
			return new TransformHandle.DirectChildrenEnumerator(this);
		}

		public override int GetHashCode()
		{
			return this.id.GetHashCode();
		}

		private static void AssertHandleIsValid(TransformHandle handle)
		{
			bool flag = !Resources.EntityIdIsValid(handle.id);
			if (!flag)
			{
				return;
			}
			bool flag2 = handle.id == EntityId.None;
			if (flag2)
			{
				throw new NullReferenceException("The TransformHandle object is null. It may not have been properly initialized, or may refer to an object which has been destroyed. TransformHandles should only be obtained through a valid GameObject or Component.");
			}
			throw new MissingReferenceException(string.Format("The target of this TransformHandle (id='{0}') is not a valid object. The corresponding object may have been destroyed.", handle.id));
		}

		public Vector3 position
		{
			get
			{
				TransformHandle.AssertHandleIsValid(this);
				Vector3 vector;
				this.Internal_GetPosition(out vector);
				return vector;
			}
			set
			{
				TransformHandle.AssertHandleIsValid(this);
				this.Internal_SetPosition(value);
			}
		}

		[NativeMethod(Name = "GetPosition")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetPosition(out Vector3 p);

		[NativeMethod(Name = "SetPosition")]
		private void Internal_SetPosition(Vector3 p)
		{
			TransformHandle.Internal_SetPosition_Injected(ref this, ref p);
		}

		public Quaternion rotation
		{
			get
			{
				TransformHandle.AssertHandleIsValid(this);
				Quaternion quaternion;
				this.Internal_GetRotation(out quaternion);
				return quaternion;
			}
			set
			{
				TransformHandle.AssertHandleIsValid(this);
				this.Internal_SetRotation(value);
			}
		}

		[NativeMethod(Name = "GetRotation")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetRotation(out Quaternion r);

		[NativeMethod(Name = "SetRotation")]
		private void Internal_SetRotation(Quaternion r)
		{
			TransformHandle.Internal_SetRotation_Injected(ref this, ref r);
		}

		public Vector3 lossyScale
		{
			get
			{
				TransformHandle.AssertHandleIsValid(this);
				Vector3 vector;
				this.Internal_GetWorldScaleLossy(out vector);
				return vector;
			}
		}

		[NativeMethod(Name = "GetWorldScaleLossy")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetWorldScaleLossy(out Vector3 s);

		public Vector3 localPosition
		{
			get
			{
				TransformHandle.AssertHandleIsValid(this);
				Vector3 vector;
				this.Internal_GetLocalPosition(out vector);
				return vector;
			}
			set
			{
				TransformHandle.AssertHandleIsValid(this);
				this.Internal_SetLocalPosition(value);
			}
		}

		[NativeMethod(Name = "GetLocalPosition")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetLocalPosition(out Vector3 r);

		[NativeMethod(Name = "SetLocalPosition")]
		private void Internal_SetLocalPosition(Vector3 r)
		{
			TransformHandle.Internal_SetLocalPosition_Injected(ref this, ref r);
		}

		public Quaternion localRotation
		{
			get
			{
				TransformHandle.AssertHandleIsValid(this);
				Quaternion quaternion;
				this.Internal_GetLocalRotation(out quaternion);
				return quaternion;
			}
			set
			{
				TransformHandle.AssertHandleIsValid(this);
				this.Internal_SetLocalRotation(value);
			}
		}

		[NativeMethod(Name = "GetLocalRotation")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetLocalRotation(out Quaternion r);

		[NativeMethod(Name = "SetLocalRotation")]
		private void Internal_SetLocalRotation(Quaternion r)
		{
			TransformHandle.Internal_SetLocalRotation_Injected(ref this, ref r);
		}

		public Vector3 localScale
		{
			get
			{
				TransformHandle.AssertHandleIsValid(this);
				Vector3 vector;
				this.Internal_GetLocalScale(out vector);
				return vector;
			}
			set
			{
				TransformHandle.AssertHandleIsValid(this);
				this.Internal_SetLocalScale(value);
			}
		}

		[NativeMethod(Name = "GetLocalScale")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetLocalScale(out Vector3 r);

		[NativeMethod(Name = "SetLocalScale")]
		private void Internal_SetLocalScale(Vector3 r)
		{
			TransformHandle.Internal_SetLocalScale_Injected(ref this, ref r);
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

		public Matrix4x4 worldToLocalMatrix
		{
			get
			{
				TransformHandle.AssertHandleIsValid(this);
				Matrix4x4 matrix4x;
				this.Internal_GetWorldToLocalMatrix(out matrix4x);
				return matrix4x;
			}
		}

		[NativeMethod(Name = "GetWorldToLocalMatrix")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetWorldToLocalMatrix(out Matrix4x4 m);

		public Matrix4x4 localToWorldMatrix
		{
			get
			{
				TransformHandle.AssertHandleIsValid(this);
				Matrix4x4 matrix4x;
				this.Internal_GetLocalToWorldMatrix(out matrix4x);
				return matrix4x;
			}
		}

		[NativeMethod(Name = "GetLocalToWorldMatrix")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetLocalToWorldMatrix(out Matrix4x4 m);

		public bool IsValid()
		{
			return this.Internal_IsValid();
		}

		[NativeMethod(Name = "IsValid")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_IsValid();

		public TransformHandle root
		{
			get
			{
				TransformHandle.AssertHandleIsValid(this);
				TransformHandle transformHandle;
				this.Internal_GetRoot(out transformHandle);
				return transformHandle;
			}
		}

		[NativeMethod(Name = "Internal_GetRoot")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetRoot(out TransformHandle outRootHandle);

		public TransformHandle parent
		{
			get
			{
				TransformHandle.AssertHandleIsValid(this);
				TransformHandle transformHandle;
				this.Internal_TryGetParent(out transformHandle);
				return transformHandle;
			}
			set
			{
				TransformHandle.AssertHandleIsValid(this);
				TransformHandle transformHandle = value;
				bool flag = transformHandle != TransformHandle.None && !transformHandle.IsValid();
				if (flag)
				{
					transformHandle = TransformHandle.None;
				}
				this.Internal_SetParent(transformHandle, true);
			}
		}

		[NativeMethod(Name = "TryGetParent")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_TryGetParent(out TransformHandle parentHandle);

		public void SetParent(TransformHandle p)
		{
			TransformHandle.AssertHandleIsValid(this);
			bool flag = p != TransformHandle.None && !p.IsValid();
			if (flag)
			{
				p = TransformHandle.None;
			}
			this.Internal_SetParent(p, true);
		}

		public void SetParent(TransformHandle parent, bool worldPositionStays)
		{
			TransformHandle.AssertHandleIsValid(this);
			bool flag = parent != TransformHandle.None && !parent.IsValid();
			if (flag)
			{
				parent = TransformHandle.None;
			}
			this.Internal_SetParent(parent, worldPositionStays);
		}

		[NativeMethod(Name = "SetParent_Internal")]
		private void Internal_SetParent(TransformHandle parent, bool worldPositionStays)
		{
			TransformHandle.Internal_SetParent_Injected(ref this, ref parent, worldPositionStays);
		}

		public TransformHandle GetChild(int index)
		{
			TransformHandle.AssertHandleIsValid(this);
			TransformHandle transformHandle;
			this.Internal_GetChild(index, out transformHandle);
			return transformHandle;
		}

		[NativeMethod(Name = "Internal_GetChild")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetChild(int index, out TransformHandle outChildHandle);

		public bool HasParent()
		{
			TransformHandle.AssertHandleIsValid(this);
			return this.Internal_HasParent();
		}

		[NativeMethod(Name = "HasParent")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_HasParent();

		public bool IsChildOf(TransformHandle parent)
		{
			TransformHandle.AssertHandleIsValid(this);
			bool flag = parent != TransformHandle.None;
			if (flag)
			{
				TransformHandle.AssertHandleIsValid(parent);
			}
			return this.Internal_IsChildOf(parent);
		}

		[NativeMethod(Name = "IsChildOrSameAsOther")]
		private bool Internal_IsChildOf(TransformHandle parent)
		{
			return TransformHandle.Internal_IsChildOf_Injected(ref this, ref parent);
		}

		public int childCount
		{
			get
			{
				TransformHandle.AssertHandleIsValid(this);
				return this.Internal_GetChildrenCount();
			}
		}

		[NativeMethod(Name = "GetChildrenCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetChildrenCount();

		public void DetachChildren()
		{
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_DetachChildren();
		}

		[NativeMethod(Name = "DetachChildren")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DetachChildren();

		public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
		{
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_SetPositionAndRotation(position, rotation);
		}

		[NativeMethod(Name = "SetPositionAndRotation")]
		private void Internal_SetPositionAndRotation(Vector3 position, Quaternion rotation)
		{
			TransformHandle.Internal_SetPositionAndRotation_Injected(ref this, ref position, ref rotation);
		}

		public void SetLocalPositionAndRotation(Vector3 localPosition, Quaternion localRotation)
		{
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_SetLocalPositionAndRotation(localPosition, localRotation);
		}

		[NativeMethod(Name = "SetLocalPositionAndRotation")]
		private void Internal_SetLocalPositionAndRotation(Vector3 localPosition, Quaternion localRotation)
		{
			TransformHandle.Internal_SetLocalPositionAndRotation_Injected(ref this, ref localPosition, ref localRotation);
		}

		public void GetPositionAndRotation(out Vector3 position, out Quaternion rotation)
		{
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_GetPositionAndRotation(out position, out rotation);
		}

		[NativeMethod(Name = "GetPositionAndRotation")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetPositionAndRotation(out Vector3 position, out Quaternion rotation);

		public void GetLocalPositionAndRotation(out Vector3 localPosition, out Quaternion localRotation)
		{
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_GetLocalPositionAndRotation(out localPosition, out localRotation);
		}

		[NativeMethod(Name = "GetLocalPositionAndRotation")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetLocalPositionAndRotation(out Vector3 localPosition, out Quaternion localRotation);

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

		public void Translate(Vector3 translation, TransformHandle relativeTo)
		{
			bool flag = relativeTo != TransformHandle.None;
			if (flag)
			{
				this.position += relativeTo.TransformDirection(translation);
			}
			else
			{
				this.position += translation;
			}
		}

		public void Translate(float x, float y, float z, TransformHandle relativeTo)
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

		public void Rotate(Vector3 axis, float angle, [DefaultValue("Space.Self")] Space relativeTo)
		{
			TransformHandle.AssertHandleIsValid(this);
			bool flag = relativeTo == Space.Self;
			if (flag)
			{
				this.Internal_RotateAround(this.TransformDirection(axis), angle * 0.017453292f);
			}
			else
			{
				this.Internal_RotateAround(axis, angle * 0.017453292f);
			}
		}

		public void RotateAround(Vector3 point, Vector3 axis, float angle)
		{
			Vector3 vector = this.position;
			Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
			Vector3 vector2 = vector - point;
			vector2 = quaternion * vector2;
			vector = point + vector2;
			this.position = vector;
			this.Internal_RotateAround(axis, angle * 0.017453292f);
		}

		[NativeMethod(Name = "RotateAround")]
		private void Internal_RotateAround(Vector3 worldAxis, float rad)
		{
			TransformHandle.Internal_RotateAround_Injected(ref this, ref worldAxis, rad);
		}

		public void Rotate(Vector3 axis, float angle)
		{
			this.Rotate(axis, angle, Space.Self);
		}

		public void LookAt(TransformHandle target, [DefaultValue("Vector3.up")] Vector3 worldUp)
		{
			bool flag = target != TransformHandle.None;
			if (flag)
			{
				TransformHandle.AssertHandleIsValid(this);
				TransformHandle.AssertHandleIsValid(target);
				this.Internal_LookAt(target.position, worldUp);
			}
		}

		public void LookAt(TransformHandle target)
		{
			bool flag = target != TransformHandle.None;
			if (flag)
			{
				TransformHandle.AssertHandleIsValid(this);
				TransformHandle.AssertHandleIsValid(target);
				this.Internal_LookAt(target.position, Vector3.up);
			}
		}

		public void LookAt(Vector3 worldPosition, [DefaultValue("Vector3.up")] Vector3 worldUp)
		{
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_LookAt(worldPosition, worldUp);
		}

		public void LookAt(Vector3 worldPosition)
		{
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_LookAt(worldPosition, Vector3.up);
		}

		[NativeMethod(Name = "LookAt")]
		private void Internal_LookAt(Vector3 worldPosition, Vector3 worldUp)
		{
			TransformHandle.Internal_LookAt_Injected(ref this, ref worldPosition, ref worldUp);
		}

		public Vector3 TransformPoint(float x, float y, float z)
		{
			TransformHandle.AssertHandleIsValid(this);
			return this.Internal_TransformPoint(new Vector3(x, y, z));
		}

		public Vector3 TransformPoint(Vector3 point)
		{
			TransformHandle.AssertHandleIsValid(this);
			return this.Internal_TransformPoint(point);
		}

		[NativeMethod(Name = "TransformPoint")]
		private Vector3 Internal_TransformPoint(Vector3 point)
		{
			Vector3 vector;
			TransformHandle.Internal_TransformPoint_Injected(ref this, ref point, out vector);
			return vector;
		}

		public void TransformPoints(ReadOnlySpan<Vector3> positions, Span<Vector3> transformedPositions)
		{
			bool flag = positions.Length != transformedPositions.Length;
			if (flag)
			{
				throw new InvalidOperationException("Both spans passed to Transform.TransformPoints() must be the same length");
			}
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_TransformPoints(positions, transformedPositions);
		}

		public void TransformPoints(Span<Vector3> positions)
		{
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_TransformPoints(positions, positions);
		}

		[NativeMethod(Name = "TransformPoints")]
		private unsafe void Internal_TransformPoints(ReadOnlySpan<Vector3> points, Span<Vector3> transformedPoints)
		{
			ReadOnlySpan<Vector3> readOnlySpan = points;
			fixed (Vector3* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<Vector3> span = transformedPoints;
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					TransformHandle.Internal_TransformPoints_Injected(ref this, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public Vector3 TransformDirection(float x, float y, float z)
		{
			TransformHandle.AssertHandleIsValid(this);
			return this.Internal_TransformDirection(new Vector3(x, y, z));
		}

		public Vector3 TransformDirection(Vector3 direction)
		{
			TransformHandle.AssertHandleIsValid(this);
			return this.Internal_TransformDirection(direction);
		}

		[NativeMethod(Name = "TransformDirection")]
		private Vector3 Internal_TransformDirection(Vector3 direction)
		{
			Vector3 vector;
			TransformHandle.Internal_TransformDirection_Injected(ref this, ref direction, out vector);
			return vector;
		}

		public void TransformDirections(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
		{
			bool flag = directions.Length != transformedDirections.Length;
			if (flag)
			{
				throw new InvalidOperationException("Both spans passed to Transform.TransformDirections() must be the same length");
			}
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_TransformDirections(directions, transformedDirections);
		}

		public void TransformDirections(Span<Vector3> directions)
		{
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_TransformDirections(directions, directions);
		}

		[NativeMethod(Name = "TransformDirections")]
		private unsafe void Internal_TransformDirections(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
		{
			ReadOnlySpan<Vector3> readOnlySpan = directions;
			fixed (Vector3* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<Vector3> span = transformedDirections;
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					TransformHandle.Internal_TransformDirections_Injected(ref this, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public Vector3 TransformVector(float x, float y, float z)
		{
			return this.Internal_TransformVector(new Vector3(x, y, z));
		}

		public Vector3 TransformVector(Vector3 vector)
		{
			return this.Internal_TransformVector(vector);
		}

		[NativeMethod(Name = "TransformVector")]
		private Vector3 Internal_TransformVector(Vector3 vector)
		{
			Vector3 vector2;
			TransformHandle.Internal_TransformVector_Injected(ref this, ref vector, out vector2);
			return vector2;
		}

		public void TransformVectors(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
		{
			bool flag = vectors.Length != transformedVectors.Length;
			if (flag)
			{
				throw new InvalidOperationException("Both spans passed to Transform.TransformVectors() must be the same length");
			}
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_TransformVectors(vectors, transformedVectors);
		}

		public void TransformVectors(Span<Vector3> vectors)
		{
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_TransformVectors(vectors, vectors);
		}

		[NativeMethod(Name = "TransformVectors")]
		private unsafe void Internal_TransformVectors(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
		{
			ReadOnlySpan<Vector3> readOnlySpan = vectors;
			fixed (Vector3* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<Vector3> span = transformedVectors;
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					TransformHandle.Internal_TransformVectors_Injected(ref this, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public Vector3 InverseTransformPoint(float x, float y, float z)
		{
			TransformHandle.AssertHandleIsValid(this);
			return this.Internal_InverseTransformPoint(new Vector3(x, y, z));
		}

		public Vector3 InverseTransformPoint(Vector3 point)
		{
			TransformHandle.AssertHandleIsValid(this);
			return this.Internal_InverseTransformPoint(point);
		}

		[NativeMethod(Name = "InverseTransformPoint")]
		private Vector3 Internal_InverseTransformPoint(Vector3 point)
		{
			Vector3 vector;
			TransformHandle.Internal_InverseTransformPoint_Injected(ref this, ref point, out vector);
			return vector;
		}

		public void InverseTransformPoints(ReadOnlySpan<Vector3> positions, Span<Vector3> transformedPositions)
		{
			bool flag = positions.Length != transformedPositions.Length;
			if (flag)
			{
				throw new InvalidOperationException("Both spans passed to Transform.InverseTransformPoints() must be the same length");
			}
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_InverseTransformPoints(positions, transformedPositions);
		}

		public void InverseTransformPoints(Span<Vector3> positions)
		{
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_InverseTransformPoints(positions, positions);
		}

		[NativeMethod(Name = "InverseTransformPoints")]
		private unsafe void Internal_InverseTransformPoints(ReadOnlySpan<Vector3> points, Span<Vector3> transformedPoints)
		{
			ReadOnlySpan<Vector3> readOnlySpan = points;
			fixed (Vector3* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<Vector3> span = transformedPoints;
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					TransformHandle.Internal_InverseTransformPoints_Injected(ref this, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public Vector3 InverseTransformDirection(float x, float y, float z)
		{
			TransformHandle.AssertHandleIsValid(this);
			return this.Internal_InverseTransformDirection(new Vector3(x, y, z));
		}

		public Vector3 InverseTransformDirection(Vector3 direction)
		{
			TransformHandle.AssertHandleIsValid(this);
			return this.Internal_InverseTransformDirection(direction);
		}

		[NativeMethod(Name = "InverseTransformDirection")]
		private Vector3 Internal_InverseTransformDirection(Vector3 direction)
		{
			Vector3 vector;
			TransformHandle.Internal_InverseTransformDirection_Injected(ref this, ref direction, out vector);
			return vector;
		}

		public void InverseTransformDirections(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
		{
			bool flag = directions.Length != transformedDirections.Length;
			if (flag)
			{
				throw new InvalidOperationException("Both spans passed to Transform.InverseTransformDirections() must be the same length");
			}
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_InverseTransformDirections(directions, transformedDirections);
		}

		public void InverseTransformDirections(Span<Vector3> directions)
		{
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_InverseTransformDirections(directions, directions);
		}

		[NativeMethod(Name = "InverseTransformDirections")]
		private unsafe void Internal_InverseTransformDirections(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
		{
			ReadOnlySpan<Vector3> readOnlySpan = directions;
			fixed (Vector3* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<Vector3> span = transformedDirections;
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					TransformHandle.Internal_InverseTransformDirections_Injected(ref this, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public Vector3 InverseTransformVector(float x, float y, float z)
		{
			TransformHandle.AssertHandleIsValid(this);
			return this.Internal_InverseTransformVector(new Vector3(x, y, z));
		}

		public Vector3 InverseTransformVector(Vector3 vector)
		{
			TransformHandle.AssertHandleIsValid(this);
			return this.Internal_InverseTransformVector(vector);
		}

		[NativeMethod(Name = "InverseTransformVector")]
		private Vector3 Internal_InverseTransformVector(Vector3 vector)
		{
			Vector3 vector2;
			TransformHandle.Internal_InverseTransformVector_Injected(ref this, ref vector, out vector2);
			return vector2;
		}

		public void InverseTransformVectors(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
		{
			bool flag = vectors.Length != transformedVectors.Length;
			if (flag)
			{
				throw new InvalidOperationException("Both spans passed to Transform.InverseTransformVectors() must be the same length");
			}
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_InverseTransformVectors(vectors, transformedVectors);
		}

		public void InverseTransformVectors(Span<Vector3> vectors)
		{
			TransformHandle.AssertHandleIsValid(this);
			this.Internal_InverseTransformVectors(vectors, vectors);
		}

		[NativeMethod(Name = "InverseTransformVectors")]
		private unsafe void Internal_InverseTransformVectors(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
		{
			ReadOnlySpan<Vector3> readOnlySpan = vectors;
			fixed (Vector3* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<Vector3> span = transformedVectors;
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					TransformHandle.Internal_InverseTransformVectors_Injected(ref this, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public int hierarchyCapacity
		{
			get
			{
				TransformHandle.AssertHandleIsValid(this);
				return this.Internal_GetHierarchyCapacity();
			}
			set
			{
				TransformHandle.AssertHandleIsValid(this);
				this.Internal_SetHierarchyCapacity(value);
			}
		}

		[NativeMethod("GetHierarchyCapacity")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetHierarchyCapacity();

		[NativeMethod("SetHierarchyCapacity")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetHierarchyCapacity(int value);

		public int hierarchyCount
		{
			get
			{
				TransformHandle.AssertHandleIsValid(this);
				return this.Internal_GetHierarchyCount();
			}
		}

		[NativeMethod("GetHierarchyCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetHierarchyCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetPosition_Injected(ref TransformHandle _unity_self, [In] ref Vector3 p);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRotation_Injected(ref TransformHandle _unity_self, [In] ref Quaternion r);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetLocalPosition_Injected(ref TransformHandle _unity_self, [In] ref Vector3 r);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetLocalRotation_Injected(ref TransformHandle _unity_self, [In] ref Quaternion r);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetLocalScale_Injected(ref TransformHandle _unity_self, [In] ref Vector3 r);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetParent_Injected(ref TransformHandle _unity_self, [In] ref TransformHandle parent, bool worldPositionStays);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_IsChildOf_Injected(ref TransformHandle _unity_self, [In] ref TransformHandle parent);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetPositionAndRotation_Injected(ref TransformHandle _unity_self, [In] ref Vector3 position, [In] ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetLocalPositionAndRotation_Injected(ref TransformHandle _unity_self, [In] ref Vector3 localPosition, [In] ref Quaternion localRotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_RotateAround_Injected(ref TransformHandle _unity_self, [In] ref Vector3 worldAxis, float rad);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_LookAt_Injected(ref TransformHandle _unity_self, [In] ref Vector3 worldPosition, [In] ref Vector3 worldUp);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_TransformPoint_Injected(ref TransformHandle _unity_self, [In] ref Vector3 point, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_TransformPoints_Injected(ref TransformHandle _unity_self, ref ManagedSpanWrapper points, ref ManagedSpanWrapper transformedPoints);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_TransformDirection_Injected(ref TransformHandle _unity_self, [In] ref Vector3 direction, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_TransformDirections_Injected(ref TransformHandle _unity_self, ref ManagedSpanWrapper directions, ref ManagedSpanWrapper transformedDirections);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_TransformVector_Injected(ref TransformHandle _unity_self, [In] ref Vector3 vector, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_TransformVectors_Injected(ref TransformHandle _unity_self, ref ManagedSpanWrapper vectors, ref ManagedSpanWrapper transformedVectors);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_InverseTransformPoint_Injected(ref TransformHandle _unity_self, [In] ref Vector3 point, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_InverseTransformPoints_Injected(ref TransformHandle _unity_self, ref ManagedSpanWrapper points, ref ManagedSpanWrapper transformedPoints);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_InverseTransformDirection_Injected(ref TransformHandle _unity_self, [In] ref Vector3 direction, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_InverseTransformDirections_Injected(ref TransformHandle _unity_self, ref ManagedSpanWrapper directions, ref ManagedSpanWrapper transformedDirections);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_InverseTransformVector_Injected(ref TransformHandle _unity_self, [In] ref Vector3 vector, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_InverseTransformVectors_Injected(ref TransformHandle _unity_self, ref ManagedSpanWrapper vectors, ref ManagedSpanWrapper transformedVectors);

		internal IntPtr pTransformData;

		[SerializeField]
		internal EntityId id;

		public struct DirectChildrenEnumerable : IEnumerable<TransformHandle>, IEnumerable
		{
			public DirectChildrenEnumerable(TransformHandle root)
			{
				this.Root = root;
			}

			public IEnumerator<TransformHandle> GetEnumerator()
			{
				return new TransformHandle.DirectChildrenEnumerator(this.Root);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private TransformHandle Root;
		}

		public struct DirectChildrenEnumerator : IEnumerator<TransformHandle>, IEnumerator, IDisposable
		{
			internal DirectChildrenEnumerator(TransformHandle parent)
			{
				this.parent = parent;
				this.currentIndex = -1;
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public TransformHandle Current
			{
				get
				{
					return this.parent.GetChild(this.currentIndex);
				}
			}

			public bool MoveNext()
			{
				int num = this.currentIndex + 1;
				this.currentIndex = num;
				return num < this.parent.childCount;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}

			public void Dispose()
			{
			}

			private TransformHandle parent;

			private int currentIndex;
		}
	}
}
