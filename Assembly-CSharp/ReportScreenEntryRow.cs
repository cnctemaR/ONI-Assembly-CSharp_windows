using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ReportScreenEntryRow : KMonoBehaviour
{
	private List<ReportManager.ReportEntry.Note> Sort(List<ReportManager.ReportEntry.Note> notes, ReportManager.ReportEntry.Order order)
	{
		if (order == ReportManager.ReportEntry.Order.Ascending)
		{
			notes.Sort((ReportManager.ReportEntry.Note x, ReportManager.ReportEntry.Note y) => x.value.CompareTo(y.value));
		}
		else if (order == ReportManager.ReportEntry.Order.Descending)
		{
			notes.Sort((ReportManager.ReportEntry.Note x, ReportManager.ReportEntry.Note y) => y.value.CompareTo(x.value));
		}
		return notes;
	}

	public static void DestroyStatics()
	{
		ReportScreenEntryRow.notes = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.added.GetComponent<ToolTip>().OnToolTip = new Func<string>(this.OnPositiveNoteTooltip);
		this.removed.GetComponent<ToolTip>().OnToolTip = new Func<string>(this.OnNegativeNoteTooltip);
		this.net.GetComponent<ToolTip>().OnToolTip = new Func<string>(this.OnNetNoteTooltip);
		this.name.GetComponent<ToolTip>().OnToolTip = new Func<string>(this.OnNetNoteTooltip);
	}

	private string OnNoteTooltip(float total_accumulation, string tooltip_text, ReportManager.ReportEntry.Order order, ReportManager.FormattingFn format_fn, Func<ReportManager.ReportEntry.Note, bool> is_note_applicable_cb)
	{
		ReportScreenEntryRow.notes.Clear();
		this.entry.IterateNotes(delegate(ReportManager.ReportEntry.Note note)
		{
			if (is_note_applicable_cb(note))
			{
				ReportScreenEntryRow.notes.Add(note);
			}
		});
		string text = string.Empty;
		foreach (ReportManager.ReportEntry.Note note2 in this.Sort(ReportScreenEntryRow.notes, this.reportGroup.posNoteOrder))
		{
			text = string.Format(UI.ENDOFDAYREPORT.NOTES.NOTE_ENTRY_LINE_ITEM, text, note2.note, format_fn(note2.value));
		}
		return string.Format(tooltip_text + "\n" + text, format_fn(total_accumulation));
	}

	private string OnNegativeNoteTooltip()
	{
		return this.OnNoteTooltip(this.entry.Negative, this.reportGroup.negativeTooltip, this.reportGroup.negNoteOrder, this.reportGroup.formatfn, (ReportManager.ReportEntry.Note note) => note.value < 0f);
	}

	private string OnPositiveNoteTooltip()
	{
		return this.OnNoteTooltip(this.entry.Positive, this.reportGroup.positiveTooltip, this.reportGroup.posNoteOrder, this.reportGroup.formatfn, (ReportManager.ReportEntry.Note note) => note.value > 0f);
	}

	private string OnNetNoteTooltip()
	{
		if (this.entry.Net > 0f)
		{
			return this.OnPositiveNoteTooltip();
		}
		return this.OnNegativeNoteTooltip();
	}

	public void SetLine(ReportManager.ReportEntry entry, ReportManager.ReportGroup reportGroup)
	{
		this.entry = entry;
		this.reportGroup = reportGroup;
		LayoutElement component = this.name.GetComponent<LayoutElement>();
		if (entry.context == null)
		{
			LayoutElement layoutElement = component;
			float num = this.nameWidth;
			component.preferredWidth = num;
			layoutElement.minWidth = num;
			if (entry.HasContextEntries())
			{
				this.toggle.gameObject.SetActive(true);
				this.spacer.minWidth = this.groupSpacerWidth;
			}
			else
			{
				this.toggle.gameObject.SetActive(false);
				this.spacer.minWidth = this.groupSpacerWidth + this.toggle.GetComponent<LayoutElement>().minWidth;
			}
			this.name.text = reportGroup.stringKey;
		}
		else
		{
			this.toggle.gameObject.SetActive(false);
			this.spacer.minWidth = this.contextSpacerWidth;
			this.name.text = entry.context;
			LayoutElement layoutElement2 = component;
			float num = this.nameWidth - this.indentWidth;
			component.preferredWidth = num;
			layoutElement2.minWidth = num;
			if (base.transform.GetSiblingIndex() % 2 != 0)
			{
				this.bgImage.color = this.oddRowColor;
			}
		}
		if (this.addedValue != entry.Positive)
		{
			this.added.text = reportGroup.formatfn(entry.Positive);
			this.addedValue = entry.Positive;
		}
		if (this.removedValue != entry.Negative)
		{
			this.removed.text = reportGroup.formatfn(entry.Negative);
			this.removedValue = entry.Negative;
		}
		if (this.netValue != entry.Net)
		{
			this.net.text = ((reportGroup.formatfn != null) ? reportGroup.formatfn(entry.Net) : entry.Net.ToString());
			this.netValue = entry.Net;
		}
	}

	[SerializeField]
	public new LocText name;

	[SerializeField]
	public LocText added;

	[SerializeField]
	public LocText removed;

	[SerializeField]
	public LocText net;

	private float addedValue = float.NegativeInfinity;

	private float removedValue = float.NegativeInfinity;

	private float netValue = float.NegativeInfinity;

	[SerializeField]
	public MultiToggle toggle;

	[SerializeField]
	private LayoutElement spacer;

	[SerializeField]
	private Image bgImage;

	public float groupSpacerWidth;

	public float contextSpacerWidth;

	private float nameWidth = 164f;

	private float indentWidth = 6f;

	[SerializeField]
	private Color oddRowColor;

	private static List<ReportManager.ReportEntry.Note> notes = new List<ReportManager.ReportEntry.Note>();

	private ReportManager.ReportEntry entry;

	private ReportManager.ReportGroup reportGroup;
}
