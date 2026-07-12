using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace ImGuiNET
{
	public static class ImGui
	{
		public unsafe static ImGuiPayloadPtr AcceptDragDropPayload(string type)
		{
			int num = 0;
			byte* ptr;
			if (type != null)
			{
				num = Encoding.UTF8.GetByteCount(type);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(type, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiDragDropFlags imGuiDragDropFlags = ImGuiDragDropFlags.None;
			ImGuiPayload* ptr2 = ImGuiNative.igAcceptDragDropPayload(ptr, imGuiDragDropFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return new ImGuiPayloadPtr(ptr2);
		}

		public unsafe static ImGuiPayloadPtr AcceptDragDropPayload(string type, ImGuiDragDropFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (type != null)
			{
				num = Encoding.UTF8.GetByteCount(type);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(type, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiPayload* ptr2 = ImGuiNative.igAcceptDragDropPayload(ptr, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return new ImGuiPayloadPtr(ptr2);
		}

		public static void AlignTextToFramePadding()
		{
			ImGuiNative.igAlignTextToFramePadding();
		}

		public unsafe static bool ArrowButton(string str_id, ImGuiDir dir)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igArrowButton(ptr, dir);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool Begin(string name)
		{
			int num = 0;
			byte* ptr;
			if (name != null)
			{
				num = Encoding.UTF8.GetByteCount(name);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(name, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte* ptr2 = null;
			ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.None;
			int num2 = (int)ImGuiNative.igBegin(ptr, ptr2, imGuiWindowFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool Begin(string name, ref bool p_open)
		{
			int num = 0;
			byte* ptr;
			if (name != null)
			{
				num = Encoding.UTF8.GetByteCount(name);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(name, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (p_open ? 1 : 0);
			byte* ptr2 = &b;
			ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.None;
			int num2 = (int)ImGuiNative.igBegin(ptr, ptr2, imGuiWindowFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			p_open = b > 0;
			return num2 != 0;
		}

		public unsafe static bool Begin(string name, ref bool p_open, ImGuiWindowFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (name != null)
			{
				num = Encoding.UTF8.GetByteCount(name);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(name, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (p_open ? 1 : 0);
			byte* ptr2 = &b;
			int num2 = (int)ImGuiNative.igBegin(ptr, ptr2, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			p_open = b > 0;
			return num2 != 0;
		}

		public unsafe static bool BeginChild(string str_id)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			Vector2 vector = default(Vector2);
			byte b = 0;
			ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.None;
			int num2 = (int)ImGuiNative.igBeginChildStr(ptr, vector, b, imGuiWindowFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginChild(string str_id, Vector2 size)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = 0;
			ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.None;
			int num2 = (int)ImGuiNative.igBeginChildStr(ptr, size, b, imGuiWindowFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginChild(string str_id, Vector2 size, bool border)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (border ? 1 : 0);
			ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.None;
			int num2 = (int)ImGuiNative.igBeginChildStr(ptr, size, b, imGuiWindowFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginChild(string str_id, Vector2 size, bool border, ImGuiWindowFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (border ? 1 : 0);
			int num2 = (int)ImGuiNative.igBeginChildStr(ptr, size, b, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public static bool BeginChild(uint id)
		{
			Vector2 vector = default(Vector2);
			byte b = 0;
			ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.None;
			return ImGuiNative.igBeginChildID(id, vector, b, imGuiWindowFlags) > 0;
		}

		public static bool BeginChild(uint id, Vector2 size)
		{
			byte b = 0;
			ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.None;
			return ImGuiNative.igBeginChildID(id, size, b, imGuiWindowFlags) > 0;
		}

		public static bool BeginChild(uint id, Vector2 size, bool border)
		{
			byte b = (border ? 1 : 0);
			ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.None;
			return ImGuiNative.igBeginChildID(id, size, b, imGuiWindowFlags) > 0;
		}

		public static bool BeginChild(uint id, Vector2 size, bool border, ImGuiWindowFlags flags)
		{
			byte b = (border ? 1 : 0);
			return ImGuiNative.igBeginChildID(id, size, b, flags) > 0;
		}

		public static bool BeginChildFrame(uint id, Vector2 size)
		{
			ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.None;
			return ImGuiNative.igBeginChildFrame(id, size, imGuiWindowFlags) > 0;
		}

		public static bool BeginChildFrame(uint id, Vector2 size, ImGuiWindowFlags flags)
		{
			return ImGuiNative.igBeginChildFrame(id, size, flags) > 0;
		}

		public unsafe static bool BeginCombo(string label, string preview_value)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (preview_value != null)
			{
				num2 = Encoding.UTF8.GetByteCount(preview_value);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(preview_value, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiComboFlags imGuiComboFlags = ImGuiComboFlags.None;
			int num3 = (int)ImGuiNative.igBeginCombo(ptr, ptr2, imGuiComboFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr2);
			}
			return num3 != 0;
		}

		public unsafe static bool BeginCombo(string label, string preview_value, ImGuiComboFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (preview_value != null)
			{
				num2 = Encoding.UTF8.GetByteCount(preview_value);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(preview_value, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			int num3 = (int)ImGuiNative.igBeginCombo(ptr, ptr2, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr2);
			}
			return num3 != 0;
		}

		public static bool BeginDragDropSource()
		{
			return ImGuiNative.igBeginDragDropSource(ImGuiDragDropFlags.None) > 0;
		}

		public static bool BeginDragDropSource(ImGuiDragDropFlags flags)
		{
			return ImGuiNative.igBeginDragDropSource(flags) > 0;
		}

		public static bool BeginDragDropTarget()
		{
			return ImGuiNative.igBeginDragDropTarget() > 0;
		}

		public static void BeginGroup()
		{
			ImGuiNative.igBeginGroup();
		}

		public unsafe static bool BeginListBox(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igBeginListBox(ptr, default(Vector2));
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginListBox(string label, Vector2 size)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igBeginListBox(ptr, size);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public static bool BeginMainMenuBar()
		{
			return ImGuiNative.igBeginMainMenuBar() > 0;
		}

		public unsafe static bool BeginMenu(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = 1;
			int num2 = (int)ImGuiNative.igBeginMenu(ptr, b);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginMenu(string label, bool enabled)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (enabled ? 1 : 0);
			int num2 = (int)ImGuiNative.igBeginMenu(ptr, b);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public static bool BeginMenuBar()
		{
			return ImGuiNative.igBeginMenuBar() > 0;
		}

		public unsafe static bool BeginPopup(string str_id)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.None;
			int num2 = (int)ImGuiNative.igBeginPopup(ptr, imGuiWindowFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginPopup(string str_id, ImGuiWindowFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igBeginPopup(ptr, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginPopupContextItem()
		{
			byte* ptr = null;
			ImGuiPopupFlags imGuiPopupFlags = ImGuiPopupFlags.MouseButtonRight;
			return ImGuiNative.igBeginPopupContextItem(ptr, imGuiPopupFlags) > 0;
		}

		public unsafe static bool BeginPopupContextItem(string str_id)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiPopupFlags imGuiPopupFlags = ImGuiPopupFlags.MouseButtonRight;
			int num2 = (int)ImGuiNative.igBeginPopupContextItem(ptr, imGuiPopupFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginPopupContextItem(string str_id, ImGuiPopupFlags popup_flags)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igBeginPopupContextItem(ptr, popup_flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginPopupContextVoid()
		{
			byte* ptr = null;
			ImGuiPopupFlags imGuiPopupFlags = ImGuiPopupFlags.MouseButtonRight;
			return ImGuiNative.igBeginPopupContextVoid(ptr, imGuiPopupFlags) > 0;
		}

		public unsafe static bool BeginPopupContextVoid(string str_id)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiPopupFlags imGuiPopupFlags = ImGuiPopupFlags.MouseButtonRight;
			int num2 = (int)ImGuiNative.igBeginPopupContextVoid(ptr, imGuiPopupFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginPopupContextVoid(string str_id, ImGuiPopupFlags popup_flags)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igBeginPopupContextVoid(ptr, popup_flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginPopupContextWindow()
		{
			byte* ptr = null;
			ImGuiPopupFlags imGuiPopupFlags = ImGuiPopupFlags.MouseButtonRight;
			return ImGuiNative.igBeginPopupContextWindow(ptr, imGuiPopupFlags) > 0;
		}

		public unsafe static bool BeginPopupContextWindow(string str_id)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiPopupFlags imGuiPopupFlags = ImGuiPopupFlags.MouseButtonRight;
			int num2 = (int)ImGuiNative.igBeginPopupContextWindow(ptr, imGuiPopupFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginPopupContextWindow(string str_id, ImGuiPopupFlags popup_flags)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igBeginPopupContextWindow(ptr, popup_flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginPopupModal(string name)
		{
			int num = 0;
			byte* ptr;
			if (name != null)
			{
				num = Encoding.UTF8.GetByteCount(name);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(name, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte* ptr2 = null;
			ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.None;
			int num2 = (int)ImGuiNative.igBeginPopupModal(ptr, ptr2, imGuiWindowFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginPopupModal(string name, ref bool p_open)
		{
			int num = 0;
			byte* ptr;
			if (name != null)
			{
				num = Encoding.UTF8.GetByteCount(name);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(name, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (p_open ? 1 : 0);
			byte* ptr2 = &b;
			ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.None;
			int num2 = (int)ImGuiNative.igBeginPopupModal(ptr, ptr2, imGuiWindowFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			p_open = b > 0;
			return num2 != 0;
		}

		public unsafe static bool BeginPopupModal(string name, ref bool p_open, ImGuiWindowFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (name != null)
			{
				num = Encoding.UTF8.GetByteCount(name);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(name, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (p_open ? 1 : 0);
			byte* ptr2 = &b;
			int num2 = (int)ImGuiNative.igBeginPopupModal(ptr, ptr2, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			p_open = b > 0;
			return num2 != 0;
		}

		public unsafe static bool BeginTabBar(string str_id)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiTabBarFlags imGuiTabBarFlags = ImGuiTabBarFlags.None;
			int num2 = (int)ImGuiNative.igBeginTabBar(ptr, imGuiTabBarFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginTabBar(string str_id, ImGuiTabBarFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igBeginTabBar(ptr, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginTabItem(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte* ptr2 = null;
			ImGuiTabItemFlags imGuiTabItemFlags = ImGuiTabItemFlags.None;
			int num2 = (int)ImGuiNative.igBeginTabItem(ptr, ptr2, imGuiTabItemFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool BeginTabItem(string label, ref bool p_open)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (p_open ? 1 : 0);
			byte* ptr2 = &b;
			ImGuiTabItemFlags imGuiTabItemFlags = ImGuiTabItemFlags.None;
			int num2 = (int)ImGuiNative.igBeginTabItem(ptr, ptr2, imGuiTabItemFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			p_open = b > 0;
			return num2 != 0;
		}

		public unsafe static bool BeginTabItem(string label, ref bool p_open, ImGuiTabItemFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (p_open ? 1 : 0);
			byte* ptr2 = &b;
			int num2 = (int)ImGuiNative.igBeginTabItem(ptr, ptr2, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			p_open = b > 0;
			return num2 != 0;
		}

		public unsafe static bool BeginTable(string str_id, int column)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiTableFlags imGuiTableFlags = ImGuiTableFlags.None;
			Vector2 vector = default(Vector2);
			float num2 = 0f;
			int num3 = (int)ImGuiNative.igBeginTable(ptr, column, imGuiTableFlags, vector, num2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num3 != 0;
		}

		public unsafe static bool BeginTable(string str_id, int column, ImGuiTableFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			Vector2 vector = default(Vector2);
			float num2 = 0f;
			int num3 = (int)ImGuiNative.igBeginTable(ptr, column, flags, vector, num2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num3 != 0;
		}

		public unsafe static bool BeginTable(string str_id, int column, ImGuiTableFlags flags, Vector2 outer_size)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 0f;
			int num3 = (int)ImGuiNative.igBeginTable(ptr, column, flags, outer_size, num2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num3 != 0;
		}

		public unsafe static bool BeginTable(string str_id, int column, ImGuiTableFlags flags, Vector2 outer_size, float inner_width)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igBeginTable(ptr, column, flags, outer_size, inner_width);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public static void BeginTooltip()
		{
			ImGuiNative.igBeginTooltip();
		}

		public static void Bullet()
		{
			ImGuiNative.igBullet();
		}

		public unsafe static void BulletText(string fmt)
		{
			int num = 0;
			byte* ptr;
			if (fmt != null)
			{
				num = Encoding.UTF8.GetByteCount(fmt);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(fmt, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igBulletText(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static bool Button(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igButton(ptr, default(Vector2));
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool Button(string label, Vector2 size)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igButton(ptr, size);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public static float CalcItemWidth()
		{
			return ImGuiNative.igCalcItemWidth();
		}

		public static void CaptureKeyboardFromApp()
		{
			ImGuiNative.igCaptureKeyboardFromApp(1);
		}

		public static void CaptureKeyboardFromApp(bool want_capture_keyboard_value)
		{
			ImGuiNative.igCaptureKeyboardFromApp(want_capture_keyboard_value ? 1 : 0);
		}

		public static void CaptureMouseFromApp()
		{
			ImGuiNative.igCaptureMouseFromApp(1);
		}

		public static void CaptureMouseFromApp(bool want_capture_mouse_value)
		{
			ImGuiNative.igCaptureMouseFromApp(want_capture_mouse_value ? 1 : 0);
		}

		public unsafe static bool Checkbox(string label, ref bool v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (v ? 1 : 0);
			byte* ptr2 = &b;
			int num2 = (int)ImGuiNative.igCheckbox(ptr, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			v = b > 0;
			return num2 != 0;
		}

		public unsafe static bool CheckboxFlags(string label, ref int flags, int flags_value)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			fixed (int* ptr2 = &flags)
			{
				int* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igCheckboxFlagsIntPtr(ptr, ptr3, flags_value);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool CheckboxFlags(string label, ref uint flags, uint flags_value)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			fixed (uint* ptr2 = &flags)
			{
				uint* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igCheckboxFlagsUintPtr(ptr, ptr3, flags_value);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public static void CloseCurrentPopup()
		{
			ImGuiNative.igCloseCurrentPopup();
		}

		public unsafe static bool CollapsingHeader(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiTreeNodeFlags imGuiTreeNodeFlags = ImGuiTreeNodeFlags.None;
			int num2 = (int)ImGuiNative.igCollapsingHeaderTreeNodeFlags(ptr, imGuiTreeNodeFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool CollapsingHeader(string label, ImGuiTreeNodeFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igCollapsingHeaderTreeNodeFlags(ptr, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool CollapsingHeader(string label, ref bool p_visible)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (p_visible ? 1 : 0);
			byte* ptr2 = &b;
			ImGuiTreeNodeFlags imGuiTreeNodeFlags = ImGuiTreeNodeFlags.None;
			int num2 = (int)ImGuiNative.igCollapsingHeaderBoolPtr(ptr, ptr2, imGuiTreeNodeFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			p_visible = b > 0;
			return num2 != 0;
		}

		public unsafe static bool CollapsingHeader(string label, ref bool p_visible, ImGuiTreeNodeFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (p_visible ? 1 : 0);
			byte* ptr2 = &b;
			int num2 = (int)ImGuiNative.igCollapsingHeaderBoolPtr(ptr, ptr2, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			p_visible = b > 0;
			return num2 != 0;
		}

		public unsafe static bool ColorButton(string desc_id, Vector4 col)
		{
			int num = 0;
			byte* ptr;
			if (desc_id != null)
			{
				num = Encoding.UTF8.GetByteCount(desc_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(desc_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiColorEditFlags imGuiColorEditFlags = ImGuiColorEditFlags.None;
			int num2 = (int)ImGuiNative.igColorButton(ptr, col, imGuiColorEditFlags, default(Vector2));
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool ColorButton(string desc_id, Vector4 col, ImGuiColorEditFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (desc_id != null)
			{
				num = Encoding.UTF8.GetByteCount(desc_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(desc_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igColorButton(ptr, col, flags, default(Vector2));
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool ColorButton(string desc_id, Vector4 col, ImGuiColorEditFlags flags, Vector2 size)
		{
			int num = 0;
			byte* ptr;
			if (desc_id != null)
			{
				num = Encoding.UTF8.GetByteCount(desc_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(desc_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igColorButton(ptr, col, flags, size);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public static uint ColorConvertFloat4ToU32(Vector4 @in)
		{
			return ImGuiNative.igColorConvertFloat4ToU32(@in);
		}

		public unsafe static void ColorConvertHSVtoRGB(float h, float s, float v, out float out_r, out float out_g, out float out_b)
		{
			fixed (float* ptr = &out_r)
			{
				float* ptr2 = ptr;
				fixed (float* ptr3 = &out_g)
				{
					float* ptr4 = ptr3;
					fixed (float* ptr5 = &out_b)
					{
						float* ptr6 = ptr5;
						ImGuiNative.igColorConvertHSVtoRGB(h, s, v, ptr2, ptr4, ptr6);
					}
				}
			}
		}

		public unsafe static void ColorConvertRGBtoHSV(float r, float g, float b, out float out_h, out float out_s, out float out_v)
		{
			fixed (float* ptr = &out_h)
			{
				float* ptr2 = ptr;
				fixed (float* ptr3 = &out_s)
				{
					float* ptr4 = ptr3;
					fixed (float* ptr5 = &out_v)
					{
						float* ptr6 = ptr5;
						ImGuiNative.igColorConvertRGBtoHSV(r, g, b, ptr2, ptr4, ptr6);
					}
				}
			}
		}

		public unsafe static Vector4 ColorConvertU32ToFloat4(uint @in)
		{
			Vector4 vector;
			ImGuiNative.igColorConvertU32ToFloat4(&vector, @in);
			return vector;
		}

		public unsafe static bool ColorEdit3(string label, ref Vector3 col)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiColorEditFlags imGuiColorEditFlags = ImGuiColorEditFlags.None;
			fixed (Vector3* ptr2 = &col)
			{
				Vector3* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igColorEdit3(ptr, ptr3, imGuiColorEditFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool ColorEdit3(string label, ref Vector3 col, ImGuiColorEditFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			fixed (Vector3* ptr2 = &col)
			{
				Vector3* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igColorEdit3(ptr, ptr3, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool ColorEdit4(string label, ref Vector4 col)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiColorEditFlags imGuiColorEditFlags = ImGuiColorEditFlags.None;
			fixed (Vector4* ptr2 = &col)
			{
				Vector4* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igColorEdit4(ptr, ptr3, imGuiColorEditFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool ColorEdit4(string label, ref Vector4 col, ImGuiColorEditFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			fixed (Vector4* ptr2 = &col)
			{
				Vector4* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igColorEdit4(ptr, ptr3, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool ColorPicker3(string label, ref Vector3 col)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiColorEditFlags imGuiColorEditFlags = ImGuiColorEditFlags.None;
			fixed (Vector3* ptr2 = &col)
			{
				Vector3* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igColorPicker3(ptr, ptr3, imGuiColorEditFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool ColorPicker3(string label, ref Vector3 col, ImGuiColorEditFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			fixed (Vector3* ptr2 = &col)
			{
				Vector3* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igColorPicker3(ptr, ptr3, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool ColorPicker4(string label, ref Vector4 col)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiColorEditFlags imGuiColorEditFlags = ImGuiColorEditFlags.None;
			float* ptr2 = null;
			fixed (Vector4* ptr3 = &col)
			{
				Vector4* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igColorPicker4(ptr, ptr4, imGuiColorEditFlags, ptr2);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool ColorPicker4(string label, ref Vector4 col, ImGuiColorEditFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float* ptr2 = null;
			fixed (Vector4* ptr3 = &col)
			{
				Vector4* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igColorPicker4(ptr, ptr4, flags, ptr2);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool ColorPicker4(string label, ref Vector4 col, ImGuiColorEditFlags flags, ref float ref_col)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			fixed (Vector4* ptr2 = &col)
			{
				Vector4* ptr3 = ptr2;
				fixed (float* ptr4 = &ref_col)
				{
					float* ptr5 = ptr4;
					int num2 = (int)ImGuiNative.igColorPicker4(ptr, ptr3, flags, ptr5);
					if (num > 2048)
					{
						Util.Free(ptr);
					}
					return num2 != 0;
				}
			}
		}

		public unsafe static void Columns()
		{
			int num = 1;
			byte* ptr = null;
			byte b = 1;
			ImGuiNative.igColumns(num, ptr, b);
		}

		public unsafe static void Columns(int count)
		{
			byte* ptr = null;
			byte b = 1;
			ImGuiNative.igColumns(count, ptr, b);
		}

		public unsafe static void Columns(int count, string id)
		{
			int num = 0;
			byte* ptr;
			if (id != null)
			{
				num = Encoding.UTF8.GetByteCount(id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = 1;
			ImGuiNative.igColumns(count, ptr, b);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void Columns(int count, string id, bool border)
		{
			int num = 0;
			byte* ptr;
			if (id != null)
			{
				num = Encoding.UTF8.GetByteCount(id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (border ? 1 : 0);
			ImGuiNative.igColumns(count, ptr, b);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static bool Combo(string label, ref int current_item, string[] items, int items_count)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int* ptr2;
			int num2;
			checked
			{
				ptr2 = stackalloc int[unchecked((UIntPtr)items.Length) * 4];
				num2 = 0;
			}
			for (int i = 0; i < items.Length; i++)
			{
				string text = items[i];
				ptr2[i] = Encoding.UTF8.GetByteCount(text);
				num2 += ptr2[i] + 1;
			}
			byte* ptr3 = stackalloc byte[(UIntPtr)num2];
			int num3 = 0;
			for (int j = 0; j < items.Length; j++)
			{
				string text2 = items[j];
				fixed (string text3 = text2)
				{
					char* ptr4 = text3;
					if (ptr4 != null)
					{
						ptr4 += RuntimeHelpers.OffsetToStringData / 2;
					}
					num3 += Encoding.UTF8.GetBytes(ptr4, text2.Length, ptr3 + num3, ptr2[j]);
					ptr3[num3] = 0;
					num3++;
				}
			}
			byte** ptr5;
			checked
			{
				ptr5 = stackalloc byte*[unchecked((UIntPtr)items.Length) * (UIntPtr)sizeof(byte*)];
				num3 = 0;
			}
			for (int k = 0; k < items.Length; k++)
			{
				*(IntPtr*)(ptr5 + (IntPtr)k * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr3 + num3;
				num3 += ptr2[k] + 1;
			}
			int num4 = -1;
			fixed (int* ptr6 = &current_item)
			{
				int* ptr7 = ptr6;
				int num5 = (int)ImGuiNative.igComboStr_arr(ptr, ptr7, ptr5, items_count, num4);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num5 != 0;
			}
		}

		public unsafe static bool Combo(string label, ref int current_item, string[] items, int items_count, int popup_max_height_in_items)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int* ptr2;
			int num2;
			checked
			{
				ptr2 = stackalloc int[unchecked((UIntPtr)items.Length) * 4];
				num2 = 0;
			}
			for (int i = 0; i < items.Length; i++)
			{
				string text = items[i];
				ptr2[i] = Encoding.UTF8.GetByteCount(text);
				num2 += ptr2[i] + 1;
			}
			byte* ptr3 = stackalloc byte[(UIntPtr)num2];
			int num3 = 0;
			for (int j = 0; j < items.Length; j++)
			{
				string text2 = items[j];
				fixed (string text3 = text2)
				{
					char* ptr4 = text3;
					if (ptr4 != null)
					{
						ptr4 += RuntimeHelpers.OffsetToStringData / 2;
					}
					num3 += Encoding.UTF8.GetBytes(ptr4, text2.Length, ptr3 + num3, ptr2[j]);
					ptr3[num3] = 0;
					num3++;
				}
			}
			byte** ptr5;
			checked
			{
				ptr5 = stackalloc byte*[unchecked((UIntPtr)items.Length) * (UIntPtr)sizeof(byte*)];
				num3 = 0;
			}
			for (int k = 0; k < items.Length; k++)
			{
				*(IntPtr*)(ptr5 + (IntPtr)k * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr3 + num3;
				num3 += ptr2[k] + 1;
			}
			fixed (int* ptr6 = &current_item)
			{
				int* ptr7 = ptr6;
				int num4 = (int)ImGuiNative.igComboStr_arr(ptr, ptr7, ptr5, items_count, popup_max_height_in_items);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num4 != 0;
			}
		}

		public unsafe static bool Combo(string label, ref int current_item, string items_separated_by_zeros)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (items_separated_by_zeros != null)
			{
				num2 = Encoding.UTF8.GetByteCount(items_separated_by_zeros);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(items_separated_by_zeros, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			int num3 = -1;
			fixed (int* ptr3 = &current_item)
			{
				int* ptr4 = ptr3;
				int num4 = (int)ImGuiNative.igComboStr(ptr, ptr4, ptr2, num3);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num4 != 0;
			}
		}

		public unsafe static bool Combo(string label, ref int current_item, string items_separated_by_zeros, int popup_max_height_in_items)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (items_separated_by_zeros != null)
			{
				num2 = Encoding.UTF8.GetByteCount(items_separated_by_zeros);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(items_separated_by_zeros, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (int* ptr3 = &current_item)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igComboStr(ptr, ptr4, ptr2, popup_max_height_in_items);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static IntPtr CreateContext()
		{
			ImFontAtlas* ptr = null;
			return ImGuiNative.igCreateContext(ptr);
		}

		public static IntPtr CreateContext(ImFontAtlasPtr shared_font_atlas)
		{
			return ImGuiNative.igCreateContext(shared_font_atlas.NativePtr);
		}

		public unsafe static bool DebugCheckVersionAndDataLayout(string version_str, uint sz_io, uint sz_style, uint sz_vec2, uint sz_vec4, uint sz_drawvert, uint sz_drawidx)
		{
			int num = 0;
			byte* ptr;
			if (version_str != null)
			{
				num = Encoding.UTF8.GetByteCount(version_str);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(version_str, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igDebugCheckVersionAndDataLayout(ptr, sz_io, sz_style, sz_vec2, sz_vec4, sz_drawvert, sz_drawidx);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public static void DestroyContext()
		{
			ImGuiNative.igDestroyContext(IntPtr.Zero);
		}

		public static void DestroyContext(IntPtr ctx)
		{
			ImGuiNative.igDestroyContext(ctx);
		}

		public static void DestroyPlatformWindows()
		{
			ImGuiNative.igDestroyPlatformWindows();
		}

		public unsafe static void DockSpace(uint id)
		{
			Vector2 vector = default(Vector2);
			ImGuiDockNodeFlags imGuiDockNodeFlags = ImGuiDockNodeFlags.None;
			ImGuiWindowClass* ptr = null;
			ImGuiNative.igDockSpace(id, vector, imGuiDockNodeFlags, ptr);
		}

		public unsafe static void DockSpace(uint id, Vector2 size)
		{
			ImGuiDockNodeFlags imGuiDockNodeFlags = ImGuiDockNodeFlags.None;
			ImGuiWindowClass* ptr = null;
			ImGuiNative.igDockSpace(id, size, imGuiDockNodeFlags, ptr);
		}

		public unsafe static void DockSpace(uint id, Vector2 size, ImGuiDockNodeFlags flags)
		{
			ImGuiWindowClass* ptr = null;
			ImGuiNative.igDockSpace(id, size, flags, ptr);
		}

		public unsafe static void DockSpace(uint id, Vector2 size, ImGuiDockNodeFlags flags, ImGuiWindowClassPtr window_class)
		{
			ImGuiWindowClass* nativePtr = window_class.NativePtr;
			ImGuiNative.igDockSpace(id, size, flags, nativePtr);
		}

		public unsafe static uint DockSpaceOverViewport()
		{
			ImGuiViewport* ptr = null;
			ImGuiDockNodeFlags imGuiDockNodeFlags = ImGuiDockNodeFlags.None;
			ImGuiWindowClass* ptr2 = null;
			return ImGuiNative.igDockSpaceOverViewport(ptr, imGuiDockNodeFlags, ptr2);
		}

		public unsafe static uint DockSpaceOverViewport(ImGuiViewportPtr viewport)
		{
			ImGuiViewport* nativePtr = viewport.NativePtr;
			ImGuiDockNodeFlags imGuiDockNodeFlags = ImGuiDockNodeFlags.None;
			ImGuiWindowClass* ptr = null;
			return ImGuiNative.igDockSpaceOverViewport(nativePtr, imGuiDockNodeFlags, ptr);
		}

		public unsafe static uint DockSpaceOverViewport(ImGuiViewportPtr viewport, ImGuiDockNodeFlags flags)
		{
			ImGuiViewport* nativePtr = viewport.NativePtr;
			ImGuiWindowClass* ptr = null;
			return ImGuiNative.igDockSpaceOverViewport(nativePtr, flags, ptr);
		}

		public unsafe static uint DockSpaceOverViewport(ImGuiViewportPtr viewport, ImGuiDockNodeFlags flags, ImGuiWindowClassPtr window_class)
		{
			ImGuiViewport* nativePtr = viewport.NativePtr;
			ImGuiWindowClass* nativePtr2 = window_class.NativePtr;
			return ImGuiNative.igDockSpaceOverViewport(nativePtr, flags, nativePtr2);
		}

		public unsafe static bool DragFloat(string label, ref float v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 1f;
			float num3 = 0f;
			float num4 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num5 = (int)ImGuiNative.igDragFloat(ptr, ptr4, num2, num3, num4, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num5 != 0;
			}
		}

		public unsafe static bool DragFloat(string label, ref float v, float v_speed)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 0f;
			float num3 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num4 = (int)ImGuiNative.igDragFloat(ptr, ptr4, v_speed, num2, num3, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num4 != 0;
			}
		}

		public unsafe static bool DragFloat(string label, ref float v, float v_speed, float v_min)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragFloat(ptr, ptr4, v_speed, v_min, num2, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragFloat(string label, ref float v, float v_speed, float v_min, float v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igDragFloat(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool DragFloat(string label, ref float v, float v_speed, float v_min, float v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragFloat(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragFloat(string label, ref float v, float v_speed, float v_min, float v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragFloat(ptr, ptr4, v_speed, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragFloat2(string label, ref Vector2 v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 1f;
			float num3 = 0f;
			float num4 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector2* ptr3 = &v)
			{
				Vector2* ptr4 = ptr3;
				int num5 = (int)ImGuiNative.igDragFloat2(ptr, ptr4, num2, num3, num4, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num5 != 0;
			}
		}

		public unsafe static bool DragFloat2(string label, ref Vector2 v, float v_speed)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 0f;
			float num3 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector2* ptr3 = &v)
			{
				Vector2* ptr4 = ptr3;
				int num4 = (int)ImGuiNative.igDragFloat2(ptr, ptr4, v_speed, num2, num3, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num4 != 0;
			}
		}

		public unsafe static bool DragFloat2(string label, ref Vector2 v, float v_speed, float v_min)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector2* ptr3 = &v)
			{
				Vector2* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragFloat2(ptr, ptr4, v_speed, v_min, num2, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragFloat2(string label, ref Vector2 v, float v_speed, float v_min, float v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector2* ptr3 = &v)
			{
				Vector2* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igDragFloat2(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool DragFloat2(string label, ref Vector2 v, float v_speed, float v_min, float v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector2* ptr3 = &v)
			{
				Vector2* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragFloat2(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragFloat2(string label, ref Vector2 v, float v_speed, float v_min, float v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (Vector2* ptr3 = &v)
			{
				Vector2* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragFloat2(ptr, ptr4, v_speed, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragFloat3(string label, ref Vector3 v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 1f;
			float num3 = 0f;
			float num4 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector3* ptr3 = &v)
			{
				Vector3* ptr4 = ptr3;
				int num5 = (int)ImGuiNative.igDragFloat3(ptr, ptr4, num2, num3, num4, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num5 != 0;
			}
		}

		public unsafe static bool DragFloat3(string label, ref Vector3 v, float v_speed)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 0f;
			float num3 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector3* ptr3 = &v)
			{
				Vector3* ptr4 = ptr3;
				int num4 = (int)ImGuiNative.igDragFloat3(ptr, ptr4, v_speed, num2, num3, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num4 != 0;
			}
		}

		public unsafe static bool DragFloat3(string label, ref Vector3 v, float v_speed, float v_min)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector3* ptr3 = &v)
			{
				Vector3* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragFloat3(ptr, ptr4, v_speed, v_min, num2, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragFloat3(string label, ref Vector3 v, float v_speed, float v_min, float v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector3* ptr3 = &v)
			{
				Vector3* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igDragFloat3(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool DragFloat3(string label, ref Vector3 v, float v_speed, float v_min, float v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector3* ptr3 = &v)
			{
				Vector3* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragFloat3(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragFloat3(string label, ref Vector3 v, float v_speed, float v_min, float v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (Vector3* ptr3 = &v)
			{
				Vector3* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragFloat3(ptr, ptr4, v_speed, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragFloat4(string label, ref Vector4 v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 1f;
			float num3 = 0f;
			float num4 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector4* ptr3 = &v)
			{
				Vector4* ptr4 = ptr3;
				int num5 = (int)ImGuiNative.igDragFloat4(ptr, ptr4, num2, num3, num4, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num5 != 0;
			}
		}

		public unsafe static bool DragFloat4(string label, ref Vector4 v, float v_speed)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 0f;
			float num3 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector4* ptr3 = &v)
			{
				Vector4* ptr4 = ptr3;
				int num4 = (int)ImGuiNative.igDragFloat4(ptr, ptr4, v_speed, num2, num3, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num4 != 0;
			}
		}

		public unsafe static bool DragFloat4(string label, ref Vector4 v, float v_speed, float v_min)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector4* ptr3 = &v)
			{
				Vector4* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragFloat4(ptr, ptr4, v_speed, v_min, num2, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragFloat4(string label, ref Vector4 v, float v_speed, float v_min, float v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector4* ptr3 = &v)
			{
				Vector4* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igDragFloat4(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool DragFloat4(string label, ref Vector4 v, float v_speed, float v_min, float v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector4* ptr3 = &v)
			{
				Vector4* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragFloat4(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragFloat4(string label, ref Vector4 v, float v_speed, float v_min, float v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (Vector4* ptr3 = &v)
			{
				Vector4* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragFloat4(ptr, ptr4, v_speed, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragFloatRange2(string label, ref float v_current_min, ref float v_current_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 1f;
			float num3 = 0f;
			float num4 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			byte* ptr3 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr4 = &v_current_min)
			{
				float* ptr5 = ptr4;
				fixed (float* ptr6 = &v_current_max)
				{
					float* ptr7 = ptr6;
					int num5 = (int)ImGuiNative.igDragFloatRange2(ptr, ptr5, ptr7, num2, num3, num4, ptr2, ptr3, imGuiSliderFlags);
					if (num > 2048)
					{
						Util.Free(ptr);
					}
					if (byteCount > 2048)
					{
						Util.Free(ptr2);
					}
					return num5 != 0;
				}
			}
		}

		public unsafe static bool DragFloatRange2(string label, ref float v_current_min, ref float v_current_max, float v_speed)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 0f;
			float num3 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			byte* ptr3 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr4 = &v_current_min)
			{
				float* ptr5 = ptr4;
				fixed (float* ptr6 = &v_current_max)
				{
					float* ptr7 = ptr6;
					int num4 = (int)ImGuiNative.igDragFloatRange2(ptr, ptr5, ptr7, v_speed, num2, num3, ptr2, ptr3, imGuiSliderFlags);
					if (num > 2048)
					{
						Util.Free(ptr);
					}
					if (byteCount > 2048)
					{
						Util.Free(ptr2);
					}
					return num4 != 0;
				}
			}
		}

		public unsafe static bool DragFloatRange2(string label, ref float v_current_min, ref float v_current_max, float v_speed, float v_min)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			byte* ptr3 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr4 = &v_current_min)
			{
				float* ptr5 = ptr4;
				fixed (float* ptr6 = &v_current_max)
				{
					float* ptr7 = ptr6;
					int num3 = (int)ImGuiNative.igDragFloatRange2(ptr, ptr5, ptr7, v_speed, v_min, num2, ptr2, ptr3, imGuiSliderFlags);
					if (num > 2048)
					{
						Util.Free(ptr);
					}
					if (byteCount > 2048)
					{
						Util.Free(ptr2);
					}
					return num3 != 0;
				}
			}
		}

		public unsafe static bool DragFloatRange2(string label, ref float v_current_min, ref float v_current_max, float v_speed, float v_min, float v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			byte* ptr3 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr4 = &v_current_min)
			{
				float* ptr5 = ptr4;
				fixed (float* ptr6 = &v_current_max)
				{
					float* ptr7 = ptr6;
					int num2 = (int)ImGuiNative.igDragFloatRange2(ptr, ptr5, ptr7, v_speed, v_min, v_max, ptr2, ptr3, imGuiSliderFlags);
					if (num > 2048)
					{
						Util.Free(ptr);
					}
					if (byteCount > 2048)
					{
						Util.Free(ptr2);
					}
					return num2 != 0;
				}
			}
		}

		public unsafe static bool DragFloatRange2(string label, ref float v_current_min, ref float v_current_max, float v_speed, float v_min, float v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			byte* ptr3 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr4 = &v_current_min)
			{
				float* ptr5 = ptr4;
				fixed (float* ptr6 = &v_current_max)
				{
					float* ptr7 = ptr6;
					int num3 = (int)ImGuiNative.igDragFloatRange2(ptr, ptr5, ptr7, v_speed, v_min, v_max, ptr2, ptr3, imGuiSliderFlags);
					if (num > 2048)
					{
						Util.Free(ptr);
					}
					if (num2 > 2048)
					{
						Util.Free(ptr2);
					}
					return num3 != 0;
				}
			}
		}

		public unsafe static bool DragFloatRange2(string label, ref float v_current_min, ref float v_current_max, float v_speed, float v_min, float v_max, string format, string format_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			int num3 = 0;
			byte* ptr3;
			if (format_max != null)
			{
				num3 = Encoding.UTF8.GetByteCount(format_max);
				if (num3 > 2048)
				{
					ptr3 = Util.Allocate(num3 + 1);
				}
				else
				{
					ptr3 = stackalloc byte[(UIntPtr)(num3 + 1)];
				}
				int utf3 = Util.GetUtf8(format_max, ptr3, num3);
				ptr3[utf3] = 0;
			}
			else
			{
				ptr3 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr4 = &v_current_min)
			{
				float* ptr5 = ptr4;
				fixed (float* ptr6 = &v_current_max)
				{
					float* ptr7 = ptr6;
					int num4 = (int)ImGuiNative.igDragFloatRange2(ptr, ptr5, ptr7, v_speed, v_min, v_max, ptr2, ptr3, imGuiSliderFlags);
					if (num > 2048)
					{
						Util.Free(ptr);
					}
					if (num2 > 2048)
					{
						Util.Free(ptr2);
					}
					if (num3 > 2048)
					{
						Util.Free(ptr3);
					}
					return num4 != 0;
				}
			}
		}

		public unsafe static bool DragFloatRange2(string label, ref float v_current_min, ref float v_current_max, float v_speed, float v_min, float v_max, string format, string format_max, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			int num3 = 0;
			byte* ptr3;
			if (format_max != null)
			{
				num3 = Encoding.UTF8.GetByteCount(format_max);
				if (num3 > 2048)
				{
					ptr3 = Util.Allocate(num3 + 1);
				}
				else
				{
					ptr3 = stackalloc byte[(UIntPtr)(num3 + 1)];
				}
				int utf3 = Util.GetUtf8(format_max, ptr3, num3);
				ptr3[utf3] = 0;
			}
			else
			{
				ptr3 = null;
			}
			fixed (float* ptr4 = &v_current_min)
			{
				float* ptr5 = ptr4;
				fixed (float* ptr6 = &v_current_max)
				{
					float* ptr7 = ptr6;
					int num4 = (int)ImGuiNative.igDragFloatRange2(ptr, ptr5, ptr7, v_speed, v_min, v_max, ptr2, ptr3, flags);
					if (num > 2048)
					{
						Util.Free(ptr);
					}
					if (num2 > 2048)
					{
						Util.Free(ptr2);
					}
					if (num3 > 2048)
					{
						Util.Free(ptr3);
					}
					return num4 != 0;
				}
			}
		}

		public unsafe static bool DragInt(string label, ref int v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 1f;
			int num3 = 0;
			int num4 = 0;
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num5 = (int)ImGuiNative.igDragInt(ptr, ptr4, num2, num3, num4, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num5 != 0;
			}
		}

		public unsafe static bool DragInt(string label, ref int v, float v_speed)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			int num3 = 0;
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num4 = (int)ImGuiNative.igDragInt(ptr, ptr4, v_speed, num2, num3, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num4 != 0;
			}
		}

		public unsafe static bool DragInt(string label, ref int v, float v_speed, int v_min)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragInt(ptr, ptr4, v_speed, v_min, num2, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragInt(string label, ref int v, float v_speed, int v_min, int v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igDragInt(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool DragInt(string label, ref int v, float v_speed, int v_min, int v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragInt(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragInt(string label, ref int v, float v_speed, int v_min, int v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragInt(ptr, ptr4, v_speed, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragInt2(string label, ref int v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 1f;
			int num3 = 0;
			int num4 = 0;
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num5 = (int)ImGuiNative.igDragInt2(ptr, ptr4, num2, num3, num4, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num5 != 0;
			}
		}

		public unsafe static bool DragInt2(string label, ref int v, float v_speed)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			int num3 = 0;
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num4 = (int)ImGuiNative.igDragInt2(ptr, ptr4, v_speed, num2, num3, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num4 != 0;
			}
		}

		public unsafe static bool DragInt2(string label, ref int v, float v_speed, int v_min)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragInt2(ptr, ptr4, v_speed, v_min, num2, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragInt2(string label, ref int v, float v_speed, int v_min, int v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igDragInt2(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool DragInt2(string label, ref int v, float v_speed, int v_min, int v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragInt2(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragInt2(string label, ref int v, float v_speed, int v_min, int v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragInt2(ptr, ptr4, v_speed, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragInt3(string label, ref int v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 1f;
			int num3 = 0;
			int num4 = 0;
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num5 = (int)ImGuiNative.igDragInt3(ptr, ptr4, num2, num3, num4, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num5 != 0;
			}
		}

		public unsafe static bool DragInt3(string label, ref int v, float v_speed)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			int num3 = 0;
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num4 = (int)ImGuiNative.igDragInt3(ptr, ptr4, v_speed, num2, num3, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num4 != 0;
			}
		}

		public unsafe static bool DragInt3(string label, ref int v, float v_speed, int v_min)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragInt3(ptr, ptr4, v_speed, v_min, num2, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragInt3(string label, ref int v, float v_speed, int v_min, int v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igDragInt3(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool DragInt3(string label, ref int v, float v_speed, int v_min, int v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragInt3(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragInt3(string label, ref int v, float v_speed, int v_min, int v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragInt3(ptr, ptr4, v_speed, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragInt4(string label, ref int v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 1f;
			int num3 = 0;
			int num4 = 0;
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num5 = (int)ImGuiNative.igDragInt4(ptr, ptr4, num2, num3, num4, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num5 != 0;
			}
		}

		public unsafe static bool DragInt4(string label, ref int v, float v_speed)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			int num3 = 0;
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num4 = (int)ImGuiNative.igDragInt4(ptr, ptr4, v_speed, num2, num3, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num4 != 0;
			}
		}

		public unsafe static bool DragInt4(string label, ref int v, float v_speed, int v_min)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragInt4(ptr, ptr4, v_speed, v_min, num2, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragInt4(string label, ref int v, float v_speed, int v_min, int v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igDragInt4(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool DragInt4(string label, ref int v, float v_speed, int v_min, int v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragInt4(ptr, ptr4, v_speed, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragInt4(string label, ref int v, float v_speed, int v_min, int v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igDragInt4(ptr, ptr4, v_speed, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool DragIntRange2(string label, ref int v_current_min, ref int v_current_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 1f;
			int num3 = 0;
			int num4 = 0;
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			byte* ptr3 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr4 = &v_current_min)
			{
				int* ptr5 = ptr4;
				fixed (int* ptr6 = &v_current_max)
				{
					int* ptr7 = ptr6;
					int num5 = (int)ImGuiNative.igDragIntRange2(ptr, ptr5, ptr7, num2, num3, num4, ptr2, ptr3, imGuiSliderFlags);
					if (num > 2048)
					{
						Util.Free(ptr);
					}
					if (byteCount > 2048)
					{
						Util.Free(ptr2);
					}
					return num5 != 0;
				}
			}
		}

		public unsafe static bool DragIntRange2(string label, ref int v_current_min, ref int v_current_max, float v_speed)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			int num3 = 0;
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			byte* ptr3 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr4 = &v_current_min)
			{
				int* ptr5 = ptr4;
				fixed (int* ptr6 = &v_current_max)
				{
					int* ptr7 = ptr6;
					int num4 = (int)ImGuiNative.igDragIntRange2(ptr, ptr5, ptr7, v_speed, num2, num3, ptr2, ptr3, imGuiSliderFlags);
					if (num > 2048)
					{
						Util.Free(ptr);
					}
					if (byteCount > 2048)
					{
						Util.Free(ptr2);
					}
					return num4 != 0;
				}
			}
		}

		public unsafe static bool DragIntRange2(string label, ref int v_current_min, ref int v_current_max, float v_speed, int v_min)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			byte* ptr3 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr4 = &v_current_min)
			{
				int* ptr5 = ptr4;
				fixed (int* ptr6 = &v_current_max)
				{
					int* ptr7 = ptr6;
					int num3 = (int)ImGuiNative.igDragIntRange2(ptr, ptr5, ptr7, v_speed, v_min, num2, ptr2, ptr3, imGuiSliderFlags);
					if (num > 2048)
					{
						Util.Free(ptr);
					}
					if (byteCount > 2048)
					{
						Util.Free(ptr2);
					}
					return num3 != 0;
				}
			}
		}

		public unsafe static bool DragIntRange2(string label, ref int v_current_min, ref int v_current_max, float v_speed, int v_min, int v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			byte* ptr3 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr4 = &v_current_min)
			{
				int* ptr5 = ptr4;
				fixed (int* ptr6 = &v_current_max)
				{
					int* ptr7 = ptr6;
					int num2 = (int)ImGuiNative.igDragIntRange2(ptr, ptr5, ptr7, v_speed, v_min, v_max, ptr2, ptr3, imGuiSliderFlags);
					if (num > 2048)
					{
						Util.Free(ptr);
					}
					if (byteCount > 2048)
					{
						Util.Free(ptr2);
					}
					return num2 != 0;
				}
			}
		}

		public unsafe static bool DragIntRange2(string label, ref int v_current_min, ref int v_current_max, float v_speed, int v_min, int v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			byte* ptr3 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr4 = &v_current_min)
			{
				int* ptr5 = ptr4;
				fixed (int* ptr6 = &v_current_max)
				{
					int* ptr7 = ptr6;
					int num3 = (int)ImGuiNative.igDragIntRange2(ptr, ptr5, ptr7, v_speed, v_min, v_max, ptr2, ptr3, imGuiSliderFlags);
					if (num > 2048)
					{
						Util.Free(ptr);
					}
					if (num2 > 2048)
					{
						Util.Free(ptr2);
					}
					return num3 != 0;
				}
			}
		}

		public unsafe static bool DragIntRange2(string label, ref int v_current_min, ref int v_current_max, float v_speed, int v_min, int v_max, string format, string format_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			int num3 = 0;
			byte* ptr3;
			if (format_max != null)
			{
				num3 = Encoding.UTF8.GetByteCount(format_max);
				if (num3 > 2048)
				{
					ptr3 = Util.Allocate(num3 + 1);
				}
				else
				{
					ptr3 = stackalloc byte[(UIntPtr)(num3 + 1)];
				}
				int utf3 = Util.GetUtf8(format_max, ptr3, num3);
				ptr3[utf3] = 0;
			}
			else
			{
				ptr3 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr4 = &v_current_min)
			{
				int* ptr5 = ptr4;
				fixed (int* ptr6 = &v_current_max)
				{
					int* ptr7 = ptr6;
					int num4 = (int)ImGuiNative.igDragIntRange2(ptr, ptr5, ptr7, v_speed, v_min, v_max, ptr2, ptr3, imGuiSliderFlags);
					if (num > 2048)
					{
						Util.Free(ptr);
					}
					if (num2 > 2048)
					{
						Util.Free(ptr2);
					}
					if (num3 > 2048)
					{
						Util.Free(ptr3);
					}
					return num4 != 0;
				}
			}
		}

		public unsafe static bool DragIntRange2(string label, ref int v_current_min, ref int v_current_max, float v_speed, int v_min, int v_max, string format, string format_max, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			int num3 = 0;
			byte* ptr3;
			if (format_max != null)
			{
				num3 = Encoding.UTF8.GetByteCount(format_max);
				if (num3 > 2048)
				{
					ptr3 = Util.Allocate(num3 + 1);
				}
				else
				{
					ptr3 = stackalloc byte[(UIntPtr)(num3 + 1)];
				}
				int utf3 = Util.GetUtf8(format_max, ptr3, num3);
				ptr3[utf3] = 0;
			}
			else
			{
				ptr3 = null;
			}
			fixed (int* ptr4 = &v_current_min)
			{
				int* ptr5 = ptr4;
				fixed (int* ptr6 = &v_current_max)
				{
					int* ptr7 = ptr6;
					int num4 = (int)ImGuiNative.igDragIntRange2(ptr, ptr5, ptr7, v_speed, v_min, v_max, ptr2, ptr3, flags);
					if (num > 2048)
					{
						Util.Free(ptr);
					}
					if (num2 > 2048)
					{
						Util.Free(ptr2);
					}
					if (num3 > 2048)
					{
						Util.Free(ptr3);
					}
					return num4 != 0;
				}
			}
		}

		public unsafe static bool DragScalar(string label, ImGuiDataType data_type, IntPtr p_data, float v_speed)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = null;
			void* ptr4 = null;
			byte* ptr5 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			int num2 = (int)ImGuiNative.igDragScalar(ptr, data_type, ptr2, v_speed, ptr3, ptr4, ptr5, imGuiSliderFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool DragScalar(string label, ImGuiDataType data_type, IntPtr p_data, float v_speed, IntPtr p_min)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = null;
			byte* ptr5 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			int num2 = (int)ImGuiNative.igDragScalar(ptr, data_type, ptr2, v_speed, ptr3, ptr4, ptr5, imGuiSliderFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool DragScalar(string label, ImGuiDataType data_type, IntPtr p_data, float v_speed, IntPtr p_min, IntPtr p_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = p_max.ToPointer();
			byte* ptr5 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			int num2 = (int)ImGuiNative.igDragScalar(ptr, data_type, ptr2, v_speed, ptr3, ptr4, ptr5, imGuiSliderFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool DragScalar(string label, ImGuiDataType data_type, IntPtr p_data, float v_speed, IntPtr p_min, IntPtr p_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = p_max.ToPointer();
			int num2 = 0;
			byte* ptr5;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr5 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr5 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr5, num2);
				ptr5[utf2] = 0;
			}
			else
			{
				ptr5 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			int num3 = (int)ImGuiNative.igDragScalar(ptr, data_type, ptr2, v_speed, ptr3, ptr4, ptr5, imGuiSliderFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr5);
			}
			return num3 != 0;
		}

		public unsafe static bool DragScalar(string label, ImGuiDataType data_type, IntPtr p_data, float v_speed, IntPtr p_min, IntPtr p_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = p_max.ToPointer();
			int num2 = 0;
			byte* ptr5;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr5 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr5 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr5, num2);
				ptr5[utf2] = 0;
			}
			else
			{
				ptr5 = null;
			}
			int num3 = (int)ImGuiNative.igDragScalar(ptr, data_type, ptr2, v_speed, ptr3, ptr4, ptr5, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr5);
			}
			return num3 != 0;
		}

		public unsafe static bool DragScalarN(string label, ImGuiDataType data_type, IntPtr p_data, int components, float v_speed)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = null;
			void* ptr4 = null;
			byte* ptr5 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			int num2 = (int)ImGuiNative.igDragScalarN(ptr, data_type, ptr2, components, v_speed, ptr3, ptr4, ptr5, imGuiSliderFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool DragScalarN(string label, ImGuiDataType data_type, IntPtr p_data, int components, float v_speed, IntPtr p_min)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = null;
			byte* ptr5 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			int num2 = (int)ImGuiNative.igDragScalarN(ptr, data_type, ptr2, components, v_speed, ptr3, ptr4, ptr5, imGuiSliderFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool DragScalarN(string label, ImGuiDataType data_type, IntPtr p_data, int components, float v_speed, IntPtr p_min, IntPtr p_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = p_max.ToPointer();
			byte* ptr5 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			int num2 = (int)ImGuiNative.igDragScalarN(ptr, data_type, ptr2, components, v_speed, ptr3, ptr4, ptr5, imGuiSliderFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool DragScalarN(string label, ImGuiDataType data_type, IntPtr p_data, int components, float v_speed, IntPtr p_min, IntPtr p_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = p_max.ToPointer();
			int num2 = 0;
			byte* ptr5;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr5 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr5 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr5, num2);
				ptr5[utf2] = 0;
			}
			else
			{
				ptr5 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			int num3 = (int)ImGuiNative.igDragScalarN(ptr, data_type, ptr2, components, v_speed, ptr3, ptr4, ptr5, imGuiSliderFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr5);
			}
			return num3 != 0;
		}

		public unsafe static bool DragScalarN(string label, ImGuiDataType data_type, IntPtr p_data, int components, float v_speed, IntPtr p_min, IntPtr p_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = p_max.ToPointer();
			int num2 = 0;
			byte* ptr5;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr5 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr5 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr5, num2);
				ptr5[utf2] = 0;
			}
			else
			{
				ptr5 = null;
			}
			int num3 = (int)ImGuiNative.igDragScalarN(ptr, data_type, ptr2, components, v_speed, ptr3, ptr4, ptr5, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr5);
			}
			return num3 != 0;
		}

		public static void Dummy(Vector2 size)
		{
			ImGuiNative.igDummy(size);
		}

		public static void End()
		{
			ImGuiNative.igEnd();
		}

		public static void EndChild()
		{
			ImGuiNative.igEndChild();
		}

		public static void EndChildFrame()
		{
			ImGuiNative.igEndChildFrame();
		}

		public static void EndCombo()
		{
			ImGuiNative.igEndCombo();
		}

		public static void EndDragDropSource()
		{
			ImGuiNative.igEndDragDropSource();
		}

		public static void EndDragDropTarget()
		{
			ImGuiNative.igEndDragDropTarget();
		}

		public static void EndFrame()
		{
			ImGuiNative.igEndFrame();
		}

		public static void EndGroup()
		{
			ImGuiNative.igEndGroup();
		}

		public static void EndListBox()
		{
			ImGuiNative.igEndListBox();
		}

		public static void EndMainMenuBar()
		{
			ImGuiNative.igEndMainMenuBar();
		}

		public static void EndMenu()
		{
			ImGuiNative.igEndMenu();
		}

		public static void EndMenuBar()
		{
			ImGuiNative.igEndMenuBar();
		}

		public static void EndPopup()
		{
			ImGuiNative.igEndPopup();
		}

		public static void EndTabBar()
		{
			ImGuiNative.igEndTabBar();
		}

		public static void EndTabItem()
		{
			ImGuiNative.igEndTabItem();
		}

		public static void EndTable()
		{
			ImGuiNative.igEndTable();
		}

		public static void EndTooltip()
		{
			ImGuiNative.igEndTooltip();
		}

		public static ImGuiViewportPtr FindViewportByID(uint id)
		{
			return new ImGuiViewportPtr(ImGuiNative.igFindViewportByID(id));
		}

		public static ImGuiViewportPtr FindViewportByPlatformHandle(IntPtr platform_handle)
		{
			return new ImGuiViewportPtr(ImGuiNative.igFindViewportByPlatformHandle(platform_handle.ToPointer()));
		}

		public unsafe static void GetAllocatorFunctions(ref IntPtr p_alloc_func, ref IntPtr p_free_func, ref void* p_user_data)
		{
			fixed (IntPtr* ptr = &p_alloc_func)
			{
				IntPtr* ptr2 = ptr;
				fixed (IntPtr* ptr3 = &p_free_func)
				{
					IntPtr* ptr4 = ptr3;
					fixed (void** ptr5 = &p_user_data)
					{
						void** ptr6 = ptr5;
						ImGuiNative.igGetAllocatorFunctions(ptr2, ptr4, ptr6);
					}
				}
			}
		}

		public static ImDrawListPtr GetBackgroundDrawList()
		{
			return new ImDrawListPtr(ImGuiNative.igGetBackgroundDrawListNil());
		}

		public static ImDrawListPtr GetBackgroundDrawList(ImGuiViewportPtr viewport)
		{
			return new ImDrawListPtr(ImGuiNative.igGetBackgroundDrawListViewportPtr(viewport.NativePtr));
		}

		public static string GetClipboardText()
		{
			return Util.StringFromPtr(ImGuiNative.igGetClipboardText());
		}

		public static uint GetColorU32(ImGuiCol idx)
		{
			float num = 1f;
			return ImGuiNative.igGetColorU32Col(idx, num);
		}

		public static uint GetColorU32(ImGuiCol idx, float alpha_mul)
		{
			return ImGuiNative.igGetColorU32Col(idx, alpha_mul);
		}

		public static uint GetColorU32(Vector4 col)
		{
			return ImGuiNative.igGetColorU32Vec4(col);
		}

		public static uint GetColorU32(uint col)
		{
			return ImGuiNative.igGetColorU32U32(col);
		}

		public static int GetColumnIndex()
		{
			return ImGuiNative.igGetColumnIndex();
		}

		public static float GetColumnOffset()
		{
			return ImGuiNative.igGetColumnOffset(-1);
		}

		public static float GetColumnOffset(int column_index)
		{
			return ImGuiNative.igGetColumnOffset(column_index);
		}

		public static int GetColumnsCount()
		{
			return ImGuiNative.igGetColumnsCount();
		}

		public static float GetColumnWidth()
		{
			return ImGuiNative.igGetColumnWidth(-1);
		}

		public static float GetColumnWidth(int column_index)
		{
			return ImGuiNative.igGetColumnWidth(column_index);
		}

		public unsafe static Vector2 GetContentRegionAvail()
		{
			Vector2 vector;
			ImGuiNative.igGetContentRegionAvail(&vector);
			return vector;
		}

		public unsafe static Vector2 GetContentRegionMax()
		{
			Vector2 vector;
			ImGuiNative.igGetContentRegionMax(&vector);
			return vector;
		}

		public static IntPtr GetCurrentContext()
		{
			return ImGuiNative.igGetCurrentContext();
		}

		public unsafe static Vector2 GetCursorPos()
		{
			Vector2 vector;
			ImGuiNative.igGetCursorPos(&vector);
			return vector;
		}

		public static float GetCursorPosX()
		{
			return ImGuiNative.igGetCursorPosX();
		}

		public static float GetCursorPosY()
		{
			return ImGuiNative.igGetCursorPosY();
		}

		public unsafe static Vector2 GetCursorScreenPos()
		{
			Vector2 vector;
			ImGuiNative.igGetCursorScreenPos(&vector);
			return vector;
		}

		public unsafe static Vector2 GetCursorStartPos()
		{
			Vector2 vector;
			ImGuiNative.igGetCursorStartPos(&vector);
			return vector;
		}

		public static ImGuiPayloadPtr GetDragDropPayload()
		{
			return new ImGuiPayloadPtr(ImGuiNative.igGetDragDropPayload());
		}

		public static ImDrawDataPtr GetDrawData()
		{
			return new ImDrawDataPtr(ImGuiNative.igGetDrawData());
		}

		public static IntPtr GetDrawListSharedData()
		{
			return ImGuiNative.igGetDrawListSharedData();
		}

		public static ImFontPtr GetFont()
		{
			return new ImFontPtr(ImGuiNative.igGetFont());
		}

		public static float GetFontSize()
		{
			return ImGuiNative.igGetFontSize();
		}

		public unsafe static Vector2 GetFontTexUvWhitePixel()
		{
			Vector2 vector;
			ImGuiNative.igGetFontTexUvWhitePixel(&vector);
			return vector;
		}

		public static ImDrawListPtr GetForegroundDrawList()
		{
			return new ImDrawListPtr(ImGuiNative.igGetForegroundDrawListNil());
		}

		public static ImDrawListPtr GetForegroundDrawList(ImGuiViewportPtr viewport)
		{
			return new ImDrawListPtr(ImGuiNative.igGetForegroundDrawListViewportPtr(viewport.NativePtr));
		}

		public static int GetFrameCount()
		{
			return ImGuiNative.igGetFrameCount();
		}

		public static float GetFrameHeight()
		{
			return ImGuiNative.igGetFrameHeight();
		}

		public static float GetFrameHeightWithSpacing()
		{
			return ImGuiNative.igGetFrameHeightWithSpacing();
		}

		public unsafe static uint GetID(string str_id)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			uint num2 = ImGuiNative.igGetIDStr(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2;
		}

		public static uint GetID(IntPtr ptr_id)
		{
			return ImGuiNative.igGetIDPtr(ptr_id.ToPointer());
		}

		public static ImGuiIOPtr GetIO()
		{
			return new ImGuiIOPtr(ImGuiNative.igGetIO());
		}

		public unsafe static Vector2 GetItemRectMax()
		{
			Vector2 vector;
			ImGuiNative.igGetItemRectMax(&vector);
			return vector;
		}

		public unsafe static Vector2 GetItemRectMin()
		{
			Vector2 vector;
			ImGuiNative.igGetItemRectMin(&vector);
			return vector;
		}

		public unsafe static Vector2 GetItemRectSize()
		{
			Vector2 vector;
			ImGuiNative.igGetItemRectSize(&vector);
			return vector;
		}

		public static int GetKeyIndex(ImGuiKey imgui_key)
		{
			return ImGuiNative.igGetKeyIndex(imgui_key);
		}

		public static int GetKeyPressedAmount(int key_index, float repeat_delay, float rate)
		{
			return ImGuiNative.igGetKeyPressedAmount(key_index, repeat_delay, rate);
		}

		public static ImGuiViewportPtr GetMainViewport()
		{
			return new ImGuiViewportPtr(ImGuiNative.igGetMainViewport());
		}

		public static ImGuiMouseCursor GetMouseCursor()
		{
			return ImGuiNative.igGetMouseCursor();
		}

		public unsafe static Vector2 GetMouseDragDelta()
		{
			ImGuiMouseButton imGuiMouseButton = ImGuiMouseButton.Left;
			float num = -1f;
			Vector2 vector;
			ImGuiNative.igGetMouseDragDelta(&vector, imGuiMouseButton, num);
			return vector;
		}

		public unsafe static Vector2 GetMouseDragDelta(ImGuiMouseButton button)
		{
			float num = -1f;
			Vector2 vector;
			ImGuiNative.igGetMouseDragDelta(&vector, button, num);
			return vector;
		}

		public unsafe static Vector2 GetMouseDragDelta(ImGuiMouseButton button, float lock_threshold)
		{
			Vector2 vector;
			ImGuiNative.igGetMouseDragDelta(&vector, button, lock_threshold);
			return vector;
		}

		public unsafe static Vector2 GetMousePos()
		{
			Vector2 vector;
			ImGuiNative.igGetMousePos(&vector);
			return vector;
		}

		public unsafe static Vector2 GetMousePosOnOpeningCurrentPopup()
		{
			Vector2 vector;
			ImGuiNative.igGetMousePosOnOpeningCurrentPopup(&vector);
			return vector;
		}

		public static ImGuiPlatformIOPtr GetPlatformIO()
		{
			return new ImGuiPlatformIOPtr(ImGuiNative.igGetPlatformIO());
		}

		public static float GetScrollMaxX()
		{
			return ImGuiNative.igGetScrollMaxX();
		}

		public static float GetScrollMaxY()
		{
			return ImGuiNative.igGetScrollMaxY();
		}

		public static float GetScrollX()
		{
			return ImGuiNative.igGetScrollX();
		}

		public static float GetScrollY()
		{
			return ImGuiNative.igGetScrollY();
		}

		public static ImGuiStoragePtr GetStateStorage()
		{
			return new ImGuiStoragePtr(ImGuiNative.igGetStateStorage());
		}

		public static ImGuiStylePtr GetStyle()
		{
			return new ImGuiStylePtr(ImGuiNative.igGetStyle());
		}

		public static string GetStyleColorName(ImGuiCol idx)
		{
			return Util.StringFromPtr(ImGuiNative.igGetStyleColorName(idx));
		}

		public unsafe static Vector4* GetStyleColorVec4(ImGuiCol idx)
		{
			return ImGuiNative.igGetStyleColorVec4(idx);
		}

		public static float GetTextLineHeight()
		{
			return ImGuiNative.igGetTextLineHeight();
		}

		public static float GetTextLineHeightWithSpacing()
		{
			return ImGuiNative.igGetTextLineHeightWithSpacing();
		}

		public static double GetTime()
		{
			return ImGuiNative.igGetTime();
		}

		public static float GetTreeNodeToLabelSpacing()
		{
			return ImGuiNative.igGetTreeNodeToLabelSpacing();
		}

		public static string GetVersion()
		{
			return Util.StringFromPtr(ImGuiNative.igGetVersion());
		}

		public unsafe static Vector2 GetWindowContentRegionMax()
		{
			Vector2 vector;
			ImGuiNative.igGetWindowContentRegionMax(&vector);
			return vector;
		}

		public unsafe static Vector2 GetWindowContentRegionMin()
		{
			Vector2 vector;
			ImGuiNative.igGetWindowContentRegionMin(&vector);
			return vector;
		}

		public static float GetWindowContentRegionWidth()
		{
			return ImGuiNative.igGetWindowContentRegionWidth();
		}

		public static uint GetWindowDockID()
		{
			return ImGuiNative.igGetWindowDockID();
		}

		public static float GetWindowDpiScale()
		{
			return ImGuiNative.igGetWindowDpiScale();
		}

		public static ImDrawListPtr GetWindowDrawList()
		{
			return new ImDrawListPtr(ImGuiNative.igGetWindowDrawList());
		}

		public static float GetWindowHeight()
		{
			return ImGuiNative.igGetWindowHeight();
		}

		public unsafe static Vector2 GetWindowPos()
		{
			Vector2 vector;
			ImGuiNative.igGetWindowPos(&vector);
			return vector;
		}

		public unsafe static Vector2 GetWindowSize()
		{
			Vector2 vector;
			ImGuiNative.igGetWindowSize(&vector);
			return vector;
		}

		public static ImGuiViewportPtr GetWindowViewport()
		{
			return new ImGuiViewportPtr(ImGuiNative.igGetWindowViewport());
		}

		public static float GetWindowWidth()
		{
			return ImGuiNative.igGetWindowWidth();
		}

		public static void Image(IntPtr user_texture_id, Vector2 size)
		{
			Vector2 vector = default(Vector2);
			Vector2 vector2 = new Vector2(1f, 1f);
			Vector4 vector3 = new Vector4(1f, 1f, 1f, 1f);
			Vector4 vector4 = default(Vector4);
			ImGuiNative.igImage(user_texture_id, size, vector, vector2, vector3, vector4);
		}

		public static void Image(IntPtr user_texture_id, Vector2 size, Vector2 uv0)
		{
			Vector2 vector = new Vector2(1f, 1f);
			Vector4 vector2 = new Vector4(1f, 1f, 1f, 1f);
			Vector4 vector3 = default(Vector4);
			ImGuiNative.igImage(user_texture_id, size, uv0, vector, vector2, vector3);
		}

		public static void Image(IntPtr user_texture_id, Vector2 size, Vector2 uv0, Vector2 uv1)
		{
			Vector4 vector = new Vector4(1f, 1f, 1f, 1f);
			Vector4 vector2 = default(Vector4);
			ImGuiNative.igImage(user_texture_id, size, uv0, uv1, vector, vector2);
		}

		public static void Image(IntPtr user_texture_id, Vector2 size, Vector2 uv0, Vector2 uv1, Vector4 tint_col)
		{
			ImGuiNative.igImage(user_texture_id, size, uv0, uv1, tint_col, default(Vector4));
		}

		public static void Image(IntPtr user_texture_id, Vector2 size, Vector2 uv0, Vector2 uv1, Vector4 tint_col, Vector4 border_col)
		{
			ImGuiNative.igImage(user_texture_id, size, uv0, uv1, tint_col, border_col);
		}

		public static bool ImageButton(IntPtr user_texture_id, Vector2 size)
		{
			Vector2 vector = default(Vector2);
			Vector2 vector2 = new Vector2(1f, 1f);
			int num = -1;
			Vector4 vector3 = default(Vector4);
			Vector4 vector4 = new Vector4(1f, 1f, 1f, 1f);
			return ImGuiNative.igImageButton(user_texture_id, size, vector, vector2, num, vector3, vector4) > 0;
		}

		public static bool ImageButton(IntPtr user_texture_id, Vector2 size, Vector2 uv0)
		{
			Vector2 vector = new Vector2(1f, 1f);
			int num = -1;
			Vector4 vector2 = default(Vector4);
			Vector4 vector3 = new Vector4(1f, 1f, 1f, 1f);
			return ImGuiNative.igImageButton(user_texture_id, size, uv0, vector, num, vector2, vector3) > 0;
		}

		public static bool ImageButton(IntPtr user_texture_id, Vector2 size, Vector2 uv0, Vector2 uv1)
		{
			int num = -1;
			Vector4 vector = default(Vector4);
			Vector4 vector2 = new Vector4(1f, 1f, 1f, 1f);
			return ImGuiNative.igImageButton(user_texture_id, size, uv0, uv1, num, vector, vector2) > 0;
		}

		public static bool ImageButton(IntPtr user_texture_id, Vector2 size, Vector2 uv0, Vector2 uv1, int frame_padding)
		{
			Vector4 vector = default(Vector4);
			Vector4 vector2 = new Vector4(1f, 1f, 1f, 1f);
			return ImGuiNative.igImageButton(user_texture_id, size, uv0, uv1, frame_padding, vector, vector2) > 0;
		}

		public static bool ImageButton(IntPtr user_texture_id, Vector2 size, Vector2 uv0, Vector2 uv1, int frame_padding, Vector4 bg_col)
		{
			Vector4 vector = new Vector4(1f, 1f, 1f, 1f);
			return ImGuiNative.igImageButton(user_texture_id, size, uv0, uv1, frame_padding, bg_col, vector) > 0;
		}

		public static bool ImageButton(IntPtr user_texture_id, Vector2 size, Vector2 uv0, Vector2 uv1, int frame_padding, Vector4 bg_col, Vector4 tint_col)
		{
			return ImGuiNative.igImageButton(user_texture_id, size, uv0, uv1, frame_padding, bg_col, tint_col) > 0;
		}

		public static void Indent()
		{
			ImGuiNative.igIndent(0f);
		}

		public static void Indent(float indent_w)
		{
			ImGuiNative.igIndent(indent_w);
		}

		public unsafe static bool InputDouble(string label, ref double v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			double num2 = 0.0;
			double num3 = 0.0;
			int byteCount = Encoding.UTF8.GetByteCount("%.6f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.6f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (double* ptr3 = &v)
			{
				double* ptr4 = ptr3;
				int num4 = (int)ImGuiNative.igInputDouble(ptr, ptr4, num2, num3, ptr2, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num4 != 0;
			}
		}

		public unsafe static bool InputDouble(string label, ref double v, double step)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			double num2 = 0.0;
			int byteCount = Encoding.UTF8.GetByteCount("%.6f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.6f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (double* ptr3 = &v)
			{
				double* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igInputDouble(ptr, ptr4, step, num2, ptr2, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool InputDouble(string label, ref double v, double step, double step_fast)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.6f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.6f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (double* ptr3 = &v)
			{
				double* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igInputDouble(ptr, ptr4, step, step_fast, ptr2, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool InputDouble(string label, ref double v, double step, double step_fast, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (double* ptr3 = &v)
			{
				double* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igInputDouble(ptr, ptr4, step, step_fast, ptr2, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool InputDouble(string label, ref double v, double step, double step_fast, string format, ImGuiInputTextFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (double* ptr3 = &v)
			{
				double* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igInputDouble(ptr, ptr4, step, step_fast, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool InputFloat(string label, ref float v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 0f;
			float num3 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num4 = (int)ImGuiNative.igInputFloat(ptr, ptr4, num2, num3, ptr2, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num4 != 0;
			}
		}

		public unsafe static bool InputFloat(string label, ref float v, float step)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 0f;
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igInputFloat(ptr, ptr4, step, num2, ptr2, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool InputFloat(string label, ref float v, float step, float step_fast)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igInputFloat(ptr, ptr4, step, step_fast, ptr2, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool InputFloat(string label, ref float v, float step, float step_fast, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igInputFloat(ptr, ptr4, step, step_fast, ptr2, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool InputFloat(string label, ref float v, float step, float step_fast, string format, ImGuiInputTextFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igInputFloat(ptr, ptr4, step, step_fast, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool InputFloat2(string label, ref Vector2 v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (Vector2* ptr3 = &v)
			{
				Vector2* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igInputFloat2(ptr, ptr4, ptr2, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool InputFloat2(string label, ref Vector2 v, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (Vector2* ptr3 = &v)
			{
				Vector2* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igInputFloat2(ptr, ptr4, ptr2, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool InputFloat2(string label, ref Vector2 v, string format, ImGuiInputTextFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (Vector2* ptr3 = &v)
			{
				Vector2* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igInputFloat2(ptr, ptr4, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool InputFloat3(string label, ref Vector3 v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (Vector3* ptr3 = &v)
			{
				Vector3* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igInputFloat3(ptr, ptr4, ptr2, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool InputFloat3(string label, ref Vector3 v, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (Vector3* ptr3 = &v)
			{
				Vector3* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igInputFloat3(ptr, ptr4, ptr2, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool InputFloat3(string label, ref Vector3 v, string format, ImGuiInputTextFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (Vector3* ptr3 = &v)
			{
				Vector3* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igInputFloat3(ptr, ptr4, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool InputFloat4(string label, ref Vector4 v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (Vector4* ptr3 = &v)
			{
				Vector4* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igInputFloat4(ptr, ptr4, ptr2, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool InputFloat4(string label, ref Vector4 v, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (Vector4* ptr3 = &v)
			{
				Vector4* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igInputFloat4(ptr, ptr4, ptr2, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool InputFloat4(string label, ref Vector4 v, string format, ImGuiInputTextFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (Vector4* ptr3 = &v)
			{
				Vector4* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igInputFloat4(ptr, ptr4, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool InputInt(string label, ref int v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 1;
			int num3 = 100;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (int* ptr2 = &v)
			{
				int* ptr3 = ptr2;
				int num4 = (int)ImGuiNative.igInputInt(ptr, ptr3, num2, num3, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num4 != 0;
			}
		}

		public unsafe static bool InputInt(string label, ref int v, int step)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 100;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (int* ptr2 = &v)
			{
				int* ptr3 = ptr2;
				int num3 = (int)ImGuiNative.igInputInt(ptr, ptr3, step, num2, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool InputInt(string label, ref int v, int step, int step_fast)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (int* ptr2 = &v)
			{
				int* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igInputInt(ptr, ptr3, step, step_fast, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool InputInt(string label, ref int v, int step, int step_fast, ImGuiInputTextFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			fixed (int* ptr2 = &v)
			{
				int* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igInputInt(ptr, ptr3, step, step_fast, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool InputInt2(string label, ref int v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (int* ptr2 = &v)
			{
				int* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igInputInt2(ptr, ptr3, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool InputInt2(string label, ref int v, ImGuiInputTextFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			fixed (int* ptr2 = &v)
			{
				int* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igInputInt2(ptr, ptr3, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool InputInt3(string label, ref int v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (int* ptr2 = &v)
			{
				int* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igInputInt3(ptr, ptr3, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool InputInt3(string label, ref int v, ImGuiInputTextFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			fixed (int* ptr2 = &v)
			{
				int* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igInputInt3(ptr, ptr3, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool InputInt4(string label, ref int v)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			fixed (int* ptr2 = &v)
			{
				int* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igInputInt4(ptr, ptr3, imGuiInputTextFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool InputInt4(string label, ref int v, ImGuiInputTextFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			fixed (int* ptr2 = &v)
			{
				int* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igInputInt4(ptr, ptr3, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool InputScalar(string label, ImGuiDataType data_type, IntPtr p_data)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = null;
			void* ptr4 = null;
			byte* ptr5 = null;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			int num2 = (int)ImGuiNative.igInputScalar(ptr, data_type, ptr2, ptr3, ptr4, ptr5, imGuiInputTextFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool InputScalar(string label, ImGuiDataType data_type, IntPtr p_data, IntPtr p_step)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_step.ToPointer();
			void* ptr4 = null;
			byte* ptr5 = null;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			int num2 = (int)ImGuiNative.igInputScalar(ptr, data_type, ptr2, ptr3, ptr4, ptr5, imGuiInputTextFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool InputScalar(string label, ImGuiDataType data_type, IntPtr p_data, IntPtr p_step, IntPtr p_step_fast)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_step.ToPointer();
			void* ptr4 = p_step_fast.ToPointer();
			byte* ptr5 = null;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			int num2 = (int)ImGuiNative.igInputScalar(ptr, data_type, ptr2, ptr3, ptr4, ptr5, imGuiInputTextFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool InputScalar(string label, ImGuiDataType data_type, IntPtr p_data, IntPtr p_step, IntPtr p_step_fast, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_step.ToPointer();
			void* ptr4 = p_step_fast.ToPointer();
			int num2 = 0;
			byte* ptr5;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr5 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr5 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr5, num2);
				ptr5[utf2] = 0;
			}
			else
			{
				ptr5 = null;
			}
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			int num3 = (int)ImGuiNative.igInputScalar(ptr, data_type, ptr2, ptr3, ptr4, ptr5, imGuiInputTextFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr5);
			}
			return num3 != 0;
		}

		public unsafe static bool InputScalar(string label, ImGuiDataType data_type, IntPtr p_data, IntPtr p_step, IntPtr p_step_fast, string format, ImGuiInputTextFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_step.ToPointer();
			void* ptr4 = p_step_fast.ToPointer();
			int num2 = 0;
			byte* ptr5;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr5 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr5 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr5, num2);
				ptr5[utf2] = 0;
			}
			else
			{
				ptr5 = null;
			}
			int num3 = (int)ImGuiNative.igInputScalar(ptr, data_type, ptr2, ptr3, ptr4, ptr5, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr5);
			}
			return num3 != 0;
		}

		public unsafe static bool InputScalarN(string label, ImGuiDataType data_type, IntPtr p_data, int components)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = null;
			void* ptr4 = null;
			byte* ptr5 = null;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			int num2 = (int)ImGuiNative.igInputScalarN(ptr, data_type, ptr2, components, ptr3, ptr4, ptr5, imGuiInputTextFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool InputScalarN(string label, ImGuiDataType data_type, IntPtr p_data, int components, IntPtr p_step)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_step.ToPointer();
			void* ptr4 = null;
			byte* ptr5 = null;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			int num2 = (int)ImGuiNative.igInputScalarN(ptr, data_type, ptr2, components, ptr3, ptr4, ptr5, imGuiInputTextFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool InputScalarN(string label, ImGuiDataType data_type, IntPtr p_data, int components, IntPtr p_step, IntPtr p_step_fast)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_step.ToPointer();
			void* ptr4 = p_step_fast.ToPointer();
			byte* ptr5 = null;
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			int num2 = (int)ImGuiNative.igInputScalarN(ptr, data_type, ptr2, components, ptr3, ptr4, ptr5, imGuiInputTextFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool InputScalarN(string label, ImGuiDataType data_type, IntPtr p_data, int components, IntPtr p_step, IntPtr p_step_fast, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_step.ToPointer();
			void* ptr4 = p_step_fast.ToPointer();
			int num2 = 0;
			byte* ptr5;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr5 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr5 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr5, num2);
				ptr5[utf2] = 0;
			}
			else
			{
				ptr5 = null;
			}
			ImGuiInputTextFlags imGuiInputTextFlags = ImGuiInputTextFlags.None;
			int num3 = (int)ImGuiNative.igInputScalarN(ptr, data_type, ptr2, components, ptr3, ptr4, ptr5, imGuiInputTextFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr5);
			}
			return num3 != 0;
		}

		public unsafe static bool InputScalarN(string label, ImGuiDataType data_type, IntPtr p_data, int components, IntPtr p_step, IntPtr p_step_fast, string format, ImGuiInputTextFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_step.ToPointer();
			void* ptr4 = p_step_fast.ToPointer();
			int num2 = 0;
			byte* ptr5;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr5 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr5 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr5, num2);
				ptr5[utf2] = 0;
			}
			else
			{
				ptr5 = null;
			}
			int num3 = (int)ImGuiNative.igInputScalarN(ptr, data_type, ptr2, components, ptr3, ptr4, ptr5, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr5);
			}
			return num3 != 0;
		}

		public unsafe static bool InvisibleButton(string str_id, Vector2 size)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiButtonFlags imGuiButtonFlags = ImGuiButtonFlags.None;
			int num2 = (int)ImGuiNative.igInvisibleButton(ptr, size, imGuiButtonFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool InvisibleButton(string str_id, Vector2 size, ImGuiButtonFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igInvisibleButton(ptr, size, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public static bool IsAnyItemActive()
		{
			return ImGuiNative.igIsAnyItemActive() > 0;
		}

		public static bool IsAnyItemFocused()
		{
			return ImGuiNative.igIsAnyItemFocused() > 0;
		}

		public static bool IsAnyItemHovered()
		{
			return ImGuiNative.igIsAnyItemHovered() > 0;
		}

		public static bool IsAnyMouseDown()
		{
			return ImGuiNative.igIsAnyMouseDown() > 0;
		}

		public static bool IsItemActivated()
		{
			return ImGuiNative.igIsItemActivated() > 0;
		}

		public static bool IsItemActive()
		{
			return ImGuiNative.igIsItemActive() > 0;
		}

		public static bool IsItemClicked()
		{
			return ImGuiNative.igIsItemClicked(ImGuiMouseButton.Left) > 0;
		}

		public static bool IsItemClicked(ImGuiMouseButton mouse_button)
		{
			return ImGuiNative.igIsItemClicked(mouse_button) > 0;
		}

		public static bool IsItemDeactivated()
		{
			return ImGuiNative.igIsItemDeactivated() > 0;
		}

		public static bool IsItemDeactivatedAfterEdit()
		{
			return ImGuiNative.igIsItemDeactivatedAfterEdit() > 0;
		}

		public static bool IsItemEdited()
		{
			return ImGuiNative.igIsItemEdited() > 0;
		}

		public static bool IsItemFocused()
		{
			return ImGuiNative.igIsItemFocused() > 0;
		}

		public static bool IsItemHovered()
		{
			return ImGuiNative.igIsItemHovered(ImGuiHoveredFlags.None) > 0;
		}

		public static bool IsItemHovered(ImGuiHoveredFlags flags)
		{
			return ImGuiNative.igIsItemHovered(flags) > 0;
		}

		public static bool IsItemToggledOpen()
		{
			return ImGuiNative.igIsItemToggledOpen() > 0;
		}

		public static bool IsItemVisible()
		{
			return ImGuiNative.igIsItemVisible() > 0;
		}

		public static bool IsKeyDown(int user_key_index)
		{
			return ImGuiNative.igIsKeyDown(user_key_index) > 0;
		}

		public static bool IsKeyPressed(int user_key_index)
		{
			byte b = 1;
			return ImGuiNative.igIsKeyPressed(user_key_index, b) > 0;
		}

		public static bool IsKeyPressed(int user_key_index, bool repeat)
		{
			byte b = (repeat ? 1 : 0);
			return ImGuiNative.igIsKeyPressed(user_key_index, b) > 0;
		}

		public static bool IsKeyReleased(int user_key_index)
		{
			return ImGuiNative.igIsKeyReleased(user_key_index) > 0;
		}

		public static bool IsMouseClicked(ImGuiMouseButton button)
		{
			byte b = 0;
			return ImGuiNative.igIsMouseClicked(button, b) > 0;
		}

		public static bool IsMouseClicked(ImGuiMouseButton button, bool repeat)
		{
			byte b = (repeat ? 1 : 0);
			return ImGuiNative.igIsMouseClicked(button, b) > 0;
		}

		public static bool IsMouseDoubleClicked(ImGuiMouseButton button)
		{
			return ImGuiNative.igIsMouseDoubleClicked(button) > 0;
		}

		public static bool IsMouseDown(ImGuiMouseButton button)
		{
			return ImGuiNative.igIsMouseDown(button) > 0;
		}

		public static bool IsMouseDragging(ImGuiMouseButton button)
		{
			float num = -1f;
			return ImGuiNative.igIsMouseDragging(button, num) > 0;
		}

		public static bool IsMouseDragging(ImGuiMouseButton button, float lock_threshold)
		{
			return ImGuiNative.igIsMouseDragging(button, lock_threshold) > 0;
		}

		public static bool IsMouseHoveringRect(Vector2 r_min, Vector2 r_max)
		{
			byte b = 1;
			return ImGuiNative.igIsMouseHoveringRect(r_min, r_max, b) > 0;
		}

		public static bool IsMouseHoveringRect(Vector2 r_min, Vector2 r_max, bool clip)
		{
			byte b = (clip ? 1 : 0);
			return ImGuiNative.igIsMouseHoveringRect(r_min, r_max, b) > 0;
		}

		public unsafe static bool IsMousePosValid()
		{
			Vector2* ptr = null;
			return ImGuiNative.igIsMousePosValid(ptr) > 0;
		}

		public unsafe static bool IsMousePosValid(ref Vector2 mouse_pos)
		{
			fixed (Vector2* ptr = &mouse_pos)
			{
				return ImGuiNative.igIsMousePosValid(ptr) > 0;
			}
		}

		public static bool IsMouseReleased(ImGuiMouseButton button)
		{
			return ImGuiNative.igIsMouseReleased(button) > 0;
		}

		public unsafe static bool IsPopupOpen(string str_id)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiPopupFlags imGuiPopupFlags = ImGuiPopupFlags.None;
			int num2 = (int)ImGuiNative.igIsPopupOpenStr(ptr, imGuiPopupFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool IsPopupOpen(string str_id, ImGuiPopupFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igIsPopupOpenStr(ptr, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public static bool IsRectVisible(Vector2 size)
		{
			return ImGuiNative.igIsRectVisibleNil(size) > 0;
		}

		public static bool IsRectVisible(Vector2 rect_min, Vector2 rect_max)
		{
			return ImGuiNative.igIsRectVisibleVec2(rect_min, rect_max) > 0;
		}

		public static bool IsWindowAppearing()
		{
			return ImGuiNative.igIsWindowAppearing() > 0;
		}

		public static bool IsWindowCollapsed()
		{
			return ImGuiNative.igIsWindowCollapsed() > 0;
		}

		public static bool IsWindowDocked()
		{
			return ImGuiNative.igIsWindowDocked() > 0;
		}

		public static bool IsWindowFocused()
		{
			return ImGuiNative.igIsWindowFocused(ImGuiFocusedFlags.None) > 0;
		}

		public static bool IsWindowFocused(ImGuiFocusedFlags flags)
		{
			return ImGuiNative.igIsWindowFocused(flags) > 0;
		}

		public static bool IsWindowHovered()
		{
			return ImGuiNative.igIsWindowHovered(ImGuiHoveredFlags.None) > 0;
		}

		public static bool IsWindowHovered(ImGuiHoveredFlags flags)
		{
			return ImGuiNative.igIsWindowHovered(flags) > 0;
		}

		public unsafe static void LabelText(string label, string fmt)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (fmt != null)
			{
				num2 = Encoding.UTF8.GetByteCount(fmt);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(fmt, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiNative.igLabelText(ptr, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr2);
			}
		}

		public unsafe static bool ListBox(string label, ref int current_item, string[] items, int items_count)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int* ptr2;
			int num2;
			checked
			{
				ptr2 = stackalloc int[unchecked((UIntPtr)items.Length) * 4];
				num2 = 0;
			}
			for (int i = 0; i < items.Length; i++)
			{
				string text = items[i];
				ptr2[i] = Encoding.UTF8.GetByteCount(text);
				num2 += ptr2[i] + 1;
			}
			byte* ptr3 = stackalloc byte[(UIntPtr)num2];
			int num3 = 0;
			for (int j = 0; j < items.Length; j++)
			{
				string text2 = items[j];
				fixed (string text3 = text2)
				{
					char* ptr4 = text3;
					if (ptr4 != null)
					{
						ptr4 += RuntimeHelpers.OffsetToStringData / 2;
					}
					num3 += Encoding.UTF8.GetBytes(ptr4, text2.Length, ptr3 + num3, ptr2[j]);
					ptr3[num3] = 0;
					num3++;
				}
			}
			byte** ptr5;
			checked
			{
				ptr5 = stackalloc byte*[unchecked((UIntPtr)items.Length) * (UIntPtr)sizeof(byte*)];
				num3 = 0;
			}
			for (int k = 0; k < items.Length; k++)
			{
				*(IntPtr*)(ptr5 + (IntPtr)k * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr3 + num3;
				num3 += ptr2[k] + 1;
			}
			int num4 = -1;
			fixed (int* ptr6 = &current_item)
			{
				int* ptr7 = ptr6;
				int num5 = (int)ImGuiNative.igListBoxStr_arr(ptr, ptr7, ptr5, items_count, num4);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num5 != 0;
			}
		}

		public unsafe static bool ListBox(string label, ref int current_item, string[] items, int items_count, int height_in_items)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int* ptr2;
			int num2;
			checked
			{
				ptr2 = stackalloc int[unchecked((UIntPtr)items.Length) * 4];
				num2 = 0;
			}
			for (int i = 0; i < items.Length; i++)
			{
				string text = items[i];
				ptr2[i] = Encoding.UTF8.GetByteCount(text);
				num2 += ptr2[i] + 1;
			}
			byte* ptr3 = stackalloc byte[(UIntPtr)num2];
			int num3 = 0;
			for (int j = 0; j < items.Length; j++)
			{
				string text2 = items[j];
				fixed (string text3 = text2)
				{
					char* ptr4 = text3;
					if (ptr4 != null)
					{
						ptr4 += RuntimeHelpers.OffsetToStringData / 2;
					}
					num3 += Encoding.UTF8.GetBytes(ptr4, text2.Length, ptr3 + num3, ptr2[j]);
					ptr3[num3] = 0;
					num3++;
				}
			}
			byte** ptr5;
			checked
			{
				ptr5 = stackalloc byte*[unchecked((UIntPtr)items.Length) * (UIntPtr)sizeof(byte*)];
				num3 = 0;
			}
			for (int k = 0; k < items.Length; k++)
			{
				*(IntPtr*)(ptr5 + (IntPtr)k * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr3 + num3;
				num3 += ptr2[k] + 1;
			}
			fixed (int* ptr6 = &current_item)
			{
				int* ptr7 = ptr6;
				int num4 = (int)ImGuiNative.igListBoxStr_arr(ptr, ptr7, ptr5, items_count, height_in_items);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num4 != 0;
			}
		}

		public unsafe static void LoadIniSettingsFromDisk(string ini_filename)
		{
			int num = 0;
			byte* ptr;
			if (ini_filename != null)
			{
				num = Encoding.UTF8.GetByteCount(ini_filename);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(ini_filename, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igLoadIniSettingsFromDisk(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void LoadIniSettingsFromMemory(string ini_data)
		{
			int num = 0;
			byte* ptr;
			if (ini_data != null)
			{
				num = Encoding.UTF8.GetByteCount(ini_data);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(ini_data, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			uint num2 = 0U;
			ImGuiNative.igLoadIniSettingsFromMemory(ptr, num2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void LoadIniSettingsFromMemory(string ini_data, uint ini_size)
		{
			int num = 0;
			byte* ptr;
			if (ini_data != null)
			{
				num = Encoding.UTF8.GetByteCount(ini_data);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(ini_data, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igLoadIniSettingsFromMemory(ptr, ini_size);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public static void LogButtons()
		{
			ImGuiNative.igLogButtons();
		}

		public static void LogFinish()
		{
			ImGuiNative.igLogFinish();
		}

		public unsafe static void LogText(string fmt)
		{
			int num = 0;
			byte* ptr;
			if (fmt != null)
			{
				num = Encoding.UTF8.GetByteCount(fmt);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(fmt, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igLogText(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public static void LogToClipboard()
		{
			ImGuiNative.igLogToClipboard(-1);
		}

		public static void LogToClipboard(int auto_open_depth)
		{
			ImGuiNative.igLogToClipboard(auto_open_depth);
		}

		public unsafe static void LogToFile()
		{
			int num = -1;
			byte* ptr = null;
			ImGuiNative.igLogToFile(num, ptr);
		}

		public unsafe static void LogToFile(int auto_open_depth)
		{
			byte* ptr = null;
			ImGuiNative.igLogToFile(auto_open_depth, ptr);
		}

		public unsafe static void LogToFile(int auto_open_depth, string filename)
		{
			int num = 0;
			byte* ptr;
			if (filename != null)
			{
				num = Encoding.UTF8.GetByteCount(filename);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(filename, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igLogToFile(auto_open_depth, ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public static void LogToTTY()
		{
			ImGuiNative.igLogToTTY(-1);
		}

		public static void LogToTTY(int auto_open_depth)
		{
			ImGuiNative.igLogToTTY(auto_open_depth);
		}

		public static IntPtr MemAlloc(uint size)
		{
			return (IntPtr)ImGuiNative.igMemAlloc(size);
		}

		public static void MemFree(IntPtr ptr)
		{
			ImGuiNative.igMemFree(ptr.ToPointer());
		}

		public unsafe static bool MenuItem(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte* ptr2 = null;
			byte b = 0;
			byte b2 = 1;
			int num2 = (int)ImGuiNative.igMenuItemBool(ptr, ptr2, b, b2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool MenuItem(string label, string shortcut)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (shortcut != null)
			{
				num2 = Encoding.UTF8.GetByteCount(shortcut);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(shortcut, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			byte b = 0;
			byte b2 = 1;
			int num3 = (int)ImGuiNative.igMenuItemBool(ptr, ptr2, b, b2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr2);
			}
			return num3 != 0;
		}

		public unsafe static bool MenuItem(string label, string shortcut, bool selected)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (shortcut != null)
			{
				num2 = Encoding.UTF8.GetByteCount(shortcut);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(shortcut, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			byte b = (selected ? 1 : 0);
			byte b2 = 1;
			int num3 = (int)ImGuiNative.igMenuItemBool(ptr, ptr2, b, b2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr2);
			}
			return num3 != 0;
		}

		public unsafe static bool MenuItem(string label, string shortcut, bool selected, bool enabled)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (shortcut != null)
			{
				num2 = Encoding.UTF8.GetByteCount(shortcut);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(shortcut, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			byte b = (selected ? 1 : 0);
			byte b2 = (enabled ? 1 : 0);
			int num3 = (int)ImGuiNative.igMenuItemBool(ptr, ptr2, b, b2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr2);
			}
			return num3 != 0;
		}

		public unsafe static bool MenuItem(string label, string shortcut, ref bool p_selected)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (shortcut != null)
			{
				num2 = Encoding.UTF8.GetByteCount(shortcut);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(shortcut, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			byte b = (p_selected ? 1 : 0);
			byte* ptr3 = &b;
			byte b2 = 1;
			int num3 = (int)ImGuiNative.igMenuItemBoolPtr(ptr, ptr2, ptr3, b2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr2);
			}
			p_selected = b > 0;
			return num3 != 0;
		}

		public unsafe static bool MenuItem(string label, string shortcut, ref bool p_selected, bool enabled)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (shortcut != null)
			{
				num2 = Encoding.UTF8.GetByteCount(shortcut);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(shortcut, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			byte b = (p_selected ? 1 : 0);
			byte* ptr3 = &b;
			byte b2 = (enabled ? 1 : 0);
			int num3 = (int)ImGuiNative.igMenuItemBoolPtr(ptr, ptr2, ptr3, b2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr2);
			}
			p_selected = b > 0;
			return num3 != 0;
		}

		public static void NewFrame()
		{
			ImGuiNative.igNewFrame();
		}

		public static void NewLine()
		{
			ImGuiNative.igNewLine();
		}

		public static void NextColumn()
		{
			ImGuiNative.igNextColumn();
		}

		public unsafe static void OpenPopup(string str_id)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiPopupFlags imGuiPopupFlags = ImGuiPopupFlags.None;
			ImGuiNative.igOpenPopup(ptr, imGuiPopupFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void OpenPopup(string str_id, ImGuiPopupFlags popup_flags)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igOpenPopup(ptr, popup_flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void OpenPopupOnItemClick()
		{
			byte* ptr = null;
			ImGuiPopupFlags imGuiPopupFlags = ImGuiPopupFlags.MouseButtonRight;
			ImGuiNative.igOpenPopupOnItemClick(ptr, imGuiPopupFlags);
		}

		public unsafe static void OpenPopupOnItemClick(string str_id)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiPopupFlags imGuiPopupFlags = ImGuiPopupFlags.MouseButtonRight;
			ImGuiNative.igOpenPopupOnItemClick(ptr, imGuiPopupFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void OpenPopupOnItemClick(string str_id, ImGuiPopupFlags popup_flags)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igOpenPopupOnItemClick(ptr, popup_flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void PlotHistogram(string label, ref float values, int values_count)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2 = null;
			float maxValue = float.MaxValue;
			float maxValue2 = float.MaxValue;
			Vector2 vector = default(Vector2);
			int num3 = 4;
			fixed (float* ptr3 = &values)
			{
				float* ptr4 = ptr3;
				ImGuiNative.igPlotHistogramFloatPtr(ptr, ptr4, values_count, num2, ptr2, maxValue, maxValue2, vector, num3);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
			}
		}

		public unsafe static void PlotHistogram(string label, ref float values, int values_count, int values_offset)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte* ptr2 = null;
			float maxValue = float.MaxValue;
			float maxValue2 = float.MaxValue;
			Vector2 vector = default(Vector2);
			int num2 = 4;
			fixed (float* ptr3 = &values)
			{
				float* ptr4 = ptr3;
				ImGuiNative.igPlotHistogramFloatPtr(ptr, ptr4, values_count, values_offset, ptr2, maxValue, maxValue2, vector, num2);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
			}
		}

		public unsafe static void PlotHistogram(string label, ref float values, int values_count, int values_offset, string overlay_text)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (overlay_text != null)
			{
				num2 = Encoding.UTF8.GetByteCount(overlay_text);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(overlay_text, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			float maxValue = float.MaxValue;
			float maxValue2 = float.MaxValue;
			Vector2 vector = default(Vector2);
			int num3 = 4;
			fixed (float* ptr3 = &values)
			{
				float* ptr4 = ptr3;
				ImGuiNative.igPlotHistogramFloatPtr(ptr, ptr4, values_count, values_offset, ptr2, maxValue, maxValue2, vector, num3);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
			}
		}

		public unsafe static void PlotHistogram(string label, ref float values, int values_count, int values_offset, string overlay_text, float scale_min)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (overlay_text != null)
			{
				num2 = Encoding.UTF8.GetByteCount(overlay_text);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(overlay_text, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			float maxValue = float.MaxValue;
			Vector2 vector = default(Vector2);
			int num3 = 4;
			fixed (float* ptr3 = &values)
			{
				float* ptr4 = ptr3;
				ImGuiNative.igPlotHistogramFloatPtr(ptr, ptr4, values_count, values_offset, ptr2, scale_min, maxValue, vector, num3);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
			}
		}

		public unsafe static void PlotHistogram(string label, ref float values, int values_count, int values_offset, string overlay_text, float scale_min, float scale_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (overlay_text != null)
			{
				num2 = Encoding.UTF8.GetByteCount(overlay_text);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(overlay_text, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			Vector2 vector = default(Vector2);
			int num3 = 4;
			fixed (float* ptr3 = &values)
			{
				float* ptr4 = ptr3;
				ImGuiNative.igPlotHistogramFloatPtr(ptr, ptr4, values_count, values_offset, ptr2, scale_min, scale_max, vector, num3);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
			}
		}

		public unsafe static void PlotHistogram(string label, ref float values, int values_count, int values_offset, string overlay_text, float scale_min, float scale_max, Vector2 graph_size)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (overlay_text != null)
			{
				num2 = Encoding.UTF8.GetByteCount(overlay_text);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(overlay_text, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			int num3 = 4;
			fixed (float* ptr3 = &values)
			{
				float* ptr4 = ptr3;
				ImGuiNative.igPlotHistogramFloatPtr(ptr, ptr4, values_count, values_offset, ptr2, scale_min, scale_max, graph_size, num3);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
			}
		}

		public unsafe static void PlotHistogram(string label, ref float values, int values_count, int values_offset, string overlay_text, float scale_min, float scale_max, Vector2 graph_size, int stride)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (overlay_text != null)
			{
				num2 = Encoding.UTF8.GetByteCount(overlay_text);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(overlay_text, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (float* ptr3 = &values)
			{
				float* ptr4 = ptr3;
				ImGuiNative.igPlotHistogramFloatPtr(ptr, ptr4, values_count, values_offset, ptr2, scale_min, scale_max, graph_size, stride);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
			}
		}

		public unsafe static void PlotLines(string label, ref float values, int values_count)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2 = null;
			float maxValue = float.MaxValue;
			float maxValue2 = float.MaxValue;
			Vector2 vector = default(Vector2);
			int num3 = 4;
			fixed (float* ptr3 = &values)
			{
				float* ptr4 = ptr3;
				ImGuiNative.igPlotLinesFloatPtr(ptr, ptr4, values_count, num2, ptr2, maxValue, maxValue2, vector, num3);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
			}
		}

		public unsafe static void PlotLines(string label, ref float values, int values_count, int values_offset)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte* ptr2 = null;
			float maxValue = float.MaxValue;
			float maxValue2 = float.MaxValue;
			Vector2 vector = default(Vector2);
			int num2 = 4;
			fixed (float* ptr3 = &values)
			{
				float* ptr4 = ptr3;
				ImGuiNative.igPlotLinesFloatPtr(ptr, ptr4, values_count, values_offset, ptr2, maxValue, maxValue2, vector, num2);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
			}
		}

		public unsafe static void PlotLines(string label, ref float values, int values_count, int values_offset, string overlay_text)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (overlay_text != null)
			{
				num2 = Encoding.UTF8.GetByteCount(overlay_text);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(overlay_text, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			float maxValue = float.MaxValue;
			float maxValue2 = float.MaxValue;
			Vector2 vector = default(Vector2);
			int num3 = 4;
			fixed (float* ptr3 = &values)
			{
				float* ptr4 = ptr3;
				ImGuiNative.igPlotLinesFloatPtr(ptr, ptr4, values_count, values_offset, ptr2, maxValue, maxValue2, vector, num3);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
			}
		}

		public unsafe static void PlotLines(string label, ref float values, int values_count, int values_offset, string overlay_text, float scale_min)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (overlay_text != null)
			{
				num2 = Encoding.UTF8.GetByteCount(overlay_text);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(overlay_text, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			float maxValue = float.MaxValue;
			Vector2 vector = default(Vector2);
			int num3 = 4;
			fixed (float* ptr3 = &values)
			{
				float* ptr4 = ptr3;
				ImGuiNative.igPlotLinesFloatPtr(ptr, ptr4, values_count, values_offset, ptr2, scale_min, maxValue, vector, num3);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
			}
		}

		public unsafe static void PlotLines(string label, ref float values, int values_count, int values_offset, string overlay_text, float scale_min, float scale_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (overlay_text != null)
			{
				num2 = Encoding.UTF8.GetByteCount(overlay_text);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(overlay_text, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			Vector2 vector = default(Vector2);
			int num3 = 4;
			fixed (float* ptr3 = &values)
			{
				float* ptr4 = ptr3;
				ImGuiNative.igPlotLinesFloatPtr(ptr, ptr4, values_count, values_offset, ptr2, scale_min, scale_max, vector, num3);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
			}
		}

		public unsafe static void PlotLines(string label, ref float values, int values_count, int values_offset, string overlay_text, float scale_min, float scale_max, Vector2 graph_size)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (overlay_text != null)
			{
				num2 = Encoding.UTF8.GetByteCount(overlay_text);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(overlay_text, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			int num3 = 4;
			fixed (float* ptr3 = &values)
			{
				float* ptr4 = ptr3;
				ImGuiNative.igPlotLinesFloatPtr(ptr, ptr4, values_count, values_offset, ptr2, scale_min, scale_max, graph_size, num3);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
			}
		}

		public unsafe static void PlotLines(string label, ref float values, int values_count, int values_offset, string overlay_text, float scale_min, float scale_max, Vector2 graph_size, int stride)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (overlay_text != null)
			{
				num2 = Encoding.UTF8.GetByteCount(overlay_text);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(overlay_text, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (float* ptr3 = &values)
			{
				float* ptr4 = ptr3;
				ImGuiNative.igPlotLinesFloatPtr(ptr, ptr4, values_count, values_offset, ptr2, scale_min, scale_max, graph_size, stride);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
			}
		}

		public static void PopAllowKeyboardFocus()
		{
			ImGuiNative.igPopAllowKeyboardFocus();
		}

		public static void PopButtonRepeat()
		{
			ImGuiNative.igPopButtonRepeat();
		}

		public static void PopClipRect()
		{
			ImGuiNative.igPopClipRect();
		}

		public static void PopFont()
		{
			ImGuiNative.igPopFont();
		}

		public static void PopID()
		{
			ImGuiNative.igPopID();
		}

		public static void PopItemWidth()
		{
			ImGuiNative.igPopItemWidth();
		}

		public static void PopStyleColor()
		{
			ImGuiNative.igPopStyleColor(1);
		}

		public static void PopStyleColor(int count)
		{
			ImGuiNative.igPopStyleColor(count);
		}

		public static void PopStyleVar()
		{
			ImGuiNative.igPopStyleVar(1);
		}

		public static void PopStyleVar(int count)
		{
			ImGuiNative.igPopStyleVar(count);
		}

		public static void PopTextWrapPos()
		{
			ImGuiNative.igPopTextWrapPos();
		}

		public unsafe static void ProgressBar(float fraction)
		{
			Vector2 vector = new Vector2(float.MaxValue, 0f);
			byte* ptr = null;
			ImGuiNative.igProgressBar(fraction, vector, ptr);
		}

		public unsafe static void ProgressBar(float fraction, Vector2 size_arg)
		{
			byte* ptr = null;
			ImGuiNative.igProgressBar(fraction, size_arg, ptr);
		}

		public unsafe static void ProgressBar(float fraction, Vector2 size_arg, string overlay)
		{
			int num = 0;
			byte* ptr;
			if (overlay != null)
			{
				num = Encoding.UTF8.GetByteCount(overlay);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(overlay, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igProgressBar(fraction, size_arg, ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public static void PushAllowKeyboardFocus(bool allow_keyboard_focus)
		{
			ImGuiNative.igPushAllowKeyboardFocus(allow_keyboard_focus ? 1 : 0);
		}

		public static void PushButtonRepeat(bool repeat)
		{
			ImGuiNative.igPushButtonRepeat(repeat ? 1 : 0);
		}

		public static void PushClipRect(Vector2 clip_rect_min, Vector2 clip_rect_max, bool intersect_with_current_clip_rect)
		{
			byte b = (intersect_with_current_clip_rect ? 1 : 0);
			ImGuiNative.igPushClipRect(clip_rect_min, clip_rect_max, b);
		}

		public static void PushFont(ImFontPtr font)
		{
			ImGuiNative.igPushFont(font.NativePtr);
		}

		public unsafe static void PushID(string str_id)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igPushIDStr(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public static void PushID(IntPtr ptr_id)
		{
			ImGuiNative.igPushIDPtr(ptr_id.ToPointer());
		}

		public static void PushID(int int_id)
		{
			ImGuiNative.igPushIDInt(int_id);
		}

		public static void PushItemWidth(float item_width)
		{
			ImGuiNative.igPushItemWidth(item_width);
		}

		public static void PushStyleColor(ImGuiCol idx, uint col)
		{
			ImGuiNative.igPushStyleColorU32(idx, col);
		}

		public static void PushStyleColor(ImGuiCol idx, Vector4 col)
		{
			ImGuiNative.igPushStyleColorVec4(idx, col);
		}

		public static void PushStyleVar(ImGuiStyleVar idx, float val)
		{
			ImGuiNative.igPushStyleVarFloat(idx, val);
		}

		public static void PushStyleVar(ImGuiStyleVar idx, Vector2 val)
		{
			ImGuiNative.igPushStyleVarVec2(idx, val);
		}

		public static void PushTextWrapPos()
		{
			ImGuiNative.igPushTextWrapPos(0f);
		}

		public static void PushTextWrapPos(float wrap_local_pos_x)
		{
			ImGuiNative.igPushTextWrapPos(wrap_local_pos_x);
		}

		public unsafe static bool RadioButton(string label, bool active)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (active ? 1 : 0);
			int num2 = (int)ImGuiNative.igRadioButtonBool(ptr, b);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool RadioButton(string label, ref int v, int v_button)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			fixed (int* ptr2 = &v)
			{
				int* ptr3 = ptr2;
				int num2 = (int)ImGuiNative.igRadioButtonIntPtr(ptr, ptr3, v_button);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				return num2 != 0;
			}
		}

		public static void Render()
		{
			ImGuiNative.igRender();
		}

		public unsafe static void RenderPlatformWindowsDefault()
		{
			void* ptr = null;
			void* ptr2 = null;
			ImGuiNative.igRenderPlatformWindowsDefault(ptr, ptr2);
		}

		public unsafe static void RenderPlatformWindowsDefault(IntPtr platform_render_arg)
		{
			void* ptr = platform_render_arg.ToPointer();
			void* ptr2 = null;
			ImGuiNative.igRenderPlatformWindowsDefault(ptr, ptr2);
		}

		public unsafe static void RenderPlatformWindowsDefault(IntPtr platform_render_arg, IntPtr renderer_render_arg)
		{
			void* ptr = platform_render_arg.ToPointer();
			void* ptr2 = renderer_render_arg.ToPointer();
			ImGuiNative.igRenderPlatformWindowsDefault(ptr, ptr2);
		}

		public static void ResetMouseDragDelta()
		{
			ImGuiNative.igResetMouseDragDelta(ImGuiMouseButton.Left);
		}

		public static void ResetMouseDragDelta(ImGuiMouseButton button)
		{
			ImGuiNative.igResetMouseDragDelta(button);
		}

		public static void SameLine()
		{
			float num = 0f;
			float num2 = -1f;
			ImGuiNative.igSameLine(num, num2);
		}

		public static void SameLine(float offset_from_start_x)
		{
			float num = -1f;
			ImGuiNative.igSameLine(offset_from_start_x, num);
		}

		public static void SameLine(float offset_from_start_x, float spacing)
		{
			ImGuiNative.igSameLine(offset_from_start_x, spacing);
		}

		public unsafe static void SaveIniSettingsToDisk(string ini_filename)
		{
			int num = 0;
			byte* ptr;
			if (ini_filename != null)
			{
				num = Encoding.UTF8.GetByteCount(ini_filename);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(ini_filename, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igSaveIniSettingsToDisk(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static string SaveIniSettingsToMemory()
		{
			uint* ptr = null;
			return Util.StringFromPtr(ImGuiNative.igSaveIniSettingsToMemory(ptr));
		}

		public unsafe static string SaveIniSettingsToMemory(out uint out_ini_size)
		{
			fixed (uint* ptr = &out_ini_size)
			{
				return Util.StringFromPtr(ImGuiNative.igSaveIniSettingsToMemory(ptr));
			}
		}

		public unsafe static bool Selectable(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = 0;
			ImGuiSelectableFlags imGuiSelectableFlags = ImGuiSelectableFlags.None;
			int num2 = (int)ImGuiNative.igSelectableBool(ptr, b, imGuiSelectableFlags, default(Vector2));
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool Selectable(string label, bool selected)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (selected ? 1 : 0);
			ImGuiSelectableFlags imGuiSelectableFlags = ImGuiSelectableFlags.None;
			int num2 = (int)ImGuiNative.igSelectableBool(ptr, b, imGuiSelectableFlags, default(Vector2));
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool Selectable(string label, bool selected, ImGuiSelectableFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (selected ? 1 : 0);
			int num2 = (int)ImGuiNative.igSelectableBool(ptr, b, flags, default(Vector2));
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool Selectable(string label, bool selected, ImGuiSelectableFlags flags, Vector2 size)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (selected ? 1 : 0);
			int num2 = (int)ImGuiNative.igSelectableBool(ptr, b, flags, size);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool Selectable(string label, ref bool p_selected)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (p_selected ? 1 : 0);
			byte* ptr2 = &b;
			ImGuiSelectableFlags imGuiSelectableFlags = ImGuiSelectableFlags.None;
			int num2 = (int)ImGuiNative.igSelectableBoolPtr(ptr, ptr2, imGuiSelectableFlags, default(Vector2));
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			p_selected = b > 0;
			return num2 != 0;
		}

		public unsafe static bool Selectable(string label, ref bool p_selected, ImGuiSelectableFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (p_selected ? 1 : 0);
			byte* ptr2 = &b;
			int num2 = (int)ImGuiNative.igSelectableBoolPtr(ptr, ptr2, flags, default(Vector2));
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			p_selected = b > 0;
			return num2 != 0;
		}

		public unsafe static bool Selectable(string label, ref bool p_selected, ImGuiSelectableFlags flags, Vector2 size)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (p_selected ? 1 : 0);
			byte* ptr2 = &b;
			int num2 = (int)ImGuiNative.igSelectableBoolPtr(ptr, ptr2, flags, size);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			p_selected = b > 0;
			return num2 != 0;
		}

		public static void Separator()
		{
			ImGuiNative.igSeparator();
		}

		public unsafe static void SetAllocatorFunctions(IntPtr alloc_func, IntPtr free_func)
		{
			void* ptr = null;
			ImGuiNative.igSetAllocatorFunctions(alloc_func, free_func, ptr);
		}

		public unsafe static void SetAllocatorFunctions(IntPtr alloc_func, IntPtr free_func, IntPtr user_data)
		{
			void* ptr = user_data.ToPointer();
			ImGuiNative.igSetAllocatorFunctions(alloc_func, free_func, ptr);
		}

		public unsafe static void SetClipboardText(string text)
		{
			int num = 0;
			byte* ptr;
			if (text != null)
			{
				num = Encoding.UTF8.GetByteCount(text);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(text, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igSetClipboardText(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public static void SetColorEditOptions(ImGuiColorEditFlags flags)
		{
			ImGuiNative.igSetColorEditOptions(flags);
		}

		public static void SetColumnOffset(int column_index, float offset_x)
		{
			ImGuiNative.igSetColumnOffset(column_index, offset_x);
		}

		public static void SetColumnWidth(int column_index, float width)
		{
			ImGuiNative.igSetColumnWidth(column_index, width);
		}

		public static void SetCurrentContext(IntPtr ctx)
		{
			ImGuiNative.igSetCurrentContext(ctx);
		}

		public static void SetCursorPos(Vector2 local_pos)
		{
			ImGuiNative.igSetCursorPos(local_pos);
		}

		public static void SetCursorPosX(float local_x)
		{
			ImGuiNative.igSetCursorPosX(local_x);
		}

		public static void SetCursorPosY(float local_y)
		{
			ImGuiNative.igSetCursorPosY(local_y);
		}

		public static void SetCursorScreenPos(Vector2 pos)
		{
			ImGuiNative.igSetCursorScreenPos(pos);
		}

		public unsafe static bool SetDragDropPayload(string type, IntPtr data, uint sz)
		{
			int num = 0;
			byte* ptr;
			if (type != null)
			{
				num = Encoding.UTF8.GetByteCount(type);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(type, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = data.ToPointer();
			ImGuiCond imGuiCond = ImGuiCond.None;
			int num2 = (int)ImGuiNative.igSetDragDropPayload(ptr, ptr2, sz, imGuiCond);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool SetDragDropPayload(string type, IntPtr data, uint sz, ImGuiCond cond)
		{
			int num = 0;
			byte* ptr;
			if (type != null)
			{
				num = Encoding.UTF8.GetByteCount(type);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(type, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = data.ToPointer();
			int num2 = (int)ImGuiNative.igSetDragDropPayload(ptr, ptr2, sz, cond);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public static void SetItemAllowOverlap()
		{
			ImGuiNative.igSetItemAllowOverlap();
		}

		public static void SetItemDefaultFocus()
		{
			ImGuiNative.igSetItemDefaultFocus();
		}

		public static void SetKeyboardFocusHere()
		{
			ImGuiNative.igSetKeyboardFocusHere(0);
		}

		public static void SetKeyboardFocusHere(int offset)
		{
			ImGuiNative.igSetKeyboardFocusHere(offset);
		}

		public static void SetMouseCursor(ImGuiMouseCursor cursor_type)
		{
			ImGuiNative.igSetMouseCursor(cursor_type);
		}

		public static void SetNextItemOpen(bool is_open)
		{
			byte b = (is_open ? 1 : 0);
			ImGuiCond imGuiCond = ImGuiCond.None;
			ImGuiNative.igSetNextItemOpen(b, imGuiCond);
		}

		public static void SetNextItemOpen(bool is_open, ImGuiCond cond)
		{
			ImGuiNative.igSetNextItemOpen(is_open ? 1 : 0, cond);
		}

		public static void SetNextItemWidth(float item_width)
		{
			ImGuiNative.igSetNextItemWidth(item_width);
		}

		public static void SetNextWindowBgAlpha(float alpha)
		{
			ImGuiNative.igSetNextWindowBgAlpha(alpha);
		}

		public static void SetNextWindowClass(ImGuiWindowClassPtr window_class)
		{
			ImGuiNative.igSetNextWindowClass(window_class.NativePtr);
		}

		public static void SetNextWindowCollapsed(bool collapsed)
		{
			byte b = (collapsed ? 1 : 0);
			ImGuiCond imGuiCond = ImGuiCond.None;
			ImGuiNative.igSetNextWindowCollapsed(b, imGuiCond);
		}

		public static void SetNextWindowCollapsed(bool collapsed, ImGuiCond cond)
		{
			ImGuiNative.igSetNextWindowCollapsed(collapsed ? 1 : 0, cond);
		}

		public static void SetNextWindowContentSize(Vector2 size)
		{
			ImGuiNative.igSetNextWindowContentSize(size);
		}

		public static void SetNextWindowDockID(uint dock_id)
		{
			ImGuiCond imGuiCond = ImGuiCond.None;
			ImGuiNative.igSetNextWindowDockID(dock_id, imGuiCond);
		}

		public static void SetNextWindowDockID(uint dock_id, ImGuiCond cond)
		{
			ImGuiNative.igSetNextWindowDockID(dock_id, cond);
		}

		public static void SetNextWindowFocus()
		{
			ImGuiNative.igSetNextWindowFocus();
		}

		public static void SetNextWindowPos(Vector2 pos)
		{
			ImGuiCond imGuiCond = ImGuiCond.None;
			ImGuiNative.igSetNextWindowPos(pos, imGuiCond, default(Vector2));
		}

		public static void SetNextWindowPos(Vector2 pos, ImGuiCond cond)
		{
			ImGuiNative.igSetNextWindowPos(pos, cond, default(Vector2));
		}

		public static void SetNextWindowPos(Vector2 pos, ImGuiCond cond, Vector2 pivot)
		{
			ImGuiNative.igSetNextWindowPos(pos, cond, pivot);
		}

		public static void SetNextWindowSize(Vector2 size)
		{
			ImGuiCond imGuiCond = ImGuiCond.None;
			ImGuiNative.igSetNextWindowSize(size, imGuiCond);
		}

		public static void SetNextWindowSize(Vector2 size, ImGuiCond cond)
		{
			ImGuiNative.igSetNextWindowSize(size, cond);
		}

		public unsafe static void SetNextWindowSizeConstraints(Vector2 size_min, Vector2 size_max)
		{
			ImGuiSizeCallback imGuiSizeCallback = null;
			void* ptr = null;
			ImGuiNative.igSetNextWindowSizeConstraints(size_min, size_max, imGuiSizeCallback, ptr);
		}

		public unsafe static void SetNextWindowSizeConstraints(Vector2 size_min, Vector2 size_max, ImGuiSizeCallback custom_callback)
		{
			void* ptr = null;
			ImGuiNative.igSetNextWindowSizeConstraints(size_min, size_max, custom_callback, ptr);
		}

		public unsafe static void SetNextWindowSizeConstraints(Vector2 size_min, Vector2 size_max, ImGuiSizeCallback custom_callback, IntPtr custom_callback_data)
		{
			void* ptr = custom_callback_data.ToPointer();
			ImGuiNative.igSetNextWindowSizeConstraints(size_min, size_max, custom_callback, ptr);
		}

		public static void SetNextWindowViewport(uint viewport_id)
		{
			ImGuiNative.igSetNextWindowViewport(viewport_id);
		}

		public static void SetScrollFromPosX(float local_x)
		{
			float num = 0.5f;
			ImGuiNative.igSetScrollFromPosXFloat(local_x, num);
		}

		public static void SetScrollFromPosX(float local_x, float center_x_ratio)
		{
			ImGuiNative.igSetScrollFromPosXFloat(local_x, center_x_ratio);
		}

		public static void SetScrollFromPosY(float local_y)
		{
			float num = 0.5f;
			ImGuiNative.igSetScrollFromPosYFloat(local_y, num);
		}

		public static void SetScrollFromPosY(float local_y, float center_y_ratio)
		{
			ImGuiNative.igSetScrollFromPosYFloat(local_y, center_y_ratio);
		}

		public static void SetScrollHereX()
		{
			ImGuiNative.igSetScrollHereX(0.5f);
		}

		public static void SetScrollHereX(float center_x_ratio)
		{
			ImGuiNative.igSetScrollHereX(center_x_ratio);
		}

		public static void SetScrollHereY()
		{
			ImGuiNative.igSetScrollHereY(0.5f);
		}

		public static void SetScrollHereY(float center_y_ratio)
		{
			ImGuiNative.igSetScrollHereY(center_y_ratio);
		}

		public static void SetScrollX(float scroll_x)
		{
			ImGuiNative.igSetScrollXFloat(scroll_x);
		}

		public static void SetScrollY(float scroll_y)
		{
			ImGuiNative.igSetScrollYFloat(scroll_y);
		}

		public static void SetStateStorage(ImGuiStoragePtr storage)
		{
			ImGuiNative.igSetStateStorage(storage.NativePtr);
		}

		public unsafe static void SetTabItemClosed(string tab_or_docked_window_label)
		{
			int num = 0;
			byte* ptr;
			if (tab_or_docked_window_label != null)
			{
				num = Encoding.UTF8.GetByteCount(tab_or_docked_window_label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(tab_or_docked_window_label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igSetTabItemClosed(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void SetTooltip(string fmt)
		{
			int num = 0;
			byte* ptr;
			if (fmt != null)
			{
				num = Encoding.UTF8.GetByteCount(fmt);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(fmt, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igSetTooltip(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public static void SetWindowCollapsed(bool collapsed)
		{
			byte b = (collapsed ? 1 : 0);
			ImGuiCond imGuiCond = ImGuiCond.None;
			ImGuiNative.igSetWindowCollapsedBool(b, imGuiCond);
		}

		public static void SetWindowCollapsed(bool collapsed, ImGuiCond cond)
		{
			ImGuiNative.igSetWindowCollapsedBool(collapsed ? 1 : 0, cond);
		}

		public unsafe static void SetWindowCollapsed(string name, bool collapsed)
		{
			int num = 0;
			byte* ptr;
			if (name != null)
			{
				num = Encoding.UTF8.GetByteCount(name);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(name, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (collapsed ? 1 : 0);
			ImGuiCond imGuiCond = ImGuiCond.None;
			ImGuiNative.igSetWindowCollapsedStr(ptr, b, imGuiCond);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void SetWindowCollapsed(string name, bool collapsed, ImGuiCond cond)
		{
			int num = 0;
			byte* ptr;
			if (name != null)
			{
				num = Encoding.UTF8.GetByteCount(name);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(name, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b = (collapsed ? 1 : 0);
			ImGuiNative.igSetWindowCollapsedStr(ptr, b, cond);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public static void SetWindowFocus()
		{
			ImGuiNative.igSetWindowFocusNil();
		}

		public unsafe static void SetWindowFocus(string name)
		{
			int num = 0;
			byte* ptr;
			if (name != null)
			{
				num = Encoding.UTF8.GetByteCount(name);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(name, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igSetWindowFocusStr(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public static void SetWindowFontScale(float scale)
		{
			ImGuiNative.igSetWindowFontScale(scale);
		}

		public static void SetWindowPos(Vector2 pos)
		{
			ImGuiCond imGuiCond = ImGuiCond.None;
			ImGuiNative.igSetWindowPosVec2(pos, imGuiCond);
		}

		public static void SetWindowPos(Vector2 pos, ImGuiCond cond)
		{
			ImGuiNative.igSetWindowPosVec2(pos, cond);
		}

		public unsafe static void SetWindowPos(string name, Vector2 pos)
		{
			int num = 0;
			byte* ptr;
			if (name != null)
			{
				num = Encoding.UTF8.GetByteCount(name);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(name, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiCond imGuiCond = ImGuiCond.None;
			ImGuiNative.igSetWindowPosStr(ptr, pos, imGuiCond);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void SetWindowPos(string name, Vector2 pos, ImGuiCond cond)
		{
			int num = 0;
			byte* ptr;
			if (name != null)
			{
				num = Encoding.UTF8.GetByteCount(name);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(name, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igSetWindowPosStr(ptr, pos, cond);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public static void SetWindowSize(Vector2 size)
		{
			ImGuiCond imGuiCond = ImGuiCond.None;
			ImGuiNative.igSetWindowSizeVec2(size, imGuiCond);
		}

		public static void SetWindowSize(Vector2 size, ImGuiCond cond)
		{
			ImGuiNative.igSetWindowSizeVec2(size, cond);
		}

		public unsafe static void SetWindowSize(string name, Vector2 size)
		{
			int num = 0;
			byte* ptr;
			if (name != null)
			{
				num = Encoding.UTF8.GetByteCount(name);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(name, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiCond imGuiCond = ImGuiCond.None;
			ImGuiNative.igSetWindowSizeStr(ptr, size, imGuiCond);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void SetWindowSize(string name, Vector2 size, ImGuiCond cond)
		{
			int num = 0;
			byte* ptr;
			if (name != null)
			{
				num = Encoding.UTF8.GetByteCount(name);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(name, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igSetWindowSizeStr(ptr, size, cond);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void ShowAboutWindow()
		{
			byte* ptr = null;
			ImGuiNative.igShowAboutWindow(ptr);
		}

		public unsafe static void ShowAboutWindow(ref bool p_open)
		{
			byte b = (p_open ? 1 : 0);
			ImGuiNative.igShowAboutWindow(&b);
			p_open = b > 0;
		}

		public unsafe static void ShowDemoWindow()
		{
			byte* ptr = null;
			ImGuiNative.igShowDemoWindow(ptr);
		}

		public unsafe static void ShowDemoWindow(ref bool p_open)
		{
			byte b = (p_open ? 1 : 0);
			ImGuiNative.igShowDemoWindow(&b);
			p_open = b > 0;
		}

		public unsafe static void ShowFontSelector(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igShowFontSelector(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void ShowMetricsWindow()
		{
			byte* ptr = null;
			ImGuiNative.igShowMetricsWindow(ptr);
		}

		public unsafe static void ShowMetricsWindow(ref bool p_open)
		{
			byte b = (p_open ? 1 : 0);
			ImGuiNative.igShowMetricsWindow(&b);
			p_open = b > 0;
		}

		public unsafe static void ShowStyleEditor()
		{
			ImGuiStyle* ptr = null;
			ImGuiNative.igShowStyleEditor(ptr);
		}

		public static void ShowStyleEditor(ImGuiStylePtr @ref)
		{
			ImGuiNative.igShowStyleEditor(@ref.NativePtr);
		}

		public unsafe static bool ShowStyleSelector(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igShowStyleSelector(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public static void ShowUserGuide()
		{
			ImGuiNative.igShowUserGuide();
		}

		public unsafe static bool SliderAngle(string label, ref float v_rad)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = -360f;
			float num3 = 360f;
			int byteCount = Encoding.UTF8.GetByteCount("%.0f deg");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.0f deg", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr3 = &v_rad)
			{
				float* ptr4 = ptr3;
				int num4 = (int)ImGuiNative.igSliderAngle(ptr, ptr4, num2, num3, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num4 != 0;
			}
		}

		public unsafe static bool SliderAngle(string label, ref float v_rad, float v_degrees_min)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 360f;
			int byteCount = Encoding.UTF8.GetByteCount("%.0f deg");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.0f deg", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr3 = &v_rad)
			{
				float* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderAngle(ptr, ptr4, v_degrees_min, num2, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderAngle(string label, ref float v_rad, float v_degrees_min, float v_degrees_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.0f deg");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.0f deg", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr3 = &v_rad)
			{
				float* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igSliderAngle(ptr, ptr4, v_degrees_min, v_degrees_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool SliderAngle(string label, ref float v_rad, float v_degrees_min, float v_degrees_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr3 = &v_rad)
			{
				float* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderAngle(ptr, ptr4, v_degrees_min, v_degrees_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderAngle(string label, ref float v_rad, float v_degrees_min, float v_degrees_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (float* ptr3 = &v_rad)
			{
				float* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderAngle(ptr, ptr4, v_degrees_min, v_degrees_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderFloat(string label, ref float v, float v_min, float v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igSliderFloat(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool SliderFloat(string label, ref float v, float v_min, float v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderFloat(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderFloat(string label, ref float v, float v_min, float v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderFloat(ptr, ptr4, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderFloat2(string label, ref Vector2 v, float v_min, float v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector2* ptr3 = &v)
			{
				Vector2* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igSliderFloat2(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool SliderFloat2(string label, ref Vector2 v, float v_min, float v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector2* ptr3 = &v)
			{
				Vector2* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderFloat2(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderFloat2(string label, ref Vector2 v, float v_min, float v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (Vector2* ptr3 = &v)
			{
				Vector2* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderFloat2(ptr, ptr4, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderFloat3(string label, ref Vector3 v, float v_min, float v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector3* ptr3 = &v)
			{
				Vector3* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igSliderFloat3(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool SliderFloat3(string label, ref Vector3 v, float v_min, float v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector3* ptr3 = &v)
			{
				Vector3* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderFloat3(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderFloat3(string label, ref Vector3 v, float v_min, float v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (Vector3* ptr3 = &v)
			{
				Vector3* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderFloat3(ptr, ptr4, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderFloat4(string label, ref Vector4 v, float v_min, float v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector4* ptr3 = &v)
			{
				Vector4* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igSliderFloat4(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool SliderFloat4(string label, ref Vector4 v, float v_min, float v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (Vector4* ptr3 = &v)
			{
				Vector4* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderFloat4(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderFloat4(string label, ref Vector4 v, float v_min, float v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (Vector4* ptr3 = &v)
			{
				Vector4* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderFloat4(ptr, ptr4, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderInt(string label, ref int v, int v_min, int v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igSliderInt(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool SliderInt(string label, ref int v, int v_min, int v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderInt(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderInt(string label, ref int v, int v_min, int v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderInt(ptr, ptr4, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderInt2(string label, ref int v, int v_min, int v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igSliderInt2(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool SliderInt2(string label, ref int v, int v_min, int v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderInt2(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderInt2(string label, ref int v, int v_min, int v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderInt2(ptr, ptr4, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderInt3(string label, ref int v, int v_min, int v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igSliderInt3(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool SliderInt3(string label, ref int v, int v_min, int v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderInt3(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderInt3(string label, ref int v, int v_min, int v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderInt3(ptr, ptr4, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderInt4(string label, ref int v, int v_min, int v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igSliderInt4(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool SliderInt4(string label, ref int v, int v_min, int v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderInt4(ptr, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderInt4(string label, ref int v, int v_min, int v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igSliderInt4(ptr, ptr4, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool SliderScalar(string label, ImGuiDataType data_type, IntPtr p_data, IntPtr p_min, IntPtr p_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = p_max.ToPointer();
			byte* ptr5 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			int num2 = (int)ImGuiNative.igSliderScalar(ptr, data_type, ptr2, ptr3, ptr4, ptr5, imGuiSliderFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool SliderScalar(string label, ImGuiDataType data_type, IntPtr p_data, IntPtr p_min, IntPtr p_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = p_max.ToPointer();
			int num2 = 0;
			byte* ptr5;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr5 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr5 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr5, num2);
				ptr5[utf2] = 0;
			}
			else
			{
				ptr5 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			int num3 = (int)ImGuiNative.igSliderScalar(ptr, data_type, ptr2, ptr3, ptr4, ptr5, imGuiSliderFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr5);
			}
			return num3 != 0;
		}

		public unsafe static bool SliderScalar(string label, ImGuiDataType data_type, IntPtr p_data, IntPtr p_min, IntPtr p_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = p_max.ToPointer();
			int num2 = 0;
			byte* ptr5;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr5 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr5 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr5, num2);
				ptr5[utf2] = 0;
			}
			else
			{
				ptr5 = null;
			}
			int num3 = (int)ImGuiNative.igSliderScalar(ptr, data_type, ptr2, ptr3, ptr4, ptr5, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr5);
			}
			return num3 != 0;
		}

		public unsafe static bool SliderScalarN(string label, ImGuiDataType data_type, IntPtr p_data, int components, IntPtr p_min, IntPtr p_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = p_max.ToPointer();
			byte* ptr5 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			int num2 = (int)ImGuiNative.igSliderScalarN(ptr, data_type, ptr2, components, ptr3, ptr4, ptr5, imGuiSliderFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool SliderScalarN(string label, ImGuiDataType data_type, IntPtr p_data, int components, IntPtr p_min, IntPtr p_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = p_max.ToPointer();
			int num2 = 0;
			byte* ptr5;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr5 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr5 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr5, num2);
				ptr5[utf2] = 0;
			}
			else
			{
				ptr5 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			int num3 = (int)ImGuiNative.igSliderScalarN(ptr, data_type, ptr2, components, ptr3, ptr4, ptr5, imGuiSliderFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr5);
			}
			return num3 != 0;
		}

		public unsafe static bool SliderScalarN(string label, ImGuiDataType data_type, IntPtr p_data, int components, IntPtr p_min, IntPtr p_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = p_max.ToPointer();
			int num2 = 0;
			byte* ptr5;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr5 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr5 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr5, num2);
				ptr5[utf2] = 0;
			}
			else
			{
				ptr5 = null;
			}
			int num3 = (int)ImGuiNative.igSliderScalarN(ptr, data_type, ptr2, components, ptr3, ptr4, ptr5, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr5);
			}
			return num3 != 0;
		}

		public unsafe static bool SmallButton(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igSmallButton(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public static void Spacing()
		{
			ImGuiNative.igSpacing();
		}

		public unsafe static void StyleColorsClassic()
		{
			ImGuiStyle* ptr = null;
			ImGuiNative.igStyleColorsClassic(ptr);
		}

		public static void StyleColorsClassic(ImGuiStylePtr dst)
		{
			ImGuiNative.igStyleColorsClassic(dst.NativePtr);
		}

		public unsafe static void StyleColorsDark()
		{
			ImGuiStyle* ptr = null;
			ImGuiNative.igStyleColorsDark(ptr);
		}

		public static void StyleColorsDark(ImGuiStylePtr dst)
		{
			ImGuiNative.igStyleColorsDark(dst.NativePtr);
		}

		public unsafe static void StyleColorsLight()
		{
			ImGuiStyle* ptr = null;
			ImGuiNative.igStyleColorsLight(ptr);
		}

		public static void StyleColorsLight(ImGuiStylePtr dst)
		{
			ImGuiNative.igStyleColorsLight(dst.NativePtr);
		}

		public unsafe static bool TabItemButton(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiTabItemFlags imGuiTabItemFlags = ImGuiTabItemFlags.None;
			int num2 = (int)ImGuiNative.igTabItemButton(ptr, imGuiTabItemFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool TabItemButton(string label, ImGuiTabItemFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igTabItemButton(ptr, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public static int TableGetColumnCount()
		{
			return ImGuiNative.igTableGetColumnCount();
		}

		public static ImGuiTableColumnFlags TableGetColumnFlags()
		{
			return ImGuiNative.igTableGetColumnFlags(-1);
		}

		public static ImGuiTableColumnFlags TableGetColumnFlags(int column_n)
		{
			return ImGuiNative.igTableGetColumnFlags(column_n);
		}

		public static int TableGetColumnIndex()
		{
			return ImGuiNative.igTableGetColumnIndex();
		}

		public static string TableGetColumnName()
		{
			return Util.StringFromPtr(ImGuiNative.igTableGetColumnNameInt(-1));
		}

		public static string TableGetColumnName(int column_n)
		{
			return Util.StringFromPtr(ImGuiNative.igTableGetColumnNameInt(column_n));
		}

		public static int TableGetRowIndex()
		{
			return ImGuiNative.igTableGetRowIndex();
		}

		public static ImGuiTableSortSpecsPtr TableGetSortSpecs()
		{
			return new ImGuiTableSortSpecsPtr(ImGuiNative.igTableGetSortSpecs());
		}

		public unsafe static void TableHeader(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igTableHeader(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public static void TableHeadersRow()
		{
			ImGuiNative.igTableHeadersRow();
		}

		public static bool TableNextColumn()
		{
			return ImGuiNative.igTableNextColumn() > 0;
		}

		public static void TableNextRow()
		{
			ImGuiTableRowFlags imGuiTableRowFlags = ImGuiTableRowFlags.None;
			float num = 0f;
			ImGuiNative.igTableNextRow(imGuiTableRowFlags, num);
		}

		public static void TableNextRow(ImGuiTableRowFlags row_flags)
		{
			float num = 0f;
			ImGuiNative.igTableNextRow(row_flags, num);
		}

		public static void TableNextRow(ImGuiTableRowFlags row_flags, float min_row_height)
		{
			ImGuiNative.igTableNextRow(row_flags, min_row_height);
		}

		public static void TableSetBgColor(ImGuiTableBgTarget target, uint color)
		{
			int num = -1;
			ImGuiNative.igTableSetBgColor(target, color, num);
		}

		public static void TableSetBgColor(ImGuiTableBgTarget target, uint color, int column_n)
		{
			ImGuiNative.igTableSetBgColor(target, color, column_n);
		}

		public static bool TableSetColumnIndex(int column_n)
		{
			return ImGuiNative.igTableSetColumnIndex(column_n) > 0;
		}

		public unsafe static void TableSetupColumn(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiTableColumnFlags imGuiTableColumnFlags = ImGuiTableColumnFlags.None;
			float num2 = 0f;
			uint num3 = 0U;
			ImGuiNative.igTableSetupColumn(ptr, imGuiTableColumnFlags, num2, num3);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void TableSetupColumn(string label, ImGuiTableColumnFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 0f;
			uint num3 = 0U;
			ImGuiNative.igTableSetupColumn(ptr, flags, num2, num3);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void TableSetupColumn(string label, ImGuiTableColumnFlags flags, float init_width_or_weight)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			uint num2 = 0U;
			ImGuiNative.igTableSetupColumn(ptr, flags, init_width_or_weight, num2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void TableSetupColumn(string label, ImGuiTableColumnFlags flags, float init_width_or_weight, uint user_id)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igTableSetupColumn(ptr, flags, init_width_or_weight, user_id);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public static void TableSetupScrollFreeze(int cols, int rows)
		{
			ImGuiNative.igTableSetupScrollFreeze(cols, rows);
		}

		public unsafe static void Text(string fmt)
		{
			int num = 0;
			byte* ptr;
			if (fmt != null)
			{
				num = Encoding.UTF8.GetByteCount(fmt);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(fmt, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igText(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void TextColored(Vector4 col, string fmt)
		{
			int num = 0;
			byte* ptr;
			if (fmt != null)
			{
				num = Encoding.UTF8.GetByteCount(fmt);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(fmt, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igTextColored(col, ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void TextDisabled(string fmt)
		{
			int num = 0;
			byte* ptr;
			if (fmt != null)
			{
				num = Encoding.UTF8.GetByteCount(fmt);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(fmt, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igTextDisabled(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void TextUnformatted(string text)
		{
			int num = 0;
			byte* ptr;
			if (text != null)
			{
				num = Encoding.UTF8.GetByteCount(text);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(text, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte* ptr2 = null;
			ImGuiNative.igTextUnformatted(ptr, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void TextWrapped(string fmt)
		{
			int num = 0;
			byte* ptr;
			if (fmt != null)
			{
				num = Encoding.UTF8.GetByteCount(fmt);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(fmt, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igTextWrapped(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static bool TreeNode(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igTreeNodeStr(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool TreeNode(string str_id, string fmt)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (fmt != null)
			{
				num2 = Encoding.UTF8.GetByteCount(fmt);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(fmt, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			int num3 = (int)ImGuiNative.igTreeNodeStrStr(ptr, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr2);
			}
			return num3 != 0;
		}

		public unsafe static bool TreeNode(IntPtr ptr_id, string fmt)
		{
			void* ptr = ptr_id.ToPointer();
			int num = 0;
			byte* ptr2;
			if (fmt != null)
			{
				num = Encoding.UTF8.GetByteCount(fmt);
				if (num > 2048)
				{
					ptr2 = Util.Allocate(num + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(fmt, ptr2, num);
				ptr2[utf] = 0;
			}
			else
			{
				ptr2 = null;
			}
			int num2 = (int)ImGuiNative.igTreeNodePtr(ptr, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr2);
			}
			return num2 != 0;
		}

		public unsafe static bool TreeNodeEx(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiTreeNodeFlags imGuiTreeNodeFlags = ImGuiTreeNodeFlags.None;
			int num2 = (int)ImGuiNative.igTreeNodeExStr(ptr, imGuiTreeNodeFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool TreeNodeEx(string label, ImGuiTreeNodeFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.igTreeNodeExStr(ptr, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool TreeNodeEx(string str_id, ImGuiTreeNodeFlags flags, string fmt)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (fmt != null)
			{
				num2 = Encoding.UTF8.GetByteCount(fmt);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(fmt, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			int num3 = (int)ImGuiNative.igTreeNodeExStrStr(ptr, flags, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr2);
			}
			return num3 != 0;
		}

		public unsafe static bool TreeNodeEx(IntPtr ptr_id, ImGuiTreeNodeFlags flags, string fmt)
		{
			void* ptr = ptr_id.ToPointer();
			int num = 0;
			byte* ptr2;
			if (fmt != null)
			{
				num = Encoding.UTF8.GetByteCount(fmt);
				if (num > 2048)
				{
					ptr2 = Util.Allocate(num + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(fmt, ptr2, num);
				ptr2[utf] = 0;
			}
			else
			{
				ptr2 = null;
			}
			int num2 = (int)ImGuiNative.igTreeNodeExPtr(ptr, flags, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr2);
			}
			return num2 != 0;
		}

		public static void TreePop()
		{
			ImGuiNative.igTreePop();
		}

		public unsafe static void TreePush(string str_id)
		{
			int num = 0;
			byte* ptr;
			if (str_id != null)
			{
				num = Encoding.UTF8.GetByteCount(str_id);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str_id, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igTreePushStr(ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void TreePush()
		{
			void* ptr = null;
			ImGuiNative.igTreePushPtr(ptr);
		}

		public static void TreePush(IntPtr ptr_id)
		{
			ImGuiNative.igTreePushPtr(ptr_id.ToPointer());
		}

		public static void Unindent()
		{
			ImGuiNative.igUnindent(0f);
		}

		public static void Unindent(float indent_w)
		{
			ImGuiNative.igUnindent(indent_w);
		}

		public static void UpdatePlatformWindows()
		{
			ImGuiNative.igUpdatePlatformWindows();
		}

		public unsafe static void Value(string prefix, bool b)
		{
			int num = 0;
			byte* ptr;
			if (prefix != null)
			{
				num = Encoding.UTF8.GetByteCount(prefix);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(prefix, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte b2 = (b ? 1 : 0);
			ImGuiNative.igValueBool(ptr, b2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void Value(string prefix, int v)
		{
			int num = 0;
			byte* ptr;
			if (prefix != null)
			{
				num = Encoding.UTF8.GetByteCount(prefix);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(prefix, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igValueInt(ptr, v);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void Value(string prefix, uint v)
		{
			int num = 0;
			byte* ptr;
			if (prefix != null)
			{
				num = Encoding.UTF8.GetByteCount(prefix);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(prefix, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.igValueUint(ptr, v);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void Value(string prefix, float v)
		{
			int num = 0;
			byte* ptr;
			if (prefix != null)
			{
				num = Encoding.UTF8.GetByteCount(prefix);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(prefix, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte* ptr2 = null;
			ImGuiNative.igValueFloat(ptr, v, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe static void Value(string prefix, float v, string float_format)
		{
			int num = 0;
			byte* ptr;
			if (prefix != null)
			{
				num = Encoding.UTF8.GetByteCount(prefix);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(prefix, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (float_format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(float_format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(float_format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiNative.igValueFloat(ptr, v, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr2);
			}
		}

		public unsafe static bool VSliderFloat(string label, Vector2 size, ref float v, float v_min, float v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%.3f");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%.3f", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igVSliderFloat(ptr, size, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool VSliderFloat(string label, Vector2 size, ref float v, float v_min, float v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igVSliderFloat(ptr, size, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool VSliderFloat(string label, Vector2 size, ref float v, float v_min, float v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (float* ptr3 = &v)
			{
				float* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igVSliderFloat(ptr, size, ptr4, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool VSliderInt(string label, Vector2 size, ref int v, int v_min, int v_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int byteCount = Encoding.UTF8.GetByteCount("%d");
			byte* ptr2;
			if (byteCount > 2048)
			{
				ptr2 = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf2 = Util.GetUtf8("%d", ptr2, byteCount);
			ptr2[utf2] = 0;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num2 = (int)ImGuiNative.igVSliderInt(ptr, size, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (byteCount > 2048)
				{
					Util.Free(ptr2);
				}
				return num2 != 0;
			}
		}

		public unsafe static bool VSliderInt(string label, Vector2 size, ref int v, int v_min, int v_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igVSliderInt(ptr, size, ptr4, v_min, v_max, ptr2, imGuiSliderFlags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool VSliderInt(string label, Vector2 size, ref int v, int v_min, int v_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = 0;
			byte* ptr2;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr2 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr2 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr2, num2);
				ptr2[utf2] = 0;
			}
			else
			{
				ptr2 = null;
			}
			fixed (int* ptr3 = &v)
			{
				int* ptr4 = ptr3;
				int num3 = (int)ImGuiNative.igVSliderInt(ptr, size, ptr4, v_min, v_max, ptr2, flags);
				if (num > 2048)
				{
					Util.Free(ptr);
				}
				if (num2 > 2048)
				{
					Util.Free(ptr2);
				}
				return num3 != 0;
			}
		}

		public unsafe static bool VSliderScalar(string label, Vector2 size, ImGuiDataType data_type, IntPtr p_data, IntPtr p_min, IntPtr p_max)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = p_max.ToPointer();
			byte* ptr5 = null;
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			int num2 = (int)ImGuiNative.igVSliderScalar(ptr, size, data_type, ptr2, ptr3, ptr4, ptr5, imGuiSliderFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe static bool VSliderScalar(string label, Vector2 size, ImGuiDataType data_type, IntPtr p_data, IntPtr p_min, IntPtr p_max, string format)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = p_max.ToPointer();
			int num2 = 0;
			byte* ptr5;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr5 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr5 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr5, num2);
				ptr5[utf2] = 0;
			}
			else
			{
				ptr5 = null;
			}
			ImGuiSliderFlags imGuiSliderFlags = ImGuiSliderFlags.None;
			int num3 = (int)ImGuiNative.igVSliderScalar(ptr, size, data_type, ptr2, ptr3, ptr4, ptr5, imGuiSliderFlags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr5);
			}
			return num3 != 0;
		}

		public unsafe static bool VSliderScalar(string label, Vector2 size, ImGuiDataType data_type, IntPtr p_data, IntPtr p_min, IntPtr p_max, string format, ImGuiSliderFlags flags)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			void* ptr2 = p_data.ToPointer();
			void* ptr3 = p_min.ToPointer();
			void* ptr4 = p_max.ToPointer();
			int num2 = 0;
			byte* ptr5;
			if (format != null)
			{
				num2 = Encoding.UTF8.GetByteCount(format);
				if (num2 > 2048)
				{
					ptr5 = Util.Allocate(num2 + 1);
				}
				else
				{
					ptr5 = stackalloc byte[(UIntPtr)(num2 + 1)];
				}
				int utf2 = Util.GetUtf8(format, ptr5, num2);
				ptr5[utf2] = 0;
			}
			else
			{
				ptr5 = null;
			}
			int num3 = (int)ImGuiNative.igVSliderScalar(ptr, size, data_type, ptr2, ptr3, ptr4, ptr5, flags);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			if (num2 > 2048)
			{
				Util.Free(ptr5);
			}
			return num3 != 0;
		}

		public static bool InputText(string label, byte[] buf, uint buf_size)
		{
			return ImGui.InputText(label, buf, buf_size, ImGuiInputTextFlags.None, null, IntPtr.Zero);
		}

		public static bool InputText(string label, byte[] buf, uint buf_size, ImGuiInputTextFlags flags)
		{
			return ImGui.InputText(label, buf, buf_size, flags, null, IntPtr.Zero);
		}

		public static bool InputText(string label, byte[] buf, uint buf_size, ImGuiInputTextFlags flags, ImGuiInputTextCallback callback)
		{
			return ImGui.InputText(label, buf, buf_size, flags, callback, IntPtr.Zero);
		}

		public unsafe static bool InputText(string label, byte[] buf, uint buf_size, ImGuiInputTextFlags flags, ImGuiInputTextCallback callback, IntPtr user_data)
		{
			int byteCount = Encoding.UTF8.GetByteCount(label);
			byte* ptr;
			if (byteCount > 2048)
			{
				ptr = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			Util.GetUtf8(label, ptr, byteCount);
			bool flag;
			fixed (byte[] array = buf)
			{
				byte* ptr2;
				if (buf == null || array.Length == 0)
				{
					ptr2 = null;
				}
				else
				{
					ptr2 = &array[0];
				}
				flag = ImGuiNative.igInputText(ptr, ptr2, buf_size, flags, callback, user_data.ToPointer()) > 0;
			}
			if (byteCount > 2048)
			{
				Util.Free(ptr);
			}
			return flag;
		}

		public static bool InputText(string label, ref string input, uint maxLength)
		{
			return ImGui.InputText(label, ref input, maxLength, ImGuiInputTextFlags.None, null, IntPtr.Zero);
		}

		public static bool InputText(string label, ref string input, uint maxLength, ImGuiInputTextFlags flags)
		{
			return ImGui.InputText(label, ref input, maxLength, flags, null, IntPtr.Zero);
		}

		public static bool InputText(string label, ref string input, uint maxLength, ImGuiInputTextFlags flags, ImGuiInputTextCallback callback)
		{
			return ImGui.InputText(label, ref input, maxLength, flags, callback, IntPtr.Zero);
		}

		public unsafe static bool InputText(string label, ref string input, uint maxLength, ImGuiInputTextFlags flags, ImGuiInputTextCallback callback, IntPtr user_data)
		{
			int byteCount = Encoding.UTF8.GetByteCount(label);
			byte* ptr;
			if (byteCount > 2048)
			{
				ptr = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			Util.GetUtf8(label, ptr, byteCount);
			int byteCount2 = Encoding.UTF8.GetByteCount(input);
			int num = Math.Max((int)(maxLength + 1U), byteCount2 + 1);
			byte* ptr2;
			byte* ptr3;
			if (num > 2048)
			{
				ptr2 = Util.Allocate(num);
				ptr3 = Util.Allocate(num);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)num];
				ptr3 = stackalloc byte[(UIntPtr)num];
			}
			Util.GetUtf8(input, ptr2, num);
			uint num2 = (uint)(num - byteCount2);
			Unsafe.InitBlockUnaligned((void*)(ptr2 + byteCount2), 0, num2);
			Unsafe.CopyBlock((void*)ptr3, (void*)ptr2, (uint)num);
			byte b = ImGuiNative.igInputText(ptr, ptr2, (uint)num, flags, callback, user_data.ToPointer());
			if (!Util.AreStringsEqual(ptr3, num, ptr2))
			{
				input = Util.StringFromPtr(ptr2);
			}
			if (byteCount > 2048)
			{
				Util.Free(ptr);
			}
			if (num > 2048)
			{
				Util.Free(ptr2);
				Util.Free(ptr3);
			}
			return b > 0;
		}

		public static bool InputTextMultiline(string label, ref string input, uint maxLength, Vector2 size)
		{
			return ImGui.InputTextMultiline(label, ref input, maxLength, size, ImGuiInputTextFlags.None, null, IntPtr.Zero);
		}

		public static bool InputTextMultiline(string label, ref string input, uint maxLength, Vector2 size, ImGuiInputTextFlags flags)
		{
			return ImGui.InputTextMultiline(label, ref input, maxLength, size, flags, null, IntPtr.Zero);
		}

		public static bool InputTextMultiline(string label, ref string input, uint maxLength, Vector2 size, ImGuiInputTextFlags flags, ImGuiInputTextCallback callback)
		{
			return ImGui.InputTextMultiline(label, ref input, maxLength, size, flags, callback, IntPtr.Zero);
		}

		public unsafe static bool InputTextMultiline(string label, ref string input, uint maxLength, Vector2 size, ImGuiInputTextFlags flags, ImGuiInputTextCallback callback, IntPtr user_data)
		{
			int byteCount = Encoding.UTF8.GetByteCount(label);
			byte* ptr;
			if (byteCount > 2048)
			{
				ptr = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			Util.GetUtf8(label, ptr, byteCount);
			int byteCount2 = Encoding.UTF8.GetByteCount(input);
			int num = Math.Max((int)(maxLength + 1U), byteCount2 + 1);
			byte* ptr2;
			byte* ptr3;
			if (num > 2048)
			{
				ptr2 = Util.Allocate(num);
				ptr3 = Util.Allocate(num);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)num];
				ptr3 = stackalloc byte[(UIntPtr)num];
			}
			Util.GetUtf8(input, ptr2, num);
			uint num2 = (uint)(num - byteCount2);
			Unsafe.InitBlockUnaligned((void*)(ptr2 + byteCount2), 0, num2);
			Unsafe.CopyBlock((void*)ptr3, (void*)ptr2, (uint)num);
			byte b = ImGuiNative.igInputTextMultiline(ptr, ptr2, (uint)num, size, flags, callback, user_data.ToPointer());
			if (!Util.AreStringsEqual(ptr3, num, ptr2))
			{
				input = Util.StringFromPtr(ptr2);
			}
			if (byteCount > 2048)
			{
				Util.Free(ptr);
			}
			if (num > 2048)
			{
				Util.Free(ptr2);
				Util.Free(ptr3);
			}
			return b > 0;
		}

		public static bool InputTextWithHint(string label, string hint, ref string input, uint maxLength)
		{
			return ImGui.InputTextWithHint(label, hint, ref input, maxLength, ImGuiInputTextFlags.None, null, IntPtr.Zero);
		}

		public static bool InputTextWithHint(string label, string hint, ref string input, uint maxLength, ImGuiInputTextFlags flags)
		{
			return ImGui.InputTextWithHint(label, hint, ref input, maxLength, flags, null, IntPtr.Zero);
		}

		public static bool InputTextWithHint(string label, string hint, ref string input, uint maxLength, ImGuiInputTextFlags flags, ImGuiInputTextCallback callback)
		{
			return ImGui.InputTextWithHint(label, hint, ref input, maxLength, flags, callback, IntPtr.Zero);
		}

		public unsafe static bool InputTextWithHint(string label, string hint, ref string input, uint maxLength, ImGuiInputTextFlags flags, ImGuiInputTextCallback callback, IntPtr user_data)
		{
			int byteCount = Encoding.UTF8.GetByteCount(label);
			byte* ptr;
			if (byteCount > 2048)
			{
				ptr = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			Util.GetUtf8(label, ptr, byteCount);
			int byteCount2 = Encoding.UTF8.GetByteCount(hint);
			byte* ptr2;
			if (byteCount2 > 2048)
			{
				ptr2 = Util.Allocate(byteCount2 + 1);
			}
			else
			{
				ptr2 = stackalloc byte[(UIntPtr)(byteCount2 + 1)];
			}
			Util.GetUtf8(hint, ptr2, byteCount2);
			int byteCount3 = Encoding.UTF8.GetByteCount(input);
			int num = Math.Max((int)(maxLength + 1U), byteCount3 + 1);
			byte* ptr3;
			byte* ptr4;
			if (num > 2048)
			{
				ptr3 = Util.Allocate(num);
				ptr4 = Util.Allocate(num);
			}
			else
			{
				ptr3 = stackalloc byte[(UIntPtr)num];
				ptr4 = stackalloc byte[(UIntPtr)num];
			}
			Util.GetUtf8(input, ptr3, num);
			uint num2 = (uint)(num - byteCount3);
			Unsafe.InitBlockUnaligned((void*)(ptr3 + byteCount3), 0, num2);
			Unsafe.CopyBlock((void*)ptr4, (void*)ptr3, (uint)num);
			byte b = ImGuiNative.igInputTextWithHint(ptr, ptr2, ptr3, (uint)num, flags, callback, user_data.ToPointer());
			if (!Util.AreStringsEqual(ptr4, num, ptr3))
			{
				input = Util.StringFromPtr(ptr3);
			}
			if (byteCount > 2048)
			{
				Util.Free(ptr);
			}
			if (byteCount2 > 2048)
			{
				Util.Free(ptr2);
			}
			if (num > 2048)
			{
				Util.Free(ptr3);
				Util.Free(ptr4);
			}
			return b > 0;
		}

		public static Vector2 CalcTextSize(string text)
		{
			return ImGui.CalcTextSizeImpl(text, 0, null, false, -1f);
		}

		public static Vector2 CalcTextSize(string text, int start)
		{
			return ImGui.CalcTextSizeImpl(text, start, null, false, -1f);
		}

		public static Vector2 CalcTextSize(string text, float wrapWidth)
		{
			return ImGui.CalcTextSizeImpl(text, 0, null, false, wrapWidth);
		}

		public static Vector2 CalcTextSize(string text, bool hideTextAfterDoubleHash)
		{
			return ImGui.CalcTextSizeImpl(text, 0, null, hideTextAfterDoubleHash, -1f);
		}

		public static Vector2 CalcTextSize(string text, int start, int length)
		{
			return ImGui.CalcTextSizeImpl(text, start, new int?(length), false, -1f);
		}

		public static Vector2 CalcTextSize(string text, int start, bool hideTextAfterDoubleHash)
		{
			return ImGui.CalcTextSizeImpl(text, start, null, hideTextAfterDoubleHash, -1f);
		}

		public static Vector2 CalcTextSize(string text, int start, float wrapWidth)
		{
			return ImGui.CalcTextSizeImpl(text, start, null, false, wrapWidth);
		}

		public static Vector2 CalcTextSize(string text, bool hideTextAfterDoubleHash, float wrapWidth)
		{
			return ImGui.CalcTextSizeImpl(text, 0, null, hideTextAfterDoubleHash, wrapWidth);
		}

		public static Vector2 CalcTextSize(string text, int start, int length, bool hideTextAfterDoubleHash)
		{
			return ImGui.CalcTextSizeImpl(text, start, new int?(length), hideTextAfterDoubleHash, -1f);
		}

		public static Vector2 CalcTextSize(string text, int start, int length, float wrapWidth)
		{
			return ImGui.CalcTextSizeImpl(text, start, new int?(length), false, wrapWidth);
		}

		public static Vector2 CalcTextSize(string text, int start, int length, bool hideTextAfterDoubleHash, float wrapWidth)
		{
			return ImGui.CalcTextSizeImpl(text, start, new int?(length), hideTextAfterDoubleHash, wrapWidth);
		}

		private unsafe static Vector2 CalcTextSizeImpl(string text, int start = 0, int? length = null, bool hideTextAfterDoubleHash = false, float wrapWidth = -1f)
		{
			byte* ptr = null;
			byte* ptr2 = null;
			int num = 0;
			if (text != null)
			{
				int num2 = ((length != null) ? length.Value : text.Length);
				num = Util.CalcSizeInUtf8(text, start, num2);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(text, start, num2, ptr, num);
				ptr[utf] = 0;
				ptr2 = ptr + utf;
			}
			Vector2 vector;
			ImGuiNative.igCalcTextSize(&vector, ptr, ptr2, (*(&hideTextAfterDoubleHash)) ? 1 : 0, wrapWidth);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return vector;
		}

		public static bool InputText(string label, IntPtr buf, uint buf_size)
		{
			return ImGui.InputText(label, buf, buf_size, ImGuiInputTextFlags.None, null, IntPtr.Zero);
		}

		public static bool InputText(string label, IntPtr buf, uint buf_size, ImGuiInputTextFlags flags)
		{
			return ImGui.InputText(label, buf, buf_size, flags, null, IntPtr.Zero);
		}

		public static bool InputText(string label, IntPtr buf, uint buf_size, ImGuiInputTextFlags flags, ImGuiInputTextCallback callback)
		{
			return ImGui.InputText(label, buf, buf_size, flags, callback, IntPtr.Zero);
		}

		public unsafe static bool InputText(string label, IntPtr buf, uint buf_size, ImGuiInputTextFlags flags, ImGuiInputTextCallback callback, IntPtr user_data)
		{
			int byteCount = Encoding.UTF8.GetByteCount(label);
			byte* ptr;
			if (byteCount > 2048)
			{
				ptr = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			Util.GetUtf8(label, ptr, byteCount);
			bool flag = ImGuiNative.igInputText(ptr, (byte*)buf.ToPointer(), buf_size, flags, callback, user_data.ToPointer()) > 0;
			if (byteCount > 2048)
			{
				Util.Free(ptr);
			}
			return flag;
		}

		public unsafe static bool Begin(string name, ImGuiWindowFlags flags)
		{
			int byteCount = Encoding.UTF8.GetByteCount(name);
			byte* ptr;
			if (byteCount > 2048)
			{
				ptr = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			Util.GetUtf8(name, ptr, byteCount);
			byte* ptr2 = null;
			int num = (int)ImGuiNative.igBegin(ptr, ptr2, flags);
			if (byteCount > 2048)
			{
				Util.Free(ptr);
			}
			return num != 0;
		}

		public static bool MenuItem(string label, bool enabled)
		{
			return ImGui.MenuItem(label, string.Empty, false, enabled);
		}
	}
}
