using System;
using System.Linq;
using UnityEngine.Bindings;

namespace UnityEngine.StyleSheets
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	[Serializable]
	internal class StyleSelector
	{
		public StyleSelectorPart[] parts
		{
			get
			{
				return this.m_Parts;
			}
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			internal set
			{
				this.m_Parts = value;
			}
		}

		public StyleSelectorRelationship previousRelationship
		{
			get
			{
				return this.m_PreviousRelationship;
			}
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			internal set
			{
				this.m_PreviousRelationship = value;
			}
		}

		public override string ToString()
		{
			return string.Join(", ", this.parts.Select<StyleSelectorPart, string>((StyleSelectorPart p) => p.ToString()).ToArray<string>());
		}

		[SerializeField]
		private StyleSelectorPart[] m_Parts;

		[SerializeField]
		private StyleSelectorRelationship m_PreviousRelationship;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal int pseudoStateMask = -1;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal int negatedPseudoStateMask = -1;
	}
}
