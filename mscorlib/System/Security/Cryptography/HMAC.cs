using System;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class HMAC : KeyedHashAlgorithm
	{
		protected HMAC()
		{
			this._disposed = false;
			this._blockSizeValue = 64;
		}

		protected int BlockSizeValue
		{
			get
			{
				return this._blockSizeValue;
			}
			set
			{
				this._blockSizeValue = value;
			}
		}

		public string HashName
		{
			get
			{
				return this._hashName;
			}
			set
			{
				this._hashName = value;
				this._algo = HashAlgorithm.Create(this._hashName);
			}
		}

		public override byte[] Key
		{
			get
			{
				return (byte[])base.Key.Clone();
			}
			set
			{
				if (value != null && value.Length > 64)
				{
					base.Key = this._algo.ComputeHash(value);
				}
				else
				{
					base.Key = (byte[])value.Clone();
				}
			}
		}

		internal BlockProcessor Block
		{
			get
			{
				if (this._block == null)
				{
					this._block = new BlockProcessor(this._algo, this.BlockSizeValue >> 3);
				}
				return this._block;
			}
		}

		private byte[] KeySetup(byte[] key, byte padding)
		{
			byte[] array = new byte[this.BlockSizeValue];
			for (int i = 0; i < key.Length; i++)
			{
				array[i] = key[i] ^ padding;
			}
			for (int j = key.Length; j < this.BlockSizeValue; j++)
			{
				array[j] = padding;
			}
			return array;
		}

		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				base.Dispose(disposing);
			}
		}

		protected override void HashCore(byte[] rgb, int ib, int cb)
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("HMACSHA1");
			}
			if (this.State == 0)
			{
				this.Initialize();
				this.State = 1;
			}
			this.Block.Core(rgb, ib, cb);
		}

		protected override byte[] HashFinal()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("HMAC");
			}
			this.State = 0;
			this.Block.Final();
			byte[] hash = this._algo.Hash;
			byte[] array = this.KeySetup(this.Key, 92);
			this._algo.Initialize();
			this._algo.TransformBlock(array, 0, array.Length, array, 0);
			this._algo.TransformFinalBlock(hash, 0, hash.Length);
			byte[] hash2 = this._algo.Hash;
			this._algo.Initialize();
			Array.Clear(array, 0, array.Length);
			Array.Clear(hash, 0, hash.Length);
			return hash2;
		}

		public override void Initialize()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("HMAC");
			}
			this.State = 0;
			this.Block.Initialize();
			byte[] array = this.KeySetup(this.Key, 54);
			this._algo.Initialize();
			this.Block.Core(array);
			Array.Clear(array, 0, array.Length);
		}

		public new static HMAC Create()
		{
			return HMAC.Create("System.Security.Cryptography.HMAC");
		}

		public new static HMAC Create(string algorithmName)
		{
			return (HMAC)CryptoConfig.CreateFromName(algorithmName);
		}

		private bool _disposed;

		private string _hashName;

		private HashAlgorithm _algo;

		private BlockProcessor _block;

		private int _blockSizeValue;
	}
}
