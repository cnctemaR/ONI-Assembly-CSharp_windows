using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

public static class FloodFill
{
	public static void BreadthTraverse<BoundaryCondition, VisitTracker, MyMaxDepth, Visitor>(int startCell, BoundaryCondition boundaryCondition, VisitTracker visited, MyMaxDepth maxDepth, Visitor visitor) where BoundaryCondition : FloodFill.IBoundaryCondition where VisitTracker : FloodFill.IVisitTracker where MyMaxDepth : FloodFill.IMaxDepth where Visitor : FloodFill.IVisitor
	{
		FloodFill.VerifyConstants();
		if (!maxDepth.Check(0))
		{
			return;
		}
		FloodFill.RetainedQueue value = FloodFill.tlOpen.Value;
		value.Clear();
		value.Enqueue((ulong)((long)startCell));
		while (value.Count > 0)
		{
			ulong num = value.Dequeue();
			int num2 = (int)(num & (ulong)(-1));
			if (Grid.IsValidCell(num2) && visited.Add(num2))
			{
				if (boundaryCondition.Check(num2) == FloodFill.BoundaryCheckResult.Halt)
				{
					visitor.VisitBoundary(num2);
				}
				else
				{
					visitor.VisitCell(num2);
					if (visitor.EarlyOut)
					{
						return;
					}
					uint num3 = (uint)((num & 2305843004918726656UL) >> 32) + 1U;
					DebugUtil.DevAssert(num3 <= 536870911U, "nextDepth overflowed allocated bitfield", null);
					DebugUtil.DevAssert(num3 <= 2147483647U, "nextDepth cannot be cast to int", null);
					if (maxDepth.Check((int)num3))
					{
						ulong num4 = (ulong)num3 << 32;
						value.Enqueue((ulong)((long)Grid.CellLeft(num2) | (long)num4));
						value.Enqueue((ulong)((long)Grid.CellRight(num2) | (long)num4));
						value.Enqueue((ulong)((long)Grid.CellAbove(num2) | (long)num4));
						value.Enqueue((ulong)((long)Grid.CellBelow(num2) | (long)num4));
					}
				}
			}
		}
	}

	public static void BreadthVisit(int startCell, Func<int, FloodFill.BoundaryCheckResult> boundaryCondition, HashSet<int> visitedCells)
	{
		FloodFill.BreadthTraverse<FloodFill.PredicateCondition, FloodFill.HashSetVisitTracker, FloodFill.NoMaxDepth, FloodFill.DoNothing>(startCell, new FloodFill.PredicateCondition(boundaryCondition), new FloodFill.HashSetVisitTracker
		{
			visited = visitedCells
		}, default(FloodFill.NoMaxDepth), default(FloodFill.DoNothing));
	}

	public static void BreadthVisit(int startCell, Func<int, FloodFill.BoundaryCheckResult> boundaryCondition, int maxDepth)
	{
		FloodFill.BreadthTraverse<FloodFill.PredicateCondition, FloodFill.GenerationGrid, FloodFill.MaxDepth, FloodFill.DoNothing>(startCell, new FloodFill.PredicateCondition(boundaryCondition), FloodFill.GenerationGrid.Default(), new FloodFill.MaxDepth(maxDepth), default(FloodFill.DoNothing));
	}

	public static void BreadthVisit(int startCell, Func<int, FloodFill.BoundaryCheckResult> boundaryCondition)
	{
		FloodFill.BreadthTraverse<FloodFill.PredicateCondition, FloodFill.GenerationGrid, FloodFill.NoMaxDepth, FloodFill.DoNothing>(startCell, new FloodFill.PredicateCondition(boundaryCondition), FloodFill.GenerationGrid.Default(), default(FloodFill.NoMaxDepth), default(FloodFill.DoNothing));
	}

	public static void BreadthCollect(int startCell, Func<int, FloodFill.BoundaryCheckResult> boundaryCondition, HashSet<int> visitedCells, List<int> validCells)
	{
		FloodFill.BreadthTraverse<FloodFill.PredicateCondition, FloodFill.HashSetVisitTracker, FloodFill.NoMaxDepth, FloodFill.Collector>(startCell, new FloodFill.PredicateCondition(boundaryCondition), new FloodFill.HashSetVisitTracker
		{
			visited = visitedCells
		}, default(FloodFill.NoMaxDepth), new FloodFill.Collector(validCells));
	}

	public static void BreadthCollect(int startCell, Func<int, FloodFill.BoundaryCheckResult> boundaryCondition, HashSet<int> visitedCells, List<int> validCells, int maxDepth)
	{
		FloodFill.BreadthTraverse<FloodFill.PredicateCondition, FloodFill.HashSetVisitTracker, FloodFill.MaxDepth, FloodFill.Collector>(startCell, new FloodFill.PredicateCondition(boundaryCondition), new FloodFill.HashSetVisitTracker
		{
			visited = visitedCells
		}, new FloodFill.MaxDepth(maxDepth), new FloodFill.Collector(validCells));
	}

	public static void BreadthCollect(int startCell, Func<int, FloodFill.BoundaryCheckResult> boundaryCondition, List<int> validCells, int maxDepth)
	{
		FloodFill.BreadthTraverse<FloodFill.PredicateCondition, FloodFill.GenerationGrid, FloodFill.MaxDepth, FloodFill.Collector>(startCell, new FloodFill.PredicateCondition(boundaryCondition), FloodFill.GenerationGrid.Default(), new FloodFill.MaxDepth(maxDepth), new FloodFill.Collector(validCells));
	}

	public static int Find<MaxDepth>(Func<int, bool> criteria, int startCell, MaxDepth maxDepth, bool stopAtSolid, bool stopAtLiquid) where MaxDepth : FloodFill.IMaxDepth
	{
		FloodFill.Finder finder = new FloodFill.Finder(criteria);
		FloodFill.ElementCheck elementCheck = new FloodFill.ElementCheck(stopAtSolid, stopAtLiquid);
		if (maxDepth.Check(10))
		{
			FloodFill.BreadthTraverse<FloodFill.ElementCheck, FloodFill.GenerationGrid, MaxDepth, FloodFill.Finder>(startCell, elementCheck, FloodFill.GenerationGrid.Default(), maxDepth, finder);
		}
		else
		{
			FloodFill.BreadthTraverse<FloodFill.ElementCheck, FloodFill.HashSetVisitTracker, MaxDepth, FloodFill.Finder>(startCell, elementCheck, FloodFill.HashSetVisitTracker.Default(), maxDepth, finder);
		}
		return finder.Cell;
	}

	public static bool Any<MaxDepth>(Func<int, bool> fn, int start_cell, MaxDepth max_depth, bool stop_at_solid, bool stop_at_liquid) where MaxDepth : FloodFill.IMaxDepth
	{
		return FloodFill.Find<MaxDepth>(fn, start_cell, max_depth, stop_at_solid, stop_at_liquid) != -1;
	}

	public static int FindBest(Func<int, float> rateCell, Func<int, FloodFill.BoundaryCheckResult> boundaryCondition, int startCell, int maxCellEvaluations = -1)
	{
		if (maxCellEvaluations == 0)
		{
			return Grid.InvalidCell;
		}
		FloodFill.Scorer scorer = new FloodFill.Scorer(rateCell, maxCellEvaluations);
		FloodFill.BreadthTraverse<FloodFill.PredicateCondition, FloodFill.GenerationGrid, FloodFill.NoMaxDepth, FloodFill.Scorer>(startCell, new FloodFill.PredicateCondition(boundaryCondition), FloodFill.GenerationGrid.Default(), default(FloodFill.NoMaxDepth), scorer);
		return scorer.BestCell;
	}

	public static void BreadthTraverseNoBacktrack<BoundaryCondition, VisitTracker, MyMaxDepth, Visitor>(int startCell, BoundaryCondition boundaryCondition, VisitTracker visited, MyMaxDepth maxDepth, Visitor visitor) where BoundaryCondition : FloodFill.IBoundaryCondition where VisitTracker : FloodFill.IVisitTracker where MyMaxDepth : FloodFill.IMaxDepth where Visitor : FloodFill.IVisitor
	{
		FloodFill.VerifyConstants();
		if (!maxDepth.Check(0))
		{
			return;
		}
		FloodFill.RetainedQueue value = FloodFill.tlOpen.Value;
		value.Clear();
		value.Enqueue((ulong)((long)startCell));
		while (value.Count > 0)
		{
			ulong num = value.Dequeue();
			int num2 = (int)(num & (ulong)(-1));
			if (Grid.IsValidCell(num2) && visited.Add(num2))
			{
				if (boundaryCondition.Check(num2) == FloodFill.BoundaryCheckResult.Halt)
				{
					visitor.VisitBoundary(num2);
				}
				else
				{
					visitor.VisitCell(num2);
					if (visitor.EarlyOut)
					{
						return;
					}
					uint num3 = (uint)((num & 2305843004918726656UL) >> 32) + 1U;
					DebugUtil.DevAssert(num3 <= 536870911U, "nextDepth overflowed allocated bitfield", null);
					DebugUtil.DevAssert(num3 <= 2147483647U, "nextDepth cannot be cast to int", null);
					if (maxDepth.Check((int)num3))
					{
						byte b = (byte)((num & 16140901064495857664UL) >> 61);
						ulong num4 = (ulong)num3 << 32;
						if (b != 1)
						{
							value.Enqueue((ulong)((long)Grid.CellLeft(num2) | (long)num4 | 4611686018427387904L));
						}
						if (b != 2)
						{
							value.Enqueue((ulong)((long)Grid.CellRight(num2) | (long)num4 | 2305843009213693952L));
						}
						if (b != 3)
						{
							value.Enqueue((ulong)((long)Grid.CellAbove(num2) | (long)num4 | long.MinValue));
						}
						if (b != 4)
						{
							value.Enqueue((ulong)((long)Grid.CellBelow(num2) | (long)num4 | 6917529027641081856L));
						}
					}
				}
			}
		}
	}

	public static void DepthTraverse<BoundaryCondition, VisitTracker, MyMaxDepth, Visitor>(int origin, BoundaryCondition boundaryCondition, VisitTracker visited, MyMaxDepth maxDepth, Visitor visitor) where BoundaryCondition : FloodFill.IBoundaryCondition where VisitTracker : FloodFill.IVisitTracker where MyMaxDepth : FloodFill.IMaxDepth where Visitor : FloodFill.IVisitor
	{
		FloodFill.VerifyConstants();
		if (!maxDepth.Check(0))
		{
			return;
		}
		if (!Grid.IsValidCell(origin))
		{
			return;
		}
		if (!visited.Add(origin))
		{
			return;
		}
		FloodFill.BoundaryCheckResult boundaryCheckResult = boundaryCondition.Check(origin);
		if (boundaryCheckResult != FloodFill.BoundaryCheckResult.Continue)
		{
			DebugUtil.DevAssert(boundaryCheckResult == FloodFill.BoundaryCheckResult.Halt, "unexpected CheckResult value", null);
			visitor.VisitBoundary(origin);
			return;
		}
		visitor.VisitCell(origin);
		if (visitor.EarlyOut)
		{
			return;
		}
		FloodFill.RetainedQueue value = FloodFill.tlAbove.Value;
		FloodFill.RetainedQueue value2 = FloodFill.tlBelow.Value;
		FloodFill.RetainedQueue value3 = FloodFill.tlLeft.Value;
		FloodFill.RetainedQueue value4 = FloodFill.tlRight.Value;
		value.Clear();
		value2.Clear();
		value3.Clear();
		value4.Clear();
		FloodFill.<>c__DisplayClass27_0<BoundaryCondition, VisitTracker, MyMaxDepth, Visitor> CS$<>8__locals1;
		CS$<>8__locals1.originCell = (ulong)((long)origin | 0L);
		FloodFill.<DepthTraverse>g__SeedDirection|27_0<BoundaryCondition, VisitTracker, MyMaxDepth, Visitor>(value, ref CS$<>8__locals1);
		FloodFill.<DepthTraverse>g__SeedDirection|27_0<BoundaryCondition, VisitTracker, MyMaxDepth, Visitor>(value2, ref CS$<>8__locals1);
		FloodFill.<DepthTraverse>g__SeedDirection|27_0<BoundaryCondition, VisitTracker, MyMaxDepth, Visitor>(value3, ref CS$<>8__locals1);
		FloodFill.<DepthTraverse>g__SeedDirection|27_0<BoundaryCondition, VisitTracker, MyMaxDepth, Visitor>(value4, ref CS$<>8__locals1);
		FloodFill.NorthRay northRay = FloodFill.NorthRay.Default();
		FloodFill.SouthRay southRay = FloodFill.SouthRay.Default();
		FloodFill.WestRay westRay = FloodFill.WestRay.Default();
		FloodFill.EastRay eastRay = FloodFill.EastRay.Default();
		IL_01AD:
		while (value3.Count + value4.Count + value.Count + value2.Count > 0)
		{
			bool flag = false;
			if (northRay.ForwardQueue.Count > 0)
			{
				flag = !FloodFill.CastRay<FloodFill.NorthRay, BoundaryCondition, VisitTracker, MyMaxDepth, Visitor>(northRay, boundaryCondition, visited, maxDepth, visitor);
			}
			else if (southRay.ForwardQueue.Count > 0)
			{
				flag = !FloodFill.CastRay<FloodFill.SouthRay, BoundaryCondition, VisitTracker, MyMaxDepth, Visitor>(southRay, boundaryCondition, visited, maxDepth, visitor);
			}
			if (flag)
			{
				return;
			}
			while (westRay.ForwardQueue.Count > 0)
			{
				flag = !FloodFill.CastRay<FloodFill.WestRay, BoundaryCondition, VisitTracker, MyMaxDepth, Visitor>(westRay, boundaryCondition, visited, maxDepth, visitor);
				if (flag)
				{
					IL_019A:
					while (eastRay.ForwardQueue.Count > 0)
					{
						flag = !FloodFill.CastRay<FloodFill.EastRay, BoundaryCondition, VisitTracker, MyMaxDepth, Visitor>(eastRay, boundaryCondition, visited, maxDepth, visitor);
						if (flag)
						{
							break;
						}
					}
					if (!flag)
					{
						goto IL_01AD;
					}
					return;
				}
			}
			goto IL_019A;
		}
	}

	public static void DepthVisit(int startCell, Func<int, FloodFill.BoundaryCheckResult> boundaryCondition, HashSet<int> visitedCells)
	{
		FloodFill.DepthTraverse<FloodFill.PredicateCondition, FloodFill.HashSetVisitTracker, FloodFill.NoMaxDepth, FloodFill.DoNothing>(startCell, new FloodFill.PredicateCondition(boundaryCondition), new FloodFill.HashSetVisitTracker
		{
			visited = visitedCells
		}, default(FloodFill.NoMaxDepth), default(FloodFill.DoNothing));
	}

	public static void DepthVisit(int startCell, Func<int, FloodFill.BoundaryCheckResult> boundaryCondition)
	{
		FloodFill.DepthTraverse<FloodFill.PredicateCondition, FloodFill.GenerationGrid, FloodFill.NoMaxDepth, FloodFill.DoNothing>(startCell, new FloodFill.PredicateCondition(boundaryCondition), FloodFill.GenerationGrid.Default(), default(FloodFill.NoMaxDepth), default(FloodFill.DoNothing));
	}

	public static void DepthCollect(int startCell, Func<int, FloodFill.BoundaryCheckResult> boundaryCondition, HashSet<int> visitedCells, List<int> validCells, int maxDepth)
	{
		FloodFill.DepthTraverse<FloodFill.PredicateCondition, FloodFill.HashSetVisitTracker, FloodFill.MaxDepth, FloodFill.Collector>(startCell, new FloodFill.PredicateCondition(boundaryCondition), new FloodFill.HashSetVisitTracker
		{
			visited = visitedCells
		}, new FloodFill.MaxDepth(maxDepth), new FloodFill.Collector(validCells));
	}

	public static void DepthCollect(int startCell, Func<int, FloodFill.BoundaryCheckResult> boundaryCondition, HashSet<int> visitedCells, List<int> validCells)
	{
		FloodFill.DepthTraverse<FloodFill.PredicateCondition, FloodFill.HashSetVisitTracker, FloodFill.NoMaxDepth, FloodFill.Collector>(startCell, new FloodFill.PredicateCondition(boundaryCondition), new FloodFill.HashSetVisitTracker
		{
			visited = visitedCells
		}, default(FloodFill.NoMaxDepth), new FloodFill.Collector(validCells));
	}

	public static void DepthCollect(int startCell, Func<int, FloodFill.BoundaryCheckResult> boundaryCondition, List<int> validCells, int maxDepth)
	{
		FloodFill.DepthTraverse<FloodFill.PredicateCondition, FloodFill.GenerationGrid, FloodFill.MaxDepth, FloodFill.Collector>(startCell, new FloodFill.PredicateCondition(boundaryCondition), FloodFill.GenerationGrid.Default(), new FloodFill.MaxDepth(maxDepth), new FloodFill.Collector(validCells));
	}

	public static void DepthCollect(int startCell, Func<int, FloodFill.BoundaryCheckResult> boundaryCondition, List<int> validCells)
	{
		FloodFill.DepthTraverse<FloodFill.PredicateCondition, FloodFill.GenerationGrid, FloodFill.NoMaxDepth, FloodFill.Collector>(startCell, new FloodFill.PredicateCondition(boundaryCondition), FloodFill.GenerationGrid.Default(), default(FloodFill.NoMaxDepth), new FloodFill.Collector(validCells));
	}

	private static void VerifyConstants()
	{
		DebugUtil.DevAssert(Grid.CellCount <= int.MaxValue, "Too few bits allocated to INDEX to handle all grid cells.", null);
		DebugUtil.DevAssert(true, "Unsigned DEPTH_MAX must not be larger than the maximum Int32 as that is the user-facing type for depth", null);
	}

	private static bool CastLateralRay<Ray, LateralRay, BoundaryCondition, VisitTracker, Visitor>(int originIndex, uint originDepth, int cellCount, Ray ray, LateralRay lateralRay, BoundaryCondition boundaryCondition, VisitTracker visited, Visitor visitor) where Ray : FloodFill.IRay where LateralRay : FloodFill.ILateralRay where BoundaryCondition : FloodFill.IBoundaryCondition where VisitTracker : FloodFill.IVisitTracker where Visitor : FloodFill.IVisitor
	{
		int num = lateralRay.FromSourceCell(originIndex);
		if (!Grid.IsValidCell(num))
		{
			return true;
		}
		bool flag = false;
		for (int num2 = 0; num2 != cellCount; num2++)
		{
			num = ray.Forward(num);
			if (visited.Add(num))
			{
				flag = boundaryCondition.Check(num) == FloodFill.BoundaryCheckResult.Continue;
				if (flag)
				{
					visitor.VisitCell(num);
					if (visitor.EarlyOut)
					{
						return false;
					}
					ulong num3 = (ulong)originDepth + (ulong)((long)num2) + 2UL;
					DebugUtil.DevAssert(num3 <= 536870911UL, "cellDepth overflowed allocated bitfield", null);
					lateralRay.Enqueue((ulong)((long)num | (long)((long)num3 << 32)));
				}
				else
				{
					visitor.VisitBoundary(num);
				}
			}
		}
		if (flag)
		{
			ulong num4 = (ulong)originDepth + (ulong)((long)cellCount) + 1UL;
			DebugUtil.DevAssert(num4 <= 536870911UL, "cellDepth overflowed allocated bitfield", null);
			ray.ForwardQueue.Enqueue((ulong)((long)num | (long)((long)num4 << 32)));
		}
		return true;
	}

	private static bool CastRay<Ray, BoundaryCondition, VisitTracker, MyMaxDepth, Visitor>(Ray ray, BoundaryCondition boundaryCondition, VisitTracker visited, MyMaxDepth maxDepth, Visitor visitor) where Ray : FloodFill.IRay where BoundaryCondition : FloodFill.IBoundaryCondition where VisitTracker : FloodFill.IVisitTracker where MyMaxDepth : FloodFill.IMaxDepth where Visitor : FloodFill.IVisitor
	{
		ulong num = ray.ForwardQueue.Dequeue();
		int num2 = (int)(num & (ulong)(-1));
		uint num3 = (uint)((num & 2305843004918726656UL) >> 32);
		uint num4 = num3;
		int num5 = num2;
		while (maxDepth.Check((int)num4))
		{
			num5 = ray.Forward(num5);
			if (!Grid.IsValidCell(num5) || !visited.Add(num5))
			{
				break;
			}
			if (boundaryCondition.Check(num5) == FloodFill.BoundaryCheckResult.Halt)
			{
				visitor.VisitBoundary(num5);
				break;
			}
			visitor.VisitCell(num5);
			if (visitor.EarlyOut)
			{
				return false;
			}
			num4 += 1U;
		}
		DebugUtil.DevAssert(num4 <= 2147483647U, "depth cannot be cast to int", null);
		int num6 = (int)(num4 - num3);
		return num6 == 0 || (FloodFill.CastLateralRay<Ray, FloodFill.PortRay<Ray>, BoundaryCondition, VisitTracker, Visitor>(num2, num3, num6, ray, new FloodFill.PortRay<Ray>(ray), boundaryCondition, visited, visitor) && FloodFill.CastLateralRay<Ray, FloodFill.StarboardRay<Ray>, BoundaryCondition, VisitTracker, Visitor>(num2, num3, num6, ray, new FloodFill.StarboardRay<Ray>(ray), boundaryCondition, visited, visitor));
	}

	[CompilerGenerated]
	internal static void <DepthTraverse>g__SeedDirection|27_0<BoundaryCondition, VisitTracker, MyMaxDepth, Visitor>(FloodFill.RetainedQueue rays, ref FloodFill.<>c__DisplayClass27_0<BoundaryCondition, VisitTracker, MyMaxDepth, Visitor> A_1) where BoundaryCondition : FloodFill.IBoundaryCondition where VisitTracker : FloodFill.IVisitTracker where MyMaxDepth : FloodFill.IMaxDepth where Visitor : FloodFill.IVisitor
	{
		rays.Enqueue(A_1.originCell);
	}

	private static readonly ThreadLocal<FloodFill.RetainedQueue> tlOpen = new ThreadLocal<FloodFill.RetainedQueue>(() => new FloodFill.RetainedQueue(1024));

	private static readonly ThreadLocal<HashSet<int>> tlVisited = new ThreadLocal<HashSet<int>>(() => new HashSet<int>());

	private static readonly ThreadLocal<byte[]> tlGrid = new ThreadLocal<byte[]>(() => new byte[Grid.CellCount]);

	private static readonly ThreadLocal<byte> tlGeneration = new ThreadLocal<byte>(() => 0);

	private const ulong LEFT_BITS = 2305843009213693952UL;

	private const ulong RIGHT_BITS = 4611686018427387904UL;

	private const ulong UP_BITS = 6917529027641081856UL;

	private const ulong DOWN_BITS = 9223372036854775808UL;

	private const ulong INDEX_MASK = 4294967295UL;

	private const int INDEX_SHIFT = 0;

	private const int INDEX_MAX = 2147483647;

	private const ulong DEPTH_MASK = 2305843004918726656UL;

	private const int DEPTH_SHIFT = 32;

	private const uint DEPTH_MAX = 536870911U;

	private const ulong QUEUED_FROM_MASK = 16140901064495857664UL;

	private const int QUEUED_FROM_SHIFT = 61;

	private static readonly ThreadLocal<FloodFill.RetainedQueue> tlAbove = new ThreadLocal<FloodFill.RetainedQueue>(() => new FloodFill.RetainedQueue(1024));

	private static readonly ThreadLocal<FloodFill.RetainedQueue> tlBelow = new ThreadLocal<FloodFill.RetainedQueue>(() => new FloodFill.RetainedQueue(1024));

	private static readonly ThreadLocal<FloodFill.RetainedQueue> tlLeft = new ThreadLocal<FloodFill.RetainedQueue>(() => new FloodFill.RetainedQueue(1024));

	private static readonly ThreadLocal<FloodFill.RetainedQueue> tlRight = new ThreadLocal<FloodFill.RetainedQueue>(() => new FloodFill.RetainedQueue(1024));

	public interface IVisitTracker
	{
		bool Contains(int cell);

		bool Add(int cell);
	}

	public struct HashSetVisitTracker : FloodFill.IVisitTracker
	{
		public readonly bool Contains(int cell)
		{
			return this.visited.Contains(cell);
		}

		public readonly bool Add(int cell)
		{
			return this.visited.Add(cell);
		}

		public static FloodFill.HashSetVisitTracker Default()
		{
			HashSet<int> value = FloodFill.tlVisited.Value;
			value.Clear();
			return new FloodFill.HashSetVisitTracker
			{
				visited = value
			};
		}

		public HashSet<int> visited;
	}

	public readonly struct GenerationGrid : FloodFill.IVisitTracker
	{
		public GenerationGrid(byte generation, byte[] grid)
		{
			this.generation = generation;
			this.grid = grid;
		}

		public bool Contains(int cell)
		{
			return this.grid[cell] == this.generation;
		}

		public bool Add(int cell)
		{
			if (this.Contains(cell))
			{
				return false;
			}
			this.grid[cell] = this.generation;
			return true;
		}

		public static FloodFill.GenerationGrid Default()
		{
			byte[] array = FloodFill.tlGrid.Value;
			if (array.Length != Grid.CellCount)
			{
				Debug.Log("Resize FloodFill.GenerationGrid");
				FloodFill.tlGrid.Value = new byte[Grid.CellCount];
				array = FloodFill.tlGrid.Value;
				FloodFill.tlGeneration.Value = 0;
			}
			byte b = FloodFill.tlGeneration.Value;
			b += 1;
			if (b == 0)
			{
				Debug.Log("Reset FloodFill.GenerationGrid");
				Array.Clear(array, 0, array.Length);
				FloodFill.tlGeneration.Value = 1;
				b = 1;
			}
			else
			{
				FloodFill.tlGeneration.Value = b;
			}
			return new FloodFill.GenerationGrid(b, array);
		}

		private readonly byte generation;

		private readonly byte[] grid;
	}

	public enum BoundaryCheckResult
	{
		Continue,
		Halt
	}

	public interface IBoundaryCondition
	{
		FloodFill.BoundaryCheckResult Check(int cell);
	}

	public readonly struct PredicateCondition : FloodFill.IBoundaryCondition
	{
		public PredicateCondition(Func<int, FloodFill.BoundaryCheckResult> predicate)
		{
			this.predicate = predicate;
		}

		public FloodFill.BoundaryCheckResult Check(int cell)
		{
			return this.predicate(cell);
		}

		private readonly Func<int, FloodFill.BoundaryCheckResult> predicate;
	}

	public readonly struct ElementCheck : FloodFill.IBoundaryCondition
	{
		public ElementCheck(bool stop_at_solid, bool stop_at_liquid)
		{
			DebugUtil.DevAssert(stop_at_solid || stop_at_liquid, "No sense in running this if it never does anything", null);
			this.stop_at_solid = stop_at_solid;
			this.stop_at_liquid = stop_at_liquid;
		}

		public FloodFill.BoundaryCheckResult Check(int cell)
		{
			Element element = Grid.Element[cell];
			if (this.stop_at_solid && element.IsSolid)
			{
				return FloodFill.BoundaryCheckResult.Halt;
			}
			if (this.stop_at_liquid && element.IsLiquid)
			{
				return FloodFill.BoundaryCheckResult.Halt;
			}
			return FloodFill.BoundaryCheckResult.Continue;
		}

		private readonly bool stop_at_solid;

		private readonly bool stop_at_liquid;
	}

	public readonly struct NoBoundary : FloodFill.IBoundaryCondition
	{
		public FloodFill.BoundaryCheckResult Check(int cell)
		{
			return FloodFill.BoundaryCheckResult.Continue;
		}
	}

	public interface IMaxDepth
	{
		bool Check(int cellDepth);
	}

	public struct NoMaxDepth : FloodFill.IMaxDepth
	{
		public readonly bool Check(int cellDepth)
		{
			return true;
		}
	}

	public readonly struct MaxDepth : FloodFill.IMaxDepth
	{
		public MaxDepth(int maxDepth)
		{
			this.value = maxDepth;
		}

		public bool Check(int cellDepth)
		{
			return cellDepth < this.value;
		}

		private readonly int value;
	}

	public interface IVisitor
	{
		void VisitCell(int cell);

		void VisitBoundary(int cell);

		bool EarlyOut { get; }
	}

	public struct DoNothing : FloodFill.IVisitor
	{
		public readonly void VisitCell(int cell)
		{
		}

		public readonly void VisitBoundary(int cell)
		{
		}

		public readonly bool EarlyOut
		{
			get
			{
				return false;
			}
		}
	}

	public readonly struct Collector : FloodFill.IVisitor
	{
		public void VisitCell(int cell)
		{
			this.cells.Add(cell);
		}

		public void VisitBoundary(int cell)
		{
		}

		public bool EarlyOut
		{
			get
			{
				return false;
			}
		}

		public Collector(List<int> cells)
		{
			this.cells = cells;
		}

		private readonly List<int> cells;
	}

	public class Finder : FloodFill.IVisitor
	{
		public int Cell
		{
			get
			{
				return this.foundCell;
			}
		}

		public bool EarlyOut
		{
			get
			{
				return this.foundCell != Grid.InvalidCell;
			}
		}

		public void VisitCell(int cell)
		{
			if (this.criteria(cell))
			{
				this.foundCell = cell;
			}
		}

		public void VisitBoundary(int cell)
		{
		}

		public Finder(Func<int, bool> criteria)
		{
			this.criteria = criteria;
		}

		private readonly Func<int, bool> criteria;

		private int foundCell = Grid.InvalidCell;
	}

	public class Scorer : FloodFill.IVisitor
	{
		public int BestCell
		{
			get
			{
				return this.bestCell;
			}
		}

		public bool EarlyOut
		{
			get
			{
				return this.threshold == 0;
			}
		}

		public Scorer(Func<int, float> rateCell, int threshold)
		{
			this.rateCell = rateCell;
			this.threshold = threshold;
			this.bestScore = float.NegativeInfinity;
			this.bestCell = Grid.InvalidCell;
		}

		public void VisitCell(int cell)
		{
			float num = this.rateCell(cell);
			if (num > this.bestScore)
			{
				this.bestScore = num;
				this.bestCell = cell;
			}
			if (this.threshold > 0)
			{
				this.threshold--;
			}
		}

		public void VisitBoundary(int cell)
		{
		}

		private readonly Func<int, float> rateCell;

		private int threshold;

		private float bestScore;

		private int bestCell;
	}

	private class RetainedQueue
	{
		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public RetainedQueue(int initialCapacity = 1024)
		{
			this.buffer = new ulong[initialCapacity];
		}

		public void Enqueue(ulong item)
		{
			if (this.count == this.buffer.Length)
			{
				this.Grow();
			}
			this.buffer[this.tail] = item;
			this.tail = (this.tail + 1) % this.buffer.Length;
			this.count++;
		}

		public ulong Dequeue()
		{
			ulong num = this.buffer[this.head];
			this.head = (this.head + 1) % this.buffer.Length;
			this.count--;
			return num;
		}

		public void Clear()
		{
			this.head = 0;
			this.tail = 0;
			this.count = 0;
		}

		public void EnsureCapacity(int capacity)
		{
			if (this.buffer.Length < capacity)
			{
				this.Resize(capacity);
			}
		}

		private void Grow()
		{
			int num = this.buffer.Length * 2;
			this.Resize(num);
		}

		private void Resize(int newCapacity)
		{
			ulong[] array = new ulong[newCapacity];
			if (this.head < this.tail)
			{
				Array.Copy(this.buffer, this.head, array, 0, this.count);
			}
			else
			{
				Array.Copy(this.buffer, this.head, array, 0, this.buffer.Length - this.head);
				Array.Copy(this.buffer, 0, array, this.buffer.Length - this.head, this.tail);
			}
			this.buffer = array;
			this.head = 0;
			this.tail = this.count;
		}

		private ulong[] buffer;

		private int head;

		private int tail;

		private int count;
	}

	private enum QueuedFrom : byte
	{
		None,
		Left,
		Right,
		Up,
		Down
	}

	private interface IRay
	{
		int Forward(int cell);

		int Port(int cell);

		int Starboard(int cell);

		FloodFill.RetainedQueue ForwardQueue { get; }

		FloodFill.RetainedQueue PortQueue { get; }

		FloodFill.RetainedQueue StarboardQueue { get; }
	}

	private struct NorthRay : FloodFill.IRay
	{
		public readonly int Forward(int cell)
		{
			return Grid.CellAbove(cell);
		}

		public readonly int Port(int cell)
		{
			return Grid.CellLeft(cell);
		}

		public readonly int Starboard(int cell)
		{
			return Grid.CellRight(cell);
		}

		public readonly FloodFill.RetainedQueue ForwardQueue
		{
			get
			{
				return this.above;
			}
		}

		public readonly FloodFill.RetainedQueue PortQueue
		{
			get
			{
				return this.left;
			}
		}

		public readonly FloodFill.RetainedQueue StarboardQueue
		{
			get
			{
				return this.right;
			}
		}

		public static FloodFill.NorthRay Default()
		{
			return new FloodFill.NorthRay
			{
				above = FloodFill.tlAbove.Value,
				left = FloodFill.tlLeft.Value,
				right = FloodFill.tlRight.Value
			};
		}

		private FloodFill.RetainedQueue above;

		private FloodFill.RetainedQueue left;

		private FloodFill.RetainedQueue right;
	}

	private struct SouthRay : FloodFill.IRay
	{
		public readonly int Forward(int cell)
		{
			return Grid.CellBelow(cell);
		}

		public readonly int Port(int cell)
		{
			return Grid.CellRight(cell);
		}

		public readonly int Starboard(int cell)
		{
			return Grid.CellLeft(cell);
		}

		public readonly FloodFill.RetainedQueue ForwardQueue
		{
			get
			{
				return this.below;
			}
		}

		public readonly FloodFill.RetainedQueue PortQueue
		{
			get
			{
				return this.right;
			}
		}

		public readonly FloodFill.RetainedQueue StarboardQueue
		{
			get
			{
				return this.left;
			}
		}

		public static FloodFill.SouthRay Default()
		{
			return new FloodFill.SouthRay
			{
				below = FloodFill.tlBelow.Value,
				right = FloodFill.tlRight.Value,
				left = FloodFill.tlLeft.Value
			};
		}

		private FloodFill.RetainedQueue below;

		private FloodFill.RetainedQueue right;

		private FloodFill.RetainedQueue left;
	}

	private struct EastRay : FloodFill.IRay
	{
		public readonly int Forward(int cell)
		{
			return Grid.CellRight(cell);
		}

		public readonly int Port(int cell)
		{
			return Grid.CellAbove(cell);
		}

		public readonly int Starboard(int cell)
		{
			return Grid.CellBelow(cell);
		}

		public readonly FloodFill.RetainedQueue ForwardQueue
		{
			get
			{
				return this.right;
			}
		}

		public readonly FloodFill.RetainedQueue PortQueue
		{
			get
			{
				return this.above;
			}
		}

		public readonly FloodFill.RetainedQueue StarboardQueue
		{
			get
			{
				return this.below;
			}
		}

		public static FloodFill.EastRay Default()
		{
			return new FloodFill.EastRay
			{
				right = FloodFill.tlRight.Value,
				above = FloodFill.tlAbove.Value,
				below = FloodFill.tlBelow.Value
			};
		}

		private FloodFill.RetainedQueue right;

		private FloodFill.RetainedQueue above;

		private FloodFill.RetainedQueue below;
	}

	private struct WestRay : FloodFill.IRay
	{
		public readonly int Forward(int cell)
		{
			return Grid.CellLeft(cell);
		}

		public readonly int Port(int cell)
		{
			return Grid.CellBelow(cell);
		}

		public readonly int Starboard(int cell)
		{
			return Grid.CellAbove(cell);
		}

		public readonly FloodFill.RetainedQueue ForwardQueue
		{
			get
			{
				return this.left;
			}
		}

		public readonly FloodFill.RetainedQueue PortQueue
		{
			get
			{
				return this.below;
			}
		}

		public readonly FloodFill.RetainedQueue StarboardQueue
		{
			get
			{
				return this.above;
			}
		}

		public static FloodFill.WestRay Default()
		{
			return new FloodFill.WestRay
			{
				left = FloodFill.tlLeft.Value,
				below = FloodFill.tlBelow.Value,
				above = FloodFill.tlAbove.Value
			};
		}

		private FloodFill.RetainedQueue left;

		private FloodFill.RetainedQueue below;

		private FloodFill.RetainedQueue above;
	}

	private interface ILateralRay
	{
		int FromSourceCell(int sourceCellIndex);

		void Enqueue(ulong cell);
	}

	private readonly struct PortRay<Ray> : FloodFill.ILateralRay where Ray : FloodFill.IRay
	{
		public int FromSourceCell(int sourceCellIndex)
		{
			Ray ray = this.ray;
			return ray.Port(sourceCellIndex);
		}

		public void Enqueue(ulong cell)
		{
			Ray ray = this.ray;
			ray.PortQueue.Enqueue(cell);
		}

		public PortRay(Ray ray)
		{
			this.ray = ray;
		}

		private readonly Ray ray;
	}

	private readonly struct StarboardRay<Ray> : FloodFill.ILateralRay where Ray : FloodFill.IRay
	{
		public int FromSourceCell(int sourceCellIndex)
		{
			Ray ray = this.ray;
			return ray.Starboard(sourceCellIndex);
		}

		public void Enqueue(ulong cell)
		{
			Ray ray = this.ray;
			ray.StarboardQueue.Enqueue(cell);
		}

		public StarboardRay(Ray ray)
		{
			this.ray = ray;
		}

		private readonly Ray ray;
	}
}
