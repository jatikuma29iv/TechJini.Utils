using System;
using System.Reflection;

namespace Techjini.Utils
{
    public static class MethodUtility
    {
        /// <summary>
        /// Gets the name of the parameter at the specified position in the given method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="paramIndex">The zeo-based index of the parameter.</param>
        /// <returns>The name of the parameter at the specified position.</returns>
        /// <exception cref="ArgumentNullException">Thrown when method is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when paramIndex is out of range.</exception>
        public static string GetParameterName(MethodBase method, uint paramIndex)
        {
            if (method == null)
                throw new ArgumentNullException(GetParameterName(MethodBase.GetCurrentMethod(), 0));

            var parameters = method.GetParameters();

            if (paramIndex >= parameters.Length)
                throw new ArgumentOutOfRangeException(GetParameterName(MethodBase.GetCurrentMethod(), 1));

            return parameters[paramIndex].Name;
        }
    }
}
