using System;

namespace TCPServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                new FASTServer();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadLine();
        }
    }
}