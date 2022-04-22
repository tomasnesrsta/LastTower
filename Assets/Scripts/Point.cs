[System.Serializable]
public struct Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public static bool IsEqual(Point p1, Point p2)
    {
        if (p1.X == p2.X && p1.Y == p2.Y)
            return true;
        return false;
    }

    public static Point operator -(Point x, Point y)
    {
        return new Point(x.X - y.X, x.Y - y.Y);
    }
}
