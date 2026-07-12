using System;
using UnityEngine;

public class KleiItemDropScreen_PermitVis_DupeEquipment : KMonoBehaviour
{
	public void ConfigureWith(DropScreenPresentationInfo info)
	{
		this.dupeKAnim.GetComponent<UIDupeRandomizer>().Randomize();
		KAnimFile anim = Assets.GetAnim(info.BuildOverride);
		this.dupeKAnim.AddAnimOverrides(anim, 0f);
		KAnimHashedString kanimHashedString = new KAnimHashedString("snapto_neck");
		KAnim.Build.Symbol symbol = anim.GetData().build.GetSymbol(kanimHashedString);
		if (symbol != null)
		{
			this.dupeKAnim.GetComponent<SymbolOverrideController>().AddSymbolOverride(kanimHashedString, symbol, 6);
			this.dupeKAnim.SetSymbolVisiblity(kanimHashedString, true);
		}
		else
		{
			this.dupeKAnim.GetComponent<SymbolOverrideController>().RemoveSymbolOverride(kanimHashedString, 6);
			this.dupeKAnim.SetSymbolVisiblity(kanimHashedString, false);
		}
		this.dupeKAnim.Play("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
		this.dupeKAnim.Queue("cheer_pre", KAnim.PlayMode.Once, 1f, 0f);
		this.dupeKAnim.Queue("cheer_loop", KAnim.PlayMode.Once, 1f, 0f);
		this.dupeKAnim.Queue("cheer_pst", KAnim.PlayMode.Once, 1f, 0f);
		this.dupeKAnim.Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
	}

	[SerializeField]
	private KBatchedAnimController droppedItemKAnim;

	[SerializeField]
	private KBatchedAnimController dupeKAnim;
}
