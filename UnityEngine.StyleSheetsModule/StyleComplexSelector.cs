using System;
using System.Linq;
using UnityEngine.Bindings;

namespace UnityEngine.StyleSheets
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	[Serializable]
	internal class StyleComplexSelector
	{
		public int specificity
		{
			get
			{
				return this.m_Specificity;
			}
			internal set
			{
				this.m_Specificity = value;
			}
		}

		public StyleRule rule { get; internal set; }

		public bool isSimple
		{
			get
			{
				return this.selectors.Length == 1;
			}
		}

		public StyleSelector[] selectors
		{
			get
			{
				return this.m_Selectors;
			}
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			internal set
			{
				this.m_Selectors = value;
			}
		}

		public override string ToString()
		{
			return string.Format("[{0}]", string.Join(", ", this.m_Selectors.Select<StyleSelector, string>((StyleSelector x) => x.ToString()).ToArray<string>()));
		}

		[SerializeField]
		private int m_Specificity;

		[SerializeField]
		private StyleSelector[] m_Selectors;

		[SerializeField]
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal int ruleIndex;
	}
}
