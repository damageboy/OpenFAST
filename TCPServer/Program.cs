using System;

namespace OpenFAST.TCPServer
{
    internal class Program
    {
        private static void Main()
        {
            try
            {
                new FastServer();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadLine();
        }
    }
}