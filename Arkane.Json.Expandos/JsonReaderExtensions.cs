#region header

// Arkane.Json.Expandos - JsonReaderExtensions.cs
// 
// Alistair J. R. Young
// Arkane Systems
// 
// Copyright Arkane Systems 2012-2017.  All rights reserved.
// 
// Created: 2017-08-17 5:20 PM

#endregion

#region using

using System ;

using JetBrains.Annotations ;

using Newtonsoft.Json ;

#endregion

namespace ArkaneSystems.Json.Expandos
{
    internal static class JsonReaderExtensions
    {
        // Adapted from internal bool JsonReader.MoveToContent()
        // https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/JsonReader.cs#L1145
        public static bool MoveToContent ([NotNull] this JsonReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException () ;

            JsonToken t = reader.TokenType ;
            while ((t == JsonToken.None) || (t == JsonToken.Comment))
            {
                if (!reader.Read ())
                    return false ;

                t = reader.TokenType ;
            }

            return true ;
        }
    }
}
