using System;

namespace UnityEngine.UIElements
{
	internal interface IStyleDataGroup<T>
	{
		T Copy();

		void CopyFrom(ref T other);
	}
}
