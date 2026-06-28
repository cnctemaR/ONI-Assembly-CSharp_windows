using System;

public class SuitDiseaseHandler : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(-1617557748, new Action<object>(this.OnEquipped));
		this.Subscribe(-170173755, new Action<object>(this.OnUnequipped));
	}

	private void OnEquipped(object data)
	{
		Equipment equipment = (Equipment)data;
		PrimaryElement component = equipment.GetComponent<PrimaryElement>();
		component.ModifyDiseaseCountHandler = new Action<int, string>(this.OnModifyDiseaseCount);
		component.AddDiseaseHandler = new Action<byte, int, string>(this.OnAddDisease);
		component.ForcePermanentDiseaseContainer(true);
		component.SetDiseaseVisualProvider(base.gameObject);
	}

	private void OnUnequipped(object data)
	{
		Equipment equipment = (Equipment)data;
		PrimaryElement component = equipment.GetComponent<PrimaryElement>();
		component.ModifyDiseaseCountHandler = null;
		component.AddDiseaseHandler = null;
		component.ForcePermanentDiseaseContainer(false);
		component.SetDiseaseVisualProvider(null);
	}

	private void OnModifyDiseaseCount(int delta, string reason)
	{
		base.GetComponent<PrimaryElement>().ModifyDiseaseCount(delta, reason);
	}

	private void OnAddDisease(byte disease_idx, int delta, string reason)
	{
		base.GetComponent<PrimaryElement>().AddDisease(disease_idx, delta, reason);
	}
}
