﻿using MediaBrowser.Naming.Common;
using MediaBrowser.Naming.IO;
using MediaBrowser.Naming.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MediaBrowser.Naming.Video
{
    public class StackResolver
    {
        private readonly NamingOptions _options;
        private readonly ILogger _logger;
        private readonly IRegexProvider _iRegexProvider;

        public StackResolver(NamingOptions options, ILogger logger)
            : this(options, logger, new RegexProvider())
        {
        }

        public StackResolver(NamingOptions options, ILogger logger, IRegexProvider iRegexProvider)
        {
            _options = options;
            _logger = logger;
            _iRegexProvider = iRegexProvider;
        }

        public StackResult ResolveDirectories(IEnumerable<string> files)
        {
            return Resolve(files.Select(i => new PortableFileInfo
            {
                FullName = i,
                Type = FileInfoType.Directory
            }));
        }

        public StackResult ResolveFiles(IEnumerable<string> files)
        {
            return Resolve(files.Select(i => new PortableFileInfo
            {
                FullName = i,
                Type = FileInfoType.File
            }));
        }

        public StackResult Resolve(IEnumerable<PortableFileInfo> files)
        {
            var result = new StackResult();

            var resolver = new VideoResolver(_options, _logger);

            var list = files
                .Where(i => i.Type == FileInfoType.Directory || (resolver.IsVideoFile(i.FullName) || resolver.IsStubFile(i.FullName)))
                .OrderBy(i => i.FullName)
                .ToList();

            var expressions = _options.VideoFileStackingExpressions;

            for (var i = 0; i < list.Count; i++)
            {
                var extraFiles = new List<string>();
                var offset = 0;

                var file1 = list[i];

                var expressionIndex = 0;
                while (expressionIndex < expressions.Count)
                {
                    var exp = expressions[expressionIndex];
                    var stack = new FileStack
                    {
                        Expression = exp
                    };

                    // (Title)(Volume)(Ignore)(Extension)
                    var match1 = FindMatch(file1, exp, offset);

                    if (match1.Success)
                    {
                        var title1 = match1.Groups[1].Value;
                        var volume1 = match1.Groups[2].Value;
                        var ignore1 = match1.Groups[3].Value;
                        var extension1 = match1.Groups[4].Value;

                        var j = i + 1;
                        while (j < list.Count)
                        {
                            var file2 = list[j];

                            // (Title)(Volume)(Ignore)(Extension)
                            var match2 = FindMatch(file2, exp, offset);

                            if (match2.Success)
                            {
                                var title2 = match2.Groups[1].Value;
                                var volume2 = match2.Groups[2].Value;
                                var ignore2 = match2.Groups[3].Value;
                                var extension2 = match2.Groups[4].Value;

                                if (string.Equals(title1, title2, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (!string.Equals(volume1, volume2, StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (string.Equals(ignore1, ignore2, StringComparison.OrdinalIgnoreCase) &&
                                            string.Equals(extension1, extension2, StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (stack.Files.Count == 0)
                                            {
                                                stack.Name = title1 + ignore1;
                                                //stack.Name = title1 + ignore1 + extension1;
                                                stack.Files.Add(file1.FullName);
                                            }
                                            stack.Files.Add(file2.FullName);
                                        }
                                        else 
                                        {
                                            // Sequel
                                            offset = 0;
                                            expressionIndex++;
                                            break;
                                        }
                                    }
                                    else if (!string.Equals(ignore1, ignore2, StringComparison.OrdinalIgnoreCase))
                                    {
                                        // False positive, try again with offset
                                        offset = match1.Groups[3].Index;
                                        break;
                                    }
                                    else
                                    {
                                        // Extension mismatch
                                        offset = 0;
                                        expressionIndex++;
                                        break;
                                    }
                                }
                                else
                                {
                                    // Title mismatch
                                    offset = 0;
                                    expressionIndex++;
                                    break;
                                }
                            }
                            else
                            {
                                // No match 2, next expression
                                offset = 0;
                                expressionIndex++;
                                break;
                            }

                            j++;
                        }

                        if (j == list.Count)
                        {
                            expressionIndex = expressions.Count;
                        }
                    }
                    else
                    {
                        // No match 1
                        offset = 0;
                        expressionIndex++;
                    }

                    if (stack.Files.Count > 1)
                    {
                        result.Stacks.Add(stack);
                        i += stack.Files.Count - 1;
                        break;
                    }
                }
            }

            return result;
        }

        private string GetRegexInput(PortableFileInfo file)
        {
            // For directories, dummy up an extension otherwise the expressions will fail
            var input = file.Type == FileInfoType.File
                ? file.FullName
                : file.FullName + ".mkv";

            return Path.GetFileName(input);
        }

        private Match FindMatch(PortableFileInfo input, string expression, int offset)
        {
            var regexInput = GetRegexInput(input);

            var regex = _iRegexProvider.GetRegex(expression, RegexOptions.IgnoreCase);
            return regex.Match(regexInput, offset);
        }
    }
}
