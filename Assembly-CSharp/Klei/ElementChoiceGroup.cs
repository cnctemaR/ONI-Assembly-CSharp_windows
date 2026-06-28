using System;
using System.Collections.Generic;
using KSerialization.Converters;

namespace Klei
{
	public class ElementChoiceGroup
	{
		public ElementChoiceGroup()
		{
			this.choices = new List<WeightedSimHash>();
		}

		public ElementChoiceGroup(List<WeightedSimHash> choices, Room.Selection selectionMethod)
		{
			this.choices = choices;
			this.selectionMethod = selectionMethod;
		}

		[StringEnumConverter]
		public Room.Selection selectionMethod { get; private set; }

		public List<WeightedSimHash> choices { get; private set; }
	}
}
