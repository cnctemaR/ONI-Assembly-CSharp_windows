using System;
using System.Diagnostics;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{Id}")]
	public class Amount : Resource
	{
		public Amount(string id, string name, string description, Attribute min_attribute, Attribute max_attribute, Attribute delta_attribute, bool show_max, Units units, float visual_delta_threshold, bool show_in_ui)
		{
			this.Id = id;
			this.Name = name;
			this.description = description;
			this.minAttribute = min_attribute;
			this.maxAttribute = max_attribute;
			this.deltaAttribute = delta_attribute;
			this.showMax = show_max;
			this.units = units;
			this.visualDeltaThreshold = visual_delta_threshold;
			this.showInUI = show_in_ui;
		}

		public void SetDisplayer(IAmountDisplayer displayer)
		{
			this.displayer = displayer;
			this.minAttribute.SetFormatter(displayer);
			this.maxAttribute.SetFormatter(displayer);
			this.deltaAttribute.SetFormatter(displayer);
		}

		public AmountInstance Lookup(Component cmp)
		{
			return this.Lookup(cmp.gameObject);
		}

		public AmountInstance Lookup(GameObject go)
		{
			Amounts amounts = go.GetAmounts();
			if (amounts != null)
			{
				return amounts.Get(this);
			}
			return null;
		}

		public string GetValueString(AmountInstance instance)
		{
			return this.displayer.GetValueString(this, instance);
		}

		public string GetDescription(AmountInstance instance)
		{
			return this.displayer.GetDescription(this, instance);
		}

		public string GetTooltip(AmountInstance instance)
		{
			return this.displayer.GetTooltip(this, instance);
		}

		public string description;

		public bool showMax;

		public Units units;

		public float visualDeltaThreshold;

		public Attribute minAttribute;

		public Attribute maxAttribute;

		public Attribute deltaAttribute;

		public bool showInUI;

		public IAmountDisplayer displayer;
	}
}
