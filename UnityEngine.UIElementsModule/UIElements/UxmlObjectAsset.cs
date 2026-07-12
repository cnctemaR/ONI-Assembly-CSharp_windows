using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	internal sealed class UxmlObjectAsset : UxmlAsset
	{
		public UxmlObjectAsset(string fullTypeName)
			: base(fullTypeName)
		{
		}
	}
}
