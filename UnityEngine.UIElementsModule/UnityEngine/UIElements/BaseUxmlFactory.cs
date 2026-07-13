using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	[Obsolete("BaseUxmlFactory<TCreatedType, TTraits> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public abstract class BaseUxmlFactory<TCreatedType, TTraits> where TCreatedType : new() where TTraits : BaseUxmlTraits, new()
	{
		protected BaseUxmlFactory()
		{
			this.m_Traits = new TTraits();
		}

		public virtual string uxmlName
		{
			get
			{
				return typeof(TCreatedType).Name;
			}
		}

		public virtual string uxmlNamespace
		{
			get
			{
				return typeof(TCreatedType).Namespace ?? string.Empty;
			}
		}

		public virtual string uxmlQualifiedName
		{
			get
			{
				return typeof(TCreatedType).FullName;
			}
		}

		public virtual Type uxmlType
		{
			get
			{
				return typeof(TCreatedType);
			}
		}

		public bool canHaveAnyAttribute
		{
			get
			{
				return this.m_Traits.canHaveAnyAttribute;
			}
		}

		public virtual IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
		{
			get
			{
				return this.m_Traits.uxmlAttributesDescription;
			}
		}

		public virtual IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get
			{
				return this.m_Traits.uxmlChildElementsDescription;
			}
		}

		public virtual string substituteForTypeName
		{
			get
			{
				bool flag = typeof(TCreatedType) == typeof(VisualElement);
				string text;
				if (flag)
				{
					text = string.Empty;
				}
				else
				{
					text = typeof(VisualElement).Name;
				}
				return text;
			}
		}

		public virtual string substituteForTypeNamespace
		{
			get
			{
				bool flag = typeof(TCreatedType) == typeof(VisualElement);
				string text;
				if (flag)
				{
					text = string.Empty;
				}
				else
				{
					text = typeof(VisualElement).Namespace ?? string.Empty;
				}
				return text;
			}
		}

		public virtual string substituteForTypeQualifiedName
		{
			get
			{
				bool flag = typeof(TCreatedType) == typeof(VisualElement);
				string text;
				if (flag)
				{
					text = string.Empty;
				}
				else
				{
					text = typeof(VisualElement).FullName;
				}
				return text;
			}
		}

		public virtual bool AcceptsAttributeBag(IUxmlAttributes bag, CreationContext cc)
		{
			return true;
		}

		internal TTraits m_Traits;
	}
}
