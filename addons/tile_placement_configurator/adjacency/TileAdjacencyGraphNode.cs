#if TOOLS
using System;
using System.Collections.Generic;
using Godot;

[Tool]
public partial class TileAdjacencyGraphNode : GraphNode
{
    [Export]
    private TextureRect textureRect;

    public TileAdjacencyGraph graph;

    public CustomTileData selectedTileData;

    private Dictionary<string, EditorProperty> propertyEditors = new();

    public AdjacencyConfig adjacencyConfig { get; private set; }
    
    public void SetAdjacencyConfig(AdjacencyConfig adjacencyConfig)
    {
        this.adjacencyConfig = adjacencyConfig;
        
        OnAdjacencyConfigSet();
    }

    public override void _Ready()
    {
        base._Ready();
        if (adjacencyConfig != null)
            OnAdjacencyConfigSet();
    }
    
    public void OnAdjacencyConfigSet()
    {
        Title = adjacencyConfig.requiredTile.GetFileName();
        textureRect.Texture = EditorTileDatabase.GetTileTexture(adjacencyConfig.requiredTile);

        foreach (var kvp in propertyEditors)
        {
            kvp.Value.QueueFree();
        }
        propertyEditors.Clear();

        var props = adjacencyConfig.GetPropertyList();

        foreach (var prop in props)
        {
            var nameProperty = (string)prop["name"].Obj;
            if (nameProperty is "distance" or "currencyEffect")
            {
                var editor = EditorInspector.InstantiatePropertyEditor(adjacencyConfig, (Variant.Type)prop["type"].Obj, nameProperty, (PropertyHint)prop["hint"].Obj, (string)prop["hint_string"].Obj, Convert.ToUInt32(prop["usage"].Obj));
                this.AddChild(editor);

                editor.SetObjectAndProperty(adjacencyConfig, nameProperty);
                editor.Label = nameProperty;
                editor.PropertyChanged += OnPropChanged;
                editor.Selected += OnPropSelected;
                editor.MinimumSizeChanged += OnPropSizeChanged;
                editor.UpdateProperty();
                propertyEditors[nameProperty] = editor;
            }
        }
    }

    private void OnPropSizeChanged()
    {
        this.ResetSize();
    }

    private void OnPropChanged(StringName property, Variant value, StringName field, bool changing)
    {
        adjacencyConfig.Set(property, value);
        
        if (value.VariantType >= Variant.Type.Dictionary)
            propertyEditors[property].UpdateProperty();
    }

    private void OnPropSelected(string path, long focusableIdx)
    {
        foreach (var kvp in propertyEditors)
        {
            if (kvp.Key != path)
                kvp.Value.Deselect();
        }
    }
    
    public void OnDeleteButton()
    {
        selectedTileData.adjacencies.Remove(adjacencyConfig);
        ResourceSaver.Save(selectedTileData);

        graph.OnNodeDataUpdated();
    }
}
#endif