using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace YamlDotNet.Serialization.ObjectGraphTraversalStrategies
{
	public class RoundtripObjectGraphTraversalStrategy : FullObjectGraphTraversalStrategy
	{
		public RoundtripObjectGraphTraversalStrategy(IEnumerable<IYamlTypeConverter> converters, ITypeInspector typeDescriptor, ITypeResolver typeResolver, int maxRecursion)
			: base(typeDescriptor, typeResolver, maxRecursion, null)
		{
			this.converters = converters;
		}

		protected override void TraverseProperties<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, int currentDepth, TContext context)
		{
			if (!value.Type.HasDefaultConstructor() && !this.converters.Any<IYamlTypeConverter>((IYamlTypeConverter c) => c.Accepts(value.Type)))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Type '{0}' cannot be deserialized because it does not have a default constructor or a type converter.", new object[] { value.Type }));
			}
			base.TraverseProperties<TContext>(value, visitor, currentDepth, context);
		}

		private readonly IEnumerable<IYamlTypeConverter> converters;
	}
}
