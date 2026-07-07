using WebApplication4.Application.Patient_Component.Patient;
using WebApplication4.Domain.Enums;
namespace WebApplication4.Domain.Models
{
  

    public class PatientFeedback
    {
        public int Id { get; set; }

        public string PatientName { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; } 

        public string Notes { get; set; }

        public ComplaintClassification ComplaintClassification { get; set; }


        public FeedbackSentiment feedbackSentiment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

