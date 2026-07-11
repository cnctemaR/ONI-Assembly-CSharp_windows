using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Joint is the base class for all joints.</para>
	/// </summary>
	[NativeHeader("Runtime/Dynamics/Joint.h")]
	[NativeClass("Unity::Joint")]
	[RequireComponent(typeof(Rigidbody))]
	public class Joint : Component
	{
		/// <summary>
		///   <para>A reference to another rigidbody this joint connects to.</para>
		/// </summary>
		public extern Rigidbody connectedBody
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The Direction of the axis around which the body is constrained.</para>
		/// </summary>
		public Vector3 axis
		{
			get
			{
				Vector3 vector;
				this.get_axis_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_axis_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The Position of the anchor around which the joints motion is constrained.</para>
		/// </summary>
		public Vector3 anchor
		{
			get
			{
				Vector3 vector;
				this.get_anchor_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_anchor_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Position of the anchor relative to the connected Rigidbody.</para>
		/// </summary>
		public Vector3 connectedAnchor
		{
			get
			{
				Vector3 vector;
				this.get_connectedAnchor_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_connectedAnchor_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Should the connectedAnchor be calculated automatically?</para>
		/// </summary>
		public extern bool autoConfigureConnectedAnchor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The force that needs to be applied for this joint to break.</para>
		/// </summary>
		public extern float breakForce
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The torque that needs to be applied for this joint to break.</para>
		/// </summary>
		public extern float breakTorque
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Enable collision between bodies connected with the joint.</para>
		/// </summary>
		public extern bool enableCollision
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Toggle preprocessing for this joint.</para>
		/// </summary>
		public extern bool enablePreprocessing
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The scale to apply to the inverse mass and inertia tensor of the body prior to solving the constraints.</para>
		/// </summary>
		public extern float massScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The scale to apply to the inverse mass and inertia tensor of the connected body prior to solving the constraints.</para>
		/// </summary>
		public extern float connectedMassScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetCurrentForces(ref Vector3 linearForce, ref Vector3 angularForce);

		/// <summary>
		///   <para>The force applied by the solver to satisfy all constraints.</para>
		/// </summary>
		public Vector3 currentForce
		{
			get
			{
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				this.GetCurrentForces(ref zero, ref zero2);
				return zero;
			}
		}

		/// <summary>
		///   <para>The torque applied by the solver to satisfy all constraints.</para>
		/// </summary>
		public Vector3 currentTorque
		{
			get
			{
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				this.GetCurrentForces(ref zero, ref zero2);
				return zero2;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_axis_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_axis_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_anchor_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_anchor_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_connectedAnchor_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_connectedAnchor_Injected(ref Vector3 value);
	}
}
