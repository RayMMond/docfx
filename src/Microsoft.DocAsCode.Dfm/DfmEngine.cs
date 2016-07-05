// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.Dfm
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    using Microsoft.DocAsCode.Common;
    using Microsoft.DocAsCode.MarkdownLite;
    using Microsoft.DocAsCode.Utility;

    public class DfmEngine : MarkdownEngine
    {
        public DfmEngine(IMarkdownContext context, IMarkdownTokenRewriter rewriter, object renderer, Options options)
            : base(context, rewriter, renderer, options, new Dictionary<string, LinkObj>())
        {
        }

        public override string Markup(string src, string path)
        {
            return Markup(src, path, null);
        }

        public string Markup(string src, string path, HashSet<string> dependency)
        {
            if (string.IsNullOrEmpty(src))
            {
                return string.Empty;
            }
            return InternalMarkup(src, ImmutableStack.Create(path), dependency ?? new HashSet<string>());
        }

        internal string InternalMarkup(string src, ImmutableStack<string> parents, HashSet<string> dependency)
        {
            using (GetFileScope(parents))
            {
                return InternalMarkup(
                    src,
                    Context
                        .SetFilePathStack(parents)
                        .SetDependency(dependency));
            }
        }

        private static LoggerFileScope GetFileScope(ImmutableStack<string> parents)
        {
            if (!parents.IsEmpty)
            {
                var path = parents.Peek().ToDisplayPath();

                if (!string.IsNullOrEmpty(path))
                {
                    return new LoggerFileScope(path);
                }
            }

            return null;
        }

        internal string InternalMarkup(string src, IMarkdownContext context) =>
            Mark(
                SourceInfo.Create(
                    Normalize(src),
                    context.GetFilePathStack().Peek()
                ),
                context
            ).ToString();
    }
}
