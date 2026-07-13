using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR
{
	internal class EntryRecorder
	{
		public EntryRecorder(EntryPool entryPool)
		{
			Debug.Assert(entryPool != null);
			this.m_EntryPool = entryPool;
		}

		public void DrawMesh(Entry parentEntry, NativeSlice<Vertex> vertices, NativeSlice<ushort> indices)
		{
			this.DrawMesh(parentEntry, vertices, indices, null, TextureOptions.None);
		}

		public void DrawMesh(Entry parentEntry, NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture, TextureOptions textureOptions = TextureOptions.None)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.vertices = vertices;
			entry.indices = indices;
			entry.texture = texture;
			entry.flags = (((textureOptions & TextureOptions.PremultipliedAlpha) != TextureOptions.None) ? EntryFlags.IsPremultiplied : ((EntryFlags)0));
			bool flag = texture == null;
			if (flag)
			{
				entry.type = EntryType.DrawSolidMesh;
			}
			else
			{
				bool flag2 = (entry.flags & EntryFlags.IsPremultiplied) != (EntryFlags)0 || (textureOptions & TextureOptions.SkipDynamicAtlas) > TextureOptions.None;
				entry.type = (flag2 ? EntryType.DrawTexturedMeshSkipAtlas : EntryType.DrawTexturedMesh);
			}
			EntryRecorder.AppendMeshEntry(parentEntry, entry);
		}

		public void DrawMesh(Entry parentEntry, NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, TextureId textureId, bool isPremultiplied = false)
		{
			Debug.Assert(textureId.IsValid());
			Entry entry = this.m_EntryPool.Get();
			entry.vertices = vertices;
			entry.indices = indices;
			entry.textureId = textureId;
			entry.flags = (isPremultiplied ? EntryFlags.IsPremultiplied : ((EntryFlags)0));
			entry.type = EntryType.DrawDynamicTexturedMesh;
			EntryRecorder.AppendMeshEntry(parentEntry, entry);
		}

		public void DrawRasterText(Entry parentEntry, NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture, bool multiChannel)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = (multiChannel ? EntryType.DrawTexturedMeshSkipAtlas : EntryType.DrawTextMesh);
			entry.flags = EntryFlags.UsesTextCoreSettings;
			entry.vertices = vertices;
			entry.indices = indices;
			entry.texture = texture;
			entry.textScale = 0f;
			entry.fontSharpness = 0f;
			EntryRecorder.AppendMeshEntry(parentEntry, entry);
		}

		public void DrawSdfText(Entry parentEntry, NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture, float scale, float sharpness)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.DrawTextMesh;
			entry.flags = EntryFlags.UsesTextCoreSettings;
			entry.vertices = vertices;
			entry.indices = indices;
			entry.texture = texture;
			entry.textScale = scale;
			entry.fontSharpness = sharpness;
			EntryRecorder.AppendMeshEntry(parentEntry, entry);
		}

		public void DrawGradients(Entry parentEntry, NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, VectorImage gradientsOwner)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.DrawGradients;
			entry.vertices = vertices;
			entry.indices = indices;
			entry.gradientsOwner = gradientsOwner;
			EntryRecorder.AppendMeshEntry(parentEntry, entry);
		}

		public void DrawImmediate(Entry parentEntry, Action callback, bool cullingEnabled)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = (cullingEnabled ? EntryType.DrawImmediateCull : EntryType.DrawImmediate);
			entry.immediateCallback = callback;
			EntryRecorder.Append(parentEntry, entry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DrawChildren(Entry parentEntry)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.DrawChildren;
			EntryRecorder.Append(parentEntry, entry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void BeginStencilMask(Entry parentEntry)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.BeginStencilMask;
			EntryRecorder.Append(parentEntry, entry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EndStencilMask(Entry parentEntry)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.EndStencilMask;
			EntryRecorder.Append(parentEntry, entry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopStencilMask(Entry parentEntry)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.PopStencilMask;
			EntryRecorder.Append(parentEntry, entry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PushClippingRect(Entry parentEntry)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.PushClippingRect;
			EntryRecorder.Append(parentEntry, entry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopClippingRect(Entry parentEntry)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.PopClippingRect;
			EntryRecorder.Append(parentEntry, entry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PushScissors(Entry parentEntry)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.PushScissors;
			EntryRecorder.Append(parentEntry, entry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopScissors(Entry parentEntry)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.PopScissors;
			EntryRecorder.Append(parentEntry, entry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PushGroupMatrix(Entry parentEntry)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.PushGroupMatrix;
			EntryRecorder.Append(parentEntry, entry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopGroupMatrix(Entry parentEntry)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.PopGroupMatrix;
			EntryRecorder.Append(parentEntry, entry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PushDefaultMaterial(Entry parentEntry, MaterialDefinition matDef)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.PushDefaultMaterial;
			entry.material = matDef.material;
			entry.userProps = matDef.BuildPropertyBlock();
			EntryRecorder.Append(parentEntry, entry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopDefaultMaterial(Entry parentEntry)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.PopDefaultMaterial;
			EntryRecorder.Append(parentEntry, entry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CutRenderChain(Entry parentEntry)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.CutRenderChain;
			EntryRecorder.Append(parentEntry, entry);
		}

		public Entry InsertPlaceholder(Entry parentEntry)
		{
			Entry entry = this.m_EntryPool.Get();
			entry.type = EntryType.DedicatedPlaceholder;
			EntryRecorder.Append(parentEntry, entry);
			return entry;
		}

		private static void AppendMeshEntry(Entry parentEntry, Entry entry)
		{
			int length = entry.vertices.Length;
			int length2 = entry.indices.Length;
			bool flag = length == 0;
			if (flag)
			{
				Debug.LogError("Attempting to add an entry without vertices.");
			}
			else
			{
				bool flag2 = (long)length > (long)((ulong)UIRenderDevice.maxVerticesPerPage);
				if (flag2)
				{
					Debug.LogError(string.Format("Attempting to add an entry with {0} vertices. The maximum number of vertices per entry is {1}.", length, UIRenderDevice.maxVerticesPerPage));
				}
				else
				{
					bool flag3 = length2 == 0;
					if (flag3)
					{
						Debug.LogError("Attempting to add an entry without indices.");
					}
					else
					{
						EntryRecorder.Append(parentEntry, entry);
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Append(Entry parentEntry, Entry entry)
		{
			bool flag = parentEntry.lastChild == null;
			if (flag)
			{
				Debug.Assert(parentEntry.firstChild == null);
				parentEntry.firstChild = entry;
				parentEntry.lastChild = entry;
			}
			else
			{
				parentEntry.lastChild.nextSibling = entry;
				parentEntry.lastChild = entry;
			}
		}

		private EntryPool m_EntryPool;
	}
}
