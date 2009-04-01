using System;
namespace TCPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Threading.Thread.Sleep(1000);
            try
            {
                var client = new FASTClient("127.0.0.1", 16121);
                client.Connect();
                System.Threading.Thread.Sleep(1000);
                while (true)
                {
                    DateTime startTime = DateTime.Now;
                    for (int i = 0; i < 64000; i++)
                    {
                        client.SendMessage("GOOG");
                    }
                    double seconds = (DateTime.Now - startTime).TotalSeconds;
                    Console.WriteLine(seconds);
                    Console.WriteLine("MSG/S:" + (64000/seconds).ToString("0"));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }
    }
}
