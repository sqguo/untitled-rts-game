using System.Collections.Generic;

public class BinaryHeap
{
    public int NumElements {get;set;} = 0;
    public int MissCounter {get;set;} = 0;
    public int MissTolerance {get;set;} = 0;
    public bool IsActive {get;set;} = false; // Whether to 
    private List<Node> HeapArray; // Null at index zero

    public BinaryHeap(bool isActive)
    {
        NumElements = 0;
        HeapArray = new List<Node>();
        HeapArray.Add(null);
        this.IsActive = isActive;
        MissCounter = 0;
    }

    public BinaryHeap(bool isActive, int missTolerance)
    {
        NumElements = 0;
        HeapArray = new List<Node>();
        HeapArray.Add(null);
        this.IsActive = isActive;
        MissCounter = 0;
        this.MissTolerance = missTolerance;
    }

    private int GetParentIndex(int index)
    {
        return index/2;
    }

    private int GetLeftChildIndex(int index)
    {
        return index*2;
    }

    private int GetRightChildIndex(int index)
    {
        return index*2+1;
    }

    public void ActivateHeap()
    {
        if (!IsActive) {
            BuildHeap();
            IsActive = true;
        }
    }

    public void DeactivateHeap()
    {
        IsActive = false;
    }

    // Extracts the node with the lowest weight
    // Must ensure the heap is ordered before extraction
    // If heap is inactive O(N), otherwise O(logn)
    public Node ExtractMinNode()
    {
        if (NumElements <= 0) {
            return null;
        }

        if (!IsActive) {
            MissCounter++;
            BuildHeap();
            if (MissCounter > MissTolerance) {
                IsActive = true;
            }
        }

        Node result = HeapArray[1];
        HeapArray[1] = HeapArray[NumElements];
        HeapArray.RemoveAt(NumElements);
        NumElements--;

        if (IsActive && NumElements > 0) {
            HeapifyDownFromIndex(1);
        }
        
        return result;
    }

    // Inserts a node into the heap O(logn) if heap is active
    // Otherwise O(1) without updating the order of the heap
    public void InsertNode(Node node)
    {
        NumElements++;
        HeapArray.Add(node);
        if (IsActive) {
            HeapifyUpFromIndex(NumElements);
        }
    }

    private void HeapifyUpFromIndex(int index)
    {
        int parentIndex = GetParentIndex(index);
        if (index > 1 && HeapArray[index].Weight < HeapArray[parentIndex].Weight) {
            Node tmp = HeapArray[parentIndex];
            HeapArray[parentIndex] = HeapArray[index];
            HeapArray[index] = tmp;
            HeapifyUpFromIndex(parentIndex);
        }
    }

    private void HeapifyDownFromIndex(int index)
    {
        int smallestIndex = index;
        int leftChildIndex = GetLeftChildIndex(index);
        int rightChildIndex = GetRightChildIndex(index);
        if (leftChildIndex <= NumElements && HeapArray[leftChildIndex].Weight < HeapArray[smallestIndex].Weight) {
            smallestIndex = leftChildIndex;
        }
        if (rightChildIndex <= NumElements && HeapArray[rightChildIndex].Weight < HeapArray[smallestIndex].Weight) {
            smallestIndex = rightChildIndex;
        }
        if (smallestIndex != index) {
            Node tmp = HeapArray[smallestIndex];
            HeapArray[smallestIndex] = HeapArray[index];
            HeapArray[index] = tmp;
            HeapifyDownFromIndex(smallestIndex);
        }
    }

    private void BuildHeap()
    {
        for (int i = NumElements/2; i >= 1; i--) {
            HeapifyDownFromIndex(i);
        }
    }
}