using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.U2D
{
	[NativeType(Header = "Runtime/2D/Common/ClipperWrapper.h")]
	internal struct Clipper2D
	{
		public unsafe static void Execute(ref Clipper2D.Solution solution, NativeArray<Vector2> inPoints, NativeArray<int> inPathSizes, NativeArray<Clipper2D.PathArguments> inPathArguments, Clipper2D.ExecuteArguments inExecuteArguments, Allocator inSolutionAllocator, int inIntScale = 65536, bool useRounding = false)
		{
			bool flag = !solution.boundingRect.IsCreated;
			if (flag)
			{
				solution.boundingRect = new NativeArray<Rect>(1, inSolutionAllocator, NativeArrayOptions.ClearMemory);
			}
			IntPtr intPtr;
			int num;
			IntPtr intPtr2;
			int num2;
			solution.boundingRect[0] = Clipper2D.Internal_Execute(out intPtr, out num, out intPtr2, out num2, new IntPtr(inPoints.m_Buffer), inPoints.Length, new IntPtr(inPathSizes.m_Buffer), new IntPtr(inPathArguments.m_Buffer), inPathSizes.Length, inExecuteArguments, (float)inIntScale, useRounding);
			bool flag2 = num > 0;
			if (flag2)
			{
				bool flag3 = !solution.pathSizes.IsCreated;
				if (flag3)
				{
					solution.pathSizes = new NativeArray<int>(num2, inSolutionAllocator, NativeArrayOptions.ClearMemory);
				}
				bool flag4 = !solution.points.IsCreated;
				if (flag4)
				{
					solution.points = new NativeArray<Vector2>(num, inSolutionAllocator, NativeArrayOptions.ClearMemory);
				}
				bool flag5 = solution.points.Length >= num && solution.pathSizes.Length >= num2;
				if (!flag5)
				{
					Clipper2D.Internal_Execute_Cleanup(intPtr, intPtr2);
					throw new IndexOutOfRangeException();
				}
				UnsafeUtility.MemCpy(solution.points.m_Buffer, intPtr.ToPointer(), (long)(num * sizeof(Vector2)));
				UnsafeUtility.MemCpy(solution.pathSizes.m_Buffer, intPtr2.ToPointer(), (long)(num2 * 4));
				Clipper2D.Internal_Execute_Cleanup(intPtr, intPtr2);
			}
			else
			{
				bool flag6 = !solution.pathSizes.IsCreated;
				if (flag6)
				{
					solution.points = new NativeArray<Vector2>(0, inSolutionAllocator, NativeArrayOptions.ClearMemory);
				}
				bool flag7 = !solution.points.IsCreated;
				if (flag7)
				{
					solution.pathSizes = new NativeArray<int>(0, inSolutionAllocator, NativeArrayOptions.ClearMemory);
				}
			}
		}

		[NativeMethod(Name = "Clipper2D::Execute", IsFreeFunction = true, IsThreadSafe = true)]
		private static Rect Internal_Execute(out IntPtr outClippedPoints, out int outClippedPointsCount, out IntPtr outClippedPathSizes, out int outClippedPathCount, IntPtr inPoints, int inPointCount, IntPtr inPathSizes, IntPtr inPathArguments, int inPathCount, Clipper2D.ExecuteArguments inExecuteArguments, float inIntScale, bool useRounding)
		{
			Rect rect;
			Clipper2D.Internal_Execute_Injected(out outClippedPoints, out outClippedPointsCount, out outClippedPathSizes, out outClippedPathCount, inPoints, inPointCount, inPathSizes, inPathArguments, inPathCount, ref inExecuteArguments, inIntScale, useRounding, out rect);
			return rect;
		}

		[NativeMethod(Name = "Clipper2D::Execute_Cleanup", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Execute_Cleanup(IntPtr inPoints, IntPtr inPathSizes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Execute_Injected(out IntPtr outClippedPoints, out int outClippedPointsCount, out IntPtr outClippedPathSizes, out int outClippedPathCount, IntPtr inPoints, int inPointCount, IntPtr inPathSizes, IntPtr inPathArguments, int inPathCount, [In] ref Clipper2D.ExecuteArguments inExecuteArguments, float inIntScale, bool useRounding, out Rect ret);

		public enum ClipType
		{
			ctIntersection,
			ctUnion,
			ctDifference,
			ctXor
		}

		public enum PolyType
		{
			ptSubject,
			ptClip
		}

		public enum PolyFillType
		{
			pftEvenOdd,
			pftNonZero,
			pftPositive,
			pftNegative
		}

		public enum InitOptions
		{
			ioDefault,
			oReverseSolution,
			ioStrictlySimple,
			ioPreserveCollinear = 4
		}

		[NativeType(Header = "Runtime/2D/Common/ClipperWrapper.h")]
		public struct PathArguments
		{
			public PathArguments(Clipper2D.PolyType inPolyType = Clipper2D.PolyType.ptSubject, bool inClosed = false)
			{
				this.polyType = inPolyType;
				this.closed = inClosed;
			}

			public Clipper2D.PolyType polyType;

			public bool closed;
		}

		[NativeType(Header = "Runtime/2D/Common/ClipperWrapper.h")]
		public struct ExecuteArguments
		{
			public ExecuteArguments(Clipper2D.InitOptions inInitOption = Clipper2D.InitOptions.ioDefault, Clipper2D.ClipType inClipType = Clipper2D.ClipType.ctIntersection, Clipper2D.PolyFillType inSubjFillType = Clipper2D.PolyFillType.pftEvenOdd, Clipper2D.PolyFillType inClipFillType = Clipper2D.PolyFillType.pftEvenOdd, bool inReverseSolution = false, bool inStrictlySimple = false, bool inPreserveColinear = false)
			{
				this.initOption = inInitOption;
				this.clipType = inClipType;
				this.subjFillType = inSubjFillType;
				this.clipFillType = inClipFillType;
				this.reverseSolution = inReverseSolution;
				this.strictlySimple = inStrictlySimple;
				this.preserveColinear = inPreserveColinear;
			}

			public Clipper2D.InitOptions initOption;

			public Clipper2D.ClipType clipType;

			public Clipper2D.PolyFillType subjFillType;

			public Clipper2D.PolyFillType clipFillType;

			public bool reverseSolution;

			public bool strictlySimple;

			public bool preserveColinear;
		}

		public struct Solution : IDisposable
		{
			public Solution(int pointsBufferSize, int pathSizesBufferSize, Allocator allocator)
			{
				this.points = new NativeArray<Vector2>(pointsBufferSize, allocator, NativeArrayOptions.ClearMemory);
				this.pathSizes = new NativeArray<int>(pathSizesBufferSize, allocator, NativeArrayOptions.ClearMemory);
				this.boundingRect = new NativeArray<Rect>(1, allocator, NativeArrayOptions.ClearMemory);
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
				bool isCreated3 = this.boundingRect.IsCreated;
				if (isCreated3)
				{
					this.boundingRect.Dispose();
				}
			}

			public NativeArray<Vector2> points;

			public NativeArray<int> pathSizes;

			public NativeArray<Rect> boundingRect;
		}
	}
}
