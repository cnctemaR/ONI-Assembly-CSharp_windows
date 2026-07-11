using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/SubstanceChunk")]
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
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.RELEASEELEMENT.NAME, new global::System.Action(this.OnRelease), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.RELEASEELEMENT.TOOLTIP, true), 1f);
	}

	private void OnRelease()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		if (component.Mass > 0f)
		{
			SimMessages.AddRemoveSubstance(num, component.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, true, -1);
		}
		base.gameObject.DeleteObject();
	}

	private static readonly KAnimHashedString symbolToTint = new KAnimHashedString("substance_tinter");
}
