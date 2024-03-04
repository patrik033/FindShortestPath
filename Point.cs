class Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Point other = (Point)obj;
        return (X == other.X) && (Y == other.Y);
    }

    public override int GetHashCode()
    {
        return (X << 2) ^ Y;
    }
}
