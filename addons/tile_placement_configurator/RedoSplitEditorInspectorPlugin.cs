#if TOOLS
using Godot;

[Tool]
public partial class RedoSplitEditorInspectorPlugin : EditorInspectorPlugin
{
    //this doesn't work and i don't know why, putting it down for now
    public override bool _ParseProperty(GodotObject @object, Variant.Type type, string name, PropertyHint hintType, string hintString, PropertyUsageFlags usageFlags, bool wide)
    {
        var editor = EditorInspector.InstantiatePropertyEditor(@object, type, name, hintType, hintString, (uint)usageFlags, wide);
        editor.NameSplitRatio = 0.2f;
        AddPropertyEditor(name, editor, true);
        return true;
    }

    public override bool _CanHandle(GodotObject @object)
    {
        return @object is Resource;
    }
}
#endif