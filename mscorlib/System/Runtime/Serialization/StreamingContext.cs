using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	[Serializable]
	public struct StreamingContext
	{
		public StreamingContext(StreamingContextStates state)
		{
			this.state = state;
			this.additional = null;
		}

		public StreamingContext(StreamingContextStates state, object additional)
		{
			this.state = state;
			this.additional = additional;
		}

		public object Context
		{
			get
			{
				return this.additional;
			}
		}

		public StreamingContextStates State
		{
			get
			{
				return this.state;
			}
		}

		public override bool Equals(object obj)
		{
			if (!(obj is StreamingContext))
			{
				return false;
			}
			StreamingContext streamingContext = (StreamingContext)obj;
			return streamingContext.state == this.state && streamingContext.additional == this.additional;
		}

		public override int GetHashCode()
		{
			return (int)this.state;
		}

		private StreamingContextStates state;

		private object additional;
	}
}
