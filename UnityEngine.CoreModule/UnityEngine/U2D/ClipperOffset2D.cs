using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.U2D
{
	[NativeType(Header = "Runtime/2D/Common/ClipperOffsetWrapper.h")]
	internal struct ClipperOffset2D
	{
		public unsafe static void Execute(ref ClipperOffset2D.Solution solution, NativeArray<Vector2> inPoints, NativeArray<int> inPathSizes, NativeArray<ClipperOffset2D.PathArguments> inPathArguments, Allocator inSolutionAllocator, double inDelta = 0.0, double inMiterLimit = 2.0, double inRoundPrecision = 0.25, double inArcTolerance = 0.0, double inIntScale = 65536.0, bool useRounding = false)
		{
			IntPtr intPtr;
			int num;
			IntPtr intPtr2;
			int num2;
			ClipperOffset2D.Internal_Execute(out intPtr, out num, out intPtr2, out num2, new IntPtr(inPoints.m_Buffer), inPoints.Length, new IntPtr(inPathSizes.m_Buffer), new IntPtr(inPathArguments.m_Buffer), inPathSizes.Length, inDelta, inMiterLimit, inRoundPrecision, inArcTolerance, inIntScale, useRounding);
			bool flag = !solution.pathSizes.IsCreated;
			if (flag)
			{
				solution.pathSizes = new NativeArray<int>(num2, inSolutionAllocator, NativeArrayOptions.ClearMemory);
			}
			bool flag2 = !solution.points.IsCreated;
			if (flag2)
			{
				solution.points = new NativeArray<Vector2>(num, inSolutionAllocator, NativeArrayOptions.ClearMemory);
			}
			bool flag3 = solution.points.Length >= num && solution.pathSizes.Length >= num2;
			if (flag3)
			{
				UnsafeUtility.MemCpy(solution.points.m_Buffer, intPtr.ToPointer(), (long)(num * sizeof(Vector2)));
				UnsafeUtility.MemCpy(solution.pathSizes.m_Buffer, intPtr2.ToPointer(), (long)(num2 * 4));
				ClipperOffset2D.Internal_Execute_Cleanup(intPtr, intPtr2);
				return;
			}
			ClipperOffset2D.Internal_Execute_Cleanup(intPtr, intPtr2);
			throw new IndexOutOfRangeException();
		}

		[NativeMethod(Name = "ClipperOffset2D::Execute", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Execute(out IntPtr outClippedPoints, out int outClippedPointsCount, out IntPtr outClippedPathSizes, out int outClippedPathCount, IntPtr inPoints, int inPointCount, IntPtr inPathSizes, IntPtr inPathArguments, int inPathCount, double inDelta, double inMiterLimit, double inRoundPrecision, double inArcTolerance, double inIntScale, bool useRounding);

		[NativeMethod(Name = "ClipperOffset2D::Execute_Cleanup", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Execute_Cleanup(IntPtr inPoints, IntPtr inPathSizes);

		public enum JoinType
		{
			jtSquare,
			jtRound,
			jtMiter
		}

		public enum EndType
		{
			etClosedPolygon,
			etClosedLine,
			etOpenButt,
			etOpenSquare,
			etOpenRound
		}

		[NativeType(Header = "Runtime/2D/Common/ClipperOffsetWrapper.h")]
		public struct PathArguments
		{
			public PathArguments(ClipperOffset2D.JoinType inJoinType = ClipperOffset2D.JoinType.jtSquare, ClipperOffset2D.EndType inEndType = ClipperOffset2D.EndType.etClosedPolygon)
			{
				this.joinType = inJoinType;
				this.endType = inEndType;
			}

			public ClipperOffset2D.JoinType joinType;

			public ClipperOffset2D.EndType endType;
		}

		public struct Solution
		{
			public Solution(int pointsBufferSize, int pathSizesBufferSize, Allocator allocator)
			{
				this.points = new NativeArray<Vector2>(pointsBufferSize, allocator, NativeArrayOptions.ClearMemory);
				this.pathSizes = new NativeArray<int>(pathSizesBufferSize, allocator, NativeArrayOptions.ClearMemory);
			}

			public void Dispose()
			{
				bool isCreated = this.points.IsCreated;
				if (isCreated)
				{
					this.points.Dispose();
				}
				bool isCreated2 = this.pathSizes.IsCreated;
				if (isCreated2)
				{
					this.pathSizes.Dispose();
				}
			}

			public NativeArray<Vector2> points;

			public NativeArray<int> pathSizes;
		}
	}
}
