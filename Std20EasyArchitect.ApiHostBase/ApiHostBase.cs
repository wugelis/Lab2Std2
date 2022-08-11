using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Std20EasyArchitect.ApiHostBase
{
    /// <summary>
    /// ApiHostBase for .NET Standard 2.0
    /// </summary>
    [Route("api/[controller]/{dllName}/{nameSpace}/{className}/{methodName}/{*pathInfo}")]
    [Route("api/[controller]/{dllName}/{nameSpace}/{methodName}/{*pathInfo}")]
    [Route("api/[controller]/{dllName}/{methodName}/{*pathInfo}")]
    [Route("api/[controller]/{methodName}/{*pathInfo}")]
    [Route("api/[controller]/{*pathInfo}")]
    [ApiController]
    public class ApiHostBase: ControllerBase
    {
        /// <summary>
        /// 實作 Get 方法.
        /// </summary>
        /// <param name="dllName"></param>
        /// <param name="nameSpace"></param>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public ActionResult<object> Get(string dllName, string nameSpace, string className, string methodName)
        {
            if(string.IsNullOrEmpty(dllName) ||
                string.IsNullOrEmpty(nameSpace) ||
                string.IsNullOrEmpty(className) ||
                string.IsNullOrEmpty(methodName))
            {
                return GetJSONMessage("使用的 Url 呼叫方式有誤，請確認！");
            }

            object result = null;
            Assembly targetAsm = null;

            try
            {
                targetAsm = Assembly.Load($"{dllName}, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            }
            catch
            {
                try
                {
                    targetAsm = Assembly.LoadFrom(Path.Combine(Directory.GetCurrentDirectory(), $"bin\\Debug\\netcoreapp2.1\\{dllName}.dll"));
                }
                catch(Exception ex)
                {
                    return GetJSONMessage($"讀取 DLLs 檔案：{dllName} 發生錯誤, 錯誤訊息：{ex.Message}");
                }
            }
            

            if(targetAsm == null)
            {
                return GetJSONMessage($"在 bin 底下找不到名稱為 {dllName} 的 DLL！");
            }

            Type targetObjType = targetAsm.GetType($"{nameSpace}.{className}");
            if(targetObjType == null)
            {
                return GetJSONMessage("使用的 Url 找不到可呼叫的 Web API！請確認！");
            }

            object targetObjIns = Activator.CreateInstance(targetObjType);
            var targetMethod = targetObjType.GetMethods(BindingFlags.Public | BindingFlags.Default | BindingFlags.Instance)
                .Where(c => c.Name.ToLower() == methodName.ToLower())
                .FirstOrDefault();

            if(targetMethod == null)
            {
                return GetJSONMessage($"在 DLL {dllName} 裡找不到名稱為 {methodName} 的方法！");
            }

            var queryString = Request.Query;
            if(queryString.Count() > 0)
            {
                ParameterInfo[] parames = targetMethod.GetParameters();
                if(parames.Length > 0)
                {
                    ParameterInfo param01 = parames[0];
                    Type param01Type = param01.ParameterType;
                    object param01Ins = Activator.CreateInstance(param01Type);

                    foreach (var q in queryString.Keys)
                    {
                        string keyName = q;
                        string keyValue = queryString[q].ToString();

                        var targetProperty = (from pm in param01Type.GetProperties(
                                                BindingFlags.Public | 
                                                BindingFlags.Instance | 
                                                BindingFlags.Default)
                                          where pm.Name.ToLower() == keyName.ToLower()
                                          select pm).FirstOrDefault();

                        if(targetProperty != null)
                        {
                            switch(targetProperty.PropertyType.FullName)
                            {
                                case "System.Int32":
                                    targetProperty.SetValue(param01Ins, int.Parse(keyValue));
                                    break;
                                case "System.String":
                                    targetProperty.SetValue(param01Ins, keyValue);
                                    break;
                                case "System.Decimal":
                                    targetProperty.SetValue(param01Ins, decimal.Parse(keyValue));
                                    break;
                                case "System.Int64":
                                    targetProperty.SetValue(param01Ins, long.Parse(keyValue));
                                    break;
                                case "System.Int16":
                                    targetProperty.SetValue(param01Ins, short.Parse(keyValue));
                                    break;
                                case "System.DataTime":
                                    targetProperty.SetValue(param01Ins, DateTime.Parse(keyValue));
                                    break;
                                case "System.Double":
                                    targetProperty.SetValue(param01Ins, double.Parse(keyValue));
                                    break;
                                default:
                                    if(targetProperty.PropertyType == typeof(Nullable<DateTime>))
                                    {
                                        targetProperty.SetValue(param01Ins, DateTime.Parse(keyValue));
                                    }
                                    break;
                            }
                            
                        }
                    }
                    result = targetMethod.Invoke(targetObjIns, new object[] { param01Ins });
                }
            }
            else
            {
                result = targetMethod.Invoke(targetObjIns, null);
            }

            return result;
        }

        private static ActionResult<object> GetJSONMessage(string message)
        {
            return new string[] { message }
                            .Select(c => new
                            {
                                Err = c
                            }).ToList();
        }
    }
}
