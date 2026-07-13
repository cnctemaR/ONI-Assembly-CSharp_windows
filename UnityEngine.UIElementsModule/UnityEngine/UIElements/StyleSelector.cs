using System;
using System.Linq;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[Serializable]
	internal class StyleSelector
	{
		public StyleSelectorPart[] parts
		{
			get
			{
				return this.m_Parts;
			}
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
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
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
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

		public const int InvalidPseudoStateMask = -1;

		internal int pseudoStateMask = -1;

		internal int negatedPseudoStateMask = -1;
	}
}
