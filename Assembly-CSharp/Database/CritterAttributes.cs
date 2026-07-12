using System;
using Klei.AI;

namespace Database
{
	public class CritterAttributes : ResourceSet<Klei.AI.Attribute>
	{
		public CritterAttributes(ResourceSet parent)
			: base("CritterAttributes", parent)
		{
			this.Happiness = base.Add(new Klei.AI.Attribute("Happiness", false, Klei.AI.Attribute.Display.General, false, 0f, null, null, null));
			this.Metabolism = base.Add(new Klei.AI.Attribute("Metabolism", false, Klei.AI.Attribute.Display.Details, false, 0f, null, null, null));
			this.Metabolism.SetFormatter(new ToPercentAttributeFormatter(100f, GameUtil.TimeSlice.None));
		}

		public Klei.AI.Attribute Happiness;

		public Klei.AI.Attribute Metabolism;
	}
}
