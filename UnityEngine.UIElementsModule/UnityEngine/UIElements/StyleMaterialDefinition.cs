using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct StyleMaterialDefinition : IStyleValue<MaterialDefinition>, IEquatable<StyleMaterialDefinition>
	{
		public MaterialDefinition value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = value;
				this.m_Keyword = StyleKeyword.Undefined;
			}
		}

		public StyleKeyword keyword
		{
			get
			{
				return this.m_Keyword;
			}
			set
			{
				this.m_Keyword = value;
			}
		}

		public StyleMaterialDefinition(MaterialDefinition m)
		{
			this = new StyleMaterialDefinition(m, StyleKeyword.Undefined);
		}

		internal StyleMaterialDefinition(object obj, StyleKeyword keyword)
		{
			this = new StyleMaterialDefinition(MaterialDefinition.FromObject(obj), keyword);
		}

		public StyleMaterialDefinition(Material m)
		{
			this = new StyleMaterialDefinition(m, StyleKeyword.Undefined);
		}

		public StyleMaterialDefinition(StyleKeyword keyword)
		{
			this = new StyleMaterialDefinition(null, keyword);
		}

		internal StyleMaterialDefinition(MaterialDefinition m, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = m;
		}

		public static bool operator ==(StyleMaterialDefinition lhs, StyleMaterialDefinition rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleMaterialDefinition lhs, StyleMaterialDefinition rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleMaterialDefinition(StyleKeyword keyword)
		{
			return new StyleMaterialDefinition(keyword);
		}

		public static implicit operator StyleMaterialDefinition(MaterialDefinition m)
		{
			return new StyleMaterialDefinition(m);
		}

		public static implicit operator StyleMaterialDefinition(Material m)
		{
			return new StyleMaterialDefinition(m);
		}

		public bool Equals(StyleMaterialDefinition other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is StyleMaterialDefinition)
			{
				StyleMaterialDefinition styleMaterialDefinition = (StyleMaterialDefinition)obj;
				flag = this.Equals(styleMaterialDefinition);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return (((this.m_Value != null) ? this.m_Value.GetHashCode() : 0) * 397) ^ (int)this.m_Keyword;
		}

		public override string ToString()
		{
			return this.DebugString<MaterialDefinition>();
		}

		[SerializeField]
		private MaterialDefinition m_Value;

		[SerializeField]
		private StyleKeyword m_Keyword;
	}
}
