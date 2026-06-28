namespace WebApplication4.Application.ChatAi_Component.IService
{
    public interface IPharmacistClinicalAssistantService
    {
        Task<string> Ask(string question);
    }
}
