using System;

namespace UnityEngine.TextCore.Text
{
	internal struct RichTextTagAttribute
	{
		public int nameHashCode;

		public int valueHashCode;

		public TagValueType valueType;

		public int valueStartIndex;

		public int valueLength;

		public TagUnitType unitType;
	}
}
