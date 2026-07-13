using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[Serializable]
	internal struct StyleSelectorPart
	{
		public string value
		{
			get
			{
				return this.m_Value;
			}
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			internal set
			{
				this.m_Value = value;
			}
		}

		public StyleSelectorType type
		{
			get
			{
				return this.m_Type;
			}
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			internal set
			{
				this.m_Type = value;
			}
		}

		public override string ToString()
		{
			return string.Format("[StyleSelectorPart: value={0}, type={1}]", this.value, this.type);
		}

		public static StyleSelectorPart CreateClass(string className)
		{
			return new StyleSelectorPart
			{
				m_Type = StyleSelectorType.Class,
				m_Value = className
			};
		}

		public static StyleSelectorPart CreatePseudoClass(string className)
		{
			return new StyleSelectorPart
			{
				m_Type = StyleSelectorType.PseudoClass,
				m_Value = className
			};
		}

		public static StyleSelectorPart CreateId(string Id)
		{
			return new StyleSelectorPart
			{
				m_Type = StyleSelectorType.ID,
				m_Value = Id
			};
		}

		public static StyleSelectorPart CreateType(Type t)
		{
			return new StyleSelectorPart
			{
				m_Type = StyleSelectorType.Type,
				m_Value = t.Name
			};
		}

		public static StyleSelectorPart CreateType(string typeName)
		{
			return new StyleSelectorPart
			{
				m_Type = StyleSelectorType.Type,
				m_Value = typeName
			};
		}

		public static StyleSelectorPart CreatePredicate(object predicate)
		{
			return new StyleSelectorPart
			{
				m_Type = StyleSelectorType.Predicate,
				tempData = predicate
			};
		}

		public static StyleSelectorPart CreateWildCard()
		{
			return new StyleSelectorPart
			{
				m_Value = "*",
				m_Type = StyleSelectorType.Wildcard
			};
		}

		[SerializeField]
		private string m_Value;

		[SerializeField]
		private StyleSelectorType m_Type;

		internal object tempData;
	}
}
