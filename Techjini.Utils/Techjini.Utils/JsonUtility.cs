﻿using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Techjini.Utils
{
    public static class JsonUtility
    {
        /// <summary>
        /// Serializes the object to json.
        /// </summary>
        /// <param name="jsonObject">The json object.</param>
        /// <returns></returns>
        public static string SerializeObjectToJson(object jsonObject)
        {
            try
            {
                return JsonConvert.SerializeObject(jsonObject);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Deserializes the json to object.
        /// </summary>
        /// <typeparam name="TReturnType">The type of the return value.</typeparam>
        /// <param name="jsonString">The json string.</param>
        /// <returns></returns>
        public static TReturnType DeserializeJsonToObject<TReturnType>(string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<TReturnType>(jsonString);
            }
            catch
            {
                return default(TReturnType);
            }
        }

        /// <summary>
        /// Injects multiple properties to json.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <param name="propertiesToInsert">The properties to insert.</param>
        /// <returns></returns>
        public static bool InjectMultiplePropertiesToJson(string jsonString, Dictionary<string, object> propertiesToInsert)
        {
            if (!string.IsNullOrEmpty(jsonString) && propertiesToInsert != null)
            {
                string modifiedJsonString = jsonString;
                foreach (string propertyKey in propertiesToInsert.Keys)
                {
                    modifiedJsonString = InjectPropertyToJson(modifiedJsonString, propertyKey, propertiesToInsert[propertyKey]);
                }
            }

            return false;
        }

        /// <summary>
        /// Injects the specified property to json.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <param name="propertyName">Name of the property to be inserted.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns></returns>
        public static string InjectPropertyToJson(string jsonString, string propertyName, object propertyValue)
        {
            if (!string.IsNullOrEmpty(jsonString) && !string.IsNullOrEmpty(propertyName))
            {
                JToken parsedJson = JToken.Parse(jsonString);
                if (parsedJson.Type == JTokenType.Object)
                {
                    parsedJson[propertyName] = JToken.FromObject(propertyValue);

                    string modifiedJsonString = JsonConvert.SerializeObject(parsedJson);
                    return modifiedJsonString;
                }
            }

            return jsonString;
        }

        /// <summary>
        /// Inserts the object to json array.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <param name="objectValue">The object value.</param>
        /// <returns></returns>
        public static string InsertObjectToJsonArray(string jsonString, object objectValue)
        {
            if (!string.IsNullOrEmpty(jsonString) && objectValue != null)
            {
                JToken parsedJson = JToken.Parse(jsonString);
                if (parsedJson.Type == JTokenType.Array)
                {
                    JArray parsedJsonArray = parsedJson as JArray;
                    parsedJsonArray.Add(JToken.FromObject(objectValue));

                    string modifiedJsonString = JsonConvert.SerializeObject(parsedJson);
                    return modifiedJsonString;
                }
            }

            return jsonString;
        }

        /// <summary>
        /// Inserts the object to json array.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <param name="objectJsonForInsertion">The object json for insertion.</param>
        /// <returns></returns>
        public static string InsertObjectToJsonArray(string jsonString, string objectJsonForInsertion)
        {
            if (!string.IsNullOrEmpty(jsonString) && !string.IsNullOrEmpty(objectJsonForInsertion))
            {
                JToken parsedJson = JToken.Parse(jsonString);
                JToken insertionJson = JToken.Parse(objectJsonForInsertion);
                if (parsedJson.Type == JTokenType.Array && insertionJson != null)
                {
                    JArray parsedJsonArray = parsedJson as JArray;
                    parsedJsonArray.Add(insertionJson);

                    string modifiedJsonString = JsonConvert.SerializeObject(parsedJson);
                    return modifiedJsonString;
                }
            }

            return jsonString;
        }
    }
}
