using System;
using UnityEngine.Bindings;

namespace UnityEngine.StyleSheets
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	[Serializable]
	internal class StyleRule
	{
		public StyleProperty[] properties
		{
			get
			{
				return this.m_Properties;
			}
			internal set
			{
				this.m_Properties = value;
			}
		}

		[SerializeField]
		private StyleProperty[] m_Properties;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[SerializeField]
		internal int line;
	}
}
