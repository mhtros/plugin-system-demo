using Humanizer;

namespace Pluralizer
{
    public class PluralizerPlugin : Plugin.Common.ITextPlugin
    {
        public string TransformText(string input) => input.Pluralize();
    }
}