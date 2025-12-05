namespace TheOtherRoles.Helper;

public static class ListHelper
{
    extension<T>(ISystem.List<T> list)
    {
        public T Get(int index)
        {
            return list._items[index];
        }

        public T Get(Index index)
        {
            return list._items[index];
        }

        public List<T> ToList()
        {
            List<T> newList = new(list.Count);
            foreach (T item in list)
            {
                newList.Add(item);
            }
            return newList;
        }

        public int Count(Func<T, bool> func = null)
        {
            int count = 0;
            foreach (T obj in list)
                if (func == null || func(obj)) count++;
            return count;
        }

        public T Find(Predicate<T> match)
        {
            foreach (var item in list)
                if (match(item)) return item;
            return default;
        }

        public T FirstOrDefault(Func<T, bool> func)
        {
            foreach (T obj in list)
                if (func(obj))
                    return obj;
            return default;
        }

    }

    extension<T>(List<T> list)
    {
        public bool Any(Func<T, bool> func)
        {
            if (list == null)
                return false;
            foreach (T obj in list)
                if (func(obj))
                    return true;
            return false;
        }

        public bool TryAdd(T item)
        {
            if (list == null || item == null || list.Contains(item)) return false;
            try
            {
                list.Add(item);
                return true;
            }
            catch (Exception e)
            {
                Message(e);
                return false;
            }
        }

        public T FirstOrDefault()
        {
            if (list.Count > 0)
                return list[0];
            return default;
        }

        public int GetRandomIndex()
        {
            var indexData = URandom.Range(0, list.Count);
            return indexData;
        }

        public List<T> Shuffle()
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
            return list;
        }

        public T RandomTake()
        {
            if (list == null || list.Count == 0) return default;
            int index = rnd.Next(list.Count);
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }


        public T RandomOrEmpty(T emptyValue, int emptyChance = 33)
        {
            if (list.Count == 0) return emptyValue;
            if (rnd.Chance(emptyChance))
                return emptyValue;

            int index = rnd.Next(list.Count);
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }

    }
}