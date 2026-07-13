using System;

namespace UnityEngine
{
	public struct HumanPose
	{
		public ReadOnlySpan<Vector3> ikGoalPositions
		{
			get
			{
				return new ReadOnlySpan<Vector3>(this.m_IkGoalPositions);
			}
		}

		public ReadOnlySpan<Quaternion> internalIkGoalRotations
		{
			get
			{
				return new ReadOnlySpan<Quaternion>(this.m_IkGoalRotations);
			}
		}

		public ReadOnlySpan<Quaternion> ikGoalRotations
		{
			get
			{
				return new ReadOnlySpan<Quaternion>(this.m_OffsetIkGoalRotations);
			}
		}

		internal void Init()
		{
			bool flag = this.muscles != null;
			if (flag)
			{
				bool flag2 = this.muscles.Length != HumanTrait.MuscleCount;
				if (flag2)
				{
					throw new InvalidOperationException("Bad array size for HumanPose.muscles. Size must equal HumanTrait.MuscleCount");
				}
			}
			bool flag3 = this.muscles == null;
			if (flag3)
			{
				this.muscles = new float[HumanTrait.MuscleCount];
				bool flag4 = this.bodyRotation.x == 0f && this.bodyRotation.y == 0f && this.bodyRotation.z == 0f && this.bodyRotation.w == 0f;
				if (flag4)
				{
					this.bodyRotation.w = 1f;
				}
			}
			bool flag5 = this.m_IkGoalPositions != null && this.m_IkGoalPositions.Length != HumanPose.k_NumIkGoals;
			if (flag5)
			{
				throw new InvalidOperationException("Bad array size for HumanPose.ikGoalPositions. Size must equal AvatakIKGoal size");
			}
			bool flag6 = this.m_IkGoalPositions == null;
			if (flag6)
			{
				this.m_IkGoalPositions = new Vector3[HumanPose.k_NumIkGoals];
			}
			bool flag7 = this.m_IkGoalRotations != null && this.m_IkGoalRotations.Length != HumanPose.k_NumIkGoals;
			if (flag7)
			{
				throw new InvalidOperationException("Bad array size for HumanPose.ikGoalPositions. Size must equal AvatakIKGoal size");
			}
			bool flag8 = this.m_IkGoalRotations == null;
			if (flag8)
			{
				this.m_IkGoalRotations = new Quaternion[HumanPose.k_NumIkGoals];
			}
			bool flag9 = this.m_OffsetIkGoalRotations != null && this.m_OffsetIkGoalRotations.Length != HumanPose.k_NumIkGoals;
			if (flag9)
			{
				throw new InvalidOperationException("Bad array size for HumanPose.ikGoalPositions. Size must equal AvatakIKGoal size");
			}
			bool flag10 = this.m_OffsetIkGoalRotations == null;
			if (flag10)
			{
				this.m_OffsetIkGoalRotations = new Quaternion[HumanPose.k_NumIkGoals];
			}
		}

		private static int k_NumIkGoals = Enum.GetValues(typeof(AvatarIKGoal)).Length;

		internal static Quaternion[] s_IKGoalOffsets = new Quaternion[]
		{
			new Quaternion(0.5f, -0.5f, 0.5f, 0.5f),
			new Quaternion(0.5f, -0.5f, 0.5f, 0.5f),
			new Quaternion(0.707107f, 0f, 0.707107f, 0f),
			new Quaternion(0f, 0.707107f, 0f, 0.707107f)
		};

		public Vector3 bodyPosition;

		public Quaternion bodyRotation;

		public float[] muscles;

		internal Vector3[] m_IkGoalPositions;

		internal Quaternion[] m_IkGoalRotations;

		internal Quaternion[] m_OffsetIkGoalRotations;
	}
}
