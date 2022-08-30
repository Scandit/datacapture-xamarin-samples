/*
 * This file is part of the Scandit Data Capture SDK
 *
 * Copyright (C) 2020- Scandit AG. All rights reserved.
 */

using System;
using System.Runtime.CompilerServices;

namespace BarcodeCaptureSettingsSample.Extensions
{
    /// <summary>
    /// This class provide extension methods to help verify parameters validity.
    /// </summary>
    public static class ArgumentValidation
    {
        /// <summary>
        /// Basic Validation helper to verify parameter validity.
        /// </summary>
        /// <typeparam name="T">The instance type</typeparam>
        /// <param name="obj">The parameter instance to verify</param>
        /// <param name="parameterName">The parameter name to verify</param>
        /// <returns>The instance that was passed to verify</returns>
        public static T RequireNotNull<T>(this T obj, string parameterName, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName");
            }

            if (obj == null)
            {
                throw new ArgumentNullException(parameterName, string.Format("Argument is null. CallerMemberName={0}, CallerFilePath={1}, CallerSourceLineNumber={2}", memberName, sourceFilePath, sourceLineNumber));
            }

            return obj;
        }

        /// <summary>
        /// Verify validity of parameter instance through a condition.
        /// </summary>
        /// <typeparam name="T">The instance type</typeparam>
        /// <param name="obj">The parameter instance to verify</param>
        /// <param name="condition">The condition to test for</param>
        /// <param name="parameterName">The parameter name to verify</param>
        /// <param name="message">The message to post when the parameter instance fails validation</param>
        /// <returns>The instance that was passed to verify</returns>
        public static T RequireCondition<T>(this T obj, Func<T, bool> condition, string parameterName, string message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj", string.Format("Argument is null. CallerMemberName={0}, CallerFilePath={1}, CallerSourceLineNumber={2}", memberName, sourceFilePath, sourceLineNumber));
            }

            if (condition == null)
            {
                throw new ArgumentNullException("condition", string.Format("Argument is null. CallerMemberName={0}, CallerFilePath={1}, CallerSourceLineNumber={2}", memberName, sourceFilePath, sourceLineNumber));
            }

            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName", string.Format("Argument is null. CallerMemberName={0}, CallerFilePath={1}, CallerSourceLineNumber={2}", memberName, sourceFilePath, sourceLineNumber));
            }

            if (!condition(obj))
            {
                throw new ArgumentException(message + string.Format(", CallerMemberName={0}, CallerFilePath={1}, CallerSourceLineNumber={2}", memberName, sourceFilePath, sourceLineNumber), parameterName);
            }

            return obj;
        }

        /// <summary>
        /// Vefiy string parameter to make sure it's not null or white space.
        /// </summary>
        /// <param name="obj">The parameter instance to verify</param>
        /// <param name="parameterName">The parameter name to verify</param>
        /// <returns>The instance that was passed to verify</returns>
        public static string RequireNotNullOrWhiteSpace(this string obj, string parameterName, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj", string.Format("Argument is null. CallerMemberName={0}, CallerFilePath={1}, CallerSourceLineNumber={2}", memberName, sourceFilePath, sourceLineNumber));
            }

            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName", string.Format("Argument is null. CallerMemberName={0}, CallerFilePath={1}, CallerSourceLineNumber={2}", memberName, sourceFilePath, sourceLineNumber));
            }

            if (string.IsNullOrWhiteSpace(obj))
            {
                throw new ArgumentException(string.Format("Argument exception. CallerMemberName={0}, CallerFilePath={1}, CallerSourceLineNumber={2}", memberName, sourceFilePath, sourceLineNumber), parameterName);
            }

            return obj;
        }
    }
}
