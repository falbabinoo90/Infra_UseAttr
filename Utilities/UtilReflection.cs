using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Utilities
{
    public static class UtilReflection
    {
        public static bool IsSubclassOfRawGeneric(Type i_genericType, Type i_TypeToCheck)
        {
            // boucle jusqu'à ce qu'on ait finit de remonter la boucle
            while (i_TypeToCheck != null && i_TypeToCheck != typeof(object))
            {
                var cur = i_TypeToCheck.IsGenericType ? i_TypeToCheck.GetGenericTypeDefinition() : i_TypeToCheck;
                if (i_genericType == cur)
                {
                    return true;
                }
                i_TypeToCheck = i_TypeToCheck.BaseType;
            }
            return false;
        }

        public static string GetPropertyName<T>(Expression<System.Func<T>> expression)
        {

            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }

        // A creuser : https://www.c-sharpcorner.com/article/boosting-up-the-reflection-performance-in-c-sharp/, https://internetexception.com/post/2016/08/05/Faster-then-Reflection-Delegates
        /*
		/// <summary>
		/// Piste à tester pour de meilleures performances.
		/// </summary>
		/// <typeparam name="MyClass"></typeparam>
		/// <param name="i_PI"></param>
		/// <param name="i_Obj"></param>
		/// <returns></returns>
		public static object GetPropertyValue<MyClass>(this PropertyInfo i_PI, MyClass i_Obj)
		{
			// Façon standard
			//return i_PI.GetValue(i_Obj);

			// Façon plus performante (utilisant les délégués et évitant des tests pénalisant les performances)
			try
			{
				MethodInfo MI = i_PI.GetGetMethod(); // typeof(MI.DeclaringType)

				Type TypeFunc = typeof(Func<>);
				Type TypeFuncGeneric = TypeFunc.MakeGenericType(MI.DeclaringType, typeof(int));


				Type T = MI.DeclaringType; // typeof(Func<MyClass, int>);

				dynamic GetterDelegate = Delegate.CreateDelegate(TypeFuncGeneric, null, MI);

				return GetterDelegate(i_Obj);
			}
			catch (Exception Ex)
			{
				return i_PI.GetValue(i_Obj);
			}
		}
		*/

        public static Type LoadTypeFromAssembly(string i_Assembly, string i_strType)
        {
            Assembly Asm = Assembly.Load(i_Assembly);

            Type type = null;


            if (Asm != null)
            {
                type = Asm.GetType(i_strType, false, true); //.MakeGenericType(strCustomObjectType);

            }
            return type;
        }

        private static FieldInfo GetBackingField(PropertyInfo i_PI)
        {
            if (!i_PI.CanRead || !i_PI.GetGetMethod(nonPublic: true).IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
            {
                return null;
            }

            FieldInfo BackingFieldInfo = i_PI.DeclaringType.GetField($"<{i_PI.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (BackingFieldInfo == null)
            {
                return null;
            }

            if (!BackingFieldInfo.IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
            {
                return null;
            }

            return BackingFieldInfo;
        }

        public static FieldInfo[] GetFieldsFromCurrentTypeAndAllInheritedTypes(this Type i_Type, BindingFlags i_BindingFlags)
        {
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            FieldInfo[] FI = i_Type.GetFields(i_BindingFlags);
            if (FI != null)
            {
                fieldInfos.AddRange(FI);
            }

            Type newType = i_Type.BaseType;
            while (newType != null && FI == null)
            {
                FI = newType.GetFields(i_BindingFlags);
                if (FI != null)
                {
                    fieldInfos.AddRange(FI);
                }
            }
            return fieldInfos.ToArray();
        }

        public static FieldInfo GetFieldsFromCurrentTypeAndAllInheritedTypes(this Type i_Type, string fieldName, BindingFlags i_BindingFlags)
        {
            FieldInfo FI = i_Type.GetField(fieldName, i_BindingFlags);
            if (FI == null)
            {
                Type newType = i_Type.BaseType;
                while (newType != null && FI == null)
                {
                    FI = newType.GetField(fieldName, i_BindingFlags);
                }

            }

            return FI;
        }

        /// <summary>
        /// Copies all public, readable properties from the source object to the target.
        /// The target type does need a parameterless constructor, as no new instance needs to be created.
        /// </summary>
        /// <remarks>Only the properties of the source and target types themselves are taken into account, regardless of the actual types of the arguments.</remarks>
        /// <typeparam name="TSource">Type of the source</typeparam>
        /// <typeparam name="TTarget">Type of the target</typeparam>
        /// <param name="source">Source to copy properties from</param>
        /// <param name="target">Target to copy properties to</param>
        public static void CopyProperties<TSource, TTarget>(TSource source, TTarget target, Type SpecificAttributeType)
            where TSource : class
            where TTarget : class
        {
            PropertyCopier<TSource, TTarget>.Copy(source, target, SpecificAttributeType);
        }

        /// <summary>
        /// Static class to efficiently store the compiled delegate which can
        /// do the copying. We need a bit of work to ensure that exceptions are
        /// appropriately propagated, as the exception is generated at type initialization
        /// time, but we wish it to be thrown as an ArgumentException.
        /// Note that this type we do not have a constructor constraint on TTarget, because
        /// we only use the constructor when we use the form which creates a new instance.
        /// </summary>
        internal static class PropertyCopier<TSource, TTarget>
        {
            /// <summary>
            /// Delegate to create a new instance of the target type given an instance of the
            /// source type. This is a single delegate from an expression tree.
            /// </summary>
            private static readonly Func<TSource, TTarget> _Creator;

            /// <summary>
            /// List of properties to grab values from. The corresponding targetProperties 
            /// list contains the same properties in the target type. Unfortunately we can't
            /// use expression trees to do this, because we basically need a sequence of statements.
            /// We could build a DynamicMethod, but that's significantly more work :) Please mail
            /// me if you really need this...
            /// </summary>
            private static readonly List<PropertyInfo> _SourceProperties = new List<PropertyInfo>();
            private static readonly List<PropertyInfo> _TargetProperties = new List<PropertyInfo>();
            private static readonly Exception _InitializationException;

            internal static TTarget Copy(TSource source)
            {
                if (_InitializationException != null)
                {
                    throw _InitializationException;
                }
                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }
                return _Creator(source);
            }

            internal static void Copy(TSource source, TTarget target, Type SpecificAttributeType)
            {
                if (_InitializationException != null)
                {
                    throw _InitializationException;
                }
                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }
                for (int i = 0; i < _SourceProperties.Count; i++)
                {
                    PropertyInfo PI = _TargetProperties[i];
                    if (PI.GetCustomAttribute(SpecificAttributeType) != null)
                    {
                        continue;
                    }

                    PI.SetValue(target, _SourceProperties[i].GetValue(source, null), null);
                }

            }

            static PropertyCopier()
            {
                try
                {
                    _Creator = BuildCreator();
                    _InitializationException = null;
                }
                catch (Exception e)
                {
                    _Creator = null;
                    _InitializationException = e;
                }
            }

            private static Func<TSource, TTarget> BuildCreator()
            {
                ParameterExpression sourceParameter = Expression.Parameter(typeof(TSource), "source");
                List<MemberBinding> bindings = new List<MemberBinding>();
                foreach (PropertyInfo sourceProperty in typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!sourceProperty.CanRead)
                    {
                        continue;
                    }
                    PropertyInfo targetProperty = typeof(TTarget).GetProperty(sourceProperty.Name);

                    bool bIsPropertyToCopy = true;
                    //try
                    {
                        if (targetProperty == null)
                        {
                            throw new ArgumentException("Property " + sourceProperty.Name + " is not present and accessible in " + typeof(TTarget).FullName);
                        }
                        else
                        {
                            if (!targetProperty.CanWrite)
                            {
                                bIsPropertyToCopy = false;
                                //throw new ArgumentException("Property " + sourceProperty.Name + " is not writable in " + typeof(TTarget).FullName);
                            }
                            else
                            {
                                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                                {
                                    bIsPropertyToCopy = false;
                                    //throw new ArgumentException("Property " + sourceProperty.Name + " is static in " + typeof(TTarget).FullName);
                                }
                                else
                                {
                                    if (!targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                                    {
                                        throw new ArgumentException("Property " + sourceProperty.Name + " has an incompatible type in " + typeof(TTarget).FullName);
                                    }
                                }
                            }
                        }
                    }
                    //catch (Exception Ex) { }

                    if (bIsPropertyToCopy)
                    {
                        bindings.Add(Expression.Bind(targetProperty, Expression.Property(sourceParameter, sourceProperty)));
                        _SourceProperties.Add(sourceProperty);
                        _TargetProperties.Add(targetProperty);
                    }
                }

                NewExpression NE = Expression.New(typeof(TTarget)); // => Constructeur vide requis !
                MemberInitExpression initializer = Expression.MemberInit(NE, bindings); // Represents an expression that creates a new object and initializes a property of the object.             

                return Expression.Lambda<Func<TSource, TTarget>>(initializer, sourceParameter).Compile();
            }
        }

        // TODO : CopyFields (fonctionne avec les champs privés)
        // var prop = s.GetType().GetField("id", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        // prop.SetValue(s,"new value");

        /// <summary>
        /// Attention :  ne copie que les propriétés disposant d'un Get et d'un Set.
        /// Donc cela ne copie pas les listes "SpeosChild" & "SpeosSelection".
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="SpecificAttributeType"></param>
        public static void CopyGetSetPropertiesWithLinq(object source, object destination, Type SpecificAttributeType)
        {
            // If any this null throw an exception
            if (source == null || destination == null)
            {
                throw new Exception("Source and Destination objects must not be 'null'");
            }

            // Getting the Types of the objects
            Type typeDest = destination.GetType();
            Type typeSrc = source.GetType();
            try
            {
                var SourceAndTargetPropertyCouples = from SourceProperties in typeSrc.GetProperties()
                                                     let TargetPI = typeDest.GetProperty(SourceProperties.Name)
                                                     where SourceProperties.CanRead
                                                     && TargetPI != null
                                                     && TargetPI.GetCustomAttribute(SpecificAttributeType) != null
                                                     && TargetPI.GetSetMethod(true) != null && !TargetPI.GetSetMethod(true).IsPrivate
                                                     && TargetPI.GetSetMethod() != null && (TargetPI.GetSetMethod().Attributes & MethodAttributes.Static) == 0
                                                     && TargetPI.PropertyType.IsAssignableFrom(SourceProperties.PropertyType)
                                                     select new { SourceProperty = SourceProperties, TargetProperty = TargetPI };

                // On liste les propriétés SPEOS disposant d'un Get ET d'un Set


                // Apply property values
                foreach (var MatchingCouple in SourceAndTargetPropertyCouples)
                {
                    PropertyInfo PI = MatchingCouple.TargetProperty;
                    object ValueToCopy = MatchingCouple.SourceProperty.GetValue(source, null);
                    if (ValueToCopy != null)
                    {
                        PI.SetValue(destination, ValueToCopy, null);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// TODO : à poursuivre pour la copie des sélections et sous-features récursivement (BUG-31707 et BUG-30726)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="SpecificAttributeType"></param>
        public static void CopyGetOnlyListPropertiesWithLinq(object source, object destination, Type SpecificAttributeType)
        {
            // If any this null throw an exception
            if (source == null || destination == null)
            {
                throw new Exception("Source and Destination objects must not be 'null'");
            }

            // Getting the Types of the objects
            Type typeDest = destination.GetType();
            Type typeSrc = source.GetType();

            // On liste les propriétés SPEOS disposant d'un Get ET d'un Set
            var SourceAndTargetPropertyCouples = from SourceProperties in typeSrc.GetProperties()
                                                 let TargetPI = typeDest.GetProperty(SourceProperties.Name)
                                                 where SourceProperties.CanRead
                                                 && TargetPI != null
                                                 && TargetPI.GetCustomAttribute(SpecificAttributeType) != null
                                                 && TargetPI.GetSetMethod() == null // <= propriété n'ayant pas de 'Set'
                                                 && TargetPI.PropertyType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>)) // <= uniquement les 'IList'
                                                 && TargetPI.PropertyType.IsAssignableFrom(SourceProperties.PropertyType)
                                                 select new { SourceProperty = SourceProperties, TargetProperty = TargetPI };

            // Apply property values
            foreach (var MatchingCouple in SourceAndTargetPropertyCouples)
            {
                PropertyInfo PI = MatchingCouple.TargetProperty;

                Type ListItemType = null;
                Type PT = PI.PropertyType;
                foreach (Type interfaceType in PT.GetInterfaces())
                {
                    if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                    {
                        ListItemType = PT.GetGenericArguments()[0];
                        break;
                    }
                }
                //if (PT.IsGenericType && PT.GetGenericTypeDefinition()== typeof(List<>))
                //{
                //	ListItemType = PT.GetGenericArguments()[0];
                //}

                /*
				if (ListItemType != null)
				{

				}

				object SourceListToCopy = MatchingCouple.SourceProperty.GetValue(source, null);
				object TargetListToFill = MatchingCouple.TargetProperty.GetValue(destination, null);

				// CopyTo...

				IEnumerable SourceEnumerable = SourceListToCopy as IEnumerable;
				IEnumerable TargetEnumerable = TargetListToFill as IEnumerable;

				if (SourceEnumerable != null && TargetEnumerable != null)
				{

				}
				*/
            }


        }

        public static IList<T> CloneListOfCloneable<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        //There's ICloneable and object.MemberwiseClone (shallow copy) (these create a whole new object, so might not meet your requirements).
        // ICloneable is the interface that was designed to support this scenario when you want to copy an object but it is rarely implemented because it does not make it clear whether you're creating a shallow or deep copy.

        // Shallow copies duplicate as little as possible. A shallow copy of a collection is a copy of the collection structure, not the elements. With a shallow copy, two collections now share the individual elements.
        // Deep copies duplicate everything. A deep copy of a collection is two collections with all of the elements in the original collection duplicated.

        //public static T DeepClone<T>(this T objectToClone) where T : BaseClass
        //{
        //    BinaryFormatter bFormatter = new BinaryFormatter();
        //    MemoryStream stream = new MemoryStream();
        //    bFormatter.Serialize(stream, objectToClone);
        //    stream.Seek(0, SeekOrigin.Begin);
        //    T clonedObject = (T)bFormatter.Deserialize(stream);
        //    return clonedObject;
        //}

        /// <summary>
        /// Accès par réflexion à la méthode PROTECTED MemberwiseClone
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T MemberwiseClone<T>(this T obj)
        {
            MethodInfo inst = obj.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return (T)inst?.Invoke(obj, null);
        }

        public static bool IsNullableType(this Type type)
        {
            bool IsNullable = false;
            {
                if (!type.IsValueType)
                {
                    IsNullable = true; // ref-type
                }
                else
                {
                    if (Nullable.GetUnderlyingType(type) != null)
                    {
                        IsNullable = true; // Nullable<T>
                    }
                }
            }

            return IsNullable;
        }

        public static bool IsAssignableToAnyOf(this Type typeOperand, IEnumerable<Type> types)
        {
            return types.Any(type => type.IsAssignableFrom(typeOperand));
        }

        #region Comparaison

        public static List<PropertyInfo> GetNotEqualsProperties(object Obj1, object Obj2)
        {
            List<PropertyInfo> differences = new List<PropertyInfo>();

            foreach (PropertyInfo property in Obj1.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object value1 = property.GetValue(Obj1, null);
                object value2 = property.GetValue(Obj2, null);
                if (value1 != null && value2 != null)
                {
                    if (!value1.Equals(value2))
                    {
                        differences.Add(property);
                    }
                }
            }
            /*yield*/
            return differences;
        }

        #endregion
    }
}
