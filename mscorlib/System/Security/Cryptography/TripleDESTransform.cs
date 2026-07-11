using System;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	internal class TripleDESTransform : SymmetricTransform
	{
		public TripleDESTransform(TripleDES algo, bool encryption, byte[] key, byte[] iv)
			: base(algo, encryption, iv)
		{
			if (key == null)
			{
				key = TripleDESTransform.GetStrongKey();
			}
			if (TripleDES.IsWeakKey(key))
			{
				string text = Locale.GetText("This is a known weak key.");
				throw new CryptographicException(text);
			}
			byte[] array = new byte[8];
			byte[] array2 = new byte[8];
			byte[] array3 = new byte[8];
			DES des = DES.Create();
			Buffer.BlockCopy(key, 0, array, 0, 8);
			Buffer.BlockCopy(key, 8, array2, 0, 8);
			if (key.Length == 16)
			{
				Buffer.BlockCopy(key, 0, array3, 0, 8);
			}
			else
			{
				Buffer.BlockCopy(key, 16, array3, 0, 8);
			}
			if (encryption || algo.Mode == CipherMode.CFB)
			{
				this.E1 = new DESTransform(des, true, array, iv);
				this.D2 = new DESTransform(des, false, array2, iv);
				this.E3 = new DESTransform(des, true, array3, iv);
			}
			else
			{
				this.D1 = new DESTransform(des, false, array3, iv);
				this.E2 = new DESTransform(des, true, array2, iv);
				this.D3 = new DESTransform(des, false, array, iv);
			}
		}

		protected override void ECB(byte[] input, byte[] output)
		{
			DESTransform.Permutation(input, output, DESTransform.ipTab, false);
			if (this.encrypt)
			{
				this.E1.ProcessBlock(output, output);
				this.D2.ProcessBlock(output, output);
				this.E3.ProcessBlock(output, output);
			}
			else
			{
				this.D1.ProcessBlock(output, output);
				this.E2.ProcessBlock(output, output);
				this.D3.ProcessBlock(output, output);
			}
			DESTransform.Permutation(output, output, DESTransform.fpTab, true);
		}

		internal static byte[] GetStrongKey()
		{
			int num = DESTransform.BLOCK_BYTE_SIZE * 3;
			byte[] array = KeyBuilder.Key(num);
			while (TripleDES.IsWeakKey(array))
			{
				array = KeyBuilder.Key(num);
			}
			return array;
		}

		private DESTransform E1;

		private DESTransform D2;

		private DESTransform E3;

		private DESTransform D1;

		private DESTransform E2;

		private DESTransform D3;
	}
}
