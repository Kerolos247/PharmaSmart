namespace WebApplication4.Application.ChatAi_Component.IService
{

    public interface IPharmasmartAiService
    {
        Task<string> StreamChatAsync(string userMessage);
        Task<string> StreamVoiceAsync(Stream audioStream, string fileName);
    }
}
