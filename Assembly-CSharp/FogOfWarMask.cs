using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FogOfWarMask")]
public class FogOfWarMask : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		global::Debug.Assert(false, "Unmaintained, presumed dead, code is being invoked!");
	}

	protected override void OnCmpEnable()
	{
		global::Debug.Assert(false, "Unmaintained, presumed dead, code is being invoked!");
	}

	public static void ClearMask(int cell)
	{
		FloodFill.BreadthTraverse<FloodFill.PredicateCondition, FloodFill.HashSetVisitTracker, FloodFill.NoMaxDepth, FogOfWarMask.ThresholdVisitor>(cell, new FloodFill.PredicateCondition(FogOfWarMask.revealFogOfWarMask), FloodFill.HashSetVisitTracker.Default(), default(FloodFill.NoMaxDepth), new FogOfWarMask.ThresholdVisitor(300));
	}

	public static FloodFill.BoundaryCheckResult RevealFogOfWarMask(int cell)
	{
		if (Grid.PreventFogOfWarReveal[cell])
		{
			Grid.PreventFogOfWarReveal[cell] = false;
			Grid.Reveal(cell, byte.MaxValue, false);
			return FloodFill.BoundaryCheckResult.Continue;
		}
		return FloodFill.BoundaryCheckResult.Halt;
	}

	private static readonly Func<int, FloodFill.BoundaryCheckResult> revealFogOfWarMask = new Func<int, FloodFill.BoundaryCheckResult>(FogOfWarMask.RevealFogOfWarMask);

	private struct ThresholdVisitor : FloodFill.IVisitor
	{
		public ThresholdVisitor(int threshold)
		{
			this.threshold = threshold;
		}

		public readonly bool EarlyOut
		{
			get
			{
				return this.threshold <= 0;
			}
		}

		public void VisitCell(int cell)
		{
			this.threshold--;
		}

		public readonly void VisitBoundary(int cell)
		{
		}

		private int threshold;
	}
}
