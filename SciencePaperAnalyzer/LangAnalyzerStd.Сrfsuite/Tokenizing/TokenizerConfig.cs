﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using LangAnalyzerStd.Core;
using LangAnalyzerStd.Ner;
using LangAnalyzerStd.Postagger;
using LangAnalyzerStd.SentenceSplitter;
using LangAnalyzerStd.Urls;

namespace LangAnalyzerStd.Tokenizing
{
    [Flags]
    public enum TokenizeMode
    {
        __UNKNOWN__ = 0x0,

        PosTagger = 0x1,
        Ner = (1 << 1),
    }

    public sealed class TokenizerModel
    {
        public TokenizerModel(string tokenizerResourcesXmlFilename)
        {
            var xdoc = XDocument.Load(tokenizerResourcesXmlFilename);

            const string PARTICLE_THAT = "ТО";
            var hyphenChars = Xlat.CHARTYPE_MAP
                .Select((ct, c) => Tuple.Create(ct, (char)c))
                .Where(t => (t.Item1 & CharType.IsHyphen) == CharType.IsHyphen)
                .Select(t => t.Item2);
            var particleThats = hyphenChars.Select(c => c + PARTICLE_THAT).ToArray();
            var particleThatLength = (PARTICLE_THAT.Length + 1);
            var particleThatExclusion = from xe in xdoc.Root.Element("particle-that-exclusion-list").Elements()
                                        let _v = xe.Value.ToUpperInvariant().Trim()
                                        where particleThats.Any(_pt => _v.EndsWith(_pt))
                                        let v = _v.Substring(0, _v.Length - particleThatLength).Trim()
                                        where !string.IsNullOrEmpty(v)
                                        from pt in particleThats
                                        select (v + pt);
            ParticleThatExclusion = new HashSet<string>(particleThatExclusion);
        }

        public HashSet<string> ParticleThatExclusion
        {
            get;
            private set;
        }
    }

    public sealed class TokenizerConfig
    {
        public TokenizerConfig(string tokenizerResourcesXmlFilename)
        {
            Model = new TokenizerModel(tokenizerResourcesXmlFilename);
        }

        public SentSplitterConfig SentSplitterConfig
        {
            get;
            set;
        }
        public TokenizerModel Model
        {
            get;
            set;
        }

        public TokenizeMode TokenizeMode
        {
            get;
            set;
        }
        public LanguageTypeEnum LanguageType
        {
            get;
            set;
        }

        public IPosTaggerInputTypeProcessorFactory PosTaggerInputTypeProcessorFactory
        {
            get;
            set;
        }
        public INerInputTypeProcessorFactory NerInputTypeProcessorFactory
        {
            get;
            set;
        }
    }

    public sealed class TokenizerConfig4NerModelBuilder
    {
        public TokenizerConfig4NerModelBuilder(string tokenizerResourcesXmlFilename)
        {
            Model = new TokenizerModel(tokenizerResourcesXmlFilename);
        }

        public UrlDetectorConfig UrlDetectorConfig
        {
            get;
            set;
        }
        public TokenizerModel Model
        {
            get;
            private set;
        }

        public LanguageTypeEnum LanguageType
        {
            get;
            set;
        }

        public INerInputTypeProcessorFactory NerInputTypeProcessorFactory
        {
            get;
            set;
        }
    }
}
