using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public class SurrogateSelector : ISurrogateSelector
	{
		public SurrogateSelector()
		{
			this.m_surrogates = new SurrogateHashtable(32);
		}

		public virtual void AddSurrogate(Type type, StreamingContext context, ISerializationSurrogate surrogate)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (surrogate == null)
			{
				throw new ArgumentNullException("surrogate");
			}
			SurrogateKey surrogateKey = new SurrogateKey(type, context);
			this.m_surrogates.Add(surrogateKey, surrogate);
		}

		[SecurityCritical]
		private static bool HasCycle(ISurrogateSelector selector)
		{
			ISurrogateSelector surrogateSelector = selector;
			ISurrogateSelector surrogateSelector2 = selector;
			while (surrogateSelector != null)
			{
				surrogateSelector = surrogateSelector.GetNextSelector();
				if (surrogateSelector == null)
				{
					return true;
				}
				if (surrogateSelector == surrogateSelector2)
				{
					return false;
				}
				surrogateSelector = surrogateSelector.GetNextSelector();
				surrogateSelector2 = surrogateSelector2.GetNextSelector();
				if (surrogateSelector == surrogateSelector2)
				{
					return false;
				}
			}
			return true;
		}

		[SecurityCritical]
		public virtual void ChainSelector(ISurrogateSelector selector)
		{
			if (selector == null)
			{
				throw new ArgumentNullException("selector");
			}
			if (selector == this)
			{
				throw new SerializationException(Environment.GetResourceString("Selector is already on the list of checked selectors."));
			}
			if (!SurrogateSelector.HasCycle(selector))
			{
				throw new ArgumentException(Environment.GetResourceString("Selector contained a cycle."), "selector");
			}
			ISurrogateSelector surrogateSelector = selector.GetNextSelector();
			ISurrogateSelector surrogateSelector2 = selector;
			while (surrogateSelector != null && surrogateSelector != this)
			{
				surrogateSelector2 = surrogateSelector;
				surrogateSelector = surrogateSelector.GetNextSelector();
			}
			if (surrogateSelector == this)
			{
				throw new ArgumentException(Environment.GetResourceString("Adding selector will introduce a cycle."), "selector");
			}
			surrogateSelector = selector;
			ISurrogateSelector surrogateSelector3 = selector;
			while (surrogateSelector != null)
			{
				if (surrogateSelector == surrogateSelector2)
				{
					surrogateSelector = this.GetNextSelector();
				}
				else
				{
					surrogateSelector = surrogateSelector.GetNextSelector();
				}
				if (surrogateSelector == null)
				{
					break;
				}
				if (surrogateSelector == surrogateSelector3)
				{
					throw new ArgumentException(Environment.GetResourceString("Adding selector will introduce a cycle."), "selector");
				}
				if (surrogateSelector == surrogateSelector2)
				{
					surrogateSelector = this.GetNextSelector();
				}
				else
				{
					surrogateSelector = surrogateSelector.GetNextSelector();
				}
				if (surrogateSelector3 == surrogateSelector2)
				{
					surrogateSelector3 = this.GetNextSelector();
				}
				else
				{
					surrogateSelector3 = surrogateSelector3.GetNextSelector();
				}
				if (surrogateSelector == surrogateSelector3)
				{
					throw new ArgumentException(Environment.GetResourceString("Adding selector will introduce a cycle."), "selector");
				}
			}
			ISurrogateSelector nextSelector = this.m_nextSelector;
			this.m_nextSelector = selector;
			if (nextSelector != null)
			{
				surrogateSelector2.ChainSelector(nextSelector);
			}
		}

		[SecurityCritical]
		public virtual ISurrogateSelector GetNextSelector()
		{
			return this.m_nextSelector;
		}

		[SecurityCritical]
		public virtual ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			selector = this;
			SurrogateKey surrogateKey = new SurrogateKey(type, context);
			ISerializationSurrogate serializationSurrogate = (ISerializationSurrogate)this.m_surrogates[surrogateKey];
			if (serializationSurrogate != null)
			{
				return serializationSurrogate;
			}
			if (this.m_nextSelector != null)
			{
				return this.m_nextSelector.GetSurrogate(type, context, out selector);
			}
			return null;
		}

		public virtual void RemoveSurrogate(Type type, StreamingContext context)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			SurrogateKey surrogateKey = new SurrogateKey(type, context);
			this.m_surrogates.Remove(surrogateKey);
		}

		internal SurrogateHashtable m_surrogates;

		internal ISurrogateSelector m_nextSelector;
	}
}
