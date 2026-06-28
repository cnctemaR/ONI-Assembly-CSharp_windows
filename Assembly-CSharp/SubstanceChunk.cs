using System;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SubstanceChunk : KMonoBehaviour, ISaveLoadableJson
{
	protected override void OnPrefabInit()
	{
		this.Subscribe(-2064133523, new EventSystem.EventHandler(this.OnAbsorb));
		this.Subscribe(493375141, new EventSystem.EventHandler(this.OnRefreshUserMenu));
	}

	private void OnAbsorb(object data)
	{
		GameObject gameObject = data as GameObject;
		if (gameObject != null)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			if (component != null)
			{
				PrimaryElement component2 = base.GetComponent<PrimaryElement>();
				if (component2.Mass > 0f)
				{
					float num = SimUtil.CalculateFinalTemperature(component2.Mass, component2.Temperature, component.Mass, component.Temperature);
					component2.Temperature = num;
				}
				else
				{
					component2.Temperature = component.Temperature;
				}
			}
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		UserMenu userMenu = this.userMenu;
		string text = UI.USERMENUACTIONS.RELEASEELEMENT.TOOLTIP;
		userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.RELEASEELEMENT.NAME, new global::System.Action(this.OnRelease), global::Action.NumActions, null, null, null, null, text));
	}

	private void OnRelease()
	{
		int num = Grid.PosToCell(this.transform.position);
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		if (component.Mass > 0f)
		{
			SimMessages.AddRemoveSubstance(num, component.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, component.Mass, component.Temperature, -1);
			component.gameObject.DeleteObject();
		}
	}

	[MyCmpAdd]
	private UserMenu userMenu;
}
