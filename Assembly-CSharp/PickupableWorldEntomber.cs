using System;
using System.Collections.Generic;

public class PickupableWorldEntomber : KMonoBehaviour, ISim33ms
{
	public void Add(Pickupable pickupable)
	{
		this.newlyAdded.Add(pickupable);
	}

	public static bool CanEntomb(Pickupable pickupable)
	{
		if (pickupable == null)
		{
			return false;
		}
		if (pickupable.storage != null)
		{
			return false;
		}
		int num = Grid.PosToCell(pickupable);
		if (!Grid.IsValidCell(num))
		{
			return false;
		}
		PrimaryElement component = pickupable.GetComponent<PrimaryElement>();
		if (component.Element.IsSolid && component.Element == Grid.Element[num])
		{
			ElementChunk component2 = pickupable.GetComponent<ElementChunk>();
			if (component2 != null)
			{
				return true;
			}
		}
		return false;
	}

	public void Sim33ms(float dt)
	{
		for (int i = 0; i < TuningData<PickupableWorldEntomber.Tuning>.Get().pickupablesPerFrame; i++)
		{
			if (this.pickupables.Count != 0)
			{
				Pickupable pickupable = this.pickupables[0];
				this.pickupables.RemoveAt(0);
				if (PickupableWorldEntomber.CanEntomb(pickupable))
				{
					int num = Grid.PosToCell(pickupable);
					PrimaryElement component = pickupable.GetComponent<PrimaryElement>();
					SimMessages.AddRemoveSubstance(num, component.Element.id, CellEventLogger.Instance.Vomit, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, false, -1);
					Util.KDestroyGameObject(pickupable.gameObject);
				}
			}
		}
		this.pickupables.AddRange(this.newlyAdded);
		this.newlyAdded.Clear();
	}

	private List<Pickupable> newlyAdded = new List<Pickupable>();

	private List<Pickupable> pickupables = new List<Pickupable>();

	public class Tuning : TuningData<PickupableWorldEntomber.Tuning>
	{
		public int pickupablesPerFrame;
	}
}
