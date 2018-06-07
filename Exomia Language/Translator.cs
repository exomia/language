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
using System.Text;
using System.Text.RegularExpressions;

namespace Exomia.Language
{
    /// <inheritdoc />
    public sealed class Translator : ITranslator
    {
        #region Variables

        private const string PHRASE_FILE_EXTENSION = ".phrases";
        private const char ESCAPECHAR = '%';

        private static readonly Regex s_regex_check_valid_phrase_file;
        private static readonly Regex s_regex_get_full_line;
        private static readonly Regex s_regex_get_category_name;
        private static readonly Regex s_regex_get_phrase_information;
        private static readonly Regex s_regex_get_phrase_format;
        private readonly Dictionary<string, Category> _categories;
        private readonly string _translationDirectory;
        private string _currentLangauge;

        #endregion

        #region Properties

        /// <inheritdoc />
        public string CurrentLanguage
        {
            get { return _currentLangauge; }
            set
            {
                _currentLangauge = value;
                _categories.Clear();
            }
        }

        #endregion

        #region Constructors

        static Translator()
        {
            s_regex_check_valid_phrase_file = new Regex(
                "^\"[Pp]hrases\"\\s*{(.*)}$",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
            s_regex_get_full_line = new Regex(
                "^\\s*(.+)$",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            s_regex_get_category_name = new Regex(
                "\"(.+)\"",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
            s_regex_get_phrase_information = new Regex(
                "\"([a-z#]+)\"\\s+\"(.*)\"",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
            s_regex_get_phrase_format = new Regex(
                "{(([0-9]{0,2})(:[a-zA-Z0-9 ]+)?)}",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
        }

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="translationDirectory">translationDirectory</param>
        /// <param name="language">language</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Translator(string translationDirectory, string language = "en")
        {
            _categories = new Dictionary<string, Category>(16);
            _translationDirectory = translationDirectory;
            _currentLangauge = language ?? throw new ArgumentNullException(nameof(language));
        }

        #endregion

        #region Methods

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

                            if (phraseInfoMatch.Groups[1].Value.Equals(_currentLangauge))
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
            int argc = 0;
            int index = 0;
            int start = 0;
            while ((index = format.IndexOf(ESCAPECHAR, index)) != -1)
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
                    case ESCAPECHAR:
                        {
                            result.Append(ESCAPECHAR);
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

        private string Format(string phrase, string[] format, object[] args, ref int argc)
        {
            StringBuilder result = new StringBuilder(phrase, 255);
            for (int i = 0; i < format.Length; i++)
            {
                string[] pFormat = format[i].Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                string sel = "{" + format[i] + "}";

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

        #endregion
    }
}