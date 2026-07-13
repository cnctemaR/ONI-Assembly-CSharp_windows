using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[Serializable]
	internal struct GlyphIDSequence
	{
		public uint[] glyphIDs
		{
			get
			{
				return this.m_GlyphIDs;
			}
			set
			{
				this.m_GlyphIDs = value;
			}
		}

		[NativeName("glyphIDs")]
		[SerializeField]
		private uint[] m_GlyphIDs;
	}
}
