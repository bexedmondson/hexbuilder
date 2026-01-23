using System;
using System.Collections.Generic;
using Godot;

[Tool]
public partial class TileInfoGraphNode : GraphNode
{
    [Export]
    private TextureRect textureRect;

    [Export]
    private Button deleteButton;

    public TileInfoGraph graph;

    public CustomTileData selectedTileData;

    private Dictionary<string, EditorProperty> propertyEditors = new();

    public CustomTileData tileData { get; private set; }

    public void SetCustomTileData(CustomTileData customTileData)
    {
        tileData = customTileData;
        
        OnCustomTileDataSet();
    }

    public override void _Ready()
    {
        base._Ready();
        if (tileData != null)
            OnCustomTileDataSet();
    }

    private void OnCustomTileDataSet()
    {
        Title = tileData.GetFileName();
        /*GD.Print(" ");
        GD.Print("texture rect: " + textureRect);
        GD.Print(new System.Diagnostics.StackTrace().ToString());*/
        textureRect.Texture = EditorTileDatabase.GetTileTexture(tileData);
        //GD.Print("texture: " + textureRect.Texture);
        //GD.Print(" ");
        
        foreach (var kvp in propertyEditors)
        {
            kvp.Value.QueueFree();
        }
        propertyEditors.Clear();

        var props = tileData.GetPropertyList();

        foreach (var prop in props)
        {
            var nameProperty = (string)prop["name"].Obj;
            if (nameProperty is "components" or "buildPrice" or "baseTurnCurrencyChange")
            {
                var editor = EditorInspector.InstantiatePropertyEditor(tileData, (Variant.Type)prop["type"].Obj, nameProperty, (PropertyHint)prop["hint"].Obj, (string)prop["hint_string"].Obj, Convert.ToUInt32(prop["usage"].Obj));
                this.AddChild(editor);

                editor.SetObjectAndProperty(tileData, nameProperty);
                editor.Label = nameProperty;
                editor.PropertyChanged += OnPropChanged;
                editor.Selected += OnPropSelected;
                editor.MinimumSizeChanged += OnPropSizeChanged;
                editor.UpdateProperty();
                propertyEditors[nameProperty] = editor;
            }
        }
    }

    public void SetupAsEditable(bool shouldBeEditable)
    {
        deleteButton.Visible = shouldBeEditable;
    }

    private void OnPropSizeChanged()
    {
        this.ResetSize();
    }

    private void OnPropChanged(StringName property, Variant value, StringName field, bool changing)
    {
        tileData.Set(property, value);
        
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

        graph.OnNodeDataUpdated();
    }

    public void OnFocusButton()
    {
        graph.SelectItem(tileData);
    }
}