using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;
using Novacode;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace CompareDocs.GPU
{

    [Cudafy]
    public struct StringSphere
    {
        public char[] ch;

        public StringSphere(string s)
        {
            this.ch = s.ToCharArray();
        }

        public int GetSize()
        {
            return ch.Length;
        }

    }

    public class NVideoManager
    {
        private static readonly Formatting _formatting = new Formatting
        {
            Bold = true,
            FontColor = System.Drawing.Color.Red
        };
        public static int N { get; set; }
        public static DocX Doc { get; set; }

        public static int Execute(IList<string> exists)
        {
            var km = CudafyTranslator.Cudafy();
            var gpu = CudafyHost.GetDevice(CudafyModes.Target, CudafyModes.DeviceId);

            gpu.LoadModule(km);

            // allocate memory for the Sphere dataset
            var s = exists.Select(a => new StringSphere(a)).ToArray();

            // allocate temp memory, initialize it, copy to constant memory on the GPU
            var dev_s = gpu.Allocate<StringSphere>(N);

            // copy the arrays 'a' and 'b' to the GPU
            gpu.CopyToDevice(s, dev_s);

            // launch add on N threads
            gpu.Launch(N, 1).replacer(dev_s);

            // copy the array 'c' back from the GPU to the CPU
            gpu.CopyFromDevice(dev_s, s);

            // display the results
            var total = 0;
            for (var i = 0; i < N; i++)
            {
                total += s[i].GetSize();
            }

            Doc.Save();

            // free the memory allocated on the GPU
            gpu.Free(dev_s);

            gpu.FreeAll();

            return total;
        }

        [Cudafy]
        public static void replacer(GThread thread, StringSphere s)
        {
            int tid = thread.blockIdx.x;

            if (tid < N)
            {
                try
                {
                    var txt = new String(s.ch);
                    Doc.ReplaceText(txt, txt, false, RegexOptions.IgnoreCase, _formatting, null, MatchFormattingOptions.ExactMatch);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
