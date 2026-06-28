using System.ComponentModel.DataAnnotations;
using WebApplication4.Domain.Models;
namespace WebApplication4.Application.Patient_Component.Patient
{
    public class PatientFeedbackDto
    {
        [Required(ErrorMessage = "اسم المريض مطلوب")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "الاسم لازم يكون بين 3 و 100 حرف")]
        public string PatientName { get; set; }

        [Required(ErrorMessage = "رقم الموبايل مطلوب")]
        [RegularExpression(@"^01[0-9]{9}$",
            ErrorMessage = "رقم الموبايل لازم يكون رقم مصري صحيح مكون من 11 رقم ويبدأ بـ 01")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "العنوان لازم يكون بين 5 و 200 حرف")]
        public string Address { get; set; }

        [StringLength(500, ErrorMessage = "الملاحظات بحد أقصى 500 حرف")]
        public string Notes { get; set; }

        public FeedbackSentiment feedbackSentiment { get; set; }
    }
}
