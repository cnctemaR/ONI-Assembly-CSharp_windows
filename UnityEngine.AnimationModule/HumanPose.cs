using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Retargetable humanoid pose.</para>
	/// </summary>
	public struct HumanPose
	{
		internal void Init()
		{
			if (this.muscles != null)
			{
				if (this.muscles.Length != HumanTrait.MuscleCount)
				{
					throw new InvalidOperationException("Bad array size for HumanPose.muscles. Size must equal HumanTrait.MuscleCount");
				}
			}
			if (this.muscles == null)
			{
				this.muscles = new float[HumanTrait.MuscleCount];
				if (this.bodyRotation.x == 0f && this.bodyRotation.y == 0f && this.bodyRotation.z == 0f && this.bodyRotation.w == 0f)
				{
					this.bodyRotation.w = 1f;
				}
			}
		}

		/// <summary>
		///   <para>The human body position for that pose.</para>
		/// </summary>
		public Vector3 bodyPosition;

		/// <summary>
		///   <para>The human body orientation for that pose.</para>
		/// </summary>
		public Quaternion bodyRotation;

		/// <summary>
		///   <para>The array of muscle values for that pose.</para>
		/// </summary>
		public float[] muscles;
	}
}
