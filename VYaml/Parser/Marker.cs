using System;
using System.Runtime.CompilerServices;

namespace VYaml.Parser
{
	public struct Marker
	{
		public Marker(int position, int line, int col)
		{
			this.Position = position;
			this.Line = line;
			this.Col = col;
		}

		[NullableContext(1)]
		public override string ToString()
		{
			return string.Format("Line: {0}, Col: {1}, Idx: {2}", this.Line, this.Col, this.Position);
		}

		public int Position;

		public int Line;

		public int Col;
	}
}
