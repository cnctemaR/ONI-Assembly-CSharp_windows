using System;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[SkipSaveFileSerialization]
public class SubstanceChunk : KMonoBehaviour, ISaveLoadable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GameComps.ElementSplitters.Add(base.gameObject);
		this.Subscribe(-2064133523, new Action<object>(this.OnAbsorb));
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
	}

	protected override void OnCleanUp()
	{
		GameComps.ElementSplitters.Remove(base.gameObject);
		base.OnCleanUp();
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
				if (component2.Mass > 0f && component.Mass > 0f)
				{
					float num = SimUtil.CalculateFinalTemperature(component2.Mass, component2.Temperature, component.Mass, component.Temperature);
					component2.Temperature = num;
				}
				else if (component.Mass > 0f)
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
		userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.RELEASEELEMENT.NAME, new global::System.Action(this.OnRelease), global::Action.NumActions, null, null, null, text, true), 1f);
	}

	private void OnRelease()
	{
		int num = Grid.PosToCell(this.transform.position);
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		if (component.Mass > 0f)
		{
			SimMessages.AddRemoveSubstance(num, component.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, component.Mass, component.Temperature, -1);
		}
		base.gameObject.DeleteObject();
	}

	[MyCmpAdd]
	private UserMenu userMenu;
}
