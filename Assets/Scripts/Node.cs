public class Cell
{
    public int X {get;} = 0;
    public int Y {get;} = 0;

    public Cell(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public bool Equals(Cell other)
    {
        return other != null && this.X == other.X && this.Y == other.Y;
    }
}

public class Node : Cell
{
    public int CurrentCost {get;set;} = 0;
    public int Weight {get;set;} = 0;
    public Node Parent {get;set;} = null;

    public Node(int x, int y) : base(x, y) { }

    public Node(int x, int y, int weight) : base(x, y)
    {
        this.Weight = weight;
    }

    public Node(int x, int y, int weight, Node parent) : base(x, y)
    {
        this.Weight = weight;
        this.Parent = parent;
    }

    public bool LocatedAt(Cell loc)
    {
        return this.X == loc.X && this.Y == loc.Y;
    }
}