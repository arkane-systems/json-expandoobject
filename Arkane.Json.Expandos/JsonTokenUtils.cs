#region header

// Arkane.Json.Expandos - JsonTokenUtils.cs
// 
// Alistair J. R. Young
// Arkane Systems
// 
// Copyright Arkane Systems 2012-2017.  All rights reserved.
// 
// Created: 2017-08-17 5:19 PM

#endregion

#region using

using Newtonsoft.Json ;

#endregion

namespace ArkaneSystems.Json.Expandos
{
    internal static class JsonTokenUtils
    {
        // Adapted from https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Utilities/JsonTokenUtils.cs
        public static bool IsPrimitiveToken (this JsonToken token)
        {
            switch (token)
            {
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Undefined:
                case JsonToken.Null:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    return true ;
                default:
                    return false ;
            }
        }
    }
}
