using System;

namespace UnityEngine.Playables
{
	internal static class DataPlayableBinding
	{
		public static PlayableBinding Create<TDataStream, TPlayer>(string name, Object key) where TDataStream : new() where TPlayer : Object
		{
			return PlayableBinding.CreateInternal(name, key, typeof(TPlayer), new PlayableBinding.CreateOutputMethod(DataPlayableBinding.CreateDataOutput<TDataStream>));
		}

		private static PlayableOutput CreateDataOutput<TDataStream>(PlayableGraph graph, string name) where TDataStream : new()
		{
			return DataPlayableOutput.Create<TDataStream>(graph, name);
		}
	}
}
