using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.Experimental.UIElements
{
	public class UxmlFactory<TCreatedType, TTraits> : IUxmlFactory where TCreatedType : VisualElement where TTraits : UxmlTraits, new()
	{
		protected UxmlFactory()
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
				return (typeof(TCreatedType).Namespace == null) ? string.Empty : typeof(TCreatedType).Namespace;
			}
		}

		public virtual string uxmlQualifiedName
		{
			get
			{
				return typeof(TCreatedType).FullName;
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
				foreach (UxmlAttributeDescription attr in this.m_Traits.uxmlAttributesDescription)
				{
					yield return attr;
				}
				yield break;
			}
		}

		public virtual IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get
			{
				foreach (UxmlChildElementDescription child in this.m_Traits.uxmlChildElementsDescription)
				{
					yield return child;
				}
				yield break;
			}
		}

		public virtual string substituteForTypeName
		{
			get
			{
				string text;
				if (typeof(TCreatedType) == typeof(VisualElement))
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
				string text;
				if (typeof(TCreatedType) == typeof(VisualElement))
				{
					text = string.Empty;
				}
				else
				{
					text = ((typeof(VisualElement).Namespace == null) ? string.Empty : typeof(VisualElement).Namespace);
				}
				return text;
			}
		}

		public virtual string substituteForTypeQualifiedName
		{
			get
			{
				string text;
				if (typeof(TCreatedType) == typeof(VisualElement))
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

		[Obsolete("Call or override AcceptsAttributeBag(IUxmlAttributes bag, CreationContext cc) instaed")]
		public virtual bool AcceptsAttributeBag(IUxmlAttributes bag)
		{
			return this.AcceptsAttributeBag(bag, default(CreationContext));
		}

		public virtual bool AcceptsAttributeBag(IUxmlAttributes bag, CreationContext cc)
		{
			return true;
		}

		public virtual VisualElement Create(IUxmlAttributes bag, CreationContext cc)
		{
			Type[] array = new Type[]
			{
				typeof(IUxmlAttributes),
				typeof(CreationContext)
			};
			bool flag = base.GetType().GetMethod("DoCreate", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.ExactBinding, null, array, null) != null;
			TCreatedType tcreatedType;
			if (flag)
			{
				if (!UxmlFactory<TCreatedType, TTraits>.s_WarningLogged)
				{
					Debug.LogWarning("Calling obsolete method " + base.GetType().FullName + ".DoCreate(IUxmlAttributes bag, CreationContext cc). Remove and implemenent a default constructor for the created type instead.");
					UxmlFactory<TCreatedType, TTraits>.s_WarningLogged = true;
				}
				tcreatedType = this.DoCreate(bag, cc);
			}
			else
			{
				try
				{
					tcreatedType = (TCreatedType)((object)Activator.CreateInstance(typeof(TCreatedType)));
				}
				catch (MemberAccessException)
				{
					if (!UxmlFactory<TCreatedType, TTraits>.s_WarningLogged)
					{
						Debug.LogError("No accessible default constructor for " + typeof(TCreatedType));
						UxmlFactory<TCreatedType, TTraits>.s_WarningLogged = true;
					}
					tcreatedType = (TCreatedType)((object)null);
				}
			}
			if (tcreatedType != null)
			{
				this.m_Traits.Init(tcreatedType, bag, cc);
			}
			return tcreatedType;
		}

		[Obsolete("Remove and implemenent a default constructor for the created type instead.")]
		protected virtual TCreatedType DoCreate(IUxmlAttributes bag, CreationContext cc)
		{
			return (TCreatedType)((object)null);
		}

		[Obsolete("Use uxmlName and uxmlNamespace instead.")]
		public Type CreatesType
		{
			get
			{
				return typeof(TCreatedType);
			}
		}

		protected TTraits m_Traits;

		private static bool s_WarningLogged = false;
	}
}
