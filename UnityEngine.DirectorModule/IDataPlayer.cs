using System;

namespace UnityEngine.Playables
{
	internal interface IDataPlayer
	{
		void Bind(DataPlayableOutput output);

		void Release(DataPlayableOutput output);
	}
}
