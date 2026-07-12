using System;

namespace UnityEngine.TextCore.Text
{
	internal struct CharacterSubstitution
	{
		public CharacterSubstitution(int index, uint unicode)
		{
			this.index = index;
			this.unicode = unicode;
		}

		public int index;

		public uint unicode;
	}
}
