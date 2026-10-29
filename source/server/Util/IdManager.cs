using System;
using System.Collections.Generic;

public class IdManager<T>
{
    private readonly HashSet<int> _usedIds = new HashSet<int>(); // Tracks assigned IDs
    private readonly Dictionary<int, T> _idToEntityMap = new Dictionary<int, T>(); // Maps IDs to entities
    private int _nextAvailableId = 0; // Tracks the smallest unused ID

    public int AssignNewId(T entity)
    {
        // Find the first available ID
        int newId = _nextAvailableId;
        while (_usedIds.Contains(newId))
        {
            newId++;
        }

        _usedIds.Add(newId);
        _idToEntityMap[newId] = entity;

        // Update next available ID for efficiency
        _nextAvailableId = newId + 1;

        return newId;
    }
    public bool IsIdBusy(int id)
    {
        return _usedIds.Contains(id);
    }
    public void RemoveId(int id)
    {
        if (_idToEntityMap.ContainsKey(id))
        {
            _idToEntityMap.Remove(id);
            _usedIds.Remove(id);

            // Optionally update _nextAvailableId to minimize the next ID value
            _nextAvailableId = Math.Min(_nextAvailableId, id);
        }
        else
        {
            throw new ArgumentException($"ID {id} does not exist.");
        }
    }

    public void SetIdBusy(int id)
    {
        if (!_usedIds.Contains(id))
        {
            _usedIds.Add(id);
            _nextAvailableId = Math.Min(_nextAvailableId, id + 1);
        }
        else
        {
            throw new InvalidOperationException($"ID {id} is already in use.");
        }
    }

    public void ReclaimId(int id)
    {
        if (_usedIds.Contains(id))
        {
            _usedIds.Remove(id);
            _idToEntityMap.Remove(id); // Remove any associated entity if exists
            _nextAvailableId = Math.Min(_nextAvailableId, id);
        }
        else
        {
            throw new ArgumentException($"ID {id} is not currently in use.");
        }
    }

    public T GetEntityById(int id)
    {
        if (_idToEntityMap.TryGetValue(id, out T entity))
        {
            return entity;
        }

        throw new KeyNotFoundException($"Entity with ID {id} does not exist.");
    }

    public int GetEntityCount()
    {
        return _idToEntityMap.Count;
    }

    public Dictionary<int, T> GetAllEntities()
    {
        return new Dictionary<int, T>(_idToEntityMap);
    }
}
