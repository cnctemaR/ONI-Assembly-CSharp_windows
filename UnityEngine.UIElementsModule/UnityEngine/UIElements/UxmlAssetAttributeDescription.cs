using System;

namespace UnityEngine.UIElements
{
	public class UxmlAssetAttributeDescription<T> : TypedUxmlAttributeDescription<T>, IUxmlAssetAttributeDescription where T : Object
	{
		public UxmlAssetAttributeDescription()
		{
			base.type = "string";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			base.defaultValue = default(T);
		}

		public override string defaultValueAsString
		{
			get
			{
				T t = base.defaultValue;
				return ((t != null) ? t.ToString() : null) ?? "null";
			}
		}

		public override T GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			string text;
			VisualTreeAsset visualTreeAsset;
			bool flag = base.TryGetValueFromBagAsString(bag, cc, out text, out visualTreeAsset) && visualTreeAsset != null;
			T t;
			if (flag)
			{
				t = visualTreeAsset.GetAsset<T>(text);
			}
			else
			{
				t = default(T);
			}
			return t;
		}

		public bool TryGetValueFromBag(IUxmlAttributes bag, CreationContext cc, out T value)
		{
			string text;
			VisualTreeAsset visualTreeAsset;
			bool flag = base.TryGetValueFromBagAsString(bag, cc, out text, out visualTreeAsset) && visualTreeAsset != null;
			bool flag2;
			if (flag)
			{
				value = visualTreeAsset.GetAsset<T>(text);
				flag2 = true;
			}
			else
			{
				value = default(T);
				flag2 = false;
			}
			return flag2;
		}

		Type IUxmlAssetAttributeDescription.assetType
		{
			get
			{
				return typeof(T);
			}
		}
	}
}
