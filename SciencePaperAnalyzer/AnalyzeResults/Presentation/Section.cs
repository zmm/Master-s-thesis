﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnalyzeResults.Presentation
{
    [Serializable]
    public class Section
    {
        public Section()
        {
            Sentences = new List<Sentence>();
            References = new List<Reference>();
            Id = Guid.NewGuid();
        }

        public Section(IEnumerable<Sentence> sentences)
        {
            Sentences.AddRange(sentences);
        }

        [BsonId]
        public Guid Id { get; }

        [BsonElement("type")]
        public SectionType Type { get; set; }

        [BsonElement("sentences")]
        public List<Sentence> Sentences { get; set; }

        [BsonElement("references")]
        public List<Reference> References { get; set; }

        [BsonElement("haserrors")]
        public bool HasErrors { get; set; }

        public string ToStringVersion()
        {
            switch (Type)
            {
                case SectionType.PaperTitle:
                case SectionType.SectionTitle:
                    return Sentences[0].ToStringVersion();
                case SectionType.Text:
                    return string.Join(" ", Sentences.Select(x => x.ToStringVersion()));
                case SectionType.ReferencesList:
                    return string.Join("\n", References.Select(x => x.Original.Original));
                default:
                    return "_";
            }
        }

        public string ToHtmlVersion()
        {
            switch (Type)
            {
                case SectionType.PaperTitle:
                    return $"<p style =\"font-weight: bold; font-size: 20px\">{Sentences[0].ToStringVersion()}</p>";
                case SectionType.SectionTitle:
                    return $"<p style =\"font-weight: bold; font-size: 16px\">{Sentences[0].ToStringVersion()}</p>";
                case SectionType.Text:
                    return $"<p style =\"font-size: 14px\">{string.Join(" ", Sentences.Select(x => x.ToStringVersion()))}</p>";
                case SectionType.ReferencesList:
                    var sb = new StringBuilder();
                    foreach (var reference in References)
                    {
                        string referedToString, referedToStyle, oldSource;
                        referedToString = reference.ReferedTo ? "Есть ссылка в статье" : "Нет ссылки в статье";
                        referedToStyle = reference.ReferedTo ? "style=\"color: green;\"" : "style=\"color: red;\"";
                        oldSource = reference.Year != 0 && reference.Year < 1990 ? "<span style=\"color: red;\">Устаревший источник</span>" : "";


                        sb.Append($"<span>{reference.Original.Original}</span> <span {referedToStyle}>{referedToString}</span> {oldSource}\n");
                    }
                    return $"<p style =\"font-size: 14px\">{sb.ToString()}</p>";
                default:
                    return "Что то не так";
            }
        }
    }
}
