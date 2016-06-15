using System;

namespace CompareDocs.Models
{
    public class CompareResult
    {

        public string FileName { get; set; }

        public string ComparedFile { get; set; }

        public float Result { get; set; }

        private string _percent;

        public string Percent
        {
            get { return _percent ?? String.Format("{0:P}", Result); }
            set { _percent = value; }
        }

        public string Elapsed { get; set; }

    }
}
