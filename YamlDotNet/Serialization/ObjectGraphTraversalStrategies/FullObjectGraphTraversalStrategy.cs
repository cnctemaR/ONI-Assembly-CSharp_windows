using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.ObjectGraphTraversalStrategies
{
	public class FullObjectGraphTraversalStrategy : IObjectGraphTraversalStrategy
	{
		public FullObjectGraphTraversalStrategy(Serializer serializer, ITypeInspector typeDescriptor, ITypeResolver typeResolver, int maxRecursion, INamingConvention namingConvention)
		{
			if (maxRecursion <= 0)
			{
				throw new ArgumentOutOfRangeException("maxRecursion", maxRecursion, "maxRecursion must be greater than 1");
			}
			this.serializer = serializer;
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

		void IObjectGraphTraversalStrategy.Traverse(IObjectDescriptor graph, IObjectGraphVisitor visitor)
		{
			this.Traverse(graph, visitor, 0);
		}

		protected virtual void Traverse(IObjectDescriptor value, IObjectGraphVisitor visitor, int currentDepth)
		{
			if (++currentDepth > this.maxRecursion)
			{
				throw new InvalidOperationException("Too much recursion when traversing the object graph");
			}
			if (!visitor.Enter(value))
			{
				return;
			}
			TypeCode typeCode = value.Type.GetTypeCode();
			switch (typeCode)
			{
			case TypeCode.Empty:
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "TypeCode.{0} is not supported.", new object[] { typeCode }));
			case TypeCode.DBNull:
				visitor.VisitScalar(new ObjectDescriptor(null, typeof(object), typeof(object)));
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
				visitor.VisitScalar(value);
				return;
			}
			if (value.Value == null || value.Type == typeof(TimeSpan))
			{
				visitor.VisitScalar(value);
				return;
			}
			Type underlyingType = Nullable.GetUnderlyingType(value.Type);
			if (underlyingType != null)
			{
				this.Traverse(new ObjectDescriptor(value.Value, underlyingType, value.Type, value.ScalarStyle), visitor, currentDepth);
				return;
			}
			this.TraverseObject(value, visitor, currentDepth);
		}

		protected virtual void TraverseObject(IObjectDescriptor value, IObjectGraphVisitor visitor, int currentDepth)
		{
			if (typeof(IDictionary).IsAssignableFrom(value.Type))
			{
				this.TraverseDictionary(value, visitor, currentDepth);
				return;
			}
			Type implementedGenericInterface = ReflectionUtility.GetImplementedGenericInterface(value.Type, typeof(IDictionary<, >));
			if (implementedGenericInterface != null)
			{
				this.TraverseGenericDictionary(value, implementedGenericInterface, visitor, currentDepth);
				return;
			}
			if (typeof(IEnumerable).IsAssignableFrom(value.Type))
			{
				this.TraverseList(value, visitor, currentDepth);
				return;
			}
			this.TraverseProperties(value, visitor, currentDepth);
		}

		protected virtual void TraverseDictionary(IObjectDescriptor dictionary, IObjectGraphVisitor visitor, int currentDepth)
		{
			visitor.VisitMappingStart(dictionary, typeof(object), typeof(object));
			foreach (object obj in ((IDictionary)dictionary.Value))
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				IObjectDescriptor objectDescriptor = this.GetObjectDescriptor(dictionaryEntry.Key, typeof(object));
				IObjectDescriptor objectDescriptor2 = this.GetObjectDescriptor(dictionaryEntry.Value, typeof(object));
				if (visitor.EnterMapping(objectDescriptor, objectDescriptor2))
				{
					this.Traverse(objectDescriptor, visitor, currentDepth);
					this.Traverse(objectDescriptor2, visitor, currentDepth);
				}
			}
			visitor.VisitMappingEnd(dictionary);
		}

		private void TraverseGenericDictionary(IObjectDescriptor dictionary, Type dictionaryType, IObjectGraphVisitor visitor, int currentDepth)
		{
			Type[] genericArguments = dictionaryType.GetGenericArguments();
			visitor.VisitMappingStart(dictionary, genericArguments[0], genericArguments[1]);
			FullObjectGraphTraversalStrategy.traverseGenericDictionaryHelper.Invoke(genericArguments, this, new object[]
			{
				dictionary.Value,
				visitor,
				currentDepth,
				this.namingConvention ?? new NullNamingConvention()
			});
			visitor.VisitMappingEnd(dictionary);
		}

		private void TraverseGenericDictionaryHelper<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IObjectGraphVisitor visitor, int currentDepth, INamingConvention namingConvention)
		{
			bool flag = dictionary.GetType().FullName.Equals("System.Dynamic.ExpandoObject");
			foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
			{
				string text;
				if (!flag)
				{
					TKey key = keyValuePair.Key;
					text = key.ToString();
				}
				else
				{
					TKey key2 = keyValuePair.Key;
					text = namingConvention.Apply(key2.ToString());
				}
				string text2 = text;
				IObjectDescriptor objectDescriptor = this.GetObjectDescriptor(text2, typeof(TKey));
				IObjectDescriptor objectDescriptor2 = this.GetObjectDescriptor(keyValuePair.Value, typeof(TValue));
				if (visitor.EnterMapping(objectDescriptor, objectDescriptor2))
				{
					this.Traverse(objectDescriptor, visitor, currentDepth);
					this.Traverse(objectDescriptor2, visitor, currentDepth);
				}
			}
		}

		private void TraverseList(IObjectDescriptor value, IObjectGraphVisitor visitor, int currentDepth)
		{
			Type implementedGenericInterface = ReflectionUtility.GetImplementedGenericInterface(value.Type, typeof(IEnumerable<>));
			Type type = ((implementedGenericInterface != null) ? implementedGenericInterface.GetGenericArguments()[0] : typeof(object));
			visitor.VisitSequenceStart(value, type);
			foreach (object obj in ((IEnumerable)value.Value))
			{
				this.Traverse(this.GetObjectDescriptor(obj, type), visitor, currentDepth);
			}
			visitor.VisitSequenceEnd(value);
		}

		protected virtual void TraverseProperties(IObjectDescriptor value, IObjectGraphVisitor visitor, int currentDepth)
		{
			visitor.VisitMappingStart(value, typeof(string), typeof(object));
			foreach (IPropertyDescriptor propertyDescriptor in this.typeDescriptor.GetProperties(value.Type, value.Value))
			{
				IObjectDescriptor objectDescriptor = propertyDescriptor.Read(value.Value);
				if (visitor.EnterMapping(propertyDescriptor, objectDescriptor))
				{
					this.Traverse(new ObjectDescriptor(propertyDescriptor.Name, typeof(string), typeof(string)), visitor, currentDepth);
					this.Traverse(objectDescriptor, visitor, currentDepth);
				}
			}
			visitor.VisitMappingEnd(value);
		}

		private IObjectDescriptor GetObjectDescriptor(object value, Type staticType)
		{
			return new ObjectDescriptor(value, this.typeResolver.Resolve(staticType, value), staticType);
		}

		protected readonly Serializer serializer;

		private readonly int maxRecursion;

		private readonly ITypeInspector typeDescriptor;

		private readonly ITypeResolver typeResolver;

		private INamingConvention namingConvention;

		private static readonly GenericInstanceMethod<FullObjectGraphTraversalStrategy> traverseGenericDictionaryHelper = new GenericInstanceMethod<FullObjectGraphTraversalStrategy>((FullObjectGraphTraversalStrategy s) => s.TraverseGenericDictionaryHelper<int, int>(null, null, 0, null));
	}
}
