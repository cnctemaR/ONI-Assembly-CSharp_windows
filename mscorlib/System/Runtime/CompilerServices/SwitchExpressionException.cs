using System;
using System.Runtime.Serialization;

namespace System.Runtime.CompilerServices
{
	[Serializable]
	public sealed class SwitchExpressionException : InvalidOperationException
	{
		public SwitchExpressionException()
			: base("Non-exhaustive switch expression failed to match its input.")
		{
		}

		public SwitchExpressionException(Exception innerException)
			: base("Non-exhaustive switch expression failed to match its input.", innerException)
		{
		}

		public SwitchExpressionException(object unmatchedValue)
			: this()
		{
			this.UnmatchedValue = unmatchedValue;
		}

		private SwitchExpressionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.UnmatchedValue = info.GetValue("UnmatchedValue", typeof(object));
		}

		public SwitchExpressionException(string message)
			: base(message)
		{
		}

		public SwitchExpressionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public object UnmatchedValue { get; }

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("UnmatchedValue", this.UnmatchedValue, typeof(object));
		}

		public override string Message
		{
			get
			{
				if (this.UnmatchedValue == null)
				{
					return base.Message;
				}
				string text = SR.Format("Unmatched value was {0}.", this.UnmatchedValue.ToString());
				return base.Message + Environment.NewLine + text;
			}
		}
	}
}
