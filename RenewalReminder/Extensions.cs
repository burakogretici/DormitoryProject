using System;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using KvsProject.Domain;
using KvsProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NuGet.Protocol;
using KvsProject.Services.Abstract;
using RenewalRemindr.Models;
using KvsProject.Models;
using System.Collections;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;

namespace KvsProject

{
    public static class Extensions
    {
        #region Static readonly
        private static readonly string[] _StringOperant = new string[] { "+", "-", "*", "!*", "?*" };

        private static string[] _ignoredColumns = new string[] { "Page", "PageSize" };

        private static readonly MethodInfo _AnyMethod = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
           .Where(a => a.Name == nameof(Enumerable.Any) && a.IsGenericMethod && a.GetGenericArguments().Length == 1 && a.GetParameters().Length == 2)
           .Where(a => a.GetParameters()[0].ParameterType == typeof(IEnumerable<>).MakeGenericType(a.GetGenericArguments()[0]))
           .Where(a => a.GetParameters()[1].ParameterType == typeof(Func<,>).MakeGenericType(a.GetGenericArguments()[0], typeof(bool)))
           .FirstOrDefault();
        #endregion
        public static string GetErrorMessage(this ModelStateDictionary modelState, string seperator = "<br />")
        {
            return string.Join(seperator, modelState.Values.Where(a => a.Errors.Count > 0).SelectMany(a => a.Errors.Select(b => b.ErrorMessage)));
        }

        #region Controller Extensions
        public static IActionResult TextFile(this Controller controller, Result result)
        {
            return controller.File(Encoding.UTF8.GetBytes(string.Join(",", result.Errors)), "text/plain", "error.txt");
        }
        public static IActionResult TextFile(this Controller controller, Exception ex)
        {
            return controller.File(Encoding.UTF8.GetBytes(string.Join(",", ex.Message)), "text/plain", "error.txt");
        }
        public static IActionResult ErrorJson(this Controller controller, string errorMsg)
        {
            return new Result(errorMsg).ToJson();
        }
        public static IActionResult ErrorJson(this Controller controller, ModelStateDictionary modelState)
        {
            var errors = new List<string>();
            foreach (var error in modelState.Values.Where(a => a.Errors.Count > 0))
            {
                foreach (var item in error.Errors)
                {
                    errors.Add(item.ErrorMessage);
                }
            }
            return new Result() { Errors = errors }.ToJson();
        }
        public static ActionResult SuccesJson(this Controller controller, object data = null)
        {
            return new JsonResult(new { HasError = false, Data = data });
        }
        public static ActionResult RedirectJson(this Controller controller, string url)
        {
            return new JsonResult(new { HasError = false, Redirect = url });
        }
        public static IActionResult ToJson(this Result result)
        {
            return new JsonResult(result);
        }
        public static IActionResult ToView<T>(this Result<T> result, Controller controller, string viewPath = null)
        {
            return ToView(result, controller, result.Data, viewPath);
        }
        public static IActionResult ToView(this Result result, Controller controller, object data = null, string viewPath = null)
        {
            if (data != null)
            {
                controller.ViewData.Model = data;
            }
            if (result.Errors != null && result.Errors.Any())
            {
                controller.ModelState.Clear();
                result.Errors.ForEach(a => controller.ModelState.AddModelError("", a));
            }
            return new ViewResult()
            {
                ViewName = viewPath,
                ViewData = controller.ViewData,
                TempData = controller.TempData,
            };
        }
        public static void StoreRequest(this Controller controller, GridRequest request)
        {
            if (string.IsNullOrEmpty(request.SessionKey))
            {
                return;
            }
            var userAccessor = controller.HttpContext.RequestServices.GetService<IUserAccessor>();
            if (request == null)
            {
                userAccessor.Clear("GridRequest");
            }
            else
            {
                userAccessor.Store("GridRequest", request);
            }
        }


        #endregion

        #region HttpContext Extensions
        public static T? GetSession<T>(this HttpContext httpContext, string key)
        {
            if (httpContext != null && httpContext.Session != null)
            {
                var value = httpContext.Session.GetString(key);
                return value == null ? default : JsonSerializer.Deserialize<T>(value);
            }
            else
            {
                throw new ArgumentNullException("HttpContext");
            }
        }
        public static void StoreSession<T>(this HttpContext httpContext, string key, T data)
        {
            if (httpContext != null && httpContext.Session != null)
            {
                httpContext.Session.SetString(key, JsonSerializer.Serialize(data));
            }
            else
            {
                throw new ArgumentNullException("HttpContext");
            }
        }
        #endregion

        #region String Extensions
        public static string Md5(this string content)
        {
            return Md5(content, Encoding.UTF8);
        }
        public static string Md5(this string content, Encoding encoding)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var data = encoding.GetBytes(content);
                var result = md5.ComputeHash(data);
                var sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    sb.Append(result[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public static string SHA1(this string content)
        {
            using (var sha1 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] data = Encoding.UTF8.GetBytes(content);
                byte[] result = sha1.ComputeHash(data);
                var sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    sb.Append(result[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public static string GenerateKeyword(int length, bool number = false)
        {
            string password = string.Empty;

            Random rnd = new Random();
            for (int charCounter = 0; charCounter < length; charCounter++)
            {
                int keyValue;
                if (number)
                {
                    keyValue = rnd.Next(0, 10);
                    password += keyValue.ToString();
                    continue;
                }
                keyValue = rnd.Next(65, 90);
                password += Convert.ToChar(keyValue);
            }
            return password;
        }
        #endregion

        #region Type Extensions
        public static bool IsValueType(this Type type)
        {
            return type.IsValueType || type.IsEnum || type.Equals(typeof(string)) || IsNullableType(type);
        }
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        #endregion

        #region Grid Extensions
        public static IActionResult ToGridResult<T>(this Result<PagedList<T>> result, GridRequest request)
        {
            if (result.HasError)
            {
                return result.ToJson();
            }
            return new JsonResult(new
            {
                HasError = false,
                Total = result.Data.TotalCount,
                TotalPage = result.Data.TotalPage,
                CurrentPage = result.Data.Page,
                Data = result.Data.CastAsDynamic(request.Fields)
            });
        }
        public static GridRequest AddFilters<T>(this Controller controller, GridRequest request) where T : class
        {
            var httpRequest = controller.HttpContext.RequestServices.GetService<IHttpContextAccessor>()?.HttpContext?.Request;
            if (httpRequest == null)
            {
                return request;
            }

            var propertyNames = typeof(T).GetProperties().Where(a => !_ignoredColumns.Contains(a.Name)).Select(a => a.Name).ToList();
            if (httpRequest.ContentLength > 0 && httpRequest.Form != null)
            {
                foreach (var item in httpRequest.Form)
                {
                    if (propertyNames.Any(a => string.Equals(a, item.Key, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (string.IsNullOrEmpty(item.Value.ToString()))
                        {
                            continue;
                        }

                        if (request.Filters == null)
                        {
                            request.Filters = new List<GridFilter>();
                        }
                        request.Filters.Add(new GridFilter()
                        {
                            Field = item.Key,
                            Operant = "=",
                            Value = item.Value.ToString()
                        });
                    }
                }
            }
            if (httpRequest.Query != null)
            {
                foreach (var item in httpRequest.Query)
                {
                    if (propertyNames.Any(a => string.Equals(a, item.Key, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (string.IsNullOrEmpty(item.Value.ToString()))
                        {
                            continue;
                        }

                        if (request.Filters == null)
                        {
                            request.Filters = new List<GridFilter>();
                        }
                        request.Filters.Add(new GridFilter()
                        {
                            Field = item.Key,
                            Operant = "=",
                            Value = item.Value.ToString()
                        });
                    }
                }
            }
            return request;
        }
        public static PagedQuery<T> ToPagedQuery<T>(this GridRequest request, Expression<Func<T, T>> additionalSelect = null) where T : Entity
        {
            var query = new PagedQuery<T>()
            {
                Orders = new List<Tuple<LambdaExpression, bool>>(),
                Filters = new List<Expression<Func<T, bool>>>(),
                Page = request.Page,
                PageSize = request.PageSize
            };
            if (request == null)
            {
                return query;
            }
            if (!string.IsNullOrEmpty(request.Sorting))
            {
                var sorting = request.Sorting.Split(":");
                var property = sorting.First();
                var parameterExpression = Expression.Parameter(typeof(T), "a");
                var expression = GetPropertySelector<T>(property, parameterExpression);
                if (expression != null)
                {
                    query.Orders.Add(new Tuple<LambdaExpression, bool>(Expression.Lambda(expression, parameterExpression), sorting.Last() == "ASC"));
                }
            }
            if (request.Filters != null)
            {
                foreach (var item in request.Filters)
                {
                    if (string.IsNullOrEmpty(item.Field))
                    {
                        continue;
                    }
                    var expression = GetFilterExpression<T>(item);
                    if (expression != null)
                    {
                        query.Filters.Add(expression);
                    }
                }
            }
            if (request.Fields != null && request.Fields.Any())
            {
                query.Select = GetSelectExpression<T>(request.Fields, additionalSelect);
            }
            return query;
        }

        private static Expression<Func<T, T>> GetSelectExpression<T>(List<string> fields, Expression<Func<T, T>> additionalSelect)
        {
            var arg = Expression.Parameter(typeof(T), "a");
            var newExpression = Expression.New(typeof(T));

            var bindings = new List<MemberBinding>();
            if (additionalSelect != null && additionalSelect.Body is MemberInitExpression)
            {
                arg = additionalSelect.Parameters.First();
            }

            if (fields != null && fields.Any())
            {
                bindings = GetMemberBindings<T>(typeof(T), fields, arg);
            }

            if (additionalSelect != null && additionalSelect.Body is MemberInitExpression)
            {
                var additionalInit = additionalSelect.Body as MemberInitExpression;
                foreach (var additionalBinding in additionalInit.Bindings)
                {
                    var extBinding = bindings.FirstOrDefault(a => a.Member == additionalBinding.Member);
                    if (extBinding != null)
                    {
                        bindings.Remove(extBinding);
                    }
                    bindings.Add(additionalBinding);
                }
            }

            var memberInitExpression = Expression.MemberInit(newExpression, bindings);
            return Expression.Lambda<Func<T, T>>(memberInitExpression, arg);
        }
        private static List<MemberBinding> GetMemberBindings<T>(Type returnType, List<string> fields, ParameterExpression arg)
        {
            var bindFields = new List<string>();
            var bindings = new List<MemberBinding>();
            foreach (var field in fields)
            {
                if (bindFields.Contains(field))
                {
                    continue;
                }
                if (field.Contains("."))
                {
                    var property = field.Split(".".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList().First();
                    var objPropInfo = returnType.GetProperty(property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (objPropInfo == null)
                    {
                        continue;
                    }
                    var propInfo = typeof(T).GetProperty(property + "Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo == null)
                    {
                        continue;
                    }
                    var sourcePropInfo = typeof(T).GetProperty(property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (sourcePropInfo == null)
                    {
                        continue;
                    }
                    var objFields = fields.Where(a => a.StartsWith(property + ".")).ToList();
                    var expression = GetNewExpression<T>(property, sourcePropInfo, objPropInfo, objFields, arg);
                    if (expression != null)
                    {
                        if (IsNullableType(propInfo.PropertyType))
                        {
                            var testExpression = Expression.Equal(GetPropertySelector<T>(property, arg), Expression.Constant(null));
                            bindings.Add(Expression.Bind(objPropInfo, Expression.Condition(testExpression, Expression.Default(objPropInfo.PropertyType), expression)));
                        }
                        else
                        {
                            bindings.Add(Expression.Bind(objPropInfo, expression));
                        }
                    }
                }
                else
                {
                    var propInfo = returnType.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        bindings.Add(Expression.Bind(propInfo, Expression.Property(arg, field)));
                    }
                    bindFields.Add(field);
                }
            }
            return bindings;
        }
        private static Expression GetPropertySelector<T>(string propertyName, ParameterExpression arg)
        {
            if (propertyName.Contains("."))
            {
                var properties = propertyName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                var type = typeof(T);
                Expression expression = arg;
                foreach (var property in properties)
                {
                    var pInfo = type.GetProperty(property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pInfo == null)
                    {
                        return null;
                    }
                    expression = Expression.Property(expression, pInfo);
                    type = pInfo.PropertyType;
                }
                return expression;
            }
            var propInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propInfo == null)
            {
                return null;
            }
            return Expression.Property(arg, propertyName);
        }
        private static Expression<Func<T, bool>> GetFilterExpression<T>(GridFilter filter)
        {
            var expressions = new List<Expression>();
            var arg = Expression.Parameter(typeof(T), "a");
            if (filter.Field.Contains(","))
            {
                foreach (var item in filter.Field.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var exp = GetPropertySelector<T>(item, arg);
                    if (exp != null)
                    {
                        expressions.Add(exp);
                    }
                }
            }
            else
            {
                var exp = GetPropertySelector<T>(filter.Field, arg);
                if (exp != null)
                {
                    expressions.Add(exp);
                }
            }

            if (expressions == null || !expressions.Any() || !(expressions.First() is MemberExpression))
            {
                return null;
            }

            var validExpressions = new List<Expression>();
            var values = new List<IList>();
            foreach (var expression in expressions)
            {
                if (string.IsNullOrEmpty(filter.Value))
                {
                    continue;
                }

                var propType = ((MemberExpression)expression).Type;
                bool isNullable = (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>));
                var value = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new[] { propType }));

                if (isNullable)
                {
                    if (filter.Value.Contains(","))
                    {
                        var list = filter.Value.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (list.Any())
                        {
                            foreach (var item in list)
                            {
                                if (TryChange(item, Nullable.GetUnderlyingType(propType), out object result))
                                {
                                    value.Add(result);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (TryChange(filter.Value, Nullable.GetUnderlyingType(propType), out object result))
                        {
                            value.Add(result);
                        }
                    }
                }
                else
                {
                    if (propType == typeof(string))
                    {
                        if (filter.Value.Contains(","))
                        {
                            var list = filter.Value.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            if (list.Any())
                            {
                                foreach (var item in list)
                                {
                                    value.Add(item);
                                }
                            }
                        }
                        else
                        {
                            value.Add(filter.Value);
                        }
                    }
                    else
                    {
                        if (propType.IsEnum)
                        {
                            if (filter.Value.Contains(","))
                            {
                                var list = filter.Value.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                if (list.Any())
                                {
                                    foreach (var item in list)
                                    {
                                        if (int.TryParse(item, out int intValue))
                                        {
                                            if (TryEnumToObject(intValue, propType, out object result))
                                            {
                                                value.Add(result);
                                            }
                                        }
                                        else
                                        {
                                            if (TryEnumParse(item, propType, out object result))
                                            {
                                                value.Add(result);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (int.TryParse(filter.Value, out int intValue))
                                {
                                    if (TryEnumToObject(intValue, propType, out object result))
                                    {
                                        value.Add(result);
                                    }
                                }
                                else
                                {
                                    if (TryEnumParse(filter.Value, propType, out object result))
                                    {
                                        value.Add(result);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (filter.Value.Contains(","))
                            {
                                var list = filter.Value.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                if (list.Any())
                                {
                                    foreach (var item in list)
                                    {
                                        if (TryChange(item, propType, out object result))
                                        {
                                            value.Add(result);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (TryChange(filter.Value, propType, out object result))
                                {
                                    value.Add(result);
                                }
                            }
                        }
                    }
                }

                if (value.Count > 0)
                {
                    validExpressions.Add(expression);
                    values.Add(value);
                }
            }

            if (values.Count == 0)
            {
                return null;
            }

            if (validExpressions.Count == 1)
            {
                var expression = validExpressions[0];
                var value = values[0];
                var propType = value.GetType().GenericTypeArguments.First();
                Expression predicateBody = GetFilterExpression(filter, expression, propType, value.Count == 1 ? value[0] : value);
                return Expression.Lambda<Func<T, bool>>(predicateBody, arg);
            }
            else
            {
                var i = 0;
                var expressionList = new List<Expression>();
                foreach (var expression in validExpressions)
                {
                    var value = values[i];
                    var propType = value.GetType().GenericTypeArguments.First();
                    Expression predicateBody = GetFilterExpression(filter, expression, propType, value.Count == 1 ? value[0] : value);
                    expressionList.Add(predicateBody);
                    i++;
                }

                Func<List<Expression>, Expression<Func<T, bool>>> getOrExpression = null;
                getOrExpression = (list) =>
                {
                    if (list.Count == 2)
                    {
                        return Expression.Lambda<Func<T, bool>>(Expression.Or(list[0], list[1]), arg);
                    }
                    return Expression.Lambda<Func<T, bool>>(Expression.Or(list[0], getOrExpression(list.Skip(1).ToList()).Body), arg);
                };

                return getOrExpression(expressionList);
            }

        }
        private static Expression GetFilterExpression(GridFilter filter, Expression expression, Type propType, object value)
        {
            var constant = default(Expression);
            if (value is IList)
            {
                constant = Expression.Constant(value, value.GetType());
            }
            else
            {
                constant = Expression.Constant(value, propType);
            }
            Expression predicateBody;
            var operant = filter.Operant;
            if (propType != typeof(string) && _StringOperant.Contains(operant))
            {
                operant = operant.StartsWith("!") ? "!=" : "=";
            }

            switch (operant)
            {
                case "=":
                    if (value is IList)
                    {
                        var arg = Expression.Parameter(propType, "b");
                        var inner = Expression.Lambda(Expression.Equal(expression, arg), arg);
                        predicateBody = Expression.Call(_AnyMethod.MakeGenericMethod(propType), constant, inner);
                    }
                    else
                    {
                        predicateBody = Expression.Equal(expression, constant);
                    }
                    break;
                case "!=":
                    if (value is IList)
                    {
                        var arg = Expression.Parameter(propType, "b");
                        var inner = Expression.Lambda(Expression.Not(Expression.Equal(expression, arg)), arg);
                        predicateBody = Expression.Call(_AnyMethod.MakeGenericMethod(propType), constant, inner);
                    }
                    else
                    {
                        predicateBody = Expression.Not(Expression.Equal(expression, constant));
                    }
                    break;
                case ">":
                    if (value is IList)
                    {
                        var arg = Expression.Parameter(propType, "b");
                        var inner = Expression.Lambda(Expression.GreaterThan(arg, expression), arg);
                        predicateBody = Expression.Call(_AnyMethod.MakeGenericMethod(propType), constant, inner);
                    }
                    else
                    {
                        predicateBody = Expression.GreaterThan(expression, constant);
                    }
                    break;
                case ">=":
                    if (value is IList)
                    {
                        var arg = Expression.Parameter(propType, "b");
                        var inner = Expression.Lambda(Expression.GreaterThanOrEqual(arg, expression), arg);
                        predicateBody = Expression.Call(_AnyMethod.MakeGenericMethod(propType), constant, inner);
                    }
                    else
                    {
                        predicateBody = Expression.GreaterThanOrEqual(expression, constant);
                    }
                    break;
                case "<":
                    if (value is IList)
                    {
                        var arg = Expression.Parameter(propType, "b");
                        var inner = Expression.Lambda(Expression.LessThan(arg, expression), arg);
                        predicateBody = Expression.Call(_AnyMethod.MakeGenericMethod(propType), constant, inner);
                    }
                    else
                    {
                        predicateBody = Expression.LessThan(expression, constant);
                    }
                    break;
                case "<=":
                    if (value is IList)
                    {
                        var arg = Expression.Parameter(propType, "b");
                        var inner = Expression.Lambda(Expression.LessThanOrEqual(arg, expression), arg);
                        predicateBody = Expression.Call(_AnyMethod.MakeGenericMethod(propType), constant, inner);
                    }
                    else
                    {
                        predicateBody = Expression.LessThanOrEqual(expression, constant);
                    }
                    break;
                case "+":
                    if (value is IList)
                    {
                        var arg = Expression.Parameter(propType, "b");
                        var inner = Expression.Lambda(Expression.Call(expression, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), arg), arg);
                        predicateBody = Expression.Call(_AnyMethod.MakeGenericMethod(propType), constant, inner);
                    }
                    else
                    {
                        predicateBody = Expression.Call(expression, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), constant);
                    }
                    break;
                case "-":
                    if (value is IList)
                    {
                        var arg = Expression.Parameter(propType, "b");
                        var inner = Expression.Lambda(Expression.Call(expression, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), arg), arg);
                        predicateBody = Expression.Call(_AnyMethod.MakeGenericMethod(propType), constant, inner);
                    }
                    else
                    {
                        predicateBody = Expression.Call(expression, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), constant);
                    }
                    break;
                case "!*":
                    if (value is IList)
                    {
                        var arg = Expression.Parameter(propType, "b");
                        var inner = Expression.Lambda(Expression.Not(Expression.Call(expression, typeof(string).GetMethod("Contains", new[] { typeof(string) }), arg)), arg);
                        predicateBody = Expression.Call(_AnyMethod.MakeGenericMethod(propType), constant, inner);
                    }
                    else
                    {
                        predicateBody = Expression.Not(Expression.Call(expression, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant));
                    }
                    break;
                case "*":
                case "?*":
                    if (value is IList)
                    {
                        var arg = Expression.Parameter(propType, "b");
                        var inner = Expression.Lambda(Expression.Call(expression, typeof(string).GetMethod("Contains", new[] { typeof(string) }), arg), arg);
                        predicateBody = Expression.Call(_AnyMethod.MakeGenericMethod(propType), constant, inner);
                    }
                    else
                    {
                        predicateBody = Expression.Call(expression, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant);
                    }
                    break;
                default:
                    throw new Exception("Not supported operator");
            }

            return predicateBody;
        }
        #endregion

        #region Expression Extensions
        public static MemberExpression? FindMemberExpression(this Expression expression)
        {
            if (expression is MemberExpression)
            {
                return (MemberExpression)expression;
            }
            else if (expression is UnaryExpression)
            {
                var unary = (UnaryExpression)expression;
                return FindMemberExpression(unary.Operand);
            }
            else if (expression is ConditionalExpression)
            {
                var conditional = (ConditionalExpression)expression;
                var falseResult = FindMemberExpression(conditional.IfFalse);
                if (falseResult != null)
                {
                    return falseResult;
                }
                var trueResult = FindMemberExpression(conditional.IfTrue);
                if (trueResult != null)
                {
                    return trueResult;
                }
            }
            return default;
        }
        private static Expression GetNewExpression<T>(string fullName, PropertyInfo sourceProperty, PropertyInfo property, List<string> fields, ParameterExpression arg)
        {
            var bindings = new List<MemberBinding>();
            var newExpression = Expression.New(property.PropertyType);
            foreach (var item in fields.Where(a => a.StartsWith(fullName + ".")))
            {
                var propName = item.Substring(fullName.Length + 1);
                if (propName.Contains("."))
                {
                    var objPropName = propName.Split('.').First();
                    var objPropInfo = property.PropertyType.GetProperty(objPropName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (objPropInfo != null)
                    {
                        var propInfo = sourceProperty.PropertyType.GetProperty(objPropName + "Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (propInfo == null)
                        {
                            continue;
                        }
                        var sourcePropInfo = sourceProperty.PropertyType.GetProperty(objPropName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (sourcePropInfo == null)
                        {
                            continue;
                        }

                        var objFullName = fullName + "." + objPropName;
                        var objFields = fields.Where(a => a.StartsWith(objFullName + ".")).ToList();
                        var expression = GetNewExpression<T>(objFullName, sourcePropInfo, objPropInfo, objFields, arg);
                        if (expression != null)
                        {
                            if (IsNullableType(propInfo.PropertyType))
                            {
                                var testExpression = Expression.Equal(GetPropertySelector<T>(objFullName, arg), Expression.Constant(null));
                                bindings.Add(Expression.Bind(objPropInfo, Expression.Condition(testExpression, Expression.Constant(property.PropertyType), expression)));
                            }
                            else
                            {
                                bindings.Add(Expression.Bind(objPropInfo, expression));
                            }
                        }
                    }
                }
                else
                {
                    var properyExpression = GetPropertySelector<T>(item, arg);
                    if (properyExpression != null)
                    {
                        var propInfo = property.PropertyType.GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        bindings.Add(Expression.Bind(propInfo, properyExpression));
                    }
                }
            }

            return Expression.MemberInit(newExpression, bindings);
        }
        #endregion

        #region Try Type Extensions
        private static bool TryChange(object value, Type type, out object result)
        {
            result = null;
            try
            {
                result = Convert.ChangeType(value, type);
                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }
        }
        private static bool TryEnumParse(string value, Type type, out object result)
        {
            result = null;
            try
            {
                result = Enum.Parse(type, value, true);
                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }
        }
        private static bool TryEnumToObject(int value, Type type, out object result)
        {
            result = null;
            try
            {
                result = Enum.ToObject(type, value);
                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }
        }
        #endregion

        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var displayAttribute = field.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? value.ToString();
        }

        public static IEnumerable<dynamic> CastAsDynamic<T>(this IEnumerable<T> data, List<string> fields)
        {
            if (fields == null || !fields.Any() || data == null)
            {
                return null;
            }

            var propertyTypes = new Dictionary<string, Type>();

            Func<string, PropertyInfo, Type> getType = null;
            getType = (fullName, objPropInfo) =>
            {
                var propBindings = new Dictionary<string, Type>();
                foreach (var field in fields.Where(a => a.StartsWith(fullName + ".")))
                {
                    var propName = field.Substring(fullName.Length + 1);
                    if (propName.Contains("."))
                    {
                        var objFieldName = propName.Split('.').First();
                        var propInfo = objPropInfo.PropertyType.GetProperty(objFieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (propInfo != null)
                        {
                            var objType = getType(fullName + "." + objFieldName, propInfo);
                            propBindings.Add(objFieldName, objType);
                        }
                    }
                    else
                    {
                        var propInfo = objPropInfo.PropertyType.GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (propInfo != null)
                        {
                            propBindings.Add(propName, propInfo.PropertyType);
                        }
                    }
                }
                return RuntimeTypeBuilder.GetDynamicType(propBindings);
            };

            var foundFields = new List<string>();
            foreach (var item in fields)
            {
                if (foundFields.Contains(item))
                {
                    continue;
                }
                if (item.Contains("."))
                {
                    var propName = item.Split('.').First();
                    var propInfo = typeof(T).GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        propertyTypes[propName] = getType(propName, propInfo);
                        foundFields.AddRange(fields.Where(a => a.StartsWith(item + ".")));
                    }
                }
                else
                {
                    var propInfo = typeof(T).GetProperty(item, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        propertyTypes[item] = propInfo.PropertyType;
                        foundFields.Add(item);
                    }
                }
            }

            var arg = Expression.Parameter(typeof(T), "a");
            var dataType = RuntimeTypeBuilder.GetDynamicType(propertyTypes);
            var expression = Expression.MemberInit(Expression.New(dataType), GetMemberBindings<T>(dataType, fields, arg));
            var lambda = Expression.Lambda<Func<T, dynamic>>(expression, arg);
            return data.AsQueryable().Select(lambda);
        }

        public static IActionResult ToPartialView<T>(this Result<T> result, Controller controller, string viewPath = null)
        {
            return ToPartialView(result, controller, result.Data, viewPath);
        }
        public static IActionResult ToPartialView(this Result result, Controller controller, object data = null, string viewPath = null)
        {
            if (data != null)
            {
                controller.ViewData.Model = data;
            }
            if (result.Errors != null && result.Errors.Any())
            {
                controller.ModelState.Clear();
                result.Errors.ForEach(a => controller.ModelState.AddModelError("", a));
            }
            return new PartialViewResult()
            {
                ViewName = viewPath,
                ViewData = controller.ViewData,
                TempData = controller.TempData
            };
        }
        public static string ToPartialViewString<T>(this Result<T> result, Controller controller, string viewName)
        {
            if (result.Data != null)
            {
                var enumerableData = result.Data as IEnumerable<T>;
                if (enumerableData != null)
                {
                    return controller.RenderPartialViewToString(viewName, enumerableData);
                }
                else
                {
                    return controller.RenderPartialViewToString(viewName, result.Data);
                }
            }

            return controller.RenderPartialViewToString(viewName, result.Data);
        }
        public static string RenderPartialViewToString(this Controller controller, string viewName, object model, Dictionary<string, object> extraData = null)
        {
            using (var sw = new StringWriter())
            {
                var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                var viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);
                if (viewResult.View == null)
                {
                    viewResult = viewEngine.GetView(null, viewName, false);
                    if (viewResult.View == null)
                    {
                        throw new ArgumentNullException($"{viewName} does not match any available view");
                    }
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                foreach (var item in controller.ViewData)
                {
                    viewDictionary[item.Key] = item.Value;
                }
                //controller.ViewData.Model = model;

                var viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    viewDictionary,
                    //controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                if (extraData != null)
                {
                    foreach (var item in extraData)
                    {
                        viewContext.ViewData[item.Key] = item.Value;
                    }
                }

                viewResult.View.RenderAsync(viewContext).Wait();
                return sw.ToString();
            }
        }
    }
}

