using System;

namespace UnityEngine.UIElements
{
	internal interface IGroupBox<T> : IGroupBox where T : IGroupManager
	{
	}
}
