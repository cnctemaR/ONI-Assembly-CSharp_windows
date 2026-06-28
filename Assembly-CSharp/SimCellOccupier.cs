using System;
using KSerialization;
using UnityEngine;

[SkipSaveFileSerialization]
public class SimCellOccupier : KMonoBehaviour
{
	public bool IsVisuallySolid
	{
		get
		{
			return this.doReplaceElement;
		}
	}

	protected override void OnPrefabInit()
	{
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal, null);
	}

	protected override void OnSpawn()
	{
		HandleVector<Game.CallbackInfo>.Handle callbackHandle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(new global::System.Action(this.OnModifyComplete), false));
		int num = this.building.Def.PlacementOffsets.Length;
		float mass_per_cell = this.primaryElement.Mass / (float)num;
		this.building.RunOnArea(delegate(int offset_cell)
		{
			if (this.doReplaceElement)
			{
				SimMessages.ReplaceAndDisplaceElement(offset_cell, this.primaryElement.ElementID, CellEventLogger.Instance.SimCellOccupierOnSpawn, mass_per_cell, this.primaryElement.Temperature, this.primaryElement.DiseaseIdx, this.primaryElement.DiseaseCount, callbackHandle.index);
				SimMessages.SetStrength(offset_cell, 0, 1f);
				Game.Instance.RemoveSolidChangedFilter(offset_cell);
			}
			else
			{
				this.ForceSetGameCellData(offset_cell);
				Game.Instance.AddSolidChangedFilter(offset_cell);
			}
			Sim.Cell.Properties simCellProperties = this.GetSimCellProperties();
			SimMessages.SetCellProperties(offset_cell, (byte)simCellProperties);
			Grid.RenderedByWorld[offset_cell] = false;
			Grid.SuitRequired[offset_cell] = false;
			Game.Instance.GetComponent<EntombedItemVisualizer>().ForceClear(offset_cell);
		});
	}

	protected override void OnCleanUp()
	{
		if (this.callDestroy)
		{
			this.DestroySelf(null);
		}
	}

	private Sim.Cell.Properties GetSimCellProperties()
	{
		Sim.Cell.Properties properties = Sim.Cell.Properties.SolidImpermeable;
		if (this.setGasImpermeable)
		{
			properties |= Sim.Cell.Properties.GasImpermeable;
		}
		if (this.setLiquidImpermeable)
		{
			properties |= Sim.Cell.Properties.LiquidImpermeable;
		}
		return properties;
	}

	public void DestroySelf(global::System.Action onComplete)
	{
		this.callDestroy = false;
		global::UnityEngine.Debug.Assert(this.building.PlacementCells.Length == 1);
		for (int i = 0; i < this.building.PlacementCells.Length; i++)
		{
			int num = this.building.PlacementCells[i];
			Game.Instance.RemoveSolidChangedFilter(num);
			Sim.Cell.Properties simCellProperties = this.GetSimCellProperties();
			SimMessages.ClearCellProperties(num, (byte)simCellProperties);
			if (this.doReplaceElement && Grid.Element[num].id == this.primaryElement.ElementID)
			{
				HandleVector<int>.Handle handle = GameComps.DiseaseContainers.GetHandle(base.gameObject);
				if (handle.IsValid())
				{
					Sim.DiseaseCell diseaseCell = Grid.Disease[num];
					DiseaseContainer data = GameComps.DiseaseContainers.GetData(handle);
					data.diseaseIdx = diseaseCell.diseaseIdx;
					data.diseaseCount = diseaseCell.elementCount;
					GameComps.DiseaseContainers.SetData(handle, data);
				}
				if (onComplete != null)
				{
					int index = Game.Instance.callbackManager.Add(new Game.CallbackInfo(onComplete, false)).index;
					SimMessages.ReplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierDestroySelf, 0f, -1f, byte.MaxValue, 0, index);
				}
				else
				{
					SimMessages.ReplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierDestroySelf, 0f, -1f, byte.MaxValue, 0, -1);
				}
				SimMessages.SetStrength(num, 1, 1f);
			}
			else
			{
				Grid.SetSolid(num, false, CellEventLogger.Instance.SimCellOccupierDestroy);
				onComplete.Signal();
				World.Instance.OnSolidChanged(num);
				GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
			}
		}
	}

	public bool IsReady()
	{
		return this.isReady;
	}

	private void OnModifyComplete()
	{
		this.isReady = true;
	}

	private void ForceSetGameCellData(int cell)
	{
		Grid.PreviousSolid[cell] = Grid.Solid[cell];
		bool flag = !Grid.ForceField[cell];
		Grid.SetSolid(cell, flag, CellEventLogger.Instance.SimCellOccupierForceSolid);
		Pathfinding.Instance.AddDirtyNavGridCell(cell);
		GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.solidChangedLayer, null);
		Grid.Damage[cell] = 0f;
		Grid.SuitRequired[cell] = false;
	}

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[SerializeField]
	public bool doReplaceElement = true;

	[SerializeField]
	public bool setGasImpermeable;

	[SerializeField]
	public bool setLiquidImpermeable;

	private bool isReady;

	private bool callDestroy = true;

	[Serialize]
	private HashedString diseaseID;

	[Serialize]
	private int diseaseCount;
}
