using ServerSide.Items;
using System.Collections.Generic;
using System;
using System.Linq;

public class ItemIdManager
{
    private int nextAvailableId = 1;
    private HashSet<int> usedIds = new HashSet<int>();

    public int AssignNewId()
    {
        // Find the next available ID
        while (usedIds.Contains(nextAvailableId))
        {
            nextAvailableId++;
        }

        int assignedId = nextAvailableId;
        usedIds.Add(assignedId);
        nextAvailableId++;
        return assignedId;
    }

    public void ReclaimId(int id)
    {
        usedIds.Remove(id);
        // Potentially reset nextAvailableId if it makes sense for your use case
        nextAvailableId = Math.Min(nextAvailableId, id);
    }
    public void SetIdBusy(int id)
    {
        usedIds.Add(id);
    }
    public void ReassignSequentialIds(List<Item> items)
    {
        // Clear existing used IDs
        usedIds.Clear();
        nextAvailableId = 1;

        // Reassign IDs sequentially
        foreach (var item in items.OrderBy(i => i.Name).ThenBy(i => i.Type))
        {
            item.localID = AssignNewId();
        }
    }
}