using System;

public class GasBreatherFromWorldProvider : OxygenBreather.IGasProvider
{
	public GasBreatherFromWorldProvider.BreathableCellData GetBestBreathableCellAtCurrentLocation()
	{
		return GasBreatherFromWorldProvider.GetBestBreathableCellAroundSpecificCell(Grid.PosToCell(this.oxygenBreather), GasBreatherFromWorldProvider.DEFAULT_BREATHABLE_OFFSETS, this.oxygenBreather);
	}

	public static GasBreatherFromWorldProvider.BreathableCellData GetBestBreathableCellAroundSpecificCell(int theSpecificCell, CellOffset[] breathRange, OxygenBreather breather)
	{
		float num;
		return GasBreatherFromWorldProvider.GetBestBreathableCellAroundSpecificCell(theSpecificCell, breathRange, breather, out num);
	}

	public static GasBreatherFromWorldProvider.BreathableCellData GetBestBreathableCellAroundSpecificCell(int theSpecificCell, CellOffset[] breathRange, OxygenBreather breather, out float totalBreathableMassAroundCell)
	{
		if (breathRange == null)
		{
			breathRange = GasBreatherFromWorldProvider.DEFAULT_BREATHABLE_OFFSETS;
		}
		float num = 0f;
		int num2 = theSpecificCell;
		SimHashes simHashes = SimHashes.Vacuum;
		totalBreathableMassAroundCell = 0f;
		foreach (CellOffset cellOffset in breathRange)
		{
			int num3 = Grid.OffsetCell(theSpecificCell, cellOffset);
			SimHashes simHashes2;
			float breathableCellMass = GasBreatherFromWorldProvider.GetBreathableCellMass(num3, out simHashes2);
			totalBreathableMassAroundCell += breathableCellMass;
			if (breathableCellMass > num && breathableCellMass > breather.noOxygenThreshold)
			{
				num = breathableCellMass;
				num2 = num3;
				simHashes = simHashes2;
			}
		}
		return new GasBreatherFromWorldProvider.BreathableCellData
		{
			Cell = num2,
			ElementID = simHashes,
			Mass = num,
			IsBreathable = (simHashes != SimHashes.Vacuum)
		};
	}

	private static float GetBreathableCellMass(int cell, out SimHashes elementID)
	{
		elementID = SimHashes.Vacuum;
		if (Grid.IsValidCell(cell))
		{
			Element element = Grid.Element[cell];
			if (element.HasTag(GameTags.Breathable))
			{
				elementID = element.id;
				return Grid.Mass[cell];
			}
		}
		return 0f;
	}

	public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
	{
		this.oxygenBreather = oxygen_breather;
		this.nav = this.oxygenBreather.GetComponent<Navigator>();
	}

	public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
	{
	}

	public bool ShouldEmitCO2()
	{
		return this.nav.CurrentNavType != NavType.Tube;
	}

	public bool ShouldStoreCO2()
	{
		return false;
	}

	public bool IsLowOxygen()
	{
		GasBreatherFromWorldProvider.BreathableCellData bestBreathableCellAtCurrentLocation = this.GetBestBreathableCellAtCurrentLocation();
		return bestBreathableCellAtCurrentLocation.IsBreathable && bestBreathableCellAtCurrentLocation.Mass < this.oxygenBreather.lowOxygenThreshold;
	}

	public bool HasOxygen()
	{
		return this.oxygenBreather.prefabID.HasTag(GameTags.RecoveringBreath) || this.oxygenBreather.prefabID.HasTag(GameTags.InTransitTube) || this.GetBestBreathableCellAtCurrentLocation().IsBreathable;
	}

	public bool IsBlocked()
	{
		return this.oxygenBreather.HasTag(GameTags.HasSuitTank);
	}

	public bool ConsumeGas(OxygenBreather oxygen_breather, float mass_to_consume)
	{
		if (this.nav.CurrentNavType != NavType.Tube)
		{
			GasBreatherFromWorldProvider.BreathableCellData bestBreathableCellAtCurrentLocation = this.GetBestBreathableCellAtCurrentLocation();
			if (!bestBreathableCellAtCurrentLocation.IsBreathable)
			{
				return false;
			}
			SimHashes elementID = bestBreathableCellAtCurrentLocation.ElementID;
			HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add(new Action<Sim.MassConsumedCallback, object>(GasBreatherFromWorldProvider.OnSimConsumeCallback), oxygen_breather, "GasBreatherFromWorldProvider");
			SimMessages.ConsumeMass(bestBreathableCellAtCurrentLocation.Cell, elementID, mass_to_consume, 3, handle.index);
		}
		return true;
	}

	private static void OnSimConsumeCallback(Sim.MassConsumedCallback mass_cb_info, object data)
	{
		SimHashes id = ElementLoader.elements[(int)mass_cb_info.elemIdx].id;
		OxygenBreather.BreathableGasConsumed(data as OxygenBreather, id, mass_cb_info.mass, mass_cb_info.temperature, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount);
	}

	public static CellOffset[] DEFAULT_BREATHABLE_OFFSETS = new CellOffset[]
	{
		new CellOffset(0, 0),
		new CellOffset(0, 1),
		new CellOffset(1, 1),
		new CellOffset(-1, 1),
		new CellOffset(1, 0),
		new CellOffset(-1, 0)
	};

	private OxygenBreather oxygenBreather;

	private Navigator nav;

	public struct BreathableCellData
	{
		public int Cell;

		public SimHashes ElementID;

		public float Mass;

		public bool IsBreathable;
	}
}
