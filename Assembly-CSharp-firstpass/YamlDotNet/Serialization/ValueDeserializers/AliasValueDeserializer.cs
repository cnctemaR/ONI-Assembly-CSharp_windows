using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		public object DeserializeValue(IParser parser, Type expectedType, SerializerState state, IValueDeserializer nestedObjectDeserializer)
		{
			AnchorAlias anchorAlias = parser.Allow<AnchorAlias>();
			if (anchorAlias != null)
			{
				AliasValueDeserializer.AliasState aliasState = state.Get<AliasValueDeserializer.AliasState>();
				AliasValueDeserializer.ValuePromise valuePromise;
				if (!aliasState.TryGetValue(anchorAlias.Value, out valuePromise))
				{
					valuePromise = new AliasValueDeserializer.ValuePromise(anchorAlias);
					aliasState.Add(anchorAlias.Value, valuePromise);
				}
				return (!valuePromise.HasValue) ? valuePromise : valuePromise.Value;
			}
			string text = null;
			NodeEvent nodeEvent = parser.Peek<NodeEvent>();
			if (nodeEvent != null && !string.IsNullOrEmpty(nodeEvent.Anchor))
			{
				text = nodeEvent.Anchor;
			}
			object obj = this.innerDeserializer.DeserializeValue(parser, expectedType, state, nestedObjectDeserializer);
			if (text != null)
			{
				AliasValueDeserializer.AliasState aliasState2 = state.Get<AliasValueDeserializer.AliasState>();
				AliasValueDeserializer.ValuePromise valuePromise2;
				if (!aliasState2.TryGetValue(text, out valuePromise2))
				{
					aliasState2.Add(text, new AliasValueDeserializer.ValuePromise(obj));
				}
				else if (!valuePromise2.HasValue)
				{
					valuePromise2.Value = obj;
				}
				else
				{
					aliasState2[text] = new AliasValueDeserializer.ValuePromise(obj);
				}
			}
			return obj;
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
			public ValuePromise(AnchorAlias alias)
			{
				this.Alias = alias;
			}

			public ValuePromise(object value)
			{
				this.HasValue = true;
				this.value = value;
			}

			[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public event Action<object> ValueAvailable;

			public bool HasValue { get; private set; }

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
