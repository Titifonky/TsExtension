using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Outils
{
    public static class Reflexion
    {
        public static String GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            var me = propertyLambda.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
            }

            return me.Member.Name;
        }

        public static String GetPropertyName<T, TReturn>(Expression<Func<T, TReturn>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }

        public static Object GetDefaultValue(this Type t)
        {
            if (t == typeof(Double))
                return 0;
            else if (t == typeof(String))
                return null;
            else if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            else
                return null;
        }

        public static String GetEnumInfo<D>(this Enum value)
            where D : PersoAttribute
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            D[] attributes = (D[])fi.GetCustomAttributes(typeof(D), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0];
            else
                return "";
        }

        public static String GetEnumInfo<D>(this Object value)
            where D : PersoAttribute
        {

            if (!value.GetType().IsEnum)
                return "";

            FieldInfo fi = value.GetType().GetField(value.ToString());

            D[] attributes = (D[])fi.GetCustomAttributes(typeof(D), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0];
            else
                return "";
        }

        public static List<String> GetListeEnumInfo<D>(this Type e)
            where D : PersoAttribute
        {
            if (!e.IsEnum)
                return null;

            List<String> a = new List<String>();

            foreach (Enum v in Enum.GetValues(e))
            {
                a.Add(v.GetEnumInfo<D>());
            }

            return a;
        }

        /// <summary>
        /// Renvoi la liste des attributs pour l'enum
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static List<String> GetListeEnumInfo<D>(this Enum e)
            where D : PersoAttribute
        {
            List<String> a = new List<String>();

            foreach (Enum v in Enum.GetValues(e.GetType()))
            {
                if(e.HasFlag(v))
                    a.Add(v.GetEnumInfo<D>());
            }

            return a;
        }

        /// <summary>
        /// Renvoi la liste des attributs pour l'enum
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static List<String> GetListeEnumInfo<D>(this Object e)
            where D : PersoAttribute
        {
            List<String> a = new List<String>();

            foreach (Enum v in Enum.GetValues(e.GetType()))
            {
                if (((Enum)e).HasFlag(v))
                    a.Add(v.GetEnumInfo<D>());
            }

            return a;
        }

        public static T GetEnumFromAtt<T,D>(this String value)
            where D : PersoAttribute
        {
            if (!typeof(T).IsEnum)
                return default(T);

            foreach (T e in Enum.GetValues(typeof(T)))
            {
                if (e.GetEnumInfo<D>() == value)
                    return e;
            }

            return default(T);
        }

        public static Boolean IsNull(this Object o)
        {
            if (o == null)
                return true;

            return false;
        }

        public static Boolean IsRef(this Object o)
        {
            if (o != null)
                return true;

            return false;
        }

        public static String IsNullToString(this Object o)
        {
            return o.IsNull().ToString();
        }

        public static String IsRefToString(this Object o)
        {
            return o.IsRef().ToString();
        }

        public static Boolean IsLastIndex<T>(this List<T> liste, int i)
        {
            if ((i + 1) < liste.Count)
                return false;

            return true;
        }

        public static Boolean IsNullOrEmpty<T>(this List<T> liste)
        {
            if (liste.IsNull())
                return true;

            if (liste.Count == 0)
                return true;

            return false;
        }

        public static Boolean TabIsRef_LgthNotNull<T>(this T[] tab)
        {
            return tab.IsRef() && (tab.Length > 0);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            return source.Where(element => knownKeys.Add(keySelector(element)));
        }
    }

    public class PersoAttribute : Attribute
    {
        private String _Val = "";

        public PersoAttribute(String val) { _Val = val; }

        public String Val { get { return _Val; } }

        public static implicit operator String(PersoAttribute att) { return att.Val; }
    }

    public class Intitule : PersoAttribute
    {
        public Intitule(string val) : base(val) { }
    }

    public class ExtFichier : PersoAttribute
    {
        public ExtFichier(string val) : base(val) { }
    }

    public class ExtGabarit : PersoAttribute
    {
        public ExtGabarit(string val) : base(val) { }
    }

    public class Unite : PersoAttribute
    {
        public Unite(string val) : base(val) { }
    }

    public class TagXml : PersoAttribute
    {
        public TagXml(string val) : base(val) { }
    }
}
