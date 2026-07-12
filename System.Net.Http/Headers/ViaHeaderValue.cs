using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	public class ViaHeaderValue : ICloneable
	{
		public ViaHeaderValue(string protocolVersion, string receivedBy)
		{
			Parser.Token.Check(protocolVersion);
			Parser.Uri.Check(receivedBy);
			this.ProtocolVersion = protocolVersion;
			this.ReceivedBy = receivedBy;
		}

		public ViaHeaderValue(string protocolVersion, string receivedBy, string protocolName)
			: this(protocolVersion, receivedBy)
		{
			if (!string.IsNullOrEmpty(protocolName))
			{
				Parser.Token.Check(protocolName);
				this.ProtocolName = protocolName;
			}
		}

		public ViaHeaderValue(string protocolVersion, string receivedBy, string protocolName, string comment)
			: this(protocolVersion, receivedBy, protocolName)
		{
			if (!string.IsNullOrEmpty(comment))
			{
				Parser.Token.CheckComment(comment);
				this.Comment = comment;
			}
		}

		private ViaHeaderValue()
		{
		}

		public string Comment { get; private set; }

		public string ProtocolName { get; private set; }

		public string ProtocolVersion { get; private set; }

		public string ReceivedBy { get; private set; }

		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		public override bool Equals(object obj)
		{
			ViaHeaderValue viaHeaderValue = obj as ViaHeaderValue;
			return viaHeaderValue != null && (string.Equals(viaHeaderValue.Comment, this.Comment, StringComparison.Ordinal) && string.Equals(viaHeaderValue.ProtocolName, this.ProtocolName, StringComparison.OrdinalIgnoreCase) && string.Equals(viaHeaderValue.ProtocolVersion, this.ProtocolVersion, StringComparison.OrdinalIgnoreCase)) && string.Equals(viaHeaderValue.ReceivedBy, this.ReceivedBy, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			int num = this.ProtocolVersion.ToLowerInvariant().GetHashCode();
			num ^= this.ReceivedBy.ToLowerInvariant().GetHashCode();
			if (!string.IsNullOrEmpty(this.ProtocolName))
			{
				num ^= this.ProtocolName.ToLowerInvariant().GetHashCode();
			}
			if (!string.IsNullOrEmpty(this.Comment))
			{
				num ^= this.Comment.GetHashCode();
			}
			return num;
		}

		public static ViaHeaderValue Parse(string input)
		{
			ViaHeaderValue viaHeaderValue;
			if (ViaHeaderValue.TryParse(input, out viaHeaderValue))
			{
				return viaHeaderValue;
			}
			throw new FormatException(input);
		}

		public static bool TryParse(string input, out ViaHeaderValue parsedValue)
		{
			Token token;
			if (ViaHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		internal static bool TryParse(string input, int minimalCount, out List<ViaHeaderValue> result)
		{
			return CollectionParser.TryParse<ViaHeaderValue>(input, minimalCount, new ElementTryParser<ViaHeaderValue>(ViaHeaderValue.TryParseElement), out result);
		}

		private static bool TryParseElement(Lexer lexer, out ViaHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			Token token = lexer.Scan(false);
			ViaHeaderValue viaHeaderValue = new ViaHeaderValue();
			if (token == Token.Type.SeparatorSlash)
			{
				token = lexer.Scan(false);
				if (token != Token.Type.Token)
				{
					return false;
				}
				viaHeaderValue.ProtocolName = lexer.GetStringValue(t);
				viaHeaderValue.ProtocolVersion = lexer.GetStringValue(token);
				token = lexer.Scan(false);
			}
			else
			{
				viaHeaderValue.ProtocolVersion = lexer.GetStringValue(t);
			}
			if (token != Token.Type.Token)
			{
				return false;
			}
			if (lexer.PeekChar() == 58)
			{
				lexer.EatChar();
				t = lexer.Scan(false);
				if (t != Token.Type.Token)
				{
					return false;
				}
			}
			else
			{
				t = token;
			}
			viaHeaderValue.ReceivedBy = lexer.GetStringValue(token, t);
			string text;
			if (lexer.ScanCommentOptional(out text, out t))
			{
				t = lexer.Scan(false);
			}
			viaHeaderValue.Comment = text;
			parsedValue = viaHeaderValue;
			return true;
		}

		public override string ToString()
		{
			string text = ((this.ProtocolName != null) ? string.Concat(new string[] { this.ProtocolName, "/", this.ProtocolVersion, " ", this.ReceivedBy }) : (this.ProtocolVersion + " " + this.ReceivedBy));
			if (this.Comment == null)
			{
				return text;
			}
			return text + " " + this.Comment;
		}
	}
}
