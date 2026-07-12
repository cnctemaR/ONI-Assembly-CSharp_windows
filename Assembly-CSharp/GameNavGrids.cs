using System;
using System.Collections.Generic;
using UnityEngine;

public class GameNavGrids
{
	public GameNavGrids(Pathfinding pathfinding)
	{
		this.CreateDuplicantNavigation(pathfinding);
		this.WalkerGrid1x1 = this.CreateWalkerNavigation(pathfinding, "WalkerNavGrid1x1", new CellOffset[]
		{
			new CellOffset(0, 0)
		});
		this.WalkerBabyGrid1x1 = this.CreateWalkerBabyNavigation(pathfinding, "WalkerBabyNavGrid", new CellOffset[]
		{
			new CellOffset(0, 0)
		});
		this.WalkerGrid1x2 = this.CreateWalkerNavigation(pathfinding, "WalkerNavGrid1x2", new CellOffset[]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1)
		});
		this.WalkerGrid2x2 = this.CreateWalkerLargeNavigation(pathfinding, "WalkerNavGrid2x2", new CellOffset[]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1)
		});
		this.CreateDreckoNavigation(pathfinding);
		this.CreateDreckoBabyNavigation(pathfinding);
		this.CreateFloaterNavigation(pathfinding);
		this.FlyerGrid1x1 = this.CreateFlyerNavigation(pathfinding, "FlyerNavGrid1x1", new CellOffset[]
		{
			new CellOffset(0, 0)
		});
		this.FlyerGrid1x2 = this.CreateFlyerNavigation(pathfinding, "FlyerNavGrid1x2", new CellOffset[]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1)
		});
		this.FlyerGrid2x2 = this.CreateFlyerNavigation(pathfinding, "FlyerNavGrid2x2", new CellOffset[]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1),
			new CellOffset(1, 0),
			new CellOffset(1, 1)
		});
		this.CreateSwimmerNavigation(pathfinding);
		this.CreateDiggerNavigation(pathfinding);
		this.CreateSquirrelNavigation(pathfinding);
	}

	private void CreateDuplicantNavigation(Pathfinding pathfinding)
	{
		NavOffset[] array = new NavOffset[]
		{
			new NavOffset(NavType.Floor, 1, 0),
			new NavOffset(NavType.Ladder, 1, 0),
			new NavOffset(NavType.Pole, 1, 0)
		};
		CellOffset[] array2 = new CellOffset[]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1)
		};
		NavGrid.Transition[] array3 = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, NavAxis.NA, true, true, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 0, 1, NavAxis.NA, false, false, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 0, -1, NavAxis.NA, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 1, NavAxis.NA, false, false, true, 14, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 1, NavAxis.NA, false, false, true, 20, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1),
				new CellOffset(1, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 0),
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, 1),
				new NavOffset(NavType.Ladder, 1, 1),
				new NavOffset(NavType.Pole, 1, 1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 0, NavAxis.NA, false, false, true, 20, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], array, false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, -1, NavAxis.NA, false, false, false, 20, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 0),
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, -1),
				new NavOffset(NavType.Ladder, 1, -1),
				new NavOffset(NavType.Pole, 1, -1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -2, NavAxis.NA, false, false, false, 20, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -1, NavAxis.NA, false, false, false, 14, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 2, NavAxis.NA, false, false, true, 20, "", new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Teleport, 0, 0, NavAxis.NA, false, false, false, 14, "fall_pre", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Teleport, NavType.Floor, 0, 0, NavAxis.NA, false, false, false, 1, "fall_pst", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 0, 0, NavAxis.NA, false, false, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 0, 1, NavAxis.NA, false, false, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 0, -1, NavAxis.NA, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 1, 0, NavAxis.NA, false, false, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 0)
			}, false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 1, 1, NavAxis.NA, false, false, true, 14, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Floor, 1, 0)
			}, false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 1, -1, NavAxis.NA, false, false, false, 14, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Floor, 1, 0)
			}, false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 2, 0, NavAxis.NA, false, false, true, 20, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], array, false, 1f),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 0, 0, NavAxis.NA, false, false, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 0, 1, NavAxis.NA, false, false, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 0, -1, NavAxis.NA, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 1, 0, NavAxis.NA, false, false, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 0, 0)
			}, false, 1f),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 1, 1, NavAxis.NA, false, false, true, 14, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Ladder, 0, 1),
				new NavOffset(NavType.Floor, 0, 1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 1, -1, NavAxis.NA, false, false, false, 14, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 0, -1),
				new NavOffset(NavType.Ladder, 0, -1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 2, 0, NavAxis.NA, false, false, true, 20, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], array, false, 1f),
			new NavGrid.Transition(NavType.Ladder, NavType.Ladder, 1, 0, NavAxis.NA, false, false, true, 15, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ladder, NavType.Ladder, 0, 1, NavAxis.NA, true, true, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ladder, NavType.Ladder, 0, -1, NavAxis.NA, true, true, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ladder, NavType.Ladder, 2, 0, NavAxis.NA, false, false, true, 25, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], array, false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Pole, 0, 0, NavAxis.NA, false, false, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Pole, 0, 1, NavAxis.NA, false, false, true, 50, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Pole, 0, -1, NavAxis.NA, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Pole, 1, 0, NavAxis.NA, false, false, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 0)
			}, false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Pole, 1, 1, NavAxis.NA, false, false, true, 50, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, 0)
			}, false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Pole, 1, -1, NavAxis.NA, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, 0)
			}, false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Pole, 2, 0, NavAxis.NA, false, false, true, 50, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], array, false, 1f),
			new NavGrid.Transition(NavType.Pole, NavType.Floor, 0, 0, NavAxis.NA, false, false, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Pole, NavType.Floor, 0, 1, NavAxis.NA, false, false, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Pole, NavType.Floor, 0, -1, NavAxis.NA, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Pole, NavType.Floor, 1, 0, NavAxis.NA, false, false, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 0, 0)
			}, false, 1f),
			new NavGrid.Transition(NavType.Pole, NavType.Floor, 1, 1, NavAxis.NA, false, false, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Pole, 0, 1),
				new NavOffset(NavType.Floor, 0, 1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Pole, NavType.Floor, 1, -1, NavAxis.NA, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 0, -1),
				new NavOffset(NavType.Pole, 0, -1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Pole, NavType.Floor, 2, 0, NavAxis.NA, false, false, true, 20, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], array, false, 1f),
			new NavGrid.Transition(NavType.Pole, NavType.Ladder, 1, 0, NavAxis.NA, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Pole, NavType.Ladder, 0, 1, NavAxis.NA, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Pole, NavType.Ladder, 0, -1, NavAxis.NA, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Pole, NavType.Ladder, 2, 0, NavAxis.NA, false, false, false, 20, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], array, false, 1f),
			new NavGrid.Transition(NavType.Ladder, NavType.Pole, 1, 0, NavAxis.NA, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ladder, NavType.Pole, 0, 1, NavAxis.NA, false, false, false, 50, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ladder, NavType.Pole, 0, -1, NavAxis.NA, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ladder, NavType.Pole, 2, 0, NavAxis.NA, false, false, false, 20, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], array, false, 1f),
			new NavGrid.Transition(NavType.Pole, NavType.Pole, 1, 0, NavAxis.NA, false, false, true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Pole, NavType.Pole, 0, 1, NavAxis.NA, true, true, true, 50, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Pole, NavType.Pole, 0, -1, NavAxis.NA, true, true, false, 6, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Pole, NavType.Pole, 2, 0, NavAxis.NA, false, false, true, 50, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], array, false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Tube, 0, 2, NavAxis.NA, false, false, false, 40, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 1, 1, NavAxis.NA, false, false, false, 7, "", new CellOffset[]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 2, 1, NavAxis.NA, false, false, false, 13, "", new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 1, 2, NavAxis.NA, false, false, false, 13, "", new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 1, 0, NavAxis.NA, false, false, false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 2, 0, NavAxis.NA, false, false, false, 10, "", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 0)
			}, false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 1, -1, NavAxis.NA, false, false, false, 7, "", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 1, -2, NavAxis.NA, false, false, false, 13, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 2, -1, NavAxis.NA, false, false, false, 13, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(2, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, -1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 2, -2, NavAxis.NA, false, false, false, 17, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(2, 0),
				new CellOffset(1, -1),
				new CellOffset(2, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, -2)
			}, false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 0, -1, NavAxis.NA, false, false, false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 0, -2, NavAxis.NA, false, false, false, 10, "", new CellOffset[]
			{
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 0, 1, NavAxis.NA, false, false, false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 0, 2, NavAxis.NA, false, false, false, 10, "", new CellOffset[]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Ladder, 0, 1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 1, 1, NavAxis.NA, false, false, false, 7, "", new CellOffset[]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 2, 1, NavAxis.NA, false, false, false, 13, "", new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 1, 2, NavAxis.NA, false, false, false, 13, "", new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 1, 0, NavAxis.NA, false, false, false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 2, 0, NavAxis.NA, false, false, false, 10, "", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 0)
			}, false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 1, -1, NavAxis.NA, false, false, false, 7, "", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 1, -2, NavAxis.NA, false, false, false, 13, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 2, -1, NavAxis.NA, false, false, false, 13, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(2, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, -1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 2, -2, NavAxis.NA, false, false, false, 17, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(2, 0),
				new CellOffset(1, -1),
				new CellOffset(2, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, -2)
			}, false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 0, -1, NavAxis.NA, false, false, false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 0, -2, NavAxis.NA, false, false, false, 10, "", new CellOffset[]
			{
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 0, 1, NavAxis.NA, false, false, false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 0, 2, NavAxis.NA, false, false, false, 10, "", new CellOffset[]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Pole, 0, 1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 1, 1, NavAxis.NA, false, false, false, 7, "", new CellOffset[]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 2, 1, NavAxis.NA, false, false, false, 13, "", new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 1, 2, NavAxis.NA, false, false, false, 13, "", new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 1, 0, NavAxis.NA, false, false, false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 2, 0, NavAxis.NA, false, false, false, 10, "", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 0)
			}, false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 1, -1, NavAxis.NA, false, false, false, 7, "", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 1, -2, NavAxis.NA, false, false, false, 13, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 2, -1, NavAxis.NA, false, false, false, 13, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(2, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, -1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 2, -2, NavAxis.NA, false, false, false, 17, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(2, 0),
				new CellOffset(1, -1),
				new CellOffset(2, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, -2)
			}, false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 0, -1, NavAxis.NA, false, false, false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 0, -2, NavAxis.NA, false, false, false, 10, "", new CellOffset[]
			{
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Tube, 1, 0, NavAxis.NA, true, false, false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Tube, 0, 1, NavAxis.NA, true, false, false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Tube, 0, -1, NavAxis.NA, true, false, false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Tube, NavType.Tube, 1, 1, NavAxis.Y, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Tube, 0, 1)
			}, new NavOffset[0], false, 2.2f),
			new NavGrid.Transition(NavType.Tube, NavType.Tube, 1, 1, NavAxis.X, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Tube, 1, 0)
			}, new NavOffset[0], false, 2.2f),
			new NavGrid.Transition(NavType.Tube, NavType.Tube, 1, -1, NavAxis.Y, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Tube, 0, -1)
			}, new NavOffset[0], false, 2.2f),
			new NavGrid.Transition(NavType.Tube, NavType.Tube, 1, -1, NavAxis.X, false, false, false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Tube, 1, 0)
			}, new NavOffset[0], false, 2.2f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 0, NavAxis.NA, true, false, false, 15, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 0, 1, NavAxis.NA, true, false, false, 15, "hover_hover_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 0, -1, NavAxis.NA, true, false, false, 15, "hover_hover_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 1, NavAxis.NA, false, false, false, 25, "", new CellOffset[0], new CellOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Hover, 1, 0),
				new NavOffset(NavType.Hover, 0, 1)
			}, new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, -1, NavAxis.NA, false, false, false, 25, "", new CellOffset[0], new CellOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Hover, 1, 0),
				new NavOffset(NavType.Hover, 0, -1)
			}, new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Hover, 1, 0, NavAxis.NA, false, false, false, 15, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Hover, 0, 1, NavAxis.NA, false, false, false, 20, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Floor, 1, 0, NavAxis.NA, false, false, false, 15, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Floor, 0, -1, NavAxis.NA, false, false, false, 15, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f)
		};
		NavGrid.Transition[] array4 = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, -1, NavAxis.NA, false, false, false, 30, "climb_down_2_-1", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[]
			{
				new CellOffset(1, 1)
			}, new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 0),
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, -1),
				new NavOffset(NavType.Ladder, 1, -1),
				new NavOffset(NavType.Pole, 1, -1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 1, NavAxis.NA, false, false, false, 30, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[]
			{
				new CellOffset(1, 2)
			}, new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 0),
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, 1),
				new NavOffset(NavType.Ladder, 1, 1),
				new NavOffset(NavType.Pole, 1, 1)
			}, false, 1f)
		};
		NavGrid.Transition[] array5 = this.MirrorTransitions(this.CombineTransitions(array3, array4));
		NavGrid.NavTypeData[] array6 = new NavGrid.NavTypeData[]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Floor,
				idleAnim = "idle_default"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Ladder,
				idleAnim = "ladder_idle"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Pole,
				idleAnim = "pole_idle"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Tube,
				idleAnim = "tube_idle_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Hover,
				idleAnim = "hover_hover_1_0_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Teleport,
				idleAnim = "idle_default"
			}
		};
		this.DuplicantGrid = new NavGrid("MinionNavGrid", array5, array6, array2, new NavTableValidator[]
		{
			new GameNavGrids.FloorValidator(true),
			new GameNavGrids.LadderValidator(),
			new GameNavGrids.PoleValidator(),
			new GameNavGrids.TubeValidator(),
			new GameNavGrids.TeleporterValidator(),
			new GameNavGrids.FlyingValidator(true, true, true)
		}, 2, 3, 32);
		this.DuplicantGrid.updateEveryFrame = true;
		pathfinding.AddNavGrid(this.DuplicantGrid);
		NavGrid.Transition[] array7 = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, -1, NavAxis.NA, false, false, false, 30, "climb_down_2_-1", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[]
			{
				new CellOffset(1, 1)
			}, new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 0),
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, -1),
				new NavOffset(NavType.Ladder, 1, -1),
				new NavOffset(NavType.Pole, 1, -1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 1, NavAxis.NA, false, false, false, 30, "climb_up_2_1", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[]
			{
				new CellOffset(1, 2)
			}, new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 0),
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, 1),
				new NavOffset(NavType.Ladder, 1, 1),
				new NavOffset(NavType.Pole, 1, 1)
			}, false, 1f)
		};
		NavGrid.Transition[] array8 = this.MirrorTransitions(this.CombineTransitions(array3, array7));
		this.RobotGrid = new NavGrid("RobotNavGrid", array8, array6, array2, new NavTableValidator[]
		{
			new GameNavGrids.FloorValidator(true),
			new GameNavGrids.LadderValidator()
		}, 2, 3, 22);
		this.RobotGrid.updateEveryFrame = true;
		pathfinding.AddNavGrid(this.RobotGrid);
	}

	private NavGrid CreateWalkerNavigation(Pathfinding pathfinding, string id, CellOffset[] bounding_offsets)
	{
		NavGrid.Transition[] array = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, NavAxis.NA, true, true, true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 1, NavAxis.NA, false, false, true, 1, "", new CellOffset[]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 0, NavAxis.NA, false, false, true, 1, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -2, NavAxis.NA, false, false, true, 1, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -1, NavAxis.NA, false, false, true, 1, "", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 2, NavAxis.NA, false, false, true, 1, "", new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f)
		};
		NavGrid.Transition[] array2 = this.MirrorTransitions(array);
		NavGrid.NavTypeData[] array3 = new NavGrid.NavTypeData[]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Floor,
				idleAnim = "idle_loop"
			}
		};
		NavGrid navGrid = new NavGrid(id, array2, array3, bounding_offsets, new NavTableValidator[]
		{
			new GameNavGrids.FloorValidator(false)
		}, 2, 3, array2.Length);
		pathfinding.AddNavGrid(navGrid);
		return navGrid;
	}

	private NavGrid CreateWalkerBabyNavigation(Pathfinding pathfinding, string id, CellOffset[] bounding_offsets)
	{
		NavGrid.Transition[] array = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, NavAxis.NA, true, true, true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f)
		};
		NavGrid.Transition[] array2 = this.MirrorTransitions(array);
		NavGrid.NavTypeData[] array3 = new NavGrid.NavTypeData[]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Floor,
				idleAnim = "idle_loop"
			}
		};
		NavGrid navGrid = new NavGrid(id, array2, array3, bounding_offsets, new NavTableValidator[]
		{
			new GameNavGrids.FloorValidator(false)
		}, 2, 3, array2.Length);
		pathfinding.AddNavGrid(navGrid);
		return navGrid;
	}

	private NavGrid CreateWalkerLargeNavigation(Pathfinding pathfinding, string id, CellOffset[] bounding_offsets)
	{
		NavGrid.Transition[] array = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, NavAxis.NA, true, true, true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 1, NavAxis.NA, false, false, true, 1, "", new CellOffset[]
			{
				new CellOffset(2, 1)
			}, new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(2, 0)
			}, new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -1, NavAxis.NA, false, false, true, 1, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(2, 0),
				new CellOffset(2, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f)
		};
		NavGrid.Transition[] array2 = this.MirrorTransitions(array);
		NavGrid.NavTypeData[] array3 = new NavGrid.NavTypeData[]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Floor,
				idleAnim = "idle_loop"
			}
		};
		NavGrid navGrid = new NavGrid(id, array2, array3, bounding_offsets, new NavTableValidator[]
		{
			new GameNavGrids.FloorValidator(false)
		}, 2, 3, array2.Length);
		pathfinding.AddNavGrid(navGrid);
		return navGrid;
	}

	private void CreateDreckoNavigation(Pathfinding pathfinding)
	{
		CellOffset[] array = new CellOffset[]
		{
			new CellOffset(0, 0)
		};
		NavGrid.Transition[] array2 = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, NavAxis.NA, true, true, true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 0, NavAxis.NA, false, false, true, 3, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -2, NavAxis.NA, false, false, true, 4, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.LeftWall, 1, -2)
			}, true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 2, NavAxis.NA, false, false, true, 5, "", new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 2)
			}, new NavOffset[]
			{
				new NavOffset(NavType.RightWall, 0, 0)
			}, true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 2, NavAxis.NA, false, false, true, 4, "", new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.RightWall, 0, 0),
				new NavOffset(NavType.Floor, 2, 2)
			}, true, 1f),
			new NavGrid.Transition(NavType.RightWall, NavType.RightWall, 0, 1, NavAxis.NA, true, true, true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.LeftWall, NavType.LeftWall, 0, -1, NavAxis.NA, true, true, true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ceiling, NavType.Ceiling, -1, 0, NavAxis.NA, true, true, true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.RightWall, 0, 1, NavAxis.NA, false, false, true, 1, "floor_wall_0_1", new CellOffset[0], new CellOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.RightWall, 0, 0)
			}, new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.RightWall, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.RightWall, 0, 1)
			}, false, 1f),
			new NavGrid.Transition(NavType.RightWall, NavType.Ceiling, -1, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_1", new CellOffset[0], new CellOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Ceiling, 0, 0)
			}, new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.RightWall, NavType.Ceiling, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Ceiling, -1, 0)
			}, false, 1f),
			new NavGrid.Transition(NavType.Ceiling, NavType.LeftWall, 0, -1, NavAxis.NA, false, false, true, 1, "floor_wall_0_1", new CellOffset[0], new CellOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.LeftWall, 0, 0)
			}, new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ceiling, NavType.LeftWall, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.LeftWall, 0, -1)
			}, false, 1f),
			new NavGrid.Transition(NavType.LeftWall, NavType.Floor, 1, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_1", new CellOffset[0], new CellOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 0, 0)
			}, new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.LeftWall, NavType.Floor, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 0)
			}, false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.LeftWall, 1, -2, NavAxis.NA, false, false, true, 2, "floor_wall_1_-2", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.LeftWall, 1, -1)
			}, new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.LeftWall, 1, -1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.LeftWall, 1, -2)
			}, true, 1f),
			new NavGrid.Transition(NavType.LeftWall, NavType.Ceiling, -2, -1, NavAxis.NA, false, false, true, 2, "floor_wall_1_-2", new CellOffset[]
			{
				new CellOffset(0, -1),
				new CellOffset(-1, -1)
			}, new CellOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Ceiling, -1, -1)
			}, new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.LeftWall, NavType.Ceiling, -1, -1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Ceiling, -2, -1)
			}, true, 1f),
			new NavGrid.Transition(NavType.Ceiling, NavType.RightWall, -1, 2, NavAxis.NA, false, false, true, 2, "floor_wall_1_-2", new CellOffset[]
			{
				new CellOffset(-1, 0),
				new CellOffset(-1, 1)
			}, new CellOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.RightWall, -1, 1)
			}, new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Ceiling, NavType.RightWall, -1, 1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(-1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.RightWall, -1, 2)
			}, true, 1f),
			new NavGrid.Transition(NavType.RightWall, NavType.Floor, 2, 1, NavAxis.NA, false, false, true, 2, "floor_wall_1_-2", new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 1)
			}, new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.RightWall, NavType.Floor, 1, 1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 2, 1)
			}, true, 1f)
		};
		NavGrid.Transition[] array3 = this.MirrorTransitions(array2);
		NavGrid.NavTypeData[] array4 = new NavGrid.NavTypeData[]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Floor,
				idleAnim = "idle_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.RightWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0.5f, -0.5f, 0f),
				rotation = -1.5707964f
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Ceiling,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0f, -1f, 0f),
				rotation = -3.1415927f
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.LeftWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(-0.5f, -0.5f, 0f),
				rotation = -4.712389f
			}
		};
		this.DreckoGrid = new NavGrid("DreckoNavGrid", array3, array4, array, new NavTableValidator[]
		{
			new GameNavGrids.FloorValidator(false),
			new GameNavGrids.WallValidator(),
			new GameNavGrids.CeilingValidator()
		}, 2, 3, 16);
		pathfinding.AddNavGrid(this.DreckoGrid);
	}

	private void CreateDreckoBabyNavigation(Pathfinding pathfinding)
	{
		CellOffset[] array = new CellOffset[]
		{
			new CellOffset(0, 0)
		};
		NavGrid.Transition[] array2 = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, NavAxis.NA, true, true, true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.RightWall, NavType.RightWall, 0, 1, NavAxis.NA, true, true, true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.LeftWall, NavType.LeftWall, 0, -1, NavAxis.NA, true, true, true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ceiling, NavType.Ceiling, -1, 0, NavAxis.NA, true, true, true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.RightWall, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.RightWall, NavType.Ceiling, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ceiling, NavType.LeftWall, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.LeftWall, NavType.Floor, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.LeftWall, 1, -1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.LeftWall, NavType.Ceiling, -1, -1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Ceiling, NavType.RightWall, -1, 1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(-1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.RightWall, NavType.Floor, 1, 1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f)
		};
		NavGrid.Transition[] array3 = this.MirrorTransitions(array2);
		NavGrid.NavTypeData[] array4 = new NavGrid.NavTypeData[]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Floor,
				idleAnim = "idle_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.RightWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0.5f, -0.5f, 0f),
				rotation = -1.5707964f
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Ceiling,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0f, -1f, 0f),
				rotation = -3.1415927f
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.LeftWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(-0.5f, -0.5f, 0f),
				rotation = -4.712389f
			}
		};
		this.DreckoBabyGrid = new NavGrid("DreckoBabyNavGrid", array3, array4, array, new NavTableValidator[]
		{
			new GameNavGrids.FloorValidator(false),
			new GameNavGrids.WallValidator(),
			new GameNavGrids.CeilingValidator()
		}, 2, 3, 16);
		pathfinding.AddNavGrid(this.DreckoBabyGrid);
	}

	private void CreateFloaterNavigation(Pathfinding pathfinding)
	{
		CellOffset[] array = new CellOffset[]
		{
			new CellOffset(0, 0)
		};
		NavGrid.Transition[] array2 = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 0, NavAxis.NA, true, false, true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Hover, 1, -1)
			}, false, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 1, NavAxis.NA, false, false, true, 1, "", new CellOffset[]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Hover, 1, 0)
			}, true, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, -1, NavAxis.NA, false, false, true, 1, "", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Hover, 1, -2)
			}, true, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 0, 1, NavAxis.NA, false, false, true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Hover, 0, 0)
			}, false, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 0, -1, NavAxis.NA, false, false, true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Hover, 0, -2)
			}, false, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 2, 1, NavAxis.NA, false, false, true, 3, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Hover, 2, 0)
			}, true, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 2, 0, NavAxis.NA, false, false, true, 3, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Hover, 2, -1)
			}, true, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 2, -1, NavAxis.NA, false, false, true, 3, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1),
				new CellOffset(1, -2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Hover, 2, -2)
			}, true, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 2, NavAxis.NA, false, false, true, 3, "", new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Hover, 1, 1)
			}, true, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, -2, NavAxis.NA, false, false, true, 3, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Hover, 1, -3)
			}, true, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, 0, NavAxis.NA, true, true, true, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 0, 1, NavAxis.NA, true, true, true, 2, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, 1, NavAxis.NA, true, true, true, 2, "swim_swim_1_0", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 0, -1, NavAxis.NA, true, true, true, 10, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, -1, NavAxis.NA, true, true, true, 10, "swim_swim_1_0", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Hover, 0, 1, NavAxis.NA, true, true, true, 1, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Hover, 1, 0, NavAxis.NA, true, true, true, 1, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f)
		};
		NavGrid.Transition[] array3 = this.MirrorTransitions(array2);
		NavGrid.NavTypeData[] array4 = new NavGrid.NavTypeData[]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Hover,
				idleAnim = "idle_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Swim,
				idleAnim = "swim_idle_loop"
			}
		};
		this.FloaterGrid = new NavGrid("FloaterNavGrid", array3, array4, array, new NavTableValidator[]
		{
			new GameNavGrids.HoverValidator(),
			new GameNavGrids.SwimValidator()
		}, 2, 3, 22);
		pathfinding.AddNavGrid(this.FloaterGrid);
	}

	private NavGrid CreateFlyerNavigation(Pathfinding pathfinding, string id, CellOffset[] bounding_offsets)
	{
		NavGrid.Transition[] array = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 0, NavAxis.NA, true, true, true, 2, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 1, NavAxis.NA, true, true, true, 2, "hover_hover_1_0", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, -1, NavAxis.NA, true, true, true, 2, "hover_hover_1_0", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 0, 1, NavAxis.NA, true, true, true, 3, "hover_hover_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 0, -1, NavAxis.NA, true, true, true, 3, "hover_hover_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, 0, NavAxis.NA, true, true, true, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 0, 1, NavAxis.NA, true, true, true, 2, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, 1, NavAxis.NA, true, true, true, 2, "swim_swim_1_0", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 0, -1, NavAxis.NA, true, true, true, 10, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, -1, NavAxis.NA, true, true, true, 10, "swim_swim_1_0", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Hover, 0, 1, NavAxis.NA, true, true, true, 1, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Hover, 1, 0, NavAxis.NA, true, true, true, 1, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f)
		};
		NavGrid.Transition[] array2 = this.MirrorTransitions(array);
		NavGrid.NavTypeData[] array3 = new NavGrid.NavTypeData[]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Hover,
				idleAnim = "idle_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Swim,
				idleAnim = "idle_loop"
			}
		};
		NavGrid navGrid = new NavGrid(id, array2, array3, bounding_offsets, new NavTableValidator[]
		{
			new GameNavGrids.FlyingValidator(false, false, false),
			new GameNavGrids.SwimValidator()
		}, 2, 2, 16);
		pathfinding.AddNavGrid(navGrid);
		return navGrid;
	}

	private void CreateSwimmerNavigation(Pathfinding pathfinding)
	{
		CellOffset[] array = new CellOffset[]
		{
			new CellOffset(0, 0)
		};
		NavGrid.Transition[] array2 = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, 0, NavAxis.NA, true, true, true, 2, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, 1, NavAxis.NA, true, true, true, 2, "swim_swim_1_0", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, -1, NavAxis.NA, true, true, true, 2, "swim_swim_1_0", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 0, 1, NavAxis.NA, true, true, true, 3, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 0, -1, NavAxis.NA, true, true, true, 3, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f)
		};
		NavGrid.Transition[] array3 = this.MirrorTransitions(array2);
		NavGrid.NavTypeData[] array4 = new NavGrid.NavTypeData[]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Swim,
				idleAnim = "idle_loop"
			}
		};
		this.SwimmerGrid = new NavGrid("SwimmerNavGrid", array3, array4, array, new NavTableValidator[]
		{
			new GameNavGrids.SwimValidator()
		}, 1, 2, array3.Length);
		pathfinding.AddNavGrid(this.SwimmerGrid);
	}

	private void CreateDiggerNavigation(Pathfinding pathfinding)
	{
		CellOffset[] array = new CellOffset[]
		{
			new CellOffset(0, 0)
		};
		NavGrid.Transition[] array2 = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, NavAxis.NA, true, true, true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.RightWall, NavType.RightWall, 0, 1, NavAxis.NA, true, true, true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.LeftWall, NavType.LeftWall, 0, -1, NavAxis.NA, true, true, true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ceiling, NavType.Ceiling, -1, 0, NavAxis.NA, true, true, true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.RightWall, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.RightWall, NavType.Ceiling, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ceiling, NavType.LeftWall, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.LeftWall, NavType.Floor, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.LeftWall, 1, -1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.LeftWall, NavType.Ceiling, -1, -1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Ceiling, NavType.RightWall, -1, 1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(-1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.RightWall, NavType.Floor, 1, 1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Solid, NavType.Solid, 1, 0, NavAxis.NA, false, false, true, 1, "idle1", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Solid, NavType.Solid, 1, 1, NavAxis.NA, false, false, true, 1, "idle2", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Solid, NavType.Solid, 0, 1, NavAxis.NA, false, false, true, 1, "idle3", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Solid, NavType.Solid, 1, -1, NavAxis.NA, false, false, true, 1, "idle4", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Solid, 0, -1, NavAxis.NA, false, true, true, 1, "drill_in", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Solid, NavType.Floor, 0, 1, NavAxis.NA, false, false, true, 1, "drill_out", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ceiling, NavType.Solid, 0, 1, NavAxis.NA, false, true, true, 1, "drill_in", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Solid, NavType.Ceiling, 0, -1, NavAxis.NA, false, false, true, 1, "drill_out_ceiling", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Solid, NavType.LeftWall, 1, 0, NavAxis.NA, false, false, true, 1, "drill_out_left_wall", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.LeftWall, NavType.Solid, -1, 0, NavAxis.NA, false, true, true, 1, "drill_in", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Solid, NavType.RightWall, -1, 0, NavAxis.NA, false, false, true, 1, "drill_out_right_wall", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.RightWall, NavType.Solid, 1, 0, NavAxis.NA, false, true, true, 1, "drill_in", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f)
		};
		NavGrid.Transition[] array3 = this.MirrorTransitions(array2);
		NavGrid.NavTypeData[] array4 = new NavGrid.NavTypeData[]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Floor,
				idleAnim = "idle_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Ceiling,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0f, -1f, 0f),
				rotation = -3.1415927f
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.RightWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0.5f, -0.5f, 0f),
				rotation = -1.5707964f
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.LeftWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(-0.5f, -0.5f, 0f),
				rotation = -4.712389f
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Solid,
				idleAnim = "idle1"
			}
		};
		this.DiggerGrid = new NavGrid("DiggerNavGrid", array3, array4, array, new NavTableValidator[]
		{
			new GameNavGrids.SolidValidator(),
			new GameNavGrids.FloorValidator(false),
			new GameNavGrids.WallValidator(),
			new GameNavGrids.CeilingValidator()
		}, 2, 3, 22);
		pathfinding.AddNavGrid(this.DiggerGrid);
	}

	private void CreateSquirrelNavigation(Pathfinding pathfinding)
	{
		CellOffset[] array = new CellOffset[]
		{
			new CellOffset(0, 0)
		};
		NavGrid.Transition[] array2 = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, NavAxis.NA, true, true, true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 0, NavAxis.NA, false, false, true, 1, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 1, NavAxis.NA, false, false, true, 1, "", new CellOffset[]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 2, NavAxis.NA, false, false, true, 1, "", new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -1, NavAxis.NA, false, false, true, 1, "", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -2, NavAxis.NA, false, false, true, 1, "", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.RightWall, NavType.RightWall, 0, 1, NavAxis.NA, true, true, true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.LeftWall, NavType.LeftWall, 0, -1, NavAxis.NA, true, true, true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ceiling, NavType.Ceiling, -1, 0, NavAxis.NA, true, true, true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.RightWall, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.RightWall, NavType.Ceiling, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Ceiling, NavType.LeftWall, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.LeftWall, NavType.Floor, 0, 0, NavAxis.NA, false, false, true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], false, 1f),
			new NavGrid.Transition(NavType.Floor, NavType.LeftWall, 1, -1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.LeftWall, NavType.Ceiling, -1, -1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.Ceiling, NavType.RightWall, -1, 1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(-1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f),
			new NavGrid.Transition(NavType.RightWall, NavType.Floor, 1, 1, NavAxis.NA, false, false, true, 1, "floor_wall_1_-1", new CellOffset[]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], true, 1f)
		};
		NavGrid.Transition[] array3 = this.MirrorTransitions(array2);
		NavGrid.NavTypeData[] array4 = new NavGrid.NavTypeData[]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Floor,
				idleAnim = "idle_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Ceiling,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0f, -1f, 0f),
				rotation = -3.1415927f
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.RightWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0.5f, -0.5f, 0f),
				rotation = -1.5707964f
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.LeftWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(-0.5f, -0.5f, 0f),
				rotation = -4.712389f
			}
		};
		this.SquirrelGrid = new NavGrid("SquirrelNavGrid", array3, array4, array, new NavTableValidator[]
		{
			new GameNavGrids.FloorValidator(false),
			new GameNavGrids.WallValidator(),
			new GameNavGrids.CeilingValidator()
		}, 2, 3, 20);
		pathfinding.AddNavGrid(this.SquirrelGrid);
	}

	private CellOffset[] MirrorOffsets(CellOffset[] offsets)
	{
		List<CellOffset> list = new List<CellOffset>();
		foreach (CellOffset cellOffset in offsets)
		{
			cellOffset.x = -cellOffset.x;
			list.Add(cellOffset);
		}
		return list.ToArray();
	}

	private NavOffset[] MirrorNavOffsets(NavOffset[] offsets)
	{
		List<NavOffset> list = new List<NavOffset>();
		foreach (NavOffset navOffset in offsets)
		{
			navOffset.navType = NavGrid.MirrorNavType(navOffset.navType);
			navOffset.offset.x = -navOffset.offset.x;
			list.Add(navOffset);
		}
		return list.ToArray();
	}

	private NavGrid.Transition[] MirrorTransitions(NavGrid.Transition[] transitions)
	{
		List<NavGrid.Transition> list = new List<NavGrid.Transition>();
		foreach (NavGrid.Transition transition in transitions)
		{
			list.Add(transition);
			if (transition.x != 0 || transition.start == NavType.RightWall || transition.end == NavType.RightWall || transition.start == NavType.LeftWall || transition.end == NavType.LeftWall)
			{
				NavGrid.Transition transition2 = transition;
				transition2.x = -transition2.x;
				transition2.voidOffsets = this.MirrorOffsets(transition.voidOffsets);
				transition2.solidOffsets = this.MirrorOffsets(transition.solidOffsets);
				transition2.validNavOffsets = this.MirrorNavOffsets(transition.validNavOffsets);
				transition2.invalidNavOffsets = this.MirrorNavOffsets(transition.invalidNavOffsets);
				transition2.start = NavGrid.MirrorNavType(transition2.start);
				transition2.end = NavGrid.MirrorNavType(transition2.end);
				list.Add(transition2);
			}
		}
		list.Sort((NavGrid.Transition x, NavGrid.Transition y) => x.cost.CompareTo(y.cost));
		return list.ToArray();
	}

	private NavGrid.Transition[] CombineTransitions(NavGrid.Transition[] setA, NavGrid.Transition[] setB)
	{
		NavGrid.Transition[] array = new NavGrid.Transition[setA.Length + setB.Length];
		Array.Copy(setA, array, setA.Length);
		Array.Copy(setB, 0, array, setA.Length, setB.Length);
		Array.Sort<NavGrid.Transition>(array, (NavGrid.Transition x, NavGrid.Transition y) => x.cost.CompareTo(y.cost));
		return array;
	}

	public NavGrid DuplicantGrid;

	public NavGrid WalkerGrid1x1;

	public NavGrid WalkerBabyGrid1x1;

	public NavGrid WalkerGrid1x2;

	public NavGrid WalkerGrid2x2;

	public NavGrid DreckoGrid;

	public NavGrid DreckoBabyGrid;

	public NavGrid FloaterGrid;

	public NavGrid FlyerGrid1x2;

	public NavGrid FlyerGrid1x1;

	public NavGrid FlyerGrid2x2;

	public NavGrid SwimmerGrid;

	public NavGrid DiggerGrid;

	public NavGrid SquirrelGrid;

	public NavGrid RobotGrid;

	public class SwimValidator : NavTableValidator
	{
		public SwimValidator()
		{
			World instance = World.Instance;
			instance.OnLiquidChanged = (Action<int>)Delegate.Combine(instance.OnLiquidChanged, new Action<int>(this.OnLiquidChanged));
			GameScenePartitioner.Instance.AddGlobalLayerListener(GameScenePartitioner.Instance.objectLayers[9], new Action<int, object>(this.OnFoundationTileChanged));
		}

		private void OnFoundationTileChanged(int cell, object unused)
		{
			if (this.onDirty != null)
			{
				this.onDirty(cell);
			}
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool flag = Grid.IsSubstantialLiquid(cell, 0.35f);
			if (!flag)
			{
				flag = Grid.IsSubstantialLiquid(Grid.CellAbove(cell), 0.35f);
			}
			bool flag2 = Grid.IsWorldValidCell(cell) && flag && base.IsClear(cell, bounding_offsets, false);
			nav_table.SetValid(cell, NavType.Swim, flag2);
		}

		private void OnLiquidChanged(int cell)
		{
			if (this.onDirty != null)
			{
				this.onDirty(cell);
			}
		}
	}

	public class FloorValidator : NavTableValidator
	{
		public FloorValidator(bool is_dupe)
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(this.OnSolidChanged));
			Components.Ladders.Register(new Action<Ladder>(this.OnAddLadder), new Action<Ladder>(this.OnRemoveLadder));
			this.isDupe = is_dupe;
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool flag = GameNavGrids.FloorValidator.IsWalkableCell(cell, Grid.CellBelow(cell), this.isDupe);
			nav_table.SetValid(cell, NavType.Floor, flag && base.IsClear(cell, bounding_offsets, this.isDupe));
		}

		public static bool IsWalkableCell(int cell, int anchor_cell, bool is_dupe)
		{
			if (!Grid.IsWorldValidCell(cell))
			{
				return false;
			}
			if (!Grid.IsWorldValidCell(anchor_cell))
			{
				return false;
			}
			if (!NavTableValidator.IsCellPassable(cell, is_dupe))
			{
				return false;
			}
			if (Grid.FakeFloor[anchor_cell])
			{
				return true;
			}
			if (Grid.Solid[anchor_cell])
			{
				return !Grid.DupePassable[anchor_cell];
			}
			return is_dupe && (Grid.NavValidatorMasks[cell] & (Grid.NavValidatorFlags.Ladder | Grid.NavValidatorFlags.Pole)) == (Grid.NavValidatorFlags)0 && (Grid.NavValidatorMasks[anchor_cell] & (Grid.NavValidatorFlags.Ladder | Grid.NavValidatorFlags.Pole)) > (Grid.NavValidatorFlags)0;
		}

		private void OnAddLadder(Ladder ladder)
		{
			int num = Grid.PosToCell(ladder);
			if (this.onDirty != null)
			{
				this.onDirty(num);
			}
		}

		private void OnRemoveLadder(Ladder ladder)
		{
			int num = Grid.PosToCell(ladder);
			if (this.onDirty != null)
			{
				this.onDirty(num);
			}
		}

		private void OnSolidChanged(int cell)
		{
			if (this.onDirty != null)
			{
				this.onDirty(cell);
			}
		}

		public override void Clear()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(this.OnSolidChanged));
			Components.Ladders.Unregister(new Action<Ladder>(this.OnAddLadder), new Action<Ladder>(this.OnRemoveLadder));
		}

		private bool isDupe;
	}

	public class WallValidator : NavTableValidator
	{
		public WallValidator()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(this.OnSolidChanged));
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool flag = GameNavGrids.WallValidator.IsWalkableCell(cell, Grid.CellRight(cell));
			bool flag2 = GameNavGrids.WallValidator.IsWalkableCell(cell, Grid.CellLeft(cell));
			nav_table.SetValid(cell, NavType.RightWall, flag && base.IsClear(cell, bounding_offsets, false));
			nav_table.SetValid(cell, NavType.LeftWall, flag2 && base.IsClear(cell, bounding_offsets, false));
		}

		private static bool IsWalkableCell(int cell, int anchor_cell)
		{
			if (Grid.IsWorldValidCell(cell) && Grid.IsWorldValidCell(anchor_cell))
			{
				if (!NavTableValidator.IsCellPassable(cell, false))
				{
					return false;
				}
				if (Grid.Solid[anchor_cell])
				{
					return true;
				}
				if (Grid.CritterImpassable[anchor_cell])
				{
					return true;
				}
			}
			return false;
		}

		private void OnSolidChanged(int cell)
		{
			if (this.onDirty != null)
			{
				this.onDirty(cell);
			}
		}

		public override void Clear()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(this.OnSolidChanged));
		}
	}

	public class CeilingValidator : NavTableValidator
	{
		public CeilingValidator()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(this.OnSolidChanged));
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool flag = GameNavGrids.CeilingValidator.IsWalkableCell(cell, Grid.CellAbove(cell));
			nav_table.SetValid(cell, NavType.Ceiling, flag && base.IsClear(cell, bounding_offsets, false));
		}

		private static bool IsWalkableCell(int cell, int anchor_cell)
		{
			if (Grid.IsWorldValidCell(cell) && Grid.IsWorldValidCell(anchor_cell))
			{
				if (!NavTableValidator.IsCellPassable(cell, false))
				{
					return false;
				}
				if (Grid.Solid[anchor_cell])
				{
					return true;
				}
				if (Grid.HasDoor[cell] && !Grid.FakeFloor[cell])
				{
					return false;
				}
				if (Grid.FakeFloor[anchor_cell])
				{
					return true;
				}
				if (Grid.HasDoor[anchor_cell])
				{
					return true;
				}
			}
			return false;
		}

		private void OnSolidChanged(int cell)
		{
			if (this.onDirty != null)
			{
				this.onDirty(cell);
			}
		}

		public override void Clear()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(this.OnSolidChanged));
		}
	}

	public class LadderValidator : NavTableValidator
	{
		public LadderValidator()
		{
			Components.Ladders.Register(new Action<Ladder>(this.OnAddLadder), new Action<Ladder>(this.OnRemoveLadder));
		}

		private void OnAddLadder(Ladder ladder)
		{
			int num = Grid.PosToCell(ladder);
			if (this.onDirty != null)
			{
				this.onDirty(num);
			}
		}

		private void OnRemoveLadder(Ladder ladder)
		{
			int num = Grid.PosToCell(ladder);
			if (this.onDirty != null)
			{
				this.onDirty(num);
			}
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			nav_table.SetValid(cell, NavType.Ladder, base.IsClear(cell, bounding_offsets, true) && Grid.HasLadder[cell]);
		}

		public override void Clear()
		{
			Components.Ladders.Unregister(new Action<Ladder>(this.OnAddLadder), new Action<Ladder>(this.OnRemoveLadder));
		}
	}

	public class PoleValidator : GameNavGrids.LadderValidator
	{
		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			nav_table.SetValid(cell, NavType.Pole, base.IsClear(cell, bounding_offsets, true) && Grid.HasPole[cell]);
		}
	}

	public class TubeValidator : NavTableValidator
	{
		public TubeValidator()
		{
			Components.ITravelTubePieces.Register(new Action<ITravelTubePiece>(this.OnAddLadder), new Action<ITravelTubePiece>(this.OnRemoveLadder));
		}

		private void OnAddLadder(ITravelTubePiece tube)
		{
			int num = Grid.PosToCell(tube.Position);
			if (this.onDirty != null)
			{
				this.onDirty(num);
			}
		}

		private void OnRemoveLadder(ITravelTubePiece tube)
		{
			int num = Grid.PosToCell(tube.Position);
			if (this.onDirty != null)
			{
				this.onDirty(num);
			}
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			nav_table.SetValid(cell, NavType.Tube, Grid.HasTube[cell]);
		}

		public override void Clear()
		{
			Components.ITravelTubePieces.Unregister(new Action<ITravelTubePiece>(this.OnAddLadder), new Action<ITravelTubePiece>(this.OnRemoveLadder));
		}
	}

	public class TeleporterValidator : NavTableValidator
	{
		public TeleporterValidator()
		{
			Components.NavTeleporters.Register(new Action<NavTeleporter>(this.OnAddTeleporter), new Action<NavTeleporter>(this.OnRemoveTeleporter));
		}

		private void OnAddTeleporter(NavTeleporter teleporter)
		{
			int num = Grid.PosToCell(teleporter);
			if (this.onDirty != null)
			{
				this.onDirty(num);
			}
		}

		private void OnRemoveTeleporter(NavTeleporter teleporter)
		{
			int num = Grid.PosToCell(teleporter);
			if (this.onDirty != null)
			{
				this.onDirty(num);
			}
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool flag = Grid.IsWorldValidCell(cell) && Grid.HasNavTeleporter[cell];
			nav_table.SetValid(cell, NavType.Teleport, flag);
		}

		public override void Clear()
		{
			Components.NavTeleporters.Unregister(new Action<NavTeleporter>(this.OnAddTeleporter), new Action<NavTeleporter>(this.OnRemoveTeleporter));
		}
	}

	public class FlyingValidator : NavTableValidator
	{
		public FlyingValidator(bool exclude_floor = false, bool exclude_jet_suit_blockers = false, bool allow_door_traversal = false)
		{
			this.exclude_floor = exclude_floor;
			this.exclude_jet_suit_blockers = exclude_jet_suit_blockers;
			this.allow_door_traversal = allow_door_traversal;
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(this.MarkCellDirty));
			World instance2 = World.Instance;
			instance2.OnLiquidChanged = (Action<int>)Delegate.Combine(instance2.OnLiquidChanged, new Action<int>(this.MarkCellDirty));
			GameScenePartitioner.Instance.AddGlobalLayerListener(GameScenePartitioner.Instance.objectLayers[1], new Action<int, object>(this.OnBuildingChange));
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool flag = false;
			if (Grid.IsWorldValidCell(Grid.CellAbove(cell)))
			{
				flag = !Grid.IsSubstantialLiquid(cell, 0.35f) && base.IsClear(cell, bounding_offsets, this.allow_door_traversal);
				if (flag && this.exclude_floor)
				{
					int num = Grid.CellBelow(cell);
					if (Grid.IsWorldValidCell(num))
					{
						flag = base.IsClear(num, bounding_offsets, this.allow_door_traversal);
					}
				}
				if (flag && this.exclude_jet_suit_blockers)
				{
					GameObject gameObject = Grid.Objects[cell, 1];
					flag = gameObject == null || !gameObject.HasTag(GameTags.JetSuitBlocker);
				}
			}
			nav_table.SetValid(cell, NavType.Hover, flag);
		}

		private void OnBuildingChange(int cell, object data)
		{
			this.MarkCellDirty(cell);
		}

		private void MarkCellDirty(int cell)
		{
			if (this.onDirty != null)
			{
				this.onDirty(cell);
			}
		}

		public override void Clear()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(this.MarkCellDirty));
			World instance2 = World.Instance;
			instance2.OnLiquidChanged = (Action<int>)Delegate.Remove(instance2.OnLiquidChanged, new Action<int>(this.MarkCellDirty));
			GameScenePartitioner.Instance.RemoveGlobalLayerListener(GameScenePartitioner.Instance.objectLayers[1], new Action<int, object>(this.OnBuildingChange));
		}

		private bool exclude_floor;

		private bool exclude_jet_suit_blockers;

		private bool allow_door_traversal;

		private HandleVector<int>.Handle buildingParititonerEntry;
	}

	public class HoverValidator : NavTableValidator
	{
		public HoverValidator()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(this.MarkCellDirty));
			World instance2 = World.Instance;
			instance2.OnLiquidChanged = (Action<int>)Delegate.Combine(instance2.OnLiquidChanged, new Action<int>(this.MarkCellDirty));
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			int num = Grid.CellBelow(cell);
			if (Grid.IsWorldValidCell(num))
			{
				bool flag = Grid.Solid[num] || Grid.FakeFloor[num] || Grid.IsSubstantialLiquid(num, 0.35f);
				nav_table.SetValid(cell, NavType.Hover, !Grid.IsSubstantialLiquid(cell, 0.35f) && flag && base.IsClear(cell, bounding_offsets, false));
			}
		}

		private void MarkCellDirty(int cell)
		{
			if (this.onDirty != null)
			{
				this.onDirty(cell);
			}
		}

		public override void Clear()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(this.MarkCellDirty));
			World instance2 = World.Instance;
			instance2.OnLiquidChanged = (Action<int>)Delegate.Remove(instance2.OnLiquidChanged, new Action<int>(this.MarkCellDirty));
		}
	}

	public class SolidValidator : NavTableValidator
	{
		public SolidValidator()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(this.OnSolidChanged));
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool flag = GameNavGrids.SolidValidator.IsDiggable(cell, Grid.CellBelow(cell));
			nav_table.SetValid(cell, NavType.Solid, flag);
		}

		public static bool IsDiggable(int cell, int anchor_cell)
		{
			if (Grid.IsWorldValidCell(cell) && Grid.Solid[cell])
			{
				if (!Grid.HasDoor[cell] && !Grid.Foundation[cell])
				{
					ushort num = Grid.ElementIdx[cell];
					Element element = ElementLoader.elements[(int)num];
					return Grid.Element[cell].hardness < 150 && !element.HasTag(GameTags.RefinedMetal);
				}
				GameObject gameObject = Grid.Objects[cell, 1];
				if (gameObject != null)
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					return Grid.Element[cell].hardness < 150 && !component.Element.HasTag(GameTags.RefinedMetal);
				}
			}
			return false;
		}

		private void OnSolidChanged(int cell)
		{
			if (this.onDirty != null)
			{
				this.onDirty(cell);
			}
		}

		public override void Clear()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(this.OnSolidChanged));
		}
	}
}
