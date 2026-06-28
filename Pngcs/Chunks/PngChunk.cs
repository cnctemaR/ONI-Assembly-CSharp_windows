using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Hjg.Pngcs.Chunks
{
	public abstract class PngChunk
	{
		public bool Priority { get; set; }

		public int ChunkGroup { get; set; }

		public int Length { get; set; }

		public long Offset { get; set; }

		protected PngChunk(string id, ImageInfo imgInfo)
		{
			this.Id = id;
			this.ImgInfo = imgInfo;
			this.Crit = ChunkHelper.IsCritical(id);
			this.Pub = ChunkHelper.IsPublic(id);
			this.Safe = ChunkHelper.IsSafeToCopy(id);
			this.Priority = false;
			this.ChunkGroup = -1;
			this.Length = -1;
			this.Offset = 0L;
		}

		private static Dictionary<string, Type> initFactory()
		{
			return new Dictionary<string, Type>
			{
				{
					"IDAT",
					typeof(PngChunkIDAT)
				},
				{
					"IHDR",
					typeof(PngChunkIHDR)
				},
				{
					"PLTE",
					typeof(PngChunkPLTE)
				},
				{
					"IEND",
					typeof(PngChunkIEND)
				},
				{
					"tEXt",
					typeof(PngChunkTEXT)
				},
				{
					"iTXt",
					typeof(PngChunkITXT)
				},
				{
					"zTXt",
					typeof(PngChunkZTXT)
				},
				{
					"bKGD",
					typeof(PngChunkBKGD)
				},
				{
					"gAMA",
					typeof(PngChunkGAMA)
				},
				{
					"pHYs",
					typeof(PngChunkPHYS)
				},
				{
					"iCCP",
					typeof(PngChunkICCP)
				},
				{
					"tIME",
					typeof(PngChunkTIME)
				},
				{
					"tRNS",
					typeof(PngChunkTRNS)
				},
				{
					"cHRM",
					typeof(PngChunkCHRM)
				},
				{
					"sBIT",
					typeof(PngChunkSBIT)
				},
				{
					"sRGB",
					typeof(PngChunkSRGB)
				},
				{
					"hIST",
					typeof(PngChunkHIST)
				},
				{
					"sPLT",
					typeof(PngChunkSPLT)
				},
				{
					"oFFs",
					typeof(PngChunkOFFS)
				},
				{
					"sTER",
					typeof(PngChunkSTER)
				}
			};
		}

		public static void FactoryRegister(string chunkId, Type type)
		{
			PngChunk.factoryMap.Add(chunkId, type);
		}

		internal static bool isKnown(string id)
		{
			return PngChunk.factoryMap.ContainsKey(id);
		}

		internal bool mustGoBeforePLTE()
		{
			return this.GetOrderingConstraint() == PngChunk.ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
		}

		internal bool mustGoBeforeIDAT()
		{
			PngChunk.ChunkOrderingConstraint orderingConstraint = this.GetOrderingConstraint();
			return orderingConstraint == PngChunk.ChunkOrderingConstraint.BEFORE_IDAT || orderingConstraint == PngChunk.ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT || orderingConstraint == PngChunk.ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
		}

		internal bool mustGoAfterPLTE()
		{
			return this.GetOrderingConstraint() == PngChunk.ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
		}

		internal static PngChunk Factory(ChunkRaw chunk, ImageInfo info)
		{
			PngChunk pngChunk = PngChunk.FactoryFromId(ChunkHelper.ToString(chunk.IdBytes), info);
			pngChunk.Length = chunk.Length;
			pngChunk.ParseFromRaw(chunk);
			return pngChunk;
		}

		internal static PngChunk FactoryFromId(string cid, ImageInfo info)
		{
			PngChunk pngChunk = null;
			if (PngChunk.factoryMap == null)
			{
				PngChunk.initFactory();
			}
			if (PngChunk.isKnown(cid))
			{
				Type type = PngChunk.factoryMap[cid];
				if (type == null)
				{
					Console.Error.WriteLine("What?? " + cid);
				}
				ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(ImageInfo) });
				object obj = constructor.Invoke(new object[] { info });
				pngChunk = (PngChunk)obj;
			}
			if (pngChunk == null)
			{
				pngChunk = new PngChunkUNKNOWN(cid, info);
			}
			return pngChunk;
		}

		public ChunkRaw createEmptyChunk(int len, bool alloc)
		{
			return new ChunkRaw(len, ChunkHelper.ToBytes(this.Id), alloc);
		}

		public static T CloneChunk<T>(T chunk, ImageInfo info) where T : PngChunk
		{
			PngChunk pngChunk = PngChunk.FactoryFromId(chunk.Id, info);
			if (pngChunk.GetType() != chunk.GetType())
			{
				throw new PngjException(string.Concat(new object[]
				{
					"bad class cloning chunk: ",
					pngChunk.GetType(),
					" ",
					chunk.GetType()
				}));
			}
			pngChunk.CloneDataFromRead(chunk);
			return (T)((object)pngChunk);
		}

		internal void write(Stream os)
		{
			ChunkRaw chunkRaw = this.CreateRawChunk();
			if (chunkRaw == null)
			{
				throw new PngjException("null chunk ! creation failed for " + this);
			}
			chunkRaw.WriteChunk(os);
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"chunk id= ",
				this.Id,
				" (len=",
				this.Length,
				" off=",
				this.Offset,
				") c=",
				base.GetType().Name
			});
		}

		public abstract ChunkRaw CreateRawChunk();

		public abstract void ParseFromRaw(ChunkRaw c);

		public abstract void CloneDataFromRead(PngChunk other);

		public abstract bool AllowsMultiple();

		public abstract PngChunk.ChunkOrderingConstraint GetOrderingConstraint();

		public readonly string Id;

		public readonly bool Crit;

		public readonly bool Pub;

		public readonly bool Safe;

		protected readonly ImageInfo ImgInfo;

		private static Dictionary<string, Type> factoryMap = PngChunk.initFactory();

		public enum ChunkOrderingConstraint
		{
			NONE,
			BEFORE_PLTE_AND_IDAT,
			AFTER_PLTE_BEFORE_IDAT,
			BEFORE_IDAT,
			NA
		}
	}
}
