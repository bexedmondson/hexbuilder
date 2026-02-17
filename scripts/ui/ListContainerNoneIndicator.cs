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
        if (n is Control c)
        {
            if (c.Visible)
                noneIndicator.Visible = false;
            
            c.VisibilityChanged += OnChildVisibilityChanged;
        }
        else
        {
            noneIndicator.Visible = false;
        }
    }
    
    private void OnChildExited(Node n)
    {
        if (n is Control c)
            c.VisibilityChanged -= OnChildVisibilityChanged;
        
        var prevChildCount = listParent?.GetChildCount() ?? GetChildCount();
        if (prevChildCount <= 1)
            noneIndicator.Visible = true;
        
        OnChildVisibilityChanged();
    }

    private void OnChildVisibilityChanged()
    {
        foreach (var child in listParent?.GetChildren() ?? GetChildren())
        {
            if (child is not Control c || c.Visible)
            {
                noneIndicator.Visible = false;
                return;
            }
        }

        noneIndicator.Visible = true;
    }
}
