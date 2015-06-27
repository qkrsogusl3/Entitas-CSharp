using System;
using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {
    public static class CodeGenerator {
        public const string componentSuffix = "Component";

        public static void Generate(Type[] types, string[] poolNames, string dir,
                                    IComponentCodeGenerator[] componentCodeGenerators,
                                    ISystemCodeGenerator[] systemCodeGenerators,
                                    IPoolCodeGenerator[] poolCodeGenerators) {

            dir = GetSafeDir(dir);
            CleanDir(dir);
            
            var components = GetComponents(types);
            foreach (var generator in componentCodeGenerators) {
                var files = generator.Generate(components);
                writeFiles(dir + "Components/", files);
            }

            var systems = GetSystems(types);
            foreach (var generator in systemCodeGenerators) {
                var files = generator.Generate(systems);
                writeFiles(dir + "Systems/", files);
            }

            foreach (var generator in poolCodeGenerators) {
                var files = generator.Generate(poolNames);
                writeFiles(dir + "Pools/", files);
            }
        }

        public static string GetSafeDir(string dir) {
            if (!dir.EndsWith("/", StringComparison.Ordinal)) {
                dir += "/";
            }
            if (!dir.EndsWith("Generated/", StringComparison.Ordinal)) {
                dir += "Generated/";
            }
            return dir;
        }

        public static void CleanDir(string dir) {
            dir = GetSafeDir(dir);
            if (Directory.Exists(dir)) {
                FileInfo[] files = new DirectoryInfo(dir).GetFiles("*.cs", SearchOption.AllDirectories);
                foreach (var file in files) {
                    try {
                        File.Delete(file.FullName);
                    } catch {
                        Console.WriteLine("Could not delete file " + file);
                    }
                }
            } else {
                Directory.CreateDirectory(dir);
            }
        }

        public static Type[] GetComponents(Type[] types) {
            return types
                .Where(type => type.GetInterfaces().Contains(typeof(IComponent)))
                .ToArray();
        }

        public static Type[] GetSystems(Type[] types) {
            return types
                .Where(type => !type.IsInterface
                    && type != typeof(ReactiveSystem)
                    && type != typeof(Systems)
                    && type.GetInterfaces().Contains(typeof(ISystem)))
                .ToArray();
        }

        static void writeFiles(string dir, CodeGenFile[] files) {
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            foreach (var file in files) {
                File.WriteAllText(dir + file.fileName + ".cs", file.fileContent);
            }
        }
    }

    public static class EntitasCodeGeneratorExtensions {
        public static string RemoveComponentSuffix(this Type type) {
            const string componentSuffix = CodeGenerator.componentSuffix;
            if (type.Name.EndsWith(componentSuffix)) {
                return type.Name.Substring(0, type.Name.Length - componentSuffix.Length);
            }

            return type.Name;
        }

        public static string PoolName(this Type type) {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            foreach (Attribute attr in attrs) {
                var customPool = attr as PoolAttribute;
                if (customPool != null) {
                    return customPool.tag;
                }
            }

            return string.Empty;
        }

        public static string IndicesLookupTag(this Type type) {
            return type.PoolName() + "ComponentIds";
        }

        public static string UppercaseFirst(this string str) {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string LowercaseFirst(this string str) {
            return char.ToLower(str[0]) + str.Substring(1);
        }
    }
}
