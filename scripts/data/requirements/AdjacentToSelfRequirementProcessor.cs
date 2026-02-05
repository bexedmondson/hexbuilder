using Godot;

public class AdjacentToSelfRequirementProcessor(AdjacentToSelfRequirement dataRequirement) : CellInfoRequirementProcessor<AdjacentToSelfRequirement>(dataRequirement)
{
    public override bool IsSatisfied(Vector2I cell)
    {
        var mapController = InjectionManager.Get<MapController>();
        var selfData = mapController.BaseMapLayer.GetCellCustomData(cell);
        var neighbours = mapController.BaseMapLayer.GetSurroundingCells(cell);

        foreach (var neighbour in neighbours)
        {
            if (mapController.BaseMapLayer.GetCellCustomData(neighbour) == selfData)
                return true;
        }
        return false;
    }
}