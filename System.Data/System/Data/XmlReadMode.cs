using System;

namespace System.Data
{
	public enum XmlReadMode
	{
		Auto,
		ReadSchema,
		IgnoreSchema,
		InferSchema,
		DiffGram,
		InferTypedSchema = 6,
		Fragment = 5
	}
}
