using System;
using System.IO;
using System.Security.Cryptography;
using Mono.Cecil.PE;
using Mono.Security.Cryptography;

namespace Mono.Cecil
{
	internal static class CryptoService
	{
		public static byte[] GetPublicKey(WriterParameters parameters)
		{
			byte[] array3;
			using (RSA rsa = parameters.CreateRSA())
			{
				byte[] array = CryptoConvert.ToCapiPublicKeyBlob(rsa);
				byte[] array2 = new byte[12 + array.Length];
				Buffer.BlockCopy(array, 0, array2, 12, array.Length);
				array2[1] = 36;
				array2[4] = 4;
				array2[5] = 128;
				array2[8] = (byte)array.Length;
				array2[9] = (byte)(array.Length >> 8);
				array2[10] = (byte)(array.Length >> 16);
				array2[11] = (byte)(array.Length >> 24);
				array3 = array2;
			}
			return array3;
		}

		public static void StrongName(Stream stream, ImageWriter writer, WriterParameters parameters)
		{
			int num;
			byte[] array = CryptoService.CreateStrongName(parameters, CryptoService.HashStream(stream, writer, out num));
			CryptoService.PatchStrongName(stream, num, array);
		}

		private static void PatchStrongName(Stream stream, int strong_name_pointer, byte[] strong_name)
		{
			stream.Seek((long)strong_name_pointer, SeekOrigin.Begin);
			stream.Write(strong_name, 0, strong_name.Length);
		}

		private static byte[] CreateStrongName(WriterParameters parameters, byte[] hash)
		{
			byte[] array2;
			using (RSA rsa = parameters.CreateRSA())
			{
				RSAPKCS1SignatureFormatter rsapkcs1SignatureFormatter = new RSAPKCS1SignatureFormatter(rsa);
				rsapkcs1SignatureFormatter.SetHashAlgorithm("SHA1");
				byte[] array = rsapkcs1SignatureFormatter.CreateSignature(hash);
				Array.Reverse(array);
				array2 = array;
			}
			return array2;
		}

		private static byte[] HashStream(Stream stream, ImageWriter writer, out int strong_name_pointer)
		{
			Section text = writer.text;
			int headerSize = (int)writer.GetHeaderSize();
			int pointerToRawData = (int)text.PointerToRawData;
			DataDirectory strongNameSignatureDirectory = writer.GetStrongNameSignatureDirectory();
			if (strongNameSignatureDirectory.Size == 0U)
			{
				throw new InvalidOperationException();
			}
			strong_name_pointer = (int)((long)pointerToRawData + (long)((ulong)(strongNameSignatureDirectory.VirtualAddress - text.VirtualAddress)));
			int size = (int)strongNameSignatureDirectory.Size;
			SHA1Managed sha1Managed = new SHA1Managed();
			byte[] array = new byte[8192];
			using (CryptoStream cryptoStream = new CryptoStream(Stream.Null, sha1Managed, CryptoStreamMode.Write))
			{
				stream.Seek(0L, SeekOrigin.Begin);
				CryptoService.CopyStreamChunk(stream, cryptoStream, array, headerSize);
				stream.Seek((long)pointerToRawData, SeekOrigin.Begin);
				CryptoService.CopyStreamChunk(stream, cryptoStream, array, strong_name_pointer - pointerToRawData);
				stream.Seek((long)size, SeekOrigin.Current);
				CryptoService.CopyStreamChunk(stream, cryptoStream, array, (int)(stream.Length - (long)(strong_name_pointer + size)));
			}
			return sha1Managed.Hash;
		}

		private static void CopyStreamChunk(Stream stream, Stream dest_stream, byte[] buffer, int length)
		{
			while (length > 0)
			{
				int num = stream.Read(buffer, 0, Math.Min(buffer.Length, length));
				dest_stream.Write(buffer, 0, num);
				length -= num;
			}
		}

		public static byte[] ComputeHash(string file)
		{
			if (!File.Exists(file))
			{
				return Empty<byte>.Array;
			}
			byte[] array;
			using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				array = CryptoService.ComputeHash(fileStream);
			}
			return array;
		}

		public static byte[] ComputeHash(Stream stream)
		{
			SHA1Managed sha1Managed = new SHA1Managed();
			byte[] array = new byte[8192];
			using (CryptoStream cryptoStream = new CryptoStream(Stream.Null, sha1Managed, CryptoStreamMode.Write))
			{
				CryptoService.CopyStreamChunk(stream, cryptoStream, array, (int)stream.Length);
			}
			return sha1Managed.Hash;
		}

		public static byte[] ComputeHash(params ByteBuffer[] buffers)
		{
			SHA1Managed sha1Managed = new SHA1Managed();
			using (CryptoStream cryptoStream = new CryptoStream(Stream.Null, sha1Managed, CryptoStreamMode.Write))
			{
				for (int i = 0; i < buffers.Length; i++)
				{
					cryptoStream.Write(buffers[i].buffer, 0, buffers[i].length);
				}
			}
			return sha1Managed.Hash;
		}

		public static Guid ComputeGuid(byte[] hash)
		{
			byte[] array = new byte[16];
			Buffer.BlockCopy(hash, 0, array, 0, 16);
			array[7] = (array[7] & 15) | 64;
			array[8] = (array[8] & 63) | 128;
			return new Guid(array);
		}
	}
}
