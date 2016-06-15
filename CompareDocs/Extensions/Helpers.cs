using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Novacode;
using System.Threading;
using System.Threading.Tasks;

namespace CompareDocs.Extensions
{
    public static class Helpers
    {
        public struct Options
        {
            public const string DOCX = ".docx";
            public const string TEMP = "~$";
            public const string FILTER = "*.docx;";
            public static readonly string EXCLUDE_CHARACTERS = " ,:;-()%?&@#$=/+*!<>~[]{}";
        }

        public static IEnumerable<FileInfo> GetFiles(string directory)
        {
            return Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(Options.DOCX, StringComparison.InvariantCultureIgnoreCase) &&
                            !s.StartsWith(Options.TEMP, StringComparison.InvariantCultureIgnoreCase))
                .Select(s => new FileInfo(s));
        }

        public static string[] ToSplit(this string s, string seperators)
        {
            return s.Split(seperators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetTempFile(string file)
        {
            return Path.Combine(Path.GetTempPath(), Path.GetFileName(file));
        }
         
    }
}
