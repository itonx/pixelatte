namespace Pixelatte.UI.Models
{
    internal class FilterOperation
    {
        public string Name { get; set; }
        public int[]? DefaultKernel { get; set; }
        public bool UseKernel { get; set; }

        public FilterOperation(string name, int[]? defaultKernel = null)
        {
            Name = name;
            DefaultKernel = defaultKernel;
            UseKernel = defaultKernel != null;
        }
    }
}
