using System;

namespace UnityEngine
{
	internal class AudioAmbisonicExtensionDefinition
	{
		public AudioAmbisonicExtensionDefinition(string ambisonicNameIn, AudioExtensionDefinition definitionIn)
		{
			this.ambisonicPluginName = ambisonicNameIn;
			this.definition = definitionIn;
		}

		public PropertyName ambisonicPluginName;

		public AudioExtensionDefinition definition;
	}
}
