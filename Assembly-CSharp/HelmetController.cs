using System;
using UnityEngine;

public class HelmetController : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(-1617557748, new Action<object>(this.OnEquipped));
		base.Subscribe(-170173755, new Action<object>(this.OnUnequipped));
	}

	private KBatchedAnimController GetAssigneeController()
	{
		Equippable component = base.GetComponent<Equippable>();
		Transform transform = component.assignee.GetSoleOwner().transform;
		return transform.GetComponent<KBatchedAnimController>();
	}

	private void OnEquipped(object data)
	{
		Equippable component = base.GetComponent<Equippable>();
		this.ShowHelmet();
		component.assignee.GetSoleOwner().transform.GetComponent<KMonoBehaviour>().Subscribe(961737054, new Action<object>(this.OnBeginRecoverBreath));
		component.assignee.GetSoleOwner().transform.GetComponent<KMonoBehaviour>().Subscribe(-2037519664, new Action<object>(this.OnEndRecoverBreath));
	}

	private void ShowHelmet()
	{
		KBatchedAnimController assigneeController = this.GetAssigneeController();
		KAnimFile anim = Assets.GetAnim("helm_oxygen_kanim");
		KAnimHashedString kanimHashedString = new KAnimHashedString("snapTo_neck");
		assigneeController.AddSymbolOverride(kanimHashedString, anim.batchTag, anim.GetData().build.GetSymbol(kanimHashedString), false);
		assigneeController.StopHidingSymbol(kanimHashedString, true);
		assigneeController.ShowSymbol(kanimHashedString);
	}

	private void HideHelmet()
	{
		KBatchedAnimController assigneeController = this.GetAssigneeController();
		KAnimHashedString kanimHashedString = new KAnimHashedString("snapTo_neck");
		assigneeController.RemoveSymbolOverride(kanimHashedString);
		assigneeController.HideSymbol(kanimHashedString, true);
		assigneeController.RemoveVisibleSymbol(kanimHashedString);
	}

	private void OnUnequipped(object data)
	{
		Equippable component = base.GetComponent<Equippable>();
		if (component != null)
		{
			this.HideHelmet();
			component.assignee.GetSoleOwner().transform.GetComponent<KMonoBehaviour>().Unsubscribe(961737054, new Action<object>(this.OnBeginRecoverBreath));
			component.assignee.GetSoleOwner().transform.GetComponent<KMonoBehaviour>().Unsubscribe(-2037519664, new Action<object>(this.OnEndRecoverBreath));
		}
	}

	private void OnBeginRecoverBreath(object data)
	{
		this.HideHelmet();
	}

	private void OnEndRecoverBreath(object data)
	{
		this.ShowHelmet();
	}
}
