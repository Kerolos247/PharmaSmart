namespace WebApplication4.Application.Modules.ChatAi.Interfaces
{
    public interface IPharmacistClinicalAssistantService
    {
        Task<string> Ask(string question);
    }
}
