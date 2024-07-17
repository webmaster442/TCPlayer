using System.ComponentModel;

namespace TcPlayer.ViewModels;
internal static class Extensions
{
    public static void Swap<T>(this IList<T> collection, int sourceIndex, int targetIndex)
    {
        (collection[targetIndex], collection[sourceIndex]) = (collection[sourceIndex], collection[targetIndex]);
    }

    public static void Shuffle<T>(this BindingList<T> list)
    {
        list.RaiseListChangedEvents = false;
        int swaps = list.Count / 2;
        HashSet<int> tracker = new(swaps);
        for (int i=0; i<swaps; i++)
        {
            int source = Random.Shared.Next(0, list.Count);
            int target = Random.Shared.Next(0, list.Count);
            while (tracker.Contains(source))
            {
                source = Random.Shared.Next(0, list.Count);
            }
            while (tracker.Contains(target) || source == target)
            {
                target = Random.Shared.Next(0, list.Count);
            }
            tracker.Add(source);
            tracker.Add(target);
            list.Swap(source, target);
        }
        list.RaiseListChangedEvents = true;
        list.ResetBindings();
    }

    public static void AddRange<T>(this BindingList<T> list, IEnumerable<T> items)
    {
        list.RaiseListChangedEvents = false;
        foreach (var item in items)
        {
            list.Add(item);
        }
        list.RaiseListChangedEvents = true;
        list.ResetBindings();
    }
}
