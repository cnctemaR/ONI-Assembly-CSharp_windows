using System;
using System.Collections.Generic;

namespace FileHelpers.Options
{
	public sealed class FieldBaseCollection : List<FieldBase>
	{
		internal FieldBaseCollection(FieldBase[] fields)
			: base(fields)
		{
		}
	}
}
