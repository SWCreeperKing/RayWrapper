using System.Numerics;
using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;
using static ImGuiNET.ImGuiTableFlags;

namespace RayWrapper.Imgui.Widgets;

public abstract class ColumnBase
{
    public string header;
    public Func<int> arraySize;
    public Action<int> fromArray;
    public ImGuiTableColumnFlags flags;
    public float widthOrWeight;

    public ColumnBase(string header, Func<int> arraySize, Action<int> fromArray,
        ImGuiTableColumnFlags flags = ImGuiTableColumnFlags.NoSort, float widthOrWeight = 0)
    {
        this.header = header;
        this.arraySize = arraySize;
        this.fromArray = fromArray;
        this.flags = flags;
        this.widthOrWeight = widthOrWeight;
    }

    public virtual void SortSettings(ImGuiSortDirection sortDir)
    {
    }

    public abstract void RenderHeader(uint id);
    public abstract void DrawRow(int row);
}

public class Column : ColumnBase
{
    public Column(string header, Func<int> arraySize, Action<int> fromArray,
        ImGuiTableColumnFlags flags = ImGuiTableColumnFlags.NoSort, float widthOrWeight = 0) : base(header, arraySize,
        fromArray, flags, widthOrWeight)
    {
    }

    public override void RenderHeader(uint id) => ImGui.TableSetupColumn(header, flags, widthOrWeight, id);
    public override void DrawRow(int row) => fromArray.Invoke(row);
}

public class SortableColumn : Column
{
    public Action<ImGuiSortDirection> onSort;

    private ImGuiSortDirection _sortCache = ImGuiSortDirection.None;

    public SortableColumn(string header, Func<int> arraySize, Action<int> fromArray, Action<ImGuiSortDirection> onSort,
        ImGuiTableColumnFlags flags = ImGuiTableColumnFlags.DefaultSort, float widthOrWeight = 0) : base(header,
        arraySize, fromArray, flags, widthOrWeight)
    {
        this.onSort = onSort;
    }

    public override void SortSettings(ImGuiSortDirection sortDir)
    {
        if (_sortCache == sortDir) return;
        onSort.Invoke(_sortCache = sortDir);
    }

    public void SetPreferredSort(ImGuiSortDirection sort)
    {
        flags &= ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.PreferSortAscending |
                 ImGuiTableColumnFlags.PreferSortDescending;
        switch (sort)
        {
            case ImGuiSortDirection.Ascending:
                flags |= ImGuiTableColumnFlags.PreferSortAscending;
                break;
            case ImGuiSortDirection.Descending:
                flags |= ImGuiTableColumnFlags.PreferSortDescending;
                break;
        }
    }
}

public class Table : Widget
{
    public string name;
    public ImGuiTableFlags flags;
    public Vector2 outerSize;
    public float innerSize;

    private List<Column> _columns = new();

    public Table(string name, ImGuiTableFlags flags = Borders, Vector2? outerSize = null,
        float innerSize = 0)
    {
        this.name = name;
        this.flags = flags;
        this.outerSize = outerSize ?? Vector2.Zero;
        this.innerSize = innerSize;
    }

    protected override void RenderCall()
    {
        if (!ImGui.BeginTable(name, _columns.Count, flags, outerSize, innerSize)) return;

        var maxSize = _columns.Select(c => c.arraySize.Invoke()).Min();
        for (var i = 0; i < _columns.Count; i++) _columns[i].RenderHeader((uint) i);
        ImGui.TableHeadersRow();
        
        unsafe
        {
            var specs = ImGui.TableGetSortSpecs();
            if (specs.NativePtr is not null && specs.SpecsDirty)
            {
                for (var i = 0; i < specs.SpecsCount; i++)
                {
                    var sortSpec = specs.Specs.NativePtr[i];
                    _columns[(int) sortSpec.ColumnUserID].SortSettings(sortSpec.SortDirection);
                }
            }
        }

        for (var row = 0; row < maxSize; row++)
        {
            foreach (var t in _columns)
            {
                ImGui.TableNextColumn();
                t.DrawRow(row);
            }
        }

        ImGui.EndTable();
    }

    public void AddColumn(Column column)
    {
        if (column is SortableColumn) flags |= Sortable;
        _columns.Add(column);
    }

    public void AddColumn(params Column[] columns)
    {
        foreach (var column in columns) AddColumn(column);
    }
}

public partial class CompoundWidgetBuilder
{
    public CompoundWidgetBuilder AddTable(string name, ImGuiTableFlags flags = Borders, Vector2? outerSize = null,
        float innerSize = 0, params Column[] columns)
    {
        Table table = new(name, flags, outerSize, innerSize);
        table.AddColumn(columns);
        RegisterWidget(table);
        return this;
    }
}