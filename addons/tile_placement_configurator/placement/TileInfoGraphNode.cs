using System;
using System.Collections.Generic;
using Godot;

[Tool]
public partial class TileInfoGraphNode : GraphNode
{
    [Export]
    private TextureRect textureRect;

    public TileInfoGraph graph;

    public CustomTileData selectedTileData;

    private Dictionary<string, EditorProperty> propertyEditors = new();

    private CustomTileData tileData;
    public CustomTileData customTileData
    {
        get => tileData;
        set
        {
            tileData = value;
            OnCustomTileDataSet();
        }
    }
    
    public void OnCustomTileDataSet()
    {
        Title = customTileData.GetFileName();
        textureRect.Texture = InjectionManager.Get<TileDatabase>().GetTileTexture(customTileData);

        foreach (var kvp in propertyEditors)
        {
            kvp.Value.QueueFree();
        }
        propertyEditors.Clear();

        var props = customTileData.GetPropertyList();

        foreach (var prop in props)
        {
            var nameProperty = (string)prop["name"].Obj;
            if (nameProperty == "price" || nameProperty == "baseTurnCurrencyChange" || nameProperty == "unlockRequirements")
            {
                var editor = EditorInspector.InstantiatePropertyEditor(customTileData, (Variant.Type)prop["type"].Obj, nameProperty, (PropertyHint)prop["hint"].Obj, (string)prop["hint_string"].Obj, Convert.ToUInt32(prop["usage"].Obj));
                this.AddChild(editor);

                editor.SetObjectAndProperty(customTileData, nameProperty);
                editor.Label = nameProperty;
                editor.PropertyChanged += OnPropChanged;
                editor.Selected += OnPropSelected;
                editor.UpdateProperty();
                propertyEditors[nameProperty] = editor;
            }
        }
    }

    private void OnPropChanged(StringName property, Variant value, StringName field, bool changing)
    {
        customTileData.Set(property, value);
        
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
        selectedTileData.canBePlacedOn.Remove(tileData);
        ResourceSaver.Save(selectedTileData);

        graph.OnAdjacencyUpdated();
    }
}