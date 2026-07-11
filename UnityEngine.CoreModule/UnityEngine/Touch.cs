using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Structure describing the status of a finger touching the screen.</para>
	/// </summary>
	public struct Touch
	{
		/// <summary>
		///   <para>The unique index for the touch.</para>
		/// </summary>
		public int fingerId
		{
			get
			{
				return this.m_FingerId;
			}
			set
			{
				this.m_FingerId = value;
			}
		}

		/// <summary>
		///   <para>The position of the touch in pixel coordinates.</para>
		/// </summary>
		public Vector2 position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				this.m_Position = value;
			}
		}

		/// <summary>
		///   <para>The raw position used for the touch.</para>
		/// </summary>
		public Vector2 rawPosition
		{
			get
			{
				return this.m_RawPosition;
			}
			set
			{
				this.m_RawPosition = value;
			}
		}

		/// <summary>
		///   <para>The position delta since last change.</para>
		/// </summary>
		public Vector2 deltaPosition
		{
			get
			{
				return this.m_PositionDelta;
			}
			set
			{
				this.m_PositionDelta = value;
			}
		}

		/// <summary>
		///   <para>Amount of time that has passed since the last recorded change in Touch values.</para>
		/// </summary>
		public float deltaTime
		{
			get
			{
				return this.m_TimeDelta;
			}
			set
			{
				this.m_TimeDelta = value;
			}
		}

		/// <summary>
		///   <para>Number of taps.</para>
		/// </summary>
		public int tapCount
		{
			get
			{
				return this.m_TapCount;
			}
			set
			{
				this.m_TapCount = value;
			}
		}

		/// <summary>
		///   <para>Describes the phase of the touch.</para>
		/// </summary>
		public TouchPhase phase
		{
			get
			{
				return this.m_Phase;
			}
			set
			{
				this.m_Phase = value;
			}
		}

		/// <summary>
		///   <para>The current amount of pressure being applied to a touch.  1.0f is considered to be the pressure of an average touch.  If Input.touchPressureSupported returns false, the value of this property will always be 1.0f.</para>
		/// </summary>
		public float pressure
		{
			get
			{
				return this.m_Pressure;
			}
			set
			{
				this.m_Pressure = value;
			}
		}

		/// <summary>
		///   <para>The maximum possible pressure value for a platform.  If Input.touchPressureSupported returns false, the value of this property will always be 1.0f.</para>
		/// </summary>
		public float maximumPossiblePressure
		{
			get
			{
				return this.m_maximumPossiblePressure;
			}
			set
			{
				this.m_maximumPossiblePressure = value;
			}
		}

		/// <summary>
		///   <para>A value that indicates whether a touch was of Direct, Indirect (or remote), or Stylus type.</para>
		/// </summary>
		public TouchType type
		{
			get
			{
				return this.m_Type;
			}
			set
			{
				this.m_Type = value;
			}
		}

		/// <summary>
		///   <para>Value of 0 radians indicates that the stylus is parallel to the surface, pi/2 indicates that it is perpendicular.</para>
		/// </summary>
		public float altitudeAngle
		{
			get
			{
				return this.m_AltitudeAngle;
			}
			set
			{
				this.m_AltitudeAngle = value;
			}
		}

		/// <summary>
		///   <para>Value of 0 radians indicates that the stylus is pointed along the x-axis of the device.</para>
		/// </summary>
		public float azimuthAngle
		{
			get
			{
				return this.m_AzimuthAngle;
			}
			set
			{
				this.m_AzimuthAngle = value;
			}
		}

		/// <summary>
		///   <para>An estimated value of the radius of a touch.  Add radiusVariance to get the maximum touch size, subtract it to get the minimum touch size.</para>
		/// </summary>
		public float radius
		{
			get
			{
				return this.m_Radius;
			}
			set
			{
				this.m_Radius = value;
			}
		}

		/// <summary>
		///   <para>This value determines the accuracy of the touch radius. Add this value to the radius to get the maximum touch size, subtract it to get the minimum touch size.</para>
		/// </summary>
		public float radiusVariance
		{
			get
			{
				return this.m_RadiusVariance;
			}
			set
			{
				this.m_RadiusVariance = value;
			}
		}

		private int m_FingerId;

		private Vector2 m_Position;

		private Vector2 m_RawPosition;

		private Vector2 m_PositionDelta;

		private float m_TimeDelta;

		private int m_TapCount;

		private TouchPhase m_Phase;

		private TouchType m_Type;

		private float m_Pressure;

		private float m_maximumPossiblePressure;

		private float m_Radius;

		private float m_RadiusVariance;

		private float m_AltitudeAngle;

		private float m_AzimuthAngle;
	}
}
