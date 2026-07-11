using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	internal interface IDirectorDriver
	{
		IList<PlayableDirector> GetDrivenDirectors(IExposedPropertyTable resolver);
	}
}
