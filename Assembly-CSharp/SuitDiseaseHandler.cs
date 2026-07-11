using System;

public class SuitDiseaseHandler : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<SuitDiseaseHandler>(-1617557748, SuitDiseaseHandler.OnEquippedDelegate);
		base.Subscribe<SuitDiseaseHandler>(-170173755, SuitDiseaseHandler.OnUnequippedDelegate);
	}

	private void OnEquipped(object data)
	{
		Equipment equipment = (Equipment)data;
		PrimaryElement component = equipment.GetComponent<PrimaryElement>();
		if (component != null)
		{
			component.ModifyDiseaseCountHandler = new Action<int, string>(this.OnModifyDiseaseCount);
			component.AddDiseaseHandler = new Action<byte, int, string>(this.OnAddDisease);
			component.ForcePermanentDiseaseContainer(true);
			component.SetDiseaseVisualProvider(base.gameObject);
		}
	}

	private void OnUnequipped(object data)
	{
		Equipment equipment = (Equipment)data;
		PrimaryElement component = equipment.GetComponent<PrimaryElement>();
		if (component != null)
		{
			component.ModifyDiseaseCountHandler = null;
			component.AddDiseaseHandler = null;
			component.ForcePermanentDiseaseContainer(false);
			component.SetDiseaseVisualProvider(null);
		}
	}

	private void OnModifyDiseaseCount(int delta, string reason)
	{
		base.GetComponent<PrimaryElement>().ModifyDiseaseCount(delta, reason);
	}

	private void OnAddDisease(byte disease_idx, int delta, string reason)
	{
		base.GetComponent<PrimaryElement>().AddDisease(disease_idx, delta, reason);
	}

	private static readonly EventSystem.IntraObjectHandler<SuitDiseaseHandler> OnEquippedDelegate = new EventSystem.IntraObjectHandler<SuitDiseaseHandler>(delegate(SuitDiseaseHandler component, object data)
	{
		component.OnEquipped(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SuitDiseaseHandler> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<SuitDiseaseHandler>(delegate(SuitDiseaseHandler component, object data)
	{
		component.OnUnequipped(data);
	});
}
