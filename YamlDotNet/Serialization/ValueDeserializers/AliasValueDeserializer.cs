using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.ValueDeserializers
{
	public sealed class AliasValueDeserializer : IValueDeserializer
	{
		public AliasValueDeserializer(IValueDeserializer innerDeserializer)
		{
			if (innerDeserializer == null)
			{
				throw new ArgumentNullException("innerDeserializer");
			}
			this.innerDeserializer = innerDeserializer;
		}

		public object DeserializeValue(EventReader reader, Type expectedType, SerializerState state, IValueDeserializer nestedObjectDeserializer)
		{
			AnchorAlias anchorAlias = reader.Allow<AnchorAlias>();
			if (anchorAlias == null)
			{
				string text = null;
				NodeEvent nodeEvent = reader.Peek<NodeEvent>();
				if (nodeEvent != null && !string.IsNullOrEmpty(nodeEvent.Anchor))
				{
					text = nodeEvent.Anchor;
				}
				object obj = this.innerDeserializer.DeserializeValue(reader, expectedType, state, nestedObjectDeserializer);
				if (text != null)
				{
					AliasValueDeserializer.AliasState aliasState = state.Get<AliasValueDeserializer.AliasState>();
					AliasValueDeserializer.ValuePromise valuePromise;
					if (!aliasState.TryGetValue(text, out valuePromise))
					{
						aliasState.Add(text, new AliasValueDeserializer.ValuePromise(obj));
					}
					else
					{
						if (valuePromise.HasValue)
						{
							throw new DuplicateAnchorException(nodeEvent.Start, nodeEvent.End, string.Format("Anchor '{0}' already defined", text));
						}
						valuePromise.Value = obj;
					}
				}
				return obj;
			}
			AliasValueDeserializer.AliasState aliasState2 = state.Get<AliasValueDeserializer.AliasState>();
			AliasValueDeserializer.ValuePromise valuePromise2;
			if (!aliasState2.TryGetValue(anchorAlias.Value, out valuePromise2))
			{
				valuePromise2 = new AliasValueDeserializer.ValuePromise(anchorAlias);
				aliasState2.Add(anchorAlias.Value, valuePromise2);
			}
			if (!valuePromise2.HasValue)
			{
				return valuePromise2;
			}
			return valuePromise2.Value;
		}

		private readonly IValueDeserializer innerDeserializer;

		private sealed class AliasState : Dictionary<string, AliasValueDeserializer.ValuePromise>, IPostDeserializationCallback
		{
			public void OnDeserialization()
			{
				foreach (AliasValueDeserializer.ValuePromise valuePromise in base.Values)
				{
					if (!valuePromise.HasValue)
					{
						throw new AnchorNotFoundException(valuePromise.Alias.Start, valuePromise.Alias.End, string.Format("Anchor '{0}' not found", valuePromise.Alias.Value));
					}
				}
			}
		}

		private sealed class ValuePromise : IValuePromise
		{
			public event Action<object> ValueAvailable;

			public bool HasValue { get; private set; }

			public ValuePromise(AnchorAlias alias)
			{
				this.Alias = alias;
			}

			public ValuePromise(object value)
			{
				this.HasValue = true;
				this.value = value;
			}

			public object Value
			{
				get
				{
					if (!this.HasValue)
					{
						throw new InvalidOperationException("Value not set");
					}
					return this.value;
				}
				set
				{
					if (this.HasValue)
					{
						throw new InvalidOperationException("Value already set");
					}
					this.HasValue = true;
					this.value = value;
					if (this.ValueAvailable != null)
					{
						this.ValueAvailable(value);
					}
				}
			}

			private object value;

			public readonly AnchorAlias Alias;
		}
	}
}
