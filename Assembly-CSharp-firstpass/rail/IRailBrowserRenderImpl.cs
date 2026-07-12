using System;

namespace rail
{
	public class IRailBrowserRenderImpl : RailObject, IRailBrowserRender, IRailComponent
	{
		internal IRailBrowserRenderImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailBrowserRenderImpl()
		{
		}

		public virtual bool GetCurrentUrl(out string url)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			bool flag;
			try
			{
				flag = RAIL_API_PINVOKE.IRailBrowserRender_GetCurrentUrl(this.swigCPtr_, intPtr);
			}
			finally
			{
				url = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return flag;
		}

		public virtual bool ReloadWithUrl(string new_url)
		{
			return RAIL_API_PINVOKE.IRailBrowserRender_ReloadWithUrl__SWIG_0(this.swigCPtr_, new_url);
		}

		public virtual bool ReloadWithUrl()
		{
			return RAIL_API_PINVOKE.IRailBrowserRender_ReloadWithUrl__SWIG_1(this.swigCPtr_);
		}

		public virtual void StopLoad()
		{
			RAIL_API_PINVOKE.IRailBrowserRender_StopLoad(this.swigCPtr_);
		}

		public virtual bool AddJavascriptEventListener(string event_name)
		{
			return RAIL_API_PINVOKE.IRailBrowserRender_AddJavascriptEventListener(this.swigCPtr_, event_name);
		}

		public virtual bool RemoveAllJavascriptEventListener()
		{
			return RAIL_API_PINVOKE.IRailBrowserRender_RemoveAllJavascriptEventListener(this.swigCPtr_);
		}

		public virtual void AllowNavigateNewPage(bool allow)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_AllowNavigateNewPage(this.swigCPtr_, allow);
		}

		public virtual void Close()
		{
			RAIL_API_PINVOKE.IRailBrowserRender_Close(this.swigCPtr_);
		}

		public virtual void UpdateCustomDrawWindowPos(int content_offset_x, int content_offset_y, uint content_window_width, uint content_window_height)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_UpdateCustomDrawWindowPos(this.swigCPtr_, content_offset_x, content_offset_y, content_window_width, content_window_height);
		}

		public virtual void SetBrowserActive(bool active)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_SetBrowserActive(this.swigCPtr_, active);
		}

		public virtual void GoBack()
		{
			RAIL_API_PINVOKE.IRailBrowserRender_GoBack(this.swigCPtr_);
		}

		public virtual void GoForward()
		{
			RAIL_API_PINVOKE.IRailBrowserRender_GoForward(this.swigCPtr_);
		}

		public virtual bool ExecuteJavascript(string event_name, string event_value)
		{
			return RAIL_API_PINVOKE.IRailBrowserRender_ExecuteJavascript(this.swigCPtr_, event_name, event_value);
		}

		public virtual void DispatchWindowsMessage(uint window_msg, uint w_param, uint l_param)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_DispatchWindowsMessage(this.swigCPtr_, window_msg, w_param, l_param);
		}

		public virtual void DispatchMouseMessage(EnumRailMouseActionType button_action, uint user_define_mouse_key, uint x_pos, uint y_pos)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_DispatchMouseMessage(this.swigCPtr_, (int)button_action, user_define_mouse_key, x_pos, y_pos);
		}

		public virtual void MouseWheel(int delta, uint user_define_mouse_key, uint x_pos, uint y_pos)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_MouseWheel(this.swigCPtr_, delta, user_define_mouse_key, x_pos, y_pos);
		}

		public virtual void SetFocus(bool has_focus)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_SetFocus(this.swigCPtr_, has_focus);
		}

		public virtual void KeyDown(uint key_code)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_KeyDown(this.swigCPtr_, key_code);
		}

		public virtual void KeyUp(uint key_code)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_KeyUp(this.swigCPtr_, key_code);
		}

		public virtual void KeyChar(uint key_code, bool is_uinchar)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_KeyChar(this.swigCPtr_, key_code, is_uinchar);
		}

		public virtual ulong GetComponentVersion()
		{
			return RAIL_API_PINVOKE.IRailComponent_GetComponentVersion(this.swigCPtr_);
		}

		public virtual void Release()
		{
			RAIL_API_PINVOKE.IRailComponent_Release(this.swigCPtr_);
		}
	}
}
