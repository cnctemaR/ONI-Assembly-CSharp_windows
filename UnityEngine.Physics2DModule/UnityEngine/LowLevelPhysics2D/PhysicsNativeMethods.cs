using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Jobs;
using UnityEngine.Scripting;

namespace UnityEngine.LowLevelPhysics2D
{
	[RequiredByNativeCode(GenerateProxy = true)]
	internal readonly struct PhysicsNativeMethods
	{
		[RequiredByNativeCode]
		private static void CreateWorldTransformAccessArray(PhysicsWorld world, int capacity, int desiredJobCount)
		{
			int num = (int)(world.m_Index1 - 1);
			TransformAccessArray transformAccessArray = PhysicsNativeMethods.s_WorldTransformAccessArrays[num];
			bool isCreated = transformAccessArray.isCreated;
			if (isCreated)
			{
				transformAccessArray.Dispose();
			}
			transformAccessArray = new TransformAccessArray(capacity, desiredJobCount);
			PhysicsNativeMethods.s_WorldTransformAccessArrays[num] = transformAccessArray;
		}

		[RequiredByNativeCode]
		private static void DestroyWorldTransformAccessArray(PhysicsWorld world)
		{
			int num = (int)(world.m_Index1 - 1);
			TransformAccessArray transformAccessArray = PhysicsNativeMethods.s_WorldTransformAccessArrays[num];
			bool isCreated = transformAccessArray.isCreated;
			if (isCreated)
			{
				transformAccessArray.Dispose();
			}
			PhysicsNativeMethods.s_WorldTransformAccessArrays[num] = default(TransformAccessArray);
		}

		private static TransformAccessArray GetWorldTransformAccessArray(PhysicsWorld world)
		{
			int num = (int)(world.m_Index1 - 1);
			TransformAccessArray transformAccessArray = PhysicsNativeMethods.s_WorldTransformAccessArrays[num];
			bool isCreated = transformAccessArray.isCreated;
			if (isCreated)
			{
				return transformAccessArray;
			}
			throw new InvalidOperationException(string.Format("Cannot access world transform access array for world {0}", world));
		}

		[RequiredByNativeCode]
		private unsafe static void WriteWorldTransforms(PhysicsWorld world, PhysicsWorld.TransformWriteMode transformWriteMode, PhysicsWorld.TransformPlane transformPlane, int eventCount, bool transformTweening)
		{
			TransformAccessArray worldTransformAccessArray = PhysicsNativeMethods.GetWorldTransformAccessArray(world);
			NativeArray<PhysicsBody.TransformWriteTween> nativeArray = new NativeArray<PhysicsBody.TransformWriteTween>(eventCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			int num = PhysicsLowLevelScripting2D.PhysicsGlobal_PopulateWorldTransformWrite(world, new IntPtr((void*)(&worldTransformAccessArray)), nativeArray.AsSpan());
			bool flag = num > 0;
			if (flag)
			{
				bool flag2 = transformWriteMode == PhysicsWorld.TransformWriteMode.Fast2D;
				if (flag2)
				{
					new PhysicsNativeMethods.FastWriteTransformsJob
					{
						TransformTweening = transformTweening,
						TransformWriteTweens = nativeArray,
						TransformPlane = transformPlane
					}.Schedule(worldTransformAccessArray, default(JobHandle)).Complete();
				}
				else
				{
					bool flag3 = transformWriteMode == PhysicsWorld.TransformWriteMode.Slow3D;
					if (!flag3)
					{
						throw new Exception("Invalid PhysicsWorld Transform Write Mode.");
					}
					new PhysicsNativeMethods.Slow3DWriteTransformsJob
					{
						TransformTweening = transformTweening,
						TransformWriteTweens = nativeArray,
						TransformPlane = transformPlane
					}.Schedule(worldTransformAccessArray, default(JobHandle)).Complete();
				}
				if (transformTweening)
				{
					world.SetTransformWriteTweens(new Span<PhysicsBody.TransformWriteTween>(nativeArray.GetUnsafeReadOnlyPtr<PhysicsBody.TransformWriteTween>(), num));
				}
			}
			nativeArray.Dispose();
		}

		[RequiredByNativeCode]
		private static void WriteTransformTweens(PhysicsWorld world, double lastSimulationTimestamp, float lastSimulationDeltaTime, PhysicsWorld.TransformWriteMode transformWriteMode, PhysicsWorld.TransformPlane transformPlane, PhysicsLowLevelScripting2D.PhysicsBuffer transformWriteTweensBuffer)
		{
			bool flag = transformWriteMode == PhysicsWorld.TransformWriteMode.Off || transformWriteTweensBuffer.IsEmpty;
			if (!flag)
			{
				NativeArray<PhysicsBody.TransformWriteTween> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<PhysicsBody.TransformWriteTween>(transformWriteTweensBuffer.ToSpan<PhysicsBody.TransformWriteTween>(), Allocator.None);
				int length = nativeArray.Length;
				TransformAccessArray worldTransformAccessArray = PhysicsNativeMethods.GetWorldTransformAccessArray(world);
				bool flag2 = length == worldTransformAccessArray.length;
				if (flag2)
				{
					float num = (float)(Time.timeAsDouble - lastSimulationTimestamp);
					float num2 = Mathf.Clamp01(num / lastSimulationDeltaTime);
					float num3 = num;
					new PhysicsNativeMethods.WriteTransformTweensJob
					{
						TransformWriteTweens = nativeArray,
						TransformWriteMode = transformWriteMode,
						TransformPlane = transformPlane,
						InterpolationTime = num2,
						ExtrapolationTime = num3
					}.Schedule(worldTransformAccessArray, default(JobHandle)).Complete();
				}
			}
		}

		private static TransformAccessArray[] s_WorldTransformAccessArrays = new TransformAccessArray[128];

		private struct FastWriteTransformsJob : IJobParallelForTransform
		{
			public void Execute(int index, TransformAccess transform)
			{
				bool flag = !transform.isValid;
				if (!flag)
				{
					PhysicsBody.TransformWriteTween transformWriteTween = this.TransformWriteTweens[index];
					Vector2 vector;
					PhysicsRotate physicsRotate;
					transformWriteTween.physicsTransform.GetPositionAndRotation(out vector, out physicsRotate);
					Vector3 vector2 = PhysicsMath.ToPosition3D(vector, transformWriteTween.positionFrom, this.TransformPlane);
					Quaternion quaternion = PhysicsMath.ToRotationFast3D(physicsRotate.angle, this.TransformPlane);
					transform.SetPositionAndRotation(vector2, quaternion);
					bool flag2 = !this.TransformTweening || transformWriteTween.transformWriteMode != PhysicsBody.TransformWriteMode.Extrapolate;
					if (!flag2)
					{
						transformWriteTween.positionFrom = vector2;
						transformWriteTween.rotationFrom = quaternion;
						this.TransformWriteTweens[index] = transformWriteTween;
					}
				}
			}

			public NativeArray<PhysicsBody.TransformWriteTween> TransformWriteTweens;

			[ReadOnly]
			public PhysicsWorld.TransformPlane TransformPlane;

			[ReadOnly]
			public bool TransformTweening;
		}

		private struct Slow3DWriteTransformsJob : IJobParallelForTransform
		{
			public void Execute(int index, TransformAccess transform)
			{
				bool flag = !transform.isValid;
				if (!flag)
				{
					PhysicsBody.TransformWriteTween transformWriteTween = this.TransformWriteTweens[index];
					Vector2 vector;
					PhysicsRotate physicsRotate;
					transformWriteTween.physicsTransform.GetPositionAndRotation(out vector, out physicsRotate);
					Vector3 vector2 = PhysicsMath.ToPosition3D(vector, transformWriteTween.positionFrom, this.TransformPlane);
					Quaternion quaternion = PhysicsMath.ToRotationSlow3D(physicsRotate.angle, transformWriteTween.rotationFrom, this.TransformPlane);
					transform.SetPositionAndRotation(vector2, quaternion);
					bool flag2 = !this.TransformTweening || transformWriteTween.transformWriteMode != PhysicsBody.TransformWriteMode.Extrapolate;
					if (!flag2)
					{
						transformWriteTween.positionFrom = vector2;
						transformWriteTween.rotationFrom = quaternion;
						this.TransformWriteTweens[index] = transformWriteTween;
					}
				}
			}

			public NativeArray<PhysicsBody.TransformWriteTween> TransformWriteTweens;

			[ReadOnly]
			public PhysicsWorld.TransformPlane TransformPlane;

			[ReadOnly]
			public bool TransformTweening;
		}

		private struct WriteTransformTweensJob : IJobParallelForTransform
		{
			public void Execute(int index, TransformAccess transform)
			{
				bool flag = !transform.isValid;
				if (!flag)
				{
					PhysicsBody.TransformWriteTween transformWriteTween = this.TransformWriteTweens[index];
					bool flag2 = !transformWriteTween.body.isValid;
					if (!flag2)
					{
						PhysicsBody.TransformWriteMode transformWriteMode = transformWriteTween.transformWriteMode;
						bool flag3 = transformWriteMode == PhysicsBody.TransformWriteMode.Interpolate;
						if (flag3)
						{
							Vector3 positionFrom = transformWriteTween.positionFrom;
							Quaternion rotationFrom = transformWriteTween.rotationFrom;
							PhysicsTransform physicsTransform = transformWriteTween.physicsTransform;
							Vector3 vector = PhysicsMath.ToPosition3D(physicsTransform.position, positionFrom, this.TransformPlane);
							Quaternion quaternion = ((this.TransformWriteMode == PhysicsWorld.TransformWriteMode.Fast2D) ? PhysicsMath.ToRotationFast3D(physicsTransform.rotation.angle, this.TransformPlane) : PhysicsMath.ToRotationSlow3D(physicsTransform.rotation.angle, rotationFrom, this.TransformPlane));
							Vector3 vector2 = Vector3.Lerp(positionFrom, vector, this.InterpolationTime);
							Quaternion quaternion2 = Quaternion.Slerp(rotationFrom, quaternion, this.InterpolationTime);
							transform.SetPositionAndRotation(vector2, quaternion2);
						}
						else
						{
							bool flag4 = transformWriteMode == PhysicsBody.TransformWriteMode.Extrapolate;
							if (flag4)
							{
								Vector2 linearVelocity = transformWriteTween.linearVelocity;
								Vector3 vector3 = PhysicsMath.Swizzle(new Vector3(linearVelocity.x * this.ExtrapolationTime, linearVelocity.y * this.ExtrapolationTime, 0f), this.TransformPlane);
								Vector3 positionFrom2 = transformWriteTween.positionFrom;
								Quaternion rotationFrom2 = transformWriteTween.rotationFrom;
								float angularVelocity = transformWriteTween.angularVelocity;
								Vector3 vector4 = positionFrom2 + vector3;
								Quaternion quaternion3 = PhysicsMath.AngularVelocityToQuaternion(angularVelocity, this.ExtrapolationTime, this.TransformPlane) * rotationFrom2;
								transform.SetPositionAndRotation(vector4, quaternion3);
							}
						}
					}
				}
			}

			[ReadOnly]
			public NativeArray<PhysicsBody.TransformWriteTween> TransformWriteTweens;

			[ReadOnly]
			public PhysicsWorld.TransformWriteMode TransformWriteMode;

			[ReadOnly]
			public PhysicsWorld.TransformPlane TransformPlane;

			[ReadOnly]
			public float InterpolationTime;

			[ReadOnly]
			public float ExtrapolationTime;
		}
	}
}
