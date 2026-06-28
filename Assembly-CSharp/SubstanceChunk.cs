using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
[SerializationConfig(MemberSerialization.OptIn)]
public class SubstanceChunk : KMonoBehaviour, ISaveLoadable
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Color color = base.GetComponent<PrimaryElement>().Element.substance.colour;
		color.a = 1f;
		base.GetComponent<KBatchedAnimController>().SetSymbolTint(SubstanceChunk.symbolToTint, color);
	}

	private void OnRefreshUserMenu(object data)
	{
		UserMenu userMenu = this.userMenu;
		string text = "action_deconstruct";
		string text2 = UI.USERMENUACTIONS.RELEASEELEMENT.NAME;
		global::System.Action action = new global::System.Action(this.OnRelease);
		string text3 = UI.USERMENUACTIONS.RELEASEELEMENT.TOOLTIP;
		userMenu.AddButton(new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 1f);
	}

	private void OnRelease()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		if (component.Mass > 0f)
		{
			SimMessages.AddRemoveSubstance(num, component.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, -1);
		}
		base.gameObject.DeleteObject();
	}

	[MyCmpAdd]
	private UserMenu userMenu;

	private static KAnimHashedString symbolToTint = new KAnimHashedString("substance_tinter");
}
