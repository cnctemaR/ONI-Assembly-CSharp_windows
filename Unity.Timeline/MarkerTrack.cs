using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	[TrackBindingType(typeof(GameObject))]
	[HideInMenu]
	[Serializable]
	public class MarkerTrack : TrackAsset
	{
		public override IEnumerable<PlayableBinding> outputs
		{
			get
			{
				if (!(this == base.timelineAsset.markerTrack))
				{
					return base.outputs;
				}
				return new List<PlayableBinding> { ScriptPlayableBinding.Create(base.name, null, typeof(GameObject)) };
			}
		}
	}
}
