using System;
using System.Diagnostics;

namespace Klei.AI
{
	[DebuggerDisplay("{Id}")]
	public class Effect : Modifier
	{
		public Effect(string id, string name, string description, float duration, bool show_in_ui, bool trigger_floating_text, bool is_bad)
			: base(id, name, description)
		{
			this.duration = duration;
			this.showInUI = show_in_ui;
			this.triggerFloatingText = trigger_floating_text;
			this.isBad = is_bad;
		}

		public event Action<Effects, Effect, bool> OnAddRemove;

		public override void AddTo(Attributes attributes)
		{
			base.AddTo(attributes);
			if (this.OnAddRemove != null)
			{
				this.OnAddRemove(attributes.gameObject.GetComponent<Effects>(), this, true);
			}
		}

		public override void RemoveFrom(Attributes attributes)
		{
			base.RemoveFrom(attributes);
			if (this.OnAddRemove != null)
			{
				this.OnAddRemove(attributes.gameObject.GetComponent<Effects>(), this, false);
			}
		}

		public float duration;

		public bool showInUI;

		public bool triggerFloatingText;

		public bool isBad;
	}
}
