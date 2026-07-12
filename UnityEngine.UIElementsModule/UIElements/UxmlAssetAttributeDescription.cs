using System;

namespace UnityEngine.UIElements
{
	internal class UxmlAssetAttributeDescription<T> : TypedUxmlAttributeDescription<T> where T : Object
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
			string text = null;
			bool flag = base.TryGetValueFromBag<string>(bag, cc, (string s, string t) => s, null, ref text);
			T t2;
			if (flag)
			{
				VisualTreeAsset visualTreeAsset = cc.visualTreeAsset;
				t2 = ((visualTreeAsset != null) ? visualTreeAsset.GetAsset<T>(text) : default(T));
			}
			else
			{
				t2 = default(T);
			}
			return t2;
		}
	}
}
