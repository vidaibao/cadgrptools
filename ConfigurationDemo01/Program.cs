using System.Configuration;

namespace ConfigurationDemo01
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var option = ConfigurationManager.AppSettings["option"];


            Console.WriteLine("Hello, World!");
        }
    }
}