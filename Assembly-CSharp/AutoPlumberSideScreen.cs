using System;
using UnityEngine;

public class AutoPlumberSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		this.activateButton.onClick += delegate
		{
			DevAutoPlumber.AutoPlumbBuilding(this.building);
		};
		this.powerButton.onClick += delegate
		{
			DevAutoPlumber.DoElectricalPlumbing(this.building);
		};
		this.pipesButton.onClick += delegate
		{
			DevAutoPlumber.DoLiquidAndGasPlumbing(this.building);
		};
		this.solidsButton.onClick += delegate
		{
			DevAutoPlumber.SetupSolidOreDelivery(this.building);
		};
		this.minionButton.onClick += delegate
		{
			this.SpawnMinion();
		};
	}

	private void SpawnMinion()
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID), null, null);
		gameObject.name = Assets.GetPrefab(MinionConfig.ID).name;
		Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
		Vector3 vector = Grid.CellToPos(Grid.PosToCell(this.building), CellAlignment.Center, Grid.SceneLayer.Move);
		gameObject.transform.SetLocalPosition(vector);
		gameObject.SetActive(true);
		new MinionStartingStats(false, null, null).Apply(gameObject);
	}

	public override int GetSideScreenSortOrder()
	{
		return -150;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return DebugHandler.InstantBuildMode && target.GetComponent<Building>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		this.building = target.GetComponent<Building>();
		this.Refresh();
	}

	public override void ClearTarget()
	{
	}

	private void Refresh()
	{
	}

	public KButton activateButton;

	public KButton powerButton;

	public KButton pipesButton;

	public KButton solidsButton;

	public KButton minionButton;

	private Building building;
}
