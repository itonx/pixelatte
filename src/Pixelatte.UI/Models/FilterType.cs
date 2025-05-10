using System.Collections.Generic;

namespace Pixelatte.UI.Models
{
    internal class FilterType
    {
        public string Name { get; set; }
        public List<FilterOperation> Operations { get; set; }

        public FilterType(string name, List<FilterOperation> operations)
        {
            Name = name;
            Operations = operations;
        }
    }
}
