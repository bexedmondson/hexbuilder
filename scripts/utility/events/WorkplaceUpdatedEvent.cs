using System.Collections.Generic;

public class WorkplaceUpdatedEvent : IEvent
{
    public List<WorkplaceData> newOrChangedWorkplaces = new();
    public List<WorkplaceData> removedWorkplaces = new();

    public bool HasAnythingUpdated => newOrChangedWorkplaces.Count > 0 || removedWorkplaces.Count > 0;
}
