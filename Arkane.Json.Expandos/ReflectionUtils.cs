#region header

// Arkane.Json.Expandos - ReflectionUtils.cs
// 
// Alistair J. R. Young
// Arkane Systems
// 
// Copyright Arkane Systems 2012-2017.  All rights reserved.
// 
// Created: 2017-08-17 5:22 PM

#endregion

#region using

using JetBrains.Annotations ;

#endregion

namespace ArkaneSystems.Json.Expandos
{
    internal static class ReflectionUtils
    {
        // Utilities taken from https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Utilities/ReflectionUtils.cs
        // I couldn't find a way to access these directly.

        public static void SplitFullyQualifiedTypeName ([NotNull] string fullyQualifiedTypeName,
                                                        out string typeName,
                                                        [CanBeNull] out string assemblyName)
        {
            int? assemblyDelimiterIndex = ReflectionUtils.GetAssemblyDelimiterIndex (fullyQualifiedTypeName) ;

            if (assemblyDelimiterIndex != null)
            {
                typeName = fullyQualifiedTypeName.Substring (0, assemblyDelimiterIndex.GetValueOrDefault ()).Trim () ;
                assemblyName = fullyQualifiedTypeName.Substring (assemblyDelimiterIndex.GetValueOrDefault () + 1,
                                                                 fullyQualifiedTypeName.Length -
                                                                 assemblyDelimiterIndex.GetValueOrDefault () - 1).Trim () ;
            }
            else
            {
                typeName = fullyQualifiedTypeName ;
                assemblyName = null ;
            }
        }

        private static int? GetAssemblyDelimiterIndex ([NotNull] string fullyQualifiedTypeName)
        {
            var scope = 0 ;
            for (var i = 0; i < fullyQualifiedTypeName.Length; i++)
            {
                char current = fullyQualifiedTypeName[i] ;
                switch (current)
                {
                    case '[':
                        scope++ ;
                        break ;
                    case ']':
                        scope-- ;
                        break ;
                    case ',':
                        if (scope == 0)
                            return i ;

                        break ;
                }
            }

            return null ;
        }
    }
}
