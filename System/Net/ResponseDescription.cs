using System;
using System.Text;

namespace System.Net
{
	internal class ResponseDescription
	{
		internal bool PositiveIntermediate
		{
			get
			{
				return this.Status >= 100 && this.Status <= 199;
			}
		}

		internal bool PositiveCompletion
		{
			get
			{
				return this.Status >= 200 && this.Status <= 299;
			}
		}

		internal bool TransientFailure
		{
			get
			{
				return this.Status >= 400 && this.Status <= 499;
			}
		}

		internal bool PermanentFailure
		{
			get
			{
				return this.Status >= 500 && this.Status <= 599;
			}
		}

		internal bool InvalidStatusCode
		{
			get
			{
				return this.Status < 100 || this.Status > 599;
			}
		}

		internal const int NoStatus = -1;

		internal bool Multiline;

		internal int Status = -1;

		internal string StatusDescription;

		internal StringBuilder StatusBuffer = new StringBuilder();

		internal string StatusCodeString;
	}
}
