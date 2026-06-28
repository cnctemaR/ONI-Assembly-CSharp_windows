using System;
using FileHelpers.Dynamic;

namespace FileHelpers.Detection
{
	public sealed class RecordFormatInfo
	{
		public int Confidence
		{
			get
			{
				return this.mConfidence;
			}
		}

		public ClassBuilder ClassBuilder
		{
			get
			{
				return this.mClassBuilder;
			}
		}

		public FixedLengthClassBuilder ClassBuilderAsFixed
		{
			get
			{
				return this.mClassBuilder as FixedLengthClassBuilder;
			}
		}

		public DelimitedClassBuilder ClassBuilderAsDelimited
		{
			get
			{
				return this.mClassBuilder as DelimitedClassBuilder;
			}
		}

		internal int mConfidence;

		internal ClassBuilder mClassBuilder;
	}
}
