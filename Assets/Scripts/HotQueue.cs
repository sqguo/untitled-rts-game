using System.Collections.Generic;

public class HotQueue
{
    public int NumBuckets {get;set;} = 0;
    public int MissTolerance = 5;
    private List<int> WeightRanges;
    private List<BinaryHeap> Heaps;


    // Accepts minimal weight of zero, minimal num buckets of one
    // And size(weightRanges) = numBuckets-1
    public HotQueue(int numBuckets, List<int> weightRanges)
    {
        this.NumBuckets = numBuckets;
        this.WeightRanges = weightRanges;
        this.Heaps = new List<BinaryHeap>();
        this.Heaps.Add(new BinaryHeap(true));
        for (int i = 1; i < NumBuckets; i++) {
            this.Heaps.Add(new BinaryHeap(false, this.MissTolerance));
        }
    }

    private int GetBucketIndex(Node node)
    {
        for (int i = 0; i < NumBuckets-1; i++) {
            if (WeightRanges[i] >= node.Weight) {
                return i;
            }
        }
        return NumBuckets-1;
    }

    public void Insert(Node node)
    {
        Heaps[GetBucketIndex(node)].InsertNode(node);
    }

    public Node Extract()
    {
        for (int i = 0; i < NumBuckets; i++) {
            Node result = Heaps[i].ExtractMinNode();
            if (result != null) {
                return result;
            }
        }
        return null;
    }

    public void Clear()
    {
        NumBuckets = 0;
        WeightRanges.Clear();
        Heaps.Clear();
    }

    public int GetTotalMissCount()
    {
        int missCounter = 0;
        for (int i = 0; i < NumBuckets; i++) {
            missCounter += Heaps[i].MissCounter;
        }
        return missCounter;
    }
}
