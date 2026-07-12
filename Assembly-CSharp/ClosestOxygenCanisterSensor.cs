using System;
using System.Collections.Generic;

public class ClosestOxygenCanisterSensor : ClosestPickupableSensor<Pickupable>
{
	public ClosestOxygenCanisterSensor(Sensors sensors, bool shouldStartActive)
		: base(sensors, GameTags.Gas, shouldStartActive)
	{
		this.requiredTags = new Tag[] { GameTags.Breathable };
		this.BreathableGasses = ElementLoader.FindElements((Element element) => element.HasTag(GameTags.Breathable) && element.HasTag(GameTags.Gas));
	}

	public override HashSet<Tag> GetForbbidenTags()
	{
		if (this.consumableConsumer == null)
		{
			return new HashSet<Tag>(0);
		}
		HashSet<Tag> forbbidenTags = base.GetForbbidenTags();
		if (forbbidenTags == null || forbbidenTags.Count <= 0)
		{
			return forbbidenTags;
		}
		Tag[] array = new Tag[forbbidenTags.Count];
		base.GetForbbidenTags().CopyTo(array);
		HashSet<Tag> hashSet = new HashSet<Tag>();
		int i = 0;
		while (i < array.Length)
		{
			Tag tag = array[i];
			if (tag == ClosestOxygenCanisterSensor.GenericBreathableGassesTankTag)
			{
				using (List<Element>.Enumerator enumerator = this.BreathableGasses.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Element element = enumerator.Current;
						hashSet.Add(element.id.ToString());
					}
					goto IL_00BB;
				}
				goto IL_00B2;
			}
			goto IL_00B2;
			IL_00BB:
			i++;
			continue;
			IL_00B2:
			hashSet.Add(tag);
			goto IL_00BB;
		}
		return hashSet;
	}

	public static readonly Tag GenericBreathableGassesTankTag = new Tag("BreathableGasTank");

	private List<Element> BreathableGasses;
}
