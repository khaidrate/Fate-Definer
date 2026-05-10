using System;
using System.Collections.Generic;
using System.Linq;

namespace FateDefiner.Models
{
    public class Note
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "New Session Note";
        public string Content { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastModified { get; set; } = DateTime.Now;
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// Convenience property for binding a comma-separated tag string in the UI.
        /// </summary>
        public string TagsString
        {
            get => string.Join(", ", Tags);
            set
            {
                Tags = value
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();
            }
        }

        public override string ToString() => Title;
    }
}
