using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TheOtherRoles.Modules;

public class SimpleTable
{
    private class ColumnConfig
    {
        public int? FixedWidth { get; }
        public int MinWidth { get; }
        public Alignment Alignment { get; }
        public int ActualWidth { get; set; }

        public ColumnConfig(int? fixedWidth, int minWidth, Alignment alignment)
        {
            FixedWidth = fixedWidth;
            MinWidth = minWidth;
            Alignment = alignment;
            ActualWidth = fixedWidth ?? minWidth;
        }
    }

    private readonly List<ColumnConfig> _columns = new();
    private readonly List<string[]> _rows = new();
    private bool _requiresWidthUpdate = true;
    private static readonly Regex _tagRegex = new(@"<(\/?(color|size|b|i|material|quad)[^>]*)>", RegexOptions.IgnoreCase);

    /// <summary>
    /// 清空所有行并保留列配置
    /// </summary>
    public SimpleTable ClearRows()
    {
        _rows.Clear();
        _requiresWidthUpdate = true;
        return this;
    }

    /// <summary>
    /// 删除指定索引的行
    /// </summary>
    /// <param name="index"></param>
    public SimpleTable RemoveRow(int index)
    {
        if (index >= 0 && index < _rows.Count)
        {
            _rows.RemoveAt(index);
            _requiresWidthUpdate = true;
        }
        return this;
    }

    /// <summary>
    /// 批量删除符合条件的行
    /// </summary>
    /// <param name="predicate"></param>
    public SimpleTable RemoveRows(Func<string[], bool> predicate)
    {
        var rowsToRemove = _rows.Where(predicate).ToList();
        foreach (var row in rowsToRemove)
        {
            _rows.Remove(row);
        }
        if (rowsToRemove.Any())
        {
            _requiresWidthUpdate = true;
        }
        return this;
    }

    public SimpleTable AddColumn(int? width = null, int minWidth = 0, Alignment alignment = Alignment.Left)
    {
        _columns.Add(new ColumnConfig(width, minWidth, alignment));
        _requiresWidthUpdate = true;
        return this;
    }

    public SimpleTable AddRow(params string[] cells)
    {
        if (cells == null)
        {
            Error($"cells Is Null");
            return null;
        }

        if (cells.Length > _columns.Count)
        {
            Error("列数过大");
            return null;
        }

        var normalizedCells = new string[_columns.Count];
        for (var i = 0; i < normalizedCells.Length; i++)
        {
            normalizedCells[i] = i < cells.Length ? (cells[i] ?? "") : "";
        }

        _rows.Add(normalizedCells);
        _requiresWidthUpdate = true;
        return this;
    }

    public override string ToString()
    {
        UpdateColumnWidths();

        var sb = new StringBuilder();
        foreach (var row in _rows)
        {
            for (int i = 0; i < _columns.Count; i++)
            {
                FormatCell(sb, row[i], _columns[i]);
                if (i < _columns.Count - 1) sb.Append(' ');
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private void UpdateColumnWidths()
    {
        if (!_requiresWidthUpdate) return;

        foreach (var col in _columns.Where(c => !c.FixedWidth.HasValue))
        {
            col.ActualWidth = col.MinWidth;
        }

        foreach (var row in _rows)
        {
            for (int i = 0; i < _columns.Count; i++)
            {
                var col = _columns[i];
                if (col.FixedWidth.HasValue) continue;

                int width = GetEffectiveWidth(row[i]);
                col.ActualWidth = Math.Max(col.ActualWidth, width);
            }
        }
        _requiresWidthUpdate = false;
    }

    private void FormatCell(StringBuilder sb, string value, ColumnConfig config)
    {
        int availableWidth = config.ActualWidth;
        var (cleanValue, formattedValue) = ProcessFormatting(value, availableWidth);

        int displayWidth = GetDisplayWidth(cleanValue);
        int padding = availableWidth - displayWidth;

        switch (config.Alignment)
        {
            case Alignment.Left:
                sb.Append(formattedValue).Append(' ', padding);
                break;
            case Alignment.Right:
                sb.Append(' ', padding).Append(formattedValue);
                break;
            case Alignment.Center:
                int leftPadding = padding / 2;
                int rightPadding = padding - leftPadding;
                sb.Append(' ', leftPadding)
                  .Append(formattedValue)
                  .Append(' ', rightPadding);
                break;
        }
    }

    private (string cleanValue, string formattedValue) ProcessFormatting(string input, int maxWidth)
    {
        input ??= "";
        var tagStack = new Stack<string>();
        var cleanSb = new StringBuilder();
        var formatSb = new StringBuilder();
        int currentWidth = 0;

        var matches = _tagRegex.Matches(input).Cast<Match>().ToList();
        int lastIndex = 0;

        foreach (var match in matches)
        {
            if (match.Index > lastIndex)
                ProcessText(input.Substring(lastIndex, match.Index - lastIndex));

            var tag = match.Groups[1].Value.ToLower();
            if (tag.StartsWith("/"))
            {
                if (tagStack.Count > 0)
                    formatSb.Append("</").Append(tagStack.Pop()).Append('>');
            }
            else
            {
                var tagName = match.Groups[2].Value.ToLower();
                tagStack.Push(tagName);
                formatSb.Append('<').Append(tag).Append('>');
            }

            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < input.Length) ProcessText(input.Substring(lastIndex));

        while (tagStack.Count > 0)
        {
            formatSb.Append("</").Append(tagStack.Pop()).Append('>');
        }

        return (cleanSb.ToString(), formatSb.ToString());

        void ProcessText(string text)
        {
            foreach (var element in GetTextElements(text))
            {
                int charWidth = IsWideCharacter(element) ? 2 : 1;
                if (currentWidth + charWidth > maxWidth) break;

                cleanSb.Append(element);
                formatSb.Append(element);
                currentWidth += charWidth;
            }
        }
    }

    private static int GetEffectiveWidth(string value)
    {
        var cleanValue = _tagRegex.Replace(value, "");
        return GetDisplayWidth(cleanValue);
    }

    private static int GetDisplayWidth(string text)
    {
        int width = 0;
        var enumerator = StringInfo.GetTextElementEnumerator(text);
        while (enumerator.MoveNext())
        {
            var element = enumerator.GetTextElement();
            width += IsWideCharacter(element) ? 2 : 1;
        }
        return width;
    }

    private static bool IsWideCharacter(string element)
    {
        if (string.IsNullOrEmpty(element)) return false;
        var codePoint = char.ConvertToUtf32(element, 0);

        return (codePoint >= 0x1100 && codePoint <= 0x11FF) ||
               (codePoint >= 0x2E80 && codePoint <= 0x9FFF) ||
               (codePoint >= 0xAC00 && codePoint <= 0xD7AF) ||
               (codePoint >= 0xF900 && codePoint <= 0xFAFF) ||
               (codePoint >= 0xFE30 && codePoint <= 0xFE4F) ||
               (codePoint >= 0xFF00 && codePoint <= 0xFFEF) ||
               (codePoint >= 0x20000 && codePoint <= 0x2FA1F);
    }

    private static IEnumerable<string> GetTextElements(string input)
    {
        var enumerator = StringInfo.GetTextElementEnumerator(input);
        while (enumerator.MoveNext())
        {
            yield return enumerator.GetTextElement();
        }
    }
}

public enum Alignment
{
    Left,
    Center,
    Right
}
