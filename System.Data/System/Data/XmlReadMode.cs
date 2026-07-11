using System;

namespace System.Data
{
	public enum XmlReadMode
	{
		Auto,
		DiffGram = 4,
		Fragment,
		IgnoreSchema = 2,
		InferSchema,
		InferTypedSchema = 6,
		ReadSchema = 1
	}
}
