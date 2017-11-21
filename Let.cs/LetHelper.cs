using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LetTestHelper
{
    public static class LetHelper
    {
        private static Dictionary<string, object> _objects = new Dictionary<string, object>();

        public static T Let<T>(string description, Func<T> func)
        {
            if (_objects.ContainsKey(description))
                return (T) _objects[description];

            var result = func();
            _objects.Add(description, result);
            return result;
        }

        public static T Let<T>(Expression<Func<T>> expression)
        {
            var expressionBody = expression.Body.ToString();
            var genericArguments = expression.ReturnType.GenericTypeArguments;

            if (genericArguments.Length > 0)
                expressionBody += string.Join(",", genericArguments.Select(t => t.Name));

            if (_objects.ContainsKey(expressionBody))
                return (T) _objects[expressionBody];

            var result = expression.Compile().Invoke();
            _objects.Add(expressionBody, result);
            return (T) result;
        }

        public static void Flush()
        {
            _objects.Clear();
        }

        private class Defined { }
    }
}
