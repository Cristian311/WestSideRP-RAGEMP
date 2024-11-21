using System.Collections.Generic;

public class PlayerIdManager
{
    private Queue<int> availableIds; // Pool of reusable IDs
    private int nextId; // Tracks the next new ID to use

    public PlayerIdManager()
    {
        availableIds = new Queue<int>();
        nextId = 0;
    }

    // Get an available ID for a new player
    public int AssignId()
    {
        if (availableIds.Count > 0)
        {
            return availableIds.Dequeue(); // Reuse an ID from the pool
        }
        return nextId++; // No reusable ID, assign a new one
    }

    // Reclaim an ID when a player disconnects
    public void ReclaimId(int id)
    {
        if (id >= 0) // Ensure valid IDs are reclaimed
        {
            availableIds.Enqueue(id);
        }
    }
}