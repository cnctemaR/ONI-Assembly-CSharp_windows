using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class UxmlTraits
	{
		protected UxmlTraits()
		{
			this.canHaveAnyAttribute = true;
		}

		public bool canHaveAnyAttribute { get; protected set; }

		public virtual IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
		{
			get
			{
				foreach (UxmlAttributeDescription attributeDescription in this.GetAllAttributeDescriptionForType(base.GetType()))
				{
					yield return attributeDescription;
				}
				yield break;
			}
		}

		public virtual IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get
			{
				yield break;
			}
		}

		public virtual void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
		}

		private IEnumerable<UxmlAttributeDescription> GetAllAttributeDescriptionForType(Type t)
		{
			Type baseType = t.BaseType;
			if (baseType != null)
			{
				foreach (UxmlAttributeDescription ident in this.GetAllAttributeDescriptionForType(baseType))
				{
					yield return ident;
				}
			}
			foreach (FieldInfo fieldInfo in from f in t.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				where typeof(UxmlAttributeDescription).IsAssignableFrom(f.FieldType)
				select f)
			{
				yield return (UxmlAttributeDescription)fieldInfo.GetValue(this);
			}
			yield break;
		}
	}
}
