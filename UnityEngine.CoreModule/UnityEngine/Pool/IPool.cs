using System;

namespace UnityEngine.Pool
{
	internal interface IPool
	{
		int CountInactive { get; }

		void Clear();
	}
}
