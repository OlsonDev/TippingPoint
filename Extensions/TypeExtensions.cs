using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TippingPoint.Extensions {
  public static class TypeExtensions {
		private static readonly IDictionary<Type, string> TypeToFriendlyName = new Dictionary<Type, string> {
				{ typeof(string), "string" },
				{ typeof(object), "object" },
				{ typeof(bool), "bool" },
				{ typeof(byte), "byte" },
				{ typeof(char), "char" },
				{ typeof(decimal), "decimal" },
				{ typeof(double), "double" },
				{ typeof(short), "short" },
				{ typeof(int), "int" },
				{ typeof(long), "long" },
				{ typeof(sbyte), "sbyte" },
				{ typeof(float), "float" },
				{ typeof(ushort), "ushort" },
				{ typeof(uint), "uint" },
				{ typeof(ulong), "ulong" },
				{ typeof(void), "void" }
		};

		public static string GetFriendlyName(this Type type) {
			if (TypeToFriendlyName.TryGetValue(type, out var name)) return name;
			if (type.IsArray) return type.GetElementType()!.GetFriendlyName() + "[]";
			if (!type.IsGenericType) return type.Name;
			name = type.Name;
			var backtick = name.IndexOf('`');
			if (backtick > 0) name = name.Remove(backtick);
			name += "<";
			var typeParameters = type.GetGenericArguments();
			for (var i = 0; i < typeParameters.Length; i++) {
				var typeParamName = typeParameters[i].GetFriendlyName();
				name += i == 0 ? typeParamName : $", {typeParamName}";
			}
			name += ">";
			return name;
		}

    public static bool IsAnonymousType(this Type type)
      => Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
      && type.IsGenericType
      && type.Name.Contains("AnonymousType")
      && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
      && type.Attributes.HasFlag(TypeAttributes.NotPublic);

    public static bool IsDerivedFrom<T>(this Type type)
			=> typeof(T).IsAssignableFrom(type);

		public static bool IsDerivedFromGenericType(this Type toCheck, Type genericType) {
			while (toCheck != null && toCheck != typeof(object)) {
				var current = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
				if (genericType == current) {
					return true;
				}
				toCheck = toCheck.BaseType!;
			}
			return false;
		}

		public static IEnumerable<Type> GetAllDerivedTypesInSameAssembly(this Type baseType)
			=> baseType.IsGenericType
				? baseType.Assembly.GetTypes().Where(t => t != baseType && (t.IsDerivedFromGenericType(baseType) || baseType.IsAssignableFrom(t)))
				: baseType.Assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));
	}
}