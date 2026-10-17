using Microsoft.AspNetCore.Mvc;
using WebApplication4.Application.Modules.ChatAi.Dto;
using WebApplication4.Application.Modules.ChatAi.Interfaces;

public class ChatbotController : Controller
{
    private readonly IPharmacistClinicalAssistantService _chatAiService;

    public ChatbotController(IPharmacistClinicalAssistantService chatAiService)
    {
        _chatAiService = chatAiService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] PharmacistQueryDto request)
    {
        if (string.IsNullOrEmpty(request?.Question))
        {
            return BadRequest(new { answer = "الرجاء إدخال سؤال." });
        }

        var answer = await _chatAiService.Ask(request.Question);

        return Json(new { answer });
    }
}