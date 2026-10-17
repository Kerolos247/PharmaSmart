namespace WebApplication4.Application.Modules.ChatAi.Interfaces
{

    public interface IPharmasmartAiService
    {
        Task<string> StreamChatAsync(string userMessage);
        Task<string> StreamVoiceAsync(Stream audioStream, string fileName);
    }
}
