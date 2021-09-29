using Plugin.Common;
using System;
using System.Linq;
using System.Runtime.Loader;

namespace Plugin.Host
{
    internal static class Program
    {
        private const bool UseCollectibleContexts = true;
        private const string SolutionPath = @"...\PluginSystemDemo";

        private static void Main()
        {
            var captializer = @$"{SolutionPath}\Capitalizer\bin\Debug\net5.0\Capitalizer.dll";
            var pluralizer = @$"{SolutionPath}\Pluralizer\bin\Debug\net5.0\Pluralizer.dll";

            Console.WriteLine(TransformText("big apple", captializer));
            Console.WriteLine(TransformText("big apple", pluralizer));

            if (UseCollectibleContexts) return;

            foreach (var context in AssemblyLoadContext.All)
            {
                Console.WriteLine($"Context: {context.GetType().Name}{context.Name}");
                foreach (var assembly in context.Assemblies)
                    Console.WriteLine($" Assembly: {assembly.FullName}");
            }
        }

        private static string TransformText(string text, string pluginPath)
        {
            var alc = new PluginLoadContext(pluginPath, UseCollectibleContexts);

            try
            {
                var assembly = alc.LoadFromAssemblyPath(pluginPath);

                // Locate the type in the assembly that implements ITextPlugin:
                var pluginType = assembly.ExportedTypes.Single(t => typeof(ITextPlugin).IsAssignableFrom(t));

                var plugin = (ITextPlugin)Activator.CreateInstance(pluginType);
                return plugin?.TransformText(text);
            }
            finally
            {
                if (UseCollectibleContexts) alc.Unload(); // unload the ALC
            }
        }
    }
}