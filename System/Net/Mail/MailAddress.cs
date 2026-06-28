using System;
using System.Text;

namespace System.Net.Mail
{
	public class MailAddress
	{
		public MailAddress(string address)
			: this(address, null)
		{
		}

		public MailAddress(string address, string displayName)
			: this(address, displayName, Encoding.Default)
		{
		}

		public MailAddress(string address, string displayName, Encoding displayNameEncoding)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			int num = address.IndexOf('"');
			if (num == 0)
			{
				int num2 = address.IndexOf('"', num + 1);
				if (num2 == -1)
				{
					throw MailAddress.CreateFormatException();
				}
				this.displayName = address.Substring(num + 1, num2 - 1).Trim();
				address = address.Substring(num2 + 1);
			}
			int num3 = address.IndexOf('<');
			if (num3 != -1)
			{
				if (num3 + 1 >= address.Length)
				{
					throw MailAddress.CreateFormatException();
				}
				int num4 = address.IndexOf('>', num3 + 1);
				if (num4 == -1)
				{
					throw MailAddress.CreateFormatException();
				}
				if (this.displayName == null)
				{
					this.displayName = address.Substring(0, num3).Trim();
				}
				address = address.Substring(++num3, num4 - num3);
			}
			if (displayName != null)
			{
				this.displayName = displayName.Trim();
			}
			this.address = address.Trim();
		}

		public string Address
		{
			get
			{
				return this.address;
			}
		}

		public string DisplayName
		{
			get
			{
				if (this.displayName == null)
				{
					return string.Empty;
				}
				return this.displayName;
			}
		}

		public string Host
		{
			get
			{
				return this.Address.Substring(this.address.IndexOf("@") + 1);
			}
		}

		public string User
		{
			get
			{
				return this.Address.Substring(0, this.address.IndexOf("@"));
			}
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as MailAddress);
		}

		private bool Equals(MailAddress other)
		{
			return other != null && this.Address == other.Address;
		}

		public override int GetHashCode()
		{
			return this.address.GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.DisplayName != null && this.DisplayName.Length > 0)
			{
				stringBuilder.Append("\"");
				stringBuilder.Append(this.DisplayName);
				stringBuilder.Append("\"");
				stringBuilder.Append(" ");
				stringBuilder.Append("<");
				stringBuilder.Append(this.Address);
				stringBuilder.Append(">");
			}
			else
			{
				stringBuilder.Append(this.Address);
			}
			return stringBuilder.ToString();
		}

		private static FormatException CreateFormatException()
		{
			return new FormatException("The specified string is not in the form required for an e-mail address.");
		}

		private string address;

		private string displayName;
	}
}
