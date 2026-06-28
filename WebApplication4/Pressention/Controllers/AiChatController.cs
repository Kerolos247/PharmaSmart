using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using WebApplication4.Application.ChatAi_Component.Dto;
using WebApplication4.Application.ChatAi_Component.IService;

namespace WebApplication4.Presentation.Controllers 
{
    public class AiChatController : Controller
    {
        private readonly IPharmasmartAiService _aiService;

        public AiChatController(IPharmasmartAiService aiService)
        {
            _aiService = aiService;
        }

       
        [HttpPost]
        public async Task<IActionResult> SendChat([FromBody] PatientInquiryDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { success = false, reply = "المحتوى فارغ!" });

            try
            {
               
                var responseText = await _aiService.StreamChatAsync(request.Message);
                return Json(new { success = true, reply = responseText });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { success = false, reply = "حصلت مشكلة في معالجة طلبك." });
            }
        }

      
        [HttpPost]
        public async Task<IActionResult> SendVoice(IFormFile audioFile)
        {
            if (audioFile == null || audioFile.Length == 0)
                return BadRequest(new { success = false, reply = "لم يتم استقبال ملف صوتي صحيح." }); 

            try
            {
                using var stream = audioFile.OpenReadStream();

              
                var responseText = await _aiService.StreamVoiceAsync(stream, audioFile.FileName);
                return Json(new { success = true, reply = responseText });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, reply = "حصلت مشكلة في معالجة الصوت." });
            }
        }
    }
}