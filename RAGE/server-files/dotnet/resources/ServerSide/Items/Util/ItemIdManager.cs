using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Items.Util
{
    public class ItemIdManager
    {
        private SortedSet<int> usedIds = new SortedSet<int>();

        public int AssignId()
        {
            int id = 0;

            // Find the smallest unused ID
            foreach (var usedId in usedIds)
            {
                if (id < usedId)
                    break;
                id++;
            }

            usedIds.Add(id);
            return id;
        }
        public void SetIdBusy(int id)
        {
            usedIds.Add(id);
        }
        public void ReclaimId(int id)
        {
            usedIds.Remove(id);
        }
    }
}
