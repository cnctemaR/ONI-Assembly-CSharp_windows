using System;
using Klei.AI;

namespace Database
{
	public class CritterAttributes : ResourceSet<Klei.AI.Attribute>
	{
		public CritterAttributes(ResourceSet parent)
			: base("CritterAttributes", parent)
		{
			this.Happiness = base.Add(new Klei.AI.Attribute("Happiness", Strings.Get("STRINGS.CREATURES.STATS.HAPPINESS.NAME"), "", Strings.Get("STRINGS.CREATURES.STATS.HAPPINESS.TOOLTIP"), 0f, Klei.AI.Attribute.Display.General, false, "ui_icon_happiness", null, null));
			this.Happiness.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Metabolism = base.Add(new Klei.AI.Attribute("Metabolism", false, Klei.AI.Attribute.Display.Details, false, 100f, "ui_icon_metabolism", null, null, null));
			this.Metabolism.SetFormatter(new ToPercentAttributeFormatter(100f, GameUtil.TimeSlice.None));
		}

		public Klei.AI.Attribute Happiness;

		public Klei.AI.Attribute Metabolism;
	}
}
