using System;
using System.Collections.Generic;
using Unity;

namespace System.Runtime.Serialization
{
	public sealed class SafeSerializationEventArgs : EventArgs
	{
		internal SafeSerializationEventArgs(StreamingContext streamingContext)
		{
			this.m_serializedStates = new List<object>();
			base..ctor();
			this.m_streamingContext = streamingContext;
		}

		public void AddSerializedState(ISafeSerializationData serializedState)
		{
			if (serializedState == null)
			{
				throw new ArgumentNullException("serializedState");
			}
			if (!serializedState.GetType().IsSerializable)
			{
				throw new ArgumentException(Environment.GetResourceString("Type '{0}' in Assembly '{1}' is not marked as serializable.", new object[]
				{
					serializedState.GetType(),
					serializedState.GetType().Assembly.FullName
				}));
			}
			this.m_serializedStates.Add(serializedState);
		}

		internal IList<object> SerializedStates
		{
			get
			{
				return this.m_serializedStates;
			}
		}

		public StreamingContext StreamingContext
		{
			get
			{
				return this.m_streamingContext;
			}
		}

		internal SafeSerializationEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private StreamingContext m_streamingContext;

		private List<object> m_serializedStates;
	}
}
