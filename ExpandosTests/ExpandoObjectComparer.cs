#region header

// ExpandosTests - ExpandoObjectComparer.cs
// 
// Alistair J. R. Young
// Arkane Systems
// 
// Copyright Arkane Systems 2012-2017.  All rights reserved.
// 
// Created: 2017-08-21 3:09 PM

#endregion

#region using

using System.Collections ;
using System.Collections.Generic ;
using System.Dynamic ;
using System.Linq ;

using Newtonsoft.Json.Linq ;

#endregion

namespace ExpandosTests
{
    public class ExpandoObjectComparer : IEqualityComparer <ExpandoObject>
    {
        #region IEqualityComparer<ExpandoObject> Members

        private bool ValueComparer (object x, object y)
        {
            if (object.ReferenceEquals (x, y))
                return true ;

            if ((x == null) || (y == null))
                return false ;

            if (x is IList && y is IList)
            {
                var xList = (IList) x ;
                var yList = (IList) y ;

                if (xList.Count != yList.Count)
                    return false ;

                for (var i = 0; i < xList.Count; i++)
                {
                    if (!this.ValueComparer (xList[i], yList[i]))
                        return false ;
                }

                return true ;
            }

            if (x.GetType () != y.GetType ())
                return false ;
            if (x is ExpandoObject && y is ExpandoObject)
                return this.Equals ((ExpandoObject) x, (ExpandoObject) y) ;

            return JToken.DeepEquals (JToken.FromObject (x), JToken.FromObject (y)) ;
        }

        public bool Equals (ExpandoObject xExpando, ExpandoObject yExpando)
        {
            IDictionary <string, object> x = xExpando ;
            IDictionary <string, object> y = yExpando ;

            if (x.Count != y.Count)
                return false ;
            if (x.Keys.Except (y.Keys).Any ())
                return false ;
            if (y.Keys.Except (x.Keys).Any ())
                return false ;

            foreach (KeyValuePair <string, object> pair in x)
            {
                if (!this.ValueComparer (pair.Value, y[pair.Key]))
                    return false ;
            }

            return true ;
        }

        public int GetHashCode (ExpandoObject obj)
        {
            if (obj == null)
                return 0 ;

            IDictionary <string, object> dict = obj ;
            return dict.Count.GetHashCode () ;
        }

        #endregion
    }
}
