#region header

// Arkane.Json.Expandos - JsonSerializationExceptionHelper.cs
// 
// Alistair J. R. Young
// Arkane Systems
// 
// Copyright Arkane Systems 2012-2023.  All rights reserved.
// 
// Created: 2023-05-10 12:40 AM

#endregion

#region using

using System ;
using System.Globalization ;

using JetBrains.Annotations ;

using Newtonsoft.Json ;

#endregion

namespace ArkaneSystems.Json.Expandos
{
    internal static class JsonSerializationExceptionHelper
    {
        [NotNull]
        public static JsonSerializationException Create ([CanBeNull] this JsonReader reader, string format, params object[] args)
        {
            // Adapted from https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/JsonPosition.cs

            var lineInfo = reader as IJsonLineInfo ;
            var path     = reader?.Path ;
            var message  = string.Format (CultureInfo.InvariantCulture, format, args) ;
            if (!message.EndsWith (Environment.NewLine, StringComparison.Ordinal))
            {
                message = message.Trim () ;
                if (!message.EndsWith (".", StringComparison.Ordinal))
                    message += "." ;
                message += " " ;
            }

            message += string.Format (CultureInfo.InvariantCulture, "Path '{0}'", path) ;
            if (lineInfo?.HasLineInfo () == true)
                message += string.Format (CultureInfo.InvariantCulture,
                                          ", line {0}, position {1}",
                                          lineInfo.LineNumber,
                                          lineInfo.LinePosition) ;
            message += "." ;

            return new JsonSerializationException (message) ;
        }
    }
}
