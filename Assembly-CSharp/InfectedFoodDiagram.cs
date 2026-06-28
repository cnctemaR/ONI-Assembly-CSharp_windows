using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfectedFoodDiagram : MonoBehaviour
{
	private void Update()
	{
		List<InfectedFoodDiagram.FoodBit> list = new List<InfectedFoodDiagram.FoodBit>();
		if (WorldInventory.Instance != null)
		{
			List<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(GameTags.Edible);
			if (pickupables == null)
			{
				return;
			}
			foreach (Pickupable pickupable in pickupables)
			{
				Edible component = pickupable.GetComponent<Edible>();
				if (component != null)
				{
					PrimaryElement component2 = component.GetComponent<PrimaryElement>();
					list.Add(new InfectedFoodDiagram.FoodBit
					{
						name = component.name,
						rations = component.Calories / 1000f / 1000f,
						disease = component2.DiseaseCount
					});
				}
			}
		}
		if (list.Count != 0)
		{
			list.Sort((InfectedFoodDiagram.FoodBit a, InfectedFoodDiagram.FoodBit b) => a.DiseasePerRation.CompareTo(b.DiseasePerRation));
			InfectedFoodDiagram.FoodBit foodBit = list[0];
			this.minText.text = "Min: " + foodBit.ToString();
			InfectedFoodDiagram.FoodBit foodBit2 = list[list.Count - 1];
			this.maxText.text = "Max: " + foodBit2.ToString();
			InfectedFoodDiagram.FoodBit foodBit3 = list[list.Count / 2];
			this.medianText.text = "Median: " + foodBit3.ToString();
			float num = list.Select<InfectedFoodDiagram.FoodBit, float>((InfectedFoodDiagram.FoodBit b) => b.rations).Sum();
			int num2 = list.Select<InfectedFoodDiagram.FoodBit, int>((InfectedFoodDiagram.FoodBit b) => b.disease).Sum();
			this.avgText.text = "Average: " + ((float)num2 / num).ToString();
		}
	}

	public LocText minText;

	public LocText maxText;

	public LocText avgText;

	public LocText medianText;

	private class FoodBit
	{
		public float DiseasePerRation
		{
			get
			{
				return (float)this.disease / this.rations;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}: {1:0.##} (x{2:0.##} = {3})", new object[] { this.name, this.DiseasePerRation, this.rations, this.disease });
		}

		public string name;

		public float rations;

		public int disease;
	}
}
