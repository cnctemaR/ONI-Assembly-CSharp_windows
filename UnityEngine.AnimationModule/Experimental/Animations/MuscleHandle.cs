using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.Animations
{
	/// <summary>
	///   <para>Handle for a muscle in the AnimationHumanStream.</para>
	/// </summary>
	[NativeHeader("Runtime/Animation/MuscleHandle.h")]
	[NativeHeader("Runtime/Animation/Animator.h")]
	public struct MuscleHandle
	{
		/// <summary>
		///   <para>The different constructors that creates the muscle handle.</para>
		/// </summary>
		/// <param name="bodyDof">The muscle body sub-part.</param>
		/// <param name="headDof">The muscle head sub-part.</param>
		/// <param name="partDof">The muscle human part.</param>
		/// <param name="legDof">The muscle leg sub-part.</param>
		/// <param name="armDof">The muscle arm sub-part.</param>
		/// <param name="fingerDof">The muscle finger sub-part.</param>
		public MuscleHandle(BodyDof bodyDof)
		{
			this.humanPartDof = HumanPartDof.Body;
			this.dof = (int)bodyDof;
		}

		/// <summary>
		///   <para>The different constructors that creates the muscle handle.</para>
		/// </summary>
		/// <param name="bodyDof">The muscle body sub-part.</param>
		/// <param name="headDof">The muscle head sub-part.</param>
		/// <param name="partDof">The muscle human part.</param>
		/// <param name="legDof">The muscle leg sub-part.</param>
		/// <param name="armDof">The muscle arm sub-part.</param>
		/// <param name="fingerDof">The muscle finger sub-part.</param>
		public MuscleHandle(HeadDof headDof)
		{
			this.humanPartDof = HumanPartDof.Head;
			this.dof = (int)headDof;
		}

		/// <summary>
		///   <para>The different constructors that creates the muscle handle.</para>
		/// </summary>
		/// <param name="bodyDof">The muscle body sub-part.</param>
		/// <param name="headDof">The muscle head sub-part.</param>
		/// <param name="partDof">The muscle human part.</param>
		/// <param name="legDof">The muscle leg sub-part.</param>
		/// <param name="armDof">The muscle arm sub-part.</param>
		/// <param name="fingerDof">The muscle finger sub-part.</param>
		public MuscleHandle(HumanPartDof partDof, LegDof legDof)
		{
			if (partDof != HumanPartDof.LeftLeg && partDof != HumanPartDof.RightLeg)
			{
				throw new InvalidOperationException("Invalid HumanPartDof for a leg, please use either HumanPartDof.LeftLeg or HumanPartDof.RightLeg.");
			}
			this.humanPartDof = partDof;
			this.dof = (int)legDof;
		}

		/// <summary>
		///   <para>The different constructors that creates the muscle handle.</para>
		/// </summary>
		/// <param name="bodyDof">The muscle body sub-part.</param>
		/// <param name="headDof">The muscle head sub-part.</param>
		/// <param name="partDof">The muscle human part.</param>
		/// <param name="legDof">The muscle leg sub-part.</param>
		/// <param name="armDof">The muscle arm sub-part.</param>
		/// <param name="fingerDof">The muscle finger sub-part.</param>
		public MuscleHandle(HumanPartDof partDof, ArmDof armDof)
		{
			if (partDof != HumanPartDof.LeftArm && partDof != HumanPartDof.RightArm)
			{
				throw new InvalidOperationException("Invalid HumanPartDof for an arm, please use either HumanPartDof.LeftArm or HumanPartDof.RightArm.");
			}
			this.humanPartDof = partDof;
			this.dof = (int)armDof;
		}

		/// <summary>
		///   <para>The different constructors that creates the muscle handle.</para>
		/// </summary>
		/// <param name="bodyDof">The muscle body sub-part.</param>
		/// <param name="headDof">The muscle head sub-part.</param>
		/// <param name="partDof">The muscle human part.</param>
		/// <param name="legDof">The muscle leg sub-part.</param>
		/// <param name="armDof">The muscle arm sub-part.</param>
		/// <param name="fingerDof">The muscle finger sub-part.</param>
		public MuscleHandle(HumanPartDof partDof, FingerDof fingerDof)
		{
			if (partDof < HumanPartDof.LeftThumb || partDof > HumanPartDof.RightLittle)
			{
				throw new InvalidOperationException("Invalid HumanPartDof for a finger.");
			}
			this.humanPartDof = partDof;
			this.dof = (int)fingerDof;
		}

		/// <summary>
		///   <para>The muscle human part. (Read Only)</para>
		/// </summary>
		public HumanPartDof humanPartDof { get; private set; }

		/// <summary>
		///   <para>The muscle human sub-part. (Read Only)</para>
		/// </summary>
		public int dof { get; private set; }

		/// <summary>
		///   <para>The name of the muscle. (Read Only)</para>
		/// </summary>
		public string name
		{
			get
			{
				return this.GetName();
			}
		}

		/// <summary>
		///   <para>The total number of DoF parts in a humanoid. (Read Only)</para>
		/// </summary>
		public static int muscleHandleCount
		{
			get
			{
				return MuscleHandle.GetMuscleHandleCount();
			}
		}

		/// <summary>
		///   <para>Fills the array with all the possible muscle handles on a humanoid.</para>
		/// </summary>
		/// <param name="muscleHandles">An array of MuscleHandle.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetMuscleHandles([NotNull] [Out] MuscleHandle[] muscleHandles);

		private string GetName()
		{
			return MuscleHandle.GetName_Injected(ref this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMuscleHandleCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetName_Injected(ref MuscleHandle _unity_self);
	}
}
