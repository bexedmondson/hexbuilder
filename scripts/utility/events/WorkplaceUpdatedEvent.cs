using System.Collections.Generic;

public class WorkplaceUpdatedEvent : IEvent
{
    public List<WorkplaceState> newOrChangedWorkplaces = new();
    public List<WorkplaceState> removedWorkplaces = new();

    public bool HasAnythingUpdated => newOrChangedWorkplaces.Count > 0 || removedWorkplaces.Count > 0;
}
