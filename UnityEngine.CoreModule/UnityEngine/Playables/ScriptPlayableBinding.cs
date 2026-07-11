using System;

namespace UnityEngine.Playables
{
	/// <summary>
	///   <para>A PlayableBinding that contains information representing a ScriptingPlayableOutput.</para>
	/// </summary>
	public static class ScriptPlayableBinding
	{
		/// <summary>
		///   <para>Creates a PlayableBinding that contains information representing a ScriptPlayableOutput.</para>
		/// </summary>
		/// <param name="key">A reference to a UnityEngine.Object that acts as a key for this binding.</param>
		/// <param name="type">The type of object that will be bound to the ScriptPlayableOutput.</param>
		/// <param name="name">The name of the ScriptPlayableOutput.</param>
		/// <returns>
		///   <para>Returns a PlayableBinding that contains information that is used to create a ScriptPlayableOutput.</para>
		/// </returns>
		public static PlayableBinding Create(string name, Object key, Type type)
		{
			return PlayableBinding.CreateInternal(name, key, type, new PlayableBinding.CreateOutputMethod(ScriptPlayableBinding.CreateScriptOutput));
		}

		private static PlayableOutput CreateScriptOutput(PlayableGraph graph, string name)
		{
			return ScriptPlayableOutput.Create(graph, name);
		}
	}
}
