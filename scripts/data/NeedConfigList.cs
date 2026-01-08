using Godot;
using Godot.Collections;

[GlobalClass][Tool]
public partial class NeedConfigList : Resource
{
    [Export]
    public Array<NeedConfig> needConfigs;
}
