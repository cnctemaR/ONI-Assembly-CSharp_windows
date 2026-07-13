using System;

public class NavTeleportTransitionLayer : TransitionDriver.OverrideLayer
{
	public NavTeleportTransitionLayer(Navigator navigator)
		: base(navigator)
	{
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		if (transition.start == NavType.Teleport)
		{
			int num = Grid.PosToCell(navigator);
			int num2;
			int num3;
			Grid.CellToXY(num, out num2, out num3);
			int num4 = num2;
			int num5 = num3;
			int num6;
			if (navigator.NavGrid.teleportTransitions.TryGetValue(num, out num6))
			{
				Grid.CellToXY(num6, out num4, out num5);
			}
			transition.x = num4 - num2;
			transition.y = num5 - num3;
		}
	}
}
