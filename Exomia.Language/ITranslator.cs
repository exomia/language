#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Language
{
    /// <summary>
    ///     Interface for translator.
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        ///     get or set the current language.
        /// </summary>
        /// <value>
        ///     The current language.
        /// </value>
        string CurrentLanguage { get; set; }

        /// <summary>
        ///     load a translation file.
        /// </summary>
        /// <param name="fileName"> fileName. </param>
        void Load(string fileName);

        /// <summary>
        ///     format a language string.
        /// </summary>
        /// <param name="format"> language string. </param>
        /// <param name="args">   arguments. </param>
        /// <returns>
        ///     a formatted language string.
        /// </returns>
        string Format(string format, params object[] args);
    }
}