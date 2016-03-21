using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

/// <summary>
/// Summary description for ReflectionHelper
/// </summary>
public static class ReflectionHelper
{
    //...
    // here are methods described in the post
    // http://dotnetfollower.com/wordpress/2012/12/c-how-to-set-or-get-value-of-a-private-or-internal-property-through-the-reflection/
    //...

    private static FieldInfo GetFieldInfo(Type type, string fieldName)
    {
        FieldInfo fieldInfo;
        do
        {
            fieldInfo = type.GetField(fieldName,
                   BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            type = type.BaseType;
        }
        while (fieldInfo == null && type != null);
        return fieldInfo;
    }

    public static object GetFieldValue(this object obj, string fieldName)
    {
        if (obj == null)
            throw new ArgumentNullException("obj");
        Type objType = obj.GetType();
        FieldInfo fieldInfo = GetFieldInfo(objType, fieldName);
        if (fieldInfo == null)
            throw new ArgumentOutOfRangeException("fieldName",
              string.Format("Couldn't find field {0} in type {1}", fieldName, objType.FullName));
        return fieldInfo.GetValue(obj);
    }

    public static void SetFieldValue(this object obj, string fieldName, object val)
    {
        if (obj == null)
            throw new ArgumentNullException("obj");
        Type objType = obj.GetType();
        FieldInfo fieldInfo = GetFieldInfo(objType, fieldName);
        if (fieldInfo == null)
            throw new ArgumentOutOfRangeException("fieldName",
              string.Format("Couldn't find field {0} in type {1}", fieldName, objType.FullName));
        fieldInfo.SetValue(obj, val);
    }

    // Added Versions from Static - MRD
    private static FieldInfo GetStaticFieldInfo(Type type, string fieldName)
    {
        FieldInfo fieldInfo;
        do
        {
            fieldInfo = type.GetField(fieldName,
                   BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            type = type.BaseType;
        }
        while (fieldInfo == null && type != null);
        return fieldInfo;
    }
    public static object GetStaticFieldValue(this Type type, string fieldName)
    {
        FieldInfo fieldInfo = GetStaticFieldInfo(type, fieldName);
        if (fieldInfo == null)
            throw new ArgumentOutOfRangeException("fieldName",
              string.Format("Couldn't find field {0} in type {1}", fieldName, type.FullName));
        return fieldInfo.GetValue(null);
    }

    public static void SetStaticFieldValue(this Type type, string fieldName, object val)
    {
        FieldInfo fieldInfo = GetStaticFieldInfo(type, fieldName);
        if (fieldInfo == null)
            throw new ArgumentOutOfRangeException("fieldName",
              string.Format("Couldn't find field {0} in type {1}", fieldName, type.FullName));
        fieldInfo.SetValue(null, val);
    }
}