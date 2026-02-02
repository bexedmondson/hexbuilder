#if TOOLS
using System;
using System.Collections.Generic;
using System.Text;
using Godot;

[Tool]
public partial class TilePluginParentNode : Control
{
    [Export]
    private TileSet tileSet;

    [Export]
    private Control genericInspectorFloatingWindow;

    [Export]
    private Control genericInspectorContainer;

    public override void _EnterTree()
    {
        EditorTileDatabase.AddTileSetTileData(tileSet);
        genericInspectorFloatingWindow.Visible = false;
        //GD.Print("enter");
    }

    public override void _Ready()
    {
        base._Ready();
        EditorTileDatabase.AddTileSetTileData(tileSet);
    }

    public void AddTileData()
    {
        EditorTileDatabase.AddTileSetTileData(tileSet);
    }

    private Resource currentSelectedGenericResource;

    private Dictionary<string, EditorProperty> propertyEditors = new();
    private EditorInspector inspector;

    public void ShowGenericResourceInspector(object obj)
    {
        if (obj is not Resource resource)
            return;

        currentSelectedGenericResource = resource;

        if (inspector == null)
        {
            inspector = new EditorInspector();
            genericInspectorContainer.AddChild(inspector);
        }

        inspector.Edit(resource);
        inspector.Theme = EditorInterface.Singleton.GetEditorTheme();

        IterateAllNodesForResize(inspector);
        
        genericInspectorFloatingWindow.Visible = true;
        OnPropSizeChanged();

        inspector.EditedObjectChanged += () => IterateAllNodesForResize(inspector);
    }

    private void IterateAllNodesForResize(Node node, int i = 0)
    {
        foreach (var child in node.GetChildren())
        {
            /*StringBuilder sb = new();
            for (int j = 0; j < i; j++)
            {
                sb.Append("|");
            }
            GD.Print(sb.AppendLine(child.Name).ToString());*/

            if (child is EditorProperty prop)
                prop.NameSplitRatio = 0.2f;

            if (child.GetChildCount() == 0)
                continue;
            
            i++;
            IterateAllNodesForResize(child, i);
        }
    }

    private void OnPropSizeChanged()
    {
        genericInspectorContainer.ResetSize();
    }

    private void OnPropChanged(StringName property, Variant value, StringName field, bool changing)
    {
        currentSelectedGenericResource.Set(property, value);
        
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

    public void HideGenericResourceInspector()
    {
        genericInspectorFloatingWindow.Visible = false;
        foreach (var kvp in propertyEditors)
        {
            kvp.Value.QueueFree();
        }
        propertyEditors.Clear();
    }

    public override void _ExitTree()
    {
        base._ExitTree(); 
        if (inspector != null)
            inspector.QueueFree();
        inspector = null;
        //GD.Print("exit");
    }
}
#endif