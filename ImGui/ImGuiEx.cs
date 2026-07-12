using System;
using System.Collections.Generic;
using ImGuiNET;
using ImGuiObjectDrawer;
using UnityEngine;

public static class ImGuiEx
{
	public static bool InputIntRange(string label, ref int v, int v_min, int v_max)
	{
		bool flag = ImGui.InputInt(label, ref v);
		if (flag)
		{
			if (v < v_min)
			{
				v = v_min;
			}
			if (v > v_max)
			{
				v = v_max;
			}
		}
		return flag;
	}

	public static void Image(Texture2D tex, Vector2 size)
	{
		ImGui.Image(ImGuiEx.GetTextureId(tex), size);
	}

	public static bool ImageButton(Sprite sprite, Vector2 size)
	{
		Vector2 vector = new Vector2(sprite.textureRect.xMin / (float)sprite.texture.width, 1f - sprite.textureRect.yMax / (float)sprite.texture.height);
		Vector2 vector2 = new Vector2(sprite.textureRect.xMax / (float)sprite.texture.width, 1f - sprite.textureRect.yMin / (float)sprite.texture.height);
		return ImGui.ImageButton(ImGuiEx.GetTextureId(sprite.texture), size, vector, vector2);
	}

	public static IntPtr GetTextureId(Texture2D tex)
	{
		IntPtr intPtr;
		if (!ImGuiEx.ImguiTextureIds.TryGetValue(tex, out intPtr))
		{
			intPtr = (ImGuiEx.ImguiTextureIds[tex] = ImGuiRenderer.GetInstance().BindTexture(tex, true));
		}
		return intPtr;
	}

	public static bool InputFilter(string label, ref string filter, uint maxCharacterCount = 50U)
	{
		bool flag = false;
		if (ImGui.Button("X"))
		{
			filter = "";
			flag = true;
		}
		ImGuiEx.TooltipForPrevious("Clear filter");
		ImGui.SameLine();
		return flag | ImGui.InputText(label, ref filter, maxCharacterCount);
	}

	public static void TooltipForPrevious(string help)
	{
		if (ImGui.IsItemHovered())
		{
			ImGui.BeginTooltip();
			ImGui.SetTooltip(help);
			ImGui.EndTooltip();
		}
	}

	public static bool Button(string txt, bool enabled)
	{
		if (!enabled)
		{
			ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.6f, 0.6f, 0.6f, 1f));
			ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.2f, 0.2f, 1f));
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.2f, 0.2f, 0.2f, 1f));
			ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.2f, 0.2f, 0.2f, 1f));
			ImGui.Button(txt);
			ImGui.PopStyleColor(4);
			return false;
		}
		return ImGui.Button(txt);
	}

	public static bool MenuItem(string txt, bool enabled)
	{
		if (!enabled)
		{
			ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.6f, 0.6f, 0.6f, 1f));
			ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.2f, 0.2f, 1f));
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.2f, 0.2f, 0.2f, 1f));
			ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.2f, 0.2f, 0.2f, 1f));
			ImGui.MenuItem(txt);
			ImGui.PopStyleColor(4);
			return false;
		}
		return ImGui.MenuItem(txt);
	}

	private static string GetDefaultDebugDrawObjectName(object obj)
	{
		if (obj == null)
		{
			return "Debugging a null value";
		}
		return string.Format("Debugging a value of type {0}", obj.GetType());
	}

	public static void DrawObject(object obj, MemberDrawContext? context = null)
	{
		ImGuiEx.DrawObject(ImGuiEx.GetDefaultDebugDrawObjectName(obj), obj, context);
	}

	public static void DrawObject(string name, object obj, MemberDrawContext? context = null)
	{
		if (context == null)
		{
			context = new MemberDrawContext?(new MemberDrawContext(false, true));
		}
		MemberDrawContext value = context.Value;
		MemberDetails memberDetails = new MemberDetails(name, (obj == null) ? typeof(object) : obj.GetType(), obj);
		ImGuiEx.Internal_DrawMember(in value, in memberDetails, 0);
	}

	public static void Internal_DrawMember(in MemberDrawContext context, in MemberDetails member, int depth)
	{
		MemberDrawerCollection.DrawFor(in context, in member, depth);
	}

	public static void SimpleField(string field_name, object field_object)
	{
		ImGuiEx.SimpleField(field_name, field_object.ToString());
	}

	public static void SimpleField(string field_name, string field_value)
	{
		if (25 < field_name.Length)
		{
			field_name = field_name.Substring(0, 25);
		}
		ImGui.Text(string.Format("{0,-26} {1}", field_name + ":", field_value));
	}

	public static void DrawObjectTable<T>(string tableId, IEnumerable<T> objList, ImGuiTableFlags? overrideFlags = null)
	{
		ImGuiEx.DrawObjectTable<T>(tableId, objList.GetEnumerator(), overrideFlags);
	}

	public static void DrawObjectTable<T>(string tableId, IEnumerator<T> objListIterator, ImGuiTableFlags? overrideFlags = null)
	{
		ImGuiTableFlags imGuiTableFlags;
		if (overrideFlags != null)
		{
			imGuiTableFlags = overrideFlags.Value;
		}
		else
		{
			imGuiTableFlags = ImGuiTableFlags.Reorderable | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInnerH | ImGuiTableFlags.BordersOuterH | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.BordersOuterV | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY;
		}
		ImGuiObjectTableDrawer<T>.New().Id(tableId).ColumnsFromType()
			.Flags(imGuiTableFlags)
			.Build()
			.Draw(objListIterator);
	}

	private static Dictionary<Texture2D, IntPtr> ImguiTextureIds = new Dictionary<Texture2D, IntPtr>();

	public const ImGuiTableFlags DEFAULT_TABLE_FLAGS = ImGuiTableFlags.Reorderable | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInnerH | ImGuiTableFlags.BordersOuterH | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.BordersOuterV | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY;
}
