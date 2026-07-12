using System;

namespace UnityEngine.UIElements
{
	internal interface IPrefixLabel
	{
		string label { get; }

		Label labelElement { get; }
	}
}
