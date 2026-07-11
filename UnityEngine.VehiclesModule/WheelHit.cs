using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Contact information for the wheel, reported by WheelCollider.</para>
	/// </summary>
	[NativeHeader("Runtime/Vehicles/WheelCollider.h")]
	public struct WheelHit
	{
		/// <summary>
		///   <para>The other Collider the wheel is hitting.</para>
		/// </summary>
		public Collider collider
		{
			get
			{
				return this.m_Collider;
			}
			set
			{
				this.m_Collider = value;
			}
		}

		/// <summary>
		///   <para>The point of contact between the wheel and the ground.</para>
		/// </summary>
		public Vector3 point
		{
			get
			{
				return this.m_Point;
			}
			set
			{
				this.m_Point = value;
			}
		}

		/// <summary>
		///   <para>The normal at the point of contact.</para>
		/// </summary>
		public Vector3 normal
		{
			get
			{
				return this.m_Normal;
			}
			set
			{
				this.m_Normal = value;
			}
		}

		/// <summary>
		///   <para>The direction the wheel is pointing in.</para>
		/// </summary>
		public Vector3 forwardDir
		{
			get
			{
				return this.m_ForwardDir;
			}
			set
			{
				this.m_ForwardDir = value;
			}
		}

		/// <summary>
		///   <para>The sideways direction of the wheel.</para>
		/// </summary>
		public Vector3 sidewaysDir
		{
			get
			{
				return this.m_SidewaysDir;
			}
			set
			{
				this.m_SidewaysDir = value;
			}
		}

		/// <summary>
		///   <para>The magnitude of the force being applied for the contact.</para>
		/// </summary>
		public float force
		{
			get
			{
				return this.m_Force;
			}
			set
			{
				this.m_Force = value;
			}
		}

		/// <summary>
		///   <para>Tire slip in the rolling direction. Acceleration slip is negative, braking slip is positive.</para>
		/// </summary>
		public float forwardSlip
		{
			get
			{
				return this.m_ForwardSlip;
			}
			set
			{
				this.m_ForwardSlip = value;
			}
		}

		/// <summary>
		///   <para>Tire slip in the sideways direction.</para>
		/// </summary>
		public float sidewaysSlip
		{
			get
			{
				return this.m_SidewaysSlip;
			}
			set
			{
				this.m_SidewaysSlip = value;
			}
		}

		[NativeName("point")]
		private Vector3 m_Point;

		[NativeName("normal")]
		private Vector3 m_Normal;

		[NativeName("forwardDir")]
		private Vector3 m_ForwardDir;

		[NativeName("sidewaysDir")]
		private Vector3 m_SidewaysDir;

		[NativeName("force")]
		private float m_Force;

		[NativeName("forwardSlip")]
		private float m_ForwardSlip;

		[NativeName("sidewaysSlip")]
		private float m_SidewaysSlip;

		[NativeName("collider")]
		private Collider m_Collider;
	}
}
