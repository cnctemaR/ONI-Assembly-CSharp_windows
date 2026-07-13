using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode(Optional = true)]
	[NativeHeader("Modules/Physics2D/Public/Collider2D.h")]
	[RequireComponent(typeof(Transform))]
	public class Collider2D : Behaviour
	{
		public float density
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Collider2D.get_density_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Collider2D.set_density_Injected(intPtr, value);
			}
		}

		public bool isTrigger
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Collider2D.get_isTrigger_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Collider2D.set_isTrigger_Injected(intPtr, value);
			}
		}

		public bool usedByEffector
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Collider2D.get_usedByEffector_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Collider2D.set_usedByEffector_Injected(intPtr, value);
			}
		}

		public Collider2D.CompositeOperation compositeOperation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Collider2D.get_compositeOperation_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Collider2D.set_compositeOperation_Injected(intPtr, value);
			}
		}

		public int compositeOrder
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Collider2D.get_compositeOrder_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Collider2D.set_compositeOrder_Injected(intPtr, value);
			}
		}

		public CompositeCollider2D composite
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<CompositeCollider2D>(Collider2D.get_composite_Injected(intPtr));
			}
		}

		public Vector2 offset
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				Collider2D.get_offset_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Collider2D.set_offset_Injected(intPtr, ref value);
			}
		}

		public Rigidbody2D attachedRigidbody
		{
			[NativeMethod("GetAttachedRigidbody_Binding")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Rigidbody2D>(Collider2D.get_attachedRigidbody_Injected(intPtr));
			}
		}

		public Matrix4x4 localToWorldMatrix
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				Collider2D.get_localToWorldMatrix_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
		}

		public int shapeCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Collider2D.get_shapeCount_Injected(intPtr);
			}
		}

		[ExcludeFromDocs]
		public Mesh CreateMesh(bool useBodyPosition, bool useBodyRotation)
		{
			return this.CreateMesh(useBodyPosition, useBodyRotation, true);
		}

		[NativeMethod("CreateMesh_Binding")]
		public Mesh CreateMesh(bool useBodyPosition, bool useBodyRotation, [DefaultValue("true")] bool useDelaunay = true)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Mesh>(Collider2D.CreateMesh_Injected(intPtr, useBodyPosition, useBodyRotation, useDelaunay));
		}

		[NativeMethod("GetShapeHash_Binding")]
		public uint GetShapeHash()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Collider2D.GetShapeHash_Injected(intPtr);
		}

		public int GetShapes(PhysicsShapeGroup2D physicsShapeGroup)
		{
			return this.GetShapes_Internal(ref physicsShapeGroup.m_GroupState, 0, this.shapeCount);
		}

		public int GetShapes(PhysicsShapeGroup2D physicsShapeGroup, int shapeIndex, [DefaultValue("1")] int shapeCount = 1)
		{
			int shapeCount2 = this.shapeCount;
			bool flag = shapeIndex < 0 || shapeIndex >= shapeCount2 || shapeCount < 1 || shapeIndex + shapeCount > shapeCount2;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Cannot get shape range from {0} to {1} as Collider2D only has {2} shape(s).", shapeIndex, shapeIndex + shapeCount - 1, shapeCount2));
			}
			return this.GetShapes_Internal(ref physicsShapeGroup.m_GroupState, shapeIndex, shapeCount);
		}

		[NativeMethod("GetShapes_Binding")]
		private int GetShapes_Internal(ref PhysicsShapeGroup2D.GroupState physicsShapeGroupState, int shapeIndex, int shapeCount)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Collider2D.GetShapes_Internal_Injected(intPtr, ref physicsShapeGroupState, shapeIndex, shapeCount);
		}

		[NativeMethod("GetShapeBounds_Binding")]
		public unsafe Bounds GetShapeBounds(List<Bounds> bounds, bool useRadii, bool useWorldSpace)
		{
			Bounds bounds3;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (bounds != null)
				{
					fixed (Bounds[] array = NoAllocHelpers.ExtractArrayFromList<Bounds>(bounds))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, bounds.Count);
					}
				}
				Bounds bounds2;
				Collider2D.GetShapeBounds_Injected(intPtr, ref blittableListWrapper, useRadii, useWorldSpace, out bounds2);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<Bounds>(bounds);
				Bounds bounds2;
				bounds3 = bounds2;
			}
			return bounds3;
		}

		public Bounds bounds
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Bounds bounds;
				Collider2D.get_bounds_Injected(intPtr, out bounds);
				return bounds;
			}
		}

		public ColliderErrorState2D errorState
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Collider2D.get_errorState_Injected(intPtr);
			}
		}

		public bool compositeCapable
		{
			[NativeMethod("GetCompositeCapable_Binding")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Collider2D.get_compositeCapable_Injected(intPtr);
			}
		}

		public PhysicsMaterial2D sharedMaterial
		{
			[NativeMethod("GetMaterial")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<PhysicsMaterial2D>(Collider2D.get_sharedMaterial_Injected(intPtr));
			}
			[NativeMethod("SetMaterial")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Collider2D.set_sharedMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<PhysicsMaterial2D>(value));
			}
		}

		public int layerOverridePriority
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Collider2D.get_layerOverridePriority_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Collider2D.set_layerOverridePriority_Injected(intPtr, value);
			}
		}

		public LayerMask excludeLayers
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LayerMask layerMask;
				Collider2D.get_excludeLayers_Injected(intPtr, out layerMask);
				return layerMask;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Collider2D.set_excludeLayers_Injected(intPtr, ref value);
			}
		}

		public LayerMask includeLayers
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LayerMask layerMask;
				Collider2D.get_includeLayers_Injected(intPtr, out layerMask);
				return layerMask;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Collider2D.set_includeLayers_Injected(intPtr, ref value);
			}
		}

		public LayerMask forceSendLayers
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LayerMask layerMask;
				Collider2D.get_forceSendLayers_Injected(intPtr, out layerMask);
				return layerMask;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Collider2D.set_forceSendLayers_Injected(intPtr, ref value);
			}
		}

		public LayerMask forceReceiveLayers
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LayerMask layerMask;
				Collider2D.get_forceReceiveLayers_Injected(intPtr, out layerMask);
				return layerMask;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Collider2D.set_forceReceiveLayers_Injected(intPtr, ref value);
			}
		}

		public LayerMask contactCaptureLayers
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LayerMask layerMask;
				Collider2D.get_contactCaptureLayers_Injected(intPtr, out layerMask);
				return layerMask;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Collider2D.set_contactCaptureLayers_Injected(intPtr, ref value);
			}
		}

		public LayerMask callbackLayers
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LayerMask layerMask;
				Collider2D.get_callbackLayers_Injected(intPtr, out layerMask);
				return layerMask;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Collider2D.set_callbackLayers_Injected(intPtr, ref value);
			}
		}

		public float friction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Collider2D.get_friction_Injected(intPtr);
			}
		}

		public float bounciness
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Collider2D.get_bounciness_Injected(intPtr);
			}
		}

		public PhysicsMaterialCombine2D frictionCombine
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Collider2D.get_frictionCombine_Injected(intPtr);
			}
		}

		public PhysicsMaterialCombine2D bounceCombine
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Collider2D.get_bounceCombine_Injected(intPtr);
			}
		}

		public LayerMask contactMask
		{
			[NativeMethod("GetContactMask_Binding")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LayerMask layerMask;
				Collider2D.get_contactMask_Injected(intPtr, out layerMask);
				return layerMask;
			}
		}

		[NativeMethod("CanContact_Binding")]
		public bool CanContact([NotNull] Collider2D collider)
		{
			if (collider == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			return Collider2D.CanContact_Injected(intPtr, intPtr2);
		}

		public bool IsTouching([NotNull] Collider2D collider)
		{
			if (collider == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			return Collider2D.IsTouching_Injected(intPtr, intPtr2);
		}

		public bool IsTouching(Collider2D collider, ContactFilter2D contactFilter)
		{
			return this.IsTouching_OtherColliderWithFilter(collider, contactFilter);
		}

		[NativeMethod("IsTouching")]
		private bool IsTouching_OtherColliderWithFilter([NotNull] Collider2D collider, ContactFilter2D contactFilter)
		{
			if (collider == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			return Collider2D.IsTouching_OtherColliderWithFilter_Injected(intPtr, intPtr2, ref contactFilter);
		}

		public bool IsTouching(ContactFilter2D contactFilter)
		{
			return this.IsTouching_AnyColliderWithFilter(contactFilter);
		}

		[NativeMethod("IsTouching")]
		private bool IsTouching_AnyColliderWithFilter(ContactFilter2D contactFilter)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Collider2D.IsTouching_AnyColliderWithFilter_Injected(intPtr, ref contactFilter);
		}

		[ExcludeFromDocs]
		public bool IsTouchingLayers()
		{
			return this.IsTouchingLayers(-1);
		}

		public bool IsTouchingLayers([DefaultValue("Physics2D.AllLayers")] int layerMask)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Collider2D.IsTouchingLayers_Injected(intPtr, layerMask);
		}

		public bool OverlapPoint(Vector2 point)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Collider2D.OverlapPoint_Injected(intPtr, ref point);
		}

		public int Overlap(ContactFilter2D contactFilter, Collider2D[] results)
		{
			return PhysicsScene2D.OverlapCollider(this, contactFilter, results);
		}

		public int Overlap(List<Collider2D> results)
		{
			return PhysicsScene2D.OverlapCollider(this, results);
		}

		public int Overlap(ContactFilter2D contactFilter, List<Collider2D> results)
		{
			return PhysicsScene2D.OverlapCollider(this, contactFilter, results);
		}

		public int Overlap(Vector2 position, float angle, List<Collider2D> results)
		{
			bool flag = this.attachedRigidbody;
			if (flag)
			{
				return PhysicsScene2D.OverlapCollider(position, angle, this, results);
			}
			throw new InvalidOperationException("Cannot perform a Collider Overlap at a specific position and angle if the Collider is not attached to a Rigidbody2D.");
		}

		public int Overlap(Vector2 position, float angle, ContactFilter2D contactFilter, List<Collider2D> results)
		{
			bool flag = this.attachedRigidbody;
			if (flag)
			{
				return PhysicsScene2D.OverlapCollider(position, angle, this, contactFilter, results);
			}
			throw new InvalidOperationException("Cannot perform a Collider Overlap at a specific position and angle if the Collider is not attached to a Rigidbody2D.");
		}

		[ExcludeFromDocs]
		public int Cast(Vector2 direction, RaycastHit2D[] results)
		{
			ContactFilter2D contactFilter2D = default(ContactFilter2D);
			contactFilter2D.useTriggers = Physics2D.queriesHitTriggers;
			contactFilter2D.SetLayerMask(this.contactMask);
			return this.CastArray_Internal(direction, float.PositiveInfinity, contactFilter2D, true, false, results);
		}

		[ExcludeFromDocs]
		public int Cast(Vector2 direction, RaycastHit2D[] results, float distance)
		{
			ContactFilter2D contactFilter2D = default(ContactFilter2D);
			contactFilter2D.useTriggers = Physics2D.queriesHitTriggers;
			contactFilter2D.SetLayerMask(this.contactMask);
			return this.CastArray_Internal(direction, distance, contactFilter2D, true, false, results);
		}

		public int Cast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("true")] bool ignoreSiblingColliders)
		{
			ContactFilter2D contactFilter2D = default(ContactFilter2D);
			contactFilter2D.useTriggers = Physics2D.queriesHitTriggers;
			contactFilter2D.SetLayerMask(this.contactMask);
			return this.CastArray_Internal(direction, distance, contactFilter2D, ignoreSiblingColliders, false, results);
		}

		[ExcludeFromDocs]
		public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return this.CastArray_Internal(direction, float.PositiveInfinity, contactFilter, true, false, results);
		}

		[ExcludeFromDocs]
		public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, float distance)
		{
			return this.CastArray_Internal(direction, distance, contactFilter, true, false, results);
		}

		public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("true")] bool ignoreSiblingColliders)
		{
			return this.CastArray_Internal(direction, distance, contactFilter, ignoreSiblingColliders, false, results);
		}

		public int Cast(Vector2 direction, List<RaycastHit2D> results, [DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity, [DefaultValue("true")] bool ignoreSiblingColliders = true)
		{
			return this.CastList_Internal(direction, distance, ignoreSiblingColliders, false, results);
		}

		public int Cast(Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity, [DefaultValue("true")] bool ignoreSiblingColliders = true)
		{
			return this.CastListFiltered_Internal(direction, distance, contactFilter, ignoreSiblingColliders, false, results);
		}

		public int Cast(Vector2 position, float angle, Vector2 direction, List<RaycastHit2D> results, [DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity, [DefaultValue("true")] bool ignoreSiblingColliders = true)
		{
			bool flag = this.attachedRigidbody;
			if (flag)
			{
				return this.CastFrom_Internal(position, angle, direction, distance, ignoreSiblingColliders, false, results);
			}
			throw new InvalidOperationException("Cannot perform a Collider Cast from a specific position and angle if the Collider is not attached to a Rigidbody2D.");
		}

		public int Cast(Vector2 position, float angle, Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity, [DefaultValue("true")] bool ignoreSiblingColliders = true)
		{
			bool flag = this.attachedRigidbody;
			if (flag)
			{
				return this.CastFromFiltered_Internal(position, angle, direction, distance, contactFilter, ignoreSiblingColliders, false, results);
			}
			throw new InvalidOperationException("Cannot perform a Collider Cast from a specific position and angle if the Collider is not attached to a Rigidbody2D.");
		}

		[NativeMethod("CastArray_Binding")]
		private unsafe int CastArray_Internal(Vector2 direction, float distance, ContactFilter2D contactFilter, bool ignoreSiblingColliders, bool checkIgnoreColliders, [NotNull] RaycastHit2D[] results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<RaycastHit2D> span = new Span<RaycastHit2D>(results);
			int num;
			fixed (RaycastHit2D* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = Collider2D.CastArray_Internal_Injected(intPtr, ref direction, distance, ref contactFilter, ignoreSiblingColliders, checkIgnoreColliders, ref managedSpanWrapper);
			}
			return num;
		}

		[NativeMethod("CastList_Binding")]
		private unsafe int CastList_Internal(Vector2 direction, float distance, bool ignoreSiblingColliders, bool checkIgnoreColliders, [NotNull] List<RaycastHit2D> results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			int num;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList<RaycastHit2D>(results))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, results.Count);
					num = Collider2D.CastList_Internal_Injected(intPtr, ref direction, distance, ignoreSiblingColliders, checkIgnoreColliders, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<RaycastHit2D>(results);
			}
			return num;
		}

		[NativeMethod("CastListFiltered_Binding")]
		private unsafe int CastListFiltered_Internal(Vector2 direction, float distance, ContactFilter2D contactFilter, bool ignoreSiblingColliders, bool checkIgnoreColliders, [NotNull] List<RaycastHit2D> results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			int num;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList<RaycastHit2D>(results))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, results.Count);
					num = Collider2D.CastListFiltered_Internal_Injected(intPtr, ref direction, distance, ref contactFilter, ignoreSiblingColliders, checkIgnoreColliders, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<RaycastHit2D>(results);
			}
			return num;
		}

		[NativeMethod("CastFrom_Binding")]
		private unsafe int CastFrom_Internal(Vector2 position, float angle, Vector2 direction, float distance, bool ignoreSiblingColliders, bool checkIgnoreColliders, [NotNull] List<RaycastHit2D> results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			int num;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList<RaycastHit2D>(results))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, results.Count);
					num = Collider2D.CastFrom_Internal_Injected(intPtr, ref position, angle, ref direction, distance, ignoreSiblingColliders, checkIgnoreColliders, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<RaycastHit2D>(results);
			}
			return num;
		}

		[NativeMethod("CastFromFiltered_Binding")]
		private unsafe int CastFromFiltered_Internal(Vector2 position, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, bool ignoreSiblingColliders, bool checkIgnoreColliders, [NotNull] List<RaycastHit2D> results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			int num;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList<RaycastHit2D>(results))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, results.Count);
					num = Collider2D.CastFromFiltered_Internal_Injected(intPtr, ref position, angle, ref direction, distance, ref contactFilter, ignoreSiblingColliders, checkIgnoreColliders, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<RaycastHit2D>(results);
			}
			return num;
		}

		[ExcludeFromDocs]
		public int Raycast(Vector2 direction, RaycastHit2D[] results)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-1, float.NegativeInfinity, float.PositiveInfinity);
			return this.RaycastArray_Internal(direction, float.PositiveInfinity, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-1, float.NegativeInfinity, float.PositiveInfinity);
			return this.RaycastArray_Internal(direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return this.RaycastArray_Internal(direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return this.RaycastArray_Internal(direction, distance, contactFilter2D, results);
		}

		public int Raycast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("Physics2D.AllLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return this.RaycastArray_Internal(direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public int Raycast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return this.RaycastArray_Internal(direction, float.PositiveInfinity, contactFilter, results);
		}

		public int Raycast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
		{
			return this.RaycastArray_Internal(direction, distance, contactFilter, results);
		}

		[NativeMethod("RaycastArray_Binding")]
		private unsafe int RaycastArray_Internal(Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] RaycastHit2D[] results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<RaycastHit2D> span = new Span<RaycastHit2D>(results);
			int num;
			fixed (RaycastHit2D* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = Collider2D.RaycastArray_Internal_Injected(intPtr, ref direction, distance, ref contactFilter, ref managedSpanWrapper);
			}
			return num;
		}

		public int Raycast(Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
		{
			return this.RaycastList_Internal(direction, distance, contactFilter, results);
		}

		[NativeMethod("RaycastList_Binding")]
		private unsafe int RaycastList_Internal(Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			int num;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList<RaycastHit2D>(results))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, results.Count);
					num = Collider2D.RaycastList_Internal_Injected(intPtr, ref direction, distance, ref contactFilter, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<RaycastHit2D>(results);
			}
			return num;
		}

		public ColliderDistance2D Distance(Collider2D collider)
		{
			return Physics2D.Distance(this, collider);
		}

		public ColliderDistance2D Distance(Vector2 thisPosition, float thisAngle, Collider2D collider, Vector2 position, float angle)
		{
			return Physics2D.Distance(this, thisPosition, thisAngle, collider, position, angle);
		}

		public Vector2 ClosestPoint(Vector2 position)
		{
			return Physics2D.ClosestPoint(position, this);
		}

		public int GetContacts(ContactPoint2D[] contacts)
		{
			return Physics2D.GetContacts(this, ContactFilter2D.noFilter, contacts);
		}

		public int GetContacts(List<ContactPoint2D> contacts)
		{
			return Physics2D.GetContacts(this, ContactFilter2D.noFilter, contacts);
		}

		public int GetContacts(ContactFilter2D contactFilter, ContactPoint2D[] contacts)
		{
			return Physics2D.GetContacts(this, contactFilter, contacts);
		}

		public int GetContacts(ContactFilter2D contactFilter, List<ContactPoint2D> contacts)
		{
			return Physics2D.GetContacts(this, contactFilter, contacts);
		}

		public int GetContacts(Collider2D[] colliders)
		{
			return Physics2D.GetContacts(this, ContactFilter2D.noFilter, colliders);
		}

		public int GetContacts(List<Collider2D> colliders)
		{
			return Physics2D.GetContacts(this, ContactFilter2D.noFilter, colliders);
		}

		public int GetContacts(ContactFilter2D contactFilter, Collider2D[] colliders)
		{
			return Physics2D.GetContacts(this, contactFilter, colliders);
		}

		public int GetContacts(ContactFilter2D contactFilter, List<Collider2D> colliders)
		{
			return Physics2D.GetContacts(this, contactFilter, colliders);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("OverlapCollider has been deprecated. Please use Overlap. (UnityUpgradable) -> Overlap(*)", false)]
		[ExcludeFromDocs]
		public int OverlapCollider(ContactFilter2D contactFilter, Collider2D[] results)
		{
			return this.Overlap(contactFilter, results);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("OverlapCollider has been deprecated. Please use Overlap. (UnityUpgradable) -> Overlap(*)", false)]
		[ExcludeFromDocs]
		public int OverlapCollider(ContactFilter2D contactFilter, List<Collider2D> results)
		{
			return this.Overlap(contactFilter, results);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("usedByComposite has been deprecated. Please use compositeOperation.", false)]
		[ExcludeFromDocs]
		public bool usedByComposite
		{
			get
			{
				return this.compositeOperation > Collider2D.CompositeOperation.None;
			}
			set
			{
				this.compositeOperation = (value ? Collider2D.CompositeOperation.Merge : Collider2D.CompositeOperation.None);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_density_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_density_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isTrigger_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_isTrigger_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_usedByEffector_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_usedByEffector_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D.CompositeOperation get_compositeOperation_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_compositeOperation_Injected(IntPtr _unity_self, Collider2D.CompositeOperation value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_compositeOrder_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_compositeOrder_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_composite_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_offset_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_offset_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_attachedRigidbody_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_localToWorldMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_shapeCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateMesh_Injected(IntPtr _unity_self, bool useBodyPosition, bool useBodyRotation, [DefaultValue("true")] bool useDelaunay);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetShapeHash_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetShapes_Internal_Injected(IntPtr _unity_self, ref PhysicsShapeGroup2D.GroupState physicsShapeGroupState, int shapeIndex, int shapeCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetShapeBounds_Injected(IntPtr _unity_self, ref BlittableListWrapper bounds, bool useRadii, bool useWorldSpace, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_bounds_Injected(IntPtr _unity_self, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ColliderErrorState2D get_errorState_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_compositeCapable_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_sharedMaterial_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sharedMaterial_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_layerOverridePriority_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_layerOverridePriority_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_excludeLayers_Injected(IntPtr _unity_self, out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_excludeLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_includeLayers_Injected(IntPtr _unity_self, out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_includeLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_forceSendLayers_Injected(IntPtr _unity_self, out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_forceSendLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_forceReceiveLayers_Injected(IntPtr _unity_self, out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_forceReceiveLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_contactCaptureLayers_Injected(IntPtr _unity_self, out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_contactCaptureLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_callbackLayers_Injected(IntPtr _unity_self, out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_callbackLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_friction_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_bounciness_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PhysicsMaterialCombine2D get_frictionCombine_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PhysicsMaterialCombine2D get_bounceCombine_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_contactMask_Injected(IntPtr _unity_self, out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CanContact_Injected(IntPtr _unity_self, IntPtr collider);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTouching_Injected(IntPtr _unity_self, IntPtr collider);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTouching_OtherColliderWithFilter_Injected(IntPtr _unity_self, IntPtr collider, [In] ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTouching_AnyColliderWithFilter_Injected(IntPtr _unity_self, [In] ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTouchingLayers_Injected(IntPtr _unity_self, [DefaultValue("Physics2D.AllLayers")] int layerMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool OverlapPoint_Injected(IntPtr _unity_self, [In] ref Vector2 point);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CastArray_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, bool ignoreSiblingColliders, bool checkIgnoreColliders, ref ManagedSpanWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CastList_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, bool ignoreSiblingColliders, bool checkIgnoreColliders, ref BlittableListWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CastListFiltered_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, bool ignoreSiblingColliders, bool checkIgnoreColliders, ref BlittableListWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CastFrom_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 position, float angle, [In] ref Vector2 direction, float distance, bool ignoreSiblingColliders, bool checkIgnoreColliders, ref BlittableListWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CastFromFiltered_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 position, float angle, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, bool ignoreSiblingColliders, bool checkIgnoreColliders, ref BlittableListWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RaycastArray_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, ref ManagedSpanWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RaycastList_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

		public enum CompositeOperation
		{
			None,
			Merge,
			Intersect,
			Difference,
			Flip
		}
	}
}
