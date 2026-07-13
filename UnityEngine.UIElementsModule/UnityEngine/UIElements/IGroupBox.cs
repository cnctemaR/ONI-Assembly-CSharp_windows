using System;

namespace UnityEngine.UIElements
{
	internal interface IGroupBox
	{
		void OnOptionAdded(IGroupBoxOption option);

		void OnOptionRemoved(IGroupBoxOption option);
	}
}
