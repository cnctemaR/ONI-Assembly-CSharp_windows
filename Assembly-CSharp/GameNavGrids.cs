using System;
using System.Collections.Generic;

public class GameNavGrids
{
	public GameNavGrids(Pathfinding pathfinding)
	{
		this.CreateDuplicantNavigation(pathfinding);
		this.CreateHatchNavigation(pathfinding);
		this.FlyerGrid1x1 = this.CreateFlyerNavigation(pathfinding, "FlyerNavGrid1x1", new CellOffset[]
		{
			new CellOffset(0, 0)
		});
		this.FlyerGrid1x2 = this.CreateFlyerNavigation(pathfinding, "FlyerNavGrid1x2", new CellOffset[]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1)
		});
		this.CreateSwimmerNavigation(pathfinding);
	}

	private void CreateDuplicantNavigation(Pathfinding pathfinding)
	{
		CellOffset[] array = new CellOffset[]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1)
		};
		NavGrid.Transition[] array2 = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, true, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 0, 1, false, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 0, -1, false, false, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 1, false, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 1, false, true, 1, string.Empty, new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 1),
				new NavOffset(NavType.Ladder, 1, 1)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 0, false, true, 1, string.Empty, new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Ladder, 1, 0)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, -1, false, false, 1, string.Empty, new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1),
				new CellOffset(1, -1),
				new CellOffset(1, -2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 0),
				new NavOffset(NavType.Ladder, 1, 0)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -2, false, false, 2, string.Empty, new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -1, false, false, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 2, false, true, 2, string.Empty, new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 0, 0, false, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 0, 1, false, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 0, -1, false, false, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 1, 0, false, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 0)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 1, 1, false, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Floor, 1, 0)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 1, -1, false, false, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Floor, 1, 0)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 2, 0, false, true, 1, string.Empty, new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1),
				new CellOffset(1, -2),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 0, 0, false, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 0, 1, false, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 0, -1, false, false, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 1, 0, false, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 0, 0)
			}),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 1, 1, false, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Ladder, 0, 1),
				new NavOffset(NavType.Floor, 0, 1)
			}),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 1, -1, false, false, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 0, -1),
				new NavOffset(NavType.Ladder, 0, -1)
			}),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 2, 0, false, true, 1, string.Empty, new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Floor, 1, 0)
			}),
			new NavGrid.Transition(NavType.Ladder, NavType.Ladder, 1, 0, false, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Ladder, 0, 1, true, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Ladder, 0, -1, true, false, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Ladder, 2, 0, false, true, 1, string.Empty, new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0])
		};
		NavGrid.Transition[] array3 = this.MirrorTransitions(array2);
		Dictionary<NavType, string> dictionary = new Dictionary<NavType, string>();
		dictionary[NavType.Floor] = "idle_default";
		dictionary[NavType.Ladder] = "ladder_idle";
		this.DuplicantGrid = new NavGrid("MinionNavGrid", array3, dictionary, array, new NavTableValidator[]
		{
			new GameNavGrids.FloorValidator(true, true),
			new GameNavGrids.LadderValidator()
		});
		pathfinding.AddNavGrid(this.DuplicantGrid);
	}

	private void CreateHatchNavigation(Pathfinding pathfinding)
	{
		CellOffset[] array = new CellOffset[]
		{
			new CellOffset(0, 0)
		};
		NavGrid.Transition[] array2 = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, true, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 0, 1, false, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 0, -1, false, true, 1, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 1, false, true, 1, string.Empty, new CellOffset[]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 0, false, true, 1, string.Empty, new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[]
			{
				new NavOffset(NavType.Ladder, 1, 0)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -2, false, true, 1, string.Empty, new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -1, false, true, 1, string.Empty, new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 2, false, true, 1, string.Empty, new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0])
		};
		NavGrid.Transition[] array3 = this.MirrorTransitions(array2);
		Dictionary<NavType, string> dictionary = new Dictionary<NavType, string>();
		dictionary[NavType.Floor] = "idle_default";
		this.HatchGrid = new NavGrid("HatchNavGrid", array3, dictionary, array, new NavTableValidator[]
		{
			new GameNavGrids.FloorValidator(false, false)
		});
		pathfinding.AddNavGrid(this.HatchGrid);
	}

	private NavGrid CreateFlyerNavigation(Pathfinding pathfinding, string id, CellOffset[] bounding_offsets)
	{
		NavGrid.Transition[] array = new NavGrid.Transition[]
		{
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 0, true, true, 2, string.Empty, new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 1, true, true, 2, "hover_hover_1_0", new CellOffset[]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, -1, true, true, 2, "hover_hover_1_0", new CellOffset[]
			{
				new CellOffset(1, 0),
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 0, 1, true, true, 3, "hover_hover_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 0, -1, true, true, 3, "hover_hover_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0])
		};
		NavGrid.Transition[] array2 = this.MirrorTransitions(array);
		Dictionary<NavType, string> dictionary = new Dictionary<NavType, string>();
		dictionary[NavType.Hover] = "Idle";
		NavGrid navGrid = new NavGrid(id, array2, dictionary, bounding_offsets, new NavTableValidator[]
		{
			new GameNavGrids.HoverValidator()
		});
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
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, 0, true, true, 2, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, 1, true, true, 2, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, -1, true, true, 2, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 0, 1, true, true, 3, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 0, -1, true, true, 3, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0])
		};
		NavGrid.Transition[] array3 = this.MirrorTransitions(array2);
		Dictionary<NavType, string> dictionary = new Dictionary<NavType, string>();
		dictionary[NavType.Swim] = "Idle";
		this.SwimmerGrid = new NavGrid("SwimmerNavGrid", array3, dictionary, array, new NavTableValidator[]
		{
			new GameNavGrids.SwimValidator()
		});
		pathfinding.AddNavGrid(this.SwimmerGrid);
	}

	private CellOffset[] MirrorOffsets(CellOffset[] offsets)
	{
		List<CellOffset> list = new List<CellOffset>();
		foreach (CellOffset cellOffset in offsets)
		{
			CellOffset cellOffset2 = cellOffset;
			cellOffset2.x = -cellOffset2.x;
			list.Add(cellOffset2);
		}
		return list.ToArray();
	}

	private NavOffset[] MirrorNavOffsets(NavOffset[] offsets)
	{
		List<NavOffset> list = new List<NavOffset>();
		foreach (NavOffset navOffset in offsets)
		{
			NavOffset navOffset2 = navOffset;
			navOffset2.offset.x = -navOffset2.offset.x;
			list.Add(navOffset2);
		}
		return list.ToArray();
	}

	private NavGrid.Transition[] MirrorTransitions(NavGrid.Transition[] transitions)
	{
		List<NavGrid.Transition> list = new List<NavGrid.Transition>();
		foreach (NavGrid.Transition transition in transitions)
		{
			list.Add(transition);
			if (transition.x != 0)
			{
				NavGrid.Transition transition2 = transition;
				transition2.x = -transition2.x;
				transition2.voidOffsets = this.MirrorOffsets(transition.voidOffsets);
				transition2.solidOffsets = this.MirrorOffsets(transition.solidOffsets);
				transition2.validNavOffsets = this.MirrorNavOffsets(transition.validNavOffsets);
				transition2.invalidNavOffsets = this.MirrorNavOffsets(transition.invalidNavOffsets);
				list.Add(transition2);
			}
		}
		return list.ToArray();
	}

	public NavGrid DuplicantGrid;

	public NavGrid HatchGrid;

	public NavGrid FlyerGrid1x2;

	public NavGrid FlyerGrid1x1;

	public NavGrid SwimmerGrid;

	public class SwimValidator : NavTableValidator
	{
		public SwimValidator()
		{
			World instance = World.Instance;
			instance.OnLiquidChanged = (Action<int>)Delegate.Combine(instance.OnLiquidChanged, new Action<int>(this.OnLiquidChanged));
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool flag = Grid.IsSubstantialLiquid(cell, 0.35f);
			nav_table.SetValid(cell, NavType.Swim, flag);
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
		public FloorValidator(bool allowLadders = true, bool allow_forcefield_traversal = false)
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(this.OnSolidChanged));
			Components.Ladders.Register(new Action<Ladder>(this.OnAddLadder), new Action<Ladder>(this.OnRemoveLadder));
			this.allowLadders = allowLadders;
			this.allowForcefieldTraversal = allow_forcefield_traversal;
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool flag = GameNavGrids.FloorValidator.IsWalkableCell(cell, Grid.CellBelow(cell), Grid.BitFields, this.allowLadders, this.allowForcefieldTraversal);
			nav_table.SetValid(cell, NavType.Floor, base.IsClear(cell, bounding_offsets, Grid.BitFields, this.allowForcefieldTraversal) && flag);
		}

		private static bool IsWalkableCell(int cell, int anchor_cell, ushort[] grid_bit_fields, bool allowLadders, bool allow_forcefield_traversal)
		{
			int num = Grid.CellAbove(cell);
			int num2 = Grid.CellAbove(num);
			if (Grid.IsValidCell(cell) && Grid.IsValidCell(num2) && Grid.IsValidCell(anchor_cell))
			{
				bool flag = !NavTableValidator.IsCellSolid(grid_bit_fields, cell, allow_forcefield_traversal);
				bool flag2 = NavTableValidator.IsCellSolid(grid_bit_fields, anchor_cell, allow_forcefield_traversal) || (grid_bit_fields[anchor_cell] & 2) != 0 || (!Grid.HasLadder[cell] && allowLadders && Grid.HasLadder[anchor_cell]);
				bool flag3 = !Grid.IsValidCell(num2) || !Grid.Element[num2].IsUnstable;
				return flag && flag2 && flag3;
			}
			return false;
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

		private bool allowLadders = true;

		private bool allowForcefieldTraversal;
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
			nav_table.SetValid(cell, NavType.Ladder, base.IsClear(cell, bounding_offsets, Grid.BitFields, true) && Grid.HasLadder[cell]);
		}

		public override void Clear()
		{
			Components.Ladders.Unregister(new Action<Ladder>(this.OnAddLadder), new Action<Ladder>(this.OnRemoveLadder));
		}
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
			int num = Grid.CellAbove(cell);
			if (Grid.IsValidCell(num))
			{
				nav_table.SetValid(cell, NavType.Hover, !Grid.IsSubstantialLiquid(cell, 0.35f) && base.IsClear(cell, bounding_offsets, Grid.BitFields, false));
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
}
