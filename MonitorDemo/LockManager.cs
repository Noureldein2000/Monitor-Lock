using System.Collections.Concurrent;
using System.Threading;

public class LockManager
{
    private static readonly ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
    
    public bool IsNumberLocked(object number)
    {
        // Check if the lock exists for the number
        if (_locks.ContainsKey(number))
        {
            return true; // Number is locked
        }
        return false; // Number is not locked
    }

    public bool TryLockNumber(object number)
    {
        // Try to add a new lock for the number
        var semaphore = _locks.GetOrAdd(number, _ => new SemaphoreSlim(1, 1));

        // Try to enter the semaphore without blocking
        if (semaphore.Wait(0))
        {
            // Successfully acquired the lock
            return true;
        }

        // Lock already exists; could not acquire it
        return false;
    }

    public void ReleaseLock(object number)
    {
        if (_locks.TryGetValue(number, out var semaphore))
        {
            // Release the semaphore and remove it if it's free
            semaphore.Release();

            // Remove the lock if no more threads are waiting
            if (semaphore.CurrentCount == 1) // If no threads are waiting
            {
                _locks.TryRemove(number, out _);
            }
        }
    }
}
