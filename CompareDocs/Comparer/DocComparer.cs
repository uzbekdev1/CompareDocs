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

namespace CompareDocs.Comparer
{
    public class DocComparer
    {
        private readonly string _sourceFilePath;
        private readonly string _targetFilePath;
        private int _exists;
        private int _total;

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

            stopWatch.Start();

            File.Copy(_sourceFilePath, filePath, true);

            using (var sourceDoc = DocX.Load(filePath))
            {
                var sourceParagraphs = sourceDoc.Paragraphs.Where(w => !string.IsNullOrWhiteSpace(w.Text));

                using (var targetDoc = DocX.Load(_targetFilePath))
                {

                    var targetParagraphs = targetDoc.Paragraphs.Where(w => !string.IsNullOrWhiteSpace(w.Text));

                    _total = targetParagraphs.Sum(s => s.Text.Length);

                    foreach (var sourceParagraph in sourceParagraphs)
                    {
                        var sourcePhrases = sourceParagraph.Text.ToSplit(".");
                         
                        foreach (var targetParagraph in targetParagraphs)
                        {
                            var targetPhrases = targetParagraph.Text.ToSplit(".");
                            var formatting = new Formatting
                            {
                                Bold = true,
                                FontColor = Color.Red
                            };

                            foreach (var sourcePhrase in sourcePhrases)
                            {
                                var sourceChunks = sourcePhrase.ToSplit(Helpers.Options.EXCLUDE_CHARACTERS);

                                foreach (var targetPhrase in targetPhrases)
                                {
                                    var targetChunks = targetPhrase.ToSplit(Helpers.Options.EXCLUDE_CHARACTERS);

                                    if (!sourceChunks.SequenceEqual(targetChunks, StringComparer.OrdinalIgnoreCase))
                                        continue;

                                    for (var i = 0; i < targetChunks.Length; i++)
                                    {
                                        if (string.CompareOrdinal(targetChunks[i], sourceChunks[i]) == 0)
                                        {
                                            try
                                            {
                                                sourceParagraph.ReplaceText
                                                (
                                                    sourceChunks[i],
                                                    sourceChunks[i],
                                                    false,
                                                    RegexOptions.IgnoreCase,
                                                    formatting,
                                                    null,
                                                    MatchFormattingOptions.ExactMatch
                                                );
                                            }
                                            catch (Exception)
                                            {
                                            }

                                            _exists += targetChunks[i].Length;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                sourceDoc.Save();
            }

            stopWatch.Stop();

            ElapsedTime = string.Format("{0}:{1}:{2}", stopWatch.Elapsed.Hours.ToString("00"),
                stopWatch.Elapsed.Minutes.ToString("00"), stopWatch.Elapsed.Seconds.ToString("00"));
            ComparedFile = filePath;

            return _exists * 1F / _total;
        }
    }
}