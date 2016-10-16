using System;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Techjini.Utils
{
    public static class CommonUtility
    {
        /// <summary>
        /// Converts the given path to server path.
        /// </summary>
        /// <param name="localPath">The local path.</param>
        /// <returns></returns>
        public static string ConvertToServerPath(string localPath, string serverBasePath)
        {
            if (!string.IsNullOrWhiteSpace(localPath))
            {
                string rootPath = HttpContext.Current.Server.MapPath("~");
                localPath = localPath.Replace('\\', '/').Replace("//", "/");

                return localPath.Replace(rootPath.Replace("\\", "/"), serverBasePath + "/").Replace('\\', '/');
            }

            return string.Empty;
        }

        /// <summary>
        /// Replaces the specified keyword in a string with new keyword.
        /// </summary>
        /// <param name="actualString">The actual string.</param>
        /// <param name="keywordToReplace">The keyword to replace.</param>
        /// <param name="keywordAfterReplacement">The keyword after replacement.</param>
        /// <returns></returns>
        public static string ReplaceKeywordInString(string actualString, string keywordToReplace, string keywordAfterReplacement)
        {
            keywordToReplace = keywordToReplace.ToLower();
            keywordAfterReplacement = keywordAfterReplacement.ToLower();

            string replacedString = actualString.Replace(keywordToReplace, keywordAfterReplacement) //replacement of all lower case letters
                                                .Replace(FirstLetterToUpper(keywordToReplace), FirstLetterToUpper(keywordAfterReplacement)) //replacement of First letter in upper case
                                                .Replace(keywordToReplace.ToUpper(), keywordAfterReplacement.ToUpper()); //replacement of all upper case letters

            return replacedString;
        }

        /// <summary>
        /// Firsts the letter to upper.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static string FirstLetterToUpper(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            return str.First().ToString().ToUpper() + str.Substring(1);
        }

        /// <summary>
        /// Converts to a similar type object.
        /// </summary>
        /// <typeparam name="TReturnType">The type of the return type.</typeparam>
        /// <param name="initialObject">The initial object.</param>
        /// <returns>The converted object</returns>
        public static TReturnType ConvertToSimilarTypeObject<TReturnType>(object initialObject)
        {
            TReturnType resultantObject = default(TReturnType);

            try
            {
                string jsonString = JsonUtility.SerializeObjectToJson(initialObject);
                resultantObject = JsonUtility.DeserializeJsonToObject<TReturnType>(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return resultantObject;
        }

        /// <summary>
        /// Converts the null for string type members to empty string.
        /// </summary>
        /// <param name="initialObject">The initial object.</param>
        public static void ConvertNullToEmptyString(object initialObject)
        {
            if (initialObject != null)
            {
                if (initialObject.GetType().IsArray)
                {
                    foreach (object item in (initialObject as System.Collections.IEnumerable))
                    {
                        ConvertNullToEmptyString(item);
                    }
                }
                else
                {
                    foreach (PropertyInfo propertyInfo in initialObject.GetType().GetProperties())
                    {
                        if (propertyInfo.CanRead && propertyInfo.CanWrite)
                        {
                            if (propertyInfo.PropertyType.Name == typeof(string).Name)
                            {
                                if (propertyInfo.GetValue(initialObject) == null)
                                {
                                    propertyInfo.SetValue(initialObject, string.Empty);
                                }
                            }
                            else if (!propertyInfo.PropertyType.IsPrimitive && propertyInfo.PropertyType.IsClass)
                            {
                                object newPropertyValues = propertyInfo.GetValue(initialObject);
                                ConvertNullToEmptyString(newPropertyValues);
                            }
                        }
                    }
                }
            }
        }
    }
}
