using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System.IO;

/// <summary>
/// Generates a detailed JSON class diagram from Unity/C# assemblies
/// Captures: namespaces, class names, inheritance, interfaces, properties, methods, and visibility
/// </summary>
public class ClassDiagramGenerator : MonoBehaviour
{
    [System.Serializable]
    public class ClassDiagram
    {
        public List<ClassEntry> classes = new();
        public List<InterfaceEntry> interfaces = new();
    }

    [System.Serializable]
    public class ClassEntry
    {
        public string @namespace;
        public string name;
        public string fullName;
        public string baseClass;
        public List<string> interfaces = new();
        public bool isAbstract;
        public bool isSealed;
        public bool isStatic;
        public List<PropertyEntry> properties = new();
        public List<MethodEntry> methods = new();
        public List<FieldEntry> fields = new();
    }

    [System.Serializable]
    public class InterfaceEntry
    {
        public string @namespace;
        public string name;
        public string fullName;
        public List<string> baseInterfaces = new();
        public List<PropertyEntry> properties = new();
        public List<MethodEntry> methods = new();
    }

    [System.Serializable]
    public class PropertyEntry
    {
        public string name;
        public string type;
        public string visibility;
        public bool canRead;
        public bool canWrite;
        public bool isAbstract;
    }

    [System.Serializable]
    public class MethodEntry
    {
        public string name;
        public string returnType;
        public string visibility;
        public bool isAbstract;
        public bool isVirtual;
        public bool isStatic;
        public List<ParameterEntry> parameters = new();
    }

    [System.Serializable]
    public class ParameterEntry
    {
        public string name;
        public string type;
        public bool isOut;
        public bool isRef;
        public bool isParams;
    }

    [System.Serializable]
    public class FieldEntry
    {
        public string name;
        public string type;
        public string visibility;
        public bool isStatic;
        public bool isReadOnly;
    }

    /// <summary>
    /// Generates class diagram from specified types
    /// </summary>
    public static ClassDiagram GenerateDiagram(Type[] types)
    {
        var diagram = new ClassDiagram();

        foreach (var type in types)
        {
            if (type.IsInterface)
            {
                diagram.interfaces.Add(ExtractInterfaceInfo(type));
            }
            else
            {
                diagram.classes.Add(ExtractClassInfo(type));
            }
        }

        return diagram;
    }

    /// <summary>
    /// Extracts class information using reflection
    /// </summary>
    private static ClassEntry ExtractClassInfo(Type type)
    {
        var classInfo = new ClassEntry
        {
            @namespace = type.Namespace ?? "Global",
            name = type.Name,
            fullName = type.FullName,
            baseClass = type.BaseType?.Name != "Object" ? type.BaseType?.Name : null,
            isAbstract = type.IsAbstract,
            isSealed = type.IsSealed,
            isStatic = type.IsAbstract && type.IsSealed
        };

        // Extract interfaces
        foreach (var interfaceType in type.GetInterfaces())
        {
            classInfo.interfaces.Add(interfaceType.Name);
        }

        // Extract properties
        var properties = type.GetProperties(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
        foreach (var prop in properties)
        {
            classInfo.properties.Add(new PropertyEntry
            {
                name = prop.Name,
                type = GetFriendlyTypeName(prop.PropertyType),
                visibility = GetPropertyVisibility(prop),
                canRead = prop.CanRead,
                canWrite = prop.CanWrite,
                isAbstract = prop.GetGetMethod()?.IsAbstract ?? false
            });
        }

        // Extract fields
        var fields = type.GetFields(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
        foreach (var field in fields)
        {
            classInfo.fields.Add(new FieldEntry
            {
                name = field.Name,
                type = GetFriendlyTypeName(field.FieldType),
                visibility = GetFieldVisibility(field),
                isStatic = field.IsStatic,
                isReadOnly = field.IsInitOnly
            });
        }

        // Extract methods
        var methods = type.GetMethods(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
        foreach (var method in methods.Where(m => !m.IsSpecialName)) // Exclude property accessors
        {
            var methodEntry = new MethodEntry
            {
                name = method.Name,
                returnType = GetFriendlyTypeName(method.ReturnType),
                visibility = GetMethodVisibility(method),
                isAbstract = method.IsAbstract,
                isVirtual = method.IsVirtual,
                isStatic = method.IsStatic
            };

            // Extract parameters
            foreach (var param in method.GetParameters())
            {
                methodEntry.parameters.Add(new ParameterEntry
                {
                    name = param.Name,
                    type = GetFriendlyTypeName(param.ParameterType),
                    isOut = param.IsOut,
                    isRef = param.ParameterType.IsByRef,
                    isParams = param.GetCustomAttribute<ParamArrayAttribute>() != null
                });
            }

            classInfo.methods.Add(methodEntry);
        }

        return classInfo;
    }

    /// <summary>
    /// Extracts interface information using reflection
    /// </summary>
    private static InterfaceEntry ExtractInterfaceInfo(Type type)
    {
        var interfaceInfo = new InterfaceEntry
        {
            @namespace = type.Namespace ?? "Global",
            name = type.Name,
            fullName = type.FullName
        };

        // Extract base interfaces
        foreach (var baseInterface in type.GetInterfaces())
        {
            interfaceInfo.baseInterfaces.Add(baseInterface.Name);
        }

        // Extract properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            interfaceInfo.properties.Add(new PropertyEntry
            {
                name = prop.Name,
                type = GetFriendlyTypeName(prop.PropertyType),
                visibility = "public",
                canRead = prop.CanRead,
                canWrite = prop.CanWrite
            });
        }

        // Extract methods
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (var method in methods)
        {
            var methodEntry = new MethodEntry
            {
                name = method.Name,
                returnType = GetFriendlyTypeName(method.ReturnType),
                visibility = "public"
            };

            foreach (var param in method.GetParameters())
            {
                methodEntry.parameters.Add(new ParameterEntry
                {
                    name = param.Name,
                    type = GetFriendlyTypeName(param.ParameterType)
                });
            }

            interfaceInfo.methods.Add(methodEntry);
        }

        return interfaceInfo;
    }

    /// <summary>
    /// Gets friendly type name (handles generics and built-in types)
    /// </summary>
    private static string GetFriendlyTypeName(Type type)
    {
        if (type.IsArray)
            return GetFriendlyTypeName(type.GetElementType()) + "[]";

        if (type.IsByRef)
            return GetFriendlyTypeName(type.GetElementType());

        if (type.IsGenericType)
        {
            var genericName = type.Name.Substring(0, type.Name.IndexOf('`'));
            var genericArgs = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyTypeName));
            return $"{genericName}<{genericArgs}>";
        }

        return type.Name;
    }

    private static string GetPropertyVisibility(PropertyInfo property)
    {
        var getter = property.GetGetMethod(true);
        var setter = property.GetSetMethod(true);

        if (getter?.IsPublic == true || setter?.IsPublic == true)
            return "public";
        if (getter?.IsPrivate == true && setter?.IsPrivate == true)
            return "private";
        if (getter?.IsAssembly == true || setter?.IsAssembly == true)
            return "internal";
        if (getter?.IsFamilyOrAssembly == true || setter?.IsFamilyOrAssembly == true)
            return "protected internal";
        if (getter?.IsFamily == true || setter?.IsFamily == true)
            return "protected";

        return "private";
    }

    private static string GetFieldVisibility(FieldInfo field)
    {
        if (field.IsPublic) return "public";
        if (field.IsPrivate) return "private";
        if (field.IsAssembly) return "internal";
        if (field.IsFamilyOrAssembly) return "protected internal";
        if (field.IsFamily) return "protected";

        return "private";
    }

    private static string GetMethodVisibility(MethodInfo method)
    {
        if (method.IsPublic) return "public";
        if (method.IsPrivate) return "private";
        if (method.IsAssembly) return "internal";
        if (method.IsFamilyOrAssembly) return "protected internal";
        if (method.IsFamily) return "protected";

        return "private";
    }

    // ==================== EDITOR/RUNTIME GENERATION ====================

#if UNITY_EDITOR
    [ContextMenu("Generate Class Diagram (All Assemblies)")]
    public void GenerateFromAllAssemblies()
    {
        var allTypes = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.FullName.StartsWith("Unity.") && !a.FullName.StartsWith("UnityEditor."))
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsGenericTypeDefinition && t.Namespace != null)
            .ToArray();

        GenerateAndSave(allTypes, "Assets/JSON/ClassDiagram_AllAssemblies.json");
    }

    [ContextMenu("Generate Class Diagram (Project Only)")]
    public void GenerateFromProjectAssemblies()
    {
        var projectAssemblies = new[] { "Assembly-CSharp" };
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => projectAssemblies.Contains(a.GetName().Name))
            .SelectMany(a => a.GetTypes())
            .Where(t => 
                !t.IsGenericTypeDefinition
                && !t.Name.StartsWith("<")  // Compiler-generated
                && t.GetCustomAttribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>() == null)
            .ToArray();

        GenerateAndSave(types, "Assets/JSON/ClassDiagram_ProjectOnly.json");
    }
#endif

    public static void GenerateAndSave(Type[] types, string outputPath)
    {
        var diagram = GenerateDiagram(types);
        var json = JsonUtility.ToJson(diagram, prettyPrint: true);

        // Ensure directory exists
        var directory = Path.GetDirectoryName(outputPath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(outputPath, json);
        Debug.Log($"Class diagram saved to: {outputPath}");
    }
}
