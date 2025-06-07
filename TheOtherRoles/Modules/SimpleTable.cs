using System.Text;
using System.Text.RegularExpressions;

namespace TheOtherRoles.Modules;

public class SimpleTable
{
    public enum Alignment { Left, Center, Right }

    private class Column
    {
        public string Header { get; set; }
        public Alignment Alignment { get; set; }
        public int? ManualWidth { get; set; }
    }

    private readonly List<Column> _columns = new();
    private readonly List<string[]> _rows = new();
    private int[] _finalWidths;
    private static readonly Regex _richTextRegex = new(@"<[^>]*>");

    // API
    public SimpleTable AddColumn(string header = "", Alignment alignment = Alignment.Left, int? manualWidth = null)
    {
        _columns.Add(new Column { Header = header, Alignment = alignment, ManualWidth = manualWidth });
        return this;
    }

    public SimpleTable AddRow(params string[] cells)
    {
        _rows.Add(cells ?? Array.Empty<string>());
        return this;
    }

    public SimpleTable ClearRows()
    {
        _rows.Clear();
        return this;
    }

    public SimpleTable RemoveRow(int index)
    {
        if (index >= 0 && index < _rows.Count) _rows.RemoveAt(index);
        return this;
    }

    public SimpleTable RemoveRows(Func<string[], bool> predicate)
    {
        _rows.RemoveAll(row => predicate(row));
        return this;
    }

    // Core
    public override string ToString()
    {
        if (_columns.Count == 0) return string.Empty;

        CalculateColumnWidths();
        var sb = new StringBuilder();

        AppendRow(sb, _columns.Select(c => c.Header).ToArray());

        foreach (var row in _rows)
        {
            AppendRow(sb, row);
        }

        return sb.ToString();
    }

    private void CalculateColumnWidths()
    {
        _finalWidths = new int[_columns.Count];

        for (int i = 0; i < _columns.Count; i++)
        {
            if (_columns[i].ManualWidth.HasValue)
            {
                _finalWidths[i] = _columns[i].ManualWidth.Value;
            }
        }

        for (int i = 0; i < _columns.Count; i++)
        {
            if (!_columns[i].ManualWidth.HasValue)
            {
                string cleanText = StripRichText(_columns[i].Header);
                _finalWidths[i] = Math.Max(_finalWidths[i], GetVisualWidth(cleanText));
            }
        }

        foreach (var row in _rows)
        {
            for (int i = 0; i < _columns.Count; i++)
            {
                if (i < row.Length && !_columns[i].ManualWidth.HasValue)
                {
                    string cleanText = StripRichText(row[i]);
                    _finalWidths[i] = Math.Max(_finalWidths[i], GetVisualWidth(cleanText));
                }
            }
        }
    }

    private static string StripRichText(string input)
    {
        return string.IsNullOrEmpty(input)
            ? input
            : _richTextRegex.Replace(input, "");
    }

    private static int GetVisualWidth(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        int width = 0;
        foreach (char c in text)
        {
            width += IsWideChar(c) ? 2 : 1;
        }
        return width;
    }

    private static bool IsWideChar(char c)
    {
        return (c >= 0x4E00 && c <= 0x9FFF) ||
               (c >= 0x3000 && c <= 0x303F) ||
               (c >= 0xFF00 && c <= 0xFFEF) ||
               (c >= 0x1100 && c <= 0x11FF) ||
               (c >= 0x1F600 && c <= 0x1F64F);
    }

    private void AppendRow(StringBuilder sb, string[] cells)
    {
        for (int i = 0; i < _columns.Count; i++)
        {
            string cell = (i < cells.Length) ? cells[i] : "";
            string cleanText = StripRichText(cell);
            int targetWidth = _finalWidths[i];
            int visualWidth = GetVisualWidth(cleanText);

            if (visualWidth > targetWidth)
            {
                cell = TruncateWithRichText(cell, targetWidth);
                cleanText = StripRichText(cell);
                visualWidth = GetVisualWidth(cleanText);
            }

            switch (_columns[i].Alignment)
            {
                case Alignment.Left:
                    sb.Append(cell).Append(' ', targetWidth - visualWidth);
                    break;
                case Alignment.Right:
                    sb.Append(' ', targetWidth - visualWidth).Append(cell);
                    break;
                case Alignment.Center:
                    int leftSpaces = (targetWidth - visualWidth) / 2;
                    int rightSpaces = targetWidth - visualWidth - leftSpaces;
                    sb.Append(' ', leftSpaces).Append(cell).Append(' ', rightSpaces);
                    break;
            }

            if (i < _columns.Count - 1) sb.Append(' ');
        }
        sb.AppendLine();
    }

    private static string TruncateWithRichText(string text, int maxWidth)
    {
        if (string.IsNullOrEmpty(text)) return text;

        StringBuilder sb = new StringBuilder();
        int width = 0;
        bool insideTag = false;

        foreach (char c in text)
        {
            if (c == '<') insideTag = true;

            if (insideTag)
            {
                sb.Append(c);
                if (c == '>') insideTag = false;
            }
            else
            {
                int charWidth = IsWideChar(c) ? 2 : 1;
                if (width + charWidth > maxWidth) break;
                sb.Append(c);
                width += charWidth;
            }
        }

        return sb.ToString();
    }
}