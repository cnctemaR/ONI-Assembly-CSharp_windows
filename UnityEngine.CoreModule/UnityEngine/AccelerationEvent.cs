using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Structure describing acceleration status of the device.</para>
	/// </summary>
	public struct AccelerationEvent
	{
		/// <summary>
		///   <para>Value of acceleration.</para>
		/// </summary>
		public Vector3 acceleration
		{
			get
			{
				return new Vector3(this.x, this.y, this.z);
			}
		}

		/// <summary>
		///   <para>Amount of time passed since last accelerometer measurement.</para>
		/// </summary>
		public float deltaTime
		{
			get
			{
				return this.m_TimeDelta;
			}
		}

		private float x;

		private float y;

		private float z;

		private float m_TimeDelta;
	}
}
