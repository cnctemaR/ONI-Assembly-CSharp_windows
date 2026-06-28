using System;
using System.Collections.Generic;
using UnityEngine;

public class GreedyGreen : StateMachineComponent<GreedyGreen.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		for (int i = 0; i < 7; i++)
		{
			if (global::UnityEngine.Random.Range(0, 100) > 50)
			{
				this.growthState.ForceMaturity(this.growthState.Maturity + 1);
				this.Mature();
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	private void OnDugOut(object param)
	{
		if (!Grid.Solid[Grid.PosToCell(base.transform.position)])
		{
			this.Emit();
			base.smi.GoTo(base.smi.sm.harvestable.death);
		}
	}

	private void Emit()
	{
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, "Stored CO2 Released", base.gameObject.transform, 1.5f, false);
		float num = Mathf.Max(1f, base.smi.master.consumer.consumedMass);
		base.smi.master.consumer.consumedMass = 0f;
		this.emitter.ForceEmit(num, byte.MaxValue, 0, -1f);
	}

	private void Mature()
	{
		if (!this.CellIsClear(this.nextGrowCell()))
		{
			this.growthState.Regress(1);
		}
		this.plantRenderer.SetStage(this.growthState.Maturity);
		this.SetMaturityDisplayed();
	}

	private int TopOfVineCell()
	{
		Vector3 vector = base.transform.position + (float)this.growthState.Maturity * this.GrowDirection;
		return Grid.PosToCell(vector);
	}

	private int nextGrowCell()
	{
		Vector3 vector = base.transform.position + (float)(this.growthState.Maturity + 1) * this.GrowDirection;
		return Grid.PosToCell(vector);
	}

	private void EnableGrow()
	{
		this.growthState.GrowingEnabled = true;
	}

	private void DisableGrow()
	{
		this.growthState.GrowingEnabled = false;
	}

	private void CleanUp()
	{
		global::Debug.Log("Clean up", null);
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
		}
		this.growthState.Regress(this.growthState.Maturity);
		Util.KDestroyGameObject(base.gameObject);
	}

	private void SetMaturityDisplayed()
	{
		this.plantRenderer.SetStage(this.growthState.Maturity);
		this.consumer.sampleCellOffset = Vector3.up * (float)this.growthState.Maturity;
		this.MarkTiles();
		this.SetCollider();
	}

	private void MarkTiles()
	{
		for (int i = 0; i < this.OccupiedCells.Count; i++)
		{
			if (Grid.Objects[this.OccupiedCells[i], 5] == base.gameObject)
			{
				Grid.Objects[this.OccupiedCells[i], 5] = null;
			}
			if (Grid.Objects[this.OccupiedCells[i], 1] == base.gameObject)
			{
				Grid.Objects[this.OccupiedCells[i], 1] = null;
			}
		}
		this.OccupiedCells.Clear();
		this.OccupiedCells.Add(Grid.PosToCell(base.gameObject.transform.position));
		Grid.Objects[Grid.PosToCell(base.gameObject.transform.position), 5] = base.gameObject;
		Grid.Objects[Grid.PosToCell(base.gameObject.transform.position), 1] = base.gameObject;
		for (int j = 1; j < this.growthState.Maturity; j++)
		{
			this.OccupiedCells.Add(Grid.PosToCell(base.gameObject.transform.position + this.GrowDirection * (float)j));
			Grid.Objects[Grid.PosToCell(base.gameObject.transform.position + this.GrowDirection * (float)j), 5] = base.gameObject;
			Grid.Objects[Grid.PosToCell(base.gameObject.transform.position + this.GrowDirection * (float)j), 1] = base.gameObject;
		}
	}

	private void SetCollider()
	{
		BoxCollider2D component = base.GetComponent<BoxCollider2D>();
		if (this.growthState.Maturity == 0)
		{
			component.size = Vector2.one;
			component.offset = Vector2.zero;
		}
		else
		{
			component.size = new Vector2(1f, (float)(this.growthState.Maturity + 1));
			component.offset = new Vector2(0f, (float)Mathf.Clamp((this.growthState.Maturity + 1) / 2, 1, this.growthState.Maturity));
		}
	}

	private void Spread(int startCell)
	{
		int num = Grid.CellLeft(startCell);
		int num2 = Grid.CellRight(startCell);
		int num3 = Grid.PosToCell(Grid.CellToPos(num2, 0f, -this.GrowDirection.y, 0f));
		int num4 = Grid.PosToCell(Grid.CellToPos(num, 0f, -this.GrowDirection.y, 0f));
		int num5 = Grid.PosToCell(Grid.CellToPos(num2, 0f, this.GrowDirection.y, 0f));
		int num6 = Grid.PosToCell(Grid.CellToPos(num, 0f, this.GrowDirection.y, 0f));
		if (this.PlantableCell(num))
		{
			GameObject gameObject = Scenario.SpawnPrefab(num, 0, 0, "GreedyGreen", Grid.SceneLayer.Ore, Folder.Entities);
			gameObject.SetActive(true);
		}
		else if (this.PlantableCell(num4))
		{
			GameObject gameObject2 = Scenario.SpawnPrefab(num, 0, (int)(-(int)this.GrowDirection.y), "GreedyGreen", Grid.SceneLayer.Ore, Folder.Entities);
			gameObject2.SetActive(true);
		}
		else if (this.PlantableCell(num6))
		{
			GameObject gameObject3 = Scenario.SpawnPrefab(num, 0, (int)this.GrowDirection.y, "GreedyGreen", Grid.SceneLayer.Ore, Folder.Entities);
			gameObject3.SetActive(true);
		}
		if (this.PlantableCell(num2))
		{
			GameObject gameObject4 = Scenario.SpawnPrefab(num2, 0, 0, "GreedyGreen", Grid.SceneLayer.Ore, Folder.Entities);
			gameObject4.SetActive(true);
		}
		else if (this.PlantableCell(num3))
		{
			GameObject gameObject5 = Scenario.SpawnPrefab(num2, 0, (int)(-(int)this.GrowDirection.y), "GreedyGreen", Grid.SceneLayer.Ore, Folder.Entities);
			gameObject5.SetActive(true);
		}
		else if (this.PlantableCell(num5))
		{
			GameObject gameObject6 = Scenario.SpawnPrefab(num2, 0, (int)this.GrowDirection.y, "GreedyGreen", Grid.SceneLayer.Ore, Folder.Entities);
			gameObject6.SetActive(true);
		}
	}

	private bool PlantableCell(int cell)
	{
		return Grid.Solid[cell] && !Grid.Solid[Grid.CellAbove(cell)] && Grid.Objects[cell, 5] == null && Grid.Objects[cell, 0] == null && Grid.Objects[cell, 1] == null;
	}

	private bool CellIsClear(int cell)
	{
		return !Grid.Solid[cell] && Grid.Objects[cell, 5] == null && Grid.Objects[cell, 0] == null && Grid.Objects[cell, 1] == null;
	}

	[MyCmpAdd]
	private PlantRenderer plantRenderer;

	[MyCmpAdd]
	private Harvestable harvestable;

	[MyCmpAdd]
	private ElementConsumer consumer;

	[MyCmpAdd]
	private ElementEmitter emitter;

	[MyCmpAdd]
	private GrowthState growthState;

	private GameScenePartitionerEntry partitionerEntry;

	private int[] markedTiles;

	public int rootCell;

	private Vector3 GrowDirection = Vector3.up;

	private List<int> OccupiedCells = new List<int>();

	public class StatesInstance : GameStateMachine<GreedyGreen.States, GreedyGreen.StatesInstance, GreedyGreen, object>.GameInstance
	{
		public StatesInstance(GreedyGreen smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<GreedyGreen.States, GreedyGreen.StatesInstance, GreedyGreen>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.harvestable;
			this.harvestable.EventTransition(GameHashes.Harvest, this.harvestable.harvest, null);
			this.harvestable.Enter(delegate(GreedyGreen.StatesInstance smi)
			{
				int num = Grid.PosToCell(smi.master);
				if (smi.master.PlantableCell(num))
				{
					int num2 = num;
					smi.master.rootCell = num2;
					smi.master.partitionerEntry = GameScenePartitioner.Instance.Add("GreedyGreens.Harvestable", smi.gameObject, num, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(smi.master.OnDugOut));
				}
				else if (Grid.Solid[Grid.CellBelow(num)])
				{
					smi.master.rootCell = Grid.CellBelow(num);
					smi.transform.SetPosition(smi.transform.position + Vector3.down);
					smi.master.partitionerEntry = GameScenePartitioner.Instance.Add("GreedyGreens.Harvestable", smi.gameObject, Grid.PosToCell(smi.gameObject), GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(smi.master.OnDugOut));
				}
				else
				{
					Util.KDestroyGameObject(smi.master.gameObject);
				}
				smi.master.SetCollider();
				smi.GoTo(this.harvestable.idle);
			});
			this.harvestable.idle.Enter(delegate(GreedyGreen.StatesInstance smi)
			{
				smi.master.MarkTiles();
				smi.master.SetCollider();
				smi.master.consumer.sampleCellOffset = smi.master.GrowDirection * (float)smi.master.growthState.Maturity;
				smi.master.consumer.enabled = true;
				smi.master.growthState.GrowingEnabled = true;
			}).EventTransition(GameHashes.GrowthStateMature, this.harvestable.grow, null);
			this.harvestable.harvest.Enter(delegate(GreedyGreen.StatesInstance smi)
			{
				smi.master.Emit();
				smi.master.harvestable.ForceCancelHarvest(null);
				smi.GoTo(this.harvestable.death);
			});
			this.harvestable.grow.Enter(delegate(GreedyGreen.StatesInstance smi)
			{
				smi.master.Spread(Grid.PosToCell(smi.transform.position));
				smi.master.Mature();
				smi.ScheduleGoTo(1f, this.harvestable.idle);
			});
			this.harvestable.death.Enter(delegate(GreedyGreen.StatesInstance smi)
			{
				smi.master.growthState.GrowingEnabled = false;
				smi.master.plantRenderer.SetDead(true);
				smi.Schedule(2f, delegate(object d)
				{
					smi.master.CleanUp();
				}, null);
			});
		}

		public GreedyGreen.States.HarvestableState harvestable;

		public class HarvestableState : GameStateMachine<GreedyGreen.States, GreedyGreen.StatesInstance, GreedyGreen, object>.State
		{
			public GameStateMachine<GreedyGreen.States, GreedyGreen.StatesInstance, GreedyGreen, object>.State idle;

			public GameStateMachine<GreedyGreen.States, GreedyGreen.StatesInstance, GreedyGreen, object>.State grow;

			public GameStateMachine<GreedyGreen.States, GreedyGreen.StatesInstance, GreedyGreen, object>.State harvest;

			public GameStateMachine<GreedyGreen.States, GreedyGreen.StatesInstance, GreedyGreen, object>.State death;
		}
	}
}
