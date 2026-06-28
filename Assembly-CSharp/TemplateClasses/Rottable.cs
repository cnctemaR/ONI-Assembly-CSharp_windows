using System;
using Klei;

namespace TemplateClasses
{
	[Serializable]
	public class Rottable : YamlIO<Rottable>
	{
		public float rotAmount { get; set; }
	}
}
