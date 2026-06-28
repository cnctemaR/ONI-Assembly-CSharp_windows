using System;

public static class NavigationTactics
{
	public static NavTactic ReduceTravelDistance = new NavTactic(0, 0, 1, 4);

	public static NavTactic StandOnTop = new NavTactic(0, 2, 0, 0);

	public static NavTactic Range_1_AvoidOverlaps = new NavTactic(1, 6, 12, 1);

	public static NavTactic Range_2_AvoidOverlaps = new NavTactic(2, 6, 12, 1);

	public static NavTactic Range_3_AvoidOverlaps = new NavTactic(3, 6, 12, 1);

	public static NavTactic Range_3_ProhibitOverlap = new NavTactic(3, 6, 9999, 1);
}
