﻿using System;
using System.Collections.Generic;
using System.Linq;
using AnalyzeResults.Errors;
using AnalyzeResults.Settings;
using MongoDB.Bson.Serialization.Attributes;

namespace AnalyzeResults.Presentation
{
    [Serializable]
    public class PaperAnalysisResult
    {
        public PaperAnalysisResult(IEnumerable<Section> sections, IEnumerable<Criterion> criteria, IEnumerable<Error> errors, double maxScore)
        {
            Sections = new List<Section>();
            Sections.AddRange(sections);
            Criteria = new List<Criterion>();
            Criteria.AddRange(criteria);
            Errors = new List<Error>();
            Errors.AddRange(errors);
            Error = "";
            MaxScore = maxScore;
        }

        [BsonElement("sections")]
        public List<Section> Sections { get; set; }

        [BsonElement("criteria")]
        public List<Criterion> Criteria { get; set; }

        [BsonElement("errors")]
        public List<Error> Errors { get; set; }

        [BsonElement("paperTitle")]
        public string PaperTitle { get; set; }
        [BsonElement("paperTitleRefs")]
        public Dictionary<string, List<int>> PaperTitleRefs { get; set; }
        [BsonElement("keywords")]
        public Dictionary<string, List<int>> Keywords { get; set; }

        [BsonElement("error")]
        public string Error { get; set; }

        [BsonElement("maxScore")]
        public double MaxScore { get; set; }

        public bool IsScientific()
        {
            return Criteria.All(x => x.IsMet());
        }

        public double GetPaperGrade()
        {
            var resultScore = Criteria.Where(x => x is Criterion).Select(crit => (crit as Criterion).GetGradePart())
                .Aggregate((result, part) => result + part);
            var weightTmp = MaxScore - Criteria.Where(x => x is Criterion)
                .Sum(crit => (crit as Criterion).Factor);


            foreach (var error in Enum.GetValues(typeof(ErrorType)))
            {
                var specialError = Errors.FirstOrDefault(e => e.ErrorType == ((ErrorType)error));
                if (specialError == null)
                    continue;
                var weight = specialError.Weight;
                weightTmp -= weight;
                resultScore += GetSpecialGrade((ErrorType) error);
            }

            return Math.Round(resultScore) + weightTmp;
        }

        public double GetSpecialGrade(ErrorType type)
        {
            var specialError = Errors.FirstOrDefault(e => e.ErrorType == type);

            if (specialError == null)
                return 0;
            var weight = specialError.Weight;
            if (specialError.ErrorCost < 0)
            {
                return weight;
            }
            var errorCost = specialError.ErrorCost;
            var errorCount = Errors.Count(e => e.ErrorType == type);

            switch (specialError.GradingType)
            {
                case GradingType.ErrorCostSubtraction:
                    return Math.Max(weight - errorCount * errorCost, 0);
                case GradingType.GradingTable:
                    var result = specialError.Grading.OrderByDescending(g => g.Boarder)
                        .FirstOrDefault(g => errorCount >= g.Boarder);
                    if (result == null)
                        return weight;
                    else
                        return Math.Max(weight - result.Value, 0);
            }

            return 0;
        }

        public string GetShortSummary()
        {
            var result = "Проведенные проверки:\n";
            foreach (Criterion check in Criteria.Where(x => (x as Criterion)?.Factor > 0))
            {
                var res = check.IsMet() ? Math.Round(check.Factor, 2) : 0;
                result += $"{check.Name}: Набрано {res} из {Math.Round(check.Factor, 2)} баллов\n";
            }
            foreach(var error in Enum.GetValues(typeof(ErrorType)))
            {
                var specialError = Errors.FirstOrDefault(e => e.ErrorType == (ErrorType)error);
                if (specialError != null && specialError.Weight > 0)
                {
                    var @res = GetSpecialGrade((ErrorType)error);
                    result += $"{specialError.Name} : Набрано {res} из {Math.Round(specialError.Weight, 2)} баллов\n";
                }
            }

            return result;
        }
    }
}
