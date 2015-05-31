namespace GraphKinectDLL
{
    sealed class XYQueue
    {
        int[] xs, ys;
        int front, rear;

        public int Count;


        public void delete(out int x, out int y)
        {
            x = xs[rear];
            y = ys[rear];
            rear = (rear + 1) % xs.Length;
            Count--;
        }

        public XYQueue(int count)
        {
            Count = 0;
            xs = new int[count];
            ys = new int[count];
            front = rear = 0;
        }

        public void insert(int x, int y)
        {
            if (Count < xs.Length)
            {
                xs[front] = x;
                ys[front] = y;
                front = (front + 1) % xs.Length;
                Count++;
            }
        }
    }
}
