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
            return Let(
                CreateDescriptionFromExpression(expression),
                expression.Compile()
            );
        }

        public static void Flush()
        {
            _objects.Clear();
        }

        private static string CreateDescriptionFromExpression<T>(Expression<Func<T>> expression)
        {
            var description = expression.Body.ToString();
            var genericArguments = expression.ReturnType.GenericTypeArguments;

            if (genericArguments.Length > 0)
                description += string.Join(",", genericArguments.Select(t => t.Name));

            return description;
        }
    }
}
