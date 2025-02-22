﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalyzeResults.Presentation;
using AnalyzeResults.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaperAnalyzer;
using PaperAnalyzer.Service;
using TestWebApp.Models;
using WebPaperAnalyzer.DAL;
using WebPaperAnalyzer.Models;
using WebPaperAnalyzer.Services;
using WebPaperAnalyzer.ViewModels;

namespace WebPaperAnalyzer.Controllers
{
    [System.Runtime.InteropServices.Guid("AC77F42B-4207-4468-A583-0999046DBAFD")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationContext _context;
        private IViewRenderService _viewRenderService;

        private readonly IPaperAnalyzerService _analyzeService;
        protected IResultRepository Repository;
        protected MongoSettings MongoSettings { get; set; }
        public HomeController(
            ILogger<HomeController> logger,
            IPaperAnalyzerService analyzeService,
            IResultRepository repository,
            IOptions<MongoSettings> mongoSettings = null,
            IViewRenderService viewRenderService = null)
        {
            _viewRenderService = viewRenderService;
            MongoSettings = mongoSettings?.Value;
            Repository = repository;
            _context = new ApplicationContext(MongoSettings);
            _logger = logger;
            _analyzeService = analyzeService;
        }

        private async Task ProcessFile(string resultId, IFormFile file, string titles, string paperName, string refsName,
                                                    string criterionName = null, string keywords = "")
        {
            _logger.LogInformation($"Received request UploadFile with criterionName {criterionName}");
            if (file == null)
            {
                throw new Exception("File not uploaded");
                //return new PartialViewResult();
            }

            var uploadFile = new UploadFile
            {
                FileName = file.FileName,
                Length = file.Length,
                DataStream = new MemoryStream()
            };

            await file.CopyToAsync(uploadFile.DataStream);

            ResultCriterion criterion = null;

            try
            {
                var criteria = await _context.GetCriteria();
                criterion = criteria.FirstOrDefault(c => c.Name == criterionName);
            }
            catch (Exception)
            {
                //Возможно только во время выполнения теста
            }

            ResultScoreSettings settings;

            if (criterion != null)
            {
                settings = CriteriaMapper.GetAnalyzeCriteria(criterion);
                if (criterion.ForbiddenWordDictionary != null)
                {
                    _logger.LogInformation($"Upload forbiddenwords dictionary: {string.Join(",", criterion.ForbiddenWordDictionary)}");
                    settings.ForbiddenWords = await GetForbiddenWords(criterion.ForbiddenWordDictionary);
                    _logger.LogInformation($"Upload forbiddenwords dictionary: {string.Join(",", criterion.ForbiddenWordDictionary)}");
                }
                else
                {
                    _logger.LogInformation("No dictionaries uploaded");
                    settings.ForbiddenWords = new List<ForbiddenWords>();
                }
            }
            else
            {
                //Возможно только во время выполнения теста
                settings = new ResultScoreSettings()
                {
                    WaterCriteria = new BoundedCriteria
                    {
                        Weight = 35,
                        LowerBound = 14,
                        UpperBound = 20
                    },
                    KeyWordsCriteria = new BoundedCriteria
                    {
                        Weight = 30,
                        LowerBound = 6,
                        UpperBound = 14,
                    },
                    Zipf = new BoundedCriteria
                    {
                        Weight = 30,
                        LowerBound = 5.5,
                        UpperBound = 9.5,
                    },
                    KeywordsMentioning = new BoundedCriteria
                    {
                        Weight = 5,
                        LowerBound = 0,
                        UpperBound = 1,
                    },
                    UseOfPersonalPronounsCost = 0,
                    UseOfPersonalPronounsErrorCost = 0,
                    SourceNotReferencedCost = 0,
                    SourceNotReferencedErrorCost = 0,
                    ShortSectionCost = 0,
                    ShortSectionErrorCost = 0,
                    PictureNotReferencedCost = 0,
                    PictureNotReferencedErrorCost = 0,
                    TableNotReferencedCost = 0,
                    TableNotReferencedErrorCost = 0,
                    ForbiddenWords = new List<ForbiddenWords>()
                };
            }

            PaperAnalysisResult result=null;
            try
            {
                _logger.LogInformation($"Settings have {settings.ForbiddenWords.Count(x => true)} dictionary");
                result = _analyzeService.GetAnalyze(uploadFile, titles, paperName, refsName, keywords, settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                result = new PaperAnalysisResult(new List<Section>(), new List<Criterion>(),
                    new List<AnalyzeResults.Errors.Error>(), 0)
                { Error = ex.Message };
                throw;// Error(ex.Message);
            }
            finally
            {
                AnalysisResult analysisResult;
                if (criterion != null)
                {
                    analysisResult = new AnalysisResult
                    {
                        Id = resultId,
                        Result = result,
                        StudentLogin = User.Identity.Name,
                        TeacherLogin = criterion.TeacherLogin,
                        Criterion = criterion.Name
                    };
                }
                else
                {
                    analysisResult = new AnalysisResult
                    {
                        Id = resultId,
                        Result = result,
                        StudentLogin = null,
                        TeacherLogin = null,
                        Criterion = null
                    };
                }

                _logger.LogDebug($"Result saved by Id {analysisResult.Id}");
                Repository.AddResult(analysisResult);
            }

        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, string titles, string paperName, string refsName,
                                                    string criterionName = null, string keywords = "")
        {
            try
            {
                string id = Guid.NewGuid().ToString();
                await ProcessFile(id, file, titles, paperName, refsName, criterionName, keywords);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CurlUploadFile(IFormFile file, IFormFile paperName, string criteriaName)
        {
            Stream stream = paperName.OpenReadStream();
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);
            Console.WriteLine(criteriaName);
            string id = Guid.NewGuid().ToString();
            Response.OnCompleted(async () =>
            {
                try
                {
                    await ProcessFile(id, file, "", Encoding.UTF8.GetString(buffer), "", criteriaName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.Message);
                }
            });
            return Ok(id);
        }

        [HttpGet]
        public IActionResult Result(string id)
        {
            _logger.LogDebug($"Try to show result by ID: {id}");
            AnalysisResult analysisResult = Repository.GetResult(id);
            if (analysisResult != null)
            {
                return View(analysisResult.Result);
            }
            else
            {
                PaperAnalysisResult result = new PaperAnalysisResult(new List<Section>(), new List<Criterion>(),
                                                                new List<AnalyzeResults.Errors.Error>(), 0)
                { Error = "Your work is being processed..." };
                return View(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Badge(string id)
        {
            AnalysisResult analysisResult = Repository.GetResult(id);
            if (analysisResult != null)
            {
                string criterion = string.IsNullOrEmpty(analysisResult.Criterion) ? "" : analysisResult.Criterion;
                int score = Convert.ToInt32(analysisResult.Result.GetPaperGrade());
                int maxScore = Convert.ToInt32(analysisResult.Result.MaxScore);
                var result = await _viewRenderService.RenderToStringAsync("Home/Badge", new BadgeModel()
                {
                    Criterion = criterion,
                    Score = score,
                    MaxScore = maxScore,
                    IsProcessing = false
                });
                return Content(result, "image/svg+xml", Encoding.UTF8);
            }
            else
            {
                var result = await _viewRenderService.RenderToStringAsync("Home/Badge", new BadgeModel()
                {
                    Criterion = "",
                    Score = -1,
                    MaxScore = -1,
                    IsProcessing = true
                });
                return Content(result, "image/svg+xml", Encoding.UTF8);
            }
        }
        [HttpGet]
        [Route("Home/Result/{id}/short")]
        [Route("Home/ShortResult/{id}")]
        public IActionResult ShortResult(string id)
        {
            return Content(Repository.GetResult(id)?.Result.GetShortSummary());
        }

        public async Task<IActionResult> Index()
        {
            var criteria = await _context.GetCriteria();
            SelectList criteriaList = new SelectList(criteria.ToList().Select(c => c.Name));
            ViewBag.Criteria = criteriaList;
            return View();
        }

        [HttpGet]
        public IActionResult PreviousResults()
        {
            return View(Repository.GetResultsByLogin(User.Identity.Name, false));
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier,
                Message = message
            });
        }

        private async Task<IEnumerable<ForbiddenWords>> GetForbiddenWords(IEnumerable<string> forbiddenDictNames)
        {
            var res = new List<ForbiddenWords>();
            foreach (var dict in forbiddenDictNames)
            {
                _logger.LogInformation($"Try to upload dictionary with name: {dict}");
                var item = await _context.GetDictionary(dict);
                res.Add(item);
                _logger.LogInformation($"Added forbidden words: {string.Join(",", item.Words)}");
            }
            return res;
        }
    }
}
