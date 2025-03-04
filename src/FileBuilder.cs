
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Carbon;
using Carbon.Hooks;
using Carbon.Pooling;
using ConVar;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Oxide.Core;
using RustDataHarmony;
using UnityEngine;
using Console = System.Console;

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
        public static readonly string FinishPath ="C:\\Plugin_Development\\FinishedRustData\\";

        public static void Start()
        {
            Directory.CreateDirectory(FinishPath);
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

    

    
	public class {PluginName}
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
            FileText.Append(" \n }");

           // FileText.Append("public interface IHookUtils{\n");
            // Debug.Log($"LoadedPatches{Community.Runtime.HookManager.LoadedPatches.Count()}");
            // Debug.Log($"LoadedDynamicHooks{Community.Runtime.HookManager.LoadedDynamicHooks.Count()}");
            // Debug.Log($"LoadedStaticHooks{Community.Runtime.HookManager.LoadedStaticHooks.Count()}");
            // Debug.Log($"InstalledPatches{Community.Runtime.HookManager.InstalledPatches.Count()}");
            // Debug.Log($"InstalledDynamicHooks{Community.Runtime.HookManager.InstalledDynamicHooks.Count()}");
            // Debug.Log($"InstalledDynamicHooks{Community.Runtime.HookManager.InstalledStaticHooks.Count()}");
            
            // foreach (var hook in Community.Runtime.HookManager.LoadedDynamicHooks)
            // {
            //     try
            //     {
            //         var hookex = hook as HookEx;
            //
            //         if (hookex == null)
            //         {
            //             Console.WriteLine("hookex is null!");
            //             return;
            //         }
            //         Console.WriteLine("START");
            //         Console.WriteLine($"hookex.HookName{hookex.HookName}");
            //         Console.WriteLine($"hookex.TargetType{hookex.}");
            //         
            //
            //         for (var index = 0; index < hookex.TargetMethodArgs.Length; index++)
            //         {
            //             var hookexTargetMethodArg = hookex.TargetMethodArgs[index];
            //             if (hookexTargetMethodArg != null)
            //                Console.WriteLine($"hookex.TargetMethodArgs{index} = {hookexTargetMethodArg.Name}");
            //         }
            //
            //         Console.WriteLine("END");
            //
            //         
            //     }
            //     catch (Exception e)
            //     {
            //         Console.WriteLine(e);
            //     }
            //    
            //     // FileText.Append($"private void {hook.HookName}()\n");
            //     // FileText.Append($"{{Debug.Log({hook.HookName} called!);}}\n");
            // }
            // FileText.Append("}");
           
        }
        
        #endregion
        
        private static void PrintStringToFile()
        {
            
            File.WriteAllText($"{FinishPath}{PluginName}.cs",
                FileText.ToString());
        }
        
       public static void CompileCodeToDll()
{
    // Ensure we use the latest C# version for compilation
    var parseOptions = new CSharpParseOptions(LanguageVersion.LatestMajor);
    
    // Create SyntaxTree with C# 12+ support
    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(FileText.ToString(), parseOptions);

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
        new[] { syntaxTree }, // Ensure syntaxTree uses parseOptions
        references,
        new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            optimizationLevel: OptimizationLevel.Release,
            allowUnsafe: true,
            concurrentBuild: true
        )
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