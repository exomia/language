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
    ///     A category.
    /// </summary>
    class Category
    {
        /// <summary>
        ///     Gets or sets the format to use.
        /// </summary>
        /// <value>
        ///     The format.
        /// </value>
        public string[] Format { get; set; } = null;

        /// <summary>
        ///     Gets or sets the phrase.
        /// </summary>
        /// <value>
        ///     The phrase.
        /// </value>
        public string Phrase { get; set; } = string.Empty;
    }
}