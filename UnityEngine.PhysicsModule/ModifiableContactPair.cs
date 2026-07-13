using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics/PhysXContactModification.h")]
	[NativeHeader("Modules/Physics/PhysicsCollisionGeometry.h")]
	public struct ModifiableContactPair
	{
		[FreeFunction("Physics::PhysXGeometryExtension::TranslateTriangleIndex", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern uint TranslateTriangleIndex(IntPtr shapePtr, uint rawIndex);

		[FreeFunction("Physics::PhysXContactModificationExtension::ResolveShapeToInstanceID", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int ResolveShapeToInstanceID(IntPtr shapePtr);

		[FreeFunction("Physics::PhysXContactModificationExtension::ResolveActorToInstanceID", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int ResolveActorToInstanceID(IntPtr actorPtr);

		[FreeFunction("Physics::PhysXContactModificationExtension::GetActorLinearVelocity", true)]
		internal static Vector3 GetActorLinearVelocity(IntPtr actorPtr)
		{
			Vector3 vector;
			ModifiableContactPair.GetActorLinearVelocity_Injected(actorPtr, out vector);
			return vector;
		}

		[FreeFunction("Physics::PhysXContactModificationExtension::GetActorAngularVelocity", true)]
		internal static Vector3 GetActorAngularVelocity(IntPtr actorPtr)
		{
			Vector3 vector;
			ModifiableContactPair.GetActorAngularVelocity_Injected(actorPtr, out vector);
			return vector;
		}

		public int colliderInstanceID
		{
			get
			{
				return ModifiableContactPair.ResolveShapeToInstanceID(this.shape);
			}
		}

		public int otherColliderInstanceID
		{
			get
			{
				return ModifiableContactPair.ResolveShapeToInstanceID(this.otherShape);
			}
		}

		public int bodyInstanceID
		{
			get
			{
				return ModifiableContactPair.ResolveActorToInstanceID(this.actor);
			}
		}

		public int otherBodyInstanceID
		{
			get
			{
				return ModifiableContactPair.ResolveActorToInstanceID(this.otherActor);
			}
		}

		public Vector3 bodyVelocity
		{
			get
			{
				return ModifiableContactPair.GetActorLinearVelocity(this.actor);
			}
		}

		public Vector3 bodyAngularVelocity
		{
			get
			{
				return ModifiableContactPair.GetActorAngularVelocity(this.actor);
			}
		}

		public Vector3 otherBodyVelocity
		{
			get
			{
				return ModifiableContactPair.GetActorLinearVelocity(this.otherActor);
			}
		}

		public Vector3 otherBodyAngularVelocity
		{
			get
			{
				return ModifiableContactPair.GetActorAngularVelocity(this.otherActor);
			}
		}

		public int contactCount
		{
			get
			{
				return this.numContacts;
			}
		}

		public unsafe ModifiableMassProperties massProperties
		{
			get
			{
				return this.GetContactPatch()->massProperties;
			}
			set
			{
				ModifiableContactPatch* contactPatch = this.GetContactPatch();
				contactPatch->massProperties = value;
				ModifiableContactPatch* ptr = contactPatch;
				ptr->internalFlags = ptr->internalFlags | 8;
			}
		}

		public unsafe Vector3 GetPoint(int i)
		{
			return this.GetContact(i)->contact;
		}

		public unsafe void SetPoint(int i, Vector3 v)
		{
			this.GetContact(i)->contact = v;
		}

		public unsafe Vector3 GetNormal(int i)
		{
			return this.GetContact(i)->normal;
		}

		public unsafe void SetNormal(int i, Vector3 normal)
		{
			this.GetContact(i)->normal = normal;
			ModifiableContactPatch* contactPatch = this.GetContactPatch();
			contactPatch->internalFlags = contactPatch->internalFlags | 64;
		}

		public unsafe float GetSeparation(int i)
		{
			return this.GetContact(i)->separation;
		}

		public unsafe void SetSeparation(int i, float separation)
		{
			this.GetContact(i)->separation = separation;
		}

		public unsafe Vector3 GetTargetVelocity(int i)
		{
			return this.GetContact(i)->targetVelocity;
		}

		public unsafe void SetTargetVelocity(int i, Vector3 velocity)
		{
			this.GetContact(i)->targetVelocity = velocity;
			ModifiableContactPatch* contactPatch = this.GetContactPatch();
			contactPatch->internalFlags = contactPatch->internalFlags | 16;
		}

		public unsafe float GetBounciness(int i)
		{
			return this.GetContact(i)->restitution;
		}

		public unsafe void SetBounciness(int i, float bounciness)
		{
			this.GetContact(i)->restitution = bounciness;
			ModifiableContactPatch* contactPatch = this.GetContactPatch();
			contactPatch->internalFlags = contactPatch->internalFlags | 64;
		}

		public unsafe float GetStaticFriction(int i)
		{
			return this.GetContact(i)->staticFriction;
		}

		public unsafe void SetStaticFriction(int i, float staticFriction)
		{
			this.GetContact(i)->staticFriction = staticFriction;
			ModifiableContactPatch* contactPatch = this.GetContactPatch();
			contactPatch->internalFlags = contactPatch->internalFlags | 64;
		}

		public unsafe float GetDynamicFriction(int i)
		{
			return this.GetContact(i)->dynamicFriction;
		}

		public unsafe void SetDynamicFriction(int i, float dynamicFriction)
		{
			this.GetContact(i)->dynamicFriction = dynamicFriction;
			ModifiableContactPatch* contactPatch = this.GetContactPatch();
			contactPatch->internalFlags = contactPatch->internalFlags | 64;
		}

		public unsafe float GetMaxImpulse(int i)
		{
			return this.GetContact(i)->maxImpulse;
		}

		public unsafe void SetMaxImpulse(int i, float value)
		{
			this.GetContact(i)->maxImpulse = value;
			ModifiableContactPatch* contactPatch = this.GetContactPatch();
			contactPatch->internalFlags = contactPatch->internalFlags | 32;
		}

		public void IgnoreContact(int i)
		{
			this.SetMaxImpulse(i, 0f);
		}

		public unsafe uint GetFaceIndex(int i)
		{
			bool flag = (this.GetContactPatch()->internalFlags & 1) > 0;
			uint num2;
			if (flag)
			{
				IntPtr intPtr = new IntPtr(this.contacts.ToInt64() + (long)(this.numContacts * sizeof(ModifiableContact)) + (long)((this.numContacts + i) * 4));
				uint num = *(uint*)(void*)intPtr;
				num2 = ModifiableContactPair.TranslateTriangleIndex(this.otherShape, num);
			}
			else
			{
				num2 = uint.MaxValue;
			}
			return num2;
		}

		private unsafe ModifiableContact* GetContact(int index)
		{
			IntPtr intPtr = new IntPtr(this.contacts.ToInt64() + (long)(index * sizeof(ModifiableContact)));
			return (ModifiableContact*)(void*)intPtr;
		}

		private unsafe ModifiableContactPatch* GetContactPatch()
		{
			IntPtr intPtr = new IntPtr(this.contacts.ToInt64() - (long)(this.numContacts * sizeof(ModifiableContactPatch)));
			return (ModifiableContactPatch*)(void*)intPtr;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetActorLinearVelocity_Injected(IntPtr actorPtr, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetActorAngularVelocity_Injected(IntPtr actorPtr, out Vector3 ret);

		private IntPtr actor;

		private IntPtr otherActor;

		private IntPtr shape;

		private IntPtr otherShape;

		public Quaternion rotation;

		public Vector3 position;

		public Quaternion otherRotation;

		public Vector3 otherPosition;

		private int numContacts;

		private IntPtr contacts;
	}
}
