class PathComparer : IEqualityComparer<List<Point>>
{
    public bool Equals(List<Point> x, List<Point> y)
    {
        if (x.Count != y.Count)
            return false;

        for (int i = 0; i < x.Count; i++)
        {
            if (x[i].X != y[i].X || x[i].Y != y[i].Y)
                return false;
        }

        return true;
    }

    public int GetHashCode(List<Point> obj)
    {
        int hash = 17;
        foreach (var point in obj)
        {
            hash = hash * 23 + point.X.GetHashCode();
            hash = hash * 23 + point.Y.GetHashCode();
        }
        return hash;
    }
}