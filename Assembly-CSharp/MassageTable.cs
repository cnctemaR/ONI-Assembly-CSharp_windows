using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class MassageTable : RelaxationPoint, IEffectDescriptor
{
	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		Effects component = worker.GetComponent<Effects>();
		for (int i = 0; i < MassageTable.EffectsRemoved.Length; i++)
		{
			string text = MassageTable.EffectsRemoved[i];
			component.Remove(text);
		}
	}

	public new int DescriptionOrder { get; set; }

	public new List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		base.GetEffectDescriptions(def);
		return null;
	}

	public new List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, UI.BUILDINGEFFECTS.STRESSREDUCEDPERMINUTE), GameUtil.GetFormattedPercent(this.stressModificationValue / 600f * 60f, GameUtil.TimeSlice.None)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.STRESSREDUCEDPERMINUTE, GameUtil.GetFormattedPercent(this.stressModificationValue / 600f * 60f, GameUtil.TimeSlice.None)));
		list.Add(descriptor);
		if (MassageTable.EffectsRemoved.Length > 0)
		{
			Descriptor descriptor2 = default(Descriptor);
			descriptor2.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, UI.BUILDINGEFFECTS.REMOVESEFFECTSUBTITLE), UI.BUILDINGEFFECTS.TOOLTIPS.REMOVESEFFECTSUBTITLE);
			list.Add(descriptor2);
			for (int i = 0; i < MassageTable.EffectsRemoved.Length; i++)
			{
				string text = MassageTable.EffectsRemoved[i];
				string text2 = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".NAME");
				string text3 = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".CAUSE");
				Descriptor descriptor3 = default(Descriptor);
				string text4 = UI.LISTENTRYTAB + UI.LISTENTRYTAB + "• ";
				descriptor3.SetupDescriptor(text4 + string.Format(UI.BUILDINGEFFECTS.REMOVEDEFFECT, text2), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REMOVEDEFFECT, text3));
				list.Add(descriptor3);
			}
		}
		return list;
	}

	private static readonly string[] EffectsRemoved = new string[] { "SoreBack" };
}
