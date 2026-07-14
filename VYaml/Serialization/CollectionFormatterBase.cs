using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class CollectionFormatterBase<[Nullable(2)] TElement, [Nullable(2)] TIntermediate, [Nullable(0)] TCollection> : IYamlFormatter<TCollection>, IYamlFormatter where TCollection : IEnumerable<TElement>
	{
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(2)] TCollection value, YamlSerializationContext context)
		{
			if (value == null)
			{
				emitter.WriteNull();
				return;
			}
			emitter.BeginSequence(SequenceStyle.Block);
			int? count = this.GetCount(value);
			int num = 0;
			if ((count.GetValueOrDefault() > num) & (count != null))
			{
				IYamlFormatter<TElement> formatterWithVerify = context.Resolver.GetFormatterWithVerify<TElement>();
				foreach (TElement telement in value)
				{
					formatterWithVerify.Serialize(ref emitter, telement, context);
				}
			}
			emitter.EndSequence();
		}

		[return: Nullable(2)]
		public TCollection Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return default(TCollection);
			}
			parser.ReadWithVerify(ParseEventType.SequenceStart);
			TIntermediate tintermediate = this.Create(context.Options);
			IYamlFormatter<TElement> formatterWithVerify = context.Resolver.GetFormatterWithVerify<TElement>();
			while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
			{
				TElement telement = context.DeserializeWithAlias<TElement>(formatterWithVerify, ref parser);
				this.Add(tintermediate, telement, context.Options);
			}
			parser.ReadWithVerify(ParseEventType.SequenceEnd);
			return this.Complete(tintermediate);
		}

		protected virtual int? GetCount(TCollection sequence)
		{
			ICollection<TElement> collection = sequence as ICollection<TElement>;
			if (collection != null)
			{
				return new int?(collection.Count);
			}
			IReadOnlyCollection<TElement> readOnlyCollection = sequence as IReadOnlyCollection<TElement>;
			if (readOnlyCollection != null)
			{
				return new int?(readOnlyCollection.Count);
			}
			return null;
		}

		protected abstract TIntermediate Create(YamlSerializerOptions options);

		protected abstract void Add(TIntermediate collection, TElement value, YamlSerializerOptions options);

		protected abstract TCollection Complete(TIntermediate intermediateCollection);
	}
}
