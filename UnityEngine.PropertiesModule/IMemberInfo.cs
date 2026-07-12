using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	internal interface IMemberInfo
	{
		string Name { get; }

		bool IsReadOnly { get; }

		Type ValueType { get; }

		object GetValue(object obj);

		void SetValue(object obj, object value);

		IEnumerable<Attribute> GetCustomAttributes();
	}
}
