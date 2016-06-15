using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompareDocs.Comparer;
using System.Threading.Tasks;
using System.Threading;

namespace CompareDocs.Demo
{
    static class Program
    {
        private static void Main(string[] args)
        { 
            //WordDocComparer();
       
            var parent = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Outer task executing.");

                var child = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Nested task starting.");
                    Thread.SpinWait(500000);
                    Console.WriteLine("Nested task completing.");
                });
            });

            parent.Wait();
            Console.WriteLine("Outer has completed.");

            Console.ReadKey();
        }

        private static void WordDocComparer()
        {
            var doc = new DocComparer(Environment.CurrentDirectory + "\\Source.docx", Environment.CurrentDirectory + "\\Target.docx");
            var result = doc.Compare();

            Console.WriteLine(result);
        }

    }
}
