using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class FieldAlignAttribute : Attribute
	{
		public AlignMode Align { get; private set; }

		public char AlignChar { get; private set; }

		public FieldAlignAttribute(AlignMode align)
			: this(align, ' ')
		{
		}

		public FieldAlignAttribute(AlignMode align, char alignChar)
		{
			this.Align = align;
			this.AlignChar = alignChar;
		}
	}
}
