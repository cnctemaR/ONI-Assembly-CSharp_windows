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
		Element element = base.GetComponent<PrimaryElement>().Element;
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		if (element.IsLiquid)
		{
			GameUtil.TintLiquidSymbolOnBuilding("substance_tinter", component, element);
			return;
		}
		Color color = element.substance.colour;
		color.a = 1f;
		component.SetSymbolTint(SubstanceChunk.symbolToTint, color);
		component.SetSymbolTint(SubstanceChunk.symbolToTint2, color);
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

	private const string symbolName = "substance_tinter";

	private static readonly KAnimHashedString symbolToTint = new KAnimHashedString("substance_tinter");

	private static readonly KAnimHashedString symbolToTint2 = new KAnimHashedString("substance_tinter_cap");
}
