using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequireComponent(typeof(Rigidbody))]
	[NativeClass("Unity::Joint")]
	public class Joint : Component
	{
		public extern Rigidbody connectedBody
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector3 axis
		{
			get
			{
				Vector3 vector;
				this.INTERNAL_get_axis(out vector);
				return vector;
			}
			set
			{
				this.INTERNAL_set_axis(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_axis(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_axis(ref Vector3 value);

		public Vector3 anchor
		{
			get
			{
				Vector3 vector;
				this.INTERNAL_get_anchor(out vector);
				return vector;
			}
			set
			{
				this.INTERNAL_set_anchor(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_anchor(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_anchor(ref Vector3 value);

		public Vector3 connectedAnchor
		{
			get
			{
				Vector3 vector;
				this.INTERNAL_get_connectedAnchor(out vector);
				return vector;
			}
			set
			{
				this.INTERNAL_set_connectedAnchor(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_connectedAnchor(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_connectedAnchor(ref Vector3 value);

		public extern bool autoConfigureConnectedAnchor
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float breakForce
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float breakTorque
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enableCollision
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enablePreprocessing
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector3 currentForce
		{
			get
			{
				Vector3 vector;
				this.INTERNAL_get_currentForce(out vector);
				return vector;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_currentForce(out Vector3 value);

		public Vector3 currentTorque
		{
			get
			{
				Vector3 vector;
				this.INTERNAL_get_currentTorque(out vector);
				return vector;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_currentTorque(out Vector3 value);
	}
}
