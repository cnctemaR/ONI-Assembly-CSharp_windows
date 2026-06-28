using System;

namespace Hjg.Pngcs.Chunks
{
	public class PngChunkICCP : PngChunkSingle
	{
		public PngChunkICCP(ImageInfo info)
			: base("iCCP", info)
		{
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = base.createEmptyChunk(this.profileName.Length + this.compressedProfile.Length + 2, true);
			Array.Copy(ChunkHelper.ToBytes(this.profileName), 0, chunkRaw.Data, 0, this.profileName.Length);
			chunkRaw.Data[this.profileName.Length] = 0;
			chunkRaw.Data[this.profileName.Length + 1] = 0;
			Array.Copy(this.compressedProfile, 0, chunkRaw.Data, this.profileName.Length + 2, this.compressedProfile.Length);
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw chunk)
		{
			int num = ChunkHelper.PosNullByte(chunk.Data);
			this.profileName = PngHelperInternal.charsetLatin1.GetString(chunk.Data, 0, num);
			int num2 = (int)(chunk.Data[num + 1] & byte.MaxValue);
			if (num2 != 0)
			{
				throw new Exception("bad compression for ChunkTypeICCP");
			}
			int num3 = chunk.Data.Length - (num + 2);
			this.compressedProfile = new byte[num3];
			Array.Copy(chunk.Data, num + 2, this.compressedProfile, 0, num3);
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkICCP pngChunkICCP = (PngChunkICCP)other;
			this.profileName = pngChunkICCP.profileName;
			this.compressedProfile = new byte[pngChunkICCP.compressedProfile.Length];
			Array.Copy(pngChunkICCP.compressedProfile, this.compressedProfile, this.compressedProfile.Length);
		}

		public void SetProfileNameAndContent(string name, string profile)
		{
			this.SetProfileNameAndContent(name, ChunkHelper.ToBytes(this.profileName));
		}

		public void SetProfileNameAndContent(string name, byte[] profile)
		{
			this.profileName = name;
			this.compressedProfile = ChunkHelper.compressBytes(profile, true);
		}

		public string GetProfileName()
		{
			return this.profileName;
		}

		public byte[] GetProfile()
		{
			return ChunkHelper.compressBytes(this.compressedProfile, false);
		}

		public string GetProfileAsString()
		{
			return ChunkHelper.ToString(this.GetProfile());
		}

		public const string ID = "iCCP";

		private string profileName;

		private byte[] compressedProfile;
	}
}
