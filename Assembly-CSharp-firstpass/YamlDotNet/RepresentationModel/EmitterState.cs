using System;
using System.Collections.Generic;

namespace YamlDotNet.RepresentationModel
{
	internal class EmitterState
	{
		public HashSet<string> EmittedAnchors
		{
			get
			{
				return this.emittedAnchors;
			}
		}

		private readonly HashSet<string> emittedAnchors = new HashSet<string>();
	}
}
