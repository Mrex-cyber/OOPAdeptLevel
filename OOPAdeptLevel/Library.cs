using System.Threading;

namespace OOPAdeptLevel
{
    class Consumer
    {
        public string? Name { get; set; }
        public int? Age { get; set; }
        private bool IsReading = false;
        Thread consumerThread;
        Semaphore libSemaphore = new Semaphore(2, 2);
        int count = 2;
        public Consumer(int count)
        {
            consumerThread = new Thread(Shopping);
            consumerThread.Name = $"Consumer: {count}";
            consumerThread.Start();
        }

        public void Shopping()
        {
            while (count > 0)
            {
                libSemaphore.WaitOne();
                Console.WriteLine($"Consumer {Thread.CurrentThread.Name} went in!");

                Console.WriteLine($"Consumer {Thread.CurrentThread.Name} is looking for...");
                Thread.Sleep(1000);

                Console.WriteLine($"Consumer {Thread.CurrentThread.Name} went out ^_^");
                libSemaphore.Release();

                count--;
                Thread.Sleep(1000);
            }

        }
    }
    
}
