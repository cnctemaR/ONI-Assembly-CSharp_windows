using System;
using UnityEngine;

namespace Klei.AI
{
	public static class ModifiersExtensions
	{
		public static Attributes GetAttributes(this KMonoBehaviour cmp)
		{
			return cmp.gameObject.GetAttributes();
		}

		public static Attributes GetAttributes(this GameObject go)
		{
			Modifiers component = go.GetComponent<Modifiers>();
			Attributes attributes;
			if (component != null)
			{
				attributes = component.attributes;
			}
			else
			{
				attributes = null;
			}
			return attributes;
		}

		public static Amounts GetAmounts(this KMonoBehaviour cmp)
		{
			return cmp.gameObject.GetAmounts();
		}

		public static Amounts GetAmounts(this GameObject go)
		{
			Modifiers component = go.GetComponent<Modifiers>();
			Amounts amounts;
			if (component != null)
			{
				amounts = component.amounts;
			}
			else
			{
				amounts = null;
			}
			return amounts;
		}

		public static Diseases GetDiseases(this KMonoBehaviour cmp)
		{
			return cmp.gameObject.GetDiseases();
		}

		public static Diseases GetDiseases(this GameObject go)
		{
			Modifiers component = go.GetComponent<Modifiers>();
			Diseases diseases;
			if (component != null)
			{
				diseases = component.diseases;
			}
			else
			{
				diseases = null;
			}
			return diseases;
		}
	}
}
