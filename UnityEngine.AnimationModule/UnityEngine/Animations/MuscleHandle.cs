using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations
{
	[NativeHeader("Modules/Animation/Animator.h")]
	[MovedFrom("UnityEngine.Experimental.Animations")]
	[NativeHeader("Modules/Animation/MuscleHandle.h")]
	public struct MuscleHandle
	{
		public HumanPartDof humanPartDof { readonly get; private set; }

		public int dof { readonly get; private set; }

		public MuscleHandle(BodyDof bodyDof)
		{
			this.humanPartDof = HumanPartDof.Body;
			this.dof = (int)bodyDof;
		}

		public MuscleHandle(HeadDof headDof)
		{
			this.humanPartDof = HumanPartDof.Head;
			this.dof = (int)headDof;
		}

		public MuscleHandle(HumanPartDof partDof, LegDof legDof)
		{
			bool flag = partDof != HumanPartDof.LeftLeg && partDof != HumanPartDof.RightLeg;
			if (flag)
			{
				throw new InvalidOperationException("Invalid HumanPartDof for a leg, please use either HumanPartDof.LeftLeg or HumanPartDof.RightLeg.");
			}
			this.humanPartDof = partDof;
			this.dof = (int)legDof;
		}

		public MuscleHandle(HumanPartDof partDof, ArmDof armDof)
		{
			bool flag = partDof != HumanPartDof.LeftArm && partDof != HumanPartDof.RightArm;
			if (flag)
			{
				throw new InvalidOperationException("Invalid HumanPartDof for an arm, please use either HumanPartDof.LeftArm or HumanPartDof.RightArm.");
			}
			this.humanPartDof = partDof;
			this.dof = (int)armDof;
		}

		public MuscleHandle(HumanPartDof partDof, FingerDof fingerDof)
		{
			bool flag = partDof < HumanPartDof.LeftThumb || partDof > HumanPartDof.RightLittle;
			if (flag)
			{
				throw new InvalidOperationException("Invalid HumanPartDof for a finger.");
			}
			this.humanPartDof = partDof;
			this.dof = (int)fingerDof;
		}

		public string name
		{
			get
			{
				return this.GetName();
			}
		}

		public static int muscleHandleCount
		{
			get
			{
				return MuscleHandle.GetMuscleHandleCount();
			}
		}

		public unsafe static void GetMuscleHandles([NotNull] [Out] MuscleHandle[] muscleHandles)
		{
			if (muscleHandles == null)
			{
				ThrowHelper.ThrowArgumentNullException(muscleHandles, "muscleHandles");
			}
			try
			{
				fixed (MuscleHandle[] array = muscleHandles)
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					MuscleHandle.GetMuscleHandles_Injected(out blittableArrayWrapper);
				}
			}
			finally
			{
				MuscleHandle[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<MuscleHandle>(ref array);
			}
		}

		private string GetName()
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				MuscleHandle.GetName_Injected(ref this, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMuscleHandleCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMuscleHandles_Injected(out BlittableArrayWrapper muscleHandles);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetName_Injected(ref MuscleHandle _unity_self, out ManagedSpanWrapper ret);
	}
}
