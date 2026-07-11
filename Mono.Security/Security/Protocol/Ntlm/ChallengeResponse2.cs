using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Mono.Security.Cryptography;

namespace Mono.Security.Protocol.Ntlm
{
	public static class ChallengeResponse2
	{
		private static byte[] Compute_LM(string password, byte[] challenge)
		{
			byte[] array = new byte[21];
			DES des = DES.Create();
			des.Mode = CipherMode.ECB;
			if (password == null || password.Length < 1)
			{
				Buffer.BlockCopy(ChallengeResponse2.nullEncMagic, 0, array, 0, 8);
			}
			else
			{
				des.Key = ChallengeResponse2.PasswordToKey(password, 0);
				des.CreateEncryptor().TransformBlock(ChallengeResponse2.magic, 0, 8, array, 0);
			}
			if (password == null || password.Length < 8)
			{
				Buffer.BlockCopy(ChallengeResponse2.nullEncMagic, 0, array, 8, 8);
			}
			else
			{
				des.Key = ChallengeResponse2.PasswordToKey(password, 7);
				des.CreateEncryptor().TransformBlock(ChallengeResponse2.magic, 0, 8, array, 8);
			}
			des.Clear();
			return ChallengeResponse2.GetResponse(challenge, array);
		}

		private static byte[] Compute_NTLM_Password(string password)
		{
			byte[] array = new byte[21];
			HashAlgorithm hashAlgorithm = MD4.Create();
			byte[] array2 = ((password == null) ? new byte[0] : Encoding.Unicode.GetBytes(password));
			byte[] array3 = hashAlgorithm.ComputeHash(array2);
			Buffer.BlockCopy(array3, 0, array, 0, 16);
			Array.Clear(array2, 0, array2.Length);
			Array.Clear(array3, 0, array3.Length);
			return array;
		}

		private static byte[] Compute_NTLM(string password, byte[] challenge)
		{
			byte[] array = ChallengeResponse2.Compute_NTLM_Password(password);
			return ChallengeResponse2.GetResponse(challenge, array);
		}

		private static void Compute_NTLMv2_Session(string password, byte[] challenge, out byte[] lm, out byte[] ntlm)
		{
			byte[] array = new byte[8];
			RandomNumberGenerator.Create().GetBytes(array);
			byte[] array2 = new byte[challenge.Length + 8];
			challenge.CopyTo(array2, 0);
			array.CopyTo(array2, challenge.Length);
			lm = new byte[24];
			array.CopyTo(lm, 0);
			byte[] array3 = MD5.Create().ComputeHash(array2);
			byte[] array4 = new byte[8];
			Array.Copy(array3, array4, 8);
			ntlm = ChallengeResponse2.Compute_NTLM(password, array4);
			Array.Clear(array, 0, array.Length);
			Array.Clear(array2, 0, array2.Length);
			Array.Clear(array4, 0, array4.Length);
			Array.Clear(array3, 0, array3.Length);
		}

		private static byte[] Compute_NTLMv2(Type2Message type2, string username, string password, string domain)
		{
			byte[] array = ChallengeResponse2.Compute_NTLM_Password(password);
			byte[] bytes = Encoding.Unicode.GetBytes(username.ToUpperInvariant());
			byte[] bytes2 = Encoding.Unicode.GetBytes(domain);
			byte[] array2 = new byte[bytes.Length + bytes2.Length];
			bytes.CopyTo(array2, 0);
			Array.Copy(bytes2, 0, array2, bytes.Length, bytes2.Length);
			HMACMD5 hmacmd = new HMACMD5(array);
			byte[] array3 = hmacmd.ComputeHash(array2);
			Array.Clear(array, 0, array.Length);
			hmacmd.Clear();
			HMACMD5 hmacmd2 = new HMACMD5(array3);
			long num = DateTime.Now.Ticks - 504911232000000000L;
			byte[] array4 = new byte[8];
			RandomNumberGenerator.Create().GetBytes(array4);
			byte[] array5 = new byte[28 + type2.TargetInfo.Length];
			array5[0] = 1;
			array5[1] = 1;
			Buffer.BlockCopy(BitConverterLE.GetBytes(num), 0, array5, 8, 8);
			Buffer.BlockCopy(array4, 0, array5, 16, 8);
			Buffer.BlockCopy(type2.TargetInfo, 0, array5, 28, type2.TargetInfo.Length);
			byte[] nonce = type2.Nonce;
			byte[] array6 = new byte[nonce.Length + array5.Length];
			nonce.CopyTo(array6, 0);
			array5.CopyTo(array6, nonce.Length);
			byte[] array7 = hmacmd2.ComputeHash(array6);
			byte[] array8 = new byte[array5.Length + array7.Length];
			array7.CopyTo(array8, 0);
			array5.CopyTo(array8, array7.Length);
			Array.Clear(array3, 0, array3.Length);
			hmacmd2.Clear();
			Array.Clear(array4, 0, array4.Length);
			Array.Clear(array5, 0, array5.Length);
			Array.Clear(array6, 0, array6.Length);
			Array.Clear(array7, 0, array7.Length);
			return array8;
		}

		public static void Compute(Type2Message type2, NtlmAuthLevel level, string username, string password, string domain, out byte[] lm, out byte[] ntlm)
		{
			lm = null;
			switch (level)
			{
			case NtlmAuthLevel.LM_and_NTLM:
				break;
			case NtlmAuthLevel.LM_and_NTLM_and_try_NTLMv2_Session:
				if ((type2.Flags & NtlmFlags.NegotiateNtlm2Key) != (NtlmFlags)0)
				{
					ChallengeResponse2.Compute_NTLMv2_Session(password, type2.Nonce, out lm, out ntlm);
					return;
				}
				break;
			case NtlmAuthLevel.NTLM_only:
				if ((type2.Flags & NtlmFlags.NegotiateNtlm2Key) != (NtlmFlags)0)
				{
					ChallengeResponse2.Compute_NTLMv2_Session(password, type2.Nonce, out lm, out ntlm);
					return;
				}
				ntlm = ChallengeResponse2.Compute_NTLM(password, type2.Nonce);
				return;
			case NtlmAuthLevel.NTLMv2_only:
				ntlm = ChallengeResponse2.Compute_NTLMv2(type2, username, password, domain);
				return;
			default:
				throw new InvalidOperationException();
			}
			lm = ChallengeResponse2.Compute_LM(password, type2.Nonce);
			ntlm = ChallengeResponse2.Compute_NTLM(password, type2.Nonce);
		}

		private static byte[] GetResponse(byte[] challenge, byte[] pwd)
		{
			byte[] array = new byte[24];
			DES des = DES.Create();
			des.Mode = CipherMode.ECB;
			des.Key = ChallengeResponse2.PrepareDESKey(pwd, 0);
			des.CreateEncryptor().TransformBlock(challenge, 0, 8, array, 0);
			des.Key = ChallengeResponse2.PrepareDESKey(pwd, 7);
			des.CreateEncryptor().TransformBlock(challenge, 0, 8, array, 8);
			des.Key = ChallengeResponse2.PrepareDESKey(pwd, 14);
			des.CreateEncryptor().TransformBlock(challenge, 0, 8, array, 16);
			return array;
		}

		private static byte[] PrepareDESKey(byte[] key56bits, int position)
		{
			return new byte[]
			{
				key56bits[position],
				(byte)(((int)key56bits[position] << 7) | (key56bits[position + 1] >> 1)),
				(byte)(((int)key56bits[position + 1] << 6) | (key56bits[position + 2] >> 2)),
				(byte)(((int)key56bits[position + 2] << 5) | (key56bits[position + 3] >> 3)),
				(byte)(((int)key56bits[position + 3] << 4) | (key56bits[position + 4] >> 4)),
				(byte)(((int)key56bits[position + 4] << 3) | (key56bits[position + 5] >> 5)),
				(byte)(((int)key56bits[position + 5] << 2) | (key56bits[position + 6] >> 6)),
				(byte)(key56bits[position + 6] << 1)
			};
		}

		private static byte[] PasswordToKey(string password, int position)
		{
			byte[] array = new byte[7];
			int num = Math.Min(password.Length - position, 7);
			Encoding.ASCII.GetBytes(password.ToUpper(CultureInfo.CurrentCulture), position, num, array, 0);
			byte[] array2 = ChallengeResponse2.PrepareDESKey(array, 0);
			Array.Clear(array, 0, array.Length);
			return array2;
		}

		private static byte[] magic = new byte[] { 75, 71, 83, 33, 64, 35, 36, 37 };

		private static byte[] nullEncMagic = new byte[] { 170, 211, 180, 53, 181, 20, 4, 238 };
	}
}
