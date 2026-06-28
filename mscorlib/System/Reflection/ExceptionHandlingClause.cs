using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	public sealed class ExceptionHandlingClause
	{
		internal ExceptionHandlingClause()
		{
		}

		public Type CatchType
		{
			get
			{
				return this.catch_type;
			}
		}

		public int FilterOffset
		{
			get
			{
				return this.filter_offset;
			}
		}

		public ExceptionHandlingClauseOptions Flags
		{
			get
			{
				return this.flags;
			}
		}

		public int HandlerLength
		{
			get
			{
				return this.handler_length;
			}
		}

		public int HandlerOffset
		{
			get
			{
				return this.handler_offset;
			}
		}

		public int TryLength
		{
			get
			{
				return this.try_length;
			}
		}

		public int TryOffset
		{
			get
			{
				return this.try_offset;
			}
		}

		public override string ToString()
		{
			string text = string.Format("Flags={0}, TryOffset={1}, TryLength={2}, HandlerOffset={3}, HandlerLength={4}", new object[] { this.flags, this.try_offset, this.try_length, this.handler_offset, this.handler_length });
			if (this.catch_type != null)
			{
				text = string.Format("{0}, CatchType={1}", text, this.catch_type);
			}
			if (this.flags == ExceptionHandlingClauseOptions.Filter)
			{
				text = string.Format(CultureInfo.InvariantCulture, "{0}, FilterOffset={1}", new object[] { text, this.filter_offset });
			}
			return text;
		}

		internal Type catch_type;

		internal int filter_offset;

		internal ExceptionHandlingClauseOptions flags;

		internal int try_offset;

		internal int try_length;

		internal int handler_offset;

		internal int handler_length;
	}
}
