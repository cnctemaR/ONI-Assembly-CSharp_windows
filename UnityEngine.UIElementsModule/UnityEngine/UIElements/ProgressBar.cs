using System;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class ProgressBar : AbstractProgressBar
	{
		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : AbstractProgressBar.UxmlSerializedData
		{
			public override object CreateInstance()
			{
				return new ProgressBar();
			}
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<ProgressBar, AbstractProgressBar.UxmlTraits>
		{
		}
	}
}
