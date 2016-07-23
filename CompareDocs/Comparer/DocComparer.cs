using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CompareDocs.Extensions;
using Novacode;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using DocumentFormat.OpenXml.Packaging;
using CompareDocs.GPU;

namespace CompareDocs.Comparer
{
    public class DocComparer
    {
        private readonly string _sourceFilePath;
        private readonly string _targetFilePath;

        public DocComparer(string source, string target)
        {
            _sourceFilePath = source;
            _targetFilePath = target;
        }

        public string ElapsedTime { get; private set; }
        public string ComparedFile { get; private set; }

        public float Compare()
        {
            var filePath = Helpers.GetTempFile(_sourceFilePath);
            var stopWatch = new Stopwatch();
            var existItems = 0;
            var totalItems = 0;

            stopWatch.Start();

            File.Copy(_sourceFilePath, filePath, true);

            var totalSourceChunks = new List<string>();
            using (var sourceDoc = DocX.Load(filePath))
            {
                var sourceParagraphs = sourceDoc.Paragraphs.Where(w => !string.IsNullOrWhiteSpace(w.Text));

                foreach (var sourceParagraph in sourceParagraphs)
                {
                    var sourcePhrases = sourceParagraph.Text.ToSplit(".");
                    var sourceChunks = sourcePhrases.SelectMany(s => s.ToSplit(Helpers.Options.EXCLUDE_CHARACTERS));

                    totalSourceChunks.AddRange(sourceChunks);
                }

            }

            var totalTargetChunks = new List<string>();
            using (var targetDoc = DocX.Load(_targetFilePath))
            {
                var targetParagraphs = targetDoc.Paragraphs.Where(w => !string.IsNullOrWhiteSpace(w.Text));

                totalItems = targetParagraphs.Sum(s => s.Text.Length);

                foreach (var targetParagraph in targetParagraphs)
                {
                    var targetPhrases = targetParagraph.Text.ToSplit(".");
                    var targetChunks = targetPhrases.SelectMany(s => s.ToSplit(Helpers.Options.EXCLUDE_CHARACTERS));

                    totalTargetChunks.AddRange(targetChunks);
                }
            }

            var exists = totalTargetChunks.FindAll(f => totalSourceChunks.Exists(e => string.CompareOrdinal(f, e) == 0));

            NVideoManager.N = exists.Count;

            using (var sourceDoc = DocX.Load(filePath))
            {
                NVideoManager.Doc = sourceDoc;

                existItems = NVideoManager.Execute(exists);
            }

            stopWatch.Stop();

            ElapsedTime = string.Format("{0}:{1}:{2}", stopWatch.Elapsed.Hours.ToString("00"), stopWatch.Elapsed.Minutes.ToString("00"), stopWatch.Elapsed.Seconds.ToString("00"));
            ComparedFile = filePath;

            return existItems * 1F / totalItems;
        }
    }
}