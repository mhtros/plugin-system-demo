using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Plugin.Common;

namespace Plugin.Host
{
    internal class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath, bool collectible) : base(Path.GetFileName(pluginPath), collectible)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (assemblyName.Name == typeof(ITextPlugin).Assembly.GetName().Name)
                return null;

            var target = _resolver.ResolveAssemblyToPath(assemblyName);
            return target != null ? LoadFromAssemblyPath(target) : null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var path = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            return path == null
                ? IntPtr.Zero
                : LoadUnmanagedDllFromPath(path);
        }
    }
}