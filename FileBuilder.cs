
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using RustDataHarmony;
using UnityEngine;

namespace RustDataHarmonyMod
{
    public static class FileBuilder
    {
        public static StringBuilder FileText = new StringBuilder();
        public const string PluginName = "RustData";
        public const string ItemEnumName = "ItemEnum";
        public const string EntityEnumName = "EntityEnum";
        public const string EntityDictionaryName = "EntityPaths";
        public const string SoundEnumName = "SoundEnum";
        public const string SoundDictionaryName = "SoundPaths";
        public static readonly string FinishPath ="C:\\Plugin_Development\\RustDataHarmonyMod\\Finished\\";

        public static void Start()
        {
            Directory.CreateDirectory($"C:\\Plugin_Development\\RustDataHarmonyMod\\Finished\\");
            StartPluginText();
            AppendItems();
            AppendEntites();
            AppendSounds();
            FinishPluginText();
            
            PrintStringToFile();
            CompileCodeToDll();
        }

        #region FileBuilding

        private static void StartPluginText()
        {
            FileText.Append($@"using System;
using System.Collections.Generic;   
using Carbon;
using API.Assembly;

    namespace Carbon.Plugins {{

    [Hotloadable]
	public class {PluginName} : ICarbonExtension
	{{

        public void Awake(EventArgs args)
        {{
        }}

        public void OnLoaded(EventArgs args)
        {{
        }}

        public void OnUnloaded(EventArgs args)
        {{
        }}
");
        }
        
        private static void AppendItems()
        {
            FileText.Append($@"
        public enum {ItemEnumName}
	    {{
        ");
            foreach (var row in ElementChecker.Items)
            {
                FileText.Append($"           {row.EnumElementName} = {row.Path},\n");
            }

            FileText.Append("\n }\n");
        }

        private static void AppendEntites()
        {
            CreateEntityEnum();
            CreateEntityDictionary();
        }

        

        private static void CreateEntityEnum()
        {
            FileText.Append($@"
       public enum {EntityEnumName}
	    {{
");
            foreach (var entry in ElementChecker.Entities)
            {
                FileText.AppendLine($"           {entry.EnumElementName},");
            }
            
            FileText.Append("\n    }\n");

            
        }
        private static void CreateEntityDictionary()
        {
            FileText.Append("\n");
            
            FileText.Append(
                $"      public static Dictionary<{EntityEnumName}, string> {EntityDictionaryName} = new Dictionary<{EntityEnumName}, string>() \n        {{\n");
            
            foreach (var entry in ElementChecker.Entities)
            {
                string dictkey = entry.EnumElementName;
                string dictvalue = entry.Path;

                FileText.AppendLine($"           {{{EntityEnumName}.{dictkey},\"{dictvalue}\"}},");
            }

            FileText.Append("   };\n");
        }
        private static void AppendSounds()
        {
            CreateSoundEnum();
            CreateSoundDictionary();
        }

        private static void CreateSoundEnum()
        {
            FileText.Append($@"
       public enum {SoundEnumName}
	    {{
");

            foreach (var entry in ElementChecker.Sounds)
            {
                FileText.AppendLine($"           {entry.EnumElementName},");
            }
            FileText.Append("\n    }\n");
        }

        private static void CreateSoundDictionary()
        {
            FileText.Append(
                $"      public static Dictionary<{SoundEnumName}, string> {SoundDictionaryName} = new Dictionary<{SoundEnumName}, string>() \n        {{\n");
            
            foreach (var entry in ElementChecker.Sounds)
            {
                string dictkey = entry.EnumElementName;
                string dictvalue = entry.Path;

                FileText.AppendLine($"           {{{SoundEnumName}.{dictkey},\"{dictvalue}\"}},");
            }

            FileText.Append("   };\n");
        }

        private static void FinishPluginText()
        {
            FileText.Append(" \n } \n }");


        }

        #endregion
        
        private static void PrintStringToFile()
        {
            
            File.WriteAllText($"{FinishPath}{PluginName}.cs",
                FileText.ToString());
        }
        
        public static void CompileCodeToDll()
{
    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(FileText.ToString());

    // Path to .NET Framework 4.8 reference assemblies
    string frameworkPath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\";

    // Core .NET Framework references
    List<PortableExecutableReference> references = new List<PortableExecutableReference>
    {
        MetadataReference.CreateFromFile(Path.Combine(frameworkPath, "mscorlib.dll")),
        MetadataReference.CreateFromFile(Path.Combine(frameworkPath, "System.dll")),
        MetadataReference.CreateFromFile(Path.Combine(frameworkPath, "System.Core.dll")),
        MetadataReference.CreateFromFile(Path.Combine(frameworkPath, "System.Data.dll")),
        MetadataReference.CreateFromFile(Path.Combine(frameworkPath, "System.Xml.dll"))
    };

    // Custom assemblies
    references.Add(MetadataReference.CreateFromFile("C:\\Rust_DedicatedData\\Carbon.dll"));
    references.Add(MetadataReference.CreateFromFile("C:\\Rust_DedicatedData\\Carbon.SDK.dll"));

    // Define output path
    string outputPath = $"{FinishPath}{PluginName}.dll";

    // Ensure directory exists
    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

    // Create compilation
    var compilation = CSharpCompilation.Create(
        PluginName,
        new[] { syntaxTree },
        references,
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
    );

    // Emit DLL
    using (var fileStream = new FileStream(outputPath, FileMode.Create))
    {
        EmitResult result = compilation.Emit(fileStream);
        
        fileStream.Close(); // Explicitly close the file stream

        if (!result.Success)
        {
            Debug.LogError("Compilation failed. Errors:");
            foreach (var diagnostic in result.Diagnostics)
            {
                Debug.LogError(diagnostic.ToString());
            }
        }
        else
        {
            Debug.Log($"DLL compiled successfully! Output: {outputPath}");
        }
    }

    // Final check if file actually exists
    if (!File.Exists(outputPath))
    {
        Debug.LogError("DLL compilation reported success, but file was not created.");
    }
}
    }
}