using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System.Net.Http.Headers
{
	public class CacheControlHeaderValue : ICloneable
	{
		public ICollection<NameValueHeaderValue> Extensions
		{
			get
			{
				List<NameValueHeaderValue> list;
				if ((list = this.extensions) == null)
				{
					list = (this.extensions = new List<NameValueHeaderValue>());
				}
				return list;
			}
		}

		public TimeSpan? MaxAge { get; set; }

		public bool MaxStale { get; set; }

		public TimeSpan? MaxStaleLimit { get; set; }

		public TimeSpan? MinFresh { get; set; }

		public bool MustRevalidate { get; set; }

		public bool NoCache { get; set; }

		public ICollection<string> NoCacheHeaders
		{
			get
			{
				List<string> list;
				if ((list = this.no_cache_headers) == null)
				{
					list = (this.no_cache_headers = new List<string>());
				}
				return list;
			}
		}

		public bool NoStore { get; set; }

		public bool NoTransform { get; set; }

		public bool OnlyIfCached { get; set; }

		public bool Private { get; set; }

		public ICollection<string> PrivateHeaders
		{
			get
			{
				List<string> list;
				if ((list = this.private_headers) == null)
				{
					list = (this.private_headers = new List<string>());
				}
				return list;
			}
		}

		public bool ProxyRevalidate { get; set; }

		public bool Public { get; set; }

		public TimeSpan? SharedMaxAge { get; set; }

		object ICloneable.Clone()
		{
			CacheControlHeaderValue cacheControlHeaderValue = (CacheControlHeaderValue)base.MemberwiseClone();
			if (this.extensions != null)
			{
				cacheControlHeaderValue.extensions = new List<NameValueHeaderValue>();
				foreach (NameValueHeaderValue nameValueHeaderValue in this.extensions)
				{
					cacheControlHeaderValue.extensions.Add(nameValueHeaderValue);
				}
			}
			if (this.no_cache_headers != null)
			{
				cacheControlHeaderValue.no_cache_headers = new List<string>();
				foreach (string text in this.no_cache_headers)
				{
					cacheControlHeaderValue.no_cache_headers.Add(text);
				}
			}
			if (this.private_headers != null)
			{
				cacheControlHeaderValue.private_headers = new List<string>();
				foreach (string text2 in this.private_headers)
				{
					cacheControlHeaderValue.private_headers.Add(text2);
				}
			}
			return cacheControlHeaderValue;
		}

		public override bool Equals(object obj)
		{
			CacheControlHeaderValue cacheControlHeaderValue = obj as CacheControlHeaderValue;
			return cacheControlHeaderValue != null && (!(this.MaxAge != cacheControlHeaderValue.MaxAge) && this.MaxStale == cacheControlHeaderValue.MaxStale) && !(this.MaxStaleLimit != cacheControlHeaderValue.MaxStaleLimit) && (!(this.MinFresh != cacheControlHeaderValue.MinFresh) && this.MustRevalidate == cacheControlHeaderValue.MustRevalidate && this.NoCache == cacheControlHeaderValue.NoCache && this.NoStore == cacheControlHeaderValue.NoStore && this.NoTransform == cacheControlHeaderValue.NoTransform && this.OnlyIfCached == cacheControlHeaderValue.OnlyIfCached && this.Private == cacheControlHeaderValue.Private && this.ProxyRevalidate == cacheControlHeaderValue.ProxyRevalidate && this.Public == cacheControlHeaderValue.Public) && !(this.SharedMaxAge != cacheControlHeaderValue.SharedMaxAge) && (this.extensions.SequenceEqual<NameValueHeaderValue>(cacheControlHeaderValue.extensions) && this.no_cache_headers.SequenceEqual<string>(cacheControlHeaderValue.no_cache_headers)) && this.private_headers.SequenceEqual<string>(cacheControlHeaderValue.private_headers);
		}

		public override int GetHashCode()
		{
			return (((((((((((((((29 * 29 + HashCodeCalculator.Calculate<NameValueHeaderValue>(this.extensions)) * 29 + this.MaxAge.GetHashCode()) * 29 + this.MaxStale.GetHashCode()) * 29 + this.MaxStaleLimit.GetHashCode()) * 29 + this.MinFresh.GetHashCode()) * 29 + this.MustRevalidate.GetHashCode()) * 29 + HashCodeCalculator.Calculate<string>(this.no_cache_headers)) * 29 + this.NoCache.GetHashCode()) * 29 + this.NoStore.GetHashCode()) * 29 + this.NoTransform.GetHashCode()) * 29 + this.OnlyIfCached.GetHashCode()) * 29 + this.Private.GetHashCode()) * 29 + HashCodeCalculator.Calculate<string>(this.private_headers)) * 29 + this.ProxyRevalidate.GetHashCode()) * 29 + this.Public.GetHashCode()) * 29 + this.SharedMaxAge.GetHashCode();
		}

		public static CacheControlHeaderValue Parse(string input)
		{
			CacheControlHeaderValue cacheControlHeaderValue;
			if (CacheControlHeaderValue.TryParse(input, out cacheControlHeaderValue))
			{
				return cacheControlHeaderValue;
			}
			throw new FormatException(input);
		}

		public static bool TryParse(string input, out CacheControlHeaderValue parsedValue)
		{
			parsedValue = null;
			if (input == null)
			{
				return true;
			}
			CacheControlHeaderValue cacheControlHeaderValue = new CacheControlHeaderValue();
			Lexer lexer = new Lexer(input);
			Token token;
			for (;;)
			{
				token = lexer.Scan(false);
				if (token != Token.Type.Token)
				{
					break;
				}
				string stringValue = lexer.GetStringValue(token);
				bool flag = false;
				uint num = global::<PrivateImplementationDetails>.ComputeStringHash(stringValue);
				if (num <= 1922561311U)
				{
					TimeSpan? timeSpan;
					if (num <= 719568158U)
					{
						if (num != 129047354U)
						{
							if (num != 412259456U)
							{
								if (num != 719568158U)
								{
									goto IL_03B1;
								}
								if (!(stringValue == "no-store"))
								{
									goto IL_03B1;
								}
								cacheControlHeaderValue.NoStore = true;
								goto IL_040A;
							}
							else if (!(stringValue == "s-maxage"))
							{
								goto IL_03B1;
							}
						}
						else if (!(stringValue == "min-fresh"))
						{
							goto IL_03B1;
						}
					}
					else if (num != 962188105U)
					{
						if (num != 1657474316U)
						{
							if (num != 1922561311U)
							{
								goto IL_03B1;
							}
							if (!(stringValue == "max-age"))
							{
								goto IL_03B1;
							}
						}
						else
						{
							if (!(stringValue == "private"))
							{
								goto IL_03B1;
							}
							goto IL_02FE;
						}
					}
					else
					{
						if (!(stringValue == "max-stale"))
						{
							goto IL_03B1;
						}
						cacheControlHeaderValue.MaxStale = true;
						token = lexer.Scan(false);
						if (token != Token.Type.SeparatorEqual)
						{
							flag = true;
							goto IL_040A;
						}
						token = lexer.Scan(false);
						if (token != Token.Type.Token)
						{
							return false;
						}
						timeSpan = lexer.TryGetTimeSpanValue(token);
						if (timeSpan == null)
						{
							return false;
						}
						cacheControlHeaderValue.MaxStaleLimit = timeSpan;
						goto IL_040A;
					}
					token = lexer.Scan(false);
					if (token != Token.Type.SeparatorEqual)
					{
						return false;
					}
					token = lexer.Scan(false);
					if (token != Token.Type.Token)
					{
						return false;
					}
					timeSpan = lexer.TryGetTimeSpanValue(token);
					if (timeSpan == null)
					{
						return false;
					}
					int i = stringValue.Length;
					if (i != 7)
					{
						if (i != 8)
						{
							cacheControlHeaderValue.MinFresh = timeSpan;
						}
						else
						{
							cacheControlHeaderValue.SharedMaxAge = timeSpan;
						}
					}
					else
					{
						cacheControlHeaderValue.MaxAge = timeSpan;
					}
				}
				else if (num <= 2802093227U)
				{
					if (num != 2033558065U)
					{
						if (num != 2154495528U)
						{
							if (num != 2802093227U)
							{
								goto IL_03B1;
							}
							if (!(stringValue == "no-transform"))
							{
								goto IL_03B1;
							}
							cacheControlHeaderValue.NoTransform = true;
						}
						else
						{
							if (!(stringValue == "must-revalidate"))
							{
								goto IL_03B1;
							}
							cacheControlHeaderValue.MustRevalidate = true;
						}
					}
					else
					{
						if (!(stringValue == "proxy-revalidate"))
						{
							goto IL_03B1;
						}
						cacheControlHeaderValue.ProxyRevalidate = true;
					}
				}
				else if (num != 2866772502U)
				{
					if (num != 3432027008U)
					{
						if (num != 3443516981U)
						{
							goto IL_03B1;
						}
						if (!(stringValue == "no-cache"))
						{
							goto IL_03B1;
						}
						goto IL_02FE;
					}
					else
					{
						if (!(stringValue == "public"))
						{
							goto IL_03B1;
						}
						cacheControlHeaderValue.Public = true;
					}
				}
				else
				{
					if (!(stringValue == "only-if-cached"))
					{
						goto IL_03B1;
					}
					cacheControlHeaderValue.OnlyIfCached = true;
				}
				IL_040A:
				if (!flag)
				{
					token = lexer.Scan(false);
				}
				if (token != Token.Type.SeparatorComma)
				{
					goto Block_46;
				}
				continue;
				IL_02FE:
				if (stringValue.Length == 7)
				{
					cacheControlHeaderValue.Private = true;
				}
				else
				{
					cacheControlHeaderValue.NoCache = true;
				}
				token = lexer.Scan(false);
				if (token != Token.Type.SeparatorEqual)
				{
					flag = true;
					goto IL_040A;
				}
				token = lexer.Scan(false);
				if (token != Token.Type.QuotedString)
				{
					return false;
				}
				string[] array = lexer.GetQuotedStringValue(token).Split(',', StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i].Trim(new char[] { '\t', ' ' });
					if (stringValue.Length == 7)
					{
						cacheControlHeaderValue.PrivateHeaders.Add(text);
					}
					else
					{
						cacheControlHeaderValue.NoCache = true;
						cacheControlHeaderValue.NoCacheHeaders.Add(text);
					}
				}
				goto IL_040A;
				IL_03B1:
				string stringValue2 = lexer.GetStringValue(token);
				string text2 = null;
				token = lexer.Scan(false);
				if (token == Token.Type.SeparatorEqual)
				{
					token = lexer.Scan(false);
					Token.Type kind = token.Kind;
					if (kind - Token.Type.Token > 1)
					{
						return false;
					}
					text2 = lexer.GetStringValue(token);
				}
				else
				{
					flag = true;
				}
				cacheControlHeaderValue.Extensions.Add(NameValueHeaderValue.Create(stringValue2, text2));
				goto IL_040A;
			}
			return false;
			Block_46:
			if (token != Token.Type.End)
			{
				return false;
			}
			parsedValue = cacheControlHeaderValue;
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.NoStore)
			{
				stringBuilder.Append("no-store");
				stringBuilder.Append(", ");
			}
			if (this.NoTransform)
			{
				stringBuilder.Append("no-transform");
				stringBuilder.Append(", ");
			}
			if (this.OnlyIfCached)
			{
				stringBuilder.Append("only-if-cached");
				stringBuilder.Append(", ");
			}
			if (this.Public)
			{
				stringBuilder.Append("public");
				stringBuilder.Append(", ");
			}
			if (this.MustRevalidate)
			{
				stringBuilder.Append("must-revalidate");
				stringBuilder.Append(", ");
			}
			if (this.ProxyRevalidate)
			{
				stringBuilder.Append("proxy-revalidate");
				stringBuilder.Append(", ");
			}
			if (this.NoCache)
			{
				stringBuilder.Append("no-cache");
				if (this.no_cache_headers != null)
				{
					stringBuilder.Append("=\"");
					this.no_cache_headers.ToStringBuilder<string>(stringBuilder);
					stringBuilder.Append("\"");
				}
				stringBuilder.Append(", ");
			}
			if (this.MaxAge != null)
			{
				stringBuilder.Append("max-age=");
				stringBuilder.Append(this.MaxAge.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(", ");
			}
			if (this.SharedMaxAge != null)
			{
				stringBuilder.Append("s-maxage=");
				stringBuilder.Append(this.SharedMaxAge.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(", ");
			}
			if (this.MaxStale)
			{
				stringBuilder.Append("max-stale");
				if (this.MaxStaleLimit != null)
				{
					stringBuilder.Append("=");
					stringBuilder.Append(this.MaxStaleLimit.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture));
				}
				stringBuilder.Append(", ");
			}
			if (this.MinFresh != null)
			{
				stringBuilder.Append("min-fresh=");
				stringBuilder.Append(this.MinFresh.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(", ");
			}
			if (this.Private)
			{
				stringBuilder.Append("private");
				if (this.private_headers != null)
				{
					stringBuilder.Append("=\"");
					this.private_headers.ToStringBuilder<string>(stringBuilder);
					stringBuilder.Append("\"");
				}
				stringBuilder.Append(", ");
			}
			this.extensions.ToStringBuilder<NameValueHeaderValue>(stringBuilder);
			if (stringBuilder.Length > 2 && stringBuilder[stringBuilder.Length - 2] == ',' && stringBuilder[stringBuilder.Length - 1] == ' ')
			{
				stringBuilder.Remove(stringBuilder.Length - 2, 2);
			}
			return stringBuilder.ToString();
		}

		private List<NameValueHeaderValue> extensions;

		private List<string> no_cache_headers;

		private List<string> private_headers;
	}
}
