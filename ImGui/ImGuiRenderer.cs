using System;
using System.Collections.Generic;
using ImGuiNET;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ImGuiRenderer : MonoBehaviour
{
	public static ImGuiRenderer GetInstance()
	{
		return ImGuiRenderer._instance;
	}

	private ImGuiRenderer()
	{
		ImGuiRenderer._instance = this;
	}

	private void Init()
	{
		if (!ImGuiRenderer.imgui_initialized)
		{
			ImGuiRenderer.imgui_initialized = true;
			this.local_init = true;
			KImGuiUtil.SetKAssertCB(new KImGuiUtil.KAssertCB(KImGuiUtil.KAssertHandler));
			ImGui.SetCurrentContext(ImGui.CreateContext());
			ImGui.GetIO().Fonts.AddFontDefault();
			this.CreateDeviceResources();
			this.SetupInput();
		}
	}

	private void OnEnable()
	{
		this.CreateTargetTexture();
		RawImage component = base.GetComponent<RawImage>();
		component.texture = this.tempRenderTexture;
		component.enabled = true;
	}

	private void Cleanup()
	{
		if (ImGuiRenderer.imgui_initialized && this.local_init)
		{
			ImGui.DestroyContext();
		}
	}

	private void OnDisable()
	{
		if (this.tempRenderTexture != null)
		{
			RenderTexture.ReleaseTemporary(this.tempRenderTexture);
			this.tempRenderTexture = null;
		}
		RawImage component = base.GetComponent<RawImage>();
		component.texture = null;
		component.enabled = false;
	}

	private void OnDestroy()
	{
		this.Cleanup();
	}

	public void OnBeforeAssemblyReload()
	{
		this.Cleanup();
	}

	public List<CommandBuffer> GetCommandBuffers()
	{
		return this.commandBuffers;
	}

	private unsafe void BuildFontAtlas()
	{
		ImGuiIOPtr io = ImGui.GetIO();
		byte* ptr;
		int num;
		int num2;
		int num3;
		io.Fonts.GetTexDataAsRGBA32(out ptr, out num, out num2, out num3);
		Texture2D texture2D = new Texture2D(num, num2, TextureFormat.RGBA32, false);
		texture2D.LoadRawTextureData((IntPtr)((void*)ptr), num * num2 * num3);
		texture2D.Apply();
		this.texture_blah = texture2D;
		if (this.fontTextureID != null)
		{
			this.UnbindTexture(this.fontTextureID.Value);
		}
		this.fontTextureID = new IntPtr?(this.BindTexture(texture2D, false));
		io.Fonts.SetTexID(this.fontTextureID.Value);
		io.Fonts.ClearTexData();
	}

	private void CreateDeviceResources()
	{
		this.CreateTargetTexture();
		this.BuildFontAtlas();
		Array values = Enum.GetValues(typeof(KeyCode));
		this.keyCodes = new KeyCode[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			this.keyCodes[i] = (KeyCode)values.GetValue(i);
		}
	}

	private void CreateTargetTexture()
	{
		if (this.tempRenderTexture == null || this.tempRenderTexture.width != Screen.width || this.tempRenderTexture.height != Screen.height)
		{
			if (this.tempRenderTexture)
			{
				this.tempRenderTexture.Release();
				this.tempRenderTexture.width = Screen.width;
				this.tempRenderTexture.height = Screen.height;
				this.tempRenderTexture.Create();
			}
			else
			{
				this.tempRenderTexture = RenderTexture.GetTemporary(new RenderTextureDescriptor(Screen.width, Screen.height, GraphicsFormat.R8G8B8A8_UNorm, 0)
				{
					sRGB = false
				});
				this.tempRenderTexture.name = "IMGUI:RenderTexture";
				this.tempRenderTexture.Create();
			}
			this.material.mainTexture = this.tempRenderTexture;
		}
	}

	public IntPtr BindTexture(Texture2D texture, bool flip_y = true)
	{
		int num = this.textureID;
		this.textureID = num + 1;
		IntPtr intPtr = new IntPtr(num);
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetTexture("_MainTexture", texture);
		materialPropertyBlock.SetInt("_FlipY", flip_y ? 1 : 0);
		this.propertyBlocks.Add(intPtr, materialPropertyBlock);
		return intPtr;
	}

	public void UnbindTexture(IntPtr textureId)
	{
		this.propertyBlocks.Remove(textureId);
	}

	public unsafe void NewFrame()
	{
		if (!ImGuiRenderer.imgui_initialized)
		{
			this.Init();
		}
		ImGuiIOPtr io = ImGui.GetIO();
		*io.DeltaTime = Mathf.Max(Time.unscaledDeltaTime, 1E-14f);
		this.UpdateInput();
		ImGui.NewFrame();
		foreach (KeyCode keyCode in this.keyCodes)
		{
			if (Input.GetKeyDown(keyCode))
			{
				if (KeyCode.A <= keyCode && keyCode <= KeyCode.Z)
				{
					char c = (char)(keyCode - KeyCode.A);
					char c2 = ((*io.KeyShift) ? 'A' : 'a');
					io.AddInputCharacter((uint)((ushort)(c2 + c)));
				}
				else if (KeyCode.Alpha0 <= keyCode && keyCode <= KeyCode.Alpha9)
				{
					char c3 = (char)(keyCode - KeyCode.Alpha0);
					char c4 = '0';
					if (!(*io.KeyShift))
					{
						io.AddInputCharacter((uint)((ushort)(c4 + c3)));
					}
					else
					{
						char c5 = '\0';
						switch (keyCode)
						{
						case KeyCode.Alpha0:
							c5 = ')';
							break;
						case KeyCode.Alpha1:
							c5 = '!';
							break;
						case KeyCode.Alpha2:
							c5 = '@';
							break;
						case KeyCode.Alpha3:
							c5 = '#';
							break;
						case KeyCode.Alpha4:
							c5 = '$';
							break;
						case KeyCode.Alpha5:
							c5 = '%';
							break;
						case KeyCode.Alpha6:
							c5 = '^';
							break;
						case KeyCode.Alpha7:
							c5 = '&';
							break;
						case KeyCode.Alpha8:
							c5 = '*';
							break;
						case KeyCode.Alpha9:
							c5 = '(';
							break;
						}
						io.AddInputCharacter((uint)c5);
					}
				}
				else if (KeyCode.Keypad0 <= keyCode && keyCode <= KeyCode.Keypad9)
				{
					char c6 = (char)(keyCode - KeyCode.Keypad0);
					char c7 = '0';
					io.AddInputCharacter((uint)((ushort)(c7 + c6)));
				}
				else
				{
					foreach (ImGuiRenderer.KeyPairs keyPairs in ImGuiRenderer.keyPairs)
					{
						if (keyCode == keyPairs.key)
						{
							char c8 = ((*io.KeyShift) ? keyPairs.shiftChr : keyPairs.chr);
							io.AddInputCharacter((uint)c8);
						}
					}
				}
			}
		}
	}

	public void EndFrame()
	{
		if (!ImGuiRenderer.imgui_initialized)
		{
			return;
		}
		ImGui.Render();
		this.RenderDrawData(ImGui.GetDrawData());
	}

	private unsafe void SetupInput()
	{
		RangeAccessor<int> keyMap = ImGui.GetIO().KeyMap;
		this.keys.Add(*keyMap[0] = 9);
		this.keys.Add(*keyMap[1] = 276);
		this.keys.Add(*keyMap[2] = 275);
		this.keys.Add(*keyMap[3] = 273);
		this.keys.Add(*keyMap[4] = 274);
		this.keys.Add(*keyMap[5] = 280);
		this.keys.Add(*keyMap[6] = 281);
		this.keys.Add(*keyMap[7] = 278);
		this.keys.Add(*keyMap[8] = 279);
		this.keys.Add(*keyMap[9] = 277);
		this.keys.Add(*keyMap[10] = 127);
		this.keys.Add(*keyMap[11] = 8);
		this.keys.Add(*keyMap[13] = 13);
		this.keys.Add(*keyMap[14] = 27);
		this.keys.Add(*keyMap[16] = 97);
		this.keys.Add(*keyMap[17] = 99);
		this.keys.Add(*keyMap[18] = 118);
		this.keys.Add(*keyMap[19] = 120);
		this.keys.Add(*keyMap[20] = 121);
		this.keys.Add(*keyMap[21] = 122);
	}

	private unsafe void UpdateInput()
	{
		ImGuiIOPtr io = ImGui.GetIO();
		RangeAccessor<bool> keysDown = io.KeysDown;
		for (int i = 0; i < this.keys.Count; i++)
		{
			KeyCode keyCode = (KeyCode)this.keys[i];
			*keysDown[(int)keyCode] = Input.GetKey(keyCode);
		}
		*io.KeyShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		*io.KeyCtrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		*io.KeyAlt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		*io.KeySuper = Input.GetKey(KeyCode.LeftWindows) || Input.GetKey(KeyCode.RightWindows);
		*io.DisplaySize = new Vector2((float)Screen.width, (float)Screen.height);
		*io.DisplayFramebufferScale = this.scale;
		*io.MousePos = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		RangeAccessor<bool> mouseDown = io.MouseDown;
		*mouseDown[0] = Input.GetMouseButton(0);
		*mouseDown[1] = Input.GetMouseButton(1);
		*mouseDown[2] = Input.GetMouseButton(2);
		*io.MouseWheel = Input.mouseScrollDelta.y;
	}

	private unsafe void RenderDrawData(ImDrawDataPtr drawData)
	{
		drawData.ScaleClipRects(*ImGui.GetIO().DisplayFramebufferScale);
		this.UpdateBuffers(drawData);
		this.RenderCommandLists(drawData);
	}

	private Color32 CreateColorFromUInt(uint c)
	{
		return new Color32((byte)(255U & c), (byte)(255U & (c >> 8)), (byte)(255U & (c >> 16)), (byte)(255U & (c >> 24)));
	}

	public static uint ImGuiUIntColor(Color32 c)
	{
		return (uint)((int)c.r | ((int)c.g << 8) | ((int)c.b << 16) | ((int)c.a << 24));
	}

	private unsafe void UpdateBuffers(ImDrawDataPtr drawData)
	{
		if (*drawData.TotalVtxCount == 0)
		{
			return;
		}
		foreach (Mesh mesh in this.meshes)
		{
			global::UnityEngine.Object.Destroy(mesh);
		}
		this.meshes.Clear();
		for (int i = 0; i < *drawData.CmdListsCount; i++)
		{
			ImDrawListPtr imDrawListPtr = drawData.CmdListsRange[i];
			ImDrawVert* ptr = (ImDrawVert*)(void*)imDrawListPtr.VtxBuffer.Data;
			short* ptr2 = (short*)(void*)imDrawListPtr.IdxBuffer.Data;
			this.vertices.Clear();
			this.uvs.Clear();
			this.colours.Clear();
			Mesh mesh2 = new Mesh();
			mesh2.name = "ImGui";
			mesh2.indexFormat = IndexFormat.UInt32;
			mesh2.subMeshCount = imDrawListPtr.CmdBuffer.Size;
			this.meshes.Add(mesh2);
			for (int j = 0; j < imDrawListPtr.VtxBuffer.Size; j++)
			{
				ImDrawVert imDrawVert = ptr[j];
				this.vertices.Add(new Vector3(imDrawVert.pos.x, imDrawVert.pos.y, 0f));
				this.colours.Add(this.CreateColorFromUInt(imDrawVert.col));
				this.uvs.Add(imDrawVert.uv);
			}
			mesh2.SetVertices(this.vertices);
			mesh2.SetColors(this.colours);
			mesh2.SetUVs(0, this.uvs);
			int num = 0;
			for (int k = 0; k < imDrawListPtr.CmdBuffer.Size; k++)
			{
				this.indices.Clear();
				uint num2 = *imDrawListPtr.CmdBuffer[k].ElemCount;
				int num3 = 0;
				while ((long)num3 < (long)((ulong)num2))
				{
					short num4 = ptr2[num + num3];
					this.indices.Add((int)num4);
					num3++;
				}
				num += (int)num2;
				mesh2.SetTriangles(this.indices, k);
			}
		}
	}

	private unsafe void RenderCommandLists(ImDrawDataPtr drawData)
	{
		foreach (CommandBuffer commandBuffer in this.commandBuffers)
		{
			commandBuffer.Dispose();
		}
		this.commandBuffers.Clear();
		CommandBuffer commandBuffer2 = new CommandBuffer();
		commandBuffer2.name = "ImGui";
		this.CreateTargetTexture();
		commandBuffer2.SetRenderTarget(this.tempRenderTexture);
		commandBuffer2.ClearRenderTarget(true, true, Color.clear);
		Matrix4x4 matrix4x = Matrix4x4.Ortho(0f, (float)Screen.width, (float)Screen.height, 0f, -10f, 10f);
		commandBuffer2.SetViewport(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
		commandBuffer2.SetViewProjectionMatrices(Matrix4x4.identity, matrix4x);
		int height = Screen.height;
		for (int i = 0; i < *drawData.CmdListsCount; i++)
		{
			ImPtrVector<ImDrawCmdPtr> cmdBuffer = drawData.CmdListsRange[i].CmdBuffer;
			Mesh mesh = this.meshes[i];
			for (int j = 0; j < cmdBuffer.Size; j++)
			{
				ImDrawCmdPtr imDrawCmdPtr = cmdBuffer[j];
				if (!this.propertyBlocks.ContainsKey(*imDrawCmdPtr.TextureId))
				{
					throw new InvalidOperationException(string.Format("Could not find a texture with id '{0}', please check your bindings", *imDrawCmdPtr.TextureId));
				}
				float num = imDrawCmdPtr.ClipRect.w - imDrawCmdPtr.ClipRect.y;
				Rect rect = new Rect(imDrawCmdPtr.ClipRect.x, (float)height - imDrawCmdPtr.ClipRect.y - num, imDrawCmdPtr.ClipRect.z - imDrawCmdPtr.ClipRect.x, num);
				MaterialPropertyBlock materialPropertyBlock = this.propertyBlocks[*imDrawCmdPtr.TextureId];
				commandBuffer2.EnableScissorRect(rect);
				commandBuffer2.DrawMesh(mesh, Matrix4x4.identity, this.material, j, 0, materialPropertyBlock);
				commandBuffer2.DisableScissorRect();
			}
		}
		Graphics.ExecuteCommandBuffer(commandBuffer2);
		this.commandBuffers.Add(commandBuffer2);
	}

	[SerializeField]
	private Material material;

	[SerializeField]
	private Vector2 scale = Vector2.one;

	private static ImGuiRenderer _instance;

	private RenderTexture tempRenderTexture;

	private Dictionary<IntPtr, MaterialPropertyBlock> propertyBlocks = new Dictionary<IntPtr, MaterialPropertyBlock>();

	private int textureID = 1;

	private IntPtr? fontTextureID;

	private List<int> keys = new List<int>();

	private List<Mesh> meshes = new List<Mesh>(512);

	private List<Vector3> vertices = new List<Vector3>(65535);

	private List<Vector2> uvs = new List<Vector2>(65535);

	private List<Color32> colours = new List<Color32>(65535);

	private List<int> indices = new List<int>(65535);

	private static bool imgui_initialized = false;

	private bool local_init;

	private List<CommandBuffer> commandBuffers = new List<CommandBuffer>(1024);

	private KeyCode[] keyCodes;

	private Texture2D texture_blah;

	private static readonly ImGuiRenderer.KeyPairs[] keyPairs = new ImGuiRenderer.KeyPairs[]
	{
		new ImGuiRenderer.KeyPairs(KeyCode.Space, ' ', ' '),
		new ImGuiRenderer.KeyPairs(KeyCode.BackQuote, '`', '~'),
		new ImGuiRenderer.KeyPairs(KeyCode.Quote, '\'', '"'),
		new ImGuiRenderer.KeyPairs(KeyCode.Minus, '-', '_'),
		new ImGuiRenderer.KeyPairs(KeyCode.Equals, '=', '+'),
		new ImGuiRenderer.KeyPairs(KeyCode.LeftBracket, '[', '{'),
		new ImGuiRenderer.KeyPairs(KeyCode.RightBracket, ']', '}'),
		new ImGuiRenderer.KeyPairs(KeyCode.Comma, ',', '<'),
		new ImGuiRenderer.KeyPairs(KeyCode.Period, '.', '>'),
		new ImGuiRenderer.KeyPairs(KeyCode.Slash, '/', '?'),
		new ImGuiRenderer.KeyPairs(KeyCode.Backslash, '\\', '|'),
		new ImGuiRenderer.KeyPairs(KeyCode.Semicolon, ';', ':'),
		new ImGuiRenderer.KeyPairs(KeyCode.KeypadPeriod, '.', '\0')
	};

	private const CameraEvent ImGuiCameraEvt = CameraEvent.AfterEverything;

	private struct KeyPairs
	{
		public KeyPairs(KeyCode key, char chr, char shiftChr)
		{
			this.key = key;
			this.chr = chr;
			this.shiftChr = shiftChr;
		}

		public KeyCode key;

		public char chr;

		public char shiftChr;
	}
}
