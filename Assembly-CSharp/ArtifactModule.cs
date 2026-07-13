using System;
using UnityEngine;

public class ArtifactModule : SingleEntityReceptacle, IRenderEveryTick, IHexCellCollector
{
	protected override void OnSpawn()
	{
		this.craft = this.module.CraftInterface.GetComponent<Clustercraft>();
		if (this.craft.Status == Clustercraft.CraftStatus.InFlight && base.occupyingObject != null)
		{
			base.occupyingObject.SetActive(false);
		}
		base.OnSpawn();
		base.Subscribe(705820818, new Action<object>(this.OnEnterSpace));
		base.Subscribe(-1165815793, new Action<object>(this.OnExitSpace));
	}

	public void RenderEveryTick(float dt)
	{
		this.ArtifactTrackModulePosition();
	}

	private void ArtifactTrackModulePosition()
	{
		this.occupyingObjectRelativePosition = this.animController.Offset + Vector3.up * 0.5f + new Vector3(0f, 0f, -1f);
		if (base.occupyingObject != null)
		{
			this.PositionOccupyingObject();
		}
	}

	private void OnEnterSpace(object data)
	{
		if (base.occupyingObject != null)
		{
			base.occupyingObject.SetActive(false);
		}
	}

	private void OnExitSpace(object data)
	{
		if (base.occupyingObject != null)
		{
			base.occupyingObject.SetActive(true);
		}
	}

	public bool CheckIsCollecting()
	{
		return false;
	}

	public string GetProperName()
	{
		return base.GetComponent<RocketModuleCluster>().GetProperName();
	}

	public Sprite GetUISprite()
	{
		return Def.GetUISprite(base.gameObject.GetComponent<KPrefabID>().PrefabID(), "ui", false).first;
	}

	public float GetCapacity()
	{
		return 1f;
	}

	public float GetMassStored()
	{
		return (float)this.storage.items.Count;
	}

	public float TimeInState()
	{
		return 0f;
	}

	public string GetCapacityBarText()
	{
		return string.Format("{0} / {1}", this.GetMassStored(), this.GetCapacity());
	}

	[MyCmpReq]
	private KBatchedAnimController animController;

	[MyCmpReq]
	private RocketModuleCluster module;

	private Clustercraft craft;
}
