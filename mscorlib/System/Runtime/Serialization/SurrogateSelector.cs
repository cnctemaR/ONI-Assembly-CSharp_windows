using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public class SurrogateSelector : ISurrogateSelector
	{
		public virtual void AddSurrogate(Type type, StreamingContext context, ISerializationSurrogate surrogate)
		{
			if (type == null || surrogate == null)
			{
				throw new ArgumentNullException("Null reference.");
			}
			string text = type.FullName + "#" + context.ToString();
			if (this.Surrogates.ContainsKey(text))
			{
				throw new ArgumentException("A surrogate for " + type.FullName + " already exists.");
			}
			this.Surrogates.Add(text, surrogate);
		}

		public virtual void ChainSelector(ISurrogateSelector selector)
		{
			if (selector == null)
			{
				throw new ArgumentNullException("Selector is null.");
			}
			if (this.nextSelector != null)
			{
				selector.ChainSelector(this.nextSelector);
			}
			this.nextSelector = selector;
		}

		public virtual ISurrogateSelector GetNextSelector()
		{
			return this.nextSelector;
		}

		public virtual ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type is null.");
			}
			string text = type.FullName + "#" + context.ToString();
			ISerializationSurrogate serializationSurrogate = (ISerializationSurrogate)this.Surrogates[text];
			if (serializationSurrogate != null)
			{
				selector = this;
				return serializationSurrogate;
			}
			if (this.nextSelector != null)
			{
				return this.nextSelector.GetSurrogate(type, context, out selector);
			}
			selector = null;
			return null;
		}

		public virtual void RemoveSurrogate(Type type, StreamingContext context)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type is null.");
			}
			string text = type.FullName + "#" + context.ToString();
			this.Surrogates.Remove(text);
		}

		private Hashtable Surrogates = new Hashtable();

		private ISurrogateSelector nextSelector;
	}
}
