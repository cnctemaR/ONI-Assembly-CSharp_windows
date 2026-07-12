using System;

namespace ProcGen.Noise
{
	public class Link
	{
		public Link.Type type { get; set; }

		public string name { get; set; }

		public Link()
		{
		}

		public Link(Link.Type type, string name)
		{
			this.type = type;
			this.name = name;
		}

		public enum Type
		{
			None,
			Primitive,
			Filter,
			Transformer,
			Selector,
			Modifier,
			Combiner,
			FloatPoints,
			ControlPoints,
			Terminator
		}
	}
}
