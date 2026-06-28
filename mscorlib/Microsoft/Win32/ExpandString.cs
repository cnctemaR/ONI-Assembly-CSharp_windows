using System;
using System.Text;

namespace Microsoft.Win32
{
	internal class ExpandString
	{
		public ExpandString(string s)
		{
			this.value = s;
		}

		public override string ToString()
		{
			return this.value;
		}

		public string Expand()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.value.Length; i++)
			{
				if (this.value[i] == '%')
				{
					int j;
					for (j = i + 1; j < this.value.Length; j++)
					{
						if (this.value[j] == '%')
						{
							string text = this.value.Substring(i + 1, j - i - 1);
							stringBuilder.Append(Environment.GetEnvironmentVariable(text));
							i += j;
							break;
						}
					}
					if (j == this.value.Length)
					{
						stringBuilder.Append('%');
					}
				}
				else
				{
					stringBuilder.Append(this.value[i]);
				}
			}
			return stringBuilder.ToString();
		}

		private string value;
	}
}
