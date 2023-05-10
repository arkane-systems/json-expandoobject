#region header

// Arkane.Json.Expandos - TypeSafeExpandoObjectConverter.cs
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
using System.Collections.Generic ;
using System.Dynamic ;

using JetBrains.Annotations ;

using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;

#endregion

namespace ArkaneSystems.Json.Expandos
{
    /// <summary>
    ///     Converts an ExpandoObject to and from JSON in a type-safe manner.
    /// </summary>
    /// <remarks>
    ///     Adapted from
    ///     https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Converters/ExpandoObjectConverter.cs
    ///     License: https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md
    /// </remarks>
    [PublicAPI]
    public class TypeSafeExpandoObjectConverter : JsonConverter
    {
        private object ReadValue ([NotNull] JsonReader reader, JsonSerializer serializer)
        {
            if (!reader.MoveToContent ())
                throw reader.Create ("Unexpected end when reading ExpandoObject.") ;

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return this.ReadObject (reader, serializer) ;
                case JsonToken.StartArray:
                    return this.ReadList (reader, serializer) ;
                default:
                    if (reader.TokenType.IsPrimitiveToken ())
                        return reader.Value ;

                    throw reader.Create ($"Unexpected token when converting ExpandoObject: {reader.TokenType}") ;
            }
        }

        [NotNull]
        private object ReadList ([NotNull] JsonReader reader, JsonSerializer serializer)
        {
            IList <object> list = new List <object> () ;

            while (reader.Read ())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Comment:
                        break ;
                    default:
                        var v = this.ReadValue (reader, serializer) ;

                        list.Add (v) ;
                        break ;
                    case JsonToken.EndArray:
                        return list ;
                }
            }

            throw reader.Create ("Unexpected end when reading ExpandoObject.") ;
        }

        private object ReadObject (JsonReader reader, [NotNull] JsonSerializer serializer)
        {
            if (serializer.TypeNameHandling != TypeNameHandling.None)
            {
                var obj = JObject.Load (reader) ;

                Type polymorphicType       = null ;
                var  polymorphicTypeString = (string) obj["$type"] ;
                if (polymorphicTypeString != null)
                {
                    if (serializer.TypeNameHandling != TypeNameHandling.None)
                    {
                        ReflectionUtils.SplitFullyQualifiedTypeName (polymorphicTypeString,
                                                                     out var typeName,
                                                                     out var assemblyName) ;
                        polymorphicType = serializer.SerializationBinder.BindToType (assemblyName, typeName) ;
                    }

                    obj.Remove ("$type") ;
                }

                if ((polymorphicType == null) || (polymorphicType == typeof (ExpandoObject)))
                    using (var subReader = obj.CreateReader ())
                        return this.ReadExpandoObject (subReader, serializer) ;
                else
                    using (var subReader = obj.CreateReader ())
                        return serializer.Deserialize (subReader, polymorphicType) ;
            }

            return this.ReadExpandoObject (reader, serializer) ;
        }

        private object ReadExpandoObject ([NotNull] JsonReader reader, JsonSerializer serializer)
        {
            IDictionary <string, object> expandoObject = new ExpandoObject () ;

            while (reader.Read ())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        var propertyName = reader.Value.ToString () ;

                        if (!reader.Read ())
                            throw reader.Create ("Unexpected end when reading ExpandoObject.") ;

                        var v = this.ReadValue (reader, serializer) ;

                        expandoObject[propertyName] = v ;
                        break ;
                    case JsonToken.Comment:
                        break ;
                    case JsonToken.EndObject:
                        return expandoObject ;
                }
            }

            throw reader.Create ("Unexpected end when reading ExpandoObject.") ;
        }

        #region Overrides

        /// <inheritdoc />
        public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Can write is set to false, so this should never be called.
            throw new NotImplementedException () ;
        }

        /// <inheritdoc />
        public override object ReadJson ([NotNull] JsonReader reader,
                                         Type                 objectType,
                                         object               existingValue,
                                         JsonSerializer       serializer) =>
            this.ReadValue (reader, serializer) ;

        /// <inheritdoc />
        public override bool CanConvert (Type objectType) => objectType == typeof (ExpandoObject) ;

        /// <inheritdoc />
        public override bool CanWrite => false ;

        #endregion Overrides
    }
}
