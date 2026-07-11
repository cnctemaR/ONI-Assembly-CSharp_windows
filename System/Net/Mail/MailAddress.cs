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
			: this(address, displayName, Encoding.UTF8)
		{
		}

		[MonoTODO("We don't do anything with displayNameEncoding")]
		public MailAddress(string address, string displayName, Encoding displayNameEncoding)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.Length == 0)
			{
				throw new ArgumentException("address");
			}
			if (displayName != null)
			{
				this.displayName = displayName.Trim();
			}
			this.ParseAddress(address);
		}

		private void ParseAddress(string address)
		{
			address = address.Trim();
			int num = address.IndexOf('"');
			if (num != -1)
			{
				if (num != 0 || address.Length == 1)
				{
					throw MailAddress.CreateFormatException();
				}
				int num2 = address.LastIndexOf('"');
				if (num2 == num)
				{
					throw MailAddress.CreateFormatException();
				}
				if (this.displayName == null)
				{
					this.displayName = address.Substring(num + 1, num2 - num - 1).Trim();
				}
				address = address.Substring(num2 + 1).Trim();
			}
			num = address.IndexOf('<');
			if (num >= 0)
			{
				if (this.displayName == null)
				{
					this.displayName = address.Substring(0, num).Trim();
				}
				if (address.Length - 1 == num)
				{
					throw MailAddress.CreateFormatException();
				}
				int num3 = address.IndexOf('>', num + 1);
				if (num3 == -1)
				{
					throw MailAddress.CreateFormatException();
				}
				address = address.Substring(num + 1, num3 - num - 1).Trim();
			}
			this.address = address;
			num = address.IndexOf('@');
			if (num <= 0)
			{
				throw MailAddress.CreateFormatException();
			}
			if (num != address.LastIndexOf('@'))
			{
				throw MailAddress.CreateFormatException();
			}
			this.user = address.Substring(0, num).Trim();
			if (this.user.Length == 0)
			{
				throw MailAddress.CreateFormatException();
			}
			this.host = address.Substring(num + 1).Trim();
			if (this.host.Length == 0)
			{
				throw MailAddress.CreateFormatException();
			}
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
				return this.host;
			}
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		public override bool Equals(object value)
		{
			return value != null && string.Compare(this.ToString(), value.ToString(), StringComparison.OrdinalIgnoreCase) == 0;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			if (this.to_string != null)
			{
				return this.to_string;
			}
			if (!string.IsNullOrEmpty(this.displayName))
			{
				this.to_string = string.Format("\"{0}\" <{1}>", this.DisplayName, this.Address);
			}
			else
			{
				this.to_string = this.address;
			}
			return this.to_string;
		}

		private static FormatException CreateFormatException()
		{
			return new FormatException("The specified string is not in the form required for an e-mail address.");
		}

		private string address;

		private string displayName;

		private string host;

		private string user;

		private string to_string;
	}
}
