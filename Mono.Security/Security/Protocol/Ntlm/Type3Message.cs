using System;
using System.Text;

namespace Mono.Security.Protocol.Ntlm
{
	public class Type3Message : MessageBase
	{
		[Obsolete("Use of this API is highly discouraged, it selects legacy-mode LM/NTLM authentication, which sends your password in very weak encryption over the wire even if the server supports the more secure NTLMv2 / NTLMv2 Session. You need to use the new `Type3Message (Type2Message)' constructor to use the more secure NTLMv2 / NTLMv2 Session authentication modes. These require the Type 2 message from the server to compute the response.")]
		public Type3Message()
			: base(3)
		{
			if (Type3Message.DefaultAuthLevel != NtlmAuthLevel.LM_and_NTLM)
			{
				throw new InvalidOperationException("Refusing to use legacy-mode LM/NTLM authentication unless explicitly enabled using DefaultAuthLevel.");
			}
			this._domain = Environment.UserDomainName;
			this._host = Environment.MachineName;
			this._username = Environment.UserName;
			this._level = NtlmAuthLevel.LM_and_NTLM;
			base.Flags = NtlmFlags.NegotiateUnicode | NtlmFlags.NegotiateNtlm | NtlmFlags.NegotiateAlwaysSign;
		}

		public Type3Message(byte[] message)
			: base(3)
		{
			this.Decode(message);
		}

		public Type3Message(Type2Message type2)
			: base(3)
		{
			this._type2 = type2;
			this._level = NtlmSettings.DefaultAuthLevel;
			this._challenge = (byte[])type2.Nonce.Clone();
			this._domain = type2.TargetName;
			this._host = Environment.MachineName;
			this._username = Environment.UserName;
			base.Flags = NtlmFlags.NegotiateNtlm | NtlmFlags.NegotiateAlwaysSign;
			if ((type2.Flags & NtlmFlags.NegotiateUnicode) != (NtlmFlags)0)
			{
				base.Flags |= NtlmFlags.NegotiateUnicode;
			}
			else
			{
				base.Flags |= NtlmFlags.NegotiateOem;
			}
			if ((type2.Flags & NtlmFlags.NegotiateNtlm2Key) != (NtlmFlags)0)
			{
				base.Flags |= NtlmFlags.NegotiateNtlm2Key;
			}
		}

		~Type3Message()
		{
			if (this._challenge != null)
			{
				Array.Clear(this._challenge, 0, this._challenge.Length);
			}
			if (this._lm != null)
			{
				Array.Clear(this._lm, 0, this._lm.Length);
			}
			if (this._nt != null)
			{
				Array.Clear(this._nt, 0, this._nt.Length);
			}
		}

		[Obsolete("Use NtlmSettings.DefaultAuthLevel")]
		public static NtlmAuthLevel DefaultAuthLevel
		{
			get
			{
				return NtlmSettings.DefaultAuthLevel;
			}
			set
			{
				NtlmSettings.DefaultAuthLevel = value;
			}
		}

		public NtlmAuthLevel Level
		{
			get
			{
				return this._level;
			}
			set
			{
				this._level = value;
			}
		}

		[Obsolete("Use of this API is highly discouraged, it selects legacy-mode LM/NTLM authentication, which sends your password in very weak encryption over the wire even if the server supports the more secure NTLMv2 / NTLMv2 Session. You need to use the new `Type3Message (Type2Message)' constructor to use the more secure NTLMv2 / NTLMv2 Session authentication modes. These require the Type 2 message from the server to compute the response.")]
		public byte[] Challenge
		{
			get
			{
				if (this._challenge == null)
				{
					return null;
				}
				return (byte[])this._challenge.Clone();
			}
			set
			{
				if (this._type2 != null || this._level != NtlmAuthLevel.LM_and_NTLM)
				{
					throw new InvalidOperationException("Refusing to use legacy-mode LM/NTLM authentication unless explicitly enabled using DefaultAuthLevel.");
				}
				if (value == null)
				{
					throw new ArgumentNullException("Challenge");
				}
				if (value.Length != 8)
				{
					throw new ArgumentException(Locale.GetText("Invalid Challenge Length (should be 8 bytes)."), "Challenge");
				}
				this._challenge = (byte[])value.Clone();
			}
		}

		public string Domain
		{
			get
			{
				return this._domain;
			}
			set
			{
				if (value == null)
				{
					value = "";
				}
				if (value == "")
				{
					base.Flags &= ~NtlmFlags.NegotiateDomainSupplied;
				}
				else
				{
					base.Flags |= NtlmFlags.NegotiateDomainSupplied;
				}
				this._domain = value;
			}
		}

		public string Host
		{
			get
			{
				return this._host;
			}
			set
			{
				if (value == null)
				{
					value = "";
				}
				if (value == "")
				{
					base.Flags &= ~NtlmFlags.NegotiateWorkstationSupplied;
				}
				else
				{
					base.Flags |= NtlmFlags.NegotiateWorkstationSupplied;
				}
				this._host = value;
			}
		}

		public string Password
		{
			get
			{
				return this._password;
			}
			set
			{
				this._password = value;
			}
		}

		public string Username
		{
			get
			{
				return this._username;
			}
			set
			{
				this._username = value;
			}
		}

		public byte[] LM
		{
			get
			{
				return this._lm;
			}
		}

		public byte[] NT
		{
			get
			{
				return this._nt;
			}
			set
			{
				this._nt = value;
			}
		}

		protected override void Decode(byte[] message)
		{
			base.Decode(message);
			this._password = null;
			if (message.Length >= 64)
			{
				base.Flags = (NtlmFlags)BitConverterLE.ToUInt32(message, 60);
			}
			else
			{
				base.Flags = NtlmFlags.NegotiateUnicode | NtlmFlags.NegotiateNtlm | NtlmFlags.NegotiateAlwaysSign;
			}
			int num = (int)BitConverterLE.ToUInt16(message, 12);
			int num2 = (int)BitConverterLE.ToUInt16(message, 16);
			this._lm = new byte[num];
			Buffer.BlockCopy(message, num2, this._lm, 0, num);
			int num3 = (int)BitConverterLE.ToUInt16(message, 20);
			int num4 = (int)BitConverterLE.ToUInt16(message, 24);
			this._nt = new byte[num3];
			Buffer.BlockCopy(message, num4, this._nt, 0, num3);
			int num5 = (int)BitConverterLE.ToUInt16(message, 28);
			int num6 = (int)BitConverterLE.ToUInt16(message, 32);
			this._domain = this.DecodeString(message, num6, num5);
			int num7 = (int)BitConverterLE.ToUInt16(message, 36);
			int num8 = (int)BitConverterLE.ToUInt16(message, 40);
			this._username = this.DecodeString(message, num8, num7);
			int num9 = (int)BitConverterLE.ToUInt16(message, 44);
			int num10 = (int)BitConverterLE.ToUInt16(message, 48);
			this._host = this.DecodeString(message, num10, num9);
		}

		private string DecodeString(byte[] buffer, int offset, int len)
		{
			if ((base.Flags & NtlmFlags.NegotiateUnicode) != (NtlmFlags)0)
			{
				return Encoding.Unicode.GetString(buffer, offset, len);
			}
			return Encoding.ASCII.GetString(buffer, offset, len);
		}

		private byte[] EncodeString(string text)
		{
			if (text == null)
			{
				return new byte[0];
			}
			if ((base.Flags & NtlmFlags.NegotiateUnicode) != (NtlmFlags)0)
			{
				return Encoding.Unicode.GetBytes(text);
			}
			return Encoding.ASCII.GetBytes(text);
		}

		public override byte[] GetBytes()
		{
			byte[] array = this.EncodeString(this._domain);
			byte[] array2 = this.EncodeString(this._username);
			byte[] array3 = this.EncodeString(this._host);
			byte[] lm;
			byte[] nt;
			if (this._type2 == null)
			{
				if (this._level != NtlmAuthLevel.LM_and_NTLM)
				{
					throw new InvalidOperationException("Refusing to use legacy-mode LM/NTLM authentication unless explicitly enabled using DefaultAuthLevel.");
				}
				using (ChallengeResponse challengeResponse = new ChallengeResponse(this._password, this._challenge))
				{
					lm = challengeResponse.LM;
					nt = challengeResponse.NT;
					goto IL_009B;
				}
			}
			ChallengeResponse2.Compute(this._type2, this._level, this._username, this._password, this._domain, out lm, out nt);
			IL_009B:
			int num = ((lm != null) ? lm.Length : 0);
			int num2 = ((nt != null) ? nt.Length : 0);
			byte[] array4 = base.PrepareMessage(64 + array.Length + array2.Length + array3.Length + num + num2);
			short num3 = (short)(64 + array.Length + array2.Length + array3.Length);
			array4[12] = (byte)num;
			array4[13] = 0;
			array4[14] = (byte)num;
			array4[15] = 0;
			array4[16] = (byte)num3;
			array4[17] = (byte)(num3 >> 8);
			short num4 = (short)((int)num3 + num);
			array4[20] = (byte)num2;
			array4[21] = (byte)(num2 >> 8);
			array4[22] = (byte)num2;
			array4[23] = (byte)(num2 >> 8);
			array4[24] = (byte)num4;
			array4[25] = (byte)(num4 >> 8);
			short num5 = (short)array.Length;
			short num6 = 64;
			array4[28] = (byte)num5;
			array4[29] = (byte)(num5 >> 8);
			array4[30] = array4[28];
			array4[31] = array4[29];
			array4[32] = (byte)num6;
			array4[33] = (byte)(num6 >> 8);
			short num7 = (short)array2.Length;
			short num8 = num6 + num5;
			array4[36] = (byte)num7;
			array4[37] = (byte)(num7 >> 8);
			array4[38] = array4[36];
			array4[39] = array4[37];
			array4[40] = (byte)num8;
			array4[41] = (byte)(num8 >> 8);
			short num9 = (short)array3.Length;
			short num10 = num8 + num7;
			array4[44] = (byte)num9;
			array4[45] = (byte)(num9 >> 8);
			array4[46] = array4[44];
			array4[47] = array4[45];
			array4[48] = (byte)num10;
			array4[49] = (byte)(num10 >> 8);
			short num11 = (short)array4.Length;
			array4[56] = (byte)num11;
			array4[57] = (byte)(num11 >> 8);
			int flags = (int)base.Flags;
			array4[60] = (byte)flags;
			array4[61] = (byte)((uint)flags >> 8);
			array4[62] = (byte)((uint)flags >> 16);
			array4[63] = (byte)((uint)flags >> 24);
			Buffer.BlockCopy(array, 0, array4, (int)num6, array.Length);
			Buffer.BlockCopy(array2, 0, array4, (int)num8, array2.Length);
			Buffer.BlockCopy(array3, 0, array4, (int)num10, array3.Length);
			if (lm != null)
			{
				Buffer.BlockCopy(lm, 0, array4, (int)num3, lm.Length);
				Array.Clear(lm, 0, lm.Length);
			}
			Buffer.BlockCopy(nt, 0, array4, (int)num4, nt.Length);
			Array.Clear(nt, 0, nt.Length);
			return array4;
		}

		private NtlmAuthLevel _level;

		private byte[] _challenge;

		private string _host;

		private string _domain;

		private string _username;

		private string _password;

		private Type2Message _type2;

		private byte[] _lm;

		private byte[] _nt;

		internal const string LegacyAPIWarning = "Use of this API is highly discouraged, it selects legacy-mode LM/NTLM authentication, which sends your password in very weak encryption over the wire even if the server supports the more secure NTLMv2 / NTLMv2 Session. You need to use the new `Type3Message (Type2Message)' constructor to use the more secure NTLMv2 / NTLMv2 Session authentication modes. These require the Type 2 message from the server to compute the response.";
	}
}
