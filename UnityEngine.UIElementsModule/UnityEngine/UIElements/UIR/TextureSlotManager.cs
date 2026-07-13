using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements.UIR
{
	internal class TextureSlotManager
	{
		static TextureSlotManager()
		{
			for (int i = 0; i < TextureSlotManager.k_MaxSlotCount; i++)
			{
				TextureSlotManager.slotIds[i] = Shader.PropertyToID(string.Format("_Texture{0}", i));
			}
		}

		public TextureSlotManager()
		{
			this.m_Textures = new TextureId[TextureSlotManager.k_MaxSlotCount];
			this.m_LastUseTime = new int[TextureSlotManager.k_MaxSlotCount];
			this.m_GpuTextures = new Vector4[TextureSlotManager.k_MaxSlotCount * TextureSlotManager.k_SlotSize];
			this.m_SlotCount = TextureSlotManager.k_MaxSlotCount;
			this.FreeSlots = TextureSlotManager.k_MaxSlotCount;
			this.Reset();
		}

		public void Reset()
		{
			this.m_CurrentTime = 0;
			this.m_BatchTime = 0;
			this.Unbind(0, TextureSlotManager.k_MaxSlotCount);
		}

		private void Unbind(int first, int count = 1)
		{
			for (int i = first; i < first + count; i++)
			{
				this.m_Textures[i] = TextureId.invalid;
				this.m_LastUseTime[i] = -1;
				this.SetGpuData(i, TextureId.invalid, 1, 1, 0f, 0f, false);
			}
		}

		public void StartNewBatch(int slotCount)
		{
			bool flag = slotCount < this.m_SlotCount;
			if (flag)
			{
				this.Unbind(slotCount, this.m_SlotCount - slotCount);
			}
			int num = this.m_CurrentTime + 1;
			this.m_CurrentTime = num;
			this.m_BatchTime = num;
			this.m_SlotCount = slotCount;
			this.FreeSlots = slotCount;
		}

		public int IndexOf(TextureId id)
		{
			for (int i = 0; i < this.m_SlotCount; i++)
			{
				bool flag = this.m_Textures[i].index == id.index;
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void MarkUsed(int slotIndex)
		{
			int num = this.m_LastUseTime[slotIndex];
			bool flag = num < this.m_BatchTime;
			int num2;
			if (flag)
			{
				num2 = this.FreeSlots - 1;
				this.FreeSlots = num2;
			}
			int[] lastUseTime = this.m_LastUseTime;
			num2 = this.m_CurrentTime + 1;
			this.m_CurrentTime = num2;
			lastUseTime[slotIndex] = num2;
		}

		public int FreeSlots { get; private set; }

		public int FindOldestSlot()
		{
			int num = this.m_LastUseTime[0];
			int num2 = 0;
			for (int i = 1; i < this.m_SlotCount; i++)
			{
				bool flag = this.m_LastUseTime[i] < num;
				if (flag)
				{
					num = this.m_LastUseTime[i];
					num2 = i;
				}
			}
			return num2;
		}

		public void Bind(TextureId id, float sdfScale, float sharpness, bool isPremultiplied, int slot, MaterialPropertyBlock mat, CommandList commandList = null)
		{
			Texture texture = this.textureRegistry.GetTexture(id);
			bool flag = texture == null;
			if (flag)
			{
				texture = Texture2D.whiteTexture;
			}
			this.m_Textures[slot] = id;
			this.MarkUsed(slot);
			this.SetGpuData(slot, id, texture.width, texture.height, sdfScale, sharpness, isPremultiplied);
			bool flag2 = commandList == null;
			if (flag2)
			{
				mat.SetTexture(TextureSlotManager.slotIds[slot], texture);
				mat.SetVectorArray(TextureSlotManager.textureTableId, this.m_GpuTextures);
			}
			else
			{
				int num = slot * TextureSlotManager.k_SlotSize;
				commandList.SetTexture(TextureSlotManager.slotIds[slot], texture, num, this.m_GpuTextures[num], this.m_GpuTextures[num + 1]);
			}
		}

		public void SetGpuData(int slotIndex, TextureId id, int textureWidth, int textureHeight, float sdfScale, float sharpness, bool isPremultiplied)
		{
			int num = slotIndex * TextureSlotManager.k_SlotSize;
			float num2 = 1f / (float)textureWidth;
			float num3 = 1f / (float)textureHeight;
			this.m_GpuTextures[num] = new Vector4(id.ConvertToGpu(), num2, num3, sdfScale);
			this.m_GpuTextures[num + 1] = new Vector4((float)textureWidth, (float)textureHeight, sharpness, isPremultiplied ? 1f : 0f);
		}

		internal static readonly int k_MaxSlotCount = 8;

		internal static readonly int k_SlotSize = 2;

		internal static int[] slotIds = new int[TextureSlotManager.k_MaxSlotCount];

		internal static readonly int textureTableId = Shader.PropertyToID("_TextureInfo");

		private TextureId[] m_Textures;

		private int[] m_LastUseTime;

		private int m_CurrentTime;

		private int m_BatchTime;

		private Vector4[] m_GpuTextures;

		private int m_SlotCount;

		internal TextureRegistry textureRegistry = TextureRegistry.instance;
	}
}
