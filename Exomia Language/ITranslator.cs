#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

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
        #region Properties

        /// <summary>
        ///     get or set the current language
        /// </summary>
        string CurrentLanguage { get; set; }

        #endregion

        #region Methods

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

        #endregion
    }
}