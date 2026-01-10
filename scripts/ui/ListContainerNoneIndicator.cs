using Godot;

public partial class ListContainerNoneIndicator : Control
{
    [Export]
    private Control noneIndicator;

    [ExportSubgroup("Optional")]
    [Export]
    private Control listParent;

    public override void _EnterTree()
    {
        base._EnterTree();
        
        if (listParent == null)
        {
            ChildEnteredTree += OnChildEntered;
            ChildExitingTree += OnChildExited;
        }
        else
        {
            listParent.ChildEnteredTree += OnChildEntered;
            listParent.ChildExitingTree += OnChildExited;
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        
        if (listParent == null)
        {
            ChildEnteredTree -= OnChildEntered;
            ChildExitingTree -= OnChildExited;
        }
        else
        {
            listParent.ChildEnteredTree -= OnChildEntered;
            listParent.ChildExitingTree -= OnChildExited;
        }
    }

    private void OnChildEntered(Node n)
    {
        noneIndicator.Visible = false;
    }
    
    private void OnChildExited(Node n)
    {
        var prevChildCount = listParent?.GetChildCount() ?? GetChildCount();
        if (prevChildCount <= 1)
            noneIndicator.Visible = true;
    }
}
