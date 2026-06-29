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
		KBatchedAnimController kbatchedAnimController = null;
		Equippable component = base.GetComponent<Equippable>();
		if (component.assignee != null)
		{
			Transform transform = component.assignee.GetSoleOwner().transform;
			kbatchedAnimController = transform.GetComponent<KBatchedAnimController>();
		}
		return kbatchedAnimController;
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
		if (assigneeController == null)
		{
			return;
		}
		KAnimFile anim = Assets.GetAnim("helm_oxygen_kanim");
		KAnimHashedString kanimHashedString = new KAnimHashedString("snapTo_neck");
		assigneeController.GetComponent<SymbolOverrideController>().AddSymbolOverride(kanimHashedString, anim.GetData().build.GetSymbol(kanimHashedString), 5);
		assigneeController.SetSymbolVisiblity(kanimHashedString, true);
	}

	private void HideHelmet()
	{
		KBatchedAnimController assigneeController = this.GetAssigneeController();
		if (assigneeController == null)
		{
			return;
		}
		KAnimHashedString kanimHashedString = "snapTo_neck";
		SymbolOverrideController component = assigneeController.GetComponent<SymbolOverrideController>();
		if (component == null)
		{
			return;
		}
		component.RemoveSymbolOverride(kanimHashedString, 5);
		assigneeController.SetSymbolVisiblity(kanimHashedString, false);
	}

	private void OnUnequipped(object data)
	{
		Equippable component = base.GetComponent<Equippable>();
		if (component != null)
		{
			this.HideHelmet();
			IAssignableIdentity assignee = component.assignee;
			if (assignee != null)
			{
				KMonoBehaviour component2 = assignee.GetSoleOwner().transform.GetComponent<KMonoBehaviour>();
				component2.Unsubscribe(961737054, new Action<object>(this.OnBeginRecoverBreath));
				component2.Unsubscribe(-2037519664, new Action<object>(this.OnEndRecoverBreath));
			}
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
