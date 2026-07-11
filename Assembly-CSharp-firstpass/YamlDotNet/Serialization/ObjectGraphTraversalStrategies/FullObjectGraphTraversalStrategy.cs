using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Helpers;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.ObjectGraphTraversalStrategies
{
	public class FullObjectGraphTraversalStrategy : IObjectGraphTraversalStrategy
	{
		public FullObjectGraphTraversalStrategy(ITypeInspector typeDescriptor, ITypeResolver typeResolver, int maxRecursion, INamingConvention namingConvention)
		{
			if (maxRecursion <= 0)
			{
				throw new ArgumentOutOfRangeException("maxRecursion", maxRecursion, "maxRecursion must be greater than 1");
			}
			if (typeDescriptor == null)
			{
				throw new ArgumentNullException("typeDescriptor");
			}
			this.typeDescriptor = typeDescriptor;
			if (typeResolver == null)
			{
				throw new ArgumentNullException("typeResolver");
			}
			this.typeResolver = typeResolver;
			this.maxRecursion = maxRecursion;
			this.namingConvention = namingConvention;
		}

		void IObjectGraphTraversalStrategy.Traverse<TContext>(IObjectDescriptor graph, IObjectGraphVisitor<TContext> visitor, TContext context)
		{
			this.Traverse<TContext>(graph, visitor, 0, context);
		}

		protected virtual void Traverse<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, int currentDepth, TContext context)
		{
			if (++currentDepth > this.maxRecursion)
			{
				throw new InvalidOperationException("Too much recursion when traversing the object graph");
			}
			if (!visitor.Enter(value, context))
			{
				return;
			}
			TypeCode typeCode = value.Type.GetTypeCode();
			switch (typeCode)
			{
			case TypeCode.Empty:
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "TypeCode.{0} is not supported.", typeCode));
			case TypeCode.DBNull:
				visitor.VisitScalar(new ObjectDescriptor(null, typeof(object), typeof(object)), context);
				return;
			case TypeCode.Boolean:
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
			case TypeCode.DateTime:
			case TypeCode.String:
				visitor.VisitScalar(value, context);
				return;
			}
			if (value.Value == null || value.Type == typeof(TimeSpan))
			{
				visitor.VisitScalar(value, context);
				return;
			}
			Type underlyingType = Nullable.GetUnderlyingType(value.Type);
			if (underlyingType != null)
			{
				this.Traverse<TContext>(new ObjectDescriptor(value.Value, underlyingType, value.Type, value.ScalarStyle), visitor, currentDepth, context);
				return;
			}
			this.TraverseObject<TContext>(value, visitor, currentDepth, context);
		}

		protected virtual void TraverseObject<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, int currentDepth, TContext context)
		{
			if (typeof(IDictionary).IsAssignableFrom(value.Type))
			{
				this.TraverseDictionary<TContext>(value, visitor, currentDepth, typeof(object), typeof(object), context);
				return;
			}
			Type implementedGenericInterface = ReflectionUtility.GetImplementedGenericInterface(value.Type, typeof(IDictionary<, >));
			if (implementedGenericInterface != null)
			{
				GenericDictionaryToNonGenericAdapter genericDictionaryToNonGenericAdapter = new GenericDictionaryToNonGenericAdapter(value.Value, implementedGenericInterface);
				Type[] genericArguments = implementedGenericInterface.GetGenericArguments();
				this.TraverseDictionary<TContext>(new ObjectDescriptor(genericDictionaryToNonGenericAdapter, value.Type, value.StaticType, value.ScalarStyle), visitor, currentDepth, genericArguments[0], genericArguments[1], context);
				return;
			}
			if (typeof(IEnumerable).IsAssignableFrom(value.Type))
			{
				this.TraverseList<TContext>(value, visitor, currentDepth, context);
				return;
			}
			this.TraverseProperties<TContext>(value, visitor, currentDepth, context);
		}

		protected virtual void TraverseDictionary<TContext>(IObjectDescriptor dictionary, IObjectGraphVisitor<TContext> visitor, int currentDepth, Type keyType, Type valueType, TContext context)
		{
			visitor.VisitMappingStart(dictionary, keyType, valueType, context);
			bool flag = dictionary.Type.FullName.Equals("System.Dynamic.ExpandoObject");
			foreach (object obj in ((IDictionary)dictionary.Value))
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				string text = (flag ? this.namingConvention.Apply(dictionaryEntry.Key.ToString()) : dictionaryEntry.Key.ToString());
				IObjectDescriptor objectDescriptor = this.GetObjectDescriptor(text, keyType);
				IObjectDescriptor objectDescriptor2 = this.GetObjectDescriptor(dictionaryEntry.Value, valueType);
				if (visitor.EnterMapping(objectDescriptor, objectDescriptor2, context))
				{
					this.Traverse<TContext>(objectDescriptor, visitor, currentDepth, context);
					this.Traverse<TContext>(objectDescriptor2, visitor, currentDepth, context);
				}
			}
			visitor.VisitMappingEnd(dictionary, context);
		}

		private void TraverseList<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, int currentDepth, TContext context)
		{
			Type implementedGenericInterface = ReflectionUtility.GetImplementedGenericInterface(value.Type, typeof(IEnumerable<>));
			Type type = ((implementedGenericInterface != null) ? implementedGenericInterface.GetGenericArguments()[0] : typeof(object));
			visitor.VisitSequenceStart(value, type, context);
			foreach (object obj in ((IEnumerable)value.Value))
			{
				this.Traverse<TContext>(this.GetObjectDescriptor(obj, type), visitor, currentDepth, context);
			}
			visitor.VisitSequenceEnd(value, context);
		}

		protected virtual void TraverseProperties<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, int currentDepth, TContext context)
		{
			visitor.VisitMappingStart(value, typeof(string), typeof(object), context);
			foreach (IPropertyDescriptor propertyDescriptor in this.typeDescriptor.GetProperties(value.Type, value.Value))
			{
				IObjectDescriptor objectDescriptor = propertyDescriptor.Read(value.Value);
				if (visitor.EnterMapping(propertyDescriptor, objectDescriptor, context))
				{
					this.Traverse<TContext>(new ObjectDescriptor(propertyDescriptor.Name, typeof(string), typeof(string)), visitor, currentDepth, context);
					this.Traverse<TContext>(objectDescriptor, visitor, currentDepth, context);
				}
			}
			visitor.VisitMappingEnd(value, context);
		}

		private IObjectDescriptor GetObjectDescriptor(object value, Type staticType)
		{
			return new ObjectDescriptor(value, this.typeResolver.Resolve(staticType, value), staticType);
		}

		private readonly int maxRecursion;

		private readonly ITypeInspector typeDescriptor;

		private readonly ITypeResolver typeResolver;

		private INamingConvention namingConvention;
	}
}
