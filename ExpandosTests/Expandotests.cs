#region header

// ExpandosTests - ExpandoTests.cs
// 
// Alistair J. R. Young
// Arkane Systems
// 
// Copyright Arkane Systems 2012-2017.  All rights reserved.
// 
// Created: 2017-08-21 11:18 AM

#endregion

#region using

using System.Collections.Generic ;
using System.Dynamic ;
using System.Linq ;
using System.Runtime.Serialization.Formatters ;

using ArkaneSystems.Json.Expandos ;

using Microsoft.VisualStudio.TestTools.UnitTesting ;

using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;

#endregion

namespace ExpandosTests
{
    [TestClass]
    public class ExpandoTests
    {
        private static ExpandoObject CreateExpandoObject ()
        {
            var model1 = new Model1 {A = 0.001m} ;
            var model2 = new Model2 {C = "something"} ;
            var model3 = new Model3
                         {
                             Items = new List <Dictionary <string, Model2>>
                                     {
                                         new Dictionary <string, Model2>
                                         {
                                             {"A Model2", new Model2 {C = "something else"}}
                                         }
                                     }
                         } ;

            var model4 = new Model4
                         {
                             Items = new List <Dictionary <string, Model3>>
                                     {
                                         new Dictionary <string, Model3>
                                         {
                                             {"A Model3", model3}
                                         }
                                     }
                         } ;

            IDictionary <string, object> expando = new ExpandoObject () ;
            expando["model1"] = model1 ;
            expando["model2"] = model2 ;
            expando["model4"] = new[] {model4}.ToList () ;

            IDictionary <string, object> outerExpando = new ExpandoObject () ;
            outerExpando["inner"] = expando ;

            return (ExpandoObject) outerExpando ;
        }

        private static void Test (TypeNameHandling typeNameHandling)
        {
            ExpandoObject expando = ExpandoTests.CreateExpandoObject () ;
            var settings = new JsonSerializerSettings
                           {
                               Formatting = Formatting.Indented,
                               TypeNameHandling = typeNameHandling,
                               TypeNameAssemblyFormat = FormatterAssemblyStyle.Full, // Needed on https://dotnetfiddle.net
                               Converters = new[] {new TypeSafeExpandoObjectConverter ()}
                           } ;
            string json = JsonConvert.SerializeObject (expando, settings) ;

            //ConsoleAndDebug.WriteLine(json);

            var expando2 = JsonConvert.DeserializeObject <ExpandoObject> (json, settings) ;

            string json2 = JsonConvert.SerializeObject (expando2, settings) ;

            //Debug.WriteLine(json2);

            Assert.IsTrue (new ExpandoObjectComparer ().Equals (expando, expando2)) ;

            Assert.IsTrue (JToken.DeepEquals (JToken.Parse (json), JToken.Parse (json2))) ;

            // ConsoleAndDebug.WriteLine("Original and round-tripped ExpandoObjects are identical and have identically typed properties.");
        }

        [TestMethod]
        public void TestObjects ()
        {
            ExpandoTests.Test (TypeNameHandling.Objects) ;
        }

        [TestMethod]
        public void TestAll ()
        {
            ExpandoTests.Test (TypeNameHandling.All) ;
        }

        [TestMethod]
        public void TestAuto ()
        {
            ExpandoTests.Test (TypeNameHandling.Auto) ;
        }
    }
}
