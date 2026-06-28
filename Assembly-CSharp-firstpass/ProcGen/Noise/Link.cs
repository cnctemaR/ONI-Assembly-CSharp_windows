using System;

namespace ProcGen.Noise
{
	public class Link
	{
		public Link.Type type { get; set; }

		public string name { get; set; }

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
