#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Exomia.Language
{
    /// <summary>
    ///     A translator. This class cannot be inherited.
    /// </summary>
    public sealed class Translator : ITranslator
    {
        /// <summary>
        ///     The phrase file extension.
        /// </summary>
        private const string PHRASE_FILE_EXTENSION = ".phrases";

        /// <summary>
        ///     The escape character.
        /// </summary>
        private const char ESCAPE_CHAR = '%';

        /// <summary>
        ///     The regular expression check valid phrase file.
        /// </summary>
        private static readonly Regex s_regex_check_valid_phrase_file;

        /// <summary>
        ///     The regular expression get full line.
        /// </summary>
        private static readonly Regex s_regex_get_full_line;

        /// <summary>
        ///     Name of the regular expression get category.
        /// </summary>
        private static readonly Regex s_regex_get_category_name;

        /// <summary>
        ///     Information describing the regular expression get phrase.
        /// </summary>
        private static readonly Regex s_regex_get_phrase_information;

        /// <summary>
        ///     The regular expression get phrase format.
        /// </summary>
        private static readonly Regex s_regex_get_phrase_format;

        /// <summary>
        ///     The categories.
        /// </summary>
        private readonly Dictionary<string, Category> _categories;

        /// <summary>
        ///     Pathname of the translation directory.
        /// </summary>
        private readonly string _translationDirectory;

        /// <summary>
        ///     The current language.
        /// </summary>
        private string _currentLanguage;

        /// <inheritdoc />
        public string CurrentLanguage
        {
            get { return _currentLanguage; }
            set
            {
                _currentLanguage = value;
                _categories.Clear();
            }
        }

        /// <summary>
        ///     Initializes static members of the <see cref="Translator" /> class.
        /// </summary>
        static Translator()
        {
            s_regex_check_valid_phrase_file = new Regex(
                "^\"[Pp]hrases\"\\s*{(.*)}$",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);
            s_regex_get_full_line = new Regex(
                "^\\s*(.+)$",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.Compiled);
            s_regex_get_category_name = new Regex(
                "\"(.+)\"",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);
            s_regex_get_phrase_information = new Regex(
                "\"([a-z#]+)\"\\s+\"(.*)\"",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);
            s_regex_get_phrase_format = new Regex(
                "{(([0-9]{0,2})(:[a-zA-Z0-9 ]+)?)}",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);
        }

        /// <summary>
        ///     constructor.
        /// </summary>
        /// <param name="translationDirectory"> translationDirectory. </param>
        /// <param name="language">             (Optional) language. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public Translator(string translationDirectory, string language = "en")
        {
            _categories           = new Dictionary<string, Category>(16);
            _translationDirectory = translationDirectory;
            _currentLanguage      = language ?? throw new ArgumentNullException(nameof(language));
        }

        /// <inheritdoc />
        public void Load(string fileName)
        {
            if (!Path.HasExtension(fileName))
            {
                fileName += PHRASE_FILE_EXTENSION;
            }
            string assetName = Path.Combine(_translationDirectory, fileName);
            if (!File.Exists(assetName))
            {
                throw new FileNotFoundException("File not found in " + _translationDirectory, fileName);
            }

            string fileContent;
            using (StreamReader sr = new StreamReader(assetName))
            {
                fileContent = sr.ReadToEnd();
            }

            if (fileContent.Length > 0)
            {
                Match cMatch = s_regex_check_valid_phrase_file.Match(fileContent);
                if (!cMatch.Success)
                {
                    throw new FormatException("Invalid phrase file format");
                }
                if (cMatch.Groups.Count != 2)
                {
                    return;
                }

                string content = cMatch.Groups[1].Value;

                Match match = s_regex_get_full_line.Match(content);
                while (match.Success)
                {
                    if (match.Groups.Count == 2)
                    {
                        Category category = new Category();

                        Match categoryMatch = s_regex_get_category_name.Match(match.Groups[1].Value);
                        if (!categoryMatch.Success)
                        {
                            throw new FormatException("Invalid phrase file format");
                        }

                        string categoryName = categoryMatch.Groups[1].Value;

                        match = match.NextMatch();

                        if (match.Success)
                        {
                            if (!match.Groups[1].Value.StartsWith("{"))
                            {
                                throw new FormatException("Invalid phrase file format");
                            }
                        }

                        match = match.NextMatch();

                        while (match.Success)
                        {
                            if (match.Groups[1].Value.StartsWith("}"))
                            {
                                break;
                            }

                            Match phraseInfoMatch = s_regex_get_phrase_information.Match(match.Groups[1].Value);
                            if (!phraseInfoMatch.Success || phraseInfoMatch.Groups.Count != 3)
                            {
                                throw new FormatException("Invalid phrase file format");
                            }

                            if (phraseInfoMatch.Groups[1].Value.Equals(_currentLanguage))
                            {
                                category.Phrase = phraseInfoMatch.Groups[2].Value;
                                MatchCollection collection =
                                    s_regex_get_phrase_format.Matches(phraseInfoMatch.Groups[2].Value);
                                if (collection.Count > 0)
                                {
                                    string[] pFormats = new string[collection.Count];
                                    foreach (Match phraseFormatMatch in collection)
                                    {
                                        if (!int.TryParse(phraseFormatMatch.Groups[2].Value, out int index))
                                        {
                                            throw new FormatException("Invalid phrase file format");
                                        }
                                        pFormats[index] = phraseFormatMatch.Groups[1].Value;
                                    }
                                    category.Format = pFormats;
                                }
                            }

                            match = match.NextMatch();
                        }

                        if (!_categories.ContainsKey(categoryName))
                        {
                            _categories.Add(categoryName, category);
                        }
                    }
                    match = match.NextMatch();
                }
            }
        }

        /// <inheritdoc />
        public string Format(string format, params object[] args)
        {
            StringBuilder result = new StringBuilder(255);
            int           argc   = 0;
            int           index  = 0;
            int           start  = 0;
            while ((index = format.IndexOf(ESCAPE_CHAR, index)) != -1)
            {
                if (argc >= args.Length) { throw new IndexOutOfRangeException("arg out of range"); }
                if (index - start > 0)
                {
                    result.Append(format.Substring(start, index - start));
                }
                start = index + 2;
                char c = format[index + 1];
                index += 2;
                switch (c)
                {
                    case 't':
                    case 'T':
                        {
                            string translationKey = args[argc++].ToString();
                            if (!_categories.TryGetValue(translationKey, out Category category))
                            {
                                throw new KeyNotFoundException(translationKey);
                            }
                            string phrase = category.Phrase;
                            if (category.Format != null && category.Format.Length > 0)
                            {
                                phrase = Format(phrase, category.Format, args, ref argc);
                            }
                            result.Append(phrase);
                        }
                        break;
                    case 's':
                    case 'S':
                    case 'n':
                    case 'N':
                        {
                            result.Append(args[argc++]);
                        }
                        break;
                    case ESCAPE_CHAR:
                        {
                            result.Append(ESCAPE_CHAR);
                        }
                        break;
                    default:
                        {
                            throw new FormatException($"Invalid Format '{c}'");
                        }
                }
            }
            if (format.Length - start > 0)
            {
                result.Append(format.Substring(start, format.Length - start));
            }
            return result.ToString();
        }

        /// <summary>
        ///     Formats.
        /// </summary>
        /// <param name="phrase"> The phrase. </param>
        /// <param name="format"> Describes the format to use. </param>
        /// <param name="args">   The arguments. </param>
        /// <param name="argc">   [in,out] The argc. </param>
        /// <returns>
        ///     The formatted value.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        ///     Thrown when a Key Not Found error condition
        ///     occurs.
        /// </exception>
        /// <exception cref="FormatException">             Thrown when the format of the ? is incorrect. </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when one or more arguments are outside
        ///     the required range.
        /// </exception>
        private string Format(string phrase, string[] format, object[] args, ref int argc)
        {
            StringBuilder result = new StringBuilder(phrase, 255);
            for (int i = 0; i < format.Length; i++)
            {
                string[] pFormat = format[i].Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                string   sel     = "{" + format[i] + "}";

                switch (pFormat.Length)
                {
                    case 1:
                        {
                            result = result.Replace(sel, args[argc++].ToString());
                        }
                        break;
                    case 2:
                        {
                            if (pFormat[1].Length == 1)
                            {
                                switch (pFormat[1][0])
                                {
                                    case 't':
                                    case 'T':
                                        {
                                            string translationKey = args[argc++].ToString();
                                            if (!_categories.TryGetValue(translationKey, out Category category))
                                            {
                                                throw new KeyNotFoundException(translationKey);
                                            }
                                            string phrase1 = category.Phrase;
                                            if (category.Format != null && category.Format.Length > 0)
                                            {
                                                phrase1 = Format(phrase, category.Format, args, ref argc);
                                            }
                                            result = result.Replace(sel, phrase1);
                                            break;
                                        }
                                    default:
                                        {
                                            throw new FormatException($"Invalid Format '{pFormat[1][0]}'");
                                        }
                                }
                            }
                            else
                            {
                                if (!_categories.TryGetValue(pFormat[1], out Category category))
                                {
                                    throw new KeyNotFoundException(pFormat[1]);
                                }
                                string phrase1 = category.Phrase;
                                if (category.Format != null && category.Format.Length > 0)
                                {
                                    phrase1 = Format(phrase1, category.Format, args, ref argc);
                                }
                                result = result.Replace(sel, phrase1);
                            }
                        }
                        break;
                    default:
                        {
                            throw new ArgumentOutOfRangeException(nameof(format), "invalid format: " + format[i]);
                        }
                }
            }
            return result.ToString();
        }
    }
}