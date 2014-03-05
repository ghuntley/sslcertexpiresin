﻿using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace SSLCertExpiresIn.Helpers
{
    public static class ReflectionHelper
    {
        /// <summary>
        ///     usage:
        ///     var company = ReflectionHelper.GetAssemblyAttribute
        ///     <AssemblyCompanyAttribute>
        ///         (x => x.Company);
        ///         var version = ReflectionHelper.GetAssemblyAttribute
        ///         <AssemblyInformationalVersionAttribute>(x => x.InformationalVersion);
        /// </summary>
        public static string GetAssemblyAttribute<T>(Func<T, string> value) where T : Attribute
        {
            Contract.Requires(value != null);

            var attribute = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
            return value.Invoke(attribute);
        }
    }
}