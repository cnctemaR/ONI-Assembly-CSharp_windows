using System;
using System.Text;

namespace System.Data.Odbc
{
	internal sealed class CStringTokenizer
	{
		internal CStringTokenizer(string text, char quote, char escape)
		{
			this._token = new StringBuilder();
			this._quote = quote;
			this._escape = escape;
			this._sqlstatement = text;
			if (text != null)
			{
				int num = text.IndexOf('\0');
				this._len = ((0 > num) ? text.Length : num);
				return;
			}
			this._len = 0;
		}

		internal int CurrentPosition
		{
			get
			{
				return this._idx;
			}
		}

		internal string NextToken()
		{
			if (this._token.Length != 0)
			{
				this._idx += this._token.Length;
				this._token.Remove(0, this._token.Length);
			}
			while (this._idx < this._len && char.IsWhiteSpace(this._sqlstatement[this._idx]))
			{
				this._idx++;
			}
			if (this._idx == this._len)
			{
				return string.Empty;
			}
			int i = this._idx;
			bool flag = false;
			while (!flag && i < this._len)
			{
				if (this.IsValidNameChar(this._sqlstatement[i]))
				{
					while (i < this._len)
					{
						if (!this.IsValidNameChar(this._sqlstatement[i]))
						{
							break;
						}
						this._token.Append(this._sqlstatement[i]);
						i++;
					}
				}
				else
				{
					char c = this._sqlstatement[i];
					if (c == '[')
					{
						i = this.GetTokenFromBracket(i);
					}
					else
					{
						if (' ' == this._quote || c != this._quote)
						{
							if (!char.IsWhiteSpace(c))
							{
								if (c == ',')
								{
									if (i == this._idx)
									{
										this._token.Append(c);
									}
								}
								else
								{
									this._token.Append(c);
								}
							}
							break;
						}
						i = this.GetTokenFromQuote(i);
					}
				}
			}
			if (this._token.Length <= 0)
			{
				return string.Empty;
			}
			return this._token.ToString();
		}

		private int GetTokenFromBracket(int curidx)
		{
			while (curidx < this._len)
			{
				this._token.Append(this._sqlstatement[curidx]);
				curidx++;
				if (this._sqlstatement[curidx - 1] == ']')
				{
					break;
				}
			}
			return curidx;
		}

		private int GetTokenFromQuote(int curidx)
		{
			int i;
			for (i = curidx; i < this._len; i++)
			{
				this._token.Append(this._sqlstatement[i]);
				if (this._sqlstatement[i] == this._quote && i > curidx && this._sqlstatement[i - 1] != this._escape && i + 1 < this._len && this._sqlstatement[i + 1] != this._quote)
				{
					return i + 1;
				}
			}
			return i;
		}

		private bool IsValidNameChar(char ch)
		{
			return char.IsLetterOrDigit(ch) || ch == '_' || ch == '-' || ch == '.' || ch == '$' || ch == '#' || ch == '@' || ch == '~' || ch == '`' || ch == '%' || ch == '^' || ch == '&' || ch == '|';
		}

		internal int FindTokenIndex(string tokenString)
		{
			string text;
			do
			{
				text = this.NextToken();
				if (this._idx == this._len || string.IsNullOrEmpty(text))
				{
					return -1;
				}
			}
			while (string.Compare(tokenString, text, StringComparison.OrdinalIgnoreCase) != 0);
			return this._idx;
		}

		internal bool StartsWith(string tokenString)
		{
			int num = 0;
			while (num < this._len && char.IsWhiteSpace(this._sqlstatement[num]))
			{
				num++;
			}
			if (this._len - num < tokenString.Length)
			{
				return false;
			}
			if (string.Compare(this._sqlstatement, num, tokenString, 0, tokenString.Length, StringComparison.OrdinalIgnoreCase) == 0)
			{
				this._idx = 0;
				this.NextToken();
				return true;
			}
			return false;
		}

		private readonly StringBuilder _token;

		private readonly string _sqlstatement;

		private readonly char _quote;

		private readonly char _escape;

		private int _len;

		private int _idx;
	}
}
