using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace ZeroX.DashboardSystem
{
    public static class ZDBUtility
    {
        #region Internal

        private static bool HasDefine(NamedBuildTarget namedBuildTarget, string define)
        {
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] listDefine);
            for (int i = 0; i < listDefine.Length; i++)
            {
                if (listDefine[i] == define)
                    return true;
            }

            return false;
        }
        
        private static bool HasAllDefine(NamedBuildTarget namedBuildTarget, List<string> listDefineWantCheck)
        {
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] listDefine);

            foreach (string define in listDefineWantCheck)
            {
                if (listDefine.Contains(define) == false)
                    return false;
            }

            return true;
        }
        
        private static bool HasAnyDefine(NamedBuildTarget namedBuildTarget, List<string> listDefineWantCheck)
        {
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] listDefine);

            foreach (string define in listDefineWantCheck)
            {
                if (listDefine.Contains(define))
                    return true;
            }

            return false;
        }
        
        private static void SetDefine(NamedBuildTarget namedBuildTarget, string define)
        {
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] arrayDefine);

            var listDefine = arrayDefine.ToList();
            if (listDefine.Contains(define))
                return;
            
            
            listDefine.Add(define);
            arrayDefine = listDefine.ToArray();
            
            
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, arrayDefine);
        }
        
        private static void SetDefines(NamedBuildTarget namedBuildTarget, List<string> listDefineWantAdd)
        {
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] arrayDefine);

            var listDefine = arrayDefine.ToList();
            foreach (var define in listDefineWantAdd)
            {
                if (listDefine.Contains(define))
                    continue;
                
                listDefine.Add(define);
            }
            
            
            arrayDefine = listDefine.ToArray();
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, arrayDefine);
        }

        private static void RemoveDefine(NamedBuildTarget namedBuildTarget, string define)
        {
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] arrayDefine);

            var listDefine = arrayDefine.ToList();
            listDefine.Remove(define);
            arrayDefine = listDefine.ToArray();
            
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, arrayDefine);
        }
        
        private static void RemoveDefines(NamedBuildTarget namedBuildTarget, List<string> listDefineWantRemove)
        {
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] arrayDefine);

            var listDefine = arrayDefine.ToList();
            foreach (var define in listDefineWantRemove)
            {
                listDefine.Remove(define);
            }
            
            arrayDefine = listDefine.ToArray();
            
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, arrayDefine);
        }

        private static NamedBuildTarget GetCurrentBuildTarget()
        {
            return NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        }
        
        private static List<NamedBuildTarget> GetAllNameBuildTargetSupported()
        {
            var result = new Dictionary<string, NamedBuildTarget>();

            foreach (BuildTarget buildTarget in Enum.GetValues(typeof(BuildTarget)))
            {
                try
                {
                    var group = BuildPipeline.GetBuildTargetGroup(buildTarget);

                    if (group == BuildTargetGroup.Unknown)
                        continue;

                    if (!BuildPipeline.IsBuildTargetSupported(group, buildTarget))
                        continue;

                    var namedTarget = NamedBuildTarget.FromBuildTargetGroup(group);

                    if (namedTarget == NamedBuildTarget.Unknown)
                        continue;

                    // Tránh duplicate, ví dụ StandaloneWindows64 / StandaloneOSX đều map về Standalone
                    result[namedTarget.TargetName] = namedTarget;
                }
                catch
                {
                    // Một số BuildTarget cũ / obsolete có thể gây lỗi tùy version Unity
                    Debug.LogException(new Exception("Not supported build target: " + buildTarget));
                }
            }

            return result.Values.ToList();
        }
        
        #endregion
        
        
        
        
        
        
        public static bool HasDefine(string define)
        {
            return HasDefine(GetCurrentBuildTarget(), define);
        }
        
        public static bool HasAllDefine(List<string> listDefine)
        {
            return HasAllDefine(GetCurrentBuildTarget(), listDefine);
        }
        
        public static bool HasAnyDefine(List<string> listDefine)
        {
            return HasAnyDefine(GetCurrentBuildTarget(), listDefine);
        }

        public static void SetDefine(string define)
        {
            var listBuildTarget = GetAllNameBuildTargetSupported();
            foreach (var buildTarget in listBuildTarget)
            {
                SetDefine(buildTarget, define);
            }
        }
        
        public static void SetDefines(List<string> listDefine)
        {
            var listBuildTarget = GetAllNameBuildTargetSupported();
            foreach (var buildTarget in listBuildTarget)
            {
                SetDefines(buildTarget, listDefine);
            }
        }

        public static void RemoveDefine(string define)
        {
            var listBuildTarget = GetAllNameBuildTargetSupported();
            foreach (var buildTarget in listBuildTarget)
            {
                RemoveDefine(buildTarget, define);
            }
        }
        
        public static void RemoveDefines(List<string> listDefine)
        {
            var listBuildTarget = GetAllNameBuildTargetSupported();
            foreach (var buildTarget in listBuildTarget)
            {
                RemoveDefines(buildTarget, listDefine);
            }
        }
    }
}