using System;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.Dynamic ;
using System.Linq ;
using System.Runtime.Serialization.Formatters ;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpandosTests
{
    [TestClass]
    public class ExpandoTests
    {
        static ExpandoObject CreateExpandoObject()
        {
            var model1 = new Model1 { A = 0.001m };
            var model2 = new Model2 { C = "something" };
            var model3 = new Model3
                         {
                             Items = new List<Dictionary<string, Model2>>
                                     {
                                         new Dictionary<string, Model2>
                                         {
                                             { "A Model2", new Model2 { C = "something else" } },
                                         }
                                     },
                         };

            var model4 = new Model4
                         {
                             Items = new List<Dictionary<string, Model3>>
                                     {
                                         new Dictionary<string, Model3>
                                         {
                                             { "A Model3", model3 },
                                         }
                                     },
                         };

            IDictionary<string, object> expando = new ExpandoObject();
            expando["model1"] = model1;
            expando["model2"] = model2;
            expando["model4"] = new[] { model4 }.ToList();

            IDictionary<string, object> outerExpando = new ExpandoObject();
            outerExpando["inner"] = expando;

            return (ExpandoObject)outerExpando;
        }

        static void Test(TypeNameHandling typeNameHandling)
        {
            var expando = CreateExpandoObject();
            var settings = new JsonSerializerSettings
                           {
                               Formatting = Newtonsoft.Json.Formatting.Indented,
                               TypeNameHandling = typeNameHandling,
                               TypeNameAssemblyFormat = FormatterAssemblyStyle.Full, // Needed on https://dotnetfiddle.net
                               Converters = new[] { new TypeNameHandlingExpandoObjectConverter() },
                           };
            var json = JsonConvert.SerializeObject(expando, settings);
            ConsoleAndDebug.WriteLine(json);

            var expando2 = JsonConvert.DeserializeObject<ExpandoObject>(json, settings);

            var json2 = JsonConvert.SerializeObject(expando2, settings);

            Debug.WriteLine(json2);

            if (!new ExpandoObjectComparer().Equals(expando, expando2))
            {
                throw new InvalidOperationException("!new ExpandoObjectComparer().Equals(expando, expando2)");
            }

            if (!JToken.DeepEquals(JToken.Parse(json), JToken.Parse(json2)))
            {
                throw new InvalidOperationException("!JToken.DeepEquals(JToken.Parse(json), JToken.Parse(json2))");
            }

            ConsoleAndDebug.WriteLine("Original and round-tripped ExpandoObjects are identical and have identically typed properties.");
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
