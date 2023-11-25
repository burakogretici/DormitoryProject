using System.Reflection.Emit;
using System.Reflection;
using System.Text;

namespace RenewalReminder.Models
{
    public static class RuntimeTypeBuilder
    {
        private static AssemblyName assemblyName = new AssemblyName() { Name = "DynamicLinqTypes" };
        private static ModuleBuilder moduleBuilder = null;
        private static Dictionary<string, Type> builtTypes = new Dictionary<string, Type>();

        static RuntimeTypeBuilder()
        {
            moduleBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(assemblyName.Name);
        }

        public static Type GetDynamicType(Dictionary<string, Type> fields)
        {
            if (fields == null)
            {
                throw new ArgumentNullException("fields");
            }
            if (!fields.Any())
            {
                throw new ArgumentOutOfRangeException("fields", "fields must have at least 1 field definition");
            }

            try
            {
                Monitor.Enter(builtTypes);
                var className = GetTypeKey(fields);

                if (builtTypes.ContainsKey(className))
                {
                    return builtTypes[className];
                }

                TypeBuilder typeBuilder = moduleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);

                foreach (var field in fields)
                {
                    DefineProperty(typeBuilder, field.Key, field.Value);
                }

                builtTypes[className] = typeBuilder.CreateTypeInfo();

                return builtTypes[className];
            }
            catch //(Exception ex)
            {
            }
            finally
            {
                Monitor.Exit(builtTypes);
            }

            return null;
        }
        public static Type GetDynamicType(IEnumerable<PropertyInfo> fields)
        {
            return GetDynamicType(fields.ToDictionary(f => f.Name, f => f.PropertyType));
        }

        private static string GetTypeKey(Dictionary<string, Type> fields)
        {
            var key = string.Empty;
            foreach (var field in fields)
            {
                key += field.Key + ";" + field.Value.Name + ";";
            }

            return "_" + Md5(key);
        }
        private static void DefineProperty(TypeBuilder typeBuilder, string name, Type type)
        {
            //build field
            FieldBuilder field = typeBuilder.DefineField("_" + name, type, FieldAttributes.Private);
            //define property
            PropertyBuilder property = typeBuilder.DefineProperty(name, PropertyAttributes.None, type, null);
            //build setter
            MethodBuilder setter = typeBuilder.DefineMethod("set_" + name, MethodAttributes.Public | MethodAttributes.Virtual, null, new Type[] { type });
            ILGenerator setterILG = setter.GetILGenerator();
            setterILG.Emit(OpCodes.Ldarg_0);
            setterILG.Emit(OpCodes.Ldarg_1);
            setterILG.Emit(OpCodes.Stfld, field);
            setterILG.Emit(OpCodes.Ret);
            property.SetSetMethod(setter);
            //build getter
            MethodBuilder getter = typeBuilder.DefineMethod("get_" + name, MethodAttributes.Public | MethodAttributes.Virtual, type, Type.EmptyTypes);
            ILGenerator getterILG = getter.GetILGenerator();
            getterILG.Emit(OpCodes.Ldarg_0);
            getterILG.Emit(OpCodes.Ldfld, field);
            getterILG.Emit(OpCodes.Ret);
            property.SetGetMethod(getter);
        }
        public static string Md5(string content)
        {
            using (var provider = System.Security.Cryptography.MD5.Create())
            {
                var data = Encoding.UTF8.GetBytes(content);
                var result = provider.ComputeHash(data);
                var sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    sb.Append(result[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
