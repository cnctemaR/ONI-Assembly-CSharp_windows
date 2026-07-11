using System;
using System.Collections;
using STRINGS;
using UnityEngine;

public class SeedPlantingStates : GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.findSeed;
		GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State root = this.root;
		string text = CREATURES.STATUSITEMS.PLANTINGSEED.NAME;
		string text2 = CREATURES.STATUSITEMS.PLANTINGSEED.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, main).Exit(new StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback(SeedPlantingStates.UnreserveSeed)).Exit(new StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback(SeedPlantingStates.DropAll))
			.Exit(new StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback(SeedPlantingStates.RemoveMouthOverride));
		this.findSeed.Enter(delegate(SeedPlantingStates.Instance smi)
		{
			SeedPlantingStates.FindSeed(smi);
			if (smi.targetSeed == null)
			{
				smi.GoTo(this.behaviourcomplete);
			}
			else
			{
				SeedPlantingStates.ReserveSeed(smi);
				smi.GoTo(this.moveToSeed);
			}
		});
		this.moveToSeed.MoveTo(new Func<SeedPlantingStates.Instance, int>(SeedPlantingStates.GetSeedCell), this.findPlantLocation, this.behaviourcomplete, false);
		this.findPlantLocation.Enter(delegate(SeedPlantingStates.Instance smi)
		{
			if (smi.targetSeed)
			{
				SeedPlantingStates.FindDirtPlot(smi);
				if (smi.targetPlot != null || smi.targetDirtPlotCell != Grid.InvalidCell)
				{
					smi.GoTo(this.pickupSeed);
				}
				else
				{
					smi.GoTo(this.behaviourcomplete);
				}
			}
			else
			{
				smi.GoTo(this.behaviourcomplete);
			}
		});
		this.pickupSeed.PlayAnim("gather").Enter(new StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback(SeedPlantingStates.PickupComplete)).OnAnimQueueComplete(this.moveToPlantLocation);
		this.moveToPlantLocation.Enter(delegate(SeedPlantingStates.Instance smi)
		{
			if (smi.targetSeed == null)
			{
				smi.GoTo(this.behaviourcomplete);
			}
			else if (smi.targetPlot != null)
			{
				smi.GoTo(this.moveToPlot);
			}
			else if (smi.targetDirtPlotCell != Grid.InvalidCell)
			{
				smi.GoTo(this.moveToDirt);
			}
			else
			{
				smi.GoTo(this.behaviourcomplete);
			}
		});
		this.moveToDirt.MoveTo((SeedPlantingStates.Instance smi) => smi.targetDirtPlotCell, this.planting, this.behaviourcomplete, false);
		this.moveToPlot.Enter(delegate(SeedPlantingStates.Instance smi)
		{
			if (smi.targetPlot == null || smi.targetSeed == null)
			{
				smi.GoTo(this.behaviourcomplete);
			}
		}).MoveTo(new Func<SeedPlantingStates.Instance, int>(SeedPlantingStates.GetPlantableCell), this.planting, this.behaviourcomplete, false);
		this.planting.Enter(new StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback(SeedPlantingStates.RemoveMouthOverride)).PlayAnim("plant").Exit(new StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback(SeedPlantingStates.PlantComplete))
			.OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToPlantSeed, false);
	}

	private static void AddMouthOverride(SeedPlantingStates.Instance smi)
	{
		SymbolOverrideController component = smi.GetComponent<SymbolOverrideController>();
		KBatchedAnimController component2 = smi.GetComponent<KBatchedAnimController>();
		KAnim.Build.Symbol symbol = component2.AnimFiles[0].GetData().build.GetSymbol("sq_mouth_cheeks");
		if (symbol != null)
		{
			component.AddSymbolOverride("sq_mouth", symbol, 0);
		}
	}

	private static void RemoveMouthOverride(SeedPlantingStates.Instance smi)
	{
		SymbolOverrideController component = smi.GetComponent<SymbolOverrideController>();
		component.TryRemoveSymbolOverride("sq_mouth", 0);
	}

	private static void PickupComplete(SeedPlantingStates.Instance smi)
	{
		if (!smi.targetSeed)
		{
			global::Debug.LogWarningFormat("PickupComplete seed {0} is null", new object[] { smi.targetSeed });
			return;
		}
		SeedPlantingStates.UnreserveSeed(smi);
		int num = Grid.PosToCell(smi.targetSeed);
		if (smi.seed_cell != num)
		{
			global::Debug.LogWarningFormat("PickupComplete seed {0} moved {1} != {2}", new object[] { smi.targetSeed, num, smi.seed_cell });
			smi.targetSeed = null;
		}
		else if (smi.targetSeed.HasTag(GameTags.Stored))
		{
			global::Debug.LogWarningFormat("PickupComplete seed {0} was stored by {1}", new object[]
			{
				smi.targetSeed,
				smi.targetSeed.storage
			});
			smi.targetSeed = null;
		}
		else
		{
			smi.targetSeed = EntitySplitter.Split(smi.targetSeed, 1f, null);
			smi.GetComponent<Storage>().Store(smi.targetSeed.gameObject, false, false, true, false);
			SeedPlantingStates.AddMouthOverride(smi);
		}
	}

	private static void PlantComplete(SeedPlantingStates.Instance smi)
	{
		PlantableSeed plantableSeed = ((!smi.targetSeed) ? null : smi.targetSeed.GetComponent<PlantableSeed>());
		PlantablePlot plantablePlot;
		if (plantableSeed && SeedPlantingStates.CheckValidPlotCell(smi, plantableSeed, smi.targetDirtPlotCell, out plantablePlot))
		{
			if (plantablePlot)
			{
				if (plantablePlot.Occupant == null)
				{
					plantablePlot.ForceDepositPickupable(smi.targetSeed);
				}
			}
			else
			{
				plantableSeed.TryPlant(true);
			}
		}
		smi.targetSeed = null;
		smi.seed_cell = Grid.InvalidCell;
		smi.targetPlot = null;
	}

	private static void DropAll(SeedPlantingStates.Instance smi)
	{
		smi.GetComponent<Storage>().DropAll(false, false, default(Vector3), true);
	}

	private static int GetPlantableCell(SeedPlantingStates.Instance smi)
	{
		int num = Grid.PosToCell(smi.targetPlot);
		if (Grid.IsValidCell(num))
		{
			return Grid.CellAbove(num);
		}
		return num;
	}

	private static void FindDirtPlot(SeedPlantingStates.Instance smi)
	{
		smi.targetDirtPlotCell = Grid.InvalidCell;
		PlantableSeed component = smi.targetSeed.GetComponent<PlantableSeed>();
		PlantableCellQuery plantableCellQuery = PathFinderQueries.plantableCellQuery.Reset(component, 20);
		Navigator component2 = smi.GetComponent<Navigator>();
		component2.RunQuery(plantableCellQuery);
		if (plantableCellQuery.result_cells.Count > 0)
		{
			smi.targetDirtPlotCell = plantableCellQuery.result_cells[global::UnityEngine.Random.Range(0, plantableCellQuery.result_cells.Count)];
		}
	}

	private static bool CheckValidPlotCell(SeedPlantingStates.Instance smi, PlantableSeed seed, int cell, out PlantablePlot plot)
	{
		plot = null;
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		int num;
		if (seed.Direction == SingleEntityReceptacle.ReceptacleDirection.Bottom)
		{
			num = Grid.CellAbove(cell);
		}
		else
		{
			num = Grid.CellBelow(cell);
		}
		if (!Grid.IsValidCell(num))
		{
			return false;
		}
		if (!Grid.Solid[num])
		{
			return false;
		}
		GameObject gameObject = Grid.Objects[num, 1];
		if (gameObject)
		{
			plot = gameObject.GetComponent<PlantablePlot>();
			return plot != null;
		}
		return seed.TestSuitableGround(cell);
	}

	private static int GetSeedCell(SeedPlantingStates.Instance smi)
	{
		global::Debug.Assert(smi.targetSeed);
		global::Debug.Assert(smi.seed_cell != Grid.InvalidCell);
		return smi.seed_cell;
	}

	private static void FindSeed(SeedPlantingStates.Instance smi)
	{
		Navigator component = smi.GetComponent<Navigator>();
		Pickupable pickupable = null;
		int num = 100;
		IEnumerator enumerator = Components.PlantableSeeds.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				PlantableSeed plantableSeed = (PlantableSeed)obj;
				if (plantableSeed.HasTag(GameTags.Seed) || plantableSeed.HasTag(GameTags.CropSeed))
				{
					if (!plantableSeed.HasTag(GameTags.Creatures.ReservedByCreature))
					{
						if (Vector2.Distance(smi.transform.position, plantableSeed.transform.position) <= 25f)
						{
							int navigationCost = component.GetNavigationCost(Grid.PosToCell(plantableSeed));
							if (navigationCost != -1 && navigationCost < num)
							{
								pickupable = plantableSeed.GetComponent<Pickupable>();
								num = navigationCost;
							}
						}
					}
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		smi.targetSeed = pickupable;
		smi.seed_cell = ((!smi.targetSeed) ? Grid.InvalidCell : Grid.PosToCell(smi.targetSeed));
	}

	private static void ReserveSeed(SeedPlantingStates.Instance smi)
	{
		GameObject gameObject = ((!smi.targetSeed) ? null : smi.targetSeed.gameObject);
		if (gameObject != null)
		{
			DebugUtil.Assert(!gameObject.HasTag(GameTags.Creatures.ReservedByCreature));
			gameObject.AddTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static void UnreserveSeed(SeedPlantingStates.Instance smi)
	{
		GameObject gameObject = ((!smi.targetSeed) ? null : smi.targetSeed.gameObject);
		if (smi.targetSeed != null)
		{
			gameObject.RemoveTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private const int MAX_NAVIGATE_DISTANCE = 100;

	public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State findSeed;

	public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State moveToSeed;

	public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State pickupSeed;

	public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State findPlantLocation;

	public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State moveToPlantLocation;

	public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State moveToPlot;

	public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State moveToDirt;

	public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State planting;

	public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.GameInstance
	{
		public Instance(Chore<SeedPlantingStates.Instance> chore, SeedPlantingStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToPlantSeed);
		}

		public PlantablePlot targetPlot;

		public int targetDirtPlotCell = Grid.InvalidCell;

		public Element plantElement = ElementLoader.FindElementByHash(SimHashes.Dirt);

		public Pickupable targetSeed;

		public int seed_cell = Grid.InvalidCell;
	}
}
