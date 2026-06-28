using System;

public class Conduit : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.Subscribe(-1201923725, new EventSystem.EventHandler(this.OnHighlighted));
	}

	protected override void OnSpawn()
	{
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Pipe, this);
		if (this.IsInsulated)
		{
			ConduitFlowVisualizer flowVisualizer = this.GetFlowVisualizer();
			flowVisualizer.SetInsulated(Grid.PosToCell(this.transform.position), true);
		}
	}

	protected override void OnCleanUp()
	{
		if (this.IsInsulated)
		{
			ConduitFlowVisualizer flowVisualizer = this.GetFlowVisualizer();
			flowVisualizer.SetInsulated(Grid.PosToCell(this.transform.position), false);
		}
		base.OnCleanUp();
	}

	private bool IsInsulated
	{
		get
		{
			return base.GetComponent<Building>().Def.Insulation < 1f;
		}
	}

	private ConduitFlowVisualizer GetFlowVisualizer()
	{
		Vent component = base.GetComponent<Vent>();
		return (component.TransferType != Vent.Transfer.Gas) ? Game.Instance.liquidFlowVisualizer : Game.Instance.gasFlowVisualizer;
	}

	private void OnHighlighted(object data)
	{
		bool flag = (bool)data;
		int num = ((!flag) ? (-1) : Grid.PosToCell(this.transform.position));
		ConduitFlowVisualizer flowVisualizer = this.GetFlowVisualizer();
		flowVisualizer.SetHighlightedCell(num);
	}
}
