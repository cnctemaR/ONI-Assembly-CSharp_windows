using System;
using System.Collections.Generic;

namespace Unity.Properties.Internal
{
	internal interface IAttributes
	{
		List<Attribute> Attributes { get; set; }

		void AddAttribute(Attribute attribute);

		void AddAttributes(IEnumerable<Attribute> attributes);

		AttributesScope CreateAttributesScope(IAttributes attributes);
	}
}
