using System;
using System.Collections.Generic;
using System.IO;

namespace Exomia.Language
{
    /// <summary>
    ///     ITranslator interface
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        ///     get or set the current language
        /// </summary>
        string CurrentLanguage { get; set; }

        /// <summary>
        ///     load a translation file
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="FormatException"></exception>
        void Load(string fileName);

        /// <summary>
        ///     format a language string
        /// </summary>
        /// <param name="format">language string</param>
        /// <param name="args">arguments</param>
        /// <returns>a formated language string</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="FormatException"></exception>
        string Format(string format, params object[] args);
    }
}